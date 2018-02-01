# memoryobserver
WebExtension for firefox to show memory consumption of your browser

![screenshot: image](http://chromia.cocotte.jp/monooki/files/ss/mo_ss01.png)

## Download

[download site](http://chromia.cocotte.jp/monooki/memoryobserver/)<br>
This plugin needs installation of extra native application.<br>
You must download "Addon file" and "Native App" separately.

## Install
### for Windows
1. run "memory_observer_native_win.msi" to install native app
2. install addon file.  
For Firefox: "memory_observer-(version)-fx.xpi"  
For Chrome: "memory_observer-(version)-ch.crx"

### for Linux
1. download sourcefiles (download files from this repository)
2. install mono

    sudo apt-get install mono-devel  
    *see [Mono official site](http://www.mono-project.com/download/#download-lin)

3. run these commands

    cd native/linux  
    mkdir build  
    cd build  
    cmake \.\.  
    make  
    sudo make install

    *If you want to install as personal, replace line 4-6 to this command:  
    cmake -DSINGLEUSER=ON -DCMAKE_INSTALL_PREFIX=~/usr \.\.  
    make  
    make install

    *The default behavior, configuration files for Firefox/Chrome/Chromium are installed.
    You can select preferred target browser by option flags( -DFIREFOX=[ON|OFF] or -DCHROME=[ON|OFF] or -DCHROMIUM=[ON|OFF], default is all ON )

4. install addon file.  
For Firefox: "memory_observer-(version)-fx.xpi"  
For Chrome: "memory_observer-(version)-ch.crx"

## Settings

On default settings, this addon gets memory consumption of "firefox.exe" or "chrome.exe" (browser detection is incomplete).  
So if you use alternative browser(e.g. Waterfox), specify browser name on addon setting page.

![screenshot: settings](http://chromia.cocotte.jp/monooki/files/ss/mo_ss02.png)

Name List of Browsers (without extension '.exe')

|Browser|Name|
|-|-|
|Firefox|firefox|
|Waterfox|waterfox|
|Chrome|chrome|
|Chromium|chrome (Windows)<br>chromium-browser (Linux)|
|Opera|opera|
|Vivaldi|vivaldi (Windows)<br>vivaldi-bin (Linux)|
