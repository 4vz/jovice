(function () {

    var uipage;

    // functions
    var enterSearchResult, setResults, setFilters, clearSearchResult, setRelated;

    // features
    var animTransferedClick;

    var isfiltersexists = false, ispagingexists = false, isnomatchexists = false;
    var necrowonline = false;

    var searchJQXHR;
    var search, columns, results, sortList, sortBy, sortType, page, npage, mpage, count, type, subType, searchid = null, filters;
    var registerstream = {};
      
    function searchDo(p) {
        var eu = p.endUrl();

        if (eu == "" || eu == "/") {
            uipage.transfer("/", { replace: true });
        }
        else {
            columns = [];
            results = [];
            sortList = ["null"];
            sortBy = null;
            sortType = null;
            page = 0;
            npage = 10;

            var eur = eu.substr(1);

            var eua = eur.split('/');
            eus = eua[0];
            eus = eus.replaceAll('+', ' ');
            eus = unescape(eus);            
            search = eus;

            center.setSearchBoxValue(search);

            $.each(eua, function (ei, ev) {
                if (ei > 0) {
                    var evt = ev.toLowerCase();
                    if (evt.startsWith("page-")) {
                        var evp = ev.substr(5);
                        var cp = parseInt(evp) - 1;
                        if (cp >= 0) {
                            page = cp;
                        }
                    }
                    else if (evt.startsWith("sort-")) {

                        //debug(filterEntriesReferences);

                        var evp = ev.substr(5);

                        var sot = "asc";
                        if (evp.endsWith("-desc")) {
                            sot = "desc";
                            evp = evp.substr(0, evp.length - 5);
                        }
                        else if (evp.endsWith("-asc")) {
                            sot = "asc";
                            evp = evp.substr(0, evp.length - 4);
                        }
                        sortBy = evp.replaceAll("-", "_");
                        sortType = sot;
                    }
                }
            });

            center.startLoading();
            searchJQXHR = $$.get(101, { s: search, p: page, n: npage, o: sortBy, ot: sortType, m: 1 }, function (d) {
                center.endLoading();
                count = d.n; type = d.t; subType = d.st;
                var refsearch = d.rs;
                if (refsearch != null) {
                    center.setSearchBoxValue(refsearch);
                }
                searchid = d.sid;
                columns = d.c;
                filters = d.f;

                var didYouMean = d.dy;
                var didntUnderstand = d.du;

                if (count > 0) {
                    mpage = Math.floor(count / npage) + 1;
                    captureResults(d.r);
                    enterSearchResult(count, false, false);
                    setResults(getCurrentEntries());
                    if (count > 1)
                        setFilters(filters);
                    else
                        setFilters(null);
                }
                else {
                    //didYouMean = $$.random(2) == 0 ? true : false;
                    mpage = 0;
                    enterSearchResult(0, didYouMean, didntUnderstand);
                    setResults(null);
                    setFilters(null);
                }
                setRelated(d.xq, d.xe, d.oq, d.oe, didYouMean, didntUnderstand);
            });
        }
    };
    function getSortListIndex() {
        var aso;
        if (sortBy == null) aso = "null";
        else aso = sortBy + "_" + sortType;
        var ar = sortList.indexOf(aso);
        if (ar == -1) {
            sortList.push(aso);
            ar = sortList.indexOf(aso);
            if (results[ar] == null) results[ar] = [];
        }
        return ar;
    };
    function captureResults(r) {
        var ar = getSortListIndex();
        $.each(r, function (ri, rv) {
            var rownum = rv[0] - 1;
            if (results[ar] == null) results[ar] = [];            
            results[ar][rownum] = rv;
        });
    };
    function getCurrentEntries() {
        var rownumstart = page * npage;
        var rownumend = ((page + 1) * npage) - 1;
        if (rownumend >= count) rownumend = count - 1;

        var entries = [];
        var ar = getSortListIndex();
        for (var i = rownumstart; i <= rownumend; i++) {
            var entry = results[ar][i];
            entries.push(entry);
        }

        return entries;
    };
    function searchModify(action) {
        var modify = false;
        if (action == "next") {
            if (page < mpage - 1) {
                page++;
                modify = true;
            }
        }
        else if (action == "back") {
            if (page > 0) {
                page--;
                modify = true;
            }
        }
        else if (action.startsWith("sort:")) {
            var ts = action.split(":");
            var key = ts[1];
            var met = ts[2];
            
            sortBy = key;
            sortType = met;
            modify = true;

            var lowername = sortBy.toLowerCase().replaceAll("_", "-");
            var lowertype = sortType.toLowerCase();

            if (lowertype == "asc") lowertype = "-asc";
            else lowertype = "-desc";

            var eur = uipage.endUrl();
            var eua = eur.split('/');
            var eleft = 0;
            var ketemu = false;

            $.each(eua, function (ei, ev) {
                if (ei > 0) {
                    eleft += 1;
                    if (ev.toLowerCase().startsWith("sort-")) {
                        var neur = "/search" + eur.substr(0, eleft) + "sort-" + lowername + lowertype + eur.substr(eleft + ev.length);
                        uipage.transfer(neur, { replace: true, nocallback: true });
                        ketemu = true;
                        return false;
                    }
                }
                eleft += ev.length;
            });

            if (ketemu == false) {
                if (!eur.endsWith("/")) eur = eur + "/";
                eur = eur + "sort-" + lowername + lowertype;
                uipage.transfer("/search" + eur, { replace: true, nocallback: true });
            }
        }
        else if (action.startsWith("page:")) {
            var ts = action.split(":");
            var met = ts[1];
            var newpage = parseInt(met) - 1;
            if (page != newpage) {                
                page = newpage;
                modify = true;

                var eur = uipage.endUrl();
                var eua = eur.split('/');
                var eleft = 0;
                var ketemu = false;
                $.each(eua, function (ei, ev) {
                    if (ei > 0) {
                        eleft += 1;
                        if (ev.toLowerCase().startsWith("page-")) {
                            var neur = "/search" + eur.substr(0, eleft) + "page-" + (page + 1) + eur.substr(eleft + ev.length);
                            uipage.transfer(neur, { replace: true, nocallback: true });
                            ketemu = true;
                            return false;
                        }
                    }
                    eleft += ev.length;                    
                });

                if (ketemu == false) {
                    if (!eur.endsWith("/")) eur = eur + "/";
                    eur = eur + "page-" + (page + 1);
                    uipage.transfer("/search" + eur, { replace: true, nocallback: true });
                }
            }
        }

        if (modify) {
            var ar = getSortListIndex();
            if (results[ar][page * npage] == null) {
                center.startLoading();
                //debug(page, npage, search);
                searchJQXHR = $$.get(101, { s: search, p: page, n: npage, o: sortBy, ot: sortType, sid: searchid, m: 1 }, function (d) {
                    center.endLoading();
                    captureResults(d.r);
                    setResults(getCurrentEntries());
                });
            }
            else {
                setResults(getCurrentEntries());
            }
        }
    };
    function preloadSearchResult(tpage, tpagelen) {
        //debug("preloading: " + tpage + " " + tpagelen);
        setTimeout(function () {
            $$.get(101, { s: search, p: tpage, n: npage, u: tpagelen, o: sortBy, ot: sortType, sid: searchid }, function (d) {
                captureResults(d.r);
            });
        }, 100);
    };



    // topology
    function drawTopology(f, ref, topology, index, area) {
        if (topology.length > 0) {
            area.show();

            if (ref.topologyCanvas == null) {
                ref.topologyCanvas = ui.raphael(area)({ left: 0, top: 0, height: 28 });
                ref.topologyContents = ui.box(area)({ left: 0, top: 0, height: 28 });
            }

            function setWidth(w) {
                ref.topologyCanvas.width(w);
                ref.topologyContents.width(w);
            };
            function getSection(key) {
                var ar = null;
                $.each(topology, function (ai, av) {
                    if (av[0] == key) {
                        ar = av;
                        return false;
                    }
                });
                return ar;
            };
            var g = ref.topologyCanvas.paper(); g.clear();
            var c = ref.topologyContents; c.removeChildren();

            ui.icon(c, center.icon("topology"))({ top: 7, left: 0, color: 45, size: [16, 16] });
            
            var clink = null;
            var clinkstate = null;

            var left = 22;
            var rightLastMile = false;

            // pi
            var pi = getSection("PI");
            if (pi != null) {
                var piNO = ui.text(c)({
                    text: pi[1], top: 3, font: 15, left: left, noBreak: true, clickToSelect: true, cursor: "copy"
                });
                left = piNO.leftWidth();
                var piName = ui.text(c)({
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

                //if (topologyIndex == 0) piid = ref.piid;
            }
            else {
                ref.piid = null;
                ref.piname = null;
                ref.pino = null;
            }

            // xpi
            var xpi = getSection("XPI");
            if (xpi != null) {
                //1: no
                //2: pi
                var xpiNO = ui.text(c)({
                    text: xpi[1], top: 3, left: left, font: 15, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                });
                left = xpiNO.leftWidth();
                var xpiName = ui.text(c)({
                    text: xpi[2], top: 5, font: 12, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                });
                left = xpiName.leftWidth();

                clink = g.rect(left + 10, 11, 15, 5).attr({ stroke: "none", fill: ui.color(75) });
                left += 25;
                clinkstate = true;

                setWidth(left);

                var nebox = ui.box(c)({
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

            var mid = getSection("MID");
            if (mid != null) {
                var midNO = ui.text(c)({
                    text: mid[1], left: left, top: 3, font: 15, color: 0, noBreak: true, clickToSelect: true, cursor: "copy"
                });
                left = midNO.leftWidth();

                var midName = ui.text(c)({
                    text: center.formatInterfaceName(mid[5], mid[2]), top: 5, font: 12, left: left + 10, color: mid[7] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                });
                left = midName.leftWidth();

                setWidth(left);
            }

            var lightningLeft = null;

            // mil
            var mil = getSection("MIL");
            if (mil != null) {
                var endlLocal = ui.text(c)({
                    text: "LAST MILE", left: left, top: 5, font: 12, noBreak: true
                });
                left = endlLocal.leftWidth();

                if (mil[1] != null) {
                    g.rect(left + 10, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                    g.rect(left + 10 + 8, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                    g.rect(left + 10 + 16, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });
                    g.rect(left + 10 + 24, 11, 5, 5).attr({ stroke: "none", fill: ui.color(35) });

                    left += 40;

                    var end2NO = ui.text(c)({
                        text: mil[1], top: 3, font: 15, left: left + 10, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = end2NO.leftWidth();

                    var end2NameVar = mil[2];
                    if (end2NameVar != "UNSPECIFIED" && end2NameVar != null) {
                        if (end2NameVar.startsWith("Ex")) end2NameVar = end2NameVar.substr(2);
                        var end2Name = ui.text(c)({
                            text: end2NameVar, top: 5, font: 12, left: left + 10, color: 55, noBreak: true, clickToSelect: true, cursor: "copy"
                        });
                        left = end2Name.leftWidth();
                    }

                    lightningLeft = ui.icon(c, center.icon("lightning"))({
                        size: [20, 20], left: left + 4, top: 5, color: 35,
                        tooltipSpanColor: ["accent+50"]
                    });
                    left += 20;
                }

                clink = g.rect(left + 10, 11, 15, 5).attr({ stroke: "none", fill: ui.color(35) });
                left += 25;
                clinkstate = true;

                setWidth(left);
            }

            // mi2
            var mi2 = getSection("MI2");

            if (mi2 != null) {

                if (mi2[5] != null) {
                    if (lightningLeft != null)
                        lightningLeft.tooltip("{0|" + mil[1] + "} is based on interface description found on {0|" + mi2[1] + "} and may not same as actual device's name");

                    if (clinkstate == mi2[8]) clink.attr({ width: 30 });
                    else g.rect(left, 11, 15, 5).attr({ stroke: "none", fill: ui.color(mi2[8] ? 35 : 75) });
                    left += 15;

                    var mi2Name = ui.text(c)({
                        text: center.formatInterfaceName(mi2[5], mi2[2]), top: 5, font: 12, left: left + 10, color: mi2[7] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    left = mi2Name.leftWidth();

                    //24
                    if (mi2[24] != null && mi2[24] > 1) {
                        var multi = ui.icon(c, center.icon("split"))({
                            size: [20, 20], left: left + 6, top: 3, color: 35, rotation: 90, flip: "V",
                            tooltip: "{0|" + mi2[24] + "} INTERFACES",
                            tooltipSpanColor: ["accent+50"]
                        });
                        left += 20;
                    }
                    left += 10;
                }

                var mi2NO = ui.text(c)({
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
                    var xmi2Name = ui.text(c)({
                        text: center.formatInterfaceName(pi[20], pi[22]), top: 5, font: 12, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    var xle = left;
                    left = xmi2Name.leftWidth();
                    var xmi2NO = ui.text(c)({
                        text: pi[21], top: 3, font: 15, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                    });
                    pi2Link.attr({ fill: ui.color(75) });
                    left = xmi2NO.leftWidth();

                    var nebox = ui.box(c)({
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
                            var piEndName = ui.text(c)({
                                text: text, top: 5, font: 12, left: left + 10, color: pi[4] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                            });
                            left = piEndName.leftWidth();
                        }
                        var piEndNO = ui.text(c)({
                            text: pi[21], top: 3, font: 15, left: left + 10, noBreak: true, clickToSelect: true, cursor: "copy"
                        });
                        left = piEndNO.leftWidth();

                    }
                    else {
                        var piEnd = ui.text(c)({
                            text: "LAST MILE", top: 5, font: 12, left: left + 10, noBreak: true
                        });
                        left = piEnd.leftWidth();
                    }
                }

                setWidth(left);
            }
            else {
                // xmi2
                var xmi2 = getSection("XMI2");
                if (xmi2 != null) {
                    var xpiName = ui.text(c)({
                        text: "PE", top: 5, font: 12, left: left + 10, color: 75, noBreak: true
                    });
                    left = xpiName.leftWidth();

                    var xpiLink = g.rect(left + 10, 11, 30, 5).attr({ stroke: "none", fill: ui.color(75) });
                    left += 40;

                    var xmi2Name = ui.text(c)({
                        text: "METRO END 2", top: 5, font: 12, left: left + 10, color: 75, noBreak: true
                    });
                    left = xmi2Name.leftWidth();

                    var nebox = ui.box(c)({
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

            var mx = getSection("MX");
            if (mx != null) {
                var multi = ui.icon(c, center.icon("split"))({
                    size: [20, 20], left: left + 4, top: 3, color: 35, rotation: 90,
                    tooltip: "{0|" + mx[1] + "} REMOTE PEERS",
                    tooltipSpanColor: ["accent+50"]
                });
                left += 20;

                var vcid = f.column("VCID")[index];

                var linkbox = ui.box(c)({
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
            var mc = getSection("MC");
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
                    var multi = ui.icon(c, center.icon("split"))({
                        size: [20, 20], left: left + 4, top: 3, color: 35, rotation: 90, flip: "V",
                        tooltip: "{0|" + mc[18] + "} REMOTE PEERS",
                        tooltipSpanColor: ["accent+50"]
                    });
                    left += 20;
                }

                var mi1NO = ui.text(c)({
                    text: mc[1], top: 3, font: 15, left: left + 10, noBreak: true, clickToSelect: true, cursor: "copy"
                });
                left = mi1NO.leftWidth();

                setWidth(left);
            }

            var xmc = getSection("XMC");
            if (xmc != null) {
                g.rect(left + 10, 11, 30, 5).attr({ stroke: "none", fill: ui.color(75) });
                var xleft = left;
                left += 40;

                var xmi1NO = ui.text(c)({
                    text: xmc[1], top: 3, font: 15, left: left + 10, color: 75, noBreak: true, clickToSelect: true, cursor: "copy"
                });
                left = xmi1NO.leftWidth();

                g.rect(left + 10, 11, 30, 5).attr({ stroke: "none", fill: ui.color(75) });
                left += 40;

                var end1Local = ui.text(c)({
                    text: "LAST MILE", top: 5, font: 12, left: left + 10, color: 75, noBreak: true
                });

                left = end1Local.leftWidth();

                var nebox = ui.box(c)({
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
            var mi1 = getSection("MI1");

            if (mi1 != null) {

                if (mi1[16] != null) {
                    rightcloud = g.rect(left + 5, 2, 0, 22).attr({ stroke: "none", fill: null });
                    rightcloudsid = mi1[16];
                }


                var ntype;
                if (mc != null) ntype = mc[2];
                else ntype = mi2[2];

                var mi1Name = ui.text(c)({
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
                        end1var2 = center.formatInterfaceName(end1var2, "NEIGHBOR");
                        var end1Name = ui.text(c)({
                            text: end1var2, top: 5, font: 12, left: left + 10, color: mi1[4] ? 0 : 55, noBreak: true, clickToSelect: true, cursor: "copy"
                        });
                        left = end1Name.leftWidth();
                    }
                }

                var end1 = ui.text(c)({
                    text: end1NO, top: end1var == null ? 5 : 3, font: end1var == null ? 12 : 15, left: left + 10, noBreak: true, clickToSelect: end1var == null ? false : true, cursor: end1var == null ? "" : "copy"
                });
                left = end1.leftWidth();
                
                if (end1var != null) {
                    var nused = null;
                    if (mc != null) nused = mc[1];
                    else if (mi2 != null) nused = mi2[1];

                    ui.icon(c, center.icon("lightning"))({
                        size: [20, 20], left: left + 4, top: 5, color: 35,
                        tooltip: "{0|" + end1NO + "} is based on interface description found on {0|" + nused + "} and may not same as actual device's name",
                        tooltipSpanColor: ["accent+50"]
                    });
                    left += 20;
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

                var end1Local = ui.text(c)({
                    text: "LAST MILE", top: 5, font: 12, left: left + 10, noBreak: true
                });

                left = end1Local.leftWidth();

            }

            if (rightcloud != null) {

                var linkbox = ui.box(c)({
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

            setWidth(left + 20);

            area.scrollCalculate();
        }
        else area.hide();
    };

    ui("search", {
        init: function (p) {
            uipage = center.init(p);

            var titlearea = ui.box(p)({ size: ["100%", 40], top: 0 });

            var titleBoxFocus = 0;
            var titleBox1 = ui.box(titlearea)({
                topBottom: [0, 0], left: 0, width: 200, hide: true, cursor: "default", button: {
                    normal: function () {
                        titleBox1({ color: null });
                    },
                    over: function () {
                        titleBox1({ color: 93 });
                    },
                    click: function () {
                        titleBox1({ disableButton: true, cursor: "default", color: null });
                        title1.color("accent", { duration: 100 });
                        title2.color(35, { duration: 100 });
                        focusBorder.animate({ left: 20, width: title1.width() }, { duration: 100 });
                        titleBox2({ enableButton: true, cursor: "pointer" });

                        if (isfiltersexists) filter.show();
                        if (ispagingexists) {
                            pagingarea.show();
                            nextbutton.show();
                            backbutton.show();
                        }
                        if (isnomatchexists) {
                            nomatch.show();
                        }
                        
                        if (results.length > 0)
                            searchresult.show();                            
                        related.hide();
                    }
                }
            });
            var title1 = ui.text(titleBox1)({ font: ["head", 12], color: "accent", left: 20, bottom: 13, noBreak: true });
            var focusBorder = ui.box(titlearea)({ height: 2.5, width: 176, color: "accent", bottom: 0, left: 20, hide: true });
            var titleBox2 = ui.box(titlearea)({
                topBottom: [0, 0], left: 0, width: 200, hide: true, cursor: "default", button: {
                    normal: function () {
                        titleBox2({ color: null });
                    },
                    over: function () {
                        titleBox2({ color: 93 });
                    },
                    click: function () {
                        titleBox2({ disableButton: true, cursor: "default", color: null });
                        title2.color("accent", { duration: 100 });
                        title1.color(35, { duration: 100 });
                        focusBorder.animate({ left: titleBox2.left() + 20, width: title2.width() }, { duration: 100 });
                        titleBox1({ enableButton: true, cursor: "pointer" });

                        if (filter.isShown()) isfiltersexists = true;
                        else isfiltersexists = false;
                        if (pagingarea.isShown()) ispagingexists = true;
                        else ispagingexists = false;
                        if (nomatch.isShown()) isnomatchexists = true;
                        else isnomatchexists = false;

                        filter.hide();
                        pagingarea.hide();
                        nextbutton.hide();
                        backbutton.hide();
                        searchresult.hide();
                        nomatch.hide();

                        related.show();
                    }
                }
            });
            var title2 = ui.text(titleBox2)({ font: ["head", 12], color: 35, left: 20, bottom: 13, noBreak: true });
                
            var filter = ui.box(p)({ width: "100%", top: 40, height: 40, color: 85, hide: true});                
            var searchresult = ui.box(p)({ width: "100%", topBottom: [40, 0], scroll: { horizontal: false }, hide: true });
            var related = ui.box(p)({ width: "100%", topBottom: [40, 0], scroll: { horizontal: false }, hide: true, color: 90 });
            var nomatch = ui.box(p)({ width: "100%", topBottom: [40, 0], hide: true, color: 90 });

            var filterHead = ui.text(filter)({ text: "SORT BY", font: ["body", 11], color: 45, left: 20, top: 13 });
            var pagingarea = ui.box(titlearea)({ height: "100%", right: 40, width: 270, hide: true });

            var nomatchtext = ui.text(nomatch)({ top: 20, leftRight: [20, 20], font: ["body", 14], text: "NO MATCH TEXT HERE" });

            var pagebox = [];
            var pagetext = [];
            $$.for(0, 9, function (index) {
                pagebox[index] = ui.box(pagingarea)({
                    height: 40, hide: true, button: {
                        normal: function () {
                            if (pagetext[index].text() != ((page + 1) + ""))
                                pagetext[index].color("accent");
                        },
                        over: function () {
                            if (pagetext[index].text() != ((page + 1) + ""))
                                pagetext[index].color("accent+25");
                        },
                        click: function () {
                            var ioc = pagetext[index].text();
                            searchModify("page:" + ioc);                                
                        }
                    }});
                pagetext[index] = ui.text(pagebox[index])({ text: "", left: 5, bottom: 13, font: ["head", 12], color: "accent" });
            });
            var trimLastBox = ui.box(pagingarea)({ height: 40, width: 20, hide: true });
            var trimLastText = ui.text(trimLastBox)({ text: "...", left: 4, bottom: 13, font: ["head", 12], color: 35 });
            var trimNextBox = ui.box(pagingarea)({ height: 40, width: 20, hide: true });
            var trimNextText = ui.text(trimNextBox)({ text: "...", left: 4, bottom: 13, font: ["head", 12], color: 35 });

            var nextbutton = ui.box(titlearea)({
                size: [40, 40], right: 0, cursor: "pointer", hide: true,
                button: {
                    normal: function () {
                        if (page < mpage - 1)
                            nexticon.color("accent");
                    },
                    over: function () {
                        if (page < mpage - 1)
                            nexticon.color("accent+25");
                    },
                    click: function () {
                        searchModify("next");
                    }
                }
            });
            var nexticon = ui.icon(nextbutton, "arrow")({
                size: [17, 17], bottom: 12, left: 5
            });
            var backbutton = ui.box(titlearea)({
                size: [40, 40], right: 340, cursor: "pointer", hide: true,
                button: {
                    normal: function () {
                        if (page > 0)
                            backicon.color("accent");
                    },
                    over: function () {
                        if (page > 0)
                            backicon.color("accent+25");
                    },
                    click: function () {
                        searchModify("back");
                    }
                }
            });
            var backicon = ui.icon(backbutton, "arrow")({
                size: [17, 17], bottom: 12, right: 5, rotation: 180
            });
                
            var firstTime = true, entersearch = true;

            enterSearchResult = function (nresult, didYouMean, didntUnderstand) {

                nomatch.hide();

                if (didYouMean == false) {

                    titleBoxFocus = 0;

                    var t1w;
                    titleBox1.show();
                        
                    if (didntUnderstand == true) {
                        title1.text("NO MATCHING RESULTS");
                        nomatch.show();
                        nomatchtext.text("We are sorry, we couldn't understand your search. Please try with another query, or try related search.");
                    }
                    else {
                        if (nresult > 0) {
                            title1.text("MATCHED " + nresult + " RESULT" + (nresult > 1 ? "S" : ""));
                        }
                        else {
                            title1.text("NO MATCHING RESULTS");
                            nomatch.show();
                            nomatchtext.text("We are sorry, your search didn't match for any results.");
                        }
                    }

                    t1w = title1.width();
                    titleBox1({ width: t1w + 40 });
                    title1({ color: "accent" });

                    if (firstTime) {
                        focusBorder({ show: true, left: (t1w + 40) / 2, width: 0 });
                        setTimeout(function () {
                            focusBorder.animate({ left: 20, width: t1w }, { duration: 200 });
                        }, 500);
                        firstTime = false;
                    }
                    else {
                        focusBorder({ show: true, left: 20, width: t1w });
                    }

                    titleBox1.disableButton();

                    titleBox2.show();
                    title2.text("RELATED SEARCH");

                    var t2w = title2.width();
                    titleBox2({ left: titleBox1.leftWidth(), width: t2w + 40, cursor: "pointer" });
                    title2({ color: 35 });

                    titleBox2.enableButton();
                }
                else {
                    titleBoxFocus = 1;

                    titleBox1.hide();

                    titleBox2.show();
                    title2.text("DID YOU MEAN");

                    var t2w = title2.width();
                    titleBox2({ left: 0, width: t2w + 40 });
                    title2({ color: "accent" });

                    if (firstTime) {
                        focusBorder({ show: true, left: (t2w + 40) / 2, width: 0 });
                        setTimeout(function () {
                            focusBorder.animate({ left: 20, width: t2w }, { duration: 200 });
                        }, 500);
                        firstTime = false;
                    }
                    else {
                        focusBorder({ show: true, left: 20, width: t2w });
                    }

                    titleBox2({ disableButton: true, cursor: "default" });

                }

            };

            var currentResultType = null, currentFilterType = null;
            var resultBoxes = [], resultEntriesReferences = [];
            var focusBox = -1, filterBoxes = [], filterEntriesReferences = [];
            var busy = false;
            var boxswapper = null, boxswapper2 = null;
            var lastPage = -1;
            var referenceSpecial = ["resize", "expand", "shrink"];
            
            registerstream = {};
            
            searchresult.resize(function (d) {
                $.each(resultEntriesReferences, function (entryIndex, entry) {
                    if (entry.resize != null) {
                        entry.resize(d);
                    }
                });
            });

            var oldnecrowonline = false;
            var necrowonlinechanged = function () {
                if (necrowonline != oldnecrowonline) {
                    oldnecrowonline = necrowonline;
                    $.each(resultEntriesReferences, function (entryIndex, entry) {
                        if (entry.necrow != null) entry.necrow("necrow", necrowonline);
                    });
                }
            };

            uipage.stream(function (type, data) {
                if (registerstream[type] != null) {
                    var rs = registerstream[type];
                    $.each(rs.callbacks, function (rsi, rsv) {
                        if ($.isFunction(rsv)) {
                            rsv(data);
                        }
                    });
                }
            });
                
            if ($$.isStreamAvailable()) {
                $$.get(50001, function (d) {
                    necrowonline = (d.data == "online") ? true : false;
                    necrowonlinechanged();
                });
            }                
                
            setResults = function (entries) {

                if (entries == null) {
                    searchresult.hide();
                    related.hide();
                    if (pagingarea.isShown()) {
                        $$(50, function () {
                            var wir = pagingarea.width();
                            pagingarea.fadeOut(50);
                            backbutton.$.css({ x: 0, opacity: 1 }).transition({ x: wir / 2, opacity: 0, duration: 166, complete: function () { backbutton({ opacity: 1, hide: true }); } });
                            nextbutton.$.css({ x: 0, opacity: 1 }).transition({ x: -wir / 2, opacity: 0, duration: 166, complete: function () { nextbutton({ opacity: 1, hide: true }); } });
                        });
                    }
                }
                else {
                    ui.script("search_" + type, function (proc) {
                        searchresult.show();
                        related.hide();

                        if (ispagingexists) {
                            pagingarea.show();
                            backbutton.show();
                            nextbutton.show();
                        }
                        ispagingexists = false;

                        var create = false, hidePaging = false, showPaging = false;
                        if (entries == null) {
                            $.each(resultBoxes, function (resultBoxIndex, resultBox) {
                                resultBox.hide();
                                resultBox.button(null);
                                resultBox.removeChildren();
                            });
                            resultEntriesReferences = [];
                            currentResultType = null;
                            searchresult.scrollCalculate();
                            if (pagingarea.isShown()) hidePaging = true;
                        }
                        else {
                            if (currentResultType != type) {
                                create = true;
                                currentResultType = type;
                                $.each(resultBoxes, function (resultBoxIndex, resultBox) {
                                    resultBox({ size: ["100%", 1], top: resultBoxIndex * 1, hide: true, button: null, cursor: null, removeChildren: true });
                                });
                                resultEntriesReferences = [];
                            }

                            if (pagingarea.isShown()) {
                                if (mpage == 1) hidePaging = true;
                            }
                            else {
                                if (mpage > 1) showPaging = true;
                            }
                        }

                        if (boxswapper == null) {
                            boxswapper = ui.box(searchresult)({ z: 2 });
                            boxswapper2 = ui.box(searchresult)({ z: 2, hide: true });
                        }
                        boxswapper.removeChildren();
                        boxswapper2.removeChildren();

                        if (!pagingarea.isShown()) boxswapper({ left: 0, opacity: 1, hide: true });
                        else boxswapper({ left: 0, opacity: 1, show: true });

                        (function (/*paging*/) {
                            // todo, move from here
                            for (var i = 0; i <= 8; i++) {
                                pagebox[i]({ hide: true, cursor: "pointer" });
                                pagetext[i]({ text: "", color: "accent" });
                            }

                            if (mpage >= 2) {
                                pagetext[0].text("1");
                                pagetext[1].text(mpage + "");

                                // 0 1 2 3 4 5 6 7 8 9
                                // F 3 2 1 P 1 2 3 L
                                var lastTrim = false;
                                var nextTrim = false;

                                var oi = 2;
                                var opage = page + 1;

                                if (opage > 1 && opage < mpage) {
                                    pagebox[oi].cursor("default");
                                    pagetext[oi++]({ text: opage + "", color: 35 }); // current page

                                    var inc = 1;
                                    while (true) {
                                        var nextpage = opage + inc;
                                        if (nextpage < mpage) pagetext[oi++].text(nextpage + "");
                                        else if (oi == mpage) break;
                                        if (oi == 9) break;
                                        var ppage = opage - inc;
                                        if (ppage > 1) pagetext[oi++].text(ppage + "");
                                        if (oi == 9) break;
                                        inc++;
                                    }

                                    if (opage + inc < mpage - 1) nextTrim = true;
                                    if (opage - inc > 2) lastTrim = true;
                                }
                                else if (opage == 1) {
                                    pagebox[0].cursor("default");
                                    pagetext[0].color(35);

                                    var inc = 1;
                                    while (true) {
                                        if (1 + inc >= mpage) break;
                                        pagetext[oi++].text((1 + inc) + "");
                                        if (oi == 9) break;
                                        inc++;
                                    }
                                    if (1 + inc < mpage - 1) nextTrim = true;
                                }
                                else if (opage == mpage) {
                                    pagebox[1].cursor("default");
                                    pagetext[1].color(35);

                                    var inc = 1;
                                    while (true) {
                                        if (mpage - inc <= 1) break;
                                        pagetext[oi++].text((mpage - inc) + "");
                                        if (oi == 9) break;
                                        inc++;
                                    }
                                    if (opage - inc > 2) lastTrim = true;
                                }

                                var availableSize = 240;
                                var totalSize = 0;
                                var oneisvisible = false;

                                var ptext = [];
                                for (var i = 0; i <= 8; i++) {
                                    if (availableSize > 0 && pagetext[i].text() != "") {
                                        var os = pagetext[i].textSize().width + 8;
                                        if (availableSize - os > 0) {
                                            availableSize -= os;
                                            totalSize += os;
                                            oneisvisible = true;
                                            if (!pagingarea.isShown()) pagingarea.show();
                                            pagebox[i]({ width: os, show: true });
                                            ptext.push(parseInt(pagetext[i].text()));
                                        }
                                    }
                                }
                                ptext.sort(function (a, b) { return a - b; });

                                var pf = null, pc;
                                var olind = getSortListIndex();
                                $.each(ptext, function (pi, pv) {

                                    var pvv = pv - 1;

                                    var r0 = results[olind][pvv * npage];
                                    var rn = results[olind][pvv * npage + (npage - 1)];

                                    if (pvv == page || r0 != null) {
                                        if (pf != null) preloadSearchResult(pf, pc);
                                        pf = null;
                                    }
                                    else if (pf == null) { pf = pvv; pc = 1; }
                                    else if (pvv == pf + pc) pc++;
                                    else {
                                        preloadSearchResult(pf, pc);
                                        pf = pvv;
                                        pc = 1;
                                    }
                                });
                                if (pf != null) {
                                    preloadSearchResult(pf, pc);
                                }

                                if (lastTrim) {
                                    trimLastBox({ show: true, left: pagebox[0].width() });
                                }
                                else {
                                    if (trimLastBox.isShown()) trimLastBox.fadeOut(166);
                                }

                                if (lastTrim) totalSize += 20;
                                if (nextTrim) totalSize += 20;

                                pagingarea.width(totalSize);
                                pagebox[1].left(totalSize - pagebox[1].width());

                                if (nextTrim) {
                                    trimNextBox({ show: true, left: pagebox[1].left() - 20 });
                                }
                                else {
                                    if (trimNextBox.isShown()) trimNextBox.fadeOut(166);
                                }

                                /////
                                var oref = [];
                                var lt = parseInt(pagetext[2].text());
                                oref.push(pagebox[2]);
                                for (var i = 3; i <= 8; i++) {
                                    if (pagebox[i].isShown()) {
                                        var tt = parseInt(pagetext[i].text());
                                        if (tt < lt) oref.splice(0, 0, pagebox[i]);
                                        else oref.push(pagebox[i]);
                                    }
                                }

                                var ls;
                                if (lastTrim) ls = trimLastBox.leftWidth();
                                else ls = pagebox[0].leftWidth();

                                $.each(oref, function (orefi, orefv) {
                                    orefv.left(ls);
                                    ls += orefv.width();
                                });
                                /////


                                if (oneisvisible == false && pagingarea.isShown()) pagingarea.hide();

                                var nbright = pagingarea.width() + 40;
                                if (backbutton.isShown()) {
                                    backbutton.animate({ right: nbright }, { duration: 166 });
                                }
                                else {
                                    backbutton.right(nbright);
                                }
                            }

                            if (mpage > 0) {
                                if (page == 0) {
                                    backbutton.cursor("default");
                                    backicon.color(75);
                                }
                                else {
                                    backbutton.cursor("pointer");
                                    backicon.color("accent");
                                }
                                if (page == mpage - 1) {
                                    nextbutton.cursor("default");
                                    nexticon.color(75);
                                }
                                else {
                                    nextbutton.cursor("pointer");
                                    nexticon.color("accent");
                                }
                            }


                            if (showPaging) {
                                pagingarea.hide();
                                $$(500, function () {
                                    pagingarea.fadeIn(100);
                                    nextbutton.show();
                                    backbutton.show();
                                    var wir = pagingarea.width();
                                    backbutton.$.css({ x: wir / 2, opacity: 0 }).transition({ x: 0, opacity: 1, duration: 166 });
                                    nextbutton.$.css({ x: -wir / 2, opacity: 0 }).transition({ x: 0, opacity: 1, duration: 166 });
                                });
                            }
                            else if (hidePaging) {
                                $$(50, function () {
                                    var wir = pagingarea.width();
                                    pagingarea.fadeOut(50);
                                    backbutton.$.css({ x: 0, opacity: 1 }).transition({ x: wir / 2, opacity: 0, duration: 166, complete: function () { backbutton.hide(); } });
                                    nextbutton.$.css({ x: 0, opacity: 1 }).transition({ x: -wir / 2, opacity: 0, duration: 166, complete: function () { nextbutton.hide(); } });
                                });
                            }
                        })();

                        if (mpage == 0) return;
                        focusBox = -1;

                        var write = 0;
                        var boxswapperToHide = [];

                        if (!entersearch) {
                        }

                        $.each(registerstream, function (rsi, rsv) {
                            $$.removeRegister(rsi);
                            delete registerstream[rsi];
                        });

                        $.each(entries, function (ei, ev) {

                            busy = false;
                            var r = resultEntriesReferences[ei];
                            if (r == null) {
                                create = true;
                                r = {};
                                resultEntriesReferences[ei] = r;
                            }

                            var b = resultBoxes[ei];
                            if (b == null) {
                                b = ui.box(searchresult)({
                                    size: ["100%", 1], top: ei * 1, z: 1
                                });
                                resultBoxes[ei] = b;
                                b.data("index", ei);
                            }

                            var index = b.data("index");
                            b.color(index % 2 == 0 ? 91 : 93);
                            b.show();

                            if (!entersearch) {
                                if (page != lastPage) {
                                    if (b.isVisibleInside(searchresult)) {
                                        //var jqClone = b.clone();
                                        //jqClone.css({ top: b.top() - searchresult.scrollTop() });
                                        //boxswapper.$.append(jqClone);
                                    }
                                }
                            }

                            var f = {};
                            f.create = create;
                            f.column = function (name) {
                                if (columns.length > 0) {
                                    var ic = columns.indexOf(name);
                                    if (ic > -1) return ev[ic];
                                }
                                return null;
                            }
                            f.setButton = function (c) {
                                b({
                                    cursor: "pointer",
                                    button: {
                                        normal: function () {
                                            b.color(index % 2 == 0 ? 91 : 93, { duration: 100 });
                                        },
                                        over: function () {
                                            b.color(97);
                                        },
                                        click: function () {
                                            if (c != null) c(ei);
                                        }
                                    }
                                });
                            };
                            f.clearButton = function () {
                                b({ cursor: null, button: null });
                            };
                            f.setNormal = function (s) {
                                b({ height: s, top: ei * s });
                            }
                            f.setSize = function (height) {
                                b({ height: height, top: ei * height });
                            };
                            f.setExpand = function (normalHeight, expandHeight, enter, leave, step, complete) {
                                b.enableButton();
                                b.cursor("pointer");
                                b({ height: normalHeight, top: ei * normalHeight });

                                if (enter == null) enter = function () { };
                                if (leave == null) leave = function () { };
                                if (step == null) step = function () { };
                                if (complete == null) complete = function () { };

                                b.button({
                                    click: function (e) {

                                        if (busy) return;

                                        if (ei == focusBox) {
                                        }
                                        else {

                                            busy = true;
                                            b.disableButton();
                                            b.cursor(null);

                                            b.color(99);

                                            var duration = 166;

                                            var tr = resultEntriesReferences[b.data("index")];

                                            if (tr.expand != null) {
                                                if (tr.expandFirstTime == null) tr.expandFirstTime = true;
                                                tr.expand(tr.expandFirstTime, function () {
                                                    //$$(10, function () {
                                                    b.height(expandHeight, { queue: false, duration: duration, complete: function () { b.color(99); searchresult.scrollCalculate(); busy = false; } });
                                                    if (focusBox == -1) {
                                                        for (var i = ei + 1; i < resultBoxes.length; i++) {
                                                            resultBoxes[i].top(i * normalHeight + (expandHeight - normalHeight), { queue: false, duration: duration });
                                                        }
                                                    }
                                                    else {
                                                        var ob = resultBoxes[focusBox];
                                                        var or = resultEntriesReferences[ob.data("index")];
                                                        if (or.shrink != null) or.shrink();

                                                        ob({
                                                            height: [normalHeight, { queue: false, duration: duration }],
                                                            cursor: "pointer",
                                                            color: [
                                                                ob.data("index") % 2 == 0 ? 91 : 93,
                                                                {
                                                                    duration: 166, step: function () {
                                                                        if (step != undefined) step(ob, or);
                                                                    }, complete: function () {
                                                                        if (complete != undefined) complete(ob, or);
                                                                    }
                                                                }],
                                                            enableButton: true
                                                        });

                                                        leave(ob, or);

                                                        if (ei < focusBox) {
                                                            for (var i = ei + 1; i < resultBoxes.length; i++) {
                                                                resultBoxes[i].top(i * normalHeight + (expandHeight - normalHeight), { queue: false, duration: duration });
                                                            }
                                                        }
                                                        else {
                                                            for (var i = focusBox + 1; i <= ei; i++) {
                                                                resultBoxes[i].top(i * normalHeight, { queue: false, duration: duration });
                                                            }
                                                            for (var i = ei + 1; i < resultBoxes.length; i++) {
                                                                resultBoxes[i].top(i * normalHeight + (expandHeight - normalHeight), { queue: false, duration: duration });
                                                            }
                                                        }
                                                    }
                                                    focusBox = ei;
                                                    searchresult.scrollTop(focusBox * normalHeight - 25, { queue: false, duration: duration });
                                                    enter();
                                                    //});
                                                });
                                                tr.expandFirstTime = false;
                                            }


                                        }
                                    }
                                });
                            };
                            f.stream = function (register, callback) {

                                if (registerstream[register] == null) {
                                    registerstream[register] = {
                                        callbacks: []
                                    };
                                    $$.register(register);
                                    //debug("add register " + register);
                                }

                                registerstream[register].callbacks.push(callback);
                            };

                            f.isNecrowOnline = function () {
                                //return true;
                                return necrowonline;
                            };

                            var t = {};
                            t.entries = ev;
                            t.resultBoxes = resultBoxes;
                            t.animTransferedClick = animTransferedClick;
                            t.searchresult = searchresult;
                            t.filter = filter;
                            t.drawTopology = drawTopology;

                            proc(b, r, f, uipage, t);

                            if (entersearch) {
                                var bhe = b.height();

                                if (b.top() < searchresult.height()) {
                                    //b.$.css({ y: bhe, opacity: 0 });
                                    //setTimeout(function () {
                                    //    b.$.transition({ y: 0, opacity: 1, duration: 150 + bhe, queue: false });
                                    //}, 200 /*ei * bhe*/);
                                }
                            }
                            if (!entersearch) {
                                if (page != lastPage) {
                                    if (b.top() < searchresult.height()) {
                                        //debug(ei);
                                        //var jqClone = b.clone();
                                        //jqClone.css({ top: b.top() });
                                        //boxswapper2.$.append(jqClone);
                                        //b.hide();
                                        //boxswapperToHide.push(b);
                                    }
                                }
                            }

                            write++;
                        });

                        for (; write < resultBoxes.length; write++) resultBoxes[write].hide();
                        searchresult.scrollCalculate();
                        searchresult.scrollTop(0);

                        if (!entersearch && page != lastPage) {

                            //debug("change page bawah");


                            //
                            /*boxswapper.size(searchresult.size());
                            boxswapper2.size(searchresult.size());
                            setTimeout(function () {
                                var swapduration = 100;
                                var swapdistance = 50;
                                if (lastPage < page) {
                                    boxswapper.$.css({ zIndex: 21, left: 0, opacity: 1 }).animate({ left: -swapdistance, opacity: 0 }, { duration: swapduration, queue: false });
                                    boxswapper2.$.show().css({ zIndex: 20, left: swapdistance, opacity: 0 }).animate({ left: 0, opacity: 1 }, {
                                        duration: swapduration, queue: false, complete: function () {
                                            boxswapper.$.hide();
                                            boxswapper2.$.hide();
                                            $.each(boxswapperToHide, function (bii, biv) {
                                                biv.show();
                                            });
                                            lastPage = page;
                                        }
                                    });
                                }
                                else {
                                    boxswapper.$.css({ zIndex: 21, left: 0, opacity: 1 }).animate({ left: swapdistance, opacity: 0 }, { duration: swapduration, queue: false });
                                    boxswapper2.$.show().css({ zIndex: 20, left: -swapdistance, opacity: 0 }).animate({ left: 0, opacity: 1 }, {
                                        duration: swapduration, queue: false, complete: function () {
                                            boxswapper.$.hide();
                                            boxswapper2.$.hide();
                                            $.each(boxswapperToHide, function (bii, biv) {
                                                biv.show();
                                            });
                                            lastPage = page;
                                        }
                                    });
                                }
                            }, 1);*/
                        }
                        else {
                        }

                        entersearch = false;
                        lastPage = page;
                    });
                }
            };
            setFilters = function (entries) {

                if (isfiltersexists) filter.show();
                isfiltersexists = false;
                    
                if (entries == null) {
                    if (filter.isShown()) {
                        filter.$.css({ height: 40 }).animate({ height: 0 }, { duration: 200, complete: function () { filter.hide(); } });
                        searchresult.$.css({ top: 80 }).animate({ top: 40 }, { duration: 200 });
                    }
                }
                else {
                    if (currentFilterType != type) {                            
                    }
                    currentFilterType = type;
                    $.each(filterBoxes, function (filterIndex, filterBox) {
                        filterBox.hide();
                    });

                    var write = 0;
                    var left = filterHead.left() + filterHead.textSize().width + 20;
                    $.each(entries, function (ei, ev) {
                        var ov = ev.split(":");
                        var text = ov[0].toUpperCase();
                        var key = ov[1];
                        var rev = false;
                        if (ov.length == 3) rev = true;

                        var b = filterBoxes[ei];
                        var rtext;

                        if (b == null) {
                            create = true;
                            b = ui.box(filter)({
                                size: [0, "100%"], cursor: "pointer"
                            });
                            filterBoxes[ei] = b;
                        }
                        b.show();

                        var r = filterEntriesReferences[ei];

                        if (r == null) {
                            r = {};
                            filterEntriesReferences[ei] = r;
                            r.text = ui.text(b)({
                                font: ["body", 11], left: 10, top: 13, noBreak: true, color: 20
                            });
                            r.icon1 = ui.icon(b, "arrow2")({
                                size: [10, 10], top: 14, right: 10, hide: true
                            });
                            r.icon2 = ui.icon(b, "arrow2")({
                                size: [10, 10], top: 13, right: 10, hide: true, rotation: 180
                            });
                        }

                        r.text({ text: text });
                        r.icon1.hide();
                        r.icon2.hide();

                        b({
                            button: null,
                            button: {
                                normal: function () {
                                    r.text.color(20);
                                },
                                over: function () {
                                    r.text.color(45);
                                },
                                click: function () {
                                    var sorttype;
                                    if (r.icon2.isShown() || !r.icon1.isShown()) {
                                        $.each(filterEntriesReferences, function (fi, fv) {
                                            fv.icon1.hide();
                                            fv.icon2.hide();
                                        });
                                        r.icon1.show(); r.icon2.hide();
                                        $({ n: -5 }).animate({ n: 0 }, {
                                            step: function (now) {
                                                //debug(now);
                                                r.icon1.css({ y: now });
                                            }, duration: 1000
                                        });
                                        r.icon1.$.css({ opacity: 0 }).animate({ opacity: 1 }, { duration: 100 });
                                        searchModify("sort:" + key + (rev ? ":desc" : ":asc"));
                                    }
                                    else if (r.icon1.isShown()) {
                                        $.each(filterEntriesReferences, function (fi, fv) {
                                            fv.icon1.hide();
                                            fv.icon2.hide();
                                        });
                                        r.icon1.hide(); r.icon2.show();
                                        $({ n: 5 }).animate({ n: 0 }, {
                                            step: function (now) {
                                                r.icon2.css({ y: now });
                                            }, duration: 1000
                                        });
                                        r.icon2.$.css({ opacity: 0 }).animate({ opacity: 1 }, { duration: 100 });
                                        searchModify("sort:" + key + (rev ? ":asc" : ":desc"));
                                    }
                                }
                            }
                        });

                        b({ width: 40 + r.text.textSize().width, left: left });
                        left += b.width();
                    });

                    if (!filter.isShown()) {
                        $$(500, function () {
                            filter.$.show().css({ height: 0 }).animate({ height: 40 }, { duration: 200 });
                            searchresult.$.css({ top: 40 }).animate({ top: 80 }, { duration: 200 });
                        });
                    }
                }
            };

            var relatedTitleBox = null, relatedTitleText = null;
            var otherTitleBox = null, otherTitleText = null;

            var relatedBoxes = [], relatedQuery = [], relatedExplaination = [];
            setRelated = function (queries, explainations, otherQueries, otherExplainations, didYouMean, didntUnderstand) {
                    
                if (relatedTitleBox == null) {
                    relatedTitleBox = ui.box(related)({ size: ["100%", 40], color: 85 });
                    relatedTitleText = ui.text(relatedTitleBox)({ font: ["body", 11], color: 45, left: 20, top: 13, text: "BASED ON YOUR SEARCH" });

                    otherTitleBox = ui.box(related)({ size: ["100%", 40], color: 85 });
                    otherTitleText = ui.text(otherTitleBox)({ font: ["body", 11], color: 45, left: 20, top: 13, text: "YOU MIGHT WANT TO TRY THIS" });

                    $$.for(0, 10, function (i) {
                        var b = ui.box(related)({
                            size: ["100%", 60], hide: true, color: i % 2 == 0 ? 91 : 93, cursor: "pointer", z: 100,
                            button: {
                                normal: function () {
                                    b.stop();
                                    b.color(i % 2 == 0 ? 91 : 93, { duration: 100 });
                                },
                                over: function () { b.color(97); },
                                click: function () {
                                    center.searchExecute(relatedQuery[i].text());
                                }
                            }
                        });
                        relatedBoxes[i] = b;
                        relatedQuery[i] = ui.text(b)({ font: ["body", 15], left: 20, top: 10 });
                        relatedExplaination[i] = ui.text(b)({ font: ["body", 13], left: 20, top: 30, color: 50 });
                    });
                }

                var endQuery = queries == null ? 0 : queries.length;
                var endOther = endQuery + (otherQueries == null ? 0 : otherQueries.length);
                    
                for (var i = 0; i < 10; i++) {
                        
                    if (i < endQuery || i < endOther) {                            
                        if (i < endQuery) {
                            relatedBoxes[i]({ show: true, top: 40 + (i * 60) });
                            relatedQuery[i].text(queries[i]);
                            if (explainations[i] != null) relatedExplaination[i]({ show: true, text: explainations[i] });
                            else relatedExplaination[i].hide();
                        }
                        else {
                            relatedBoxes[i]({ show: true, top: 80 + (i * 60) });
                            relatedQuery[i].text(otherQueries[i - endQuery]);
                            if (otherExplainations[i] != null) relatedExplaination[i]({ show: true, text: otherExplainations[i - endQuery] });
                            else relatedExplaination[i - endQuery].hide();
                        }
                    }
                    else relatedBoxes[i].hide();
                }

                if (queries != null && queries.length > 0) {
                    relatedTitleBox.show();
                }
                else {
                    relatedTitleBox.hide();
                }
                if (otherQueries != null && otherQueries.length > 0) {
                    otherTitleBox({ show: true, top: 40 + endQuery * 60 });
                }
                else {
                    otherTitleBox.hide();
                }

                if (didYouMean) {
                    related.show();
                }
            };
            
            animTransferedClick = function () {
                if (filter.isShown()) {
                    titlearea.$.css({ y: 0 }).transition({ y: -80, duration: 300 });
                    filter.$.css({ y: 0 }).transition({ y: -80, duration: 300 });
                    searchresult.$.css({ y: 0 }).transition({ y: -80, duration: 300 });
                }
                else {
                    titlearea.$.css({ y: 0 }).transition({ y: -40, duration: 300 });
                    searchresult.$.css({ y: 0 }).transition({ y: -40, duration: 300 });
                }
            };

            p.done();
        },
        start: function (p) {
            searchDo(p);
            p.done();
        },
        resize: function (p) {
        },
        local: function (p) {
            searchDo(p);
        },
        unload: function (p) {
            $.each(registerstream, function (rsi, rsv) {
                $$.removeRegister(rsi);
                delete registerstream[rsi];
            });
            p.done();
        }
    });
})();