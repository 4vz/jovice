/*! development javascript */
(function () {
    "use strict";

})();

/*! Development Mode - 2 */
(function () {
    if (ui) {
        var t = $("#top");
        var b = $("#bottom");
        // debug on screen
        (function () {
            var x = [];
            var maxLine = 15;
            var last = null;
            var lastCount = 1;
            var lastDate = null;

            var _debug = window.debug;
            var debug = function () {
                //return;
                var o = arguments;
                var s = null;

                var ss;
                if (o.length == 1)
                    ss = JSON.stringify(o[0]);
                else {
                    var sss = [];
                    $.each(o, function (i, v) {
                        sss.push(JSON.stringify(v));
                    });
                    ss = sss.join(",");
                }

                try {
                    s = ss;
                }
                catch (err) {
                    s = "error while displaying onscreen debug: " + err;
                }

                if (s != null) {
                    var separated = false;
                    if (lastDate != null) {
                        var diff = $$.date() - lastDate;
                        //_debug(diff);
                        if (diff > 500) {
                            var sep = ui.box(b)({
                                size: [100, 5],
                                left: 10,
                                data: ["tag", "separator"],
                                color: "red"
                            });
                            x.splice(0, 0, sep);
                            separated = true;
                        }
                    }



                    if (last != null && s == last && separated == false) {
                        x[0].text($$.date("{HH}:{mm}:{ss}:{sss}") + ">" + s + "(" + (++lastCount) + ")");
                    }
                    else {
                        var te = ui.text(b);
                        te.text($$.date("{HH}:{mm}:{ss}:{sss}") + ">" + s);
                        te.backgroundColor(0);
                        te.color(100);
                        te.font("Courier New", 12);
                        te.left(10);
                        //te.bottom(x.length * 15 + 50);
                        x.splice(0, 0, te);
                        var csep = 0;
                        $.each(x, function (xi, xv) {

                            var issep = (xv.data("tag") == "separator");
                            if (issep)
                                csep++;

                            if (xi == maxLine) {
                                $.each(x, function (xxi, xxv) {
                                    if (xxi >= maxLine) {
                                        xxv.remove();
                                    }
                                });
                                x.splice(maxLine, x.length - maxLine);
                                return false;
                            }

                            if (issep)
                                xv.bottom((xi - csep) * 14 + 10 + (csep * 5) + 9);
                            else
                                xv.bottom((xi - csep) * 14 + 10 + (csep * 5));
                        });
                        lastCount = 1;
                    }

                    last = s;
                    lastDate = $$.date();

                }
                _debug.apply(this, o);
            };
            var originalDebug = function () {
                var o = arguments;
                _debug.apply(this, o);
            };

            window.debug = debug;
            window._debug = originalDebug;

        })();
    }
})();
