
//target process name
const targetid = "targetname";
const target = document.getElementById(targetid);
const defaulttarget = "firefox";

function onError(error) {
    console.log(`Error: ${error}`);
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

target.addEventListener("input", event => {
    //save new value
    const savetext = target.value.trim().replace(/\s+/g, " "); //remove extra spaces
	browser.storage.local.set({ targetname: savetext });

    //dispatch to background script
    browser.runtime.sendMessage("optionupdate");
});

function restoreOptions() {
    function restoreValue(result) {
        target.value = result.targetname || "firefox"; // read value or default
    }
    getlocalstorage(targetid, restoreValue);
}

document.addEventListener("DOMContentLoaded", restoreOptions);
