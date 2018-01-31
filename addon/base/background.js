if( typeof browser === 'undefined'){ browser = undefined; }
if( typeof chrome === 'undefined'){ chrome = undefined; }

function getdefaulttarget()
{
    //Simple browser detection
    //TODO: Find more clever way
    let b = null;
    let c = null;
    if (browser && typeof browser.runtime !== 'undefined'){
        b = browser.runtime;
    }
    if (chrome && typeof chrome.runtime !== 'undefined'){
        c = chrome.runtime;
    }
    if( b && c ){
        return "firefox"; //firefox.exe
    }else if( c ){
        return "chrome"; //chrome.exe
    }else if( b ){
        return "MicrosoftEdge|MicrosoftEdgeCP"; //edge executes
    }else{
        return "firefox"; //unknown...
    }
};

//variables
let memoryuse = '----';
let enable = true;
let targetname = null;
const defaulttarget = getdefaulttarget();
const firstinterval = 1000; // [ms]
const interval = 5000; // [ms]

//initialize webextensions API object
//firefox has both 'browser' and 'chrome'
//chrome has only 'chrome'
//edge has 'browser' and INCOMPLETE 'chrome'
//So best solution is below:
browser = browser || chrome;
//browser = chrome || browser; //this is bad on Edge


//connect to native application
let port = null;
try{
    port = browser.runtime.connectNative("chromia.ext.memoryobserver");
    port.onDisconnect.addListener((p) => {
        enable = false;
        browser.browserAction.setBadgeText({text: "E"});
        //browser.browserAction.setBadgeBackgroundColor({color: "red"});
        browser.browserAction.setBadgeBackgroundColor({color: "#FF0000"}); //Note: Edge doesn't accept color name, use color code
    });
}catch(e){
    console.log(e);
}

function convertResponse(response)
{
    //On Edge, type of response from Native App is 'string' ( not object )
    if(typeof(response) == 'string'){
        return JSON.parse(response);
    }else{
        return response;
    }
}

//function call wrapping
//On Firefox&Chrome, storage.local.get requires 1 arg(key).
//But Edge requires 2 args(key, callback) because Edge doesn't support Promise model.
//ref: https://developer.microsoft.com/en-us/microsoft-edge/platform/issues/9420301/
function callwrapper(func, key, callback)
{
    const promise = func(key, callback);
    if(promise !== undefined){
        promise.then(callback, onError);
    }
}
//wrapper of storage.local.get
function getlocalstorage(key, callback) {  callwrapper(browser.storage.local.get, key, callback); }

function settarget()
{
    //get target process name from storage
    getlocalstorage("targetname", (result) =>{
        targetname = result.targetname || defaulttarget;
        //then, send it to native application
        if( enable ){
            port.postMessage("settarget," + targetname);
        }
    });
}
settarget(); //call it once on initializing phase

//initialize timer for update
function update(){
    if( enable ){
        port.postMessage("memory,workingset");
    }
    setTimeout(update, interval);
}
setTimeout(update, firstinterval);

//receiver from native application
port.onMessage.addListener( (response) => {
    response = convertResponse(response);
    if( response.command == 'memory' ){
        const mem_raw = parseInt(response.result);
        const mem = getmemtext(mem_raw);
        memoryuse = mem.toString();
        browser.browserAction.setBadgeText({ text: memoryuse });
        //browser.browserAction.setBadgeBackgroundColor({ color: "green" });
        browser.browserAction.setBadgeBackgroundColor({color: "#008000"}); //Note: Edge doesn't accept color name, use color code
    }
});

function getmemtext(memory)
{
    const mem_giga = memory / 1024.0 / 1024.0 / 1024.0;
    return mem_giga.toFixed(1);
}

//connect to popup/option script
browser.runtime.onMessage.addListener(function(message, sender, sendResponse) {
    if( message == "memoryuse" ){
        sendResponse(memoryuse);
    }else if( message == "getstatus" ){
        sendResponse({ enable: enable, targetname: targetname });
    }else if( message == "optionupdate" ){
        sendResponse("ok");
        settarget(); //update target process name
    }else if( message == "getdefaulttarget"){
        sendResponse(defaulttarget);
    }
    return true
});

function onError(error) {
    console.log(`Error: ${error}`);
}
