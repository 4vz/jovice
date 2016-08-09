(function () {

    function toTitleCase(str) {
        return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
    }

    ui("search_node", function (b, r, f) {

        //--- match properties
        //f.setButton();        
        //f.setExpand(105, 185, null, null, null, null);
        f.setSize(105);

        //--- entry values
        var nodeName = f.column("NO_Name");
        var awName = f.column("AW_Name");
        var agName = f.column("AG_Name");
        var usage = f.column("UR");
        var cpu = f.column("CPU");
        var mem = f.column("MEMORY_PERCENTAGE");
        var memt = f.column("MEMORY_TOTAL");
        var cif = f.column("INTERFACE_COUNT");
        var cupif = f.column("INTERFACE_COUNT_UP");

        //debug(cif, cupif);

        if (f.create) {
            r.nodeName = ui.text(b)({ font: ["body", 22], color: 25, weight: "600", noBreak: true, truncate: true, cursor: "copy", clickToSelect: true, position: [20, 20] });
            r.location = ui.text(b)({ font: ["body", 19], color: 25, top: 59, left: 20, clickToSelect: true, cursor: "copy" });

            r.boxInterfaceInfo = ui.box(b)({ position: [400, 20], size: [140, 65] });
            r.infcount = ui.text(r.boxInterfaceInfo)({ font: 45, bottom: -8, color: 25 });
            r.inftcount = ui.text(r.boxInterfaceInfo)({ font: 15, bottom: 1, color: 25, attach: [r.infcount, "right", 5, 45] });
            ui.text(r.boxInterfaceInfo)({ font: 12, color: 0, text: "ACTIVE INTERFACE" });

            r.boxCPUInfo = ui.box(b)({ position: [540, 20], size: [140, 65] });
            r.cpu = ui.text(r.boxCPUInfo)({ font: 45, bottom: -8, color: 25 });
            r.cpup = ui.text(r.boxCPUInfo)({ font: 15, bottom: 1, color: 25, text: "%", attach: [r.cpu, "right", 5, 45] });
            ui.text(r.boxCPUInfo)({ font: 12, color: 0, text: "CPU" });

            r.boxMEMInfo = ui.box(b)({ position: [680, 20], size: [140, 65] });
            r.mem = ui.text(r.boxMEMInfo)({ font: 45, bottom: -8, color: 25 });
            r.memp = ui.text(r.boxMEMInfo)({ font: 15, bottom: 1, color: 25, text: "%", attach: [r.mem, "right", 5, 45] });
            ui.text(r.boxMEMInfo)({ font: 12, color: 0, text: "MEMORY" });

            r.boxURInfo = ui.box(b)({ position: [820, 20], size: [220, 65] });
            r.urt = ui.text(r.boxURInfo)({ font: 45, bottom: -8, color: 25, noBreak: true });
            ui.text(r.boxURInfo)({ font: 12, color: 0, text: "USAGE RATING" });
        }

        r.expand = function (create, callback) {

            if (create) {
                callback();
            }
            else callback();
        };

        r.nodeName({ text: nodeName });
        r.location({ text: "Witel " + toTitleCase(awName) + ", " + agName });

        /*if (usage > 0.7) {
            r.nodeName({ color: "red" });
        }
        else if (usage > 0.3) {
            r.nodeName({ color: "orange" });
        }
        else {
            r.nodeName({ color: "green" });
        }*/
        r.infcount({ text: cupif });
        r.inftcount({ text: "/ " + cif });
        if (cpu == -1) {
            r.cpu({ text: "N/A", color: 75 });
            r.cpup.hide();
        }
        else {
            r.cpu({ text: cpu, color: 25 });
            r.cpup.show();
        }

        if (memt == null) {
            r.mem({ text: "N/A", color: 75 });
            r.memp.hide();
        }
        else {
            r.mem({ text: Math.round(mem * 100), color: 25 });
            r.memp.show();
        }

        if (usage > 0.85) r.urt({ text: "VERY HIGH", color: "red" });
        else if (usage > 0.7) r.urt({ text: "HIGH", color: ui.color(255, 128, 0) });
        else if (usage > 0.3) r.urt({ text: "NORMAL", color: ui.color(80, 128, 80) });
        else if (usage > 0) r.urt({ text: "LOW", color: "accent" });
        else r.urt({ text: "N/A", color: 75 });
    });


})();