
//target process name
const targetid = "targetname";
const target = document.getElementById(targetid);
const defaulttarget = "firefox";

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

    function onError(error) {
        console.log(`Error: ${error}`);
    }

    var getting = browser.storage.local.get(targetid);
    getting.then(restoreValue, onError);
}

document.addEventListener("DOMContentLoaded", restoreOptions);
