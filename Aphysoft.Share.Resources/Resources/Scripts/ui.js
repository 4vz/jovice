/*! Aphysoft.Share UI | share.aphysoft.com */
(function ($) {
    "use strict";
    
    var ui;

    // ui .load .marginTop .marginLeft .marginBottom .marginRight
    (function (window, share, jQuery) {

        var loaded = false;
        var count = 0;
        var transferring;
        var pages = [];
        var css = {};
        var transferData = null;
        var objects = {};
        var scripts = {};
        var ajaxify;
        var contentProviderUrl;
        var urlPrefix;
        var margin = { top: 0, left: 0, right: 0, bottom: 0 };
        var _new = true;

        var topContainer = $("#top");
        var bottomContainer = $("#bottom");
        var pagesContainer = $("#pages");

        var tooltipBox;

        // set up top and bottom container "page" object
        (function () {

            var event = new function () {
                var o = arguments;
                return share.event(o[0], o[1], o[2], o[3], o[4]);
            };
            var removeEvent = new function (arg1) {
                share.removeEvent(arg1);
            };

            var topObject = function () {
                var o = arguments;
                if ($.isString(o[0])) {
                    return $(o[0], topContainer);
                }
            };
            topObject.event = event;
            topObject.removeEvent = removeEvent;
            topContainer.data("page", topObject);

            var bottomObject = function () {
                var o = arguments;
                if ($.isString(o[0])) {
                    return $(o[0], bottomContainer);
                }
            };
            bottomObject.event = event;
            bottomObject.removeEvent = removeEvent;
            bottomContainer.data("page", bottomObject);


        })();

        ui = function () {

            if ($.isJQuery(arguments[0])) {
                return arguments[0].ui();
            }

            var o = share.args(arguments,
                "string family",
                "string pageUrl",
                "string scriptUrl",
                "string cssUrl",
                "string content",
                "string title",
                "array titles",
                "array urls",
                "array data",
                "array pageData",
                "optional object stateData");

            if (o) {

                // init functions
                if (o.title == "") o.title = null;

                if (loaded == false) {
                    loaded = true;

                    // ajaxify                                     
                    if (ajaxify && History.enabled == false) { // unfortunately, IE<10 is not supported
                        ajaxify = false;
                    }

                    // tooltip
                    tooltipBox = ui.box(topContainer)({
                        z: 1000, hide: true
                    });

                    (function () {
                        var tooltipContents = ui.box(tooltipBox)({
                            color: 15
                        });
                        var tooltipArrowTop = ui.raphael(tooltipBox)({ size: [16, 16], top: 0, left: 20 });
                        tooltipArrowTop.paper().path("M0,8L16,8L8,0Z").attr({ fill: ui.color(15), stroke: "none" });
                        var tooltipArrowBot = ui.raphael(tooltipBox)({ size: [16, 16], left: 20 });
                        tooltipArrowBot.paper().path("M0,8L16,8L8,16Z").attr({ fill: ui.color(15), stroke: "none" });

                        tooltipBox.data("contents", tooltipContents);
                        tooltipBox.data("arrowtop", tooltipArrowTop);
                        tooltipBox.data("arrowbot", tooltipArrowBot);
                    })();

                    // set container
                    pagesContainer.css(margin);

                    // resize
                    share.resize("ui", function () {

                        margin.top = parseInt(pagesContainer.css("top"));
                        margin.left = parseInt(pagesContainer.css("left"));
                        margin.right = parseInt(pagesContainer.css("right"));
                        margin.bottom = parseInt(pagesContainer.css("bottom"));

                        var topPage = pages.length > 0 ? pages[pages.length - 1] : null;
                        if (topPage != null) {

                            topPage.sizing = share.sizing();
                            topPage.group = share.sizeGroup();
                            topPage.resize(topPage);
                        }
                    });

                    // create page skeleton
                    var main = $("#main");
                    main.removeAttr("style");
                    $("#nojs").remove();

                    // ajaxify                                        
                    if (ajaxify) {

                        var State = History.getState();

                        if (State.data.family == undefined) {
                            // new page?, no state data?
                            //debug("no family data, replace state, reload page");

                            History.replaceState({ family: o.family, data: null }, share.title(o.title), urlPrefix + o.pageUrl);
                            State = History.getState();

                            //debug("new: " + urlPrefix + o.pageUrl);
                            //debug(State.data);
                        }

                        if (State.data.data != null) {
                            o.stateData = State.data.data;
                        }
                        //debug("adding statechange");

                        History.Adapter.bind(window, "statechange", function () {

                            var State = History.getState();
                            var ofamily = State.data.family;

                            if (ofamily == undefined) return; // kita cuma replace ofamily undefined menjadi not undefined

                            var rawUrl = State.url
                            var appHref = share.url(rawUrl, "{path}{?search}").substr(urlPrefix.length);

                            var topFamily = null;

                            var topPage = pages.length > 0 ? pages[pages.length - 1] : null;
                            if (topPage != null)
                                topFamily = topPage.family;

                            var stateData = State.data.data;

                            var nocallback = false;

                            if (ui.__nocallback == true) {
                                delete ui.__nocallback;
                                nocallback = true;
                            }


                            if (topFamily != null) {

                                if (ofamily != topFamily) {  // different family, global link

                                    if (!nocallback) {
                                        $.getJSON(contentProviderUrl, { p: appHref }, function (d) {
                                            ui(d.f, appHref, d.s, d.u, d.c, d.t, d.y, d.z, d.d, d.x, stateData);
                                        });
                                    }
                                }
                                else { // same family, local link
                                    topPage.data(stateData);
                                    topPage.url(appHref);

                                    transferring = false;

                                    if (!nocallback)
                                        topPage.local(topPage);
                                }
                            }
                        });
                    }
                    else {
                        // no ajaxify
                    }

                    // unload
                    share.unload(function () {
                        $.each(pages, function (pi, pv) {
                            pv.unload(page);
                        });
                    });
                }

                // update share.data
                if (o.data != null) {
                    var key = null;
                    $.each(o.data, function (i, v) { if (i % 2 == 0) key = v; else share.data(key, v); });
                }

                // create page object
                var data = {};
                if ($.isPlainObject(o.stateData)) {
                    data = o.stateData;
                }
                var eventHandlers = [];
                var streamHandlers = [];
                var title = o.title;
                var state = 0;
                var id = (++count);
                var last = pages.length > 0 ? pages[pages.length - 1] : null;
                var next = null;
                var transitioning = false;
                var noTransition = false;

                var page = function () {
                    var o = arguments;
                    if ($.isString(o[0])) {
                        return $(o[0], page.$);
                    }
                };

                var url = null;
                var baseUrl = null;
                var endUrl = null;
                var transferLeave = null;

                page.lastPage = last;
                page.family = o.family;
                page.titles = o.titles;
                page.urls = o.urls;

                page.url = function (arg1) {

                    if ($.isString(arg1)) {
                        var purl = share.url(arg1);
                        var burl = null;
                        var eurl = null;

                        $.each(o.urls, function (i, v) {
                            if (v.length > 1 && v.endsWith("?") && purl.path.startsWith(v.substr(0, v.length - 1))) {
                                burl = v.substr(0, v.length - 1);
                                eurl = arg1.substr(burl.length);
                                return false;
                            }
                            else if (purl.path == v) {
                                burl = v;
                                eurl = "";
                                return false;
                            }
                        });

                        if (burl != null) {
                            baseUrl = burl;
                            endUrl = eurl;
                            url = arg1;
                        }
                    }
                    else if ($.isBoolean(arg1) && arg1 == true) return share.url(url);
                    else return url;
                };
                page.baseUrl = function () {
                    return baseUrl;
                };
                page.endUrl = function () {
                    return endUrl;
                };
                page.$ = null;
                page.type = "page";
                page.height = function () {
                    return share.pageHeight() - margin.top - margin.bottom;
                };
                page.width = function () {
                    return share.pageWidth() - margin.left - margin.right;
                };
                page.event = function () {
                    var o = arguments;

                    if ($.isArray(o[0])) {
                        $.each(o[0], function (i, v) {
                            if ($.isNumber(v) || $.isArray(v))
                                page.event(v);
                        });
                    }
                    else if ($.isNumber(o[0])) {
                        eventHandlers.push(o[0]);
                    }
                    else {
                        var ei = share.event(o[0], o[1], o[2], o[3], o[4], o[5]);
                        eventHandlers.push(ei);
                        return ei;
                    }
                };
                page.removeEvent = function (arg1) {
                    if ($.isNumber(arg1) || $.isString(arg1)) {
                        share.removeEvent(arg1);
                        var ix = eventHandlers.indexOf(arg1);
                        if (ix > -1) {
                            eventHandlers.splice(ix, 1);
                        }
                    }
                    else if ($.isArray(arg1)) {
                        $.each(arg1, function (i, v) {
                            page.removeEvent(v);
                        });
                    }
                };
                page.stream = function (c) {
                    var si = share.stream(c);
                    streamHandlers.push(si);
                    return si;
                };
                page.removeStream = function (d) {
                    if ($.isNumber(d)) {
                        share.removeStream(d);
                        var ix = streamHandlers.indexOf(d);
                        if (ix > -1) {
                            streamHandlers.splice(ix, 1);
                        }
                    }
                    else if ($.isArray(d)) {
                        $.each(d, function (i, v) {
                            page.removeStream(v);
                        });
                    }
                };
                page.data = function () {
                    var o = arguments;
                    if ($.isPlainObject(o[0])) {
                        $.each(o[0], function (i, v) { data[i] = v; });
                    }
                    else if ($.isString(o[0])) {
                        if ($.isUndefined(o[1])) {
                            var d = data[o[0]];
                            return $.isUndefined(d) ? null : d;
                        }
                        else if ($.isNull(o[1])) {
                            var d = data[o[0]];
                            if (!$.isUndefined(d)) delete data[o[0]];
                        }
                        else data[o[0]] = o[1];
                    }
                    else return data;
                };
                page.state = function () {
                    var o = arguments;
                    if ($.isNumber(o[0])) state = o[0];
                    else return state;
                };
                page.last = function () {
                    var o = arguments;
                    if (o[0] != null) last = o[0];
                    else return last;
                };
                page.next = function () {
                    var o = arguments;
                    if (o[0] != null) next = o[0];
                    else return next;
                };
                page.back = function () {
                    History.back();
                };
                page.forward = function () {
                    History.forward();
                };
                page.transitioning = function () {
                    var o = arguments;
                    if ($.isBoolean(o[0])) transitioning = o[0];
                    else return transitioning;
                };
                page.isNoTransition = function () {
                    return noTransition;
                };
                page.cancelTransition = function () {
                    noTransition = true;
                };
                page.transfer = function (arg1, arg2) {
                    if ($.isString(arg1)) {

                        if (transferring) return;// transfer busy

                        if (arg1 == "") arg1 = "/";
                        if (!arg1.startsWith(urlPrefix)) return; // not began with url prefix, cannot transfer

                        var appHref = arg1.substr(urlPrefix.length); // trim url prefix
                        var appHrefClean = share.url(appHref, "{path}");

                        if (escape(appHref) == escape(page.url())) {
                            return; // already on target page;
                        }
                        // appHref ==> /urlPrefix/path/to/page?querystrings

                        if (ajaxify) {
                            //if (page.location == appHrefClean)
                            var globallink = false;
                            var replaceState = false;
                            var stateData = null;
                            var noCallback = false;

                            transferData = {
                                data: null,
                                transition: null
                            };


                            if (globallink == false) {
                                // cek for local link first
                                globallink = true;
                                $.each(page.urls, function (i, v) {
                                    if (v.length > 1 && v.endsWith("?")) {
                                        var vs = v.substr(0, v.length - 1);
                                        if (appHrefClean.startsWith(vs)) {
                                            globallink = false;
                                            return false;
                                        }
                                    }
                                    else {
                                        if (appHrefClean == v) {
                                            globallink = false;
                                            return false;
                                        }
                                    }
                                });
                            }

                            if ($.isPlainObject(arg2)) {
                                if ($.isBoolean(arg2.replace) && arg2.replace) replaceState = true;
                                if ($.isBoolean(arg2.nocallback) && arg2.nocallback) noCallback = true;
                                if ($.isPlainObject(arg2.transferData)) transferData.data = arg2.transferData;
                                if ($.isPlainObject(arg2.data)) stateData = arg2.data;
                                if ($.isString(arg2.transition)) transferData.transition = arg2.transition;
                                if ($.isFunction(arg2.leave)) transferLeave = arg2.leave;
                            }

                            if (replaceState && noCallback) {
                                ui.__nocallback = true;
                                History.replaceState(
                                    { family: page.family, data: stateData, nocallback: true },
                                    share.title(createTitle(appHref, page)),
                                    urlPrefix + appHref);
                            }
                            else {
                                if (replaceState)
                                    globallink = true;

                                transferring = true;

                                if (!globallink) {
                                    History.pushState(
                                        { family: page.family, data: stateData },
                                        share.title(createTitle(appHref, page)),
                                        urlPrefix + appHref);
                                }
                                else {
                                    // global                                
                                    $.getJSON(contentProviderUrl, { p: appHref, t: 1 }, function (d) {
                                        if (d.f == "system_status_404") {
                                            // TODO
                                            var todo = page.error("brokenlink", urlPrefix + appHref);
                                            if (todo == undefined || todo == true) {
                                                // bring to 404
                                                alert("404");
                                            }
                                            //dataUI.transferring = false;
                                            //return;
                                        }

                                        if (replaceState) {
                                            History.replaceState(
                                                { family: d.f, data: stateData },
                                                share.title(d.t),
                                                urlPrefix + appHref);
                                        }
                                        else {
                                            History.pushState(
                                                { family: d.f, data: stateData },
                                                share.title(d.t),
                                                urlPrefix + appHref);
                                        }
                                    });
                                }
                            }
                        }
                        else {
                            document.location = urlPrefix + appHref;
                        }
                    }
                    else if (arg1 == null) {
                        if (page.prototype.transfer == null) return null;
                        else return page.prototype.transfer.data;
                    }
                }; // arg1: <string: url>, arg2: <plainObject: { data: <object: data>, transferData: <object: data>, transition: <string: transition> }
                page.css = function (arg1) {

                    if ($.isString(arg1)) {
                        if (css[arg1] == null && arg1 != null && arg1 != "") {
                            var pcss = { obj: null, users: [] };
                            var obj = $("<link />");

                            pcss.obj = obj;
                            obj.attr({ href: arg1, rel: "stylesheet", type: "text/css" });

                            $("head").append(obj);

                            pcss.users.push(page);
                            css[arg1] = pcss;
                        }
                        else if (arg1 != null && arg1 != "") {
                            var pcss = css[arg1];
                            var add = true;

                            $.each(pcss.users, function (i, v) {
                                if (v == page) {
                                    add = false;
                                    return false;
                                }
                            });

                            if (add)
                                pcss.users.push(page);
                        }
                    }
                };
                page.title = function (arg1) {

                    if ($.isString(arg1)) {
                        var old = title;
                        if (arg1.length == 0)
                            title = "";
                        else
                            title = arg1;
                        if (old != title)
                            document.title = share.title(title);
                    }
                    else
                        return title;

                };
                page.done = function () {

                    //debug("done by " + page.prototype.id + " state " + page.state());

                    if (state == 0) { // done by init

                        $("a[href]", this.$).each(function (i) {
                            var e = $(this);
                            var hrefstr = e.attr("href");
                            var d = e.attr("data-transfer");
                            var transition = e.attr("data-transition");

                            e.attr("data-transfer", null);
                            e.attr("data-transition", null);

                            if (!hrefstr.startsWith(urlPrefix))
                                e.attr("href", urlPrefix + hrefstr);

                            e.click(function (event) {
                                event.preventDefault();
                                page.transfer(e.attr("href"), { transferData: d, transition: transition });
                            });
                        });
                        share.doResize("ui", page);
                        _new = false;

                        state = 1;

                        // call exit for last page if available, exit is leave
                        if (last != null) {

                            last.state(3);
                            last.leave(last);

                            // if last page available, check for page transition
                            var transition = null;

                            if (page.prototype.transfer == null) {
                                if (last.lastPage != null) {
                                    if (last.lastPage.family == page.family && last.prototype.transfer != null) {
                                        var lastTransition = last.prototype.transfer.transition;
                                        if (lastTransition == "slideleft") transition = "slideright";
                                        else if (lastTransition == "slideright") transition = "slideleft";
                                    }
                                }
                            }
                            else
                                transition = page.prototype.transfer.transition;

                            if (transition != null && page.isNoTransition() == false) {
                                last.transitioning(true);

                                var jqThisPage = page.$;
                                var jqLastPage = last.$;
                                var onCompleted = function () {
                                    last.transitioning(false);
                                    last.done();
                                }

                                var ndur = 250;

                                transition.is("slideright", function () {
                                    var width = jqThisPage.width();

                                    jqLastPage.css({ left: 0 }).animate({ left: width }, { duration: ndur, complete: onCompleted });
                                    jqThisPage.css({ left: -width }).animate({ left: 0 }, { duration: ndur });
                                });
                                transition.is("slideleft", function () {
                                    var width = jqThisPage.width();

                                    jqLastPage.css({ left: 0 }).animate({ left: -width }, { duration: ndur, complete: onCompleted });
                                    jqThisPage.css({ left: width }).animate({ left: 0 }, { duration: ndur });
                                });
                                transition.is("fadein", function () {
                                    jqLastPage.css({ opacity: 1 }).animate({ opacity: 0 }, { duration: ndur, complete: onCompleted });
                                    jqThisPage.css({ opacity: 0 }).animate({ opacity: 1 }, { duration: ndur });
                                });
                            }
                        }

                        page.start(page);
                    }
                    else if (page.state() == 1) { // done by start

                        page.state(2);

                        if (last != null) {
                            if (last.state() == 4) {
                                last.unload(last);
                            }
                        }
                    }
                    else if (page.state() == 3) { // done by leave

                        if (transferLeave != null) {
                            transferLeave();
                            transferLeave = null;
                        }

                        page.state(4);

                        share.removeEvent(eventHandlers);
                        share.removeStream(streamHandlers);

                        if (next.state() == 2) {
                            page.unload(page);
                        }
                    }
                    else if (page.state() == 4) { // done by unload

                        if (page.transitioning()) {
                            // hold
                            //debug("cancel unload... transition in progress");
                        }
                        else {
                            transferring = false;

                            // stop all animation
                            $("*", page.$).stop();

                            $.each(css, function (ci, cv) {

                                var ri = -1;
                                $.each(cv.users, function (xui, xuv) {
                                    if (xuv == page) {
                                        ri = xui;
                                        return false;
                                    }
                                });
                                if (ri > -1) {
                                    cv.users.splice(ri, 1);
                                }
                            });

                            var removeThisCss = [];

                            $.each(css, function (ci, cv) {
                                if (cv.users.length == 0) {
                                    cv.obj.remove();
                                    cv.obj = null;
                                    removeThisCss.push(ci);
                                }
                            });

                            $.each(removeThisCss, function (ri, rv) {
                                delete css[rv];
                            });

                            page.$.remove();

                            var removeFromPages = -1;
                            $.each(pages, function (pi, pv) {
                                if (pv == this) {
                                    removeFromPages = pi;
                                    return false;
                                }
                            });

                            if (removeFromPages > -1)
                                pages.splice(removeFromPages, 1);

                            if (next != null)
                                next.last(null);
                            next = null;
                        }
                    }
                };
                page.isReady = function () {
                    if (state >= 2) return true;
                    else return false;
                };
                page.init = function () { this.done(); };
                page.start = function () { this.done(); };
                page.leave = function () { this.done(); };
                page.unload = function () { this.done(); };
                page.local = function () { };
                page.resize = function () { };
                page.error = function () { };
                page.group = share.sizeGroup();
                page.sizing = share.sizing();
                page.debugEvents = function () {
                    debug(eventHandlers);
                };

                page.url(o.pageUrl);

                if (last != null) { last.next(page); }
                pages.push(page);
                page.css(o.cssUrl);

                var pageload = function () {
                    var oscript = ui.page(o.family);

                    if (oscript != null) {
                        page = $.extend(true, page, oscript);
                    }

                    if (transferData != null) page.prototype.transfer = transferData;
                    else page.prototype.transfer = null;
                    transferData = null;

                    var pageDataKey;
                    $.each(o.pageData, function (pdi, pdv) {
                        if (pdi % 2 == 0) pageDataKey = pdv;
                        else data[pageDataKey] = pdv;
                    });

                    /*if (transferData.data != null) {
                        if ($.isPlainObject(transferData.data)) {
                            $.each(transferData.data, function (tdi, tdv) {
                                data[tdi] = tdv;
                            });
                        }    
                    }*/

                    var dpage = div(pagesContainer).addClass("_PG");
                    var dcontent = div(dpage).addClass("_CO").html(o.content);

                    //div.css({ zIndex: 100 });   
                    dpage.data("page", page);
                    page.$ = dpage;

                    //page.content = dcontent;

                    // INIT
                    //debug("page " + page.pageID + " init");
                    share(function () { page.init(page); });
                    page.title(title);
                    //window.page = page;
                }

                if (ui.page(o.family) == null) {
                    ui.load(o.scriptUrl, function () {
                        pageload();
                    });
                }
                else pageload();
            }
            else {
                var ors = share.args(arguments, "required string name", "required object/function object");

                if (ors) {
                    if ($.isPlainObject(ors.object)) {
                        // page object
                        objects[ors.name] = ors.object;
                    }
                    else {
                        // general scripts
                        scripts[ors.name] = ors.object;
                    }
                }
            }
        };
        jQuery.fn.ui = function () {
            var pui = jQuery.data(this.get(0), "ui");
            if (pui != undefined) return pui;
            else return null;
        };

        var load = function () {
            var o = share.args(arguments, "required string scriptUrl", "function complete");

            if (o) {
                $.ajax({
                    url: o.scriptUrl, dataType: "script", cache: true, complete: function () {
                        if (o.complete != null) o.complete();
                    }
                });
            }
        };
        var marginTop = function (s) {
            if (s == undefined) return margin.top;
            else {
                pagesContainer.css({ top: s });
                margin.top = s;
            }
        };
        var marginBottom = function () {
            return margin.bottom;
        };
        var marginLeft = function () {
            return margin.left;
        };
        var marginRight = function (s, a) {
            if (s == undefined) return margin.right;
            else {
                if ($.isPlainObject(a)) {
                    a.step = function (now) {
                        margin.right = now;
                    };
                    pagesContainer.animate({ right: s }, a);
                }
                else {
                    pagesContainer.css({ right: s });
                    margin.right = s;
                }
            }
        };
        var isNew = function () {
            return _new;
        };
        var remove = function () {
            var o = arguments;

            var v = o[0];

            if ($.isJQuery(v)) {

                v.children().each(function (ii, vv) {
                    remove($(vv));
                });

                var vo = v.data("ui");

                if (vo != null) {
                    if (vo.type != "page") {
                        vo.remove();
                    }
                }
                else
                    v.remove();
            }
        };
        var page = function (c) {
            if ($.isString(c)) {
                var object = objects[c];
                return $.isUndefined(object) ? null : $.extend(true, {}, object);
            }
            else return pages[0];
        };
        var script = function (c, callback) {
            if ($.isString(c) && $.isFunction(callback)) {
                var _script = scripts[c];

                if ($.isUndefined(_script)) {
                    $.getJSON(contentProviderUrl, { vk: c }, function (d) {
                        ui.load(d.s, function () {
                            _script = scripts[c];
                            if (_script != null) {
                                callback(_script);
                            }
                        });
                    });
                }
                else {
                    callback(_script);
                }
            }
        };

        var _topContainer = function () {
            return topContainer;
        };
        var _bottomContainer = function () {
            return bottomContainer;
        };
        var _tooltipBox = function () {
            return tooltipBox;
        };

        var version = "1.02";
        ui.prototype = new Object();
        ui.prototype.ui = "Aphysoft Share UI " + version + " by Afis Herman Reza Devara";
        ui.toString = function () {
            return "Aphysoft Share UI " + version;
        };

        function div(par) {
            if ($.isJQuery(par)) {
                var el = $("<div />");
                par.append(el);
                return el;
            }
            else return null;
        };
        function createTitle(arg1, arg2) {

            if ($.isString(arg1) && arg2 == null) {
                return share.title(arg1);
            }
            else if ($.isString(arg1) && $.isFunction(arg2)) {
                var title = arg2.title /*default*/,
                    urls = arg2.urls,
                    titles = arg2.titles;

                var appHrefClean = share.url(arg1, "{path}");

                $.each(urls, function (i, v) {
                    if (v.length > 1 && v.endsWith("?")) {
                        var vs = v.substr(0, v.length - 1);
                        if (appHrefClean.startsWith(vs)) { title = titles[i]; return false; }
                    }
                    else {
                        if (appHrefClean == v) { title = titles[i]; return false; }
                    }
                });
                return title;
            }
            else return null;
        };

        ui.load = load;
        ui.marginTop = marginTop;
        ui.marginBottom = marginBottom;
        ui.marginLeft = marginLeft;
        ui.marginRight = marginRight;
        ui.isNew = isNew;
        ui.remove = remove;
        ui.topContainer = _topContainer;
        ui.bottomContainer = _bottomContainer;
        ui.tooltipBox = _tooltipBox;
        ui.page = page;
        ui.script = script;

        ui.width = function () {
            return page().width() + marginLeft() + marginRight();
        };
        ui.height = function () {
            return page().height() + marginTop() + marginBottom();
        };

        window.ui = ui;

        $(function () {
            ajaxify = share.system("ajaxify");
            contentProviderUrl = share.system("contentProviderUrl");
            urlPrefix = share.system("urlPrefix");

            console.log("%cCreated with love,\nby " + "%cAfis Herman Reza Devara", "font-size:20px;color:#ff0000", "font-size:20px;color:#ffcc00");
            console.log("%cNo, it's not created with a bootstrap, or whatever javascript framework thats' hot nowdays (angular, react, etc). This is created with my own front-end & back-end framework called " + "%cShare" + "%c that built alongside with this project, https://github.com/afisd/jovice. Sometimes in the future, I'd like to bring this as a standalone framework that can be used by any project. If you want to join me, contact me and I guess you probably know how to do so. Thanks.", "font-size:14px;color:#404040", "font-size:14px;color:#ff0000", "font-size:14px;color:#404040");
        });

    })(window, share, jQuery);

    // .css
    (function (ui) {
        // internal css rule
        // _Fx = FONT
        // _Cx = COLOR
        //
        // _FB = FONTBODY
        // _FH = FONTHEADINGS
        //
        // _CBy = COLOR BACKGROUND
        // _Cy = COLOR FONT
        //   y = 0 ... 100
    })(ui);

    // .font .calculateFont .measureText
    (function (ui) {

        var headings;
        var body;

        var measureTextContext = null;
        var measureTextElement = null;
        var measureTextHeightCache = [];
        var measureTextWidthCache = [];

        var fontCalculateElement = null;
        var calculateFontCache = [];

        var font = function (t) {
            if (t == "body") return body;
            else if (t == "head") return headings;
            else return null;
        };
        var parseFontFamily = function (a) {
            var p = [];
            if ($.isString(a)) {
                var as = a.split(",");
                $.each(as, function (ai, av) {
                    var asv = av.trim();
                    if (asv.length > 0) {
                        if (asv[0] == "\"") asv = asv.substr(1);
                        if (asv[asv.length - 1] == "\"") asv = asv.substr(0, asv.length - 1);
                        p.push(asv);
                    }
                });
            }
            return p;
        };
        var calculateFont = function () {

            if (share.isLoading()) {
                return null;
            }

            var o = share.args(arguments, "string text", "optional string font", "optional number size", "optional string weight", "optional boolean italic");

            if (o) {
                var otext = o.text == "" ? "&nbsp;" : o.text;
                var re = null;

                $.each(calculateFontCache, function (i, v) {
                    if (v.text == otext && v.font == o.font && v.size == o.size) {
                        re = v.wh;
                        return false;
                    }
                });

                if (re == null) {

                    var element;

                    if (fontCalculateElement == null) {
                        fontCalculateElement = $("<div class=\"_BO _FB\" style=\"visibility:hidden;top:0px;left:0px\" />");
                        $(document.body).append(fontCalculateElement);
                    }
                    element = fontCalculateElement;

                    element.html(otext);
                    if (o.font != null) {
                        var font = ui.font(o.font);
                        if (font == null) font = o.font;
                        element.css({ "font-family": font });
                    }
                    if (o.size != null)
                        element.css({ "font-size": o.size });

                    if (o.weight != null)
                        element.css({ "font-weight": o.weight });
                    if (o.italic != null) {
                        if (o.italic)
                            element.css({ "font-style": "italic" });
                    }

                    re = { width: 0, height: 0, floatingWidth: 0, floatingHeight: 0 };
                    re.width = element.width();
                    re.height = element.height();
                    var brec = element[0].getBoundingClientRect();
                    re.floatingWidth = brec.width;
                    re.floatingHeight = brec.height;

                    calculateFontCache.push({ text: otext, font: o.font, size: o.size, wh: re });
                    if (calculateFontCache.length > 100) { // limit cache
                        calculateFontCache.splice(0, 1);
                    }
                }


                return re;
            }

            return null;
        };
        var measureText = function (a1, a2, a3, a4, a5) {

            var t, f, s, w, i;
            t = a1; f = a2; s = a3; w = a4; i = a5;

            if (t == null || f == null || s == null) return null;

            var f1 = ui.font(f);
            if (f1 != null) f = f1;

            if (measureTextContext == null) {
                var measureTextCanvas = $("<canvas />");
                $(document.body).append(measureTextCanvas);
                measureTextContext = measureTextCanvas[0].getContext("2d");
            }
            if (measureTextElement == null) {
                measureTextElement = $("<div style=\"position:absolute;visibility:hidden;top:0px;left:0px\" />");
                $(document.body).append(measureTextElement);
            }

            var ret = {};

            var cacheKey = f + "|" + s + "|" + (w != null ? w : "") + "|" + (i ? "i " : "");
            var cacheHeight = -1;
            var cacheWidth = -1;

            var cacheTextKey = t + "|" + cacheKey;
            $.each(measureTextWidthCache, function (mi, mv) {
                if (mv == cacheTextKey) {
                    cacheWidth = measureTextWidthCache[mi + 1];
                    return false;
                }
            });

            if (cacheWidth == -1) {
                measureTextContext.font = (i ? "italic " : "") + (w != null ? w + " " : "") + s + "px \"" + f + "\"";
                var obj = measureTextContext.measureText(t);
                ret.width = Math.round(obj.width);
                ret.floatingWidth = obj.width;
                measureTextWidthCache.push(cacheTextKey);
                measureTextWidthCache.push(ret.floatingWidth);
                if (measureTextWidthCache.length > 200) { // limit cache
                    measureTextWidthCache.splice(0, 2);
                }
            }
            else {
                ret.width = Math.round(cacheWidth);
                ret.floatingWidth = cacheWidth;
            }

            $.each(measureTextHeightCache, function (mi, mv) {
                if (mv == cacheKey) {
                    cacheHeight = measureTextHeightCache[mi + 1];
                    return false;
                }
            });

            if (cacheHeight == -1) {
                measureTextElement.html("&nbsp;");
                measureTextElement.css({ "font-family": f });
                measureTextElement.css({ "font-size": s });
                measureTextElement.css({ "font-weight": (w != null ? (w + "") : "") });
                measureTextElement.css({ "font-style": (i ? "italic" : "") });
                ret.floatingHeight = measureTextElement[0].getBoundingClientRect().height;
                ret.height = Math.round(ret.floatingHeight);
                measureTextHeightCache.push(cacheKey);
                measureTextHeightCache.push(ret.floatingHeight);
            }
            else {
                ret.height = Math.round(cacheHeight);
                ret.floatingHeight = cacheHeight;
            }

            return ret;
        };

        ui.font = font;
        ui.measureText = measureText;
        ui.parseFontFamily = parseFontFamily;

        $(function () {
            headings = share.system("fontHeadings");
            body = share.system("fontBody");
        });

    })(ui);

    // .color
    (function (ui) {

        var colorAccent;
        var colorMin;
        var colorMax;

        var color = function () {

            var o = arguments;

            if ($.isString(o[0])) {

                var hex;

                if ($.isColor(o[0])) hex = o[0];
                else {
                    var sss = o[0].toLowerCase();
                    var ss = sss.split(/\+|\-/, 2);
                    var s = ss[0];
                    var nn = ss.length > 1 ? parseInt(ss[1]) : null;
                    var n = isNaN(nn) ? null : sss.indexOf('-') > -1 ? -1 * (nn > 100 ? 100 : nn < 0 ? 0 : nn) : (nn > 100 ? 100 : nn < 0 ? 0 : nn);
                    if (s == "accent") hex = colorAccent;
                    else hex = share.color(s);
                }

                if (n != null) {
                    if (n == 100) hex = colorMax;
                    else if (n == -100) hex = colorMin;
                    else {
                        var maxr = parseInt(colorMax.substr(1, 2), 16);
                        var minr = parseInt(colorMin.substr(1, 2), 16);
                        var maxg = parseInt(colorMax.substr(3, 2), 16);
                        var ming = parseInt(colorMin.substr(3, 2), 16);
                        var maxb = parseInt(colorMax.substr(5, 2), 16);
                        var minb = parseInt(colorMin.substr(5, 2), 16);
                        var accr = parseInt(hex.substr(1, 2), 16);
                        var accg = parseInt(hex.substr(3, 2), 16);
                        var accb = parseInt(hex.substr(5, 2), 16);
                        var cr, cg, cb;
                        if (n < 0) {
                            var on = n + 100;
                            cr = Math.floor(minr + (((accr - minr) / 100) * on));
                            cg = Math.floor(ming + (((accg - ming) / 100) * on));
                            cb = Math.floor(minb + (((accb - minb) / 100) * on));
                        }
                        else {
                            var on = n;
                            cr = Math.floor(accr + (((maxr - accr) / 100) * on));
                            cg = Math.floor(accg + (((maxg - accg) / 100) * on));
                            cb = Math.floor(accb + (((maxb - accb) / 100) * on));
                        }

                        hex = "#" + cr.toString(16).paddingLeft(2, 0) + cg.toString(16).paddingLeft(2, 0) + cb.toString(16).paddingLeft(2, 0);
                    }
                }

                if ($.isNumber(o[1])) return share.color(hex, o[1]);
                else return hex;
            }
            else if ($.isNumber(o[0])) {
                if ($.isNumber(o[1]) && $.isNumber(o[2])) {
                    if ($.isNumber(o[3])) return share.color(o[0], o[1], o[2], o[3]);
                    else return share.color(o[0], o[1], o[2]);
                }
                else {
                    var n = o[0];

                    var maxr = parseInt(colorMax.substr(1, 2), 16);
                    var minr = parseInt(colorMin.substr(1, 2), 16);
                    var maxg = parseInt(colorMax.substr(3, 2), 16);
                    var ming = parseInt(colorMin.substr(3, 2), 16);
                    var maxb = parseInt(colorMax.substr(5, 2), 16);
                    var minb = parseInt(colorMin.substr(5, 2), 16);

                    var cr = Math.floor(minr + (((maxr - minr) / 100) * n));
                    var cg = Math.floor(ming + (((maxg - ming) / 100) * n));
                    var cb = Math.floor(minb + (((maxb - minb) / 100) * n));

                    cr = cr < 0 ? 0 : cr > 255 ? 255 : cr;
                    cg = cg < 0 ? 0 : cg > 255 ? 255 : cg;
                    cb = cb < 0 ? 0 : cb > 255 ? 255 : cb;

                    var hex = "#" + cr.toString(16).paddingLeft(2, 0) + cg.toString(16).paddingLeft(2, 0) + cb.toString(16).paddingLeft(2, 0);

                    if ($.isNumber(o[1])) return share.color(hex, o[1]);
                    else return hex;
                }
            }
            else return null;
        };

        ui.color = color;

        $(function () {
            colorAccent = share.system("colorAccent");
            colorMin = share.system("color0");
            colorMax = share.system("color100");
        });

    })(ui);

    // .box
    (function (ui) {

        var box = function (container) {

            if (container == null) return null;

            // CONTAINER ---
            var page = null, parent = null, parentBox = null,
                containerType = 0, boxEvents = [];

            var selfEventHandlers = [];

            function selfEvent() {
                var o = arguments;

                if ($.isArray(o[0])) {
                    $.each(o[0], function (i, v) {
                        if ($.isNumber(v) || $.isArray(v))
                            selfEvent(v);
                    });
                }
                else if ($.isNumber(o[0])) {
                    selfEventHandlers.push(o[0]);
                }
                else {
                    var ei = share.event(o[0], o[1], o[2], o[3], o[4], o[5]);
                    selfEventHandlers.push(ei);
                    return ei;
                }
            };
            function selfRemoveEvent(arg1) {
                if ($.isNumber(arg1) || $.isString(arg1)) {
                    share.removeEvent(arg1);
                    var ix = selfEventHandlers.indexOf(arg1);
                    if (ix > -1) {
                        selfEventHandlers.splice(ix, 1);
                    }
                }
                else if ($.isArray(arg1)) {
                    $.each(arg1, function (i, v) {
                        selfRemoveEvent(v);
                    });
                }
            };

            if ($.isJQuery(container.$)) { // container is page or box
                if (container.type == "page") { // container is page
                    containerType = 0;
                    parent = container.$;
                    page = container;
                }
                else if (container.type == "box") { // container is a ui.box descendant
                    containerType = 1;
                    parent = container.$;
                    page = container.getPage();
                    parentBox = container;
                }
            }
            else if ($.isJQuery(container)) { // container is jquery object
                containerType = 2;
                parent = container;
                page = null;
            }

            if (parent == null) {
                return null; // error, parent is mandatory
            }

            // ELEMENT ---
            //var identifier = $$.identifier(8);
            //var element = $("<div id=\"" + identifier + "\" class=\"_BO\" />");
            var element = $("<div class=\"_BO\" />");
            var box = function (a) {
                if ($.isPlainObject(a)) {
                    $.each(a, function (i, v) {
                        if ($.isFunction(box[i])) {
                            if ($.isArray(v)) box[i].apply(box, v);
                            else box[i](v);
                        }
                    });
                };
                return box;
            };
            box.$ = element;
            $.data(element.get(0), "ui", box);
            box.type = "box";

            if (containerType == 1) {
                if (parentBox.isScroll()) {
                    var el = $(parent.find("._IB")[0]);
                    el.append(element);
                    parentBox.scrollCalculate(true);
                }
                else {
                    parent.append(element);
                    //setTimeout(function () {

                    //}, 1);
                }
            }
            else {
                parent.append(element);
                //setTimeout(function () {

                //}, 1);
            }

            // PUBLIC
            box.tag = function (n) {
                this.$.attr("tag", n);
            };
            box.delay = function (d) {
                return this.$.delay(d);
            };
            box.animate = function (a, b, c, d) {
                return this.$.animate(a, b, c, d);
            };
            box.stop = function () {
                return box.$.stop();
            };
            box.getPage = function () {
                return page;
            };
            box.clone = function () {
                return box.$.clone(false, false);
            };
            box.class = function () {
                var o = share.args(arguments, "string class")
                var t = this;

                if (o) {
                    var sls = o.class.split(" ");
                    var cls = t.$.attr("class").split(" ");

                    $.each(sls, function (i, v) {
                        var vv = v.trim();
                        if (vv != "") {
                            var ic = vv.charAt(0);
                            if (ic == "-") {
                                if (vv.length > 1) {
                                    var vvv = vv.substring(1);

                                    if (vvv.indexOf("?") > -1) {
                                        var vvvr = new RegExp(vvv.replace("?", "\\w+"));

                                        $.each(cls, function (ci, cv) {
                                            var cvv = cv.trim();
                                            if (cvv != "") {
                                                if (vvvr.exec(cvv) != null) {
                                                    t.$.removeClass(cvv);
                                                }
                                            }
                                        });
                                    }
                                    else t.$.removeClass(vvv);
                                }
                            }
                            else if (ic == "+") t.$.addClass(vv.substring(1));
                            else t.$.addClass(vv);
                        }
                    });
                }
                else t.$.removeClass();
            };
            box.css = function (c, d) {
                if (d == undefined)
                    return this.$.css(c);
                else
                    return this.$.css(c, d);
            };
            box.dom = function () {
                return this.$.get(0);
            };
            function getCSS(el, prop) {
                var o = window.getComputedStyle(el.get(0), null)[prop];
                if (o != null) {
                    if (o.endsWith('%')) {
                        // percentage
                        var par = el.parent();
                        var oval = Math.floor(o.substr(0, o.length - 1));
                        var parv = getCSS(par, prop);
                        var parval = 0;
                        if (parv != null) parval = parv;
                        var oreal = (oval / 100) * parval;
                        return oreal;
                    }
                    else return Math.floor(o.replace(/[^-\d\.]/g, ''));
                }
                else return null;
            };
            box.width = function (w, a) {
                if ($.isUndefined(w))
                    return getCSS(this.$, "width");//this.$.width();
                else if ($.isNull(w))
                    this.width("");
                else {
                    if ($.isUndefined(a)) {
                        this.$.css({ width: w });
                    }
                    else {
                        this.$.animate({ width: w }, a);
                    }
                }
            };
            box.height = function (h, a) {
                if ($.isUndefined(h))
                    return getCSS(this.$, "height");//this.$.height();
                else if ($.isNull(h))
                    this.height("");
                else {
                    if ($.isUndefined(a)) {
                        this.$.css({ height: h });
                        if (isCenter) {
                            //centerTop = this.height()
                            this.$.css({ marginTop: -(this.height() / 2) });
                        }
                    }
                    else {
                        this.$.animate({ height: h }, a);
                    }
                }
            };
            box.computedWidth = function () {
                return this.$.width();
            };
            box.computedHeight = function () {
                return this.$.height();
            };
            box.rawWidth = function () {
                var raw = this.$[0].style.width;
                return raw == "" ? null : raw;
            };
            box.rawHeight = function () {
                var raw = this.$[0].style.height;
                return raw == "" ? null : raw;
            };
            box.rawTop = function () {
                var raw = this.$[0].style.top;
                return raw == "" ? null : raw;
            };
            box.rawBottom = function () {
                var raw = this.$[0].style.bottom;
                return raw == "" ? null : raw;
            };
            box.rawLeft = function () {
                var raw = this.$[0].style.left;
                return raw == "" ? null : raw;
            };
            box.rawRight = function () {
                var raw = this.$[0].style.right;
                return raw == "" ? null : raw;
            };
            box.relative = function (b) {
                var ts = false;
                if ($.isNull(b)) ts = false;
                else if ($.isUndefined(b)) ts = true;
                else if ($.isBoolean(b)) {
                    if (b) ts = true;
                    else ts = false;
                }

                if (ts == true) {
                    element.css({ position: "relative" });
                }
                else {
                    element.css({ position: "" });
                }

            };
            box.isRelative = function () {
                var er = element.css("position");
                if (er == "relative") return true;
                else return false;
            };
            box.noSelect = function (c) {
                if (c == true) box.class("+_US");
                else box.class("-_US");
            };
            box.size = function () {
                var o = arguments;

                if ($.isArray(o[0])) {
                    this.width(o[0][0]);
                    this.height(o[0][1]);
                }
                else if (o.length == 2) {
                    this.width(o[0]);
                    this.height(o[1]);
                }
                else if (o.length == 1) {
                    this.width(o[0]);
                    this.height(o[0]);
                }
                else return [this.width(), this.height()];
            };
            box.z = function (z) {
                if ($.isUndefined(z)) return this.$.css("zIndex");
                else this.$.css({ zIndex: z });
            };
            box.x = function (x, a) {
                if ($.isUndefined(x)) {
                    return parseInt(this.$.css("x"));
                }
                else {
                    if ($.isPlainObject(a)) this.$.animate({ x: x }, a);
                    else this.$.css({ x: x });
                }
            };
            box.y = function (y, a) {
                if ($.isUndefined(x)) {
                    return parseInt(this.$.css("y"));
                }
                else {
                    if ($.isPlainObject(a)) this.$.animate({ y: y }, a);
                    else this.$.css({ y: y });
                }
            };
            box.top = function (t, a) {
                if ($.isUndefined(t)) {
                    if (this.isRelative()) {
                        return this.$.position().top;
                    }
                    else {
                        var cc = this.$.css("top");
                        if (cc == "auto") return 0;
                        else return parseInt(cc);
                    }
                }
                else if ($.isNull(t)) this.$.css({ top: "" });
                else {
                    if (isCenter) {
                        centerStyle.top = t + "px";
                        centerTop = t;
                        delete centerStyle.bottom;
                    }
                    else {
                        if ($.isPlainObject(a))
                            this.$.animate({ top: t }, a);
                        else
                            this.$.css({ top: t });
                    }
                }
            };
            box.left = function (l, a) {
                if ($.isUndefined(l)) {
                    if (this.isRelative()) {
                        return this.$.position().left;
                    }
                    else {
                        var cc = this.$.css("left");
                        if (cc == "auto") return 0;
                        else return parseInt(cc);
                    }
                }
                else if ($.isNull(l)) {
                    this.$.css({ left: "" });
                }
                else {
                    if (isCenter) {
                        centerStyle.left = l + "px";
                        centerLeft = l;
                        delete centerStyle.right;
                    }
                    else {
                        if ($.isPlainObject(a))
                            this.$.animate({ left: l }, a);
                        else
                            this.$.css({ left: l });
                    }
                }
            };

            var attachEvents = null;
            box.attach = function (t, p, s, s2, s3) {
                if ($.isFunction(t) && $.isJQuery(t.$)) {
                    this.removeAttach();

                    var dx = 0, dy = 0, dr = 0, db = 0;
                    if ($.isNumber(s)) {
                        if ($.isUndefined(s2)) {
                            if (p == "right" || p == "right2") dx = s;
                            else if (p == "bottom" || p == "bottom2") dy = s;
                        }
                        else if ($.isNumber(s2)) {
                            if (p == "right" || p == "bottom") { dx = s; dy = s2; }
                            else {
                                if ($.isUndefined(s3)) {
                                    if (p == "right2") { dx = s; dr = s2; }
                                    else if (p == "bottom2") { dy = s; db = s2; }
                                }
                                else if ($.isNumber(s3)) {
                                    if (p == "right2") { dx = s; dr = s2; dy = s3; }
                                    else if (p == "bottom2") { dx = s; dy = s2; db = s3; }
                                }
                            }
                        }
                    }

                    var callback = function () {
                        if (p == "right")
                            box.position(t.leftWidth() + dx, t.top() + dy);
                        else if (p == "bottom")
                            box.position(t.left() + dx, t.topHeight() + dy);
                        else if (p == "right2") {
                            box.leftRight(t.leftWidth() + dx, dr);
                            box.top(t.top() + dy);
                        }
                        else if (p == "bottom2") {
                            box.left(t.left() + dx);
                            bot.topBottom(t.topHeight() + dy, db);
                        }
                    };
                    attachEvents = [t.resize(callback), t.position(callback)];
                    Array.prototype.push.apply(boxEvents, attachEvents);
                    callback();
                }
            };
            box.removeAttach = function () {
                if (attachEvents != null) this.removeEvent(attachEvents);
            };
            box.leftWidth = function () {
                return box.left() + box.width();
            };
            box.topHeight = function () {
                return box.top() + box.height();
            };
            box.isVisibleInside = function (b) {
                var bt = b.scrollTop();
                var bh = b.scrollTop() + b.height();
                var bl = b.scrollLeft();
                var bw = b.scrollLeft() + b.width();

                var ol = box.left();
                var ow = box.leftWidth();
                var ot = box.top();
                var oh = box.topHeight();

                //debug(bt + " " + bh + " " + bl + " " + bw);
                //debug(ot + " " + oh + " " + ol + " " + ow);

                if (bt <= oh && ot <= bh && bl <= ow && ol <= bw)
                    return true;
                else
                    return false;
            };
            box.position = function (l, t) {
                if ($.no(arguments)) {
                    return [this.left(), this.top()];
                }
                else if ($.isFunction(l)) {
                    return this.event("position", l, t);
                }
                else {
                    this.top(t);
                    this.left(l);
                }
            };
            box.absoluteTop = function (t) {
                if ($.isUndefined(t)) {
                    return this.$.offset().top;
                }
            };
            box.absoluteLeft = function (l) {
                if ($.isUndefined(l)) {
                    return this.$.offset().left;
                }
            };
            box.absolutePosition = function () {
                if ($.no(arguments)) {
                    return [this.absoluteLeft(), this.absoluteTop()];
                }
            };
            box.right = function (r) {
                if ($.isUndefined(r)) {
                    var cc = this.$.css("right");
                    if (cc == "auto") return 0;
                    else return parseInt(cc);
                }
                else if ($.isNull(r)) this.$.css({ right: "" });
                else {
                    // right cannot override left
                    if (isCenter) {
                        centerStyle.right = r + "px";
                        centerRight = r;
                        delete centerStyle.left;
                    }
                    else this.$.css({ left: "", right: r });
                }
            };
            box.leftRight = function (l, r) {
                if (!$.isUndefined(l) && !$.isUndefined(r)) {
                    this.right(r);
                    this.left(l);
                }
                else if (!$.isUndefined(l)) {
                    this.right(l);
                    this.left(l);
                }
            };
            box.bottom = function (b) {
                if ($.isUndefined(b)) {
                    var cc = this.$.css("bottom");
                    if (cc == "auto") return 0;
                    else return parseInt(cc);
                }
                else if ($.isNull(b)) this.$.css({ bottom: "" });
                else {
                    // bottom cannot override top
                    if (isCenter) {
                        centerStyle.bottom = b + "px";
                        centerBottom = b;
                        delete centerStyle.top;
                    }
                    else this.$.css({ top: "", bottom: b });
                }
            };
            box.topBottom = function (t, b) {
                if (!$.isUndefined(t) && !$.isUndefined(b)) {
                    this.bottom(b);
                    this.top(t);
                }
                else if (!$.isUndefined(t)) {
                    this.bottom(t);
                    this.top(t);
                }
            };
            box.show = function (b) {
                if (b == false) box.hide();
                else this.$.show();
            };
            box.hide = function (b) {
                if (b == false) box.show();
                else this.$.hide();
            };
            box.isShown = function () {
                return this.$.is(":visible");
            };
            box.isVisible = function () {
                if (!this.isShown()) return false;
                var p = this.parent();
                if (p != null) {
                    var l = box.left();
                    var t = box.top();
                    var w = p.width();
                    var h = p.height();
                    if (p.isScroll()) {
                        l = l - p.scrollLeft();
                        t = t - p.scrollTop();
                    }
                    if (l < w && t < h) return p.isVisible();
                    else return false;
                }
                else {
                    return true;
                }
            };
            box.fadeIn = function (s, c) {
                this.$.fadeIn(s, c);
            };
            box.fadeOut = function (s, c) {
                this.$.fadeOut(s, c);
            };
            box.fadeTo = function (s, o, c) {
                this.$.fadeTo(s, o, c);
            };
            box.opacity = function (o) {
                if ($.no(arguments)) return this.$.css("opacity");
                else this.$.css({ opacity: o });
            };
            box.removeChildren = function () {
                if (box.isScroll()) {
                    var inner = $("._IB", this.$);
                    $.each(inner.children(), function (i, v) {
                        var cui = $(this).data("ui");
                        if (cui != undefined) {
                            cui.remove();
                        }
                    });
                    inner.empty();
                }
                else {
                    $.each(this.$.children(), function (i, v) {
                        var cui = $(this).data("ui");
                        if (cui != undefined) {
                            cui.remove();
                        }
                    });
                    this.$.empty();
                }
            };
            box.remove = function () {

                // remove anaknya dulu
                box.removeChildren();

                if (this.isButton()) this.removeButton();
                if (this.isDragable()) this.removeDragable();
                if (this.isResizable()) this.removeResizable();

                this.removeEvent(boxEvents);
                this.$.remove();
            };
            box.onColor = function (s, a) {
                if ($.isNull(s)) {
                    this.$.css({ backgroundColor: '' });
                }
                else if ($.isUndefined(s)) {
                    return this.$.css("backgroundColor");
                }
                else {
                    if ($.isPlainObject(a)) {
                        this.$.animate({ backgroundColor: s }, a);
                    }
                    else
                        this.$.css({ backgroundColor: s });
                }
            };
            box.color = function () {
                var o = arguments;

                if ($.isNull(o[0])) {
                    if ($.isPlainObject(o[1])) {
                        var complete = o[1].complete;
                        o[1].complete = function () {
                            box.onColor(null);
                            if (complete != null) complete.apply(this);
                        };
                        this.onColor($$.color(this.onColor(), 0), o[1]);
                    }
                    else this.onColor(null);
                }
                else if ($.isUndefined(o[0])) return this.onColor();
                else {

                    var s = null;
                    var a = null;

                    if ($.isNumber(o[0])) {
                        if ($.isNumber(o[1]) && $.isNumber(o[2])) {
                            if ($.isNumber(o[3])) {
                                s = share.color(o[0], o[1], o[2], o[3]);
                                if ($.isPlainObject(o[4])) a = o[4];
                            }
                            else {
                                s = share.color(o[0], o[1], o[2]);
                                if ($.isPlainObject(o[3])) a = o[3];
                            }
                        }
                        else {
                            if ($.isNumber(o[1])) {
                                s = ui.color(o[0], o[1]);
                                if ($.isPlainObject(o[2])) a = o[2];
                            }
                            else {
                                s = ui.color(o[0]);
                                if ($.isPlainObject(o[1])) a = o[1];
                            }
                        }
                    }
                    else if ($.isString(o[0])) {
                        if ($.isNumber(o[1])) {
                            s = ui.color(o[0], o[1]);
                            if ($.isPlainObject(o[2])) a = o[2];
                        }
                        else {
                            s = ui.color(o[0]);
                            if ($.isPlainObject(o[1])) a = o[1];
                        }
                    }

                    if (s != null) {
                        if (a != null) this.onColor(s, a);
                        else this.onColor(s);
                    }
                }
            };
            box.event = function (e, c, d, f) {
                if ($.isArray(e) || $.isNumber(e)) {
                    if (page != null)
                        page.event(e);
                    else
                        selfEvent(e);
                }
                else if ($.isJQuery(e)) {
                    var ei;
                    if (page != null)
                        ei = page.event(e, c, null, d, $.isUndefined(f) ? null : f, box);
                    else
                        ei = selfEvent(e, c, null, d, $.isUndefined(f) ? null : f, box);
                    boxEvents.push(ei);
                    return ei;
                }
                else {
                    var ei;
                    if (page != null)
                        ei = page.event(this.$, e, null, c, $.isUndefined(d) ? null : d, box);
                    else
                        ei = selfEvent(this.$, e, null, c, $.isUndefined(d) ? null : d, box);
                    boxEvents.push(ei);
                    return ei;
                }
            };
            box.triggerEvent = function (e, c) {
                $.each(boxEvents, function (i, v) {
                    var handle = $$.getEventHandle(v);
                    if (handle.type == e) {
                        $$.triggerEvent(v, c);
                    }
                });
            };
            box.removeEvent = function (e) {
                if ($.isNumber(e) || $.isString(e)) {
                    if (page != null)
                        page.removeEvent(e);
                    else
                        selfRemoveEvent(e);
                    var ix = boxEvents.indexOf(e);
                    if (ix > -1) boxEvents.splice(ix, 1);
                }
                else if ($.isArray(e)) {
                    $.each(e, function (i, v) {
                        box.removeEvent(v);
                    });
                }
            };
            box.resize = function (c, d) {
                return this.event("resize", c, d);
            };
            box.triggerResize = function (a, b, c, d) {
                this.triggerEvent("resize", [a, b, c, d]);
            };
            box.down = function (c, d) {
                return this.event("down", c, d);
            };
            box.up = function (c, d) {
                return this.event("up", c, d);
            };
            box.click = function (c, d) {
                return this.event("click", c, d);
            };
            box.move = function (c, d) {
                return this.event("move", c, d);
            };
            box.enter = function (c, d) {
                return this.event("enter", c, d);
            }
            box.leave = function (c, d) {
                return this.event("leave", c, d);
            };
            box.over = function (c, d) {
                return this.event("over", c, d);
            }
            box.out = function (c, d) {
                return this.event("out", c, d);
            };
            box.hover = function (e, l, d) {
                return { enter: this.enter(e, d), leave: this.leave(l, d) };
            };
            box.wheel = function (c, d) {
                return this.event("wheel", c, d);
            };

            var border = null, borderLeft = null, borderTop = null, borderRight = null, borderBottom = null;

            function borderSet(d, c) {
                if (c == null) box.$.css(d, "");
                else box.$.css(d, c.size + "px solid " + ui.color(c.color));

                if (border == null && borderLeft == null && borderTop == null && borderRight == null && borderBottom == null) box.$.css({ boxSizing: "" });
                else box.$.css({ boxSizing: c.outside == true ? "" : "border-box" });
            };

            box.border = function (c) {
                border = c;
                borderSet("border", c);
            };
            box.borderLeft = function (c) {
                borderLeft = c;
                borderSet("border-left", c);
            };
            box.borderRight = function (c) {
                borderRight = c;
                borderSet("border-right", c);
            };
            box.borderTop = function (c) {
                borderTop = c;
                borderSet("border-top", c);
            };
            box.borderBottom = function (c) {
                borderBottom = c;
                borderSet("border-bottom", c);
            };

            var tooltipOver = null, tooltipOut = null;
            var tooltipText = null, tooltipColor = null;

            box.tooltip = function (t) {
                if (tooltipOver == null) {
                    tooltipOver = box.over(function () {
                        var to = ui.tooltipBox();
                        var timer = to.data("timer");

                        if (timer != null) clearTimeout(timer);
                        timer = setTimeout(function () {
                            var contents = to.data("contents");
                            var arrowtop = to.data("arrowtop");
                            var arrowbot = to.data("arrowbot");

                            to.show();
                            to.opacity(0);
                            to.size(400, 250);
                            contents.size(400, 270);

                            if ($.isString(t)) {
                                tooltipText = ui.text(contents)({ font: 11, position: [10, 10], color: 90, text: t });
                                tooltipText.text(t);
                                contents.width(tooltipText.computedWidth() + 20);
                                contents.height(tooltipText.computedHeight() + 20);
                                if (tooltipColor != null) {
                                    tooltipText.spanColor(tooltipColor[0], tooltipColor[1], tooltipColor[2], tooltipColor[3], tooltipColor[4]);
                                }
                            }
                            if ($.isFunction(t)) {

                            }

                            var w = contents.width();
                            var h = contents.height();
                            to.width(w);
                            to.height(h + 8);
                            to.animate({ opacity: 1 }, { duration: 100 });

                            var abstop = box.absoluteTop();
                            var absleft = box.absoluteLeft();
                            var width = to.width();
                            var height = to.height();
                            var boxWidth = box.width();
                            var boxHeight = box.height();

                            if (absleft < 30) {
                                to.left(10);
                            }
                            else if (absleft > ($$.pageWidth() - 30)) {
                                to.left($$.pageWidth() - width - 10);
                            }
                            else if ((absleft - 20 + width) > ($$.pageWidth() - 10)) {
                                to.left($$.pageWidth() - width - 10);
                                arrowbot.left(absleft - ($$.pageWidth() - width - 10));
                                arrowtop.left(absleft - ($$.pageWidth() - width - 10));
                            }
                            else {
                                to.left(absleft - 20);
                                arrowbot.left(20);
                                arrowtop.left(20);
                            }

                            if ((abstop + height + boxHeight + 10) > ($$.pageHeight() - 10)) {
                                arrowtop.hide();

                                if (absleft < 30 || absleft > ($$.pageWidth() - 30)) arrowbot.hide();
                                else {
                                    arrowbot.show();
                                    arrowbot.top(height - 16);
                                }

                                contents.top(0);
                                to.top(abstop - height);
                            }
                            else {
                                arrowbot.hide();

                                if (absleft < 30 || absleft > ($$.pageWidth() - 30)) arrowtop.hide();
                                else {
                                    arrowtop.show();
                                }
                                
                                contents.top(8);
                                to.top(abstop + boxHeight);
                            }

                        }, 500);
                        to.data("timer", timer);
                    });
                    tooltipOut = box.out(function () {
                        var to = ui.tooltipBox();
                        var timer = to.data("timer");
                        if (timer != null) clearTimeout(timer);
                        to.hide();
                        to.data("contents").removeChildren();
                        tooltipText = null;
                    });
                }
            };
            box.tooltipSpanColor = function (a, b, c, d, e) {
                tooltipColor = [a, b, c, d, e];
                if (tooltipText != null) {
                    tooltipText.spanColor(a, b, c, d, e);
                }
            };
            box.removeTooltip = function () {
                box.removeEvent(tooltipOver);
                box.removeEvent(tooltipOut);
                ui.tooltipBox().hide();
                tooltipOver = null;
                tooltipOut = null;
                tooltipCenter = false;
                tooltipColor = [undefined, undefined, undefined, undefined, undefined];
            };

            var buttonDefaults = {
                normal: function () { },
                press: function () { },
                over: function () { },
                click: function () { },
                hold: function () { }
            };
            var button = null, buttonEvents = [];
            box.button = function () {
                var o = arguments;

                if (button == null) {
                    if ($.no(arguments)) {
                        button = $.extend({}, buttonDefaults);
                    }
                    else if ($.isPlainObject(o[0])) {
                        button = $.extend({}, buttonDefaults, o[0]);
                    }

                    if (button != null) {
                        var inside = true;
                        var repeat;
                        var idin = box.enter(function () { inside = true; });
                        var idout = box.leave(function () { inside = false; });
                        var ein = box.enter(function () { button.over.call(box); });
                        var eout = box.leave(function () { button.normal.call(box); });
                        var edown = box.down(function (e) {

                            if (!e.isTouch && e.button != 1) return;

                            e.preventDefault();
                            e.stopPropagation();
                            button.press.call(box, e);
                            share.disableEvent([ein, eout]);

                            repeat = share.repeat({ initialTrigger: true }, function (info) {
                                e.rate = info.rate;
                                button.hold.call(box, e);
                            });

                            var eup = share.up(function (e) {
                                share.enableEvent([ein, eout]);

                                if (inside) button.over.call(box, e);
                                else button.normal.call(box);

                                button.click.call(box);
                                share.removeRepeat(repeat);

                                var eupi = buttonEvents.indexOf(eup);
                                if (eupi > -1) buttonEvents.splice(eupi, 1);
                            }, true);

                            buttonEvents.push(eup);
                        });

                        buttonEvents.push(idin, idout, ein, eout, edown);
                    }
                }
                else {
                    if ($.isPlainObject(o[0])) {
                        button = $.extend(button, o[0]);
                    }
                    else if ($.isNullOrFalse(o[0])) {
                        button.normal.call(box);
                        button = null;
                        box.removeEvent(buttonEvents);
                        buttonEvents = [];
                    }
                }
            };
            box.removeButton = function () {
                this.button(null);
            };
            box.disableButton = function () {
                share.disableEvent(buttonEvents);
            };
            box.enableButton = function () {
                share.enableEvent(buttonEvents);
            };
            box.isButton = function () {
                return button != null;
            };


            var isCenter = false, centerStyle, centerTop, centerLeft, centerRight, centerBottom, centerResize = null;
            box.center = function (c) {
                if ($.isUndefined(c)) c = true;

                if (isCenter == false && c) {
                    isCenter = true;
                    centerStyle = share.css(this.$);

                    centerTop = this.top(); centerBottom = this.bottom(); centerLeft = this.left(); centerRight = this.right();
                    this.top(null); this.bottom(null); this.left(null); this.right(null);

                    // parent
                    if (box.parent() != null &&
                        (box.type == "text" ||
                        (parentBox.rawHeight() == null && parentBox.rawTop() != null && parentBox.rawBottom() != null)
                        )) {
                        var reshack = function () { box.$.css({ top: (parentBox.height() - box.height()) / 2, left: (parentBox.width() - box.width()) / 2 }); };
                        centerResize = [parentBox.resize(reshack), box.resize(reshack)];
                        reshack();

                        if (box.type == "text") box.$.css({ textAlign: "center" });
                    }
                    else
                        this.$.css({ margin: "0 auto", top: "50%", position: "relative", marginTop: -(this.height() / 2) });

                }
                else if (isCenter == true && !c) {

                    isCenter = false;
                    var oo = centerStyle;

                    this.$.css({ margin: "", position: "", left: "", right: "", top: "", bottom: "", textAlign: "" });

                    this.top($.isUndefined(oo.top) ? null : centerTop);
                    this.bottom($.isUndefined(oo.bottom) ? null : centerBottom);
                    this.left($.isUndefined(oo.left) ? null : centerLeft);
                    this.right($.isUndefined(oo.right) ? null : centerRight);

                    if (centerResize != null) share.removeEvent(centerResize);
                    centerResize = null;
                }
            };
            box.isCenter = function () {
                return isCenter;
            };
            box.cursor = function (p) {
                if (p == null) p = "";
                this.$.css({ cursor: p });
            };
            box.defaultCursor = function () {
                this.cursor("");
            };
            box.autoCursor = function () {
                this.cursor("auto");
            };
            box.parent = function () {
                if (containerType == 1)
                    return parentBox;
                else
                    return null;
            };
            box.children = function () {
                var children = [];

                $.each(box.$.children(), function (i, v) {
                    var vo = $(v);
                    var cbox = vo.data("ui");
                    if (cbox == null) return true;

                    children.push(cbox);
                });

                return children;
            };
            box.float = function (f) {
                this.$.css({ float: f });
            };
            var dragableDefaults = {
                lockInside: false,
                lockX: false,
                lockY: false,
                drag: function (x, y, touchXEdge, touchYEdge) { }
            };
            var dragable = null;
            box.dragable = function () {
                var o = arguments;

                if (dragable == null) {
                    if ($.no(arguments) || o[0] === true) {
                        dragable = $.extend({}, dragableDefaults);
                    }
                    else if ($.isPlainObject(o[0])) {
                        dragable = $.extend({}, dragableDefaults, o[0]);
                    }

                    if (dragable != null) {
                        interaction();
                    }
                }
                else {
                    if ($.isPlainObject(o[0])) {
                        dragable = $.extend(dragable, o[0]);
                    }
                    else if ($.isNullOrFalse(o[0])) {
                        dragable = null;
                        interaction();
                    }
                }
            };
            box.removeDragable = function () {
                this.dragable(null);
            };
            box.isDragable = function () {
                return dragable != null;
            };
            var resizableDefaults = {
                handleSize: 8,  // <number>
                minWidth: 0,  // <number>
                minHeight: 0, // <number> 
                maxWidth: -1,  // <number>
                maxHeight: -1, // <number>
                lockInside: false, // true, false
            };
            var resizable = null;
            box.resizable = function () {
                var o = arguments;

                if (resizable == null) {
                    if ($.no(arguments) || o[0] == true) {
                        resizable = $.extend({}, resizableDefaults);
                    }
                    else if ($.isPlainObject(o[0])) {
                        resizable = $.extend({}, resizableDefaults, o[0]);
                    }

                    if (resizable != null) {
                        interaction();
                    }
                }
                else {
                    if ($.isPlainObject(o[0])) {
                        resizable = $.extend(resizable, o[0]);
                    }
                    else if ($.isNullOrFalse(o[0])) {
                        resizable = null;
                        interaction();
                    }
                }
            };
            box.removeResizable = function () {
                this.resizable(null);
            };
            box.isResizable = function () {
                return resizable != null;
            };
            box.isMouseOver = function () {

                var isHovered = !!box.$.
                    filter(function () { return $(this).is(":hover"); }).length;

                return isHovered;
            };

            var scrollDefaults = {
                vertical: true,
                horizontal: true,
                type: "default", // default, button,
                horizontalStep: null,
            };
            var scroll = null, scrollEvents = [], scrollVerticalHandler = null, scrollHorizontalHandler = null, scrollSize = 8;
            var scrollUpButton = null, scrollDownButton = null, scrollLeftButton = null, scrollRightButton = null, scrollBeacon = null;

            function scrollCalculateProc() {
                var outer = $("div._OB", box.$).slice(0, 1);
                var outerRight = parseInt(outer.css("right"));
                var outerLeft = parseInt(outer.css("left"));
                var inner = $("div._IB", outer).slice(0, 1);
                var innerLeft = parseInt(inner.css("marginLeft"));

                var wwidth = outer[0].offsetWidth;
                var wheight = outer[0].offsetHeight;

                if (scroll.horizontalStep != null) {
                    var last = scroll.horizontalStep[scroll.horizontalStep.length - 1];
                    scrollBeacon.left(last + wwidth + outerRight + outerLeft + innerLeft + 29);
                }

                var stop = inner.scrollTop();
                var sleft = inner.scrollLeft();

                var swidth = inner[0].scrollWidth;
                var sheight = inner[0].scrollHeight;

                if (scrollVerticalHandler != null) {
                    if (wheight >= sheight) scrollVerticalHandler.hide();
                    else {
                        if (!scrollVerticalHandler.isShown()) scrollVerticalHandler.show();
                        var oh = $("._VB", box.$)[0].offsetHeight;

                        var vhh = wheight / sheight * oh;
                        if (vhh < scrollSize * 2) vhh = scrollSize * 2;
                        scrollVerticalHandler.height(vhh);

                        var vht = (stop / (sheight - wheight)) * (oh - vhh);
                        scrollVerticalHandler.top(vht);
                    }
                }
                if (scrollHorizontalHandler != null) {
                    if (wwidth >= swidth) scrollHorizontalHandler.hide();
                    else {
                        if (!scrollHorizontalHandler.isShown()) scrollHorizontalHandler.show();
                        var ow = $("._HB", box.$)[0].offsetWidth;

                        var hhw = wwidth / swidth * ow;
                        if (hhw < scrollSize * 2) hhw = scrollSize * 2;
                        scrollHorizontalHandler.width(hhw);

                        var hhl = (sleft / (swidth - wwidth)) * (ow - hhw);
                        scrollHorizontalHandler.left(hhl);
                    }
                }

                var buttonShow = scroll.type == "button";
                if (Math.abs(swidth - wwidth) < 3) wwidth = swidth;

                if (scrollLeftButton != null) {
                    if (sleft > 0) {
                        if (buttonShow) {
                            outer.css({ left: 30 });
                            inner.css({ marginLeft: -30 });
                            scrollLeftButton.show();
                        }
                    }
                    else {
                        outer.css({ left: 0 });
                        inner.css({ marginLeft: 0 });
                        scrollLeftButton.fadeOut(100);
                    }
                }
                if (scrollRightButton != null) {
                    //debug(sleft, swidth, wwidth, outerRight, innerLeft);
                    if (sleft < swidth - wwidth - outerRight + innerLeft) {
                        if (buttonShow) {
                            outer.css({ right: 30 });
                            scrollRightButton.show();
                        }
                    }
                    else {
                        //debug("we called");
                        outer.css({ right: 0 });
                        scrollRightButton.fadeOut(100);
                    }
                }
                if (scrollUpButton != null) {
                    if (stop == 0) scrollUpButton.fadeOut(100);
                    else if (buttonShow) scrollUpButton.show();
                }
                if (scrollDownButton != null) {
                    if (stop == sheight - wheight) scrollDownButton.fadeOut(100);
                    else if (buttonShow) scrollDownButton.show();
                }
            };
            function scrollCalculate(add) {
                if (add) {
                    $$(1, scrollCalculateProc);
                }
                else {
                    scrollCalculateProc();
                }
            };
            function scrollHorizontalStep() {
                var inner = $("._IB", box.$);
                var cur = inner.scrollLeft();
                var step = scroll.horizontalStep;

                if (0 <= cur && cur < (step[0] / 2)) {
                    return 0;
                }

                var lastStep = 0;
                for (var i = 0; i < step.length; i++) {

                    var firstHalf = (step[i] - lastStep) / 2;
                    lastStep = step[i];

                    var secondHalf;
                    if (i == step.length - 1) secondHalf = step[i];
                    else secondHalf = (step[i + 1] + step[i]) / 2;

                    if (firstHalf <= cur && cur < secondHalf) return step[i];
                }

                return step[step.length - 1];
            };

            box.scroll = function () {
                var o = arguments;
                if (scroll == null) {
                    if ($.no(arguments) || o[0] === true) scroll = $.extend({}, scrollDefaults);
                    else if ($.isPlainObject(o[0])) scroll = $.extend({}, scrollDefaults, o[0]);

                    if (scroll != null) {

                        var outer = $("<div class=\"_OB\" />").css({ right: 0/*pRight*/, bottom: 0/*pBottom*/ });
                        var inner = $("<div class=\"_IB\" />").css({ right: -share.scrollBarSize()[0], bottom: -share.scrollBarSize()[1] });

                        element.children().each(function () {
                            var o = $(this).detach();
                            inner.append(o);
                        });
                        element.append(outer.append(inner));

                        if (scroll.type == "button") {
                            if (scroll.horizontal) {
                                var scrollButton = {
                                    //normal: function () { this.color(100); },
                                    //over: function () { this.color(95); },
                                    //press: function () { this.color(92); },
                                    press: function (o) {
                                        if (scroll.horizontalStep != null) {
                                            var cur = scrollHorizontalStep();
                                            var go = null;
                                            var ni = null;
                                            if (this.data("right")) {
                                                if (cur > 0) {
                                                    var ole = box.scrollLeft();
                                                    if (ole < cur) {
                                                        $.each(scroll.horizontalStep, function (si, sv) { if (sv == cur) { ni = si; return false; } });
                                                    }
                                                    else $.each(scroll.horizontalStep, function (si, sv) { if (sv == cur) { ni = si + 1; return false; } });
                                                }
                                                else ni = 0;

                                                if (ni < scroll.horizontalStep.length) go = scroll.horizontalStep[ni];
                                                else if (ni >= scroll.horizontalStep.length) go = scroll.horizontalStep[scroll.horizontalStep.length - 1];
                                            }
                                            else {
                                                if (cur > 0) {
                                                    var ole = box.scrollLeft();
                                                    if (ole > cur) {
                                                        $.each(scroll.horizontalStep, function (si, sv) { if (sv == cur) { ni = si; return false; } });
                                                    }
                                                    else $.each(scroll.horizontalStep, function (si, sv) { if (sv == cur) { ni = si - 1; return false; } });
                                                }
                                                else ni = -1;

                                                if (ni != null && ni >= 0) go = scroll.horizontalStep[ni];
                                                else if (ni == -1) go = 0;
                                            }

                                            if (go != null) {
                                                box.scrollLeft(go, { queue: false, duration: 166 });
                                            }
                                        }
                                    },
                                    hold: function (o) {
                                        if (scroll.horizontalStep == null) {
                                            var c = null;
                                            if (this.data("right")) {
                                                if (box.scrollLeft() < inner[0].scrollWidth - box.width()) {
                                                    c = box.scrollLeft() + 50;
                                                    //debug("HOLD", c, box.scrollLeft(), box.width(), inner[0].scrollWidth);
                                                }
                                            }
                                            else {
                                                var c = box.scrollLeft() - 50;
                                                if (c < 0) c = 0;
                                            }
                                            if (c != null && c != box.scrollLeft()) {
                                                //debug("we called");
                                                if (o.rate == 40) { box.stop(); box.scrollLeft(c, { queue: false, duration: o.rate, easing: "linear" }); }
                                                else box.scrollLeft(c, { queue: false, duration: 100 });
                                            }
                                        }
                                    }
                                }

                                scrollRightButton = ui.box(element)({ hide: true, borderLeft: { size: 1, color: 65 }, height: "100%", width: 23, right: 0, dataTag: "right", cursor: "pointer", button: scrollButton });
                                var ric = ui.icon(scrollRightButton, "arrow")({ size: [20, 20], color: 35, center: true });
                                ric.$.css({ marginRight: -3 });

                                scrollLeftButton = ui.box(element)({ hide: true, borderRight: { size: 1, color: 65 }, left: 0, height: "100%", width: 23, cursor: "pointer", button: scrollButton });
                                var lic = ui.icon(scrollLeftButton, "arrow")({ size: [20, 20], color: 35, center: true, flip: "horizontal" });
                                lic.$.css({ marginLeft: -3 });

                                if (!$.isArray(scroll.horizontalStep) || scroll.horizontalStep.length == 0) scroll.horizontalStep = null;

                                if (scroll.horizontalStep != null) {
                                    scroll.horizontalStep.sort(function (a, b) { return a - b; });
                                }

                                if (scroll.horizontalStep != null) {
                                    scrollBeacon = ui.box(inner)({ size: 1 });
                                }
                            }
                            if (scroll.vertical) {
                                var scrollButton = {
                                    hold: function (o) {
                                        var c;
                                        if (this.data("down")) {
                                            c = box.scrollTop() + 50;
                                            if (c + box.height() > inner[0].scrollHeight) { c = inner[0].scrollHeight - box.height(); }
                                        }
                                        else {
                                            var c = box.scrollTop() - 50;
                                            if (c < 0) c = 0;
                                        }
                                        if (c != box.scrollTop()) {
                                            if (o.rate == 40) { box.stop(); box.scrollTop(c, { queue: false, duration: o.rate, easing: "linear" }); }
                                            else box.scrollTop(c, { queue: false, duration: 100 });
                                        }
                                    }
                                }

                                scrollDownButton = ui.box(element)({ hide: true, borderTop: { size: 1, color: 50 }, dataTag: "down", bottom: 0, width: "100%", height: 30, color: 90, opacity: 0.8, cursor: "pointer", button: scrollButton });
                                ui.icon(scrollDownButton, "arrow")({ size: [25, 25], color: 35, center: true, rotation: 90 });
                                scrollUpButton = ui.box(element)({ hide: true, borderBottom: { size: 1, color: 50 }, top: 0, width: "100%", height: 30, color: 90, opacity: 0.8, cursor: "pointer", button: scrollButton });
                                ui.icon(scrollUpButton, "arrow")({ size: [25, 25], color: 35, center: true, rotation: 270 });
                            }
                        }
                        else {
                            var pRight = 0, pBottom = 0;
                            if (scroll.vertical && !Modernizr.touchevents) pRight = scrollSize;
                            if (scroll.horizontal && !Modernizr.touchevents) pBottom = scrollSize;
                            var vc = pRight > 0 ? $("<div class=\"_VB\" />").css({ bottom: pBottom, width: scrollSize }) : null;
                            var hc = pBottom > 0 ? $("<div class=\"_HB\" />").css({ right: pRight, height: scrollSize }) : null;
                            element.prepend(vc, hc);

                            scrollVerticalHandler = vc != null ? ui.box(vc)({ width: "100%", height: 20, color: 65, opacity: 0.8, z: 5000 }) : null;
                            scrollHorizontalHandler = hc != null ? ui.box(hc)({ height: "100%", width: 20, color: 65, opacity: 0.8, z: 5000 }) : null;
                            if (scrollVerticalHandler != null)
                                scrollVerticalHandler.dragable({
                                    lockInside: true, drag: function (x, y, ex, ey) {
                                        var st;
                                        var sheight = inner[0].scrollHeight;
                                        var vheight = vc[0].offsetHeight;
                                        var hheight = scrollVerticalHandler.height();
                                        if (ey && y > 0) st = sheight - vheight;
                                        else st = (y * (sheight - vheight)) / (vheight - hheight);
                                        inner.scrollTop(st);
                                    }
                                });
                            if (scrollHorizontalHandler != null)
                                scrollHorizontalHandler.dragable({
                                    lockInside: true, drag: function (x, y, ex, ey) {
                                        var sl;
                                        var swidth = inner[0].scrollWidth;
                                        var vwidth = hc[0].offsetWidth;
                                        var hwidth = scrollHorizontalHandler.width();
                                        if (ex && x > 0) sl = swidth - vwidth;
                                        else sl = (x * (swidth - vwidth)) / (vwidth - hwidth);
                                        inner.scrollLeft(sl);
                                    }
                                });
                        }

                        scrollEvents.push(
                            box.resize(function () { scrollCalculate(); }),
                            box.scroll(function () { scrollCalculate(); }));

                    }
                }
                else {
                    if ($.isFunction(o[0])) {
                        return selfEvent($("._IB", box.$).slice(0, 1), "scroll", o[0], o[1]);
                    }
                    else if ($.isNullOrFalse(o[0])) {

                        box.removeEvent(scrollEvents);
                        scrollEvents = [];

                        var outer = $("._OB", box.$);
                        var inner = $("._IB", box.$);
                        var vc = scroll.vertical ? $("._VB", box.$) : null;
                        var hc = scroll.horizontal ? $("._HB", box.$) : null;

                        inner.children().each(function () {
                            var o = $(this).detach();
                            element.append(o);
                        });

                        if (scrollVerticalHandler != null) scrollVerticalHandler.remove();
                        scrollVerticalHandler = null;
                        if (scrollHorizontalHandler != null) scrollHorizontalHandler.remove();
                        scrollHorizontalHandler = null;
                        if (scrollLeftButton != null) scrollLeftButton.remove();
                        scrollLeftButton = null;
                        if (scrollRightButton != null) scrollRightButton.remove();
                        scrollRightButton = null;
                        if (scrollUpButton != null) scrollUpButton.remove();
                        scrollUpButton = null;
                        if (scrollDownButton != null) scrollDownButton.remove();
                        scrollDownButton = null;
                        if (scrollBeacon != null) scrollBeacon.remove();
                        scrollBeacon = null;

                        if (scroll.vertical) ui.remove(vc);
                        if (scroll.horizontal) ui.remove(hc);
                        ui.remove(inner);
                        ui.remove(outer);

                        scroll = null;
                    }
                }
            };
            box.removeScroll = function () {
                this.scroll(null);
            };
            box.scrollCalculate = function (add) {
                scrollCalculate(add);
            };
            box.scrollTop = function () {
                if (scroll != null) {
                    var inner = $("._IB", box.$).slice(0, 1);
                    if ($.no(arguments)) {
                        return inner.scrollTop();
                    }
                    else if ($.isNumber(arguments[0])) {
                        if ($.isPlainObject(arguments[1])) {
                            inner.animate({ scrollTop: arguments[0] }, arguments[1]);
                        }
                        else
                            inner.scrollTop(arguments[0]);
                    }
                }
                else return 0;
            };
            box.scrollLeft = function () {
                if (scroll != null) {
                    var inner = $("._IB", box.$);
                    if ($.no(arguments)) {
                        var n = inner.scrollLeft();
                        return n;
                    }
                    else if ($.isNumber(arguments[0])) {
                        var n = arguments[0];
                        if ($.isPlainObject(arguments[1])) inner.animate({ scrollLeft: n }, arguments[1]);
                        else inner.scrollLeft(n);
                    }
                }
                else return 0;
            };
            box.scrollWidth = function () {
                if (scroll != null) {
                    var inner = $("._IB", box.$);
                    return inner[0].scrollWidth;
                }
            };
            box.scrollHeight = function () {
                if (scroll != null) {
                    var inner = $("._IB", box.$);
                    return inner[0].scrollHeight;
                }
            };
            box.isScroll = function () {
                return scroll != null;
            };

            var data = {};
            box.data = function (i, v) {
                if (v != null) {
                    // ngeset
                    data[i] = v;
                }
                else {
                    // retrive
                    return data[i];
                }
            };
            box.dataTag = function (i) {
                data[i] = true;
            };
            box.removeData = function (i) {
                if (data[i] != null) {
                    delete data[i];
                }
            }
            box.removeAllData = function () {
                $.each(data, function (i, v) {
                    delete data[i];
                });
            }

            // INTERACTION ROUTINE ---
            var interactionStarted = false;
            var interactionMouseAction = null;
            var interactionTouchActions = {};
            var interactionEvents = [];
            function interaction() {
                var intn = 0;
                if (dragable != null) intn++;
                if (resizable != null) intn++;

                if (interactionStarted == false && intn > 0) {
                    interactionStarted = true;

                    // add no user-select
                    box.$.css("user-select", "none");

                    var lastx, lasty;

                    var edown = box.down(function (e) {
                        if (!e.isTouch && e.button != 1) return; // left click only

                        var action = {
                            downX: e.x,
                            downY: e.y,
                            size: box.size(),
                            position: box.position(),
                            absolutePosition: box.absolutePosition(),
                            handle: [0, 0],
                        };

                        if (e.isTouch)
                            interactionTouchActions[e.id] = action;
                        else { // mouse

                            if (resizable != null) {
                                var hs = resizable.handleSize;
                                var px = action.downX - action.absolutePosition[0];
                                var py = action.downY - action.absolutePosition[1];

                                if (px < hs) action.handle[0] = -1;
                                if (px >= action.size[0] - hs) action.handle[0] = 1;
                                if (py < hs) action.handle[1] = -1;
                                if (py >= action.size[1] - hs) action.handle[1] = 1;

                                if (action.handle[0] != 0 || action.handle[1] != 0) {
                                    e.preventDefault();
                                }
                            }
                            if (dragable != null) {
                                e.preventDefault();
                            }

                            interactionMouseAction = action;
                        }

                        var eup = share.up(function (ee) {
                            if (e.isTouch)
                                interactionTouchActions[e.id] = null;
                            else
                                interactionMouseAction = null;

                        }, true);

                        interactionEvents.push(eup);
                    });
                    var emove = share.move(function (e) {

                        var as = [];
                        if (e.isTouch) {

                            $.each(e.touches, function (i, v) {
                                var action = interactionTouchActions[v.id];
                                if (action != null) {
                                    as.push($.extend({ x: v.x, y: v.y, id: v.id }, action));
                                }
                            });
                        }
                        else {
                            if (interactionMouseAction != null) {
                                as.push($.extend({ x: e.x, y: e.y }, interactionMouseAction));
                            }
                        }

                        if (as.length == 0) return;

                        var lockInside = false;

                        if (resizable != null && resizable.lockInside) lockInside = true;
                        if (dragable != null && dragable.lockInside) lockInside = true;

                        var mx = containerType == 0 ? (share.pageWidth() - ui.marginRight() - ui.marginLeft()) :
                            containerType == 1 ? parentBox.isScroll() ? parentBox.scrollWidth() : parentBox.width() :
                            containerType == 2 ? parent.width() : 0;
                        var my = containerType == 0 ? (share.pageHeight() - ui.marginBottom() - ui.marginTop()) :
                            containerType == 1 ? parentBox.isScroll() ? parentBox.scrollHeight() : parentBox.height() :
                            containerType == 2 ? parent.height() : 0;


                        if (resizable != null) {

                            var x = null, y = null, w = null, h = null, ox, oy, ow, oh;

                            if (as.length == 1 && !e.isTouch) { // only for mouse

                                var aso = as[0];
                                var hx = aso.handle[0];
                                var hy = aso.handle[1];

                                if (hx == 1) {
                                    var dw = aso.x - aso.downX;
                                    w = aso.size[0] + dw;
                                }
                                else if (hx == -1) {
                                    var dw = aso.downX - aso.x;
                                    w = aso.size[0] + dw;
                                    x = aso.position[0] - dw;

                                    ox = aso.position[0];
                                    ow = aso.size[0];
                                }
                                if (hy == 1) {
                                    var dh = aso.y - aso.downY;
                                    h = aso.size[1] + dh;
                                }
                                else if (hy == -1) {
                                    var dh = aso.downY - aso.y;
                                    h = aso.size[1] + dh;
                                    y = aso.position[1] - dh;

                                    oy = aso.position[1];
                                    oh = aso.size[1];
                                }
                            }

                            if (w != null) {

                                if (resizable.minWidth >= 0 && w < resizable.minWidth) w = resizable.minWidth;
                                else if (resizable.maxWidth != -1 && w > resizable.maxWidth) w = resizable.maxWidth;

                                if (x != null) {
                                    x = (ow - w) + ox;
                                    if (lockInside && x < 0) {
                                        x = 0;
                                        w = ow + ox;
                                    }
                                    box.left(x);
                                }
                                if (lockInside && (box.left() + w) > mx) {
                                    w = mx - box.left();
                                }
                                box.width(w);
                            }
                            if (h != null) {

                                if (resizable.minHeight >= 0 && h < resizable.minHeight) h = resizable.minHeight;
                                else if (resizable.maxHeight != -1 && h > resizable.maxHeight) h = resizable.maxHeight;

                                if (y != null) {
                                    y = (oh - h) + oy;
                                    if (lockInside && y < 0) {
                                        y = 0;
                                        h = oh + oy;
                                    }
                                    box.top(y);
                                }
                                if (lockInside && (box.top() + h) > my) {
                                    h = my - box.top();

                                }
                                box.height(h);
                            }
                        }

                        if (dragable != null) {
                            var x = null, y = null;
                            if (as.length == 1) {
                                var aso = as[0];

                                var px = aso.downX - aso.absolutePosition[0];
                                var py = aso.downY - aso.absolutePosition[1];

                                var db = 0;
                                if (resizable != null) {
                                    db = resizable.handleSize;
                                }

                                if (px >= db && px < (aso.size[0] - db) &&
                                    py >= db && py < (aso.size[1] - db)) {

                                    var ox = aso.downX - aso.position[0];
                                    var oy = aso.downY - aso.position[1];

                                    x = aso.x - ox;
                                    y = aso.y - oy;
                                }
                            }
                            else { // only work in touch, TODO dragable                                    
                                var aso = as[0];

                                var ox = aso.downX - aso.position[0];
                                var oy = aso.downY - aso.position[1];

                                x = aso.x - ox;
                                y = aso.y - oy;
                            }

                            if (x != null && y != null) {
                                var onXEdge = false;
                                var onYEdge = false;

                                if (lockInside == true) {
                                    x = (x < 0) ? 0 : ((x + box.width()) > mx) ? (mx - box.width()) : x;
                                    y = (y < 0) ? 0 : ((y + box.height()) > my) ? (my - box.height()) : y;

                                    if (x == 0 || x == (mx - box.width()))
                                        onXEdge = true;
                                    if (y == 0 || y == (my - box.height()))
                                        onYEdge = true;
                                }
                                if (dragable.lockY == true) {
                                    y = box.top();
                                }
                                if (dragable.lockX == true) {
                                    x = box.left();
                                }
                                if (lastx != x || lasty != y) {
                                    box.position(x, y);
                                    lastx = x;
                                    lasty = y;
                                    dragable.drag(x, y, onXEdge, onYEdge);
                                }
                            }
                        }
                    });
                    interactionEvents.push(edown, emove);
                }
                else if (interactionStarted && intn == 0) {
                    interactionStarted = false;

                    // remove no user-select
                    box.$.css("user-select", "");
                    box.removeEvent(interactionEvents);
                }
            };


            box.overflow = function (i) {
                if ($.isBoolean(i)) {
                    if (i) box.css("overflow", "visible");
                    else box.css("overflow", null);
                }
            };

            return box;
        };
        ui.box = box;

    })(ui);

    // .textbase
    (function (ui) {

        var textbase = function (container) {

            var d = ui.box(container);
            if (d == null) return null;

            d.class("_FB");


            var tb = d;

            tb.refresh = function () {
            };
            tb.font = function () {
                var o = share.args(arguments, 1, "optional string font", "optional number size");

                if (o) {
                    if (o.font != null) {
                        var font = ui.font(o.font);
                        if (font == null) font = o.font;
                        d.$.css({ "font-family": font });
                    }
                    if (o.size != null) d.$.css({ "font-size": o.size });
                    tb.refresh();
                    if (d.isCenter()) {
                        d.center(false);
                        d.center();
                    }
                }
                else {
                    var ff = ui.parseFontFamily(d.$.css("fontFamily"));
                    return { font: (ff && ff.length > 0) ? ff[0] : null, size: parseInt(d.$.css("fontSize")) };
                }

            };
            tb.italic = function () {
                var o = share.args(arguments, "boolean italic");

                if (o) {
                    if (o.italic) d.$.css({ "font-style": "italic" });
                    else d.$.css({ "font-style": "" });
                    tb.refresh();
                }
                else {
                    return d.$.css("fontStyle") == "italic" ? true : false;
                }
            };
            tb.weight = function () {
                var o = share.args(arguments, "string weight");

                if (o) {
                    if (o.weight != null) d.$.css({ "font-weight": o.weight });
                    tb.refresh();
                }
                else {
                    return d.$.css("fontWeight") + "";
                }
            };
            tb.onColor = function (s, a) {
                if ($.isNull(s)) {
                    this.$.css({ color: '' });
                }
                else if ($.isUndefined(s)) {
                    return this.$.css("color");
                }
                else {
                    if ($.isPlainObject(a)) {
                        this.$.animate({ color: s }, a);
                    }
                    else
                        this.$.css({ color: s });
                }
            };
            tb.backgroundColor = function (s) {
                var o = arguments;
                var s = share.color(o[0], o[1], o[2]);

                if (s == null) s = ui.color(o[0], o[1]);
                if (s != null) {
                    d.$.css({ backgroundColor: s });
                }
            };
            tb.measureText = function (s) { // calculate specified text using current properties
                if (s != null) {
                    var fs = tb.font();
                    return ui.measureText(s, fs.font, fs.size, tb.weight(), tb.italic());
                }
            };

            return tb;
        };

        ui.textbase = textbase;

    })(ui);

    // .text
    (function (ui) {

        var text = function (container) {

            var d = ui.textbase(container);
            if (d == null) return null;

            var text_ = "", typeCase = 0, textSize;
            var clickToSelect = false;
            var clickToSelectEvents = [];
            var span_ = [];
            var color_ = [];
            var shadow = null;

            var text = d;
            text.refresh = function () {
                var rt;

                rt = text_;
                span_ = [];

                if (rt.indexOf("{") > -1 || rt.indexOf("}") > -1) {

                    // parse span
                    var crt = "";
                    var vopi = 0;
                    var spanid = 0;
                    do {
                        var vop = rt.indexOf("{", vopi);
                        var vot = rt.indexOf("}", vopi);

                        if (vot == -1) vot = 100000000000;
                        if (vop == -1) vop = 100000000000;

                        if (vop > -1 && vot > -1 && vop != vot) {
                            if (vot > vop) {
                                crt += rt.substring(vopi, vop);
                                var ne = rt.indexOf("|", vop + 1);
                                if (ne > -1) {
                                    var neid = parseInt(rt.substring(vop + 1, ne));
                                    var seid = "s_" + (spanid++);
                                    if (span_[neid] == null) span_[neid] = [];
                                    span_[neid].push(seid);
                                    crt += "<span id=\"" + seid + "\"";
                                    if (color_[neid] != null) {
                                        crt += " style=\"";
                                        crt += "color:" + color_[neid];
                                        crt += "\"";
                                    }

                                    crt += ">";
                                    vopi = ne + 1;
                                }
                                else {
                                    crt += "<span>";
                                    vopi = vop + 1;
                                }
                            }
                            else {
                                crt += rt.substring(vopi, vot);
                                crt += "</span>";
                                vopi = vot + 1;
                            }
                        }
                        else {
                            crt += rt.substr(vopi);
                            break;
                        }
                    } while (vopi < rt.length);
                    rt = crt;
                }

                // apply text properties here
                if (typeCase == 1) rt = rt.toUpperCase();
                else if (typeCase == 2) rt = rt.toLowerCase();

                // calculate text
                textSize = text.measureText(rt);

                d.$.html(rt);
            };
            text.spanColor = function (a, b, c, d, e) {
                if ($.isNumber(a)) {
                    var ca = null, cb = null;
                    if ($.isNull(b)) ca = "";
                    else if ($.isUndefined(b)) return color_[a];
                    else {
                        if (!$.isPlainObject(c)) {
                            ca = share.color(b, c, d);
                            cb = e;
                        }
                        else {
                            ca = share.color(b);
                            cb = c;
                        }

                        if (ca == null) {
                            if (!$.isPlainObject(c)) {
                                ca = ui.color(b, c);
                                cb = d;
                            }
                            else {
                                ca = ui.color(b);
                                cb = c;
                            }
                        }
                    }
                    color_[a] = ca;
                    if (span[a] != null) {
                        $.each(span_[a], function (i, v) {
                            var sel = $("span#" + v, text.$);
                            if (cb == null) {
                                sel.css({ color: ca });
                            }
                            else {
                                sel.animate({ color: ca }, cb);
                            }
                        });
                    }
                }
                else {
                    $.each(arguments, function (ai, av) {

                        if (av != undefined) {
                            var ca = null;
                            if ($.isNull(av)) ca = "";
                            else {
                                if ($.isArray(av)) ca = share.color(av[0], av[1], av[2]);
                                else ca = share.color(av);

                                if (ca == null) {
                                    if ($.isArray(av)) ca = ui.color(av[0], av[1]);
                                    else ca = ui.color(av);

                                }
                            }
                            color_[ai] = ca;

                            if (span_[ai] != null) {
                                $.each(span_[ai], function (i, v) {
                                    var sel = $("span#" + v, text.$);
                                    sel.css({ color: ca });
                                });
                            }
                        }
                    });
                }
            };
            text.case = function () {
                var o = share.args(arguments, "string type");

                if (o) {
                    if (o.type == "normal") typeCase = 0;
                    else if (o.type == "upper") typeCase = 1;
                    else if (o.type == "lower") typeCase = 2;
                    text.refresh();
                }
                else {
                    if (typeCase == 0) return "normal";
                    else if (typeCase == 1) return "upper";
                    else if (typeCase == 2) return "lower";
                }
            };
            text.lineHeight = function () {
                var o = share.args(arguments, "number height");

                if (o) {
                    text.$.css("lineHeight", o.height + "px");
                }
                else return parseInt(text.$.css("lineHeight"));
            };
            text.breakWord = function (b) {
                if ($.isBoolean(b)) {
                    if (b == true) text.$.css({ wordWrap: "break-word" });
                    else text.$.css({ wordWrap: "" });
                }
                else return text.$.css("wordWrap");
            };
            text.align = function () {
                var o = share.args(arguments, "string type");

                if (o) {
                    text.$.css("textAlign", o.type);
                }
                else return text.$.css("textAlign");
            };
            text.text = function (t, a) {
                if (!$.isUndefined(t)) {
                    var te;
                    if ($.isNull(t)) te = "";
                    else if ($.isString(t)) te = t;
                    else if ($.isNumber(t)) te = t + "";

                    if ($.isPlainObject(a) && text.isShown()) {
                        if (shadow == null) {
                            shadow = text.clone();
                            text.$.parent().append(shadow);
                        }
                        shadow.show();
                        shadow.html(text.$.html());
                        shadow.attr("style", text.$.attr("style"));

                        text_ = te.stripHTML();
                        text.refresh();

                        var complete = a.complete;
                        var onCompleted = function () { shadow.hide(); if (complete != null) complete.apply(this, arguments); };
                        if (a.duration == null) a.duration = 200;
                        if (a.slide != null) {
                            if (a.distance == null) a.distance = 20;
                            var x = 0, y = 0;
                            if (a.slide == "up") { x = 0; y = -a.distance; }
                            else if (a.slide == "down") { x = 0; y = a.distance; }
                            else if (a.slide == "right") { y = 0; x = -a.distance; }
                            else if (a.slide == "left") { y = 0; x = a.distance; }

                            text.$.css({ opacity: 0, y: -y, x: -x }).animate({ y: 0, x: 0, opacity: 1 }, { duration: a.duration, queue: false });
                            shadow.css({ opacity: 1, y: 0, x: 0 }).animate({ y: y, x: x, opacity: 0 }, { duration: a.duration, complete: onCompleted, queue: false });
                        }
                        else {
                            text.$.css({ opacity: 0 }).animate({ opacity: 1 }, { duration: a.duration, queue: false });
                            shadow.css({ opacity: 1 }).animate({ opacity: 0 }, { duration: a.duration, complete: onCompleted, queue: false });
                        }
                    }
                    else {
                        text_ = te.stripHTML();
                        text.refresh();
                    }
                }
                else return text_;
            };
            text.noBreak = function (b) {
                if ($.isBoolean(b)) {
                    if (b) {
                        d.$.css({ whiteSpace: "nowrap" });
                    }
                    else {
                        d.$.css({ whiteSpace: "" });
                    }
                }
                else if ($.isUndefined(b)) {
                    return d.$.css("whiteSpace") == "nowrap";
                }
            };
            text.textSize = function () {
                return textSize;
            };
            text.clickToSelect = function (b) {
                if ($.isBoolean(b)) {
                    if (b != clickToSelect) {
                        clickToSelect = b;
                        if (clickToSelect == true) {

                            clickToSelectEvents.push(text.click(function (e) {
                                if (document.selection) {
                                    var range = document.body.createTextRange();
                                    range.moveToElementText(text.$.get(0));
                                    range.select();
                                } else if (window.getSelection) {
                                    var range = document.createRange();
                                    range.selectNode(text.$.get(0));
                                    window.getSelection().addRange(range);
                                }
                                e.stopPropagation();
                            }), text.down(function (e) {
                                e.stopPropagation();
                            }));
                        }
                        else {
                            text.removeEvent(clickToSelectEvents);
                            clickToSelectEvents = [];
                        }
                    }
                }
                else if ($.isUndefined(b)) {
                    return clickToSelect;
                }
            };
            text.truncate = function (b) {
                if ($.isBoolean(b)) {
                    if (b) {
                        d.$.css({ textOverflow: "ellipsis" });
                    }
                    else {
                        d.$.css({ textOverflow: "" });
                    }
                }
                else if ($.isUndefined(b)) {
                    return d.$.css("textOverflow") == "...";
                }
            };

            text.type = "text";

            return text;
        };

        ui.text = text;

    })(ui);

    // .form
    (function (ui) {

        var form = function () {
            var o = share.args(arguments, "required function page");

            if (o) {
                if (o.$.family == null) return;

                var serverID = null, serverUrl = null;
                var method = "get";
                var inputs = [];
                var addData = {};
                var connection = 0;

                var f = {
                    server: function (s) {
                        if ($.isNumber(s)) {
                            serverID = s;
                            serverUrl = null;
                        }
                        else if ($.isString(s)) {
                            serverUrl = s;
                            serverID = null;
                        }
                        else if ($.isUndefined(s)) {
                            if (serverID != null) return serverID;
                            else if (serverUrl != null) return serverUrl;
                            else return null;
                        }
                    },
                    isUsingProvider: function () {
                        if (serverID != null) return true;
                        else return false;
                    },
                    method: function (m) {
                        if ($.isUndefined(m)) return method;
                        else if (m.isIn(["get", "post"])) method = m;
                    },
                    add: function (c) {
                        if (c != null) {
                            if ($.isFunction(c)) {
                                if (c.type == "textinput") {

                                    var exists = false;
                                    $.each(inputs, function (ii, ic) {
                                        if (ic == c) {
                                            exists = true;
                                            return false;
                                        }
                                    });

                                    if (!exists) {
                                        inputs.push(c);
                                        c.keydown(function (c) {
                                            if (c.key == 13) {
                                                f.submit();
                                            }
                                        });
                                    }
                                }
                            }
                            else if ($.isPlainObject(c)) {
                                addData = $.extend(addData, c);
                            }
                        }
                    },
                    length: function () {
                        return inputs.length;
                    },
                    submit: function () {

                        if (connection == 0) {
                            connection = 1;

                            var data = $.extend({}, addData);

                            $.each(inputs, function (ii, ic) {
                                var key = ic.key();
                                if (key != null) {
                                    data[key] = ic.value();
                                }
                            });

                            var done = function (d) {
                                connection = 0;
                                //debug(d);
                            }

                            if (f.isUsingProvider()) {
                                if (method == "get") share.get(serverID, data, done);
                                else if (method == "post") share.post(serverID, data, done);
                            }
                        }
                    }
                };

                return f;
            }
            else return null;
        };

        ui.form = form;

    })(ui);

    // .inputbase
    (function (ui) {

        var inputbase = function (container) {

            var d = ui.box(container);
            if (d == null) return null;

            var key = null, value = null, design = true;

            var ib = d;
            ib.key = function (n) {
                if ($.isUndefined(n)) return key;
                else if ($.isString(n)) key = n;
            };
            ib.onValue = function (v) {
                if ($.isUndefined(v)) return value;
                else value = v;
            };
            ib.value = function (n) {
                return d.onValue(n);
            };
            ib.change = function (c) {
                return d.event("change", c);
            };
            ib.onFocus = function () { };
            ib.focus = function (f) {
                if ($.isUndefined(f)) ib.onFocus();
            };

            ib.onDesign = function (v) {
            };
            ib.design = function (v) {
                if ($.isUndefined(v)) v = true;
                design = v;
                d.onDesign(v);
            };
            ib.isDesign = function () {
                return design;
            };

            return ib;

        };

        ui.inputbase = inputbase;

    })(ui);

    // .textinput
    (function (ui) {

        var textinput = function (container) {

            var d = ui.inputbase(container);
            if (d == null) return null;

            var inputcon = $("<div class=\"_IC\" />");
            var input = $("<input class=\"_IT _FB\" spellcheck=\"false\" />");
            inputcon.append(input);
            d.$.append(inputcon);

            var gline = $("<div />");
            gline.css({ width: "100%", backgroundColor: ui.color("accent"), height: 1, bottom: 0, position: "absolute" });
            d.$.append(gline);

            var textinput = d;

            var dheight = d.height;

            var csize;
            var pheight = null;
            function redraw(_this) {

                if (csize == null) {
                    var ff = ui.parseFontFamily(input.css("fontFamily"));
                    csize = ui.measureText("&nbsp;", (ff && ff.length > 0) ? ff[0] : null, size);
                }

                var bheight;
                if (pheight != null && pheight > csize.height) {
                    bheight = pheight;
                }
                else bheight = csize.height;

                input.height(bheight);

                if (d.isDesign()) {
                    bheight += 5;
                }

                dheight.apply(_this, [bheight]);
            };

            textinput.onColor = function (s, a) {
                if ($.isNull(s)) {
                    input.css({ color: '' });
                }
                else if ($.isUndefined(s)) {
                    return input.css("color");
                }
                else {
                    if ($.isPlainObject(a)) {
                        input.animate({ color: s }, a);
                    }
                    else
                        input.css({ color: s });
                }
            };
            textinput.font = function (size) {
                if ($.isNumber(size)) {
                    var ff = ui.parseFontFamily(input.css("fontFamily"));
                    csize = ui.measureText("&nbsp;", (ff && ff.length > 0) ? ff[0] : null, size);
                    input.css({ fontSize: size });
                    redraw(this);
                }
                else return { size: parseInt(input.css("fontSize")) };
            };
            textinput.italic = function () {
                var o = share.args(arguments, "boolean italic");
                if (o) {
                    if (o.italic) input.css({ "font-style": "italic" });
                    else input.css({ "font-style": null });
                }
                else return input.css("fontStyle") == "italic" ? true : false;
            };
            textinput.weight = function (w) {
                if ($.isUndefined(w)) return input.css("fontWeight");
                else input.css({ "font-weight": w });
            };
            textinput.height = function (h) {
                if ($.isNumber(h) || $.isNull(h)) {
                    pheight = h;
                    redraw(this);
                }

                return dheight.apply(this, []);
            };
            textinput.paddingLeft = function (l) {
                if ($.isNumber(l) || $.isNull(l)) {
                    if (l == null) l = "";
                    input.css({ "box-sizing": "border-box", "padding-left": l });
                    redraw(this);
                }
                else return input.css("paddingLeft");
            };
            textinput.paddingRight = function (r) {
                if ($.isNumber(r) || $.isNull(r)) {
                    if (r == null) r = "";
                    input.css({ paddingRight: r });
                    redraw(this);
                }
                else return input.css("paddingRight");
            };
            textinput.placeholder = function (t) {
                if ($.isUndefined(t)) return input.attr("placeholder");
                else if (t == null) input.removeAttr("placeholder");
                else if ($.isString(t)) input.attr("placeholder", t);
            };
            textinput.onValue = function (v) {
                if ($.isUndefined(v)) return input.val();
                else input.val(v);
            };
            textinput.keydown = function (c) {
                return this.event("keydown", c);
            };
            textinput.keyup = function (c) {
                return this.event("keyup", c);
            };
            textinput.keypress = function (c) {
                return this.event("keypress", c);
            };
            textinput.clear = function () {
                d.value("");
            };
            textinput.onFocus = function () {
                share(50, function () {
                    input.focus();
                });
            };
            textinput.isFocus = function () {
                return input.is(":focus");
            };
            textinput.selection = function (s, e) {
                if ($.isNumber(s) && $.isNumber(e)) {
                    input.setSelection(s, e);
                }
            };
            textinput.caretPosition = function (s) {
                if ($.isNumber(s)) {
                    input.setCursorPosition(s);
                }
            };
            textinput.focusEnd = function () {
                input.focusEnd();
            };
            textinput.focusin = function (d) {
                return textinput.event(input, "focusin", d);
            };
            textinput.focusout = function (d) {
                return textinput.event(input, "focusout", d);
            };
            textinput.readonly = function (s) {
                if (s == false) input.prop("readonly", false);
                else input.prop("readonly", true);
            };
            textinput.maxlength = function (s) {
                if (s == null) {
                    input.prop("maxLength", "");
                }
                else if ($.isNumber(s)) {
                    input.prop("maxLength", s);
                }
                else return input.prop("maxLength");
            };
            textinput.onDesign = function (v) {
                if (v == true) {
                    gline.show();
                }
                else {
                    //debug("false");
                    gline.hide();
                }
                redraw(this);
            };

            textinput.type = "textinput";
            textinput.font(12);
            //textinput.width(300);

            return textinput;
        };

        ui.textinput = textinput;

    })(ui);

    // .textarea
    (function (ui) {

        var textarea = function (container) {

            var d = ui.inputbase(container);
            if (d == null) return null;

            var inputcon = $("<div class=\"_IC\" />");
            var input = $("<textarea />");
            inputcon.append(input);
            d.$.append(inputcon);

            var textarea = d;
            textarea.readonly = function (s) {
                if (s == false) input.prop("readonly", false);
                else input.prop("readonly", true);
            };

            return d;

        };

        ui.textarea = textarea;

    })(ui);

    // .button
    (function (ui) {

        var button = function (container) {

            var d = ui.box(container);
            if (d == null) return null;

            var t = ui.text(d);

            var state = 1;

            var button = d;
            function onresize() {
                var ts = t.textSize();
                t.position((d.width() - ts.width) / 2, (d.height() - ts.height) / 2);
            };
            button.text = function (s, a) {
                var tr = t.text(s, a);
                onresize();
                return tr;
            };
            function setState(s) {
                if (state != s) {
                    state = s;
                    if (state == 1) {
                        d({ color: "accent", cursor: "pointer" });
                    }
                    else {
                        d({ color: 80, cursor: "default" });
                    }
                }
            };
            button.disable = function (s) {
                var _state;
                if (s == false) _state = 1;
                else _state = 0;
                setState(_state);
            };
            button.enable = function (s) {
                var _state;
                if (s == false) _state = 0;
                else _state = 1;
                setState(_state);
            };
            button.isDisabled = function () {
                return state == 1 ? true : false;
            };
            var click = null;
            button.click = function (c) {
                click = c;
            };

            d.resize(onresize);

            t({ color: 100, weight: "600", font: 16 });
            d({
                size: [300, 35], color: "accent", cursor: "pointer", button: {
                    normal: function () {
                        if (state == 1) d.color("accent");
                    },
                    over: function () {
                        if (state == 1) d.color("accent+25");
                    },
                    press: function () {
                        if (state == 1) d.color("accent+40");
                    },
                    click: function () {
                        if (state == 1 && click != null) {
                            click.call(d);
                        }
                    }
                }
            });

            return button;
        };

        ui.button = button;


    })(ui);

    // .accordion
    (function (ui) {

        var accordion = function (container) {

            var d = ui.box(container);
            if (d == null) return null;

            var windows = [];
            var orientation = 0, oldOrientation = 0;
            var focus = -1, oldFocus = -1;
            var insize = 60, oldInsize = 60;
            var clickToFocus = null;
            var windowObjectDefaults = {
                create: function (c) { },
                focus: function (a) { },
                blur: function (a) { },
                resize: function (c, s) { }
            };

            var accordion = d;
            var firsttime = true;

            function sizing(anim) {
                if ($.isUndefined(anim)) {
                    anim = { duration: 0 };
                }

                var an = null, cinsize = false, cfocus = false;

                if (anim != null && firsttime == false) {
                    an = anim;
                    cinsize = (an != null) && (insize != oldInsize);
                    cfocus = (an != null) && (focus != oldFocus);
                }

                var as;
                if (orientation == 0)
                    as = accordion.width();
                else
                    as = accordion.height();

                if (firsttime) {
                    firsttime = false;
                }

                var ms = as - (insize * windows.length - 1);


                var vb = windows[oldFocus];
                var ow = vb.container.width();

                var bfh = 0, afh = 0;
                var ofoch = false;

                $.each(windows, function (iw, vw) {
                    if (iw == focus) {
                        ofoch = true;
                    }
                    //debug(iw + ": shown: " + vw.isShown + ", cd: " + vw.changeDisplay);
                    if ((vw.isShown == false && !vw.changeDisplay) || (vw.isShown && vw.changeDisplay)) {
                        if (ofoch == false) bfh++;
                        else afh++;
                    }
                });

                var bch = 0;
                var ococh = false;

                $.each(windows, function (iw, vw) {
                    var c = vw.box;

                    var cd = vw.changeDisplay;
                    var f = vw.functions;
                    var con = vw.container;

                    if (clickToFocus != null) {
                        if (focus == iw) vw.button.hide();
                        else vw.button.show();
                    }

                    if (iw == focus) {
                        ococh = true;
                    }
                    if (ococh == false && ((vw.isShown == false && !vw.changeDisplay) || (vw.isShown && vw.changeDisplay))) {
                        bch++;
                    }
                    var ach = 0;
                    if (iw > focus) {
                        for (var iiw = iw + 1; iiw < windows.length; iiw++) {
                            var ivw = windows[iiw];
                            if ((ivw.isShown == false && !ivw.changeDisplay) || (ivw.isShown && ivw.changeDisplay)) {
                                ach++;
                            }
                        }
                    }


                    if (cinsize) {

                        if (orientation == 0) {
                            c.top(null); c.bottom(null);
                            c.height("100%");

                            if (iw < focus) {
                                c.$.css({ width: oldInsize, left: (iw - bch) * oldInsize }).animate({ width: insize, left: (iw - bch) * insize }, an);
                            }
                            else if (iw == focus) {
                                //debug("cinsize!");
                                var anx = $.extend({}, an, {
                                    step: function (now, fx) {
                                        //debug("step!");
                                        f.resize(con);
                                    }, complete:
                                        function () {
                                            con.width("100%");
                                        }
                                });

                                c.width(null);
                                c.$.css({ left: (iw - bfh) * oldInsize, right: (windows.length - iw - 1 - afh) * oldInsize })
                                    .animate({ left: (iw - bfh) * insize, right: (windows.length - iw - 1 - afh) * insize }, anx);
                            }
                            else if (iw > focus) {
                                c.$.css({ width: oldInsize, right: (windows.length - iw - 1 - ach) * oldInsize })
                                    .animate({ width: insize, right: (windows.length - iw - 1 - ach) * insize }, an);
                            }
                        }
                    }
                    else if (cfocus) {

                        if (orientation == 0) {

                            c.top(null); c.bottom(null);
                            c.height("100%");

                            if (iw < oldFocus) {
                                if (iw < focus) {
                                    // no change
                                }
                                else if (iw == focus) {
                                    con.width(ow);
                                    var anx = $.extend({}, an, {
                                        step: function (now, fx) {
                                            //debug("sii");
                                            f.resize(con, now == ((windows.length - iw - 1 - afh) * insize));
                                        }, complete: function () {
                                            con.width("100%");
                                        }
                                    });
                                    c.width(null);
                                    c.$.css({ left: (iw - bfh) * insize, right: as - ((iw + 1 - afh) * insize) })
                                        .animate({ right: (windows.length - iw - 1 - afh) * insize }, anx);
                                }
                                else if (iw > focus) {
                                    c.width(insize);
                                    c.$.css({ left: "", right: as - ((iw + 1 - ach) * insize) })
                                        .animate({ right: (windows.length - iw - 1 - ach) * insize }, an);
                                }

                            }
                            else if (iw == oldFocus) {
                                c.width(as - (windows.length - 1) * insize);
                                if (focus < oldFocus) {
                                    c.$.css({ left: "", right: (windows.length - iw - 1 - ach) * insize })
                                        .animate({ width: insize }, an);
                                }
                                else if (focus > oldFocus) {
                                    c.$.css({ left: (iw - bch) * insize, right: "" })
                                        .animate({ width: insize }, an);
                                }
                            }
                            else if (iw > oldFocus) {
                                if (iw < focus) {
                                    c.width(insize);
                                    c.$.css({ left: as - ((windows.length - iw - bch) * insize), right: "" })
                                        .animate({ left: (iw - bch) * insize }, an);
                                }
                                else if (iw == focus) {
                                    con.width(ow);
                                    var anx = $.extend({}, an, { step: function (now, fx) { f.resize(con); }, complete: function () { con.width("100%"); } });
                                    c.width(null);
                                    c.$.css({ left: as - ((windows.length - iw - bfh) * insize), right: (windows.length - iw - 1 - afh) * insize })
                                        .animate({ left: (iw - bfh) * insize }, anx);
                                }
                                else if (iw > focus) {
                                    // no change
                                }

                            }
                        }
                    }
                    else if (cd) {
                        vw.changeDisplay = false;
                        if (vw.isShown) {
                            c.hide();
                            vw.isShown = false;
                        }
                        else {
                            c.show();
                            vw.isShown = true;
                        }
                    }
                    else {
                        if (iw < focus) {
                            if (orientation == 0) {
                                c.top(null); c.bottom(null);
                                c.height("100%");
                                c.left((iw - bch) * insize);
                                c.width(insize);
                            }
                        }
                        else if (iw == focus) {
                            if (orientation == 0) {
                                c.top(null); c.bottom(null);
                                c.height("100%");
                                c.width(null);
                                c.leftRight((iw - bfh) * insize, (windows.length - iw - 1 - afh) * insize);
                                f.resize(con);
                            }
                        }
                        else if (iw > focus) {

                            if (orientation == 0) {
                                c.top(null); c.bottom(null);
                                c.height("100%");
                                c.width(insize);
                                c.right((windows.length - iw - 1 - ach) * insize);
                            }
                        }
                    }



                });
            };

            accordion.isAnimated = function () {

                var isan = false;
                $.each(windows, function (iw, vw) {
                    var c = vw.box;
                    if (c.$.is(":animated")) {
                        isan = true;
                        return false;
                    }
                });

                return isan;
            };
            accordion.clickToFocus = function (b) {
                if ($.isBoolean(b)) {
                    if (b) {
                        clickToFocus = {};
                        $.each(windows, function (iw, vw) {
                            if (iw == focus) vw.button.hide();
                            else vw.button.show();
                        });
                    }
                    else {
                        clickToFocus = null;
                        $.each(windows, function (iw, vw) {
                            vw.button.hide();
                        });
                    }
                }
                else if ($.isPlainObject(b)) {
                    clickToFocus = b;
                    $.each(windows, function (iw, vw) {
                        if (iw == focus) vw.button.hide();
                        else vw.button.show();
                    });
                }
            };
            accordion.inactiveSize = function (n, a) {
                if ($.no(arguments)) return insize;
                else if ($.isNumber(n)) {
                    //debug("sini?");
                    if (insize != n) {
                        insize = n;
                        sizing(a);
                        oldInsize = n;
                    }
                }
            };
            accordion.focus = function (n, a) {
                if ($.no(arguments)) return focus;
                else if ($.isNumber(n)) {
                    if (n >= 0 && n < windows.length) {
                        if (n != focus) {
                            var win = windows[n];
                            if (win.isShown) {
                                var old = windows[oldFocus];
                                focus = n;
                                sizing(a);
                                oldFocus = n;
                                win.functions.focus(win.container, a);
                                old.functions.blur(old.container, a);
                            }
                        }
                    }
                }
            };
            accordion.orientation = function (orientation, a) {
                if (orientation == "horizontal") {
                    orientation = 0;
                    sizing(a);
                    oldOrientation = 0;
                }
                else if (orientation == "vertical") {
                    orientation = 1;
                    sizing(a);
                    oldOrientation = 1;
                }
                else if (orientation == 0) {
                    orientation = 0;
                    sizing(a);
                    oldOrientation = 0;
                }
                else if (orientation == 1) {
                    orientation = 1;
                    sizing(a);
                    oldOrientation = 1;
                }
                else if ($.no(arguments)) {
                    return orientation == 0 ? "horizontal" : "vertical";
                }
            };
            accordion.window = function (a, b) {
                if ($.isNumber(a)) { // replace or get
                    var win = windows[index];
                    if (win != null) {
                        if (!$.isNullOrUndefined(b)) { // replace

                        }
                        else { // get container
                            return win.container;
                        }
                    }
                }
                else if ($.isPlainObject(a)) { // create
                    var ni = windows.length

                    if (ni == 0) {
                        focus = 0;
                        oldFocus = 0;
                    }

                    var obj = $.extend({}, windowObjectDefaults, a);

                    var box = ui.box(accordion);

                    var outerContainer = ui.box(box)({
                        size: ["100%", "100%"],
                        z: 0,
                    });
                    var container = ui.box(outerContainer)({
                        size: ["100%", "100%"],
                    });
                    var button = ui.box(box)({
                        size: ["100%", "100%"],
                        z: 1,
                        hide: true,
                        cursor: "pointer",
                        button: {
                            click: function () {
                                if ($.isPlainObject(clickToFocus))
                                    accordion.focus(ni, clickToFocus);
                                else
                                    accordion.focus(ni);
                            }
                        }
                    });

                    windows[ni] = {
                        box: box,
                        container: container,
                        button: button,
                        functions: obj,
                        changeDisplay: false,
                        isShown: true,
                    };

                    obj.create(container);

                    if (ni == focus) {
                        obj.focus(container);
                    }
                    else {
                        obj.blur(container);
                    }

                    sizing();

                    return ni;
                }
            };
            accordion.disable = function (a) {

            };
            accordion.hideWindow = function (n, a) {
                if ($.isNumber(n)) {
                    if (n >= 0 && n < windows.length) {
                        if (n != focus) {
                            var win = windows[n];
                            if (win.isShown) {
                                win.changeDisplay = true;
                                //debug("hiding!");
                                sizing(a);
                            }
                        }
                    }
                }
            };
            accordion.showWindow = function (n, a) {
                if ($.isNumber(n)) {
                    if (n >= 0 && n < windows.length) {
                        if (n != focus) {
                            var win = windows[n];
                            if (!win.isShown) {
                                win.changeDisplay = true;
                                sizing(a);
                            }
                        }
                    }
                }
            };

            return accordion;
        };

        ui.accordion = accordion;

    })(ui);

    // .graphics .raphael .icon
    (function (ui) {

        function polarToCartesian(centerX, centerY, radius, angleInDegrees) {
            var angleInRadians = (angleInDegrees - 90) * Math.PI / 180.0;

            return {
                x: centerX + (radius * Math.cos(angleInRadians)),
                y: centerY + (radius * Math.sin(angleInRadians))
            };
        };
        function describeArc(x, y, radius, startAngle, endAngle) {

            var start = polarToCartesian(x, y, radius, endAngle);
            var end = polarToCartesian(x, y, radius, startAngle);

            var arcSweep = endAngle - startAngle <= 180 ? "0" : "1";

            var d = [
                "M", start.x, start.y,
                "A", radius, radius, 0, arcSweep, 0, end.x, end.y
            ].join(" ") + " ";

            return d;
        };

        var graphics = function (container, type) {
            if (container == null) return;

            if (type == "raphael" && Raphael != null) {

                var d = ui.box(container);
                if (d == null) return null;

                d.size(400, 300);
                var paper = Raphael(d.$.get(0), 400, 300);

                var raphael = d;
                raphael.paper = function () {
                    return paper;
                };
                var dwidth = d.width;
                var dheight = d.height;
                raphael.width = function () {
                    var o = arguments;
                    if ($.isNumber(o[0])) {
                        paper.setSize(o[0], d.height());
                        dwidth.apply(this, arguments);
                    }
                    else return dwidth.apply(this, arguments);
                };
                raphael.height = function () {
                    var o = arguments;
                    if ($.isNumber(o[0])) {
                        paper.setSize(d.width(), o[0]);
                        dheight.apply(this, arguments);
                    }
                    else return dheight.apply(this, arguments);
                };

                raphael.type = "raphael";

                return raphael;
            }
            return null;
        };
        var icon = function (container, type) {
            if (container == null) return;
            var obj = null;

            switch (type) {
                case "powercut": obj = {
                    path: "M71.4,25.5L38.25,58.65l127.5,127.5v94.35h76.5V510l91.8-155.55L438.6,459l33.15-33.15L71.4,25.5z M420.75,204h-102	l102-204h-255v56.1L382.5,272.85L420.75,204z"
                }; break;
                case "warning": obj = {
                    path: "M15.789,13.982l-6.938-13C8.678,0.657,8.339,0.453,7.97,0.453H7.969c-0.369,0-0.707,0.203-0.881,0.528l-6.969,13c-0.166,0.312-0.157,0.686,0.023,0.986C0.323,15.268,0.649,15.453,1,15.453h13.906c0.352,0,0.677-0.184,0.858-0.486C15.945,14.666,15.954,14.292,15.789,13.982z M7.969,13.453c-0.552,0-1-0.448-1-1s0.448-1,1-1c0.551,0,1,0.448,1,1S8.521,13.453,7.969,13.453z M8.97,9.469c0,0.553-0.449,1-1,1c-0.552,0-1-0.447-1-1v-4c0-0.552,0.448-1,1-1 c0.551,0,1,0.448,1,1V9.469z",
                }; break;
                case "search": obj = {
                    path: "M29.772,26.433l-7.126-7.126c0.96-1.583,1.523-3.435,1.524-5.421C24.169,8.093,19.478,3.401,13.688,3.399C7.897,3.401,3.204,8.093,3.204,13.885c0,5.789,4.693,10.481,10.484,10.481c1.987,0,3.839-0.563,5.422-1.523l7.128,7.127L29.772,26.433zM7.203,13.885c0.006-3.582,2.903-6.478,6.484-6.486c3.579,0.008,6.478,2.904,6.484,6.486c-0.007,3.58-2.905,6.476-6.484,6.484C10.106,20.361,7.209,17.465,7.203,13.885z",
                }; break;
                case "tiles": obj = {
                    path: "M41.554,8.551v14.621H26.935V8.551H41.554z M8.446,23.172h14.62V8.551H8.446V23.172z M26.935,41.449h14.619V26.83H26.935	V41.449z M8.446,41.449h14.62V26.83H8.446V41.449z"
                }; break;
                case "cross": obj = {
                    path: "M 19,6.4 17.6,5 12,10.6 6.4,5 5,6.4 10.6,12 5,17.6 6.4,19 12,13.4 17.6,19 19,17.6 13.4,12 z",
                }; break;
                case "arrow": obj = {
                    path: "M 10,6 8.6,7.4 13.2,12 8.6,16.6 10,18 16,12 z",
                }; break;
                case "arrow2": obj = {
                    path: "M16.714,33.301h66.799l-33.4,33.398L16.714,33.301z",
                }; break;
                case "statusdown": obj = {
                    path: "M11.844,5.174L11.838,5.17C11.748,5.065,11.622,5,11.482,5H4.499c-0.143,0-0.27,0.069-0.361,0.176L4.136,5.174 c-0.195,0.217-0.195,0.569,0,0.786l3.499,3.877c0.195,0.218,0.511,0.218,0.706,0l0.012-0.021l3.491-3.856C12.039,5.743,12.039,5.391,11.844,5.174z"
                }; break;
                case "refresh": obj = {
                    path: "M92.07,256.41H50l78.344,78.019l77.536-78.019h-39.825c-0.104-61.079,49.192-111.032,110.329-111.387c61.293-0.358,111.27,49.039,111.626,110.331c0.58,98.964-119.057,148.511-188.892,79.686l-51.929,52.687c116.154,114.483,315.773,32.415,314.809-132.804C461.4,152.769,378.105,70.441,275.952,71.037C173.955,71.632,91.725,154.47,92.07,256.41z"
                }; break;
                case "back": obj = {
                    path: "M13.5,0C6.597,0,1,5.597,1,12.5S6.597,25,13.5,25S26,19.403,26,12.5S20.403,0,13.5,0ZM13.5,23.173C7.615,23.173,2.827,18.384999999999998,2.827,12.499999999999998C2.827,6.614999999999998,7.615,1.8269999999999982,13.5,1.8269999999999982C19.384999999999998,1.8269999999999982,24.173000000000002,6.614999999999998,24.173000000000002,12.499999999999998C24.173,18.385,19.385,23.173,13.5,23.173ZM14.742,18L9.242,12.5L6.383,12.5L11.883,18ZM6.383,12.5L11.883,7L14.742,7L9.242,12.5ZM10.4555,13.7L20,13.7L20,11.3L10.455,11.3L9.242,12.5Z"
                }; break;
                case "clipboard": obj = {
                    path: "M432,64h-32v32h16v256H288v128H96V96h16V64H80c-8.801,0-16,7.2-16,16v416c0,8.8,7.199,16,16,16h252l116-116V80 C448,71.2,440.8,64,432,64z M320,480v-96h96L320,480z M384,64h-64V32c0-17.6-14.4-32-32-32h-64c-17.602,0-32,14.4-32,32v32h-64v64	h256V64z M288,64h-64V32.057c0.017-0.019,0.036-0.039,0.057-0.057h63.884c0.021,0.018,0.041,0.038,0.059,0.057V64z"
                }; break;
                case "list": obj = {
                    path: "M41.166,9.166v5H18.105v-5H41.166L41.166,9.166z M18.105,23.055h23.061v-5H18.105V23.055z M18.105,31.943h23.061v-5H18.105 V31.943z M18.105,40.834h23.061v-5H18.105V40.834z M8.834,14.166h4.65v-5h-4.65V14.166z M8.834,23.055h4.65v-5h-4.65V23.055z M8.834,31.943h4.65v-5h-4.65V31.943z M8.834,40.834h4.65v-5h-4.65V40.834z"
                }; break;
                case "forward": obj = {
                    path: "M31.416,12.568L14.429,0.38c-1.126-0.604-2.416-0.747-2.417,1.579v5.305 L2.417,0.38C1.291-0.225-0.031-0.243,0.001,2.021v23.942c0.032,2.045,1.384,2.341,2.417,1.673l9.595-6.885v5.243 c0.001,1.983,1.385,2.31,2.417,1.642l16.987-12.188C32.127,14.935,32.162,13.08,31.416,12.568z M14.014,25.208 c0-0.469,0-8.4,0-8.4l-12.012,8.4v-22.4l12.012,8.4c0,0,0-6.612,0-8.4l16.016,11.2L14.014,25.208z"
                }; break;
                case "backtime": obj = {
                    path: "M53.131,0C30.902,0,12.832,17.806,12.287,39.976H0l18.393,20.5l18.391-20.5H22.506C23.045,23.468,36.545,10.25,53.131,10.25 c16.93,0,30.652,13.767,30.652,30.75S70.061,71.75,53.131,71.75c-6.789,0-13.059-2.218-18.137-5.966l-7.029,7.521 C34.904,78.751,43.639,82,53.131,82C75.703,82,94,63.645,94,41S75.703,0,53.131,0z M49.498,19v23.45l15.027,15.024l4.949-4.949 L56.5,39.55V19H49.498z"
                }; break;
                case "timeline": obj = {
                    path: "M25.921,18.395c-0.965-1.358-2.853-1.681-4.215-0.713c-1.361,0.966-1.681,2.854-0.714,4.216l5.761,8.111 c-0.063,0.299-0.099,0.61-0.099,0.929c0,2.47,2,4.472,4.469,4.472c1.3,0,2.46-0.563,3.277-1.448h6.481 c1.67,0,3.023-1.354,3.023-3.023c0-1.669-1.353-3.023-3.023-3.023h-6.482c-0.694-0.752-1.638-1.254-2.703-1.392L25.921,18.395z M52.429,21.306c0.795,1.724,1.383,3.561,1.736,5.482c0.113,0.618,0.678,1.043,1.304,0.986l5.632-0.518 c0.336-0.03,0.643-0.199,0.849-0.466c0.207-0.268,0.291-0.608,0.236-0.941c-0.542-3.265-1.589-6.356-3.061-9.196 c-0.166-0.323-0.468-0.553-0.823-0.63c-0.355-0.075-0.725,0.01-1.01,0.236l-4.517,3.588C52.34,20.196,52.195,20.798,52.429,21.306z M48.224,48.299c-0.46-0.315-1.074-0.276-1.491,0.094c-3.634,3.226-8.28,5.334-13.399,5.777v-2.716	c0-1.114-0.902-2.015-2.015-2.015c-1.113,0-2.016,0.902-2.016,2.015v2.714C18.087,53.2,9.132,44.246,8.164,33.029h2.715 c1.113,0,2.015-0.901,2.015-2.015c0-1.114-0.902-2.015-2.015-2.015H8.156C9.066,18.462,17.031,9.93,27.292,8.131 c0.58-0.102,1.004-0.604,1.004-1.193V1.213c0-0.35-0.152-0.685-0.418-0.915c-0.265-0.23-0.617-0.335-0.965-0.285 C11.702,2.153,0,15.215,0,31.014c0,17.297,14.022,31.319,31.319,31.319c8.497,0,16.183-3.403,21.823-8.899 c0.26-0.254,0.393-0.61,0.362-0.972c-0.032-0.362-0.222-0.693-0.522-0.898L48.224,48.299z M35.346,8.129c0.792,0.138,1.57,0.315,2.331,0.531c0.583,0.165,1.194-0.124,1.439-0.677l2.298-5.182 c0.14-0.315,0.138-0.675-0.004-0.99C41.269,1.498,41,1.256,40.671,1.151c-1.597-0.506-3.247-0.894-4.943-1.138 c-0.348-0.049-0.702,0.055-0.967,0.285c-0.266,0.23-0.418,0.565-0.418,0.917v5.718C34.342,7.525,34.765,8.028,35.346,8.129z M47.822,14.677c0.435,0.443,1.132,0.484,1.618,0.099l4.438-3.527c0.267-0.211,0.432-0.526,0.456-0.866 c0.024-0.341-0.097-0.673-0.332-0.921c-1.615-1.697-3.418-3.21-5.377-4.511c-0.297-0.197-0.667-0.254-1.011-0.154 c-0.343,0.1-0.625,0.346-0.769,0.673l-2.32,5.23c-0.231,0.524-0.068,1.138,0.395,1.472C45.956,12.926,46.925,13.766,47.822,14.677z M62.054,33.694c-0.256-0.262-0.615-0.396-0.979-0.362l-5.755,0.529c-0.549,0.049-0.993,0.463-1.085,1.006 c-0.431,2.569-1.285,4.99-2.487,7.198c-0.297,0.545-0.135,1.228,0.378,1.579l4.671,3.206c0.28,0.193,0.628,0.26,0.959,0.183 c0.331-0.077,0.614-0.289,0.782-0.584c2.017-3.549,3.357-7.527,3.852-11.766C62.433,34.318,62.309,33.956,62.054,33.694z"
                }; break;
                case "penbox": obj = {
                    path: "M497.944,14.059c18.75,18.75,18.719,49.141,0,67.891l-22.625,22.625l-67.906-67.891l22.625-22.625C448.787-4.675,479.194-4.691,497.944,14.059z M158.537,285.591l-22.609,90.5l90.5-22.625L452.694,127.2l-67.906-67.891 L158.537,285.591z M384.006,241.153v206.844h-320v-320h206.859l63.984-64H0.006v448h448V177.137L384.006,241.153z"
                }; break;
                case "leftarrow": obj = {
                    path: "M20,11H7.8l5.6-5.6L12,4l-8,8l8,8l1.4-1.4L7.8,13H20V11z"
                }; break;
                default:
                    obj = {
                        path: type
                    }; break;
            };

            if (obj != null) {

                var d = ui.raphael(container);
                var paper = d.paper();
                var path = paper.path(obj.path).attr({ stroke: "none" });

                var rotation = 0;
                var fliph = false, flipv = false;
                var scalew = 1, scaleh = 1;

                var bbox = path.getBBox(true);

                var htw = Math.sqrt(Math.pow((bbox.width / 2), 2) + Math.pow((bbox.height / 2), 2));
                var tw = htw * 2;

                var calculate = function () {

                    path.transform("s" + (fliph ? "-" : "") + scalew + "," + (flipv ? "-" : "") + scaleh);

                    var sbox = path.getBBox();
                    var tx = ((d.width() - sbox.width) / 2) - sbox.x;
                    var ty = ((d.height() - sbox.height) / 2) - sbox.y;

                    path.transform("r" + rotation + "," + (d.width() / 2) + "," + (d.height() / 2) + "t" + tx + "," + ty + "...");
                };

                d.size(tw, tw);
                calculate();

                var icon = d;

                icon.path = function () {
                    return path;
                };
                icon.onColor = function (s, a) {
                    if ($.isUndefined(s)) return path.attr("fill");
                    else {
                        if (!$.isPlainObject(a))
                            path.attr({ fill: s });
                        else {
                            var obj = { fill: s };
                            if (!$.isUndefined(a.duration)) path.animate(obj, a.duration);
                            else path.animate(obj);
                        }
                    }
                };
                icon.colorOpacity = function (c, a) {
                    if ($.isUndefined(c)) return path.attr("fill-opacity");
                    else {
                        if (!$.isPlainObject(a))
                            path.attr({ "fill-opacity": c });
                        else {
                            var obj = { "fill-opacity": c };
                            if (!$.isUndefined(a.duration)) path.animate(obj, a.duration);
                            else path.animate(obj);
                        }
                    }
                };
                icon.rotation = function (s, b, a) {
                    if ($.isNumber(s)) {
                        if ($.isUndefined(b)) {
                            rotation = s;
                            calculate();
                        }
                        else {
                            var anim, to;
                            if ($.isNumber(b) && $.isPlainObject(a)) {
                                rotation = s;
                                calculate();
                                anim = a;
                                to = b;
                            }
                            else {
                                anim = b;
                                to = s;
                            }

                            var obj = {
                                step: function (now, fx) {
                                    if (!$.isUndefined(anim.step)) anim.step(now, fx);
                                    rotation = now;
                                    calculate();
                                }
                            };
                            if (!$.isUndefined(anim.duration)) obj.duration = anim.duration;
                            if (!$.isUndefined(anim.complete)) obj.complete = anim.complete;
                            if (!$.isUndefined(anim.easing)) obj.easing = anim.easing;

                            $({ n: rotation }).animate({ n: to }, obj);
                        }
                    }
                };
                icon.flip = function (s) {
                    if ($.isString(s)) {
                        var ss = s.toLowerCase();
                        if (ss == "horizontal" || ss == "h") {
                            fliph = true;
                            flipv = false;
                        }
                        else if (ss == "vertical" || ss == "v") {
                            fliph = false;
                            flipv = true;
                        }
                        else if (ss == "horizontalvertical" || ss == "hv" || ss == "verticalhorizontal" || ss == "vh") {
                            fliph = true;
                            flipv = true;
                        }
                        else if (ss == "") {
                            fliph = false;
                            flipv = false;
                        }
                        calculate();
                    }
                };
                var dwidth = d.width;
                var dheight = d.height;
                icon.width = function (w) {
                    if ($.isUndefined(w)) return dwidth.apply(this, []);
                    else {
                        dwidth.apply(this, [w]);
                        scalew = (w / tw);
                        calculate();

                    }
                };
                icon.height = function (h) {
                    if ($.isUndefined(h)) return dheight.apply(this, []);
                    else {
                        dheight.apply(this, [h]);
                        scaleh = (h / tw);
                        calculate();
                    }
                };
                icon.stroke = function (c) {
                    if ($.isPlainObject(c)) {
                        path.attr({ stroke: ui.color(c.color), "stroke-width": c.size });
                    }
                    else {
                        return { color: path.attr("stroke"), size: path.attr("stroke-width") };
                    }
                };

                d.color("accent");

                return d;
            }
            else return null;
        };
        ui.raphael = function (container) {
            return graphics(container, "raphael");
        };
        ui.graphics = graphics;
        ui.icon = icon;

    })(ui);

    // .performance
    (function (ui) {

        var performance_ = function () {

            var stopwatch = performance.now();

            var area = $("<div style=\"visibility:hidden\" />");
            $(document.body).append(area);
            for (var i = 0; i < 100; i++) {
                var part = $("<div>afis</div>");
                area.append(part);
            }

            var elapsed = performance.now() - stopwatch;

            area.remove();

            return elapsed;
        };

        ui.performance = performance_;

    })(ui);

})(jQuery);
