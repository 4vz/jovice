

(function () {

    function systemCheck(p) {
        
        // check cookie
        var cookieStatus = 2;
        if ($$.cookie("shts") == "shts") {
            $$.removeCookie("shts");
            if ($$.cookie("shts") == null) {
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


        var versc = $$.cookie("vers");
        if (versc != null) {
            if ($.isArray(versc)) {                
                //$$.removeCookie("vers");
                //versionStatus = 0;
                alert("Problem with vers, we cannot continue.");
                return;
            }
            else
                version = parseInt(versc);
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
                var expDate = $$.date();
                expDate.setMonth(expDate.getMonth() + 1);

                $$.cookie("vers", serverVersion, expDate);

                var whenDone = $$.param("done");
                
                if (whenDone != null) {
                    p.transfer($$.urlDecode(whenDone), { replace: true });
                }
            }
            else {
                var whenDone = $$.param("done");
                
                if (whenDone == null) {
                    p.transfer(p.data("defaultPage"), { replace: true });
                }
                else {
                    p.transfer($$.urlDecode(whenDone), { replace: true });
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