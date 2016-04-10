(function () {

    var uipage;

    // functions
    var showLoading, hideLoading;
    var enterSearchResult, setResults, setFilters, clearSearchResult, setRelated;
    var isfiltersexists = false, ispagingexists = false, isnomatchexists = false;
    var necrowonline = false;

    var searchJQXHR;
    var search, columns, results, sortList, sortBy, sortType, page, npage, mpage, count, type, subType, searchid = null, filters;
    var registerstream = {};
      
    function setQuery(q) {
        q = q.replaceAll(' ', '+');
        q = escape(q);
        return q;
    };
    function searchExecute(query) {
        uipage.transfer("/search/" + setQuery(query));
    };
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

            jovice.setSearchBoxValue(search);

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

            showLoading();            
            searchJQXHR = $$.get(101, { s: search, p: page, n: npage, o: sortBy, ot: sortType, m: 1 }, function (d) {
                hideLoading();
                count = d.n; type = d.t; subType = d.st;
                var refsearch = d.rs;
                if (refsearch != null) {
                    jovice.setSearchBoxValue(refsearch);
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
                showLoading();
                //debug(page, npage, search);
                searchJQXHR = $$.get(101, { s: search, p: page, n: npage, o: sortBy, ot: sortType, sid: searchid, m: 1 }, function (d) {
                    hideLoading();
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

    ui("search", {
        init: function (p) {
            uipage = jovice.init(p);

            var area = ui.box(p)({ size: ["100%", "100%"], color: 100, hide: true, z: 1000, opacity: 0.6 });
            var circleBox = ui.box(p)({ size: [50, 50], center: true, z: 1001 });
            var circle = ui.loading(circleBox)({ size: [50, 50], stop: true });
                
            showLoading = function () {
                if (!area.isShown()) {
                    area.show();
                    circleBox.fadeIn();
                    circle.start();
                }
            };
            hideLoading = function () {
                if (area.isShown()) {
                    area.hide();
                    circleBox.fadeOut(200, function () {
                        circle.stop();
                    });
                }
            };

            var titlearea = ui.box(p)({ size: ["100%", 60], top: 0 });

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
                        searchresult.show();
                            
                        related.hide();
                    }
                }
            });
            var title1 = ui.text(titleBox1)({ font: ["head", 17], color: "accent", left: 20, bottom: 18, noBreak: true });
            var focusBorder = ui.box(titlearea)({ height: 3, width: 176, color: "accent", bottom: 0, left: 20, hide: true });
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
            var title2 = ui.text(titleBox2)({ font: ["head", 17], color: 35, left: 20, bottom: 18, noBreak: true });
                
            var filter = ui.box(p)({ width: "100%", top: 60, height: 45, color: 85, hide: true});                
            var searchresult = ui.box(p)({ width: "100%", topBottom: [60, 0], scroll: { horizontal: false }, hide: true });
            var related = ui.box(p)({ width: "100%", topBottom: [60, 0], scroll: { horizontal: false }, hide: true, color: 90 });
            var nomatch = ui.box(p)({ width: "100%", topBottom: [60, 0], hide: true, color: 90 });

            var filterHead = ui.text(filter)({ text: "SORT BY", font: ["body", 13], color: 45, left: 20, top: 15 });
            var pagingarea = ui.box(titlearea)({ height: "100%", right: 40, width: 270, hide: true });

            var nomatchtext = ui.text(nomatch)({ top: 20, leftRight: [20, 20], font: ["body", 17], text: "NO MATCH TEXT HERE" });

            var pagebox = [];
            var pagetext = [];
            $$.for(0, 9, function (index) {
                pagebox[index] = ui.box(pagingarea)({
                    height: 60, hide: true, button: {
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
                pagetext[index] = ui.text(pagebox[index])({ text: "", left: 5, bottom: 17, font: ["head", 17], color: "accent" });
            });
            var trimLastBox = ui.box(pagingarea)({ height: 60, width: 20, hide: true });
            var trimLastText = ui.text(trimLastBox)({ text: "...", left: 4, bottom: 17, font: ["head", 17], color: 35 });
            var trimNextBox = ui.box(pagingarea)({ height: 60, width: 20, hide: true });
            var trimNextText = ui.text(trimNextBox)({ text: "...", left: 4, bottom: 17, font: ["head", 17], color: 35 });

            var nextbutton = ui.box(titlearea)({
                size: [40, 60], right: 0, cursor: "pointer", hide: true,
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
                size: [25, 25], bottom: 14, left: 5
            });
            var backbutton = ui.box(titlearea)({
                size: [40, 60], right: 340, cursor: "pointer", hide: true,
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
                size: [25, 25], bottom: 14, right: 5, rotation: 180
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
                ui.script("search_" + type, function (proc) {

                    if (title2.text() == "DID YOU MEAN") {
                        searchresult.hide();
                        related.show();
                    }
                    else {
                        searchresult.show();
                        related.hide();
                    }

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
                        f.column = function(name) {
                            if (columns.length > 0) {
                                var ic = columns.indexOf(name);
                                if (ic > -1) return ev[ic];
                            }
                            return null;
                        }
                        f.setButton = function() {
                            b({
                                cursor: "pointer",
                                button: {
                                    normal: function () {
                                        b.color(index % 2 == 0 ? 91 : 93, { duration: 100 });
                                    },
                                    over: function () {
                                        b.color(97);
                                    },
                                }
                            });
                        };
                        f.clearButton = function() {
                            b({ cursor: null, button: null });
                        };
                        f.setNormal = function (s) {
                            b({ height: s, top: ei * s });
                        }
                        f.setExpand = function(normalHeight, expandHeight, enter, leave, step, complete) {
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
                        
                        proc(b, r, f);

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
            };
            setFilters = function (entries) {

                if (isfiltersexists) filter.show();
                isfiltersexists = false;
                    
                if (entries == null) {
                    if (filter.isShown()) {
                        filter.$.css({ height: 45 }).animate({ height: 0 }, { duration: 200, complete: function () { filter.hide(); } });
                        searchresult.$.css({ top: 105 }).animate({ top: 60 }, { duration: 200 });
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
                                font: ["body", 13], left: 10, top: 15, noBreak: true, color: 20
                            });
                            r.icon1 = ui.icon(b, "arrow2")({
                                size: [10, 10], top: 16, right: 10, hide: true
                            });
                            r.icon2 = ui.icon(b, "arrow2")({
                                size: [10, 10], top: 16, right: 10, hide: true, rotation: 180
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
                                        searchModify("sort:" + key + ":asc");
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
                                        searchModify("sort:" + key + ":desc");
                                    }
                                }
                            }
                        });

                        b({ width: 40 + r.text.textSize().width, left: left });
                        left += b.width();
                    });

                    if (!filter.isShown()) {
                        $$(500, function () {
                            filter.$.show().css({ height: 0 }).animate({ height: 45 }, { duration: 200 });
                            searchresult.$.css({ top: 60 }).animate({ top: 105 }, { duration: 200 });
                        });
                    }
                }
            };

            var relatedTitleBox = null, relatedTitleText = null;
            var otherTitleBox = null, otherTitleText = null;

            var relatedBoxes = [], relatedQuery = [], relatedExplaination = [];
            setRelated = function (queries, explainations, otherQueries, otherExplainations, didYouMean, didntUnderstand) {
                    
                if (relatedTitleBox == null) {
                    relatedTitleBox = ui.box(related)({ size: ["100%", 45], color: 85 });
                    relatedTitleText = ui.text(relatedTitleBox)({ font: ["body", 13], color: 45, left: 20, top: 15, text: "BASED ON YOUR SEARCH" });

                    otherTitleBox = ui.box(related)({ size: ["100%", 45], color: 85 });
                    otherTitleText = ui.text(otherTitleBox)({ font: ["body", 13], color: 45, left: 20, top: 15, text: "YOU MIGHT WANT TO TRY THIS" });

                    $$.for(0, 10, function (i) {
                        var b = ui.box(related)({
                            size: ["100%", 80], hide: true, color: i % 2 == 0 ? 91 : 93, cursor: "pointer", z: 100,
                            button: {
                                normal: function () {
                                    b.stop();
                                    b.color(i % 2 == 0 ? 91 : 93, { duration: 100 });
                                },
                                over: function () { b.color(97); },
                                click: function () {
                                    p.transfer("/search/" + setQuery(relatedQuery[i].text()));
                                }
                            }
                        });
                        relatedBoxes[i] = b;
                        relatedQuery[i] = ui.text(b)({ font: ["body", 18], left: 20, top: 15 });
                        relatedExplaination[i] = ui.text(b)({ font: ["body", 15], left: 20, top: 40, color: 50 });
                    });
                }

                var endQuery = queries == null ? 0 : queries.length;
                var endOther = endQuery + (otherQueries == null ? 0 : otherQueries.length);
                    
                for (var i = 0; i < 10; i++) {
                        
                    if (i < endQuery || i < endOther) {                            
                        if (i < endQuery) {
                            relatedBoxes[i]({ show: true, top: 45 + (i * 80) });
                            relatedQuery[i].text(queries[i]);
                            if (explainations[i] != null) relatedExplaination[i]({ show: true, text: explainations[i] });
                            else relatedExplaination[i].hide();
                        }
                        else {
                            relatedBoxes[i]({ show: true, top: 90 + (i * 80) });
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
                    otherTitleBox({ show: true, top: 45 + endQuery * 80 });
                }
                else {
                    otherTitleBox.hide();
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