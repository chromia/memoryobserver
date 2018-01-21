# memoryobserver
WebExtension for firefox to show memory consumption of your browser

![screenshot: image](http://chromia.cocotte.jp/monooki/files/ss/mo_ss01.png)

## Download

[download site](http://chromia.cocotte.jp/monooki/memoryobserver/)<br>
This plugin needs installation of extra native application.<br>
You must download "Addon file"(xpi) and "Native App" separately.

## Install
### for Windows
1. run "memory_observer_native_win.msi" to install native app
2. install "memory_observer-(version)-fx.xpi" to firefox

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

    *if you want to install as personal, replace line 4-6 to this command:  
    cmake -DSINGLEUSER=1 -DCMAKE_INSTALL_PREFIX=~/usr \.\.  
    make  
    make install

4. install "memory_observer-(version)-fx.xpi" to firefox

##Settings

On default settings, this addon gets memory consumption of "firefox.exe".  
So if you use Firefox alternative browser(e.g. Waterfox), specify browser name on addon setting page.

![screenshot: settings](http://chromia.cocotte.jp/monooki/files/ss/mo_ss02.png)
