if( typeof browser === 'undefined'){ browser = undefined; }
if( typeof chrome === 'undefined'){ chrome = undefined; }
browser = browser || chrome;

//target process name
const targetid = "targetname";
const target = document.getElementById(targetid);


function onError(error) {
    console.log(`Error: ${error}`);
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
//wrapper of runtime.sendMessage
function sendMessage(key, callback) { callwrapper(browser.runtime.sendMessage, key, callback); }


target.addEventListener("input", event => {
    //save new value
    const savetext = target.value.trim().replace(/\s+/g, " "); //remove extra spaces
	browser.storage.local.set({ targetname: savetext });

    //dispatch to background script
    browser.runtime.sendMessage("optionupdate");
});

function restoreOptions() {
    //get settings from storage
    getlocalstorage(targetid, (fromstorage) =>{
        //get default value
        sendMessage("getdefaulttarget", (defaulttarget) =>{
            target.value = fromstorage.targetname || defaulttarget;
        });
    });
}

document.addEventListener("DOMContentLoaded", restoreOptions);
