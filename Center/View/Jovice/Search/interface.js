
(function () {

    $$.script("search_jovice_interface", function (b, r, f, p, t) {
        f.setSize(100);

        //--- entry values
        var interfaceName = f.column("I_Name");
        var nodeName = f.column("NO_Name");
        var interfaceType = f.column("I_Type");
        var interfaceDesc = f.column("I_Desc");
        
        if (f.create) {
            r.nodeName = $$.text(b)({ font: ["body", 15], color: 25, weight: "600", top: 13, left: 20, noBreak: true, clickToSelect: true, cursor: "copy" });
            r.interfaceName = $$.text(b)({ font: ["body", 15], color: "accent", weight: "600", noBreak: true, clickToSelect: true, cursor: "copy", attach: [r.nodeName, "right", 20] });
            r.interfaceDesc = $$.text(b)({ font: ["body", 15], color: 25, noBreak: true, truncate: true, attach: [r.interfaceName, "right2", 20, 20] });
        }

        r.nodeName.text(nodeName);
        r.interfaceName.text(center.formatInterfaceName(interfaceName, f.column("NO_Manufacture")));
        r.interfaceDesc.text(interfaceDesc);

        /*
        var topologies = f.column("Topology");

        if (topologies.length > 0) {
            if (r.topologyArea == null) {
                r.topologyArea = ui.box(b)({ leftRight: [20, 20], top: 62, height: 28, hide: true, scroll: { vertical: false, horizontal: true, type: "button" } });
            }
            r.topologyArea.show();
            t.drawTopology(f, r, topologies[0], 0, r.topologyArea);
        }
        else if (r.topologyArea != null) r.topologyArea.hide();
        */
    });

})();