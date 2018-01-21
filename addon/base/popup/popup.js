
browser.runtime.sendMessage("memoryuse", function (response){
    const elem = document.querySelector(".used");
    elem.textContent = "Memory Used: " + response + " GB";
});

browser.runtime.sendMessage("getstatus", function (response){
    const enable = response.enable;
    const targetname = response.targetname;
    const elem_status = document.querySelector(".status");
    const elem_error = document.querySelector(".error");
    const elem_target = document.querySelector(".target");

    elem_status.textContent = "Status: " + ((enable) ? "Active" : "Error");
    if( enable ){
        elem_error.classList.add('hidden');
    }else{
        elem_error.innerHTML = "Please check if <br>native application(memoryobserver.exe)<br> is installed correctly.";
        elem_error.classList.remove("hidden");
    }
    elem_target.textContent = "Target: " + targetname;
});
