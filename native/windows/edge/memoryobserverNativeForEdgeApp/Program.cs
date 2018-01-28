using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

//[Note]
//'Windows' namespace is defined on assembly named 'Windows.winmd' (included in Windows 10 SDK)
//To Adding this, 
//  1. Execute "Project->Add Reference" menu and select 'Windows.winmd' file from 'browse' dialog.
//  2. Modify '(thisproject).csproj' on texteditor directly.
//   before(relative path): <HintPath>..\..\..\..\..\..\..\Program Files (x86)\Windows Kits\10\UnionMetadata\Facade\Windows.WinMD</HintPath>
//   after (absolute path): <HintPath>$(SystemDrive)\Program Files (x86)\Windows Kits\10\UnionMetadata\Windows.winmd</HintPath>
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace memoryobserverNativeForEdgeApp
{
    class Program
    {
        //connection with memoryobserverNativeForEdge(UWP app)
        static AppServiceConnection connection;

        //search target
        static string targetprocess = "MicrosoftEdge|MicrosoftEdgeCP";

        static readonly string ResultOk = "ok";

        static void Main(string[] args)
        {
            Thread workerthread = new Thread(new ThreadStart(ThreadProc));
            workerthread.Start();
            Application.Run();
        }

        private static async void ThreadProc()
        {
            connection = new AppServiceConnection();
            connection.AppServiceName = "chromia.ext.memoryobserver";
            connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            await connection.OpenAsync();
            //AppServiceConnectionStatus status = await connection.OpenAsync();
            //if( status == AppServiceConnectionStatus.Success )
            //{
            //    //succeeded
            //}
            //else
            //{
            //    //failed
            //}
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Application.Exit();
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            string message = args.Request.Message.First().Value.ToString();
            string result = processMessage(message);
            ValueSet response = new ValueSet();
            response.Add("Response", result);
            args.Request.SendResponseAsync(response).Completed += delegate { };
        }

        public static string processMessage(string message)
        {
            CommandInfo ci = ParseMessage(message);

            string response = "";
            if (ci.command == "memory")
            {
                //get memory amount
                response = MakeResponse(ci.command, Getmemory(ci));
            }
            else if (ci.command == "settarget")
            {
                targetprocess = ci.commandargs[0];
                response = MakeResponse(ci.command, ResultOk);
            }

            //Return command response to browser
            return response;
        }

        public class CommandInfo
        {
            public string command;
            public List<string> commandargs;
            public CommandInfo(string command, List<string> commandargs)
            {
                this.command = command;
                this.commandargs = commandargs;
            }
        }

        public static string MakeResponse(string command, string response)
        {
            //JSON format
            return "{ \"command\": " + "\"" + command + "\"" + ", \"result\": " + "\"" + response + "\"" + " }";
        }

        public static CommandInfo ParseMessage(string message)
        {
            //Parsing JSON is quite bother. So use more simple format...
            //"command[,arg1][,arg2],..."  ( this is also formal format as JSON )
            char[] delimiters = { ',' };
            message = message.Trim('"');
            string[] tokens = message.Split(delimiters);
            List<string> commandargs = new List<string>();
            for (int i = 1; i < tokens.Length; i++)
            {
                commandargs.Add(tokens[i]);
            }
            string command = tokens[0];
            return new CommandInfo(command, commandargs);
        }

        public static string Getmemory(CommandInfo commandinfo)
        {
            List<string> args = commandinfo.commandargs;
            string type = (args.Count != 0) ? args[0] : "";

            System.Diagnostics.
            Process[] ps = Process.GetProcesses();
            Int64 totalmemory = 0;

            char[] delimiter = { '|' };
            string[] targets = targetprocess.Split(delimiter);

            foreach (Process p in ps)
            {
                try
                {
                    string processname = p.ProcessName;
                    foreach (string target in targets)
                    {
                        if (processname == target)
                        {
                            if (type == "privateworkingset")
                            {
                                totalmemory += p.PrivateMemorySize64;
                            }
                            else
                            {
                                totalmemory += p.WorkingSet64;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return totalmemory.ToString();
        }
    }
}
