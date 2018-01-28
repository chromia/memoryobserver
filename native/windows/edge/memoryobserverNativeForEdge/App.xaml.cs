using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using System.Diagnostics;

namespace memoryobserverNativeForEdge
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private bool enable = true;
        private BackgroundTaskDeferral deferralwithEdge;
        private BackgroundTaskDeferral deferralwithApp;
        private AppServiceConnection connectionwithEdge;
        private AppServiceConnection connectionwithApp;
        private ValueSet targetmessage;

        private async void LaunchNativeApp()
        {
            try
            {
                //Launch external app to observe memory( because UWP app can't access to process informations directly ).
                //Make sure the memoryobserverNativeForEdgeApp.exe is in App folder of this project.
                //See also "Post-build event" of memoryobserverNativeForEdgeApp project.
                await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
            catch (Exception)
            {
                //launching failed
                enable = false; //do nothing anymore
            }
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            if (taskInstance.TriggerDetails is AppServiceTriggerDetails)
            {
                AppServiceTriggerDetails appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
                //determine caller (Edge or App)
                if (appService.CallerPackageFamilyName == Windows.ApplicationModel.Package.Current.Id.FamilyName)
                {
                    //[2nd]called from App
                    deferralwithApp = taskInstance.GetDeferral();
                    connectionwithApp = appService.AppServiceConnection;
                    connectionwithApp.ServiceClosed += OnAppConnectionClosed;
                    Debug.WriteLine("established: Connection with App");

                    if (targetmessage != null)
                    {
                        //send initialize parameter
                        await connectionwithApp.SendMessageAsync(targetmessage);
                    }
                }
                else
                {
                    //[1st]called from Edge
                    if (connectionwithEdge == null)
                    {
                        deferralwithEdge = taskInstance.GetDeferral();
                        connectionwithEdge = appService.AppServiceConnection;
                        connectionwithEdge.RequestReceived += OnEdgeRequestReceived;
                        connectionwithEdge.ServiceClosed += OnEdgeConnectionClosed;
                        Debug.WriteLine("established: Connection with Edge");

                        //initialize connection with App
                        LaunchNativeApp();
                    }
                }
            }
        }

        private async void OnEdgeRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //      [here]
            //Edge  ----->  Me  ------ App
            AppServiceDeferral messageDeferral = args.GetDeferral();

            ValueSet resdata = new ValueSet();
            if (enable)
            {
                //                  [here]
                //Edge  ------  Me  <----> App
                try
                {
                    if (connectionwithApp != null)
                    {
                        //Transfer message from Edge to App 
                        //and receive an response message
                        AppServiceResponse responsefromApp = await connectionwithApp.SendMessageAsync(args.Request.Message);
                        resdata.Add(responsefromApp.Message.First());
                        Debug.WriteLine("message through");
                    }
                    else
                    {
                        //App is still not ready
                        resdata.Add("Response", "{ \"result\": false }");
                        Debug.WriteLine("message not ready");

                        //Store "settarget" message
                        string message = args.Request.Message.First().Value.ToString();
                        message = message.Trim('"');
                        if (message.StartsWith("settarget"))
                        {
                            targetmessage = args.Request.Message; //send it later
                        }
                        else
                        {
                            //adandon it
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error:" + e.Message);
                }
            }
            else
            {
                resdata.Add("Response", "{ \"result\": false }");
            }

            //      [here]
            //Edge  <-----  Me  ------ App
            await args.Request.SendResponseAsync(resdata);
            messageDeferral.Complete();
        }

        private void OnEdgeConnectionClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Debug.WriteLine("closed: Connection with Edge");

            connectionwithApp.Dispose();
            connectionwithApp = null;

            if (deferralwithApp != null)
            {
                deferralwithApp.Complete();
                deferralwithApp = null;
            }

            if (deferralwithEdge != null)
            {
                connectionwithEdge.Dispose();
                connectionwithEdge = null;
            }
        }

        private void OnAppConnectionClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Debug.WriteLine("closed: Connection with App.");

            connectionwithEdge.Dispose();
            connectionwithEdge = null;

            if (deferralwithApp != null)
            {
                deferralwithApp.Complete();
                deferralwithApp = null;
            }

            if (deferralwithEdge != null)
            {
                connectionwithEdge.Dispose();
                connectionwithEdge = null;
            }
        }


        //From here, template code generated by Visual Studio.
        //No changes have been made.


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
