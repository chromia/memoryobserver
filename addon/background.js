
//variables
let memoryuse = '----';
let enable = true;
let targetname = null;
const firstinterval = 1000; // [ms]
const interval = 5000; // [ms]


//connect to native application
const port = browser.runtime.connectNative("chromia.ext.memoryobserver");
port.onDisconnect.addListener((p) => {
    if (p.error) enable = false;
    browser.browserAction.setBadgeText({text: "E"});
    browser.browserAction.setBadgeBackgroundColor({color: "red"});
});

function settarget()
{
    //get target process name from storage
    const getopt = browser.storage.local.get("targetname");
    getopt.then( (result) =>{
        targetname = result.targetname || "firefox";
        //then, send it to native application
        port.postMessage("settarget," + targetname);
    }, onError);
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
    if( response.command == 'memory' ){
        const mem_raw = parseInt(response.result);
        const mem = getmemtext(mem_raw);
        memoryuse = mem.toString();
        browser.browserAction.setBadgeText({text: memoryuse});
        browser.browserAction.setBadgeBackgroundColor({color: "green"});
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
