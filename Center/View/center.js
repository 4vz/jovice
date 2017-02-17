

/*! Center Main */
(function () {

    var center;

    // center
    (function (window) {

        var inited = false;
        var page = null;
        var onmain = false;

        var shade, close, closeThings, offline, update, frontLogo, frontLogoElements, frontLogoDone = false;
        var ctop;
        var nbox;
        var asea;

        var tbox;
        var ttitle;

        var loadingStarted = false;
        var lbox;

        var ogbox;


        // sign in
        var signinArea = null;

        center = function () { };
        center.init = function (p) {
            page = p;

            // page resize
            var opageresize = page.resize;
            page.resize = function () {
                onResize();
                opageresize.apply(this, [page]);
            };
            
            if (inited) {
                onChangePage();
                return p;
            }
            inited = true;

            // ctop
            ctop = ui.box(ui.topContainer())({ color: 98, height: 40, width: "100%" });
            close = ui.box(ui.topContainer())({ color: 95, width: "100%", z: 999 });

            closeThings = ui.box(close)({ size: [500, 300], center: true });
            offline = ui.text(closeThings)({ hide: true, text: "OFFLINE", color: 50, font: 30, top: 80 });
            update = ui.text(closeThings)({ hide: true, text: "PLEASE REFRESH YOUR BROWSER", color: 50, top: 125 })
            frontLogo = ui.box(closeThings)({ size: [200, 200], left: 150 });
            frontLogoHexaAnim();

            shade = ui.box(ui.topContainer())({ hide: true, color: 95, width: "100%", z: 888 });

            // loading bar
            lbox = ui.box(ui.topContainer())({ color: "accent", opacity: 0, height: 2, left: "0%", width: "0%", z: 600 });

            // page margin top
            ui.marginTop(40);

            // logo & title
            nbox = ui.box(ctop)({ left: 15, size: 40, cursor: "pointer", click: function () { page.transfer("/"); } });
            var ileft = ui.icon(nbox, center.icon("hex"))({ size: 18, color: 75, position: [3, 17], rotation: 90 });
            var iright = ui.icon(nbox, center.icon("hex"))({ size: 18, color: 75, position: [16.25, 17], rotation: 90 });
            var itop = ui.icon(nbox, center.icon("hex"))({ size: 18, color: 75, position: [9.5, 5.5], rotation: 90 });
            nbox.hover(function () {
                itop.color(25, { duration: 166 });
                ileft.color("accent", { duration: 166 });
                iright.color(55, { duration: 166 });
            }, function () {
                itop.color(75, { duration: 166 });
                ileft.color(75, { duration: 166 });
                iright.color(75, { duration: 166 });
            });

            ui.box(ctop)({ left: 55, height: 27, top: 7, width: 1, color: 85 });

            tbox = ui.box(ctop)({ left: 56, height: 40, cursor: "pointer", hide: true });
            ttitle = ui.text(tbox)({ left: 10, top: 9, noSelect: true, noBreak: true, font: ["body", 18], weight: "600", color: 75, resize: function(r) {
                tbox.width(r.width + 20);
            }});
            tbox.width(ttitle.textSize().width + 20);
            tbox.hover(function () {
                tbox.color(100, { duration: 100 });
                ttitle.color(35, { duration: 100 });
            }, function () {
                tbox.color(null, { duration: 166 });
                ttitle.color(75, { duration: 166 });
            });

            // login
            ogbox = ui.box(ctop)({ right: 15, width: 100, height: 40, cursor: "pointer", hide: true, click: function () { page.transfer("/signin", { transition: "slideleft" }); } });
            var ogtext = ui.text(ogbox)({ text: "SIGN IN", font: ["head", 12], position: [26, 14], color: 55 });
            ogbox.hover(function () {
                ogbox.color(100, { duration: 100 });
                ogtext.color(35, { duration: 100 });
            }, function () {
                ogbox.color(null, { duration: 166 });
                ogtext.color(55, { duration: 166 });
            });
            
            
            // search
            var aseaentered = false;
            var aseadowned = false;
            var aseaactive = false;            
            asea = ui.box(ctop)({ color: 98, height: 40, width: 400, cursor: "text", left: 56 });
            var aseasearch = ui.textinput(asea)({ font: 14, leftRight: [40, 10], top: 0, height: 40, design: false, opacity: 0 });
            var aseaglass = ui.icon(asea, center.icon("glass"))({ size: [32, 32], color: 75, position: [4, 4] });
            var aseaclear = ui.icon(asea, center.icon("arrow"))({ size: [23, 23], color: 75, position: [8, 10], hide: true });
            var aseastart = ui.text(asea)({ noSelect: true, text: "Search anything here", font: ["body", 14], color: 65, position: [40, 11] });
            
            asea.enter(function (e) {
                aseaentered = true;
            });
            asea.leave(function (e) {
                aseaentered = false;
            });
            asea.down(function () {
                aseadowned = true;
                aseasearch.focus();
            });
            aseasearch.focusin(function () {
                aseadowned = false;
                if (!aseaactive) {
                    aseaactive = true;

                    aseasearch({ opacity: 1, color: [0, { duration: 500 }] });
                    aseastart.fadeOut(50);
                    asea.color(102, { duration: 500 });
                }
            });            
            aseasearch.focusout(function () {
                if (aseaactive && !aseadowned) {
                    aseaactive = false;

                    if (aseasearch.value() != "") {
                        aseasearch.color(65, { duration: 500 });
                        asea.color(98, { duration: 500 });
                    }
                    else {                        
                        aseasearch({ opacity: 0 });
                        aseastart.fadeIn(50);
                        asea.color(98, { duration: 500 });
                    }
                }
            });
            aseasearch.keydown(function (c) {
                if (c.key == 13) {
                    var s = aseasearch.value();
                    s = s.replace(/ +(?= )/g, '');
                    s = s.replaceAll(' ', '+');
                    s = escape(s);
                    page.transfer("/search/" + s);
                }
            });

            center.setSearchBoxValue = function (s) {
                if (!aseasearch.isFocus()) {
                    if (s != null && s.length > 0) {
                        aseasearch({ opacity: 1, value: s, color: 65 });
                        aseastart.hide();
                    }
                    else {
                        aseasearch.value(null);
                        aseastart.show();
                    }
                }
                else {
                    aseasearch.value(s == null ? "" : s);
                    if (s == null || s.length == 0) aseastart.show();
                    else aseastart.hide();
                }
            };

            var firstTime = true;

            $$.stream(function (type, data) {
                if (type == "online") {
                    if (firstTime) {
                        firstTime = false;

                        $$(function () {}, 100, function () {                            
                            if (!frontLogoDone) return -1;
                            else close.fadeOut(100);
                        });
                    }
                    else close.fadeOut(100);
                }
                else if (type == "offline") {
                    if (firstTime) {
                        $.each(frontLogoElements, function (eli, el) {
                            el.animate({ transform: "s0.5,0.5,100,20", "stroke-width": 1, stroke: ui.color(35) }, 500, "easeInOut");
                        });
                        offline.text("OFFLINE");
                        offline.left((500 - offline.textSize().width) / 2);
                        $$(600, function () { offline.fadeIn(100); });
                        firstTime = false;
                    }
                    else {
                        $.each(frontLogoElements, function (eli, el) {
                            el.transform("s0.5,0.5,100,20");
                            el.attr({ "stroke-width": 1, stroke: ui.color(35) });
                        });
                        close.fadeIn(100);
                        onResize();
                        offline.show();
                        update.hide();
                        offline.text("OFFLINE");
                        offline.left((500 - offline.textSize().width) / 2);
                        $(':focus').blur();
                    }
                }
                else if (type == "update") {
                    $.each(frontLogoElements, function (eli, el) {
                        el.transform("s0.5,0.5,100,20");
                        el.attr({ "stroke-width": 1, stroke: ui.color(35) });
                    });
                    close.fadeIn(100);
                    onResize();
                    offline.show();
                    offline.text("NEW UPDATE IS AVAILABLE");
                    offline.left((500 - offline.textSize().width) / 2);
                    update.show();
                    update.left((500 - update.textSize().width) / 2);
                }

                //if (type == "online") offline.hide();
                //else if (type == "offline") offline.show();
            });

            return p;
        };
        center.icon = function (name) {
            if (name == "hex") return "M50.518,188.535h101.038l50.515-87.5l-50.515-87.5H50.518L0,101.035L50.518,188.535z M59.178,28.535h83.718l41.854,72.5l-41.854,72.5H59.178l-41.858-72.5L59.178,28.535z";
            else if (name == "glass") return "M480.606,459.394L352.832,331.619c30.306-35.168,48.654-80.918,48.654-130.876C401.485,90.053,311.433,0,200.743,0S0,90.053,0,200.743s90.053,200.743,200.743,200.743c49.958,0,95.708-18.348,130.876-48.654l127.775,127.775L480.606,459.394zM30,200.743C30,106.595,106.595,30,200.743,30s170.743,76.595,170.743,170.743s-76.595,170.743-170.743,170.743S30,294.891,30,200.743z";
            else if (name == "arrow") return "M20,11H7.8l5.6-5.6L12,4l-8,8l8,8l1.4-1.4L7.8,13H20V11z";
            else if (name == "time") return "M48.58,0C21.793,0,0,21.793,0,48.58s21.793,48.58,48.58,48.58s48.58-21.793,48.58-48.58S75.367,0,48.58,0z M48.58,86.823	c-21.087,0-38.244-17.155-38.244-38.243S27.493,10.337,48.58,10.337S86.824,27.492,86.824,48.58S69.667,86.823,48.58,86.823z M73.898,47.08H52.066V20.83c0-2.209-1.791-4-4-4c-2.209,0-4,1.791-4,4v30.25c0,2.209,1.791,4,4,4h25.832 c2.209,0,4-1.791,4-4S76.107,47.08,73.898,47.08z";
            else if (name == "map") return "M425.298,28.93l-121.55,78.2L181.915,3.996c-5.95-4.817-14.167-5.383-20.683-1.133L34.015,89.846 c-4.533,3.117-7.367,8.5-7.367,14.167V419.93c0,6.233,3.4,12.183,9.067,15.017s12.183,2.55,17.567-1.133l107.383-73.383 l130.05,113.617c3.117,2.833,7.083,4.25,11.05,4.25c3.117,0,5.95-0.85,8.783-2.55l132.883-79.617c5.1-3.117,8.217-8.5,8.217-14.45  V43.096c0-6.233-3.4-11.9-8.783-15.017C437.198,25.246,430.682,25.53,425.298,28.93z M417.648,372.33l-113.9,68.283 l-124.1-108.517v-64.033c0-9.35-7.65-17-17-17c-9.35,0-17,7.65-17,17v61.767l-85,58.083V113.08l109.083-74.517l114.75,96.617 v215.05c0,9.35,7.65,17,17,17s17-7.65,17-17V138.296l99.167-64.033V372.33zM221.865,111.663c-6.517-6.517-17.283-6.517-24.083,0l-28.9,28.9l-28.9-28.9c-6.517-6.517-17.283-6.517-24.083,0 c-6.517,6.517-6.517,17.283,0,24.083l28.9,28.9l-28.9,29.183c-6.517,6.517-6.517,17.283,0,24.083c3.4,3.4,7.65,5.1,11.9,5.1  c4.25,0,8.783-1.7,11.9-5.1l29.183-29.183l28.9,28.9c3.4,3.4,7.65,5.1,11.9,5.1s8.783-1.7,11.9-5.1 c6.517-6.517,6.517-17.283,0-24.083l-28.617-28.9l28.9-28.9C228.665,128.946,228.665,118.18,221.865,111.663z";             
            else if (name == "topology") return "M305.188,65.777c-15.874,0-28.789,12.915-28.789,28.789c0,5.521,1.565,10.682,4.271,15.068l-59.435,70.333	c-3.445-1.46-7.23-2.269-11.201-2.269c-6.512,0-12.522,2.176-17.352,5.835l-50.122-36.198c0.716-2.507,1.106-5.15,1.106-7.884 c0-15.873-12.915-28.786-28.789-28.786c-15.875,0-28.789,12.914-28.789,28.786c0,5.319,1.456,10.303,3.981,14.583L40.535,213.14c-3.591-1.611-7.564-2.516-11.748-2.516C12.914,210.624,0,223.538,0,239.41c0,15.874,12.914,28.789,28.787,28.789c15.874,0,28.789-12.914,28.789-28.789c0-5.136-1.358-9.958-3.726-14.137l49.733-59.343c3.47,1.485,7.287,2.31,11.293,2.31c6.61,0,12.703-2.246,17.568-6.006l49.982,36.096c-0.766,2.588-1.184,5.323-1.184,8.156c0,15.873,12.915,28.787,28.789,28.787c15.873,0,28.787-12.914,28.787-28.787c0-5.355-1.475-10.372-4.032-14.671l59.63-70.562c3.329,1.348,6.963,2.098,10.77,2.098c15.874,0,28.789-12.914,28.789-28.786C333.977,78.692,321.061,65.777,305.188,65.777z";
            else if (name == "speed") return "M52.173,57.876c-0.028-0.329-0.122-0.658-0.286-0.971L31.254,17.971c-0.609-1.144-1.983-1.684-3.229-1.267    c-1.248,0.423-1.993,1.674-1.756,2.95l8.041,43.173c0.063,0.343,0.196,0.65,0.377,0.93c0.078,0.369,0.173,0.736,0.3,1.104    c1.295,3.694,4.838,6.184,8.813,6.184c1.027,0,2.048-0.166,3.024-0.497c2.354-0.792,4.253-2.438,5.347-4.636    c1.095-2.198,1.251-4.68,0.444-6.994C52.492,58.555,52.34,58.21,52.173,57.876z M43.803,19.533c-1.471,0-2.662,1.17-2.662,2.613    c0,1.443,1.191,2.614,2.662,2.614c21.218,0,38.476,16.961,38.476,37.806c0,1.442,1.195,2.613,2.664,2.613    c1.473,0,2.664-1.171,2.664-2.613C87.605,38.834,67.956,19.533,43.803,19.533z M16.276,29.087C5.933,37.306,0,49.507,0,62.565    c0,1.443,1.191,2.615,2.663,2.615c1.474,0,2.665-1.172,2.665-2.615c0-11.47,5.211-22.189,14.298-29.409    c1.146-0.908,1.32-2.556,0.397-3.679C19.1,28.355,17.423,28.179,16.276,29.087z";
            else if (name == "cloud") return "M320,128c52.562,0,95.375,42.438,96,94.813c-0.25,1.938-0.438,3.875-0.5,5.875l-0.812,23.5l22.25,7.75    C462.688,268.906,480,293.062,480,320c0,35.312-28.688,64-64,64H96c-35.281,0-64-28.688-64-64c0-34.938,28.188-63.438,63-64    c1.5,0.219,3.063,0.406,4.625,0.5l24.313,1.594l8-22.969C140.938,209.313,165.063,192,192,192c3.125,0,6.563,0.375,11.188,1.188    l22.406,4.031l11.156-19.844C253.875,146.938,285.75,128,320,128 M320,96c-47.938,0-89.219,26.688-111.156,65.688    C203.375,160.719,197.781,160,192,160c-41.938,0-77.219,27.063-90.281,64.563C99.813,224.438,97.969,224,96,224c-53,0-96,43-96,96    s43,96,96,96h320c53,0,96-43,96-96c0-41.938-27.062-77.25-64.562-90.313C447.5,227.75,448,225.938,448,224    C448,153.313,390.688,96,320,96L320,96z";
            else if (name == "IP") return "M6.776,4.72h1.549v6.827H6.776V4.72z M11.751,4.669c-0.942,0-1.61,0.061-2.087,0.143v6.735h1.53    V9.106c0.143,0.02,0.324,0.031,0.527,0.031c0.911,0,1.691-0.224,2.218-0.721c0.405-0.386,0.628-0.952,0.628-1.621    c0-0.668-0.295-1.234-0.729-1.579C13.382,4.851,12.702,4.669,11.751,4.669z M11.709,7.95c-0.222,0-0.385-0.01-0.516-0.041V5.895    c0.111-0.03,0.324-0.061,0.639-0.061c0.769,0,1.205,0.375,1.205,1.002C13.037,7.535,12.53,7.95,11.709,7.95z M10.117,0    C5.523,0,1.8,3.723,1.8,8.316s8.317,11.918,8.317,11.918s8.317-7.324,8.317-11.917S14.711,0,10.117,0z M10.138,13.373    c-3.05,0-5.522-2.473-5.522-5.524c0-3.05,2.473-5.522,5.522-5.522c3.051,0,5.522,2.473,5.522,5.522    C15.66,10.899,13.188,13.373,10.138,13.373z";
            else if (name == "split") return "M255,0L313.65 58.65 239.7 132.6 275.4 168.3 349.35 94.35 408 153 408 0 z M153,0L0 0 0 153 58.65 94.35 178.5 214.2 178.5 408 229.5 408 229.5 193.8 94.35 58.65 z";
            else if (name == "upload") return "M174.804,349.607L301.934 349.607 301.266 158.912 397.281 158.912 238.369 0 79.456 158.912 174.804 158.912 z  M365.499,349.607L365.499 413.172 111.239 413.172 111.239 349.607 47.674 349.607 47.674 476.737 429.063 476.737 429.063 349.607 z";
            else if (name == "download") return "M227.996,334.394L379.993 182.397 288.795 182.397 288.795 0 167.197 0 167.744 182.397 75.999 182.397 z  M349.594,334.394L349.594 395.193 106.398 395.193 106.398 334.394 45.599 334.394 45.599 395.193 45.599 455.992 410.393 455.992 410.393 334.394 z";
            else if (name == "boxin") return "M349.138,19.909L329.195 0 189.656 139.531 189.656 62.663 161.481 62.663 161.481 187.65 286.43 187.65 286.453 159.46 209.574 159.46 zM321.749,161.568L321.749 321.755 29.28 321.755 29.28 29.273 187.464 29.273 187.464 0 0.006 0 0.006 351.028 351.022 351.028 351.022 161.568 z";
            else if (name == "boxout") return "M215.013,223.333L241.569 249.84 427.364 64.053 427.364 166.4 464.869 166.4 464.869 0 298.51 0 298.477 37.522 400.848 37.522 z M428.412,215.131L428.412 428.417 38.989 428.417 38.989 38.977 249.608 38.977 249.608 0 0.004 0 0.004 467.393 467.389 467.393 467.389 215.131 z";
            else if (name == "boxsel") return "M0,162.3L24.419 162.3 24.419 29.687 156.539 29.687 156.539 5.269 0 5.269 z M187.302,5.509L187.302 29.927 319.924 29.927 319.924 162.042 344.339 162.042 344.339 5.509 zM24.419,182.038L0 182.038 0 339.07 156.539 339.07 156.539 314.648 24.419 314.648 zM319.924,314.408L187.302 314.408 187.302 338.83 344.339 338.83 344.339 182.291 319.924 182.291 z";
            else if (name == "excl") return "M244.709,389.496c18.736,0,34.332-14.355,35.91-33.026l24.359-290.927c1.418-16.873-4.303-33.553-15.756-46.011      C277.783,7.09,261.629,0,244.709,0s-33.074,7.09-44.514,19.532C188.74,31.99,183.022,48.67,184.44,65.543l24.359,290.927  C210.377,375.141,225.973,389.496,244.709,389.496z M244.709,410.908c-21.684,0-39.256,17.571-39.256,39.256c0,21.683,17.572,39.254,39.256,39.254 s39.256-17.571,39.256-39.254C283.965,428.479,266.393,410.908,244.709,410.908z";
            else if (name == "gear") return "M113.595,133.642l-5.932-13.169c5.655-4.151,10.512-9.315,14.307-15.209l13.507,5.118c2.583,0.979,5.469-0.322,6.447-2.904    l4.964-13.103c0.47-1.24,0.428-2.616-0.117-3.825c-0.545-1.209-1.547-2.152-2.788-2.622l-13.507-5.118      c1.064-6.93,0.848-14.014-0.637-20.871l13.169-5.932c1.209-0.545,2.152-1.547,2.622-2.788c0.47-1.24,0.428-2.616-0.117-3.825       l-5.755-12.775c-1.134-2.518-4.096-3.638-6.612-2.505l-13.169,5.932c-4.151-5.655-9.315-10.512-15.209-14.307l5.118-13.507      c0.978-2.582-0.322-5.469-2.904-6.447L93.88,0.82c-1.239-0.469-2.615-0.428-3.825,0.117c-1.209,0.545-2.152,1.547-2.622,2.788   l-5.117,13.506c-6.937-1.07-14.033-0.849-20.872,0.636L55.513,4.699c-0.545-1.209-1.547-2.152-2.788-2.622   c-1.239-0.469-2.616-0.428-3.825,0.117L36.124,7.949c-2.518,1.134-3.639,4.094-2.505,6.612l5.932,13.169   c-5.655,4.151-10.512,9.315-14.307,15.209l-13.507-5.118c-1.239-0.469-2.615-0.427-3.825,0.117    c-1.209,0.545-2.152,1.547-2.622,2.788L0.326,53.828c-0.978,2.582,0.322,5.469,2.904,6.447l13.507,5.118   c-1.064,6.929-0.848,14.015,0.637,20.871L4.204,92.196c-1.209,0.545-2.152,1.547-2.622,2.788c-0.47,1.24-0.428,2.616,0.117,3.825  l5.755,12.775c0.544,1.209,1.547,2.152,2.787,2.622c1.241,0.47,2.616,0.429,3.825-0.117l13.169-5.932   c4.151,5.656,9.314,10.512,15.209,14.307l-5.118,13.507c-0.978,2.582,0.322,5.469,2.904,6.447l13.103,4.964  c0.571,0.216,1.172,0.324,1.771,0.324c0.701,0,1.402-0.147,2.054-0.441c1.209-0.545,2.152-1.547,2.622-2.788l5.117-13.506  c6.937,1.069,14.034,0.849,20.872-0.636l5.931,13.168c0.545,1.209,1.547,2.152,2.788,2.622c1.24,0.47,2.617,0.429,3.825-0.117  l12.775-5.754C113.607,139.12,114.729,136.16,113.595,133.642z M105.309,86.113c-4.963,13.1-17.706,21.901-31.709,21.901 c-4.096,0-8.135-0.744-12.005-2.21c-8.468-3.208-15.18-9.522-18.899-17.779c-3.719-8.256-4-17.467-0.792-25.935  c4.963-13.1,17.706-21.901,31.709-21.901c4.096,0,8.135,0.744,12.005,2.21c8.468,3.208,15.18,9.522,18.899,17.778  C108.237,68.434,108.518,77.645,105.309,86.113z M216.478,154.389c-0.896-0.977-2.145-1.558-3.469-1.615l-9.418-0.404  c-0.867-4.445-2.433-8.736-4.633-12.697l6.945-6.374c2.035-1.867,2.17-5.03,0.303-7.064l-6.896-7.514  c-0.896-0.977-2.145-1.558-3.47-1.615c-1.322-0.049-2.618,0.416-3.595,1.312l-6.944,6.374c-3.759-2.531-7.9-4.458-12.254-5.702  l0.404-9.418c0.118-2.759-2.023-5.091-4.782-5.209l-10.189-0.437c-2.745-0.104-5.091,2.023-5.209,4.781l-0.404,9.418   c-4.444,0.867-8.735,2.433-12.697,4.632l-6.374-6.945c-0.896-0.977-2.145-1.558-3.469-1.615c-1.324-0.054-2.618,0.416-3.595,1.312  l-7.514,6.896c-2.035,1.867-2.17,5.03-0.303,7.064l6.374,6.945c-2.531,3.759-4.458,7.899-5.702,12.254l-9.417-0.404    c-2.747-0.111-5.092,2.022-5.21,4.781l-0.437,10.189c-0.057,1.325,0.415,2.618,1.312,3.595c0.896,0.977,2.145,1.558,3.47,1.615  l9.417,0.403c0.867,4.445,2.433,8.736,4.632,12.698l-6.944,6.374c-0.977,0.896-1.558,2.145-1.615,3.469  c-0.057,1.325,0.415,2.618,1.312,3.595l6.896,7.514c0.896,0.977,2.145,1.558,3.47,1.615c1.319,0.053,2.618-0.416,3.595-1.312  l6.944-6.374c3.759,2.531,7.9,4.458,12.254,5.702l-0.404,9.418c-0.118,2.759,2.022,5.091,4.781,5.209l10.189,0.437  c0.072,0.003,0.143,0.004,0.214,0.004c1.25,0,2.457-0.468,3.381-1.316c0.977-0.896,1.558-2.145,1.615-3.469l0.404-9.418  c4.444-0.867,8.735-2.433,12.697-4.632l6.374,6.945c0.896,0.977,2.145,1.558,3.469,1.615c1.33,0.058,2.619-0.416,3.595-1.312  l7.514-6.896c2.035-1.867,2.17-5.03,0.303-7.064l-6.374-6.945c2.531-3.759,4.458-7.899,5.702-12.254l9.417,0.404  c2.756,0.106,5.091-2.022,5.21-4.781l0.437-10.189C217.847,156.659,217.375,155.366,216.478,154.389z M160.157,183.953 c-12.844-0.55-22.846-11.448-22.295-24.292c0.536-12.514,10.759-22.317,23.273-22.317c0.338,0,0.678,0.007,1.019,0.022 c12.844,0.551,22.846,11.448,22.295,24.292C183.898,174.511,173.106,184.497,160.157,183.953z";
            else if (name == "placeholder") return "M516.316,337.52l94.233,193.581c3.832,7.873-0.196,14.314-8.952,14.314H10.402c-8.756,0-12.785-6.441-8.952-14.314 L95.684,337.52c1.499-3.079,5.528-5.599,8.952-5.599h80.801c2.49,0,5.853,1.559,7.483,3.442     c5.482,6.335,11.066,12.524,16.634,18.65c5.288,5.815,10.604,11.706,15.878,17.735h-95.891c-3.425,0-7.454,2.519-8.952,5.599   L58.163,505.589h495.67l-62.421-128.242c-1.498-3.08-5.527-5.599-8.953-5.599h-96.108c5.273-6.029,10.591-11.92,15.879-17.735   c5.585-6.144,11.2-12.321,16.695-18.658c1.628-1.878,4.984-3.434,7.47-3.434h80.971  C510.789,331.921,514.817,334.439,516.316,337.52z M444.541,205.228c0,105.776-88.058,125.614-129.472,227.265  c-3.365,8.26-14.994,8.218-18.36-0.04c-37.359-91.651-112.638-116.784-127.041-198.432  c-14.181-80.379,41.471-159.115,122.729-166.796C375.037,59.413,444.541,124.204,444.541,205.228z M379.114,205.228  c0-40.436-32.779-73.216-73.216-73.216s-73.216,32.78-73.216,73.216c0,40.437,32.779,73.216,73.216,73.216  S379.114,245.665,379.114,205.228z";
            else if (name == "route") return "M303.658,141.433L350.261 214.575 387.626 0 173.043 37.357 246.184 83.962 104.768 225.371 246.78 367.377 179.243 434.921 236.731 492.394 361.74 367.377 219.726 225.371z";
            else if (name == "warning") return "M15.789,13.982l-6.938-13C8.678,0.657,8.339,0.453,7.97,0.453H7.969c-0.369,0-0.707,0.203-0.881,0.528l-6.969,13c-0.166,0.312-0.157,0.686,0.023,0.986C0.323,15.268,0.649,15.453,1,15.453h13.906c0.352,0,0.677-0.184,0.858-0.486C15.945,14.666,15.954,14.292,15.789,13.982z M7.969,13.453c-0.552,0-1-0.448-1-1s0.448-1,1-1c0.551,0,1,0.448,1,1S8.521,13.453,7.969,13.453z M8.97,9.469c0,0.553-0.449,1-1,1c-0.552,0-1-0.447-1-1v-4c0-0.552,0.448-1,1-1 c0.551,0,1,0.448,1,1V9.469z";
        };
        center.formatInterfaceName = function (name, man) {
            if (man == "ALCATEL-LUCENT") {
                if (name.startsWith("Ex")) {
                    name = name.substr(2);
                }
                else if (name.startsWith("Ag")) {
                    name = "lag-" + name.substr(2);
                }

                if (name.endsWith(".DIRECT")) {
                    name = name.substr(0, name.indexOf(".DIRECT"));
                }

                if (name.endsWith(".1") || name.indexOf(".1.") > -1) {

                }
                else {
                    var firstdot = name.indexOf(".");
                    if (firstdot > -1) {
                        name = name.substr(0, firstdot) + ":" + name.substr(firstdot + 1);
                    }
                }
            }
            else if (man == "HUAWEI") {
                if (name.startsWith("Ag")) {
                    name = "Eth-Trunk" + name.substr(2);
                }
            }

            return name;
        };
        center.searchExecute = function (s) {
            page.transfer("/search/" + prepareQuery(s));
        };
        center.startLoading = function () {
            if (!loadingStarted) {
                loadingStarted = true;

                lbox.opacity(0.5);
                lboxExpand();
            }
        };
        center.endLoading = function () {
            if (loadingStarted) {
                loadingStarted = false;

                $$(500, function () {
                    lbox.$.animate({ opacity: 0 }, {
                        duration: 300, queue: false, complete: function () {
                            lbox.$.stop();
                        }
                    });
                });
            }
        };
        center.showSignIn = function () {
            shade.show();
            shade.height(page.height() + ui.marginTop() + ui.marginBottom());
            shade.width(page.width() + ui.marginLeft() + ui.marginRight());
            shade.$.css({ opacity: 0 }).animate({ opacity: .8 }, { duration: 166, queue: false });

            if (signinArea == null) {
                signinArea = ui.box(ui.topContainer())({ size: [500, 400], z: 889 });
                signinArea.position((ui.width() - signinArea.width()) / 2, (ui.height() - signinArea.height()) / 2);                
                //signInHexaAnim();
            }
        };
        center.formatBytes = function (bytes, decimals) {
            if (bytes == 0) return '0 Byte';
            var k = 1024;
            var dm = decimals + 1 || 3;
            var sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
            var i = Math.floor(Math.log(bytes) / Math.log(k));
            return [(bytes / Math.pow(k, i)).toPrecision(dm), sizes[i]];
        };
        center.loadGoogleMaps = function (callback) {

            if (googleMapsLoaded == false) {
                $.getScript("https://maps.googleapis.com/maps/api/js?key=" + "AIzaSyCTfWC0APQfYHHL-2l-HlLXDWw2g_4zE80", function () {
                    googleMapsLoaded = true;
                    callback();
                });
            }
            else callback();
        };

        var googleMapsLoaded = false;
                
        function onResize() {

            var hw = ui.height();
            var ww = ui.width();

            if (close.isShown()) {
                close.height(hw);
                close.width(ww);
            }
            if (shade.isShown()) {
                shade.height(hw);
                shade.width(ww);
            }
        };
        function onChangePage() {
            if (shade.isShown()) {
                shade.hide();
            }
        };
        function lboxExpand() {
            lbox.$.css({ width: "0%", left: "0%" }).animate({ width: "100%" }, { duration: 500, queue: false, complete: function () { lboxCollapse(); } });
        };
        function lboxCollapse() {
            lbox.$.css({ width: "100%", left: "0%" }).animate({ left: "100%", width: "0%" }, { duration: 500, queue: false, complete: function () { lboxExpand(); } });
        };
        
        function hex_corner(x, y, size, i) {
            var angle_deg = 60 * i + 30
            var angle_rad = Math.PI / 180 * angle_deg
            return { x: x + size * Math.cos(angle_rad), y: y + size * Math.sin(angle_rad) };
        };
        function hex_pos(center, size, i) {
            var pos = hex_corner(center.x, center.y, size, i);
            return pos.x + "," + pos.y;
        };

        function frontLogoHexaAnimRec(el, paths, count) {

            if (count >= paths.length) {
                frontLogoDone = true;
                return;
            }

            var path;
            for (var i = 0; i <= count; i++) {
                path += paths[i];
            }

            var last = count == paths.length - 1;

            el.animate({ path: path }, last ? 166 : 33, last ? "easeOut" : "linear", function () {
                frontLogoHexaAnimRec(el, paths, count + 1);
            });
        };
        function frontLogoHexaAnim() {
            var area = ui.raphael(frontLogo)({ size: [200, 200] });
            var paper = area.paper();
            
            var center1 = { x: 100, y: 50 };
            var center2 = { x: 70.25, y: 102 };
            var center3 = { x: 129.75, y: 102 };
            var size = 30;

            var paths1 = ("M" + hex_pos(center1, size, 0) + "L" + hex_pos(center1, size, 1) + "L" + hex_pos(center1, size, 2) + " L" +
                 hex_pos(center1, size, 3) + " L" + hex_pos(center1, size, 4) + " L" + hex_pos(center1, size, 5) + " Z").split(" ");
            var paths2 = ("M" + hex_pos(center2, size, 4) + "L" + hex_pos(center2, size, 5) + "L" + hex_pos(center2, size, 0) + " L" +
                hex_pos(center2, size, 1) + " L" + hex_pos(center2, size, 2) + " L" + hex_pos(center2, size, 3) + " Z").split(" ");
            var paths3 = ("M" + hex_pos(center3, size, 0) + "L" + hex_pos(center3, size, 5) + "L" + hex_pos(center3, size, 4) + " L" +
                hex_pos(center3, size, 3) + " L" + hex_pos(center3, size, 2) + " L" + hex_pos(center3, size, 1) + " Z").split(" ");
            var el1 = paper.path(paths1[0]).attr({ stroke: ui.color(25), opacity: 0, "stroke-width": 2 });
            var el2 = paper.path(paths2[0]).attr({ stroke: ui.color("accent"), opacity: 0, "stroke-width": 2 });
            var el3 = paper.path(paths3[0]).attr({ stroke: ui.color(55), opacity: 0, "stroke-width": 2 });
            frontLogoHexaAnimRec(el1, paths1, 1);
            frontLogoHexaAnimRec(el2, paths2, 1);
            frontLogoHexaAnimRec(el3, paths3, 1);
            el1.animate({ opacity: 1 }, 516);
            el2.animate({ opacity: 1 }, 516);
            el3.animate({ opacity: 1 }, 516);
            frontLogoElements = [el1, el2, el3];
        };
        
        window.center = center;
        window.onerror = function (e) {

        };

        function prepareQuery(q) {
            q = q.replaceAll(' ', '+');
            q = escape(q);
            return q;
        };

    })(window);
})();