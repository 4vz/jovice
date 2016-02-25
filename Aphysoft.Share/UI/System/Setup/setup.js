

(function () {

    function systemCheck(p) {
        
        // check cookie
        var cookieStatus = 2;
        if (share.cookie("shts") == "shts") {
            share.removeCookie("shts");
            if (share.cookie("shts") == null) {
                cookieStatus = 0
            }
            else cookieStatus = 1;
        }

        // check local storage
        var lsStatus = 1;
        if (Modernizr.localstorage) lsStatus = 0;

        // check version
        var versionStatus = 1;
        var version = 0, serverVersion = p.data("serverVersion");
        //return;alert(p.data("defaultPage"));
        if (share.cookie("vers") != null) {
            version = parseInt(share.cookie("vers"));
        }
        if (version == serverVersion) {
            versionStatus = 0;
        }

        if (lsStatus == 0) {
            if (versionStatus == 1) {

                if (version == 0) {
                    // install
                    //debug("install");
                }
                else {
                    // update
                    //debug("update");
                }
                var expDate = share.date();
                expDate.setMonth(expDate.getMonth() + 1);

                share.cookie("vers", serverVersion, expDate);

                var whenDone = share.param("done");
                
                if (whenDone != null) {
                    p.transfer(share.urlDecode(whenDone), { replace: true });
                }
            }
            else {
                var whenDone = share.param("done");
                
                if (whenDone == null) {
                    p.transfer(p.data("defaultPage"), { replace: true });
                }
                else {
                    p.transfer(share.urlDecode(whenDone), { replace: true });
                }
            }
        }
    }
      
    ui("system_setup", {
        start: function (p) {
            systemCheck(p);
            p.done();
        }
    });


})();