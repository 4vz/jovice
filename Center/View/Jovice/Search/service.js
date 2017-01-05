
(function () {

    function getSection(ars, key) {
        var ar = null;
        $.each(ars, function (ai, av) {
            if (av[0] == key) {
                ar = av;
                return false;
            }
        });
        return ar;
    };

    ui("search_jovice_service", function (b, r, f, p, t) {

        //--- match properties
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
        f.setSize(100);
        
        //--- entry valuesW
        var serviceID = f.column("SE_SID");
        var customerName = f.column("SC_Name");
        var setype = f.column("SE_Type");
        var topologies = f.column("Topology");
        var vrfnames = f.column("Vrf");
        var rateinputs = f.column("RateInput");
        var inputlimits = f.column("InputLimiter");
        var rateoutputs = f.column("RateOutput");
        var outputlimits = f.column("OutputLimiter");
        var purposes = f.column("Purpose");
        var ips = f.column("IP");
        var vcids = f.column("VCID");
        var nodeinfos = f.column("NodeInfo");
        var localaccesses = f.column("LocalAccess");
        var streamSeID = f.column("StreamServiceID");
        var routeTypes = f.column("RouteType");
        
        //--- variables
        var topologyFocusIndex = 0;
        
        if (f.create) {

            r.topologyRef = [];
                        
            r.serviceID = ui.text(b)({ font: ["body", 15], color: "accent", top: 13, left: 20, weight: "600", noBreak: true, clickToSelect: true, cursor: "copy" });
            r.serviceType = ui.text(b)({ font: ["body", 15], color: 25, clickToSelect: true, cursor: "copy", attach: [r.serviceID, "right", 20] });
            r.customerName = ui.text(b)({ font: ["body", 15], color: 25, weight: "600", noBreak: true, truncate: true, attach: [r.serviceType, "right2", 20, 20] });

            //debug(r.serviceID.absolutePosition());
            
            r.topology = ui.box(b)({ top: 40, height: 60, leftRight: 20 });

            var prevArea = null;

            $$.for(0, 2, function (i) {
                var ref = r.topologyRef[i];
                if (ref == null) { ref = {}; r.topologyRef[i] = ref; }

                ref.area = ui.box(r.topology)({ size: ["100%", 60] });
                if (prevArea != null) ref.area.attach(prevArea, "bottom");
                prevArea = ref.area;

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
     
                ref.topoArea = ui.box(ref.area)({ leftRight: [0, 0], top: 22, height: 28, hide: true, scroll: { vertical: false, horizontal: true, type: "button" } });
                ref.topoCanvas = ui.raphael(ref.topoArea)({ left: 0, top: 0, height: 28 });
                ref.topoContents = ui.box(ref.topoArea)({ left: 0, top: 0, height: 28 });
            });

            var purposeNormal = function () { this.color(25); };
            var purposeOver = function () { this.color(50); };
            var purposeClick = function () {
                this({ disableButton: true, color: "accent" });
                r.purposeOther({ enableButton: true, color: 25 });
                var index = 0;
                if (this.left() < r.purposeOther.left()) {
                    r.topologyRef[0].area.top(0, { duration: 166 });
                    index = 0;
                }
                else {
                    r.topologyRef[0].area.top(-r.topologyRef[0].area.height(), { duration: 166 });
                    index = 1;
                }
                r.purposeOther = this;
                r.change(index);
            };

            r.purposeOther = null;
            r.purpose1 = ui.text(b)({ cursor: "pointer", font: ["body", 15], attach: [r.serviceType, "right", 20], hide: true, button: { normal: purposeNormal, over: purposeOver, click: purposeClick }});
            r.purpose2 = ui.text(b)({ cursor: "pointer", font: ["body", 15], attach: [r.purpose1, "right", 20], hide: true, button: { normal: purposeNormal, over: purposeOver, click: purposeClick } });
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
                else if (sesubtype == "IP") setypetext = "METRO Ethernet Point-To-Point (Intercity)";
                else setypetext = "Metro Ethernet";
            }
            r.serviceType.text(setypetext);
        }
        else r.serviceType.text("");

        //-- TOPOLOGIES
        var piid = null;
        r.topologyRef[0].area.top(0);
        r.index = 0;

        if (topologies.length > 1) {
            r.topology.show();
            r.purposeOther = r.purpose1;
            r.purpose1({ show: true, text: purposes[0], color: "accent", disableButton: true });
            r.purpose2({ show: true, text: purposes[1], color: 25, enableButton: true });
            r.customerName.attach(r.purpose2, "right2", 20, 20);
        }
        else {
            r.purpose1.hide();
            r.purpose2.hide();
            if (topologies.length == 1) r.topology.show();
            else r.topology.hide();
            r.customerName.attach(r.serviceType, "right2", 20, 20);
        }
        $.each(topologies, function (topologyIndex, topology) {

            var ref = r.topologyRef[topologyIndex];
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

            ref.localaccess = localaccess != null ? localaccess.toUpperCase() : null;

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

                ref.orateoutput = spr + " " + fb[1] + "PS";
                ref.orateoutputbn = outputlimit != null ? outputlimit: null;
            }
            else {
                ref.orateoutput = null;
                ref.orateoutputbn = null;
                ref.speedArea.hide();
            }

            if (rateinput > 0) {
                var rt = rateinput * 1024;
                var fb = center.formatBytes(rt, 10);
                var spt = fb[0] + "";
                var spr = spt.split('.')[0];

                ref.orateinput = spr + " " + fb[1] + "PS";
                ref.orateinputbn = inputlimit != null ? inputlimit : null;
            }
            else {
                ref.orateinput = null;
                ref.orateinputbn = null;
            }

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

            var olac = Number.MAX_VALUE;
            var istim = false;
            $.each(nodeinfo, function (nodeinfoindex, nodeinfovalue) {
                var tim = nodeinfovalue[1];
                if (tim != null) {
                    istim = true;
                    var lac = $$.date(nodeinfovalue[1]).getTime();
                    if (lac < olac) olac = lac;
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



            function setWidth(w) {
                ref.topoCanvas.width(w);
                ref.topoContents.width(w);
            };

            ref.perateinput = null;
            ref.perateoutput = null;
            ref.pepackage = null;

            if (topology != null && topology.length > 0) {
                ref.topoArea.show();

                var g = ref.topoCanvas.paper();
                g.clear();

                var clink = null;
                var clinkstate = null;

                ref.topoContents.removeChildren();

                ui.icon(ref.topoContents, center.icon("topology"))({ top: 7, left: 0, color: 45, size: [16, 16] });

                var left = 22;
                var rightLastMile = false;

                // pi
                var pi = getSection(topology, "PI");
                if (pi != null) {
                    var piNO = ui.text(ref.topoContents)({
                        text: pi[1], top: 3, font: 15, left: left, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = piNO.leftWidth();
                    var piName = ui.text(ref.topoContents)({
                        text: center.formatInterfaceName(pi[8], pi[2]), top: 5, font: 12, left: left + 10, color: pi[10] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = piName.leftWidth();

                    clink = g.rect(left + 10, 11, 15, 5).attr({ stroke: "none", fill: ui.color(pi[11] ? 35 : 75) });
                    left += 25;

                    clinkstate = pi[11];

                    setWidth(left);

                    ref.piid = pi[27];
                    ref.piname = pi[8];
                    ref.pistatus = pi[10];
                    ref.pino = pi[1];

                    if (pi[12] > 0) {
                        var rt = pi[12] * 1024;
                        var fb = center.formatBytes(rt, 10);
                        var spt = fb[0] + "";
                        var spr = spt.split('.')[0];
                        ref.perateinput = spr + " " + fb[1] + "PS";
                    }
                    else {
                        ref.perateinput = null;
                    }
                    if (pi[13] > 0) {
                        var rt = pi[13] * 1024;
                        var fb = center.formatBytes(rt, 10);
                        var spt = fb[0] + "";
                        var spr = spt.split('.')[0];
                        ref.perateoutput = spr + " " + fb[1] + "PS";
                    }
                    else {
                        ref.perateoutput = null;
                    }

                    if (pi[25] != null) {
                        var pkg = pi[25];
                        if (pkg == "6") ref.pepackage = "UNMANAGED";
                        else if (pkg == "7") ref.pepackage = "CUSTOMIZED";
                        else ref.pepackage = pkg;
                    }
                    else ref.pepackage = "UNMANAGED";

                    ref.piid = pi[27];

                    if (topologyIndex == 0) piid = ref.piid;
                }
                else {
                    ref.piid = null;
                    ref.piname = null;
                    ref.pino = null;
                }

                // xpi
                var xpi = getSection(topology, "XPI");
                if (xpi != null) {
                    //1: no
                    //2: pi
                    var xpiNO = ui.text(ref.topoContents)({
                        text: xpi[1], top: 3, left: left, font: 15, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = xpiNO.leftWidth();
                    var xpiName = ui.text(ref.topoContents)({
                        text: xpi[2], top: 5, font: 12, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = xpiName.leftWidth();

                    clink = g.rect(left + 10, 11, 15, 5).attr({ stroke: "none", fill: ui.color(75) });
                    left += 25;
                    clinkstate = true;

                    setWidth(left);

                    var nebox = ui.box(ref.topoContents)({
                        color: 96, size: [69, 22], left: (left - 69) / 2, top: 2, cursor: "default", button: {
                            normal: function () {
                                nebox.animate({ opacity: 1 }, { duration: 50 });
                            },
                            over: function () {
                                nebox.opacity(0);
                            }
                        }
                    });
                    var netxt = ui.text(nebox)({
                        text: "MISSING", left: 5, top: 1, font: 15, noBreak: true
                    });
                }

                var mid = getSection(topology, "MID");
                if (mid != null) {
                    var midNO = ui.text(ref.topoContents)({
                        text: mid[1], left: left, top: 3, font: 15, color: 0, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = midNO.leftWidth();

                    var midName = ui.text(ref.topoContents)({
                        text: center.formatInterfaceName(mid[5], mid[2]), top: 5, font: 12, left: left + 10, color: mid[7] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = midName.leftWidth();

                    setWidth(left);
                }

                // mil
                var mil = getSection(topology, "MIL");
                if (mil != null) {
                    var endlLocal = ui.text(ref.topoContents)({
                        text: "LAST MILE", left: left, top: 5, font: 12, noBreak: true
                    });
                    left = endlLocal.leftWidth();

                    if (mil[1] != null) {
                        g.rect(left + 10, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                        g.rect(left + 10 + 8, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                        g.rect(left + 10 + 16, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                        g.rect(left + 10 + 24, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });

                        left += 40;

                        var end2NO = ui.text(ref.topoContents)({
                            text: mil[1], top: 3, font: 15, left: left + 10, noBreak: true, clickToSelect: true, cursor: "copy"
                        });
                        left = end2NO.leftWidth();

                        var end2NameVar = mil[2];
                        if (end2NameVar != "UNSPECIFIED" && end2NameVar != null) {
                            if (end2NameVar.startsWith("Ex")) end2NameVar = end2NameVar.substr(2);
                            var end2Name = ui.text(ref.topoContents)({
                                text: end2NameVar, top: 5, font: 12, left: left + 10, color: 55, noBreak: true, clickToSelect: true, cursor: "copy"
                            });
                            left = end2Name.leftWidth();
                        }
                    }

                    clink = g.rect(left + 10, 11, 15, 5).attr({ stroke: "none", fill: ui.color(35) });
                    left += 25;
                    clinkstate = true;

                    setWidth(left);
                }

                // mi2
                var mi2 = getSection(topology, "MI2");
                if (mi2 != null) {

                    if (mi2[5] != null) {

                        if (clinkstate == mi2[8]) clink.attr({ width: 30 });
                        else g.rect(left, 11, 15, 5).attr({ stroke: "none", fill: ui.color(mi2[8] ? 35 : 75) });
                        left += 15;

                        var mi2Name = ui.text(ref.topoContents)({
                            text: center.formatInterfaceName(mi2[5], mi2[2]), top: 5, font: 12, left: left + 10, color: mi2[7] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                        });
                        left = mi2Name.leftWidth();

                        //24
                        if (mi2[24] != null && mi2[24] > 1) {
                            var multi = ui.icon(ref.topoContents, center.icon("split"))({
                                size: [20, 20], left: left + 6, top: 3, color: 35, rotation: 90, flip: "V",
                                tooltip: "{0|" + mi2[24] + "} INTERFACES",
                                tooltipSpanColor: ["accent+50"]
                            });
                            left += 20;
                        }
                        left += 10;
                    }

                    var mi2NO = ui.text(ref.topoContents)({
                        text: mi2[1], top: 3, font: 15, left: left, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = mi2NO.leftWidth();

                    setWidth(left);
                }
                else if (pi != null) {

                    var pi2Link;
                    
                    if (clinkstate == pi[11]) {
                        clink.attr({ width: 30 });
                        pi2Link = clink;
                    }
                    else
                        pi2Link = g.rect(left, 11, 15, 5).attr({ stroke: "none", fill: ui.color(pi[11] ? 35 : 75) });
                    left += 15;
                    
                    var pivar = pi[19];
                    if (pivar == "EX") {
                        // 20 21 22
                        var xmi2Name = ui.text(ref.topoContents)({
                            text: center.formatInterfaceName(pi[20], pi[22]), top: 5, font: 12, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                        });
                        var xle = left;
                        left = xmi2Name.leftWidth();
                        var xmi2NO = ui.text(ref.topoContents)({
                            text: pi[21], top: 3, font: 15, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                        });
                        pi2Link.attr({ fill: ui.color(75) });
                        left = xmi2NO.leftWidth();

                        var nebox = ui.box(ref.topoContents)({
                            color: 96, size: [69, 22], left: (left - xle - 69) / 2 + xle, top: 2, cursor: "default", button: {
                                normal: function () {
                                    nebox.animate({ opacity: 1 }, { duration: 50 });
                                },
                                over: function () {
                                    nebox.opacity(0);
                                }
                            }
                        });
                        var netxt = ui.text(nebox)({
                            text: "MISSING", left: 5, top: 1, font: 15, noBreak: true
                        });
                    }
                    else {

                        if (pi[21] != null) {
                            rightLastMile = true;

                            if (pi[20] != "UNSPECIFIED") {
                                var text = pi[20];
                                if (text.startsWith("Ex")) text = text.substr(2);
                                var piEndName = ui.text(ref.topoContents)({
                                    text: text, top: 5, font: 12, left: left + 10, color: mi1[4] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                                });
                                left = piEndName.leftWidth();
                            }
                            var piEndNO = ui.text(ref.topoContents)({
                                text: pi[21], top: 3, font: 15, left: left + 10, noBreak: true, clickToSelect: true, cursor: "copy"
                            });
                            left = piEndNO.leftWidth();

                        }
                        else {
                            var piEnd = ui.text(ref.topoContents)({
                                text: "LAST MILE", top: 5, font: 12, left: left + 10, noBreak: true
                            });
                            left = piEnd.leftWidth();
                        }
                    }

                    setWidth(left);
                }
                else {
                    // xmi2
                    var xmi2 = getSection(topology, "XMI2");
                    if (xmi2 != null) {
                        var xpiName = ui.text(ref.topoContents)({
                            text: "PE", top: 5, font: 12, left: left + 10, color: 75, noBreak: true
                        });
                        left = xpiName.leftWidth();

                        var xpiLink = g.rect(left + 10, 11, 30, 5).attr({ stroke: "none", fill: ui.color(75) });
                        left += 40;

                        var xmi2Name = ui.text(ref.topoContents)({
                            text: "METRO END 2", top: 5, font: 12, left: left + 10, color: 75, noBreak: true
                        });
                        left = xmi2Name.leftWidth();

                        var nebox = ui.box(ref.topoContents)({
                            color: 96, size: [69, 22], left: (left - 69) / 2 + 20, top: 2, cursor: "default", button: {
                                normal: function () {
                                    nebox.animate({ opacity: 1 }, { duration: 50 });
                                },
                                over: function () {
                                    nebox.opacity(0);
                                }
                            }
                        });
                        var netxt = ui.text(nebox)({
                            text: "MISSING", left: 5, top: 1, font: 15, noBreak: true
                        });
                        setWidth(left);
                    }
                }

                var mx = getSection(topology, "MX");
                if (mx != null) {
                    var multi = ui.icon(ref.topoContents, center.icon("split"))({
                        size: [20, 20], left: left + 4, top: 3, color: 35, rotation: 90,
                        tooltip: "{0|" + mx[1] + "} REMOTE PEERS",
                        tooltipSpanColor: ["accent+50"]
                    });
                    left += 20;

                    var linkbox = ui.box(ref.topoContents)({
                        left: left + 10, top: 2, height: 22, color: 50, cursor: "pointer", button: {
                            normal: function () { linkbox.color(50); },
                            click: function () { center.searchExecute("services that bound to VCID " + vcid); },
                            over: function () { linkbox.color(60); }
                        }
                    });

                    var cloudsid = ui.text(linkbox)({
                        left: 10, top: 2, font: 12, text: "CLOUD METRO VCID " + vcid, color: 100, noBreak: true
                    });

                    linkbox.width(cloudsid.width() + 20);

                    left = linkbox.leftWidth();

                    setWidth(left);
                }

                // mc
                var rightcloud = null;
                var rightcloudsid = null;
                var mc = getSection(topology, "MC");
                if (mc != null) {

                    if (mc[17] != null) {
                        rightcloud = g.rect(left + 25, 2, 0, 22).attr({ stroke: "none", fill: null });
                        rightcloudsid = mc[17];
                    }
                    

                    var mpLink1 = g.rect(left + 10, 11, 15, 5).attr({ stroke: "none", fill: ui.color(mc[6] ? 35 : 75) });
                    left += 25;

                    if (mc[9] == mc[6]) {
                        mpLink1.attr({ width: 30 });
                    }
                    else g.rect(left, 11, 15, 5).attr({ stroke: "none", fill: ui.color(mc[9] ? 35 : 75) });
                    left += 15;

                    if (mc[18] != null && mc[18] > 1) {
                        var multi = ui.icon(ref.topoContents, center.icon("split"))({
                            size: [20, 20], left: left + 4, top: 3, color: 35, rotation: 90, flip: "V",
                            tooltip: "{0|" + mc[18] + "} REMOTE PEERS",
                            tooltipSpanColor: ["accent+50"]
                        });
                        left += 20;
                    }

                    var mi1NO = ui.text(ref.topoContents)({
                        text: mc[1], top: 3, font: 15, left: left + 10, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = mi1NO.leftWidth();

                    setWidth(left);
                }

                var xmc = getSection(topology, "XMC");
                if (xmc != null) {
                    g.rect(left + 10, 11, 30, 5).attr({ stroke: "none", fill: ui.color(75) });
                    var xleft = left;
                    left += 40;

                    var xmi1NO = ui.text(ref.topoContents)({
                        text: xmc[1], top: 3, font: 15, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = xmi1NO.leftWidth();

                    g.rect(left + 10, 11, 30, 5).attr({ stroke: "none", fill: ui.color(75) });
                    left += 40;

                    var end1Local = ui.text(ref.topoContents)({
                        text: "LAST MILE", top: 5, font: 12, left: left + 10, color: 75, noBreak: true
                    });

                    left = end1Local.leftWidth();

                    var nebox = ui.box(ref.topoContents)({
                        color: 96, size: [69, 22], left: (left + xleft - 69) / 2, top: 2, cursor: "default", button: {
                            normal: function () {
                                nebox.animate({ opacity: 1 }, { duration: 50 });
                            },
                            over: function () {
                                nebox.opacity(0);
                            }
                        }
                    });
                    var netxt = ui.text(nebox)({
                        text: "MISSING", left: 5, top: 1, font: 15, noBreak: true
                    });

                    setWidth(left);
                }

                // mi1
                var mi1 = getSection(topology, "MI1");
                if (mi1 != null) {

                    if (mi1[16] != null) {
                        rightcloud = g.rect(left + 5, 2, 0, 22).attr({ stroke: "none", fill: null });
                        rightcloudsid = mi1[16];
                    }


                    var ntype;
                    if (mc != null) ntype = mc[2];
                    else ntype = mi2[2];

                    var mi1Name = ui.text(ref.topoContents)({
                        text: center.formatInterfaceName(mi1[1], ntype), top: 5, font: 12, left: left + 10, color: mi1[3] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = mi1Name.leftWidth();

                    var mi1Link = g.rect(left + 10, 11, 30, 5).attr({ stroke: "none", fill: ui.color(mi1[4] ? 35 : 75) });
                    left += 40;

                    var end1var = mi1[13];
                    var end1NO;
                    if (end1var == null) end1NO = "LAST MILE";
                    else {
                        end1NO = end1var;

                        var end1var2 = mi1[17];
                        if (end1var2 != "UNSPECIFIED" && end1var2 != null) {
                            if (end1var2.startsWith("Ex")) end1var2 = end1var2.substr(2);
                            var end1Name = ui.text(ref.topoContents)({
                                text: end1var2, top: 5, font: 12, left: left + 10, color: mi1[4] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                            });
                            left = end1Name.leftWidth();
                        }
                    }

                    var end1 = ui.text(ref.topoContents)({
                        text: end1NO, top: end1var == null ? 5 : 3, font: end1var == null ? 12 : 15, left: left + 10, noBreak: true, clickToSelect: end1var == null ? false : true, cursor: end1var == null ? "" : "copy"
                    });
                    left = end1.leftWidth();

                    if (end1var != null) {
                        rightLastMile = true;
                    }

                    setWidth(left);
                }

                if (rightLastMile) {
                    g.rect(left + 10, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                    g.rect(left + 10 + 8, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                    g.rect(left + 10 + 16, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                    g.rect(left + 10 + 24, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });

                    left += 40;

                    var end1Local = ui.text(ref.topoContents)({
                        text: "LAST MILE", top: 5, font: 12, left: left + 10, noBreak: true
                    });

                    left = end1Local.leftWidth();

                    if (rightcloud != null) {

                        var linkbox = ui.box(ref.topoContents)({
                            left: left + 10, top: 2, height: 22, color: 50, cursor: "pointer", button: {
                                normal: function () { linkbox.color(50); },
                                click: function (e) { center.searchExecute("service that sid is " + rightcloudsid); },
                                over: function () { linkbox.color(60); }
                            }
                        });

                        var cloudsid = ui.text(linkbox)({
                            left: 10, top: 2, font: 12, text: "SID " + rightcloudsid, color: 100, noBreak: true
                        });

                        linkbox.width(cloudsid.width() + 20);

                        rightcloud.attr({ width: left - rightcloud.attr("x") + 10, fill: ui.color(80), opacity: .5 });
                        left = linkbox.leftWidth();
                    }
                }
                setWidth(left + 20);
                ref.topoArea.scrollCalculate();
            }
            else {
                ref.topoArea.hide();
            }
        });

       // p.unload(function () {
        //    debug("unload");
        //});









        // BELOW THIS ARE OBSOLETED


        //-- EXPAND
        r.expanded = false;
        r.expand = function (create, callback) {
            r.expanded = true;

            if (create) {

                function scrollTo(c) {
                    r.content.scrollLeft(c, { duration: 166 });
                };

                r.content = ui.box(b)({ top: 230, height: 250, leftRight: [20, 20], scroll: { type: "button", horizontal: true, vertical: false, 
                    horizontalStep: [371, 1091, 2261, 3431, 4671] } });
                r.overviewtitle = ui.text(b)({ top: 185, left: 20, font: 28, text: "Overview", color: 30, cursor: "default", click: function () { scrollTo(0); } });
                r.analysistitle = ui.text(b)({ top: 185, left: 145, font: 28, text: "Analysis", color: 70, cursor: "pointer", click: function () { scrollTo(1091); } });
                r.sharetitle = ui.text(b)({ top: 185, left: 255, font: 28, text: "Share", color: 70, cursor: "pointer", click: function () { scrollTo(4671); } });

                r.contents = [];

                r.overviewarea = ui.box(r.content)({ topBottom: [10, 0], width: 380, borderTop: { size: 1, color: 80 } });
                r.contents[0] = r.overviewarea;

                ui.text(r.overviewarea)({ position: [0, 15], text: "Last Checked", weight: "600", color: 25, font: 18, tooltipAreaHeight: 55, tooltip: "This date represents the oldest updated network element in the current topology." });
                r.lastchecked = ui.text(r.overviewarea)({ position: [0, 45], color: 0, noBreak: true });
                ui.text(r.overviewarea)({ position: [0, 90], text: "Local Access", weight: "600", color: 25, font: 18 });
                r.localaccess = ui.text(r.overviewarea)({ position: [0, 120], color: 0, width: 380, lineHeight: 25 });

                r.overviewdetarea = ui.box(r.content)({ topBottom: [10, 0], left: 410, width: 690, borderTop: { size: 1, color: 80 } });
                r.contents[1] = r.overviewdetarea;

                r.rateinputtitle = ui.text(r.overviewdetarea)({ position: [0, 15], text: "Overall Input Rate", weight: "600", color: 25, font: 18, noBreak: true });
                r.rateinputicond = ui.icon(r.overviewdetarea, center.icon("upload"))({ size: [30, 30], position: [-2, 48], color: 40, });
                r.rateinput = ui.text(r.overviewdetarea)({ position: [35, 45], font: 25, noBreak: true });

                r.rateoutputtitle = ui.text(r.overviewdetarea)({ position: [0, 110], text: "Overall Output Rate", weight: "600", color: 25, font: 18, noBreak: true });
                r.rateoutputicond = ui.icon(r.overviewdetarea, center.icon("download"))({ size: [30, 30], position: [-2, 143], color: 40, });
                r.rateoutput = ui.text(r.overviewdetarea)({ position: [35, 140], font: 25, noBreak: true });

                r.perateinputtitle = ui.text(r.overviewdetarea)({ position: [240, 15], text: "PE Router Input Rate", weight: "600", color: 25, font: 18, noBreak: true });
                r.perateinputicon = ui.icon(r.overviewdetarea, center.icon("boxin"))({ size: [30, 30], position: [238, 48], color: 40, });
                r.perateinput = ui.text(r.overviewdetarea)({ position: [275, 45], font: 25, noBreak: true });

                r.perateoutputtitle = ui.text(r.overviewdetarea)({ position: [240, 110], text: "PE Router Output Rate", weight: "600", color: 25, font: 18, noBreak: true });
                r.perateoutputicon = ui.icon(r.overviewdetarea, center.icon("boxout"))({ size: [30, 30], position: [238, 143], color: 40, });
                r.perateoutput = ui.text(r.overviewdetarea)({ position: [275, 140], font: 25, noBreak: true });

                r.pepackagetitle = ui.text(r.overviewdetarea)({ position: [480, 15], text: "Service Package", weight: "600", color: 25, font: 18, noBreak: true });
                r.pepackageicon = ui.icon(r.overviewdetarea, center.icon("boxsel"))({ size: [30, 30], position: [478, 48], color: 40, });
                r.pepackage = ui.text(r.overviewdetarea)({ position: [515, 45], font: 25, noBreak: true });

                // ping
                r.pingarea = ui.box(r.content)({ top: 10, left: 1200, width: 1070, height: 240 });
                r.contents[2] = r.pingarea;

                r.pinghistory = ui.box(r.pingarea)({ width: 230, height: 240, borderRight: { size: 1, color: 80 }, scroll: true });

                r.pingtitlelabel = ui.text(r.pingarea)({ font: 15, text: "TITLE", position: [353, 9] });
                r.pingtitleinput = ui.textinput(r.pingarea)({ font: 23, weight: "300", position: [400, 2], width: 500, color: 20 });

                r.pingtargetlabel = ui.text(r.pingarea)({ font: 15, text: "END 2", position: [346, 56] });
                r.pingtargettext = ui.text(r.pingarea)({ font: 23, position: [400, 49], spanColor: [[30], [70]] });
                r.pingtargetwarn = ui.icon(r.pingarea, center.icon("warning"))({ size: 30, attach: [r.pingtargettext, "right", 15, 2], color: 0 });

                r.pingvrflabel = ui.text(r.pingarea)({ font: 15, text: "VRF", position: [362, 103] });
                r.pingvrftext = ui.text(r.pingarea)({ font: 23, position: [400, 96] });
                r.pingvrfwarn = ui.icon(r.pingarea, center.icon("warning"))({ size: 30, attach: [r.pingvrftext, "right", 15, 2], color: 0 });

                r.pingsoulabel = ui.text(r.pingarea)({ font: 15, text: "IP ADDRESS", position: [700, 103] });
                r.pingsoutext = ui.text(r.pingarea)({ font: 23, position: [793, 96], color: 20 });
                r.pingsouwarn = ui.icon(r.pingarea, center.icon("warning"))({ size: 30, attach: [r.pingsoutext, "right", 15, 2], color: 0 });

                r.pingdestlabel = ui.text(r.pingarea)({ font: 15, text: "PING TO", position: [331, 150] });
                r.pingdestinput = ui.textinput(r.pingarea)({ font: 23, weight: "300", position: [400, 143], width: 180, maxlength: 15, color: 20 });

                r.pingexec = ui.button(r.pingarea)({ text: "PING", left: 400, bottom: 10, width: 200 });
                r.pingexec.click(function () {
                    debug("executing...");
                    r.pingexec.disable();
                    $$.post(50005, { pi: ref.piid, ip: r.pingdestinput.value(), t: r.pingtitleinput.value() });
                });

                // routing
                r.routearea = ui.box(r.content)({ top: 10, left: 2370, width: 1070, height: 100 });
                r.contents[3] = r.routearea;

                // routing
                r.showconfigarea = ui.box(r.content)({ top: 10, left: 3540, width: 1070, height: 100 });
                r.contents[4] = r.showconfigarea;

                // share
                r.sharearea = ui.box(r.content)({ topBottom: [10, 0], left: 4710, width: 1070, borderTop: { size: 1, color: 80 } });
                r.contents[5] = r.sharearea;

                ui.text(r.sharearea)({ position: [0, 15], text: "", weight: "600", color: 25, font: 18 });


                r.analysisoffline = ui.box(b)({ top: 240, left: 130, hide: true, width: 700, height: 300 });
                ui.icon(r.analysisoffline, "powercut")({ size: 80, color: 50, position: [10, 10] });
                
                var analysismenuexpand = false;
                var analysismenuexpandid = null;

                r.analysismenu = ui.box(r.content)({
                    top: 10, width: 40, height: 120, hide: true, z: 6000, over: function (e) {
                        if (analysismenuexpand == false && this.x() == 0) {
                            analysismenuexpand = true;
                            analysismenuexpandid = setTimeout(function () {
                                r.analysismenu.width(400, { duration: 100 });
                            }, 500);
                        }
                        else e.stopPropagation();
                    }, leave: function () {
                        clearTimeout(analysismenuexpandid);
                        analysismenuexpand = false;
                        r.analysismenu.width(40, { duration: 100 });
                    }
                });

                var analysisbutton = {
                    normal: function () {
                        this.color(97, 0.9);
                    },
                    over: function () {
                        this.color(90, 0.9);
                    }
                };

                var boxes = [];
                boxes[0] = ui.box(r.analysismenu)({
                    width: "100%", height: 40, color: [40, 0.9], top: 0
                });
                boxes[1] = ui.box(r.analysismenu)({
                    width: "100%", top: 0, color: [97, 0.9], height: 0, cursor: "pointer"
                });
                boxes[2] = ui.box(r.analysismenu)({
                    width: "100%", bottom: 0, color: [97, 0.9], height: 80, cursor: "pointer"
                });

                $.each(boxes, function (bi, bv) {
                    var ping = ui.box(bv)({ top: 0, height: 40, width: "100%" });
                    if (bi != 0) {
                        ping.button(analysisbutton);
                        ping.click(function () { scrollTo(1091); });
                    }
                    ui.icon(ping, center.icon("excl"))({ size: 15, color: bi == 0 ? 100 : 25, position: [7, 12] });
                    ui.icon(ping, center.icon("excl"))({ size: 15, color: bi == 0 ? 100 : 25, position: [13, 12] });
                    ui.icon(ping, center.icon("excl"))({ size: 15, color: bi == 0 ? 100 : 25, position: [19, 12] });
                    ui.text(ping)({ position: [50, 4], noSelect: true, text: "Ping Test", color: bi == 0 ? 100 : 30, font: 20, noBreak: true });

                    var mac = ui.box(bv)({ top: 40, height: 40, width: "100%" });
                    if (bi != 0) {
                        mac.button(analysisbutton);
                        mac.click(function () { scrollTo(2261); });
                    }
                    ui.icon(mac, center.icon("route"))({ size: 25, color: bi == 0 ? 100 : 25, position: [7, 8] });
                    ui.text(mac)({ position: [50, 4], noSelect: true, text: "Display Routing", color: bi == 0 ? 100 : 30, font: 20, noBreak: true });

                    var run = ui.box(bv)({ top: 80, height: 40, width: "100%" });
                    if (bi != 0) {
                        run.button(analysisbutton);
                        run.click(function () { scrollTo(3431); });
                    }
                    ui.icon(run, center.icon("gear"))({ size: 30, color: bi == 0 ? 100 : 30, position: [5, 5] });
                    ui.text(run)({ position: [50, 4], noSelect: true, text: "Display Running Configuration", color: bi == 0 ? 100 : 30, font: 20, noBreak: true });
                });

                function scrollColor(current, prevuntil, main, until, nextmain) {

                    if (main <= current && current < until) return 0;
                    else if (nextmain <= current || current < prevuntil) return 100;
                    else if (until <= current && current < nextmain) {
                        var span = nextmain - until;
                        var untilcurrent = current - until;
                        var dived = (untilcurrent / span) * 100;
                        return dived;
                    }
                    else if (prevuntil <= current && current < main) {
                        var span = main - prevuntil;
                        var untilcurrent = current - prevuntil;
                        var dived = (untilcurrent / span) * 100;
                        return 100 - dived;
                    }

                }

                r.content.scroll(function (o) {
                    var l = o.left;

                    //debug(l);

                    var c1 = scrollColor(l, 0, 0, 691, 1091);
                    r.overviewtitle({ cursor: c1 == 0 ? "default" : "pointer", color: (c1 / 100 * 40) + 30 });
                    var c2 = scrollColor(l, 691, 1091, 3431, 4671);
                    r.analysistitle({ cursor: c2 == 0 ? "default" : "pointer", color: (c2 / 100 * 40) + 30 });
                    var c3 = scrollColor(l, 3431, 4671, 6000, 7000);
                    r.sharetitle({ curspr: c3 == 0 ? "default" : "pointer", color: (c3 / 100 * 40) + 30 });

                    if (4601 <= l) {
                        r.analysismenu({ show: true });
                        r.analysismenu.$.css({ left: 4646, opacity: 1 });

                        boxes[0].top(80);
                        boxes[0].$.scrollTop(80);
                        boxes[1].height(80);
                        boxes[2].height(0);
                        boxes[2].$.scrollTop(120);

                    }
                    else if (1091 <= l) {
                        r.analysismenu({ show: true });
                        r.analysismenu.$.css({ left: l + 44, opacity: 1 });

                        var b0t, b0s, b1h, b2h, b2s;

                        if (3431 <= l) {
                            b0t = 80; b0s = 80; b1h = 80; b2h = 0; b2s = 120;
                        }
                        else if (2931 <= l) {
                            var sel = Math.round(((l - 2931) / 500) * 40);
                            b0t = 40 + sel; b0s = b0t; b1h = 40 + sel; b2h = 40 - sel; b2s = 80 + sel;
                        }
                        else if (2261 <= l) {
                            b0t = 40; b0s = 40; b1h = 40; b2h = 40; b2s = 80;
                        }
                        else if (1761 <= l) {
                            var sel = Math.round(((l - 1761) / 500) * 40);
                            b0t = sel; b0s = b0t; b1h = sel; b2h = 80 - sel; b2s = 40 + sel;
                        }
                        else {
                            b0t = 0; b0s = 0; b1h = 0; b2h = 80; b2s = 40;
                        }

                        boxes[0].top(b0t);
                        boxes[0].$.scrollTop(b0s);
                        boxes[1].height(b1h);
                        boxes[2].height(b2h);
                        boxes[2].$.scrollTop(b2s);

                        //if (!f.isNecrowOnline()) r.analysisoffline.show();
                        //r.analysisoffline.$.css({ x: 0, opacity: 1 });
                    }
                    else if (691 <= l) {
                        r.analysismenu({ show: true });
                        r.analysismenu.$.css({ opacity: (l - 691) / (1091 - 691), left: 1135 });

                        //if (!f.isNecrowOnline()) r.analysisoffline.show();
                        //r.analysisoffline.$.css({ x: 1091 - l, opacity: (l - 691) / (1091 - 691) });
                    }
                    else {
                        r.analysismenu.hide();
                        r.analysisoffline.hide();
                    }
                });

                $$($$.deltaperfdom(), function () {
                    callback();
                });
            }
            else callback();

            var ref = r.topologyRef[r.index];
            
            r.lastchecked.text(ref.lastchecked);
            r.localaccess({ text: ref.localaccess == null ? "NONE" : ref.localaccess, color: ref.localaccess == null ? 70 : 0 });
            r.rateinput({ text: ref.orateinput == null ? "NO LIMIT" : ref.orateinput, color: ref.orateinput == null ? 70 : 20 });
            r.rateoutput({ text: ref.orateoutput == null ? "NO LIMIT" : ref.orateoutput, color: ref.orateoutput == null ? 70 : 20 });

            if (setype != "ME" && setype != "ID") {
                r.perateinputtitle.show();
                r.perateinputicon.show();
                r.perateinput({ show: true, text: ref.perateinput == null ? "NO LIMIT" : ref.perateinput, color: ref.perateinput == null ? 70 : 20 });

                r.perateoutputtitle.show();
                r.perateoutputicon.show();
                r.perateoutput({ show: true, text: ref.perateoutput == null ? "NO LIMIT" : ref.perateoutput, color: ref.perateoutput == null ? 70 : 20 });

                r.pepackagetitle.show();
                r.pepackageicon.show();
                r.pepackage({ show: true, text: ref.pepackage == null ? "NONE" : ref.pepackage, color: ref.pepackage == null ? 80 : 20 });
            }
            else {
                r.perateinputtitle.hide();
                r.perateinputicon.hide();
                r.perateinput.hide();

                r.perateoutputtitle.hide();
                r.perateoutputicon.hide();
                r.perateoutput.hide();

                r.pepackagetitle.hide();
                r.pepackageicon.hide();
                r.pepackage.hide();
            }
            
            r.pinghistory.removeChildren();

            var pfocusp = null;
            var pfocus = ui.box(r.pinghistory)({ width: "100%", height: 85, top: 0, color: 93 });
            var pbutton = {
                normal: function () { this.color(null, { duration: 100 }); },
                over: function () { if (this.top() != pfocus.top()) this.color(97); },
                press: function () { if (this.top() != pfocus.top()) this.color(98); },
                click: function() {
                    if (this.top() != pfocus.top()) {
                        var ct = pfocus.top();
                        var tt = this.top();
                        if (Math.abs(tt - ct) > 150) {
                            if (ct < tt) pfocus.top(tt - 150);
                            else pfocus.top(tt + 150);
                        }
                        pfocus.top(this.top(), { duration: 100 });
                        this({ color: [null, { duration: 100 }], cursor: "default" });
                        pfocusp({ cursor: "pointer" });
                        pfocusp = this;
                    }
                }
            };
            var pnew = ui.box(r.pinghistory)({ width: "100%", height: 85, top: 0, cursor: "default", button: pbutton });
            pfocusp = pnew;
            ui.text(pnew)({ text: "New Ping Test", top: 12, weight: "300", color: 25, font: 23, left: 20 });
            $$.for(0, 0, function (i) {
                var par = ui.box(r.pinghistory)({ width: "100%", height: 85, top: i * 85 + 85, cursor: "pointer", button: pbutton });
                var tp = ui.text(par)({ text: "Ping Test 5", top: 12, weight: "300", color: 25, font: 23, leftRight: [20, 20], truncate: true });
                var td = ui.text(par)({ text: "22/12/2015 8:16:32", position: [20, 45], noBreak: true });
            });

            var warned = false;

            if (ref.piname != null) {
                r.pingtargettext({ text: ref.pino + " {" + (ref.pistatus ? "0" : "1") + "|" + ref.piname + "}", color: 20 });
                if (!ref.pistatus) {
                    r.pingtargetwarn.show();
                    warned = true;
                }
                else r.pingtargetwarn.hide();
            }
            else {
                r.pingtargettext({ text: "UNAVAILABLE", color: 70 });
                r.pingtargetwarn.show();
                warned = true;
            }

            if (vrfnames[r.index] != null) {
                r.pingvrftext({ text: vrfnames[r.index], color: 20 });
                r.pingvrfwarn.hide();
            }
            else {
                r.pingvrftext({ text: "UNAVAILABLE", color: 70 });
                r.pingvrfwarn.show();
                warned = true;
            }

            if (ips[r.index] != null) {                
                r.pingsoutext({ text: ips[r.index], color: 20 });
                r.pingsouwarn.hide();
            }
            else {
                r.pingsoutext({ text: "UNAVAILABLE", color: 70 });
                r.pingsouwarn.show();
                warned = true;
            }

            r.pingtitleinput.value("Ping Test " + (f.column("AnPingHistoryCount") + 1));
            r.pingdestinput.value(f.column("AnPingDefaultDest")[r.index]);

            if (warned)
                r.pingexec.disable();
            else
                r.pingexec.enable();

            //if (!f.isNecrowOnline()) r.pingarea({ opacity: 0 });
            //else r.pingarea({ opacity: 1 });

            r.pingarea.hide();

            r.content.scrollLeft(0);

            //callback();
        };

        //-- CHANGE TOPOLOGY
        r.change = function (index, create) {
            var oldindex = r.index;
            r.index = index;

            var ref = r.topologyRef[r.index];

            if (r.expanded) {

                var adir = "up";
                if (oldindex > index) adir = "down";

                r.lastchecked.text(ref.lastchecked, { duration: 200, distance: 20, slide: adir });
                r.rateinput({ text: [ref.orateinput == null ? "NO LIMIT" : ref.orateinput, { duration: 200, distance: 20, slide: adir }], color: ref.orateinput == null ? 70 : 20 });
                r.rateoutput({ text: [ref.orateoutput == null ? "NO LIMIT" : ref.orateoutput, { duration: 200, distance: 20, slide: adir }], color: ref.orateoutput == null ? 70 : 20 });

                var warned = false;
                var targetwarnshow = function () {
                    $$(100, function () {
                        r.pingtargetwarn({ show: true, x: 20, opacity: 0 });
                        r.pingtargetwarn.$.animate({ x: 0, opacity: 1 }, { duration: 100 });
                    });
                    warned = true;
                };

                if (ref.piname != null) {                    
                    if (ref.pistatus == false && !r.pingtargetwarn.isShown()) targetwarnshow();
                    else if (ref.pistatus == true && r.pingtargetwarn.isShown()) {
                        r.pingtargetwarn.$.animate({ x: 20, opacity: 0 }, { duration: 100, complete: function () { r.pingtargetwarn.hide(); } });
                    }
                    r.pingtargettext({ text: [ref.pino + " {" + (ref.pistatus ? "0" : "1") + "|" + ref.piname + "}", { duration: 200, distance: 20, slide: adir }] });
                    r.perateinput({ text: [ref.perateinput == null ? "NO LIMIT" : ref.perateinput, { duration: 200, distance: 20, slide: adir }], color: ref.perateinput == null ? 70 : 20 });
                    r.perateoutput({ text: [ref.perateoutput == null ? "NO LIMIT" : ref.perateoutput, { duration: 200, distance: 20, slide: adir }], color: ref.perateoutput == null ? 70 : 20 });
                    r.pepackage({ text: [ref.pepackage, { duration: 200, distance: 20, slide: adir }] });
                }
                else {
                    if (!r.pingtargetwarn.isShown()) targetwarnshow();
                    r.pingtargettext({ text: "UNAVAILABLE", color: 70 });
                    // TODO for perateinput etc..
                }

                if (vrfnames[r.index] != null) {
                    r.pingvrftext({ text: [vrfnames[r.index], { duration: 200, distance: 20, slide: adir }], color: 20 });
                    if (r.pingvrfwarn.isShown()) {
                        r.pingvrfwarn.$.animate({ x: 20, opacity: 0 }, { duration: 100, complete: function () { r.pingvrfwarn.hide(); } });
                    }
                }
                else {
                    r.pingvrftext({ text: ["UNAVAILABLE", { duration: 200, distance: 20, slide: adir }], color: 70 });
                    if (!r.pingvrfwarn.isShown()) {
                        $$(100, function () {
                            r.pingvrfwarn({ show: true, x: 20, opacity: 0 });
                            r.pingvrfwarn.$.animate({ x: 0, opacity: 1 }, { duration: 100 });
                        });
                    }
                    warned = true;
                }

                if (ips[r.index] != null) {
                    r.pingsoutext({ text: [ips[r.index], { duration: 200, distance: 20, slide: adir }], color: 20 });
                    if (r.pingsouwarn.isShown()) {
                        r.pingsouwarn.$.animate({ x: 20, opacity: 0 }, { duration: 100, complete: function () { r.pingsouwarn.hide(); } });
                    }
                }
                else {
                    r.pingsoutext({ text: ["UNAVAILABLE", { duration: 200, distance: 20, slide: adir }], color: 70 });
                    if (!r.pingsouwarn.isShown()) {
                        $$(100, function () {
                            r.pingsouwarn({ show: true, x: 20, opacity: 0 });
                            r.pingsouwarn.$.animate({ x: 0, opacity: 1 }, { duration: 100 });
                        });
                    }
                    warned = true;
                }

                r.pingdestinput.value(f.column("AnPingDefaultDest")[r.index]);


                if (warned)
                    r.pingexec.disable();
                else
                    r.pingexec.enable();
            }
        }

        //-- STREAM
        f.stream("service_" + streamSeID, function (data) {
            debug("incoming data: " + data);
        });
        
        r.necrow = function (type, data) {
            
            if (type == "necrow") {
                if (r.content != null) {
                    if (r.content.scrollLeft() >= 691) {
                        if (data == true) {
                            r.analysisoffline.hide();
                        }
                        else {
                            r.analysisoffline.show();
                        }
                    }

                    if (data == true) {
                        r.pingarea({ opacity: 1 });
                    }
                    else {
                        r.pingarea({ opacity: 0 });
                    }
                }
            }
        };
                     
        r.topologyRef[0].pingdata = [];
        r.topologyRef[1].pingdata = [];

        r.topologyRef[0].pingdataindex = -1;
        r.topologyRef[1].pingdataindex = -1;

        var pinggraphline = null;
        var pinggraphfill = null;

        function redrawpinggraph() {
            var w = r.pigraph.width();
            var h = r.pigraph.height();
            var aw = w;
            var ah = h - 25;
            var gb = r.pigraph.paper();

            gb.clear();

            var dat = pingdata[pingdataindex];
            var pco = dat.count;

            // line and fill
            pinggraphfill = gb.path("").attr({ stroke: "none", fill: ui.color(97) });
            pinggraphline = gb.path("").attr({ stroke: ui.color(50) });

            // prop
            gb.rect(0, ah, aw, 1).attr({ stroke: "none", fill: ui.color(75) });
            var rs = aw / (pco - 1);
            while (rs < 50) { pco = Math.round(pco / 2); rs = aw / (pco); }
            var dit = dat.count / pco;
            var ox = 0;
            var odit = (pco == dat.count) ? 2 : dit;
            $$.while(function () { return ox < aw }, function () {
                ox += rs;
                if (ox < aw)
                    gb.rect(ox, ah - 5, 2, 6).attr({ stroke: "none", fill: ui.color(75) });

                if (odit > 0 && odit < dat.count) {
                    var rdit = Math.round(odit);
                    var oxo = ui.text(r.pigraphbox)({ color: 50, text: rdit + "", bottom: 0 });
                    oxo.left(ox - (oxo.textSize().width / 2));
                }
                odit += dit;
            });
        };
        function redrawpinggraphline() {
            var w = r.pigraph.width();
            var h = r.pigraph.height();
            var aw = w;
            var ah = h - 25;
            var gb = r.pigraph.paper();

            var dat = pingdata[pingdataindex];
            var pco = dat.count;

            // line and fill
            var nw = w / (pco - 1);
            var min = Number.MAX_SAFE_INTEGER;
            var max = 0;
            $.each(dat.ping, function (pi, pv) {
                if (pv != null) {
                    if (pv < min) min = pv;
                    if (pv > max) max = pv;
                }
            });
            var range = max - min;
            if (range == 0) range = 20;
            var rrange = ah - 80 - 40;
            var omv = rrange / range;

            var pathLINE = "";
            var pathFILL = "";
            var lpt = 0;

            var prevnull = true;
            var previx = 0;
            $.each(dat.ping, function (pi, pv) {
                var ix = Math.round(nw * pi);
                if (pv != null) {
                    if (prevnull) {
                        pathLINE += "M";
                        pathFILL += "L" + ix + "," + ah;
                    }
                    else pathLINE += "L";
                    var mrt = Math.round(((max - pv) * omv) + 40);
                    pathLINE += "" + ix + "," + mrt;
                    pathFILL += "L" + ix + "," + mrt;
                    prevnull = false;
                }
                else {
                    pathFILL += "L" + previx + "," + ah + "L" + ix + "," + ah;
                    prevnull = true;
                }
                previx = ix;
            });

            if (pathLINE != "") {
                pinggraphfill.attr({ path: "M0," + ah + "" + pathFILL + "L" + previx + "," + ah + "C" });
                pinggraphline.attr({ path: pathLINE });
            }
        };

        r.expandx = function (create) {

            var ref = r.topologyRef[focusedindex];

            if (create) {
                r.overbox = ui.box(r.content)({ height: 42, cursor: "pointer" });
                r.overtext = ui.text(r.overbox)({ font: ["body", 17], text: "OVERVIEW", weight: "600", top: 10, color: 15 });
                r.overbox.width(r.overtext.width());
                r.overbox.button({
                    normal: function () { r.overtext.color(15); }, over: function () { r.overtext.color(55); },
                    click: function () { showcon(0); r.overbox({ cursor: "default", disableButton: true }); r.overtext.color("accent"); }
                });

                r.detbox = ui.box(r.content)({ height: 42, cursor: "pointer" });
                r.dettext = ui.text(r.detbox)({ font: ["body", 17], text: "DETAILS", weight: "600", top: 10, color: 65 });
                r.detbox.width(r.dettext.width());
                r.detbox.button({
                    normal: function () { r.dettext.color(65); },
                    over: function () { return; r.dettext.color(55); },
                    click: function () { return; r.detbox({ cursor: "default", disableButton: true }); r.dettext.color("accent"); }
                });

                r.pingbox = ui.box(r.content)({ height: 42, cursor: "pointer" });
                r.pingtext = ui.text(r.pingbox)({ font: ["body", 17], text: "ANALYSIS", weight: "600", top: 10, color: 15, noBreak: true });
                r.pingbox.width(r.pingtext.width());
                r.pingbox.button({
                    normal: function () { r.pingtext.color(15); }, over: function () { r.pingtext.color(55); },
                    click: function () { showcon(2); r.pingbox({ cursor: "default", disableButton: true }); r.pingtext.color("accent"); }
                });

                r.sepcon = ui.box(r.content)({ top: 42, height: 1, color: 85, leftRight: [0, 0] });

                r.overcon = ui.box(r.content)({ topBottom: [44, 0], width: "100%" });
                r.detcon = ui.box(r.content)({ topBottom: [44, 0], width: "100%", hide: true });
                r.pingcon = ui.box(r.content)({ topBottom: [44, 0], width: "100%", hide: true });

                var ileft = 0;

                ileft = r.overbox.leftWidth();
                r.detbox.left(ileft + 20);
                ileft = r.detbox.leftWidth();
                r.pingbox.left(ileft + 20);
                ileft = r.pingbox.leftWidth();

                // -- overviewS

                r.tlu = ui.text(r.overcon)({ position: [20, 20], text: "Last Checked", weight: "600", color: 25, font: 18 });
                r.tlut = ui.text(r.overcon)({ position: [20, 50], color: 0, noBreak: true });

                r.e1sa = ui.text(r.overcon)({ position: [20, 95], text: "Local Access", weight: "600", color: 25, font: 18 });
                r.e1sat = ui.text(r.overcon)({ position: [20, 125], spanColor: [[75]], width: 380, text: "SLIPI, WITEL JAKARTA BARAT, DIVISI REGIONAL 2", color: 0 });

                r.ira = ui.text(r.overcon)({ position: [430, 20], weight: "600", color: 25, font: 18 });
                r.irat = ui.text(r.overcon)({ position: [430, 50], font: 25, color: 40, noBreak: true, spanColor: [[75]] });
                r.irl = ui.text(r.overcon)({ position: [430, 90], font: 15, color: 60 });

                r.ora = ui.text(r.overcon)({ position: [680, 20], weight: "600", color: 25, font: 18 });
                r.orat = ui.text(r.overcon)({ position: [680, 50], font: 25, color: 40, noBreak: true, spanColor: [[75]] });
                r.orl = ui.text(r.overcon)({ position: [680, 90], font: 15, color: 60 });

                r.peir = ui.text(r.overcon)({ position: [430, 135], text: "PE Input Rate", weight: "600", color: 25, font: 18 });
                r.peirt = ui.text(r.overcon)({ position: [430, 165], color: 0, noBreak: true, spanColor: [[75]] });
                r.peor = ui.text(r.overcon)({ position: [680, 135], text: "PE Output Rate", weight: "600", color: 25, font: 18 });
                r.peort = ui.text(r.overcon)({ position: [680, 165], color: 0, noBreak: true, spanColor: [[75]] });
                r.peox = ui.text(r.overcon)({ position: [430, 210], text: "Service Package", weight: "600", color: 25, font: 18 });
                r.peoxt = ui.text(r.overcon)({ position: [430, 240], color: 0, noBreak: true });

                r.pigraphbox = ui.box(r.pingcon)({ position: [0, 20], width: 625, height: 210 });
                r.pigraph = ui.raphael(r.pigraphbox)({ size: [625, 210] });

                r.pinotready = ui.box(r.pingcon)({ position: [0, 0], size: ["100%", "100%"], color: 99, z: 50 });

                ui.button(r.pinotready)({
                    position: [20, 20], width: 200, text: "START PING TEST", click: function () { r.pinotready.hide(); }
                });

                ui.text(r.pingcon)({ text: "DESTINATION", font: 15, color: 20, position: [0, 250] });
                var i1 = ui.textinput(r.pingcon)({ font: 22, position: [105, 242], width: 150, textcolor: 20, weight: "600", value: "10.168.73.242", maxlength: 15 });

                ui.text(r.pingcon)({ text: "COUNT", font: 15, color: 20, position: [270, 250] });
                var i2 = ui.textinput(r.pingcon)({ value: "5", font: 22, textcolor: 20, position: [330, 242], weight: "600", width: 60, maxlength: 4 });

                ui.text(r.pingcon)({ text: "SIZE", font: 15, color: 20, position: [405, 250] });
                var i3 = ui.textinput(r.pingcon)({ value: "100", font: 22, textcolor: 20, position: [445, 242], weight: "600", width: 60, maxlength: 4 });

                ui.button(r.pingcon)({
                    position: [520, 240], width: 105, text: "PING", click: function () {
                        i1.readonly();
                        i2.readonly();
                        i3.readonly();
                        $$.post(50005, { pi: piid, ip: i1.value() }, function (d) {
                            debug(d.data);
                        });
                    }
                });

                r.piofflinec = ui.box(r.pingcon)({ position: [0, 0], size: ["100%", "100%"], color: 99, opacity: 0.90, z: 100 });
                r.piofflinet = ui.text(r.pingcon)({ position: [20, 20], text: "We are sorry, the service is not currently available. Please try again later.", z: 101, font: 17 });

                r.stream = function (type, data) {
                    if (type == "necrow") {
                        if (r.pigraphbox != null) {
                            if (f.isNecrowOnline()) {
                                r.piofflinec.hide();
                                r.piofflinet.hide();
                            }
                            else {
                                r.piofflinec.show();
                                r.piofflinet.show();
                            }
                        }
                    }
                };
            }


            if (setype != "ME") {
                r.ira.text("End-to-end Input Rate");
                r.ora.text("End-to-end Output Rate");
                r.pingbox.show();
            }
            else {
                if (sesubtype == "PP") {
                    r.ira.text("End-to-end Input Rate");
                    r.ora.text("End-to-end Output Rate");
                }
                else {
                    r.ira.text("Service Input Rate");
                    r.ora.text("Service Output Rate");
                }
                r.pingbox.hide();
            }



            r.overtext.color("accent");
            r.overbox({ cursor: "default", disableButton: true });
            r.dettext.color(65);
            r.detbox({ cursor: "default", enableButton: true });
            r.pingtext.color(15);
            r.pingbox({ cursor: "pointer", enableButton: true });

            r.overcon.show();
            r.detcon.hide();
            r.pingcon.hide();

            $.each(r.topologyRef, function (i, v) {
                v.topoLeft.color(99);
                v.topoRight.color(99);
            });

            // ping
            if (r.pigraphbox == null) {

               
            }

            //redrawpinggraph();
            //redrawpinggraphline();

            if (f.isNecrowOnline()) {
                r.piofflinec.hide();
                r.piofflinet.hide();
            }
            else {
                r.piofflinec.show();
                r.piofflinet.show();
            }
        };

        if (true) {
            /*
            r.tlut.text(ref.tlut);
            r.e1sat.text(ref.e1sat);

            r.irat.text(ref.irat);
            r.orat.text(ref.orat);

            if (ref.irl != null) {
                r.irl.show();
                r.irl.text(ref.irl);
            }
            else r.irl.hide();
            if (ref.orl != null) {
                r.orl.show();
                r.orl.text(ref.orl);
            }
            else r.orl.hide();

            if (ref.peirt != null) {
                r.peir.show();
                r.peirt.show();
                r.peirt.text(ref.peirt);
            }
            else {
                r.peir.hide();
                r.peirt.hide();
            }

            if (ref.peort != null) {
                r.peor.show();
                r.peort.show();
                r.peort.text(ref.peort);
            }
            else {
                r.peor.hide();
                r.peort.hide();
            }

            if (ref.peoxt != null) {
                r.peox.show();
                r.peoxt.show();
                r.peoxt.text(ref.peoxt);
            }
            else {
                r.peox.hide();
                r.peoxt.hide();
            }
            */
        }
    });

})();