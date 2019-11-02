
(function () {



    $$.script("search_jovice_service", function (b, r, f, p, t) {

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
        f.setSize(125);

        //--- entry values
        var visualID = f.column("SI_VID");
        var serviceID = f.column("SE_SID");
        var checked = f.column("SI_SE_Check");

        var detail = f.column("SE_Detail");

        var customerName = f.column("SC_Name");
        var setype = f.column("SI_Type");
        var seproduct = f.column("SP_Product");

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

        var orders = f.column("Orders");

        var ipd = f.column("IPD");

        var streamSeID = f.column("StreamServiceID");



        if (f.create) {
            r.serviceID = $$.text(b)({ font: ["body", 15], color: "accent", top: 13, left: 20, weight: "600", noBreak: true, clickToSelect: true, cursor: "copy" });
            r.verified = $$.icon(b, center.icon("verified"))({ attach: [r.serviceID, "right", 5], color: 45, size: [16, 16], tooltip: "This SID is verified" });

            r.serviceType = $$.text(b)({ font: ["body", 15], color: 25, noBreak: true, clickToSelect: true, cursor: "copy", attach: [r.serviceID, "right", 35] });
            r.customerName = $$.text(b)({ font: ["body", 15], color: 25, weight: "600", noBreak: true, truncate: true, attach: [r.serviceType, "right", 20] });
            r.customerDiscovered = $$.icon(b, center.icon("lightning"))({ color: 35, size: [20, 20], attach: [r.customerName, "right", 5], tooltipSpanColor: ["accent+50"], tooltip: "The data in this service are {0|discovered} and not verified" });

            r.orderInfo = "";
            r.orderInfoExpand = "";
            r.orderInfoDetail = "";
            r.orderInfoDetail2 = "";

            r.serviceOrderInformation = $$.box(b)({ color: 85, height: 22, attach: [r.customerName, "right", 20, -2], z: 10 });
            r.serviceOrderInformation({
                leave: function () {
                    r.serviceOrderInformation.stop();
                    r.serviceOrderInformation.height(22, {
                        duration: 100, complete: function () {
                            $$(100, function () {
                                if (!r.serviceOrderInformation.isMouseOver()) {
                                    r.serviceOrderInformationText.text(r.orderInfo);
                                    r.serviceOrderInformation.width(r.serviceOrderInformationText.width() + 20, { duration: 100 });
                                }
                            });
                        }
                    });
                },
                enter: function () {
                    r.serviceOrderInformationText.text(r.orderInfoExpand);
                    r.serviceOrderInformationDetail.text(r.orderInfoDetail);

                    r.serviceOrderInformationWidth = 0;
                    r.serviceOrderInformationHeight = 44;

                    if (r.serviceOrderInformationText.width() > r.serviceOrderInformationWidth) r.serviceOrderInformationWidth = r.serviceOrderInformationText.width();
                    if (r.serviceOrderInformationDetail.width() > r.serviceOrderInformationWidth) r.serviceOrderInformationWidth = r.serviceOrderInformationDetail.width();

                    if (r.orderInfoDetail2 != null) {
                        r.serviceOrderInformationDetail2.width(r.serviceOrderInformationWidth);
                        r.serviceOrderInformationDetail2.text(r.orderInfoDetail2);
                        r.serviceOrderInformationHeight += r.serviceOrderInformationDetail2.height() + 9;
                    }

                    r.serviceOrderInformation.stop();
                    r.serviceOrderInformation.width(r.serviceOrderInformationWidth + 20, {
                        duration: 100, complete: function () {
                            $$(100, function () {
                                if (r.serviceOrderInformation.isMouseOver()) {
                                    r.serviceOrderInformation.height(r.serviceOrderInformationHeight, { duration: 100 });
                                }
                            });
                        }
                    });
                }
            });
            r.serviceOrderInformationText = $$.text(r.serviceOrderInformation)({
                noBreak: true, cursor: "default", left: 10, top: 4, font: 12, text: "SUSPENDED",
                spanColor: ["accent"]
            });
            r.serviceOrderInformationDetail = $$.text(r.serviceOrderInformation)({
                noBreak: true, cursor: "default", left: 10, top: 26, font: 12, text: "DETAIL1",
                spanColor: ["accent"]
            });
            r.serviceOrderInformationDetail2 = $$.text(r.serviceOrderInformation)({
                noBreak: false, cursor: "default", left: 10, top: 48, font: 12, text: "DETAIL2",
                spanColor: ["accent"]
            });

            r.detail = $$.box(b)({ top: 40, height: 40, leftRight: 20 });
            r.serviceDetail = $$.text(r.detail)({ color: 25, font: 12, italic: true, noBreak: true, truncate: true });

            r.serviceInformation = $$.box(b)({ bottom: 0, height: 60, leftRight: 20, });

            r.topologyReferences = [];

        }

        //-- MAIN

        if (serviceID != null) {
            r.serviceID.text(serviceID);
            r.verified.show();
        }
        else {
            r.serviceID.text(visualID);
            r.verified.hide();
        }

        if (detail != null) {
            r.serviceDetail.text(detail);
        }
        else {
            r.serviceDetail.text("");
        }

        if (customerName != null) {

            if (customerName.startsWith("!")) {
                customerName = customerName.substring(1);
                r.customerDiscovered.show();
            }
            else {
                r.customerDiscovered.hide();
            }

            r.customerName.text(customerName);
        }
        else {
            r.customerName.text("");
            r.customerDiscovered.hide();
        }





        
        r.orderInfo = null;
        r.orderInfoExpand = null;
        r.orderInfoDetail = null;
        r.orderInfoDetail2 = null;

        //0 ORDER, 1 DATE, 2 ACTION, 3 STATUS, 4 AM
        //ACTION=  A add, D delete, U update, S suspend, R resume, N none
        //STATUS=  F failed, I inprogress, C complete, L cancelled, A abandoned, S submitted, P pending, G pending cancel, N none

        if (orders.length > 0) {

            var reind = -1;
            $.each(orders, function (oi, ov) {
                if (ov[3].isIn(["C", "I", "S", "P"])) {
                    reind = oi;
                    return false;
                }
            });

            if (reind > -1) {
                if (orders[reind][2] == "D") {

                    var d = $$.date("{DD}/{MM}/{YYYY}", $$.date(orders[reind][1]));

                    if (orders[reind][3].isIn(["I", "S", "P"])) {

                        r.orderInfo = "DELETE IN PROGRESS";
                        r.orderInfoExpand = "DELETE IN PROGRESS SINCE {0|" + d + "}";
                        r.orderInfoDetail = "ORDER: {0BX|" + orders[reind][0] + "}";

                        $.each(orders, function (oi, ov) {
                            if (oi > reind) {
                                if (ov[2] == "S" && ov[3] == "C") {
                                    r.orderInfoDetail2 = "Currently SUSPENDED by order: {0BKX|" + ov[0] + "}";
                                    return false;
                                }
                            }
                        });
                    }
                    else if (orders[reind][3] == "C") {
                        r.orderInfo = "DELETED";
                        r.orderInfoExpand = "DELETED SINCE {0|" + d + "}";
                        r.orderInfoDetail = "ORDER: {0BX|" + orders[reind][0] + "}";
                    }
                }
                else if (orders[reind][2] == "S") {

                    var d = $$.date("{DD}/{MM}/{YYYY}", $$.date(orders[reind][1]));

                    if (orders[reind][3].isIn(["I", "S", "P"])) {

                        r.orderInfo = "SUSPEND IN PROGRESS";
                        r.orderInfoExpand = "SUSPEND IN PROGRESS SINCE {0|" + d + "}";
                        r.orderInfoDetail = "ORDER: {0BX|" + orders[reind][0] + "}";
                    }
                    else if (orders[reind][3] == "C") {
                        r.orderInfo = "SUSPENDED";
                        r.orderInfoExpand = "SUSPENDED SINCE {0|" + d + "}";
                        r.orderInfoDetail = "ORDER: {0BX|" + orders[reind][0] + "}";
                    }
                }
                else if (orders[reind][2] == "A") {

                    var d = $$.date("{DD}/{MM}/{YYYY}", $$.date(orders[reind][1]));

                    if (orders[reind][3].isIn(["I"])) {

                        r.orderInfo = "NEW SERVICE IN PROGRESS";
                        r.orderInfoExpand = "NEW SERVICE IN PROGRESS SINCE {0|" + d + "}";
                        r.orderInfoDetail = "ORDER: {0BX|" + orders[reind][0] + "}";


                    }
                    else if (orders[reind][3] == "C") {

                        var sel = ($$.date() - $$.date(orders[reind][1])) / (1000 * 3600 * 24);

                        if (sel < 60) {

                            r.orderInfo = "NEW SERVICE";
                            r.orderInfoExpand = "NEW SERVICE SINCE {0|" + d + "}";
                            r.orderInfoDetail = "ORDER: {0BX|" + orders[reind][0] + "}";
                            //r.orderInfoDetail2 = "This label is provided for new services less than 2 months old";

                        }
                    }
                }
            }
        }

        if (r.orderInfo != null) {
            r.serviceOrderInformation.show();
            r.serviceOrderInformationText.text(r.orderInfo);
            r.serviceOrderInformation.width(r.serviceOrderInformationText.width() + 20);
        }
        else {
            r.serviceOrderInformation.hide();
        }


        if (setype != null) {
            var sesubtype = f.column("SubType");
            var setypetext = null;

            if (setype == "VP") setypetext = "VPNIP";
            else if (setype == "TA") setypetext = "Trans Access";
            else if (setype == "AS") setypetext = "Astinet";
            else if (setype == "AB") setypetext = "Astinet Beda Bandwidth";
            else if (setype == "VI") setypetext = "VPN Instant";
            else if (setype == "IT") setypetext = "IP Transit";
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
            r.serviceType.color(25);
        }
        else {

            var product = f.column("SP_Product");
            var setypetext = product;

            r.serviceType.text(setypetext);
            r.serviceType.color(60);
        }

        //-- TOPOLOGIES
        if (topologies.length > 0) r.serviceInformation.show();
        else r.serviceInformation.hide();
               
        $.each(topologies, function (topologyIndex, topology) {

            var ref = r.topologyReferences[topologyIndex];

            if (ref == null) {
                r.topologyReferences[topologyIndex] = {};
                ref = r.topologyReferences[topologyIndex];

                ref.purpose = $$.box(b)({
                    hide: true, button: {
                        normal: function () { this.color(50); },
                        over: function () { this.color(60); },
                    },
                    color: 50,
                    height: 22,
                    cursor: "pointer"
                });
                ref.purposeText = $$.text(ref.purpose)({
                    noBreak: true, left: 10, top: 4, font: 12, color: 100
                });

                ref.area = $$.box(r.serviceInformation)({ size: ["100%", 60], top: topologyIndex * 60 });

                ref.speedArea = $$.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: false });
                $$.icon(ref.speedArea, center.icon("speed"))({ top: 2, left: 0, color: 45, size: [16, 16], tooltip: "Minimum speeds that were configured end-to-end" });
                ref.speedText = $$.text(ref.speedArea)({ font: ["body", 13], color: 0, top: 2, left: 22, noBreak: true });

                ref.vrfArea = $$.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                $$.icon(ref.vrfArea, center.icon("cloud"))({ top: 2, left: 0, color: 45, size: [16, 16], tooltip: "VRF/Cloud that was configured on the PE interface" });
                ref.vrfText = $$.text(ref.vrfArea)({ font: ["body", 13], color: 0, top: 2, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });

                ref.ipArea = $$.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                $$.icon(ref.ipArea, center.icon("IP"))({ top: 2, left: 0, color: 45, size: [16, 16], tooltip: "WAN IP that was configured on the PE interface" });
                ref.ipText = $$.text(ref.ipArea)({ font: ["body", 13], color: 0, top: 2, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });

                ref.ceArea = $$.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                ref.ceIcon = $$.icon(ref.ceArea, center.icon("fire"))({ top: 2, left: 0, color: 45, size: [16, 16], tooltip: "First IP/ARP resolution entry on PE" });
                ref.ceIPText = $$.text(ref.ceArea)({ font: ["body", 13], color: 0, top: 2, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });
                ref.ceToIcon = $$.icon(ref.ceArea, center.icon("skipright"))({ top: 3, left: 0, color: 45, size: [14, 14] });
                ref.ceMACText = $$.text(ref.ceArea)({ font: ["body", 13], weight: "500", color: 0, top: 2, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });
                
                ref.vcidArea = $$.box(ref.area)({ cursor: "default", left: 0, top: 0, height: 22, width: 0, hide: true });
                ref.vcidLabel = $$.box(ref.vcidArea)({ top: 0, left: 0, height: 22, width: 20, tooltip: "VCID/SDP label that was configured on ME routes" });
                $$.text(ref.vcidLabel)({ top: 3, left: 4, font: ["body", 6], text: "VC", color: 10 });
                $$.text(ref.vcidLabel)({ top: 9, left: 5, font: ["body", 6], text: "ID", color: 10 });
                ref.vcidText = $$.text(ref.vcidArea)({ font: ["body", 13], color: 0, top: 2, left: 22, noBreak: true, clickToSelect: true, cursor: "copy" });

                ref.routeArea = $$.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                $$.icon(ref.routeArea, center.icon("map"))({ top: 2, left: 0, color: 45, size: [16, 16], tooltip: "Route types that were configured on PE" });
                ref.routeText = $$.text(ref.routeArea)({ font: ["body", 13], color: 0, top: 2, left: 22, noBreak: true });

                ref.updateArea = $$.box(ref.area)({ left: 0, top: 0, height: 22, width: 0, hide: true });
                $$.icon(ref.updateArea, center.icon("time"))({ top: 2, left: 0, color: 45, size: [16, 16], tooltip: "Last update" });
                ref.updateText = $$.text(ref.updateArea)({ font: ["body", 13], color: 0, top: 2, left: 22, noBreak: true });

                ref.topologyArea = $$.box(ref.area)({ leftRight: [0, 0], top: 22, height: 28, hide: true, scroll: { vertical: false, horizontal: true, type: "button" } });
            }

            ref.area.show();
            ref.area.top(topologyIndex * 60);
            ref.purpose.button({
                click: function () {
                    $.each(topologies, function (index) {
                        var iref = r.topologyReferences[index];
                        if (index != topologyIndex) {
                            iref.purpose.enableButton();
                            iref.purpose.color(50);
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

            var sp;

            if (purposes[topologyIndex] == null) sp = "TOPOLOGY " + (topologyIndex + 1);
            else sp = purposes[topologyIndex];

            //debug(customerName + " " + purposes.length);

            ref.purpose.show();
            ref.purposeText.text(sp);
            ref.purpose.width(ref.purposeText.leftWidth() + 10);

            if (topologyIndex == 0) {
                ref.purpose.disableButton();
                ref.purpose.color("accent");
            }
            else {
                ref.purpose.enableButton();
                ref.purpose.color(50);
            }

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
            var arp = ipd[topologyIndex];

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

            if (arp != null && ip != null) {

                var completed = false;

                if (arp.length > 0) {
                    var arps = arp.split(",");
                    var sip = null;
                    var smac = null;

                    var iplocal = "";
                    var iplx = ip.split("/");
                    if (iplx.length == 2) iplocal = iplx[0];

                    $.each(arps, function (ai, av) {

                        var avs = av.split("-");

                        if (avs[0] != iplocal) {
                            completed = true;
                            sip = avs[0];
                            smac = avs[1].toUpperCase();
                            return false;
                        }

                    });
                }

                var clw = 0;

                if (completed == false) {
                    ref.ceArea.show();
                    ref.ceIPText.text("ARP INCOMPLETED");
                    ref.ceIPText.color(50);
                    ref.ceIcon.color(75);

                    ref.ceToIcon.hide();
                    ref.ceMACText.hide();

                    clw = ref.ceIPText.leftWidth();

                }
                else if (sip != null && smac != null)
                {
                    ref.ceArea.show();
                    ref.ceIPText.text(sip);
                    ref.ceIPText.color(0);
                    ref.ceIcon.color(45);

                    ref.ceToIcon.show();
                    ref.ceToIcon.left(ref.ceIPText.leftWidth() + 10);

                    ref.ceMACText.show();
                    ref.ceMACText.text(smac);
                    ref.ceMACText.left(ref.ceToIcon.leftWidth() + 8);

                    clw = ref.ceMACText.leftWidth();

                    if (r.orderInfo == "NEW SERVICE IN PROGRESS") {
                        r.orderInfoDetail2 = "Waiting for order completion";
                    }
                }
                else {
                    ref.ceArea.hide();
                }

                if (clw > 0) {
                    ref.ceArea({ left: lanc, width: clw + 10 });
                    lanc = ref.ceArea.leftWidth() + 20;
                }
            }
            else ref.ceArea.hide();

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

        $.each(r.topologyReferences, function (index, reference) {
            if (index < topologies.length) {
                if (topologies.length > 1) {
                    reference.purpose.show();
                    if (index == 0) reference.purpose.attach(r.serviceType, "right", 20, -2);
                    else reference.purpose.attach(r.topologyReferences[index - 1].purpose, "right", 0);
                }
                else {
                    reference.purpose.hide();
                }
            }
            else {
                reference.area.hide();
                reference.purpose.hide();
            }
        });

        if (topologies.length > 1) {
            r.customerName.attach(r.topologyReferences[topologies.length - 1].purpose, "right", 20);
        }
        else {
            r.customerName.attach(r.serviceType, "right", 20);
        }
    });

})();