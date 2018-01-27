
//variables
let memoryuse = '----';
let enable = true;
let targetname = null;
const firstinterval = 1000; // [ms]
const interval = 5000; // [ms]


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

//wrapper of storage.local.get
function getlocalstorage(key, callback)
{
    //On Firefox&Chrome, storage.local.get requires 1 arg(key).
    //But Edge requires 2 args(key, callback) because Edge doesn't support Promise model.
    //ref: https://developer.microsoft.com/en-us/microsoft-edge/platform/issues/9420301/
    const promise = browser.storage.local.get(key, callback); //Edge calls callback-func here( and returns undefined )
    if(promise !== undefined){
        promise.then(callback, onError); //Firefox&Chrome calls callback-func here
    }
}

function settarget()
{
    //get target process name from storage
    getlocalstorage("targetname", (result) =>{
        targetname = result.targetname || "firefox";
        //then, send it to native application
        port.postMessage("settarget," + targetname);
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
    }
    return true
});

function onError(error) {
    console.log(`Error: ${error}`);
}
