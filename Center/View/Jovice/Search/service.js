
(function () {



    ui("search_jovice_service", function (b, r, f, p, t) {
                
        //--- match properties
        /*
        f.setButton(function () {
            p.transfer("/network/service/" + serviceID, {
                transferData: {
                    serviceID: r.serviceID.text(),
                    customerName: r.customerName.text(),
                    serviceType: r.serviceType.text(),
                    entries: t.entries
                },
                leave: function () {
                    t.animTransferedClick();

                    var at = b.absoluteTop();
                    b.$.detach();
                    p.$.append(b.$);
                    b.top(at - ui.marginTop());

                    b.color(null, { duration: 300, queue: false });
                    b.height(200, { duration: 300, queue: false });
                    
                    r.serviceID.$.transition({ y: -20, duration: 200, opacity: 0 });
                    r.customerName.$.transition({ x: 20, duration: 200, opacity: 0 });
                    r.serviceType.$.transition({ x: 20, duration: 200, opacity: 0 });
                    r.purpose1.$.transition({ x: 20, duration: 200, opacity: 0 });
                    r.purpose2.$.transition({ x: 20, duration: 200, opacity: 0 });

                    r.topology.$.transition({ y: 20, duration: 200, opacity: 0 });

                    $$(700, function () {
                        t.searchresult.fadeOut(100);
                    });

                    var found = false;
                    $.each(t.resultBoxes, function (bi, bv) {
                        if (bv == b) {
                            found = true;
                        }
                        else {
                            var bt = bv.absoluteTop();
                            bv.$.detach();
                            p.$.append(bv.$);
                            bv.top(bt - ui.marginTop());

                            if (!found) {
                                if (bv.top() <= 200) bv.hide();
                                else {
                                    //bv.$.animate({ opacity: 0 }, { duration: 300, queue: false });
                                    bv.$.transition({ y: -(t.filter.isShown() ? 80 : 40), opacity: 0, duration: 300 });
                                }
                            }
                            else { // found
                                bv.$.transition({ y: 200, opacity: 0, duration: 300 });
                            }
                        }
                    });
                    //b.height(50, { duration: 300 });
                }
            });
        });
        */
        f.setSize(100);
        
        //--- entry values
        var serviceID = f.column("SE_SID");
        var customerName = f.column("SC_Name");
        var setype = f.column("SE_Type");

        var topologies = f.column("Topology");
        var purposes = f.column("Purpose");
        
        var vrfnames = f.column("Vrf");
        var rateinputs = f.column("RateInput");
        var inputlimits = f.column("InputLimiter");
        var rateoutputs = f.column("RateOutput");
        var outputlimits = f.column("OutputLimiter");
        var ips = f.column("IP");
        var vcids = f.column("VCID");
        var nodeinfos = f.column("NodeInfo");
        var localaccesses = f.column("LocalAccess");       
        var routeTypes = f.column("RouteType");

        var streamSeID = f.column("StreamServiceID");
        
        if (f.create) {                        
            r.serviceID = ui.text(b)({ font: ["body", 15], color: "accent", top: 13, left: 20, weight: "600", noBreak: true, clickToSelect: true, cursor: "copy" });
            r.serviceType = ui.text(b)({ font: ["body", 15], color: 25, clickToSelect: true, cursor: "copy", attach: [r.serviceID, "right", 20] });
            r.customerName = ui.text(b)({ font: ["body", 15], color: 25, weight: "600", noBreak: true, truncate: true, attach: [r.serviceType, "right2", 20, 20] });

            r.serviceInformation = ui.box(b)({ top: 40, height: 60, leftRight: 20 });

            r.topologyReferences = [];
        }

        //-- MAIN
        r.serviceID.text(serviceID);
        r.customerName.text(customerName != null ? customerName : "");                
        if (setype != null) {
            var sesubtype = f.column("SE_SubType");
            var setypetext = null;

            if (setype == "VP") {
                if (sesubtype == "TA") setypetext = "Trans Access";
                else setypetext = "VPNIP";
            }
            else if (setype == "AS") setypetext = "Astinet";
            else if (setype == "AB") setypetext = "Astinet Beda Bandwidth";
            else if (setype == "VI") setypetext = "VPN Instant";
            else if (setype == "ID") setypetext = "Metro Ethernet (Incompleted)";
            else if (setype == "ME") {
                if (sesubtype == "PP") setypetext = "Metro Ethernet Point-To-Point";
                else if (sesubtype == "PM") setypetext = "Metro Ethernet Point-To-Multipoint";
                else if (sesubtype == "MM") setypetext = "Metro Ethernet Multipoint-To-Multipoint";
                else if (sesubtype == "IP") setypetext = "Metro Ethernet Point-To-Point (Intercity)";
                else setypetext = "Metro Ethernet";
            }
            else if (setype == "TS") {
                if (sesubtype == "SI") setypetext = "Telkomsel Site";
            }
            r.serviceType.text(setypetext);
        }
        else r.serviceType.text("");

        //-- TOPOLOGIES
        if (topologies.length > 0) r.serviceInformation.show();
        else r.serviceInformation.hide();
               
        $.each(topologies, function (topologyIndex, topology) {

            var ref = r.topologyReferences[topologyIndex];

            if (ref == null) {
                r.topologyReferences[topologyIndex] = {};
                ref = r.topologyReferences[topologyIndex];

                ref.purpose = ui.text(b)({
                    cursor: "pointer", font: ["body", 15], hide: true, button: {
                        normal: function () { this.color(25); },
                        over: function () { this.color(50); },
                    }
                });

                ref.area = ui.box(r.serviceInformation)({ size: ["100%", 60], top: topologyIndex * 60 });

                ref.speedArea = ui.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: false });
                ui.icon(ref.speedArea, center.icon("speed"))({ top: 2, left: 0, color: 45, size: [16, 16] });
                ref.speedText = ui.text(ref.speedArea)({ font: ["body", 13], color: 0, top: 0, left: 22, noBreak: true });

                ref.vrfArea = ui.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                ui.icon(ref.vrfArea, center.icon("cloud"))({ top: 2, left: 0, color: 45, size: [16, 16] });
                ref.vrfText = ui.text(ref.vrfArea)({ font: ["body", 13], color: 0, top: 0, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });

                ref.ipArea = ui.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                ui.icon(ref.ipArea, center.icon("IP"))({ top: 2, left: 0, color: 45, size: [16, 16] });
                ref.ipText = ui.text(ref.ipArea)({ font: ["body", 13], color: 0, top: 0, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });

                ref.vcidArea = ui.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                ui.text(ref.vcidArea)({ top: 2, left: 4, font: ["body", 6], text: "VC", color: 10 });
                ui.text(ref.vcidArea)({ top: 8, left: 5, font: ["body", 6], text: "ID", color: 10 });
                ref.vcidText = ui.text(ref.vcidArea)({ font: ["body", 13], color: 0, top: 0, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });

                ref.routeArea = ui.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                ui.icon(ref.routeArea, center.icon("map"))({ top: 2, left: 0, color: 45, size: [16, 16] });
                ref.routeText = ui.text(ref.routeArea)({ font: ["body", 13], color: 0, top: 0, left: 22, noBreak: true });

                ref.updateArea = ui.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                ui.icon(ref.updateArea, center.icon("time"))({ top: 2, left: 0, color: 45, size: [16, 16] });
                ref.updateText = ui.text(ref.updateArea)({ font: ["body", 13], color: 0, top: 0, left: 22, noBreak: true });

                ref.topologyArea = ui.box(ref.area)({ leftRight: [0, 0], top: 22, height: 28, hide: true, scroll: { vertical: false, horizontal: true, type: "button" } });
            }

            ref.area.show();
            ref.area.top(topologyIndex * 60);            
            ref.purpose.button({
                click: function () {
                    $.each(topologies, function (index) {
                        var iref = r.topologyReferences[index];
                        if (index != topologyIndex) {
                            iref.purpose.enableButton();
                            iref.purpose.color(25);
                        }
                        else {
                            iref.purpose.disableButton();
                            iref.purpose.color("accent");
                        }
                        iref.area.top((index - topologyIndex) * 60, { duration: 166 });
                    });
                }
            });

            t.drawTopology(f, ref, topology, topologyIndex, ref.topologyArea);

            ref.purpose({ text: purposes[topologyIndex], color: topologyIndex == 0 ? "accent" : 25 });
            if (topologyIndex == 0) ref.purpose.disableButton();
            else ref.purpose.enableButton();
                        
            var vrf = vrfnames[topologyIndex];
            var rateinput = rateinputs[topologyIndex];
            var inputlimit = inputlimits[topologyIndex];
            var rateoutput = rateoutputs[topologyIndex];
            var outputlimit = outputlimits[topologyIndex];
            var ip = ips[topologyIndex];
            var vcid = vcids[topologyIndex];
            var nodeinfo = nodeinfos[topologyIndex];
            var localaccess = localaccesses[topologyIndex];
            var routetype = routeTypes[topologyIndex];

            var lanc = 0;

            if (rateoutput > 0) {
                var rt = rateoutput * 1024;
                var fb = center.formatBytes(rt, 10);
                var spt = fb[0] + "";
                var spr = spt.split('.')[0];

                ref.speedArea.show();
                ref.speedText.text(spr + " " + fb[1] + "PS");
                ref.speedArea({ width: ref.speedText.leftWidth() + 10 });

                lanc = ref.speedArea.leftWidth() + 20;
            }
            else ref.speedArea.hide();

            if (vrf != null) {
                ref.vrfArea.show();
                ref.vrfText.text(vrf);
                ref.vrfArea({ left: lanc, width: ref.vrfText.leftWidth() + 10 });

                lanc = ref.vrfArea.leftWidth() + 20;
            }
            else ref.vrfArea.hide();

            if (ip != null) {
                ref.ipArea.show();
                ref.ipText.text(ip);
                ref.ipArea({ left: lanc, width: ref.ipText.leftWidth() + 10 });

                lanc = ref.ipArea.leftWidth() + 20;
            }
            else ref.ipArea.hide();

            if (vcid != null) {
                ref.vcidArea.show();
                ref.vcidText.text(vcid);
                ref.vcidArea({ left: lanc, width: ref.vcidText.leftWidth() + 10 });

                lanc = ref.vcidArea.leftWidth() + 20;
            }
            else ref.vcidArea.hide();

            if (routetype != null) {
                ref.routeArea.show();
                var types = [];
                $.each(routetype, function (ri, rv) {
                    var rdesc;
                    if (rv == "R") rdesc = "RIP";
                    else if (rv == "O") rdesc = "OSPF";
                    else if (rv == "S") rdesc = "STATIC";
                    else if (rv == "B") rdesc = "BGP";
                    else if (rv == "E") rdesc = "EIGRP";
                    if (types.indexOf(rdesc) == -1)
                        types.push(rdesc);
                });
                //var rdesc;

                ref.routeText.text(types.join(", "));
                ref.routeArea({ left: lanc, width: ref.routeText.leftWidth() + 10 });

                lanc = ref.routeArea.leftWidth() + 20;
            }
            else ref.routeArea.hide();

            var olac = Number.MIN_VALUE;
            var istim = false;
            $.each(nodeinfo, function (nodeinfoindex, nodeinfovalue) {
                var tim = nodeinfovalue[1];
                if (tim != null) {
                    //debug(tim);
                    istim = true;
                    var lac = $$.date(nodeinfovalue[1]).getTime();
                    if (lac > olac) olac = lac;
                }
            });
            if (istim) {
                var olacd = new Date(olac);
                ref.updateArea.show();
                ref.updateText.text("UPDATED " + $$.fromNow(olacd).description.toUpperCase());
                //ref.updateText.text($$.date("{DD} {MMMM} {YYYY} {HH}:{mm}:{ss}", olacd).toUpperCase() + " (" + $$.fromNow(olacd).description.toUpperCase() + ")");
                ref.updateArea({ left: lanc, width: ref.updateText.leftWidth() + 10 });
                lanc = ref.updateText.leftWidth() + 20;
            }
            else ref.updateArea.hide();            
        });

        if (topologies.length > 0) {
            $.each(r.topologyReferences, function (index, reference) {
                if (index < topologies.length) {
                    if (topologies.length > 1) {
                        reference.purpose.show();
                        if (index == 0) reference.purpose.attach(r.serviceType, "right", 20);
                        else reference.purpose.attach(r.topologyReferences[index - 1].purpose, "right", 20);
                    }
                    else reference.purpose.hide();
                }
                else {
                    reference.area.hide();
                    reference.purpose.hide();
                }
            });

            if (topologies.length > 1) r.customerName.attach(r.topologyReferences[topologies.length - 1].purpose, "right2", 20, 20);
            else r.customerName.attach(r.serviceType, "right2", 20, 20);
        }
        else r.customerName.attach(r.serviceType, "right2", 20, 20);
    });

})();