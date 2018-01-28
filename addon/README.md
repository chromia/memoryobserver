## Addon

- addon: webextension root
    - base: source code of addon excluding manifest.json
    - manifest: manifest.json file for each browser
    - (pub_firefox): output directory for publishing
    - publish.sh: make output directory named 'pub_**'

## Publishing

- For Firefox
    1. Run publish.sh(On windows, please use [git bash](http://gitforwindows.org/))

        ./publish.sh

    2. Signing and publishing files in pub_firefox directory.  
    See <https://developer.mozilla.org/en-US/Add-ons/WebExtensions/Publishing_your_WebExtension>

- For Edge
    1. Open /native/windows/edge/memoryobserverForEdge.sln (Visual Studio 2017 is needed)
    2. Run "Build" "Deploy" "Create App Package" from menu
    3. Signing and publishing  
    See <https://docs.microsoft.com/en-us/microsoft-edge/extensions/guides/packaging>
