/*! Aphysoft.Share | share.aphysoft.com */
(function (window, $) {
    "use strict";

    window.debug = function (arg1) {
        console.debug(arg1);
    };
    window.assert = function (arg1, arg2, arg3) {
        console.assert(arg1, arg2, arg3)
    };

    function escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    // jquery document object
    $.window = $(window);

    // shorthand type detection for jQuery
    $.isObject = function (arg1) {
        console.info("$.isObject is deprecated");
        return arg1 == null ? false : typeof arg1 === "object" ? true : false;
    };
    $.isString = function (arg1) {
        return $.type(arg1) === "string";
    };
    $.isBoolean = function (arg1) {
        return $.type(arg1) === "boolean";
    };
    $.isNumber = function (arg1) {
        return $.type(arg1) === "number";
    };
    $.isDate = function (arg1) {
        return $.type(arg1) === "date";
    };
    $.isRegExp = function (arg1) {
        return $.type(arg1) === "regexp";
    };
    $.isNull = function (arg1) {
        return $.type(arg1) === "null";
    };
    $.isUndefined = function (arg1) {
        return $.type(arg1) === "undefined";
    };
    $.isNullOrUndefined = function (arg1) {
        return $.isNull(arg1) || $.isUndefined(arg1);
    };
    $.isNullOrFalse = function (arg1) {
        return $.isNull(arg1) || arg1 == false;
    };
    $.isJQuery = function (arg1) {
        return $.isNullOrUndefined(arg1) ? false : !$.isUndefined(arg1.jquery);
    };
    $.isBinary = function (arg1) {
        var string = Object.prototype.toString.call(arg1);
        return string === "[object Blob]" || string === "[object ArrayBuffer]";
    };
    $.isEmail = function (arg1) {
        return /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[a-z]{2}|com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)\b/.test(arg1);
    };
    $.isDomain = function (arg1) {
        return /(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[a-z]{2}|com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)\b/.test(arg1);
    };
    $.isColor = function (arg1) {
        return /(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test(arg1);
    };
    $.isEmpty = function (arg1) {
        if (!$.isUndefined(arg1.length)) return arg1.length == 0;
        else return false;
    };

    // no arguments
    $.no = $.isEmpty; // $.no(arguments)

    // other helper
    $.fn.setCursorPosition = function (position) {
        if (this.length == 0) return this;
        return $(this).setSelection(position, position);
    };
    $.fn.setSelection = function (selectionStart, selectionEnd) {
        if (this.length == 0) return this;
        var input = this[0];

        if (input.createTextRange) {
            var range = input.createTextRange();
            range.collapse(true);
            range.moveEnd('character', selectionEnd);
            range.moveStart('character', selectionStart);
            range.select();
        } else if (input.setSelectionRange) {
            input.focus();
            input.setSelectionRange(selectionStart, selectionEnd);
        }

        return this;
    };
    $.fn.focusEnd = function () {
        this.setCursorPosition(this.val().length);
        return this;
    };

    // String methods
    String.prototype.is = function (c, a) {
        if (this == c) a();
    };
    String.prototype.isIn = function (c) {
        if ($.inArray(this, c) > -1) return true;
        else return false;
    };
    if (!String.prototype.includes) {
        String.prototype.includes = function () {
            'use strict';
            return String.prototype.indexOf.apply(this, arguments) !== -1;
        };
    }
    String.prototype.replaceAll = function (find, replace) {
        return this.replace(new RegExp(escapeRegExp(find), 'g'), replace);
    };
    if (!String.prototype.startsWith) {
        String.prototype.startsWith = function (searchString, position) {
            position = position || 0;
            return this.indexOf(searchString, position) === position;
        };
    }
    if (!String.prototype.endsWith) {
        String.prototype.endsWith = function (searchString, position) {
            var subjectString = this.toString();
            if (position === undefined || position > subjectString.length) {
                position = subjectString.length;
            }
            position -= searchString.length;
            var lastIndex = subjectString.indexOf(searchString, position);
            return lastIndex !== -1 && lastIndex === position;
        };
    }
    String.prototype.paddingLeft = function (length, character) {
        if ($.isUndefined(length)) return this;

        var left = length - this.length;

        if ($.isUndefined(character)) character = " ";
        else if (!$.isString(character)) character = character + "";

        var output = "";
        for (var ic = 0; ic < left; ic += character.length)
            output = (left - ic > character.length ? character : character.substr(character.length - (left - ic), (left - ic))) + output;
        return output + this;
    };
    String.prototype.repeat = function (count) {
        var s = this;
        for (var i = 1; i < count; i++) s += this;
        return s;
    };
    String.prototype.insert = function (string, position) {
        if (!$.isString(string) && !$.isNumber(position)) return this;
        return this.substring(0, position) + string + this.substring(position);
    };
    String.prototype.nbsp = function () {
        return this.replace(/\s/g, "&nbsp;");
    };
    String.prototype.stripHTML = function () {
        return this.replace(/<(?:.|\n)*?>/gm, '');
    };

    // Number methods
    Number.prototype.is = function (c, a) {
        if (this == c) a();
    };
    Number.prototype.isNeg = function () {
        return this < 0 ? true : false;
    };
    Number.prototype.isPos = function () {
        return this > 0 ? true : false;
    };
    Number.prototype.isOdd = function () {
        return this % 2 == 1 ? true : false;
    };
    Number.prototype.isEven = function () {
        return this % 2 == 0 ? true : false;
    };
    Number.prototype.abs = function () {
        return Math.abs(this);
    };
    Number.prototype.floor = function () {
        return Math.floor(this);
    };
    Number.prototype.ceil = function () {
        return Math.ceil(this);
    };
    Number.prototype.pow = function (y) {
        return Math.pow(this, y);
    };
    Number.prototype.sqrt = function () {
        return Math.sqrt(this);
    };
    Number.prototype.log = function () {
        return Math.log(this);
    };
    Number.prototype.gte = function (a) {
        return a <= this ? true : false;
    };
    Number.prototype.between = function (a, b) {
        return a <= this && this < b ? true : false;
    };

    // jQuery add-on
    $.fn.copyCSS = function (source) {
        var dom = $(source).get(0);
        var dest = {};
        var style, prop;
        if (window.getComputedStyle) {
            var camelize = function (a, b) {
                return b.toUpperCase();
            };
            if (style = window.getComputedStyle(dom, null)) {
                var camel, val;
                if (style.length) {
                    for (var i = 0, l = style.length; i < l; i++) {
                        prop = style[i];
                        camel = prop.replace(/\-([a-z])/, camelize);
                        val = style.getPropertyValue(prop);
                        dest[camel] = val;
                    }
                } else {
                    for (prop in style) {
                        camel = prop.replace(/\-([a-z])/, camelize);
                        val = style.getPropertyValue(prop) || style[prop];
                        dest[camel] = val;
                    }
                }
                return this.css(dest);
            }
        }
        if (style = dom.currentStyle) {
            for (prop in style) {
                dest[prop] = style[prop];
            }
            return this.css(dest);
        }
        if (style = dom.style) {
            for (prop in style) {
                if (typeof style[prop] != 'function') {
                    dest[prop] = style[prop];
                }
            }
        }
        return this.css(dest);
    };

    $.toString = function () {
        return "jQuery " + $.prototype.jquery;
    };    
    
    var share;

    // share .isLoading
    (function (window) {

        var onloads = [];
        var state = 0;

        share = function () {
            var o = arguments;

            if ($.isNumber(o[0]) && $.isFunction(o[1]) && arguments.length == 2) {
                setTimeout(o[1], o[0]);
            }
            else if ($.isBoolean(o[0]) && o[0]) {
                chainingFunctions(1, o);
            }
            else if ($.isFunction(o[0]) || $.isNumber(o[0])) {
                if (state < 2) { onloads.push(o); }
                else if (o.length == 1) o[0]();
                else chainingFunctions(0, o);
            }
        };

        window.share = share;
        window.$$ = share;

        jQuery.fn.share = function () {
            var s = jQuery.data(this.get(0), "share");
            if (s != undefined) return s;
            else return null;
        };

        share.isLoading = function () {
            return (state < 2) ? true : false;
        };
        share.prototype = new Object();
        share.prototype.share = "Aphysoft Share 2.0 by Afis Herman Reza Devara";
        share.toString = function () {
            return "Aphysoft Share 2.0";
        };

        function chainingFunctions(iteration, functions) {
            if (iteration < 0 || functions == null) return;

            if (iteration < functions.length) {

                var returnValue;
                var iterationArgument = functions[iteration];

                if ($.isFunction(iterationArgument))
                    returnValue = functions[iteration]();
                else if ($.isNumber(iterationArgument))
                    returnValue = iterationArgument;

                if ($.isNumber(returnValue)) {
                    if (returnValue < 0) { // if returns negative value, then move backward
                        if ((iteration + returnValue) >= 0) {
                            chainingFunctions(iteration + returnValue, functions);
                        }
                    }
                    else if (returnValue > 0) { // if returns positive, then wait for specified value (in ms)
                        setTimeout(function () {
                            chainingFunctions(iteration + 1, functions);
                        }, returnValue);
                    }
                    else if (returnValue == 0) { // 0, retry
                        chainingFunctions(iteration, functions);
                    }
                }
                else if (returnValue == false) { }
                else chainingFunctions(iteration + 1, functions);
            }
        };

        $(function () {
            state = 2;
            $.each(onloads, function (i, v) {
                chainingFunctions(0, v);
            });
        });

    })(window);
    
    // .system .data .removeData .ready
    (function (share) {

        //share.prototype.data = [];
        var store = {};//share.prototype.data;

        var system = function () {
            var o = arguments;
            if ($.isString(o[0])) {
                var k = "____system____" + o[0];
                var d = store[k];
                if (!$.isUndefined(d)) {
                    delete store[k];

                    var stillContain = false;
                    $.each(store, function (i, v) {
                        if (i.startsWith("____system____")) {
                            stillContain = true;
                            return false;
                        }
                    });
                    if (stillContain == false) delete share.system;

                    return d;
                }
                else return null;
            }
        };
        var data = function () {
            var o = arguments;

            if ($.isPlainObject(o[0])) {
                $.each(o[0], function (i, v) { store[i] = v; });
            }
            else if ($.isString(o[0])) {
                if (o.length == 1) {
                    var d = store[o[0]];
                    return $.isUndefined(d) ? null : d;
                }
                else store[o[0]] = o[1];
            }
        };
        var removeData = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                if (!$.isUndefined(store[o[0]])) delete store[o[0]];
            }
        };
        var ready = function (c) {
            share(1, c);
        };

        share.system = system;
        share.data = data;
        share.removeData = removeData;
        share.ready = ready;

    })(share);

    // .client
    (function (share) {

        var clientID;

        var client = function () {
            return clientID;
        };

        share.client = client;

        $(function () {
            clientID = share.system("clientID");
        });

    })(share);
    
    // .unload .removeUnload
    (function (share) {
        // ps: non-null returned function will confirm user whether they want to stay on page or not

        var guid = 0;
        var handlers = {};

        var unload = function () {
            var o = arguments;
            var id, callback;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                if ($.isFunction(o[1])) {
                    if ($.isUndefined(handlers[o[0]])) {
                        id = o[0];
                    }
                    callback = o[1];
                }
            }
            else if ($.isFunction(o[0])) {
                guid = share.lookup(guid, handlers);
                id = guid;
                callback = o[0];
            }

            if (id != null && callback != null) {

                $.window.on("beforeunload." + guid + ".unload.share", function () {
                    var rv = callback();

                    return rv;
                });
                handlers[id] = callback;
                return id;
            }
        };
        var removeUnload = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                if (!$.isUndefined(handlers[id])) {
                    $.window.off("." + id + ".unload.share");
                    delete handlers[id];
                }
            }
            else if (Array.isArray(o[0])) {
                $.each(o[0], function (i, v) {
                    removeUnload(v);
                });
            }
        };

        share.unload = unload;
        share.removeUnload = removeUnload;

    })(share);

    // .random .identifier
    (function (share) {

        var lowerAlphabetic = "abcdefghijklmnopqrstuvwxyz";
        var upperAlphabetic = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var alphabetic = lowerAlphabetic + upperAlphabetic;
        var numeric = "0123456789";
        var alphanumeric = alphabetic + numeric;

        var random = function () {
            var o = arguments;

            if ($.isNumber(o[0])) {
                if ($.isNumber(o[1])) {
                    var min = o[0], max = o[1];
                    if (max >= min) return min + random(max - min);
                    else return min;
                }
                else {
                    var max = o[0];
                    if (max >= 0) return Math.floor(Math.random() * max);
                    else return random(max, 0);
                }
            }
            else if (Array.isArray(o[0])) {
                var a = o[0];
                return a.length > 0 ? a[random(a.length)] : null;
            }
            else return Math.random();
        };
        var identifier = function () {
            var o = arguments;

            if ($.isNumber(o[0])) {
                var len = o[0];
                if (len > 0) {
                    var tstr;
                    for (var i = 0; i < len; i++) {
                        if (i == 0) tstr = alphabetic.charAt(random(alphabetic.length));
                        else tstr += alphanumeric.charAt(random(alphanumeric.length));
                    }
                    return tstr;
                }
                else return "";
            }
        };

        share.random = random;
        share.identifier = identifier;

    })(share);

    // .encode .decode .urlEncode .urlDecode
    (function (share) {

        var base64Key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        var encode = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                var output = "", chr1, chr2, chr3, enc1, enc2, enc3, enc4, i = 0, input = utf8Encode(o[0]);
                while (i < input.length) {
                    chr1 = input.charCodeAt(i++); chr2 = input.charCodeAt(i++); chr3 = input.charCodeAt(i++);
                    enc1 = chr1 >> 2; enc2 = ((chr1 & 3) << 4) | (chr2 >> 4); enc3 = ((chr2 & 15) << 2) | (chr3 >> 6); enc4 = chr3 & 63;
                    if (isNaN(chr2)) enc3 = enc4 = 64;
                    else if (isNaN(chr3)) enc4 = 64;
                    output = output + base64Key.charAt(enc1) + base64Key.charAt(enc2) + base64Key.charAt(enc3) + base64Key.charAt(enc4);
                }
                return output;
            }
        };
        var decode = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                var output = "", chr1, chr2, chr3, enc1, enc2, enc3, enc4, i = 0, input = o[0].replace(/[^A-Za-z0-9\+\/\=]/g, "");
                while (i < input.length) {
                    enc1 = base64Key.indexOf(input.charAt(i++)); enc2 = base64Key.indexOf(input.charAt(i++)); enc3 = base64Key.indexOf(input.charAt(i++)); enc4 = base64Key.indexOf(input.charAt(i++));
                    chr1 = (enc1 << 2) | (enc2 >> 4); chr2 = ((enc2 & 15) << 4) | (enc3 >> 2); chr3 = ((enc3 & 3) << 6) | enc4;
                    output = output + String.fromCharCode(chr1);
                    if (enc3 != 64) output = output + String.fromCharCode(chr2);
                    if (enc4 != 64) output = output + String.fromCharCode(chr3);
                }
                return utf8Decode(output);
            }
        };
        var urlEncode = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                return escape(encode(o[0]));
            }
        }
        var urlDecode = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                return decode(unescape(o[0]));
            }
        }

        function utf8Encode(text) {
            var utftext = "";
            text = text.replace(/\r\n/g, "\n");
            for (var n = 0; n < text.length; n++) {
                var c = text.charCodeAt(n);
                if (c < 128) utftext += String.fromCharCode(c);
                else if ((c > 127) && (c < 2048)) { utftext += String.fromCharCode((c >> 6) | 192); utftext += String.fromCharCode((c & 63) | 128); }
                else { utftext += String.fromCharCode((c >> 12) | 224); utftext += String.fromCharCode(((c >> 6) & 63) | 128); utftext += String.fromCharCode((c & 63) | 128); }
            }
            return utftext;
        };
        function utf8Decode(utftext) {
            var string = "", i = 0, c = 0, c1 = 0, c2 = 0;
            while (i < utftext.length) {
                c = utftext.charCodeAt(i);
                if (c < 128) { string += String.fromCharCode(c); i++; }
                else if ((c > 191) && (c < 224)) { c2 = utftext.charCodeAt(i + 1); string += String.fromCharCode(((c & 31) << 6) | (c2 & 63)); i += 2; }
                else { c2 = utftext.charCodeAt(i + 1); c3 = utftext.charCodeAt(i + 2); string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63)); i += 3; }
            }
            return string;
        };

        share.encode = encode;
        share.decode = decode;
        share.urlEncode = urlEncode;
        share.urlDecode = urlDecode;

    })(share);

    // .lookup
    (function (share) {

        var lookup = function (id, array) {

            while (!$.isUndefined(array[id])) id++;
            return id;

        };

        share.lookup = lookup;

    })(share);

    // .title
    (function (share) {

        var titleFormat;
        var titleEmpty;

        var title = function () {
            var o = arguments;

            if ($.isString(o[0])) {

                if (o[0].length > 0) {
                    var t = titleFormat;
                    t = t.replace("{TITLE}", o[0]);
                    return t;
                }
                else return titleEmpty;
            }
            else return titleEmpty;
        };

        share.title = title;

        $(function () {
            titleFormat = share.system("titleFormat");
            titleEmpty = share.system("titleEmpty");
        });

    })(share);

    // .url .domain .protocol
    (function (share) {

        var baseDomain;
        var pageDomain;
        var protocol;

        var url = function () {
            var o = arguments;

            if ($.isUndefined(o[0]))
                return this.url(document.location.href);
            else if ($.isString(o[0])) {
                var s = o[0];

                var ourl = {
                    protocol: "",
                    domain: "",
                    path: "",
                    search: "",
                    hash: ""
                };

                // protocol://domain/path/to/resource?querystring#hash
                // 
                var sslsl, sd;
                if ((sslsl = s.indexOf("://")) > -1) {
                    ourl.protocol = s.substr(0, sslsl).toLowerCase();
                    sd = sslsl + 3;
                }
                else sd = 0;
                var sdsl;
                if ((sdsl = s.indexOf("/", sd)) > -1) {
                    ourl.domain = s.substring(sd, sdsl);
                }
                else ourl.domain = s.substring(sd);

                if (sdsl > -1) {

                    var spqm = s.indexOf("?", sdsl);
                    var sh = s.indexOf("#", sdsl);

                    if (spqm > -1 && sh > -1) {
                        if (sh < spqm) {
                            ourl.path = s.substring(sdsl, sh);
                            ourl.hash = s.substring(sh + 1);
                        }
                        else {
                            ourl.path = s.substring(sdsl, spqm);
                            ourl.search = s.substring(spqm + 1, sh);
                            ourl.hash = s.substring(sh + 1);
                        }
                    }
                    else if (spqm > -1) {
                        ourl.path = s.substring(sdsl, spqm);
                        ourl.search = s.substring(spqm + 1);
                    }
                    else if (sh > -1) {
                        ourl.path = s.substring(sdsl, sh);
                        ourl.hash = s.substring(sh + 1);
                    }
                    else ourl.path = s.substring(sdsl);
                }

                if (ourl.domain != "" && ourl.path == "") ourl.path = "/";

                if ($.isString(o[1])) {
                    var sraw = o[1];
                    sraw = sraw.replace("{protocol}", ourl.protocol);
                    sraw = sraw.replace("{protocol://}", ourl.protocol == "" ? "" : ourl.protocol + "://");
                    sraw = sraw.replace("{domain}", ourl.domain);
                    sraw = sraw.replace("{path}", ourl.path);
                    sraw = sraw.replace("{search}", ourl.search);
                    sraw = sraw.replace("{?search}", ourl.search == "" ? "" : "?" + ourl.search);
                    sraw = sraw.replace("{hash}", ourl.hash);
                    sraw = sraw.replace("{#hash}", ourl.hash == "" ? "" : "#" + ourl.hash);
                    return sraw;
                }
                else
                    return ourl;
            }
        };
        var domain = function () {
            var o = arguments;
            if ($.isString(o[0])) {
                if (o[0] == "base") return baseDomain;
                else if (o[0] == "page") return pageDomain;
            }
            else if (o.length == 0) return baseDomain;
        };
        var protocol = function () {
            return protocol;
        };

        share.url = url;
        share.domain = domain;
        share.protocol = protocol;

        $(function () {
            baseDomain = share.system("baseDomain");
            pageDomain = share.system("pageDomain");
            protocol = share.system("protocol");
        });

    })(share);
    
    // .date .timer .removeTimer .fromNow
    (function (share) {

        var guid = 0;
        var handlers = {};
        var timeDiff;
        var timeOutHandler = null;

        var relativeTimeLANGUS = {
            future: "in {0}",
            past: "{0} ago",
            s: "a few seconds",
            m: "a minute",
            mm: "{0} minutes",
            h: "an hour",
            hh: "{0} hours",
            d: "a day",
            dn: "yesterday",
            dp: "tomorrow",
            dd: "{0} days",
            M: "a month",
            Mn: "last month",
            Mp: "next month",
            MM: "{0} months",
            y: "a year",
            yn: "last year",
            yp: "next year",
            yy: "{0} years"
        };

        var date = function () {
            var o = arguments;

            if (o.length == 0) {
                if (timeDiff != null)
                    return new Date(new Date() - timeDiff);
                else
                    return new Date();
            }
            else {
                if ($.isString(o[0])) {
                    if (o[0].charAt(0) == '/') {
                        return new Date(parseInt(o[0].substr(6)));
                    }
                    else {
                        var dateObject, locale;
                        if ($.isDate(o[1])) {
                            if (!isNaN(o[1].getTime())) {
                                dateObject = o[1];
                                locale = "en-us";
                            }
                            else return "INVALID DATE";
                        }
                        else if ($.isString(o[1])) {
                            locale = o[1];
                            if ($.isDate(o[2])) dateObject = o[2];
                        }
                        else dateObject = date();

                        var oyear, omonth, odate, oday, ohour, oday, omin, osec, omil, otzdiff, otzdiffz, otzdiffh, otzdiffhm, otzdiffd;

                        oyear = dateObject.getFullYear();
                        omonth = dateObject.getMonth();
                        odate = dateObject.getDate();
                        oday = dateObject.getDay();
                        ohour = dateObject.getHours();
                        omin = dateObject.getMinutes();
                        osec = dateObject.getSeconds();
                        omil = dateObject.getMilliseconds();
                        otzdiff = dateObject.getTimezoneOffset();

                        otzdiffz = Math.abs(otzdiff);
                        otzdiffh = Math.floor(otzdiffz) / 60;
                        otzdiffhm = otzdiffz - otzdiffh * 60;
                        otzdiffd = zeroPadding(otzdiffh, 2) + "" + zeroPadding(otzdiffhm, 2);

                        var res = o[0];

                        res = res.replace("{YYYY}", oyear);
                        res = res.replace("{MMMM}", share.localization("monthLong", locale)[omonth]);
                        res = res.replace("{MMM}", share.localization("monthShort", locale)[omonth]);
                        res = res.replace("{MM}", zeroPadding((omonth + 1), 2));
                        res = res.replace("{M}", (omonth + 1));
                        res = res.replace("{DD}", zeroPadding(odate, 2));
                        res = res.replace("{D}", odate);
                        res = res.replace("{dddd}", share.localization("dayLong", locale)[oday]);
                        res = res.replace("{ddd}", share.localization("dayShort", locale)[oday]);
                        res = res.replace("{d}", share.localization("dayShort", locale)[oday].charAt(0));
                        res = res.replace("{HH}", zeroPadding(ohour, 2));
                        res = res.replace("{hh}", zeroPadding(ohour == 0 ? 12 : ohour <= 12 ? ohour : (ohour - 12), 2));
                        res = res.replace("{H}", ohour);
                        res = res.replace("{h}", ohour == 0 ? 12 : ohour <= 12 ? ohour : (ohour - 12));
                        res = res.replace("{mm}", zeroPadding(omin, 2));
                        res = res.replace("{m}", omin);
                        res = res.replace("{ss}", zeroPadding(osec, 2));
                        res = res.replace("{s}", osec);
                        res = res.replace("{sss}", omil);
                        res = res.replace("{A}", ohour <= 12 ? "AM" : "PM");
                        res = res.replace("{tz}", otzdiff < 0 ? "UTC+" + otzdiffd : "UTC-" + otzdiffd);

                        return res;

                    }
                }
            }
        };

        var timer = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                if ($.isFunction(o[1])) {
                    var id = o[0];
                    if ($.isUndefined(handlers[id])) {
                        handlers[id] = o[1];
                        if (timeOutHandler == null) ticks();
                        o[1]();
                        return id;
                    }
                }
                else if (o.length == 1) {
                    var id = o[0];
                    if (handlers[id] != null) {
                        handlers[id]();
                    }
                }
            }
            else if ($.isFunction(o[0])) {
                guid = share.lookup(guid, handlers);
                handlers[guid] = o[0];
                if (timeOutHandler == null) ticks();
                o[0]();
                return guid;
            }
        };
        var removeTimer = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                if (!$.isUndefined(handlers[id])) {
                    delete handlers[id];
                    if (handlers.length == 0) clearTimeout(timeOutHandler);
                }
            }
            else if (Array.isArray(o[0])) {
                $.each(o[0], function (i, v) {
                    removeTimer(v);
                });
            }
        };
        var fromNow = function (d) {

            var agespan;

            if ($.isDate(d)) {
                agespan = share.date() - d;
            }
            else return null;

            var lang = relativeTimeLANGUS;
            var mas;

            var p;
            if (agespan > 0) {
                mas = lang.past;
                p = -1;
            }
            else {
                mas = lang.future;
                p = 1;
            }

            var agespansec = Math.abs(agespan / 1000);

            var ret;

            if (agespansec < 30) ret = share.format(mas, share.format(lang.s, Math.round(agespansec)));
            else if (agespansec < 90) ret = share.format(mas, lang.m);
            else if (agespansec < 3600) ret = share.format(mas, share.format(lang.mm, Math.round(agespansec / 60)));
            else if (agespansec < 5400) ret = share.format(mas, lang.h);
            else if (agespansec < 84600) ret = share.format(mas, share.format(lang.hh, Math.round(agespansec / 3600)));
            else if (agespansec < 129600) ret = p == -1 ? lang.dn : lang.dp;
            else if (agespansec < 2547900) ret = share.format(mas, share.format(lang.dd, Math.round(agespansec / 86400)));
            else if (agespansec < 3888000) ret = p == -1 ? lang.Mn : lang.Mp;
            else if (agespansec < 29808000) ret = share.format(mas, share.format(lang.MM, Math.round(agespansec / 2592000)));
            else if (agespansec < 47304000) ret = p == -1 ? lang.yn : lang.yp;
            else ret = share.format(mas, share.format(lang.yy, Math.round(agespansec / 31536000)));

            return {
                description: ret,
                span: agespan
            };
        };
        
        function zeroPadding(c, len) {
            var cs = $.isString(c) ? c : c + "";
            return cs.paddingLeft(2, "0");
        };
        function ticks() {

            var now = share.date();
            var wait = 1000 - (now.valueOf() % 1000);

            $.each(handlers, function (i, v) { v(); });

            timeOutHandler = setTimeout(ticks, wait);
        }

        share.date = date;
        share.timer = timer;
        share.removeTimer = removeTimer;
        share.fromNow = fromNow;

        $(function () {
            timeDiff = (window.performance.timing.responseEnd - eval("new " + share.system("serverTime").slice(1, -1)));
        });

    })(share);
    
    // .get .post 
    (function (share) {

        var providerPath;

        var get = function () {
            var o = share.args(arguments, "number appid", "optional object data", "optional function callback");

            if (o) {
                return request("GET", o.appid, o.data, o.callback);
            }
        };
        var post = function () {
            var o = share.args(arguments, "number appid", "optional object data", "optional function callback");

            if (o) {
                return request("POST", o.appid, o.data, o.callback);
            }
        };

        function request(type, oappid, odata, ocallback) {

            var data;
            if ($.isPlainObject(odata)) data = odata;
            else data = {};

            var minTime = 0;

            if ($.isNumber(data.minimumTime)) {
                minTime = data.minimumTime;
                data.minimumTime = undefined;
            }

            data.i = oappid;
            data.c = share.client();

            var sw = share.stopwatch();

            var jqXHR = $.ajax({
                type: type, url: providerPath, data: data, dataType: 'json',
                success: function (data) {

                    var elapsed = sw.elapsed();

                    if (ocallback != null) {

                        if (minTime > 0) {
                            if (elapsed < minTime) {
                                setTimeout(function () {
                                    ocallback(data);
                                }, (minTime - elapsed));
                            }
                            else
                                ocallback(data);
                        }
                        else {
                            ocallback(data);
                        }

                    }
                }
            });

            return jqXHR;

        };

        share.get = get;
        share.post = post;

        $(function () {
            providerPath = share.system("providerPath");
        });

    })(share);

    // .param
    (function (share) {

        var param = function () {
            var o = share.args(arguments, "required string key", "optional string url");

            if (o) {
                var urlp;

                if (o.url != null) urlp = share.url(o.url);
                else urlp = share.url();

                var sp = {};

                $.each(urlp.search.split("&"), function (i, v) {
                    if (v.length > 0) {
                        var key, value = null;
                        var iss = v.indexOf("=");

                        if (iss > -1) {
                            key = v.substring(0, iss);
                            value = v.substring(iss + 1);
                        }
                        else key = v;

                        if ($.isUndefined(sp[key])) sp[key] = [value];
                        else sp[key].push(value);
                    }
                });

                var keySearch = o.key;
                var valueResult = sp[keySearch];

                return valueResult.length == 1 ? valueResult[0] : valueResult;
            }
        };

        share.param = param;

    })(share);

    // .localization
    (function (share) {

        var localization = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                if (o[0] == "monthLong")
                    return ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
                else if (o[0] == "dayLong")
                    return ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
                else if (o[0] == "monthShort")
                    return ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                else if (o[0] == "dayShort")
                    return ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
            }
        };

        share.localization = localization;

    })(share);

    // .scrollBarSize
    (function (share) {

        var size = null;

        var scrollBarSize = function () {

            if (size == null) {
                var inner = $('<p></p>').css({
                    'width': '100%',
                    'height': '100%'
                });
                var outer = $('<div></div>').css({
                    'position': 'absolute',
                    'width': '100px',
                    'height': '100px',
                    'top': '0',
                    'left': '0',
                    'visibility': 'hidden',
                    'overflow': 'hidden'
                }).append(inner);

                $(document.body).append(outer);

                var w1 = inner.width(), h1 = inner.height();
                outer.css('overflow', 'scroll');
                var w2 = inner.width(), h2 = inner.height();
                if (w1 == w2 && outer[0].clientWidth) {
                    w2 = outer[0].clientWidth;
                }
                if (h1 == h2 && outer[0].clientHeight) {
                    h2 = outer[0].clientHeight;
                }

                outer.remove();

                size = [(w1 - w2), (h1 - h2)];
            }

            return size;
        };

        share.scrollBarSize = scrollBarSize;

    })(share);

    // .stopwatch
    (function (share) {

        var stopwatch = function () {

            var d = new Date();

            var a = {
                elapsed: function () {
                    var e = new Date();
                    return (e - d);
                }
            };

            return a;
        };

        share.stopwatch = stopwatch;

    })(share);

    // .diff .for
    (function (share) {

        var diff = function (array1, array2) {
            if (Array.isArray(array1) && Array.isArray(array2)) {
                return array1.filter(function (i) { return !(array2.indexOf(i) > -1); });
            }
        };
        var _for = function (a, b, c) {

            var initValue = null, length = null, loop = null, condition = null;

            if ($.isNumber(a)) {
                if ($.isNumber(b) && $.isFunction(c)) {
                    initValue = a;
                    length = b;
                    loop = c;
                }
                else if ($.isFunction(b)) {
                    if ($.isUndefined(c)) {
                        initValue = 0;
                        length = a;
                        loop = b;
                    }
                    else if ($.isFunction(c)) {
                        initValue = a;
                        condition = b;
                        loop = c;
                    }
                }
            }

            if (initValue != null && loop != null) {
                if (length != null) {
                    var toValue = initValue + length;
                    var i = initValue;
                    for (; i < toValue; i++) {
                        var _return = loop(i);
                        if (_return == false) break;
                    }
                    return i;
                }
                else if (condition != null) {
                    var i = initValue;
                    for (; condition(i) ; i++) {
                        var _return = loop(i);
                        if (_return == false) break;
                    }
                    return i;
                }
            }
        };
        var _while = function (a, b) {
            if ($.isFunction(a)) {
                if ($.isFunction(b)) {
                    while (a()) {
                        var _return = b();
                        if (_return == false) break;
                    }
                }
                else if ($.isUndefined(b)) {
                    while (true) {
                        var _return = a();
                        if (_return == false) break;
                    }
                }
            }
        }

        share.diff = diff;
        share.for = _for;
        share.while = _while;

    })(share);

    // .format .dictionary
    (function (share) {

        var format = function () {
            var s = arguments[0];
            for (var i = 0; i < arguments.length - 1; i++) {
                var reg = new RegExp("\\{" + i + "\\}", "gm");
                s = s.replace(reg, arguments[i + 1]);
            }
            return s;
        };
        var dictionary = function () {
            var o = arguments;
            var s = o[0];

            var obj = {};

            if (Array.isArray(s)) {

                var vo = null;
                $.each(s, function (i, v) {
                    if (vo == null) {
                        vo = v;
                    }
                    else {
                        obj[vo] = v;
                        vo = null;
                    }
                });

            }

            return obj;
        };
        var thousand = function (x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        };

        share.format = format;
        share.dictionary = dictionary;
        share.thousand = thousand;

    })(share);

    // .reduceWord .reduceLetter
    (function (share) {

        var reduceWord = function (s, by) {
            var ss = s.split(" ");
            var ssl = ss.length;

            if (ssl > by) {
                var sos = [];
                for (var i = 0; i < (ssl - by) ; i++) {
                    sos.push(ss[i]);
                }
                return sos.join(" ");
            }
            else return "";
        };
        var reduceLetter = function (s, by) {
            return s.substr(0, s.length - by);
        };

        share.reduceWord = reduceWord;
        share.reduceLetter = reduceLetter;

    })(share);

    // .css
    (function (share) {

        var css = function () {
            var o = arguments;

            if ($.isJQuery(o[0])) {
                var s = o[0].attr("style");
                var po = {};
                if (s != null) {
                    var ss = s.split(";");
                    $.each(ss, function (i, v) {
                        v = v.trim();
                        if (v.length > 0) {
                            var vs = v.split(":");
                            var vk = vs[0].trim();
                            var vv = vs[1].trim();
                            po[vk] = vv;
                        }
                    });
                }
                return po;
            }
        }
        share.css = css;

    })(share);

    // .color
    (function (share) {

        var colors = {
            "aliceblue": "#f0f8ff", "antiquewhite": "#faebd7", "aqua": "#00ffff", "aquamarine": "#7fffd4", "azure": "#f0ffff",
            "beige": "#f5f5dc", "bisque": "#ffe4c4", "black": "#000000", "blanchedalmond": "#ffebcd", "blue": "#0000ff", "blueviolet": "#8a2be2", "brown": "#a52a2a", "burlywood": "#deb887",
            "cadetblue": "#5f9ea0", "chartreuse": "#7fff00", "chocolate": "#d2691e", "coral": "#ff7f50", "cornflowerblue": "#6495ed", "cornsilk": "#fff8dc", "crimson": "#dc143c", "cyan": "#00ffff",
            "darkblue": "#00008b", "darkcyan": "#008b8b", "darkgoldenrod": "#b8860b", "darkgray": "#a9a9a9", "darkgreen": "#006400", "darkkhaki": "#bdb76b", "darkmagenta": "#8b008b", "darkolivegreen": "#556b2f",
            "darkorange": "#ff8c00", "darkorchid": "#9932cc", "darkred": "#8b0000", "darksalmon": "#e9967a", "darkseagreen": "#8fbc8f", "darkslateblue": "#483d8b", "darkslategray": "#2f4f4f", "darkturquoise": "#00ced1",
            "darkviolet": "#9400d3", "deeppink": "#ff1493", "deepskyblue": "#00bfff", "dimgray": "#696969", "dodgerblue": "#1e90ff",
            "firebrick": "#b22222", "floralwhite": "#fffaf0", "forestgreen": "#228b22", "fuchsia": "#ff00ff",
            "gainsboro": "#dcdcdc", "ghostwhite": "#f8f8ff", "gold": "#ffd700", "goldenrod": "#daa520", "gray": "#808080", "green": "#008000", "greenyellow": "#adff2f",
            "honeydew": "#f0fff0", "hotpink": "#ff69b4",
            "indianred ": "#cd5c5c", "indigo ": "#4b0082", "ivory": "#fffff0", "khaki": "#f0e68c",
            "lavender": "#e6e6fa", "lavenderblush": "#fff0f5", "lawngreen": "#7cfc00", "lemonchiffon": "#fffacd", "lightblue": "#add8e6", "lightcoral": "#f08080", "lightcyan": "#e0ffff", "lightgoldenrodyellow": "#fafad2",
            "lightgrey": "#d3d3d3", "lightgreen": "#90ee90", "lightpink": "#ffb6c1", "lightsalmon": "#ffa07a", "lightseagreen": "#20b2aa", "lightskyblue": "#87cefa", "lightslategray": "#778899", "lightsteelblue": "#b0c4de",
            "lightyellow": "#ffffe0", "lime": "#00ff00", "limegreen": "#32cd32", "linen": "#faf0e6",
            "magenta": "#ff00ff", "maroon": "#800000", "mediumaquamarine": "#66cdaa", "mediumblue": "#0000cd", "mediumorchid": "#ba55d3", "mediumpurple": "#9370d8", "mediumseagreen": "#3cb371", "mediumslateblue": "#7b68ee",
            "mediumspringgreen": "#00fa9a", "mediumturquoise": "#48d1cc", "mediumvioletred": "#c71585", "midnightblue": "#191970", "mintcream": "#f5fffa", "mistyrose": "#ffe4e1", "moccasin": "#ffe4b5",
            "navajowhite": "#ffdead", "navy": "#000080",
            "oldlace": "#fdf5e6", "olive": "#808000", "olivedrab": "#6b8e23", "orange": "#ffa500", "orangered": "#ff4500", "orchid": "#da70d6",
            "palegoldenrod": "#eee8aa", "palegreen": "#98fb98", "paleturquoise": "#afeeee", "palevioletred": "#d87093", "papayawhip": "#ffefd5", "peachpuff": "#ffdab9", "peru": "#cd853f", "pink": "#ffc0cb", "plum": "#dda0dd", "powderblue": "#b0e0e6", "purple": "#800080",
            "red": "#ff0000", "rosybrown": "#bc8f8f", "royalblue": "#4169e1",
            "saddlebrown": "#8b4513", "salmon": "#fa8072", "sandybrown": "#f4a460", "seagreen": "#2e8b57", "seashell": "#fff5ee", "sienna": "#a0522d", "silver": "#c0c0c0", "skyblue": "#87ceeb", "slateblue": "#6a5acd", "slategray": "#708090", "snow": "#fffafa", "springgreen": "#00ff7f", "steelblue": "#4682b4",
            "tan": "#d2b48c", "teal": "#008080", "thistle": "#d8bfd8", "tomato": "#ff6347", "turquoise": "#40e0d0",
            "violet": "#ee82ee",
            "wheat": "#f5deb3", "white": "#ffffff", "whitesmoke": "#f5f5f5",
            "yellow": "#ffff00", "yellowgreen": "#9acd32"
        };
        var colorAccent, colorMin, colorMax;

        share.color = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                var hex = null, com;
                var sss = o[0].toLowerCase().trim();
                var ss = sss.split(/\+|\-/, 2);
                var s = ss[0];
                var nn = ss.length > 1 ? parseInt(ss[1]) : null;
                var n = isNaN(nn) ? null : sss.indexOf('-') > -1 ? -1 * (nn > 100 ? 100 : nn < 0 ? 0 : nn) : (nn > 100 ? 100 : nn < 0 ? 0 : nn);

                if (colors[s] != null) hex = colors[s];
                else if (s == "transparent") hex = "rgba(0, 0, 0, 0)"
                else if (s == "accent") hex = colorAccent;
                else if (s.length == 3 && $.isColor("#" + s)) hex = "#" + s;
                else if (s.length == 4 && s[0] == "#" && $.isColor(s)) hex = s;
                else if (s.length == 6 && $.isColor("#" + s)) hex = "#" + s;
                else if (s.length == 7 && s[0] == "#" && $.isColor(s)) hex = s;
                else if (com = s.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/)) hex = "#" + dec2hex(com[1]) + dec2hex(com[2]) + dec2hex(com[3]);

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



                if (hex != null) {
                    if ($.isNumber(o[1])) {
                        var r = 0, g = 0, b = 0;

                        if (hex.length == 4) {
                            r = hex2dec(hex[1]);
                            g = hex2dec(hex[2]);
                            b = hex2dec(hex[3]);
                        }
                        else {
                            r = hex2dec(hex.substr(1, 2));
                            g = hex2dec(hex.substr(3, 2));
                            b = hex2dec(hex.substr(5, 2));
                        }

                        var a = o[1] < 0 ? 0 : o[1] > 1 ? 1 : o[1];
                        return "rgba(" + r + "," + g + "," + b + "," + a + ")";
                    }
                    else {
                        return hex;
                    }
                }
                else return null;
            }
            else if ($.isNumber(o[0])) {
                if ($.isNumber(o[1])) {
                    if ($.isNumber(o[2])) {
                        if ($.isNumber(o[3])) {
                            var r, g, b, a;
                            r = o[0] < 0 ? 0 : o[0] > 255 ? 255 : Math.round(o[0]);
                            g = o[1] < 0 ? 0 : o[1] > 255 ? 255 : Math.round(o[1]);
                            b = o[2] < 0 ? 0 : o[2] > 255 ? 255 : Math.round(o[2]);
                            a = o[3] < 0 ? 0 : o[3] > 1 ? 1 : o[3];
                            return "rgba(" + r + "," + g + "," + b + "," + a + ")";
                        }
                        else {
                            return "#" + dec2hex(o[0]) + dec2hex(o[1]) + dec2hex(o[2]);
                        }
                    }
                    else {
                        var n = o[0];
                        var minr = parseInt(colorMin.substr(1, 2), 16);
                        var ming = parseInt(colorMin.substr(3, 2), 16);
                        var minb = parseInt(colorMin.substr(5, 2), 16);
                        var cr = Math.floor(minr + (((parseInt(colorMax.substr(1, 2), 16) - minr) / 100) * n));
                        var cg = Math.floor(ming + (((parseInt(colorMax.substr(3, 2), 16) - ming) / 100) * n));
                        var cb = Math.floor(minb + (((parseInt(colorMax.substr(5, 2), 16) - minb) / 100) * n));
                        cr = cr < 0 ? 0 : cr > 255 ? 255 : cr;
                        cg = cg < 0 ? 0 : cg > 255 ? 255 : cg;
                        cb = cb < 0 ? 0 : cb > 255 ? 255 : cb;
                        return share.color("#" + cr.toString(16).paddingLeft(2, 0) + cg.toString(16).paddingLeft(2, 0) + cb.toString(16).paddingLeft(2, 0), o[1]);
                    }
                }
                else {
                    var n = o[0];
                    var minr = parseInt(colorMin.substr(1, 2), 16);
                    var ming = parseInt(colorMin.substr(3, 2), 16);
                    var minb = parseInt(colorMin.substr(5, 2), 16);
                    var cr = Math.floor(minr + (((parseInt(colorMax.substr(1, 2), 16) - minr) / 100) * n));
                    var cg = Math.floor(ming + (((parseInt(colorMax.substr(3, 2), 16) - ming) / 100) * n));
                    var cb = Math.floor(minb + (((parseInt(colorMax.substr(5, 2), 16) - minb) / 100) * n));
                    cr = cr < 0 ? 0 : cr > 255 ? 255 : cr;
                    cg = cg < 0 ? 0 : cg > 255 ? 255 : cg;
                    cb = cb < 0 ? 0 : cb > 255 ? 255 : cb;
                    return share.color("#" + cr.toString(16).paddingLeft(2, 0) + cg.toString(16).paddingLeft(2, 0) + cb.toString(16).paddingLeft(2, 0), o[1]);
                }
            }
        };

        function dec2hex(dec) {
            if ($.isNumeric(dec)) {
                if (dec >= 0 && dec <= 255)
                    return ("0" + parseInt(dec).toString(16)).slice(-2);
            }
            return "00";
        };
        function hex2dec(hex) {
            if ($.isString(hex)) {
                if (hex.length == 2)
                    return parseInt(hex, 16);
                else
                    return parseInt(hex, 16) * 17;
            }
        };

        share(function () {
            
            colorAccent = share.system("colorAccent");
            colorMin = share.system("color0");
            colorMax = share.system("color100");

            
        });

    })(share);

    // .resize .removeResize .isFullScreen .pageWidth .pageHeight
    (function (share) {

        var guid = 0;
        var handlers = {};

        var _isFullScreen = false;
        var width = 0;
        var height = 0;
        var lastWidth = 0;
        var lastHeight = 0;

        var resize = function () {
            var o = arguments;

            if ($.isJQuery(o[0])) {
                return share.event(o[0], "resize", o[1], o[2]);
            }
            else if ($.isNumber(o[0]) || $.isString(o[0])) {
                if ($.isFunction(o[1])) {
                    var id = o[0];
                    if ($.isUndefined(handlers[id])) {
                        handlers[id] = o[1];
                        var ww = $.window.width();
                        var wh = $.window.height();
                        o[1]({ width: ww, height: wh, lastWidth: ww, lastHeight: wh, widthChanged: false, heightChanged: false });
                        return id;
                    }
                }
            }
            else if ($.isFunction(o[0])) {
                guid = share.lookup(guid, handlers);
                handlers[guid] = o[0];
                var ww = $.window.width();
                var wh = $.window.height();
                o[0]({ width: ww, height: wh, lastWidth: ww, lastHeight: wh, widthChanged: false, heightChanged: false });

                return guid;
            }
        };
        var triggerResize = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];

                var ww = $.window.width();
                var wh = $.window.height();

                if (o.length == 1) {
                    if (handlers[id] != null) handlers[id]({ width: ww, height: wh, lastWidth: ww, lastHeight: wh, widthChanged: false, heightChanged: false });
                }
                else {
                    if (handlers[id] != null) handlers[id](o[1], { width: ww, height: wh, lastWidth: ww, lastHeight: wh, widthChanged: false, heightChanged: false });
                }
            }

        };
        var removeResize = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                if (!$.isUndefined(handlers[id])) {
                    delete handlers[id];
                }
            }
            else if (Array.isArray(o[0])) {
                $.each(o[0], function (i, v) {
                    removeResize(v);
                });
            }
        };
        var isFullScreen = function () {
            return _isFullScreen;
        };
        var pageWidth = function () {
            return width;
        };
        var pageHeight = function () {
            return height;
        };

        function refresh() {
            lastWidth = width;
            lastHeight = height;
            width = $.window.width();
            height = $.window.height();
            _isFullScreen = width == screen.width && height >= (screen.height - 1/*firefox 1px bug*/);

            return {
                width: width,
                height: height,
                lastWidth: lastWidth,
                lastHeight: lastHeight,
                widthChanged: width != lastWidth,
                heightChanged: height != lastHeight
            };
        };

        share.resize = resize;
        share.triggerResize = triggerResize;
        share.removeResize = removeResize;
        share.isFullScreen = isFullScreen;
        share.pageWidth = pageWidth;
        share.pageHeight = pageHeight;

        $.window.resize(function (a) {
            var obj = refresh();
            $.each(handlers, function (bi, bv) {
                bv(obj);
            });
        });

        $(function () {
            refresh();
        });

    })(share);

    // .repeat .removeRepeat
    (function (share) {

        var guid = 0;
        var handlers = {};
        var optionsDefaults = {
            initialTrigger: false
        };

        var repeat = function () {
            var o = share.args(arguments, "optional number delay", "optional number rate", "optional object options", "function callback");

            if (o) {
                var delay = 400, rate = 40;
                var options;

                if ($.isPlainObject(o.options))
                    options = $.extend({}, optionsDefaults, o.options);
                else
                    options = $.extend({}, optionsDefaults);

                if (o.delay != null && o.delay > 0) delay = o.delay;
                if (o.rate != null && o.rate > 0) rate = o.rate;

                var obj = { timer: null };

                var repeater = function () {
                    o.callback({
                        rate: rate
                    });
                    obj.timer = setTimeout(repeater, rate);
                };
                if (options.initialTrigger) o.callback({
                    rate: delay
                });
                obj.timer = setTimeout(repeater, delay);

                guid = share.lookup(guid, handlers);
                handlers[guid] = obj;

                return guid;
            }
        };
        var removeRepeat = function () {
            var o = arguments;

            if ($.isNumber(o[0])) {
                var id = o[0];
                if (!$.isUndefined(handlers[id])) {
                    var handler = handlers[id];
                    clearTimeout(handler.timer);
                    delete handlers[id];
                }
            }
        };

        share.repeat = repeat;
        share.removeRepeat = removeRepeat;

    })(share);

    // .once
    (function (share) {

        var handlers = [];

        var once = function (id) {
            if (handlers.indexOf(id) == -1) {
                handlers.push(id);
                return false;
            }
            else return true;
        };
        var removeOnce = function (id) {
            var ix = handlers.indexOf(id);
            if (ix > -1) {
                handlers.splice(ix, 1);
            }
        };

        share.once = once;
        share.removeOnce = removeOnce;

    })(share);

    // .deltaperfdom
    (function (share) {

        var deltaperfdom = function () {
            if (Modernizr.performance)
                return performance.timing.domComplete - performance.timing.domLoading;
            else
                return 200;
        };

        share.deltaperfdom = deltaperfdom;

    })(share);

    // .args
    (function (share) {

        var args = function () {
            var args = arguments;

            if (!$.isUndefined(args[0])) {

                var o = {};
                var margs = args[0];

                for (var i = 0; i < margs.length; i++) {
                    o[i] = margs[i];
                }

                if (args.length > 1) {

                    var j = 1;

                    if ($.isNumber(args[1])) {
                        var ra = args[1];
                        if (ra > margs.length) return null;
                        j = 2;
                    }

                    for (var i = 0; i < margs.length; i++) {

                        var marg = margs[i];
                        var mismatch = true;

                        if ($.isUndefined(marg)) break;

                        while (j < args.length) {

                            var optional = false, type, key, required = false;

                            var targkv = args[j++].split(" ");

                            if (targkv[0] == "optional") {
                                optional = true;
                                type = targkv[1];
                                key = targkv[2];
                            }
                            else if (targkv[0] == "required") {
                                required = true;
                                type = targkv[1];
                                key = targkv[2];
                            }
                            else {
                                type = targkv[0];
                                key = targkv[1];
                            }
                            var tetype = $.type(marg);

                            if (tetype == "object") {
                                if ($.isJQuery(marg)) tetype = "jquery";
                            }

                            if (tetype == "null" && required == false) {
                                o[key] = null;
                                mismatch = false;
                            }
                            else {
                                if (type.indexOf("/") > -1) {
                                    var typealts = type.split("/");
                                    var typealtb = false;

                                    for (var typealti = 0; typealti < typealts.length; typealti++) {
                                        if (tetype == typealts[typealti]) {
                                            mismatch = false;
                                            o[key] = marg;
                                            typealtb = true;
                                            break;
                                        }
                                    }

                                    if (typealtb == false) {
                                        if (optional) {
                                            o[key] = null;
                                            continue;
                                        }
                                    }
                                }
                                else if (type.indexOf("<") > -1) {
                                    var typeconvs = type.split("<");
                                    var typeconvto = typeconvs[0];
                                    var typeconvb = false;

                                    if (tetype == typeconvto) {
                                        o[key] = marg;
                                        mismatch = false;
                                        typeconvb = true;
                                    }
                                    else {
                                        for (var typeconvi = 1; typeconvi < typeconvs.length; typeconvi++) {
                                            if (tetype == typeconvs[typeconvi]) {
                                                mismatch = false;

                                                if (typeconvto == "string") {
                                                    o[key] = marg + "";
                                                }
                                                else if (typeconvto == "number") {
                                                    if (tetype == "string") o[key] = parseInt(marg);
                                                    else if (tetype == "boolean") o[key] = marg ? 1 : 0;
                                                    else o[key] = null;
                                                }
                                                else if (typeconvto == "boolean") {
                                                    if (tetype == "string") o[key] = marg == "true" ? true : marg == "false" ? false : "";
                                                    else if (tytype == "number") o[key] = marg >= 1 ? true : false;
                                                }

                                                typeconvb = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (typeconvb == false) {
                                        if (optional)
                                            continue;
                                    }
                                }
                                else {
                                    if (tetype == type) {
                                        o[key] = marg;
                                        mismatch = false;
                                    }
                                    else if (optional) {
                                        o[key] = null;
                                        continue;
                                    }
                                }
                            }

                            break;
                        }

                        if (mismatch) return null;
                    }

                    for (; j < args.length; j++) {
                        var targkv = args[j].split(" ");
                        if (targkv[0] != "optional") return null;
                        else o[targkv[2]] = null;
                    }
                }

                return o;
            }
            else return null;
        };

        share.args = args;

    })(share);

    // .cookie .removeCookie
    (function (share) {

        var cookie = function () {
            var o = share.args(arguments, "string key", "optional string<number<boolean value", "optional string path", "optional date expires");

            if (o) {

                var dc = document.cookie;
                var cs = dc.split(";");
                var c = {};

                $.each(cs, function (i, v) {
                    var iv = v.trim();
                    var ivfe = iv.indexOf("=");
                    var key = iv.substr(0, ivfe);
                    var value = iv.substr(ivfe + 1);

                    if (c[key] != null) {
                        if (!Array.isArray(c[key])) c[key] = [c[key]];
                        c[key].push(value);
                    }
                    else c[key] = value;
                });

                if (o.value == null) {
                    return $.isUndefined(c[o.key]) ? null : c[o.key];
                }
                else {
                    var sdom = share.domain();
                    var s = o.key + "=" + o.value + (sdom != null ? "; domain=" + sdom : "");

                    if (o.path != null)
                        s += "; path=" + o.path;
                    if (o.expires != null) {
                        s += "; expires= " + share.date("{ddd}, {DD}-{MMM}-{YYYY} {HH}:{mm}:{ss} {tz}", o.expires);
                    }

                    //alert(s);

                    document.cookie = s;
                }
            }
        };
        var removeCookie = function (key, a, b) {
            if ($.isString(key)) {
                var sdom = null;
                var spath = null;
                if ($.isString(b)) {
                    if ($.isString(a)) {
                        spath = b;
                        sdom = a;
                    }
                }
                else if ($.isString(a)) {
                    sdom = a;
                }

                document.cookie = key + "=deleted; expires=Thu, 01-Jan-1970 00:00:01 GMT" + (sdom != null ? "; domain=" + sdom : "") + (spath != null ? "; path=" + spath : "");
            }
        };

        share.cookie = cookie;
        share.removeCookie = removeCookie;

    })(share);

    // .event .removeEvent .triggerEvent .disableEvent .enableEvent
    // .down .up .move .click .enter .leave .wheel .keydown .keyup .keypress .change .scroll .resize
    (function (share) {

        var guid = 0;
        var handlers = {};
        var disabledHandlers = [];

        var event = function () {

            var o = share.args(arguments, "optional jquery object", "string type", "optional string/number id", "function callback", "optional boolean once", "optional function caller");

            if (o) {
                if (o.object != null) {
                    if (o.type == "down" || o.type == "up" || o.type == "move" || o.type == "click" ||
                        o.type == "enter" || o.type == "leave" || o.type == "over" || o.type == "out" ||
                        o.type == "wheel" || o.type == "scroll" ||
                        o.type == "keydown" || o.type == "keyup" || o.type == "keypress" || o.type == "input" ||
                        o.type == "change" || o.type == "resize" ||
                        o.type == "focusin" || o.type == "focusout" ||
                        o.type == "position"
                        ) {

                        var so = o.object;
                        var id = null;
                        var newHandle;

                        if (o.id != null) {
                            if (disabledHandlers.indexOf(o.id) > -1) { // we're going to enable again
                                newHandle = false;
                                id = o.id;
                            }
                            else if ($.isUndefined(handlers[o.id]) || $.isNull(handlers[o.id])) { // new handler with specified id
                                newHandle = true;
                                id = o.id;
                            }
                            else {
                                newHandle = true;
                                id = null;
                            }
                        }
                        else newHandle = true;

                        if (id == null && newHandle) {
                            guid = share.lookup(guid, handlers);
                            id = guid;
                        }

                        var handle = { type: o.type, object: so, tag: null, once: o.once, callback: o.callback };

                        if (newHandle) {
                            handlers[id] = handle;
                        }

                        if (o.type == "down") {
                            if (Modernizr.touchevents) {
                                so.on("touchstart." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(true, event, 1);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                            else {
                                so.on("mousedown." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(false, event, 1);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                        }
                        else if (o.type == "up") {
                            if (Modernizr.touchevents) {
                                so.on("touchend." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(true, event, 2);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                            else {
                                so.on("mouseup." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(false, event, 2);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                        }
                        else if (o.type == "move") {
                            if (Modernizr.touchevents) {
                                so.on("touchmove." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(true, event, 3);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                            else {
                                so.on("mousemove." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(false, event, 3);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                        }
                        else if (o.type == "click") {
                            if (Modernizr.touchevents) {
                                so.on("tap." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(true, event, 7);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                            else {
                                so.on("click." + id + ".event.share", function (event) {
                                    var pev = pointingEvent(false, event, 7);
                                    if (o.once) removeEvent(id);
                                    var ev = o.callback.call(o.caller, pev);
                                    return ev;
                                });
                            }
                        }
                        else if (o.type == "enter") {
                            so.on("mouseenter." + id + ".event.share", function (event) {
                                var pev = pointingEvent(false, event, 4);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "leave") {
                            so.on("mouseleave." + id + ".event.share", function (event) {
                                var pev = pointingEvent(false, event, 5);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "over") {
                            so.on("mouseover." + id + ".event.share", function (event) {
                                var pev = pointingEvent(false, event, 8);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "out") {
                            so.on("mouseout." + id + ".event.share", function (event) {
                                var pev = pointingEvent(false, event, 9);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "wheel") {
                            so.on("mousewheel." + id + ".event.share", function (event, delta, deltaX, deltaY) {
                                var pev = pointingEvent(false, event, 6, delta, deltaX, deltaY);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "scroll") {
                            so.on("scroll." + id + ".event.share", function (event) {
                                var pev = scrollEvent(event);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "keydown") {
                            so.on("keydown." + id + ".event.share", function (event) {
                                var pev = keyboardEvent(event, 1);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "keyup") {
                            so.on("keyup." + id + ".event.share", function (event) {
                                var pev = keyboardEvent(event, 2);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "keypress") {
                            so.on("keypress." + id + ".event.share", function (event) {
                                var pev = keyboardEvent(event, 3);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "input") {
                            so.on("input." + id + ".event.share", function (event) {
                                var pev = keyboardEvent(event, 4);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "change") {
                            so.on("change." + id + ".event.share", function (event) {
                                var pev = formEvent(event, 1);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "focusin") {
                            so.on("focusin." + id + ".event.share", function (event) {
                                var pev = focusEvent(event, 1);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "focusout") {
                            so.on("focusout." + id + ".event.share", function (event) {
                                var pev = focusEvent(event, 2);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                return ev;
                            });
                        }
                        else if (o.type == "resize") {
                            $.each(so, function (i, v) {
                                var obj = $(v);
                                var dom = obj.get(0);
                                var data = obj.data("share.event.resize.data");

                                if (data == undefined) {
                                    var lw = parseInt(window.getComputedStyle(dom, null)["width"]);
                                    var lh = parseInt(window.getComputedStyle(dom, null)["height"]);
                                    data = {
                                        width: isNaN(lw) ? -1 : lw,
                                        height: isNaN(lh) ? -1 : lh,
                                        instance: 1,
                                        window: null,
                                        observer: null,
                                        targets: []
                                    };
                                    obj.data("share.event.resize.data", data);

                                    resizeTraverse(obj, obj);
                                }
                                else {
                                    data.instance = data.instance + 1;
                                }
                            });
                            so.on("_resize." + id + ".event.share", function (event, width, height, lastWidth, lastHeight) {
                                var pev = resizeEvent(event, width, height, lastWidth, lastHeight);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                event.stopPropagation();
                                return ev;
                            });
                        }
                        else if (o.type == "position") {
                            $.each(so, function (i, v) {
                                var obj = $(v);
                                var dom = obj.get(0);
                                var data = obj.data("share.event.position.data");

                                if (data == undefined) {
                                    var ll = parseInt(window.getComputedStyle(dom, null)["left"]);
                                    var lt = parseInt(window.getComputedStyle(dom, null)["top"]);
                                    data = {
                                        left: isNaN(ll) ? 0 : ll,
                                        top: isNaN(lt) ? 0 : lt,
                                        instance: 1,
                                        observer: new MutationObserver(function (mutations) {
                                            var ol = data.left;
                                            var ot = data.top;
                                            var cl = parseInt(window.getComputedStyle(dom, null)["left"]);
                                            var ct = parseInt(window.getComputedStyle(dom, null)["top"]);
                                            if (isNaN(cl)) cl = 0;
                                            if (isNaN(ct)) ct = 0;
                                            data.left = cl;
                                            data.top = ct;
                                            if (ol != cl || ot != ct) {
                                                obj.trigger("_position", [cl, ct, ol, ot]);
                                            }
                                        }),
                                    };
                                    obj.data("share.event.position.data", data);
                                }
                                else {
                                    data.instance = data.instance + 1;
                                }
                                data.observer.observe(dom, { attributes: true });
                            });
                            so.on("_position." + id + ".event.share", function (event, left, top, lastLeft, lastTop) {
                                var pev = positionEvent(event, left, top, lastLeft, lastTop);
                                if (o.once) removeEvent(id);
                                var ev = o.callback.call(o.caller, pev);
                                event.stopPropagation();
                                return ev;
                            });
                        }

                        return id;
                    }
                }
                else {
                    if (o.type == "change" || o.type == "resize") // o.object required
                        return null;
                    else
                        return event($.window, o.type, o.id, o.callback, o.once);
                }
            }
        };

        function resizeTraverse(cobj, oobj) {

            var cdom = cobj.get(0);
            var data = oobj.data("share.event.resize.data");

            data.targets.push(cdom);

            if (sizeChangedParentResized(cdom)) {
                resizeTraverse(cobj.parent(), oobj);
            }
            else {
                var observerOptions = { attributes: true, childList: true, characterData: true, subtree: true };

                if (data.observer == null) {
                    data.observer = new MutationObserver(function (mutations) {
                        var trigger = false;
                        var lid = data.targets.length;
                        var xid = [];

                        $.each(mutations, function (i, mutation) {
                            if (mutation.type == "characterData"
                                || mutation.type == "childList"
                                || mutation.type == "attributes"
                                ) trigger = true;
                            var did = data.targets.indexOf(mutation.target);
                            if (did > -1 && xid.indexOf(did) == -1) xid.push(did);
                        });

                        xid.sort(function (a, b) { return a - b; });

                        $.each(xid, function (i, did) {
                            trigger = true;

                            var mdom = data.targets[did];
                            var mobj = $(mdom);

                            if (!sizeChangedParentResized(mdom)) {
                                if (did < lid - 1) {
                                    data.observer.disconnect();
                                    data.targets.splice(did + 1, Number.MAX_VALUE);
                                    data.observer.observe(mdom, observerOptions);
                                    if (data.window != null) {
                                        share.removeResize(data.window);
                                    }
                                    return false;
                                }
                            }
                            else {
                                if (did == lid - 1) {
                                    data.observer.disconnect();
                                    resizeTraverse(mobj.parent(), oobj);
                                    return false;
                                }
                            }
                        });

                        if (trigger) resizeTrigger(oobj);
                    });
                }
                data.observer.observe(cdom, observerOptions);

                if (cobj.hasClass("_PG")) {
                    if (data.window != null) {
                        share.removeResize(data.window);
                    }
                    data.window = share.resize(function (v) {
                        if (v != null) resizeTrigger(oobj);
                    });
                }
            }

        };
        function resizeTrigger(obj) {
            var dom = obj.get(0);
            var data = obj.data("share.event.resize.data");

            if (data != undefined && obj.is(":visible")) {
                var lw = data.width;
                var lh = data.height;
                var cw = parseInt(window.getComputedStyle(dom, null)["width"]);
                var ch = parseInt(window.getComputedStyle(dom, null)["height"]);
                if (cw < 0 || isNaN(cw)) cw = 0;
                if (ch < 0 || isNaN(ch)) ch = 0;
                data.width = cw;
                data.height = ch;
                if (lw != cw || lh != ch) {
                    obj.trigger("_resize", [cw, ch, lw, lh]);
                }
            }
        };
        function sizeChangedParentResized(dom) {

            if (dom.className == "_PG") return false;

            //debug(dom.className);

            var po = (dom.style.position + "").toLowerCase();
            var pw = (dom.style.width + "");
            var ph = (dom.style.height + "");
            var pt = (dom.style.top + "");
            var pb = (dom.style.bottom + "");
            var pl = (dom.style.left + "");
            var pr = (dom.style.right + "");

            if (
                (pw.indexOf("%") > -1) || (ph.indexOf("%") > -1) ||
                (pr != "" && pw == "") || (pb != "" && ph == "") ||
                //(pt != "" && pb != "" && ph == "") || (pl != "" && pr != "" && pw == "") ||
                (po == "relative" && pw == "")
               ) {
                return true;
            }
            else {
                return false;
            }
        };

        var deleteEvent = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                var handle = handlers[id];

                if (handle.type == "resize") {
                    $.each(handle.object, function (i, v) {
                        var obj = $(v);
                        var data = obj.data("share.event.resize.data");

                        if (data != undefined) {
                            var instance = data.instance;

                            if (instance > 0) instance--;

                            if (instance == 0) {
                                if (data.window != null)
                                    share.removeResize(data.window);
                                if (data.observer != null) {
                                    data.observer.disconnect();
                                    data.observer = null;
                                }
                                data = null;
                                obj.removeData("share.event.resize.data");
                            }

                        }
                    });
                }
                else if (handle.type == "position") {
                    $.each(handle.object, function (i, v) {
                        var obj = $(v);
                        var data = obj.data("share.event.position.data");

                        if (data != undefined) {
                            var instance = data.instance;

                            if (instance > 0) instance--;

                            if (instance == 0) {
                                if (data.observer != null) {
                                    data.observer.disconnect();
                                    data.observer = null;
                                }
                                data = null;
                                obj.removeData("share.event.position.data");
                            }

                        }
                    });
                }
                handle.object.off("." + id + ".event.share");
            }
        };
        var removeEvent = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                if (!$.isUndefined(handlers[id])) {
                    var handle = handlers[id];
                    deleteEvent(id);
                    delete handlers[id];
                }
            }
            else if (Array.isArray(o[0])) {
                $.each(o[0], function (i, v) {
                    removeEvent(v);
                });
            }
        };
        var disableEvent = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                if (!$.isUndefined(handlers[id]) && disabledHandlers.indexOf(id) == -1) {
                    disabledHandlers.push(id);
                    deleteEvent(id);
                }
            }
            else if (Array.isArray(o[0])) {
                $.each(o[0], function (i, v) {
                    disableEvent(v);
                });
            }
        };
        var enableEvent = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                var did = disabledHandlers.indexOf(id);

                if (!$.isUndefined(handlers[id]) && did > -1) {
                    var handle = handlers[id];
                    event(handle.object, handle.type, id, handle.callback, handle.once);
                    disabledHandlers.splice(did, 1);
                }
            }
            else if (Array.isArray(o[0])) {
                $.each(o[0], function (i, v) {
                    enableEvent(v);
                });
            }
        };
        var triggerEvent = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];

                if (!$.isUndefined(handlers[id]) && disabledHandlers.indexOf(id) == -1) {
                    var handle = handlers[id];

                    if (handle.type == "resize") {
                        handle.object.trigger("_resize", o[1]);
                    }
                }
            }
            else if (Array.isArray(o[0])) {
                $.each(o[0], function (i, v) {
                    triggerEvent(v);
                });
            }
        };
        var getEventHandle = function () {
            var o = arguments;
            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                if (!$.isUndefined(handlers[id])) {
                    var handle = handlers[id];
                    return handle;
                }
            }
        };

        var down = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "down", o[1], o[2], o[3]);
            else
                return event("down", o[0], o[1], o[2]);
        };
        var up = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "up", o[1], o[2], o[3]);
            else {
                return event("up", o[0], o[1], o[2]);
            }
        };
        var press = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return [
                    down(o[0], null, o[1], o[3]),
                    up(o[0], null, o[2], o[3])
                ];
            else
                return [
                    down(null, o[0], o[2]),
                    up(null, o[1], o[2])
                ];
        };
        
        var move = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "move", o[1], o[2], o[3]);
            else
                return event("move", o[0], o[1], o[2]);
        };
        var click = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "click", o[1], o[2], o[3]);
            else
                return event("click", o[0], o[1], o[2]);
        };
        var enter = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "enter", o[1], o[2], o[3]);
            else
                return event("enter", o[0], o[1], o[2]);
        };
        var leave = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "leave", o[1], o[2], o[3]);
            else
                return event("leave", o[0], o[1], o[2]);
        };
        var over = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "over", o[1], o[2], o[3]);
            else
                return event("over", o[0], o[1], o[2]);
        };
        var out = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "out", o[1], o[2], o[3]);
            else
                return event("out", o[0], o[1], o[2]);
        };
        var hover = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return [
                    enter(o[0], null, o[1], o[3]),
                    leave(o[0], null, o[2], o[3])
                ];
            else
                return [
                    enter(null, o[0], o[2]),
                    leave(null, o[1], o[2])
                ];
        };
        var wheel = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "wheel", o[1], o[2], o[3]);
            else
                return event("wheel", o[0], o[1], o[2]);
        };
        var keydown = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "keydown", o[1], o[2], o[3]);
            else
                return event("keydown", o[0], o[1], o[2]);
        };
        var keyup = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "keyup", o[1], o[2], o[3]);
            else
                return event("keyup", o[0], o[1], o[2]);
        };
        var keypress = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "keypress", o[1], o[2], o[3]);
            else
                return event("keypress", o[0], o[1], o[2]);
        };
        var input = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "input", o[1], o[2], o[3]);
            else
                return event("input", o[0], o[1], o[2]);
        };
        var change = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "change", o[1], o[2], o[3]);
            else
                return event("change", o[0], o[1], o[2]);
        };
        var scroll = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "scroll", o[1], o[2], o[3]);
            else
                return event("scroll", o[0], o[1], o[2]);
        };
        var focusin = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "focusin", o[1], o[2], o[3]);
            else
                return event("focusin", o[0], o[1], o[2]);
        };
        var focusout = function () {
            var o = arguments;
            if ($.isJQuery(o[0]))
                return event(o[0], "focusout", o[1], o[2], o[3]);
            else
                return event("focusout", o[0], o[1], o[2]);
        };

        // 1: down, 2: up, 3: move, 7: click
        // 4: enter, 5: leave, 8: over, 9: out
        // 6: wheel
        function pointingEvent(touch, event, type, a1, a2, a3) {
            var o = {
                x: null, y: null, touches: [], isTouch: false, id: null,
                button: null, // 1: left, 2: middle, 3: right
                preventDefault: function () {
                    event.preventDefault();
                },
                stopPropagation: function () {
                    event.stopPropagation();
                },
                stopImmediatePropagation: function () {
                    event.stopImmediatePropagation();
                },
                isPropagationStopped: function () {
                    return event.isPropagationStopped();
                },
                isImmediatePropagationStopped: function () {
                    return event.isImmediatePropagationStopped();
                },
            };

            if (!touch) {
                o.x = event.pageX;
                o.y = event.pageY;

                if (type == 6) {
                    o.delta = a1;
                    o.deltaX = a2;
                    o.deltaY = a3;
                }
                else if (type == 1 || type == 2 || type == 3 || type == 7) {
                    o.button = event.which;
                }
            }
            else {
                o.isTouch = true;
                //event.preventDefault();

                var ct = event.originalEvent.changedTouches;
                var t = event.originalEvent.touches;

                $.each(t, function (cti, ctv) {
                    o.touches.push({
                        x: ctv.pageX,
                        y: ctv.pageY,
                        id: ctv.identifier
                    });
                });

                if (type != 3 || ct.length == 1) {
                    o.x = ct[0].pageX;
                    o.y = ct[0].pageY;
                    o.id = ct[0].identifier;
                }
            }

            return o;
        };

        // 1: keydown, 2: keyup, 3: keypress, 4: textinput
        function keyboardEvent(event, type) {
            var o = {
                key: event.which,
                shift: event.shiftKey,
                ctrl: event.ctrlKey,
                alt: event.altKey,
                event: event,
                preventDefault: function () {
                    event.preventDefault();
                }
            };

            return o;
        };

        // 1: change
        function formEvent(event, type) {
            var o = {};
            return o;
        };

        // scroll
        function scrollEvent(event) {
            var el = $(event.currentTarget);
            var o = {
                top: el.scrollTop(),
                left: el.scrollLeft(),
                width: el.width(),
                height: el.height()
            };
            return o;
        };

        // resize
        function resizeEvent(event, width, height, lastWidth, lastHeight) {
            var o = {
                width: width,
                height: height,
                lastWidth: lastWidth,
                lastHeight: lastHeight,
                widthChanged: width != lastWidth,
                heightChanged: height != lastHeight
            };

            return o;
        };

        // position
        function positionEvent(event, left, top, lastLeft, lastTop) {
            var o = {
                left: left,
                top: top,
                lastLeft: lastLeft,
                lastTop: lastTop,
                leftChanged: left != lastLeft,
                topChanged: top != lastTop
            };

            return o;
        };

        // focus 1: in 2: out
        function focusEvent(event, type) {
            var o = {
                type: type == 1 ? "in" : "out"
            };
            return o;
        };

        // mutation
        function mutationEvent(event) {
            var o = {

            };
            return o;
        };

        share.event = event;
        share.removeEvent = removeEvent;
        share.disableEvent = disableEvent;
        share.enableEvent = enableEvent;
        share.triggerEvent = triggerEvent;
        share.getEventHandle = getEventHandle;

        share.down = down;
        share.up = up;
        share.press = press;
        share.move = move;
        share.click = click;
        share.enter = enter;
        share.leave = leave;
        share.over = over;
        share.out = out;
        share.hover = hover;
        share.wheel = wheel;
        share.keydown = keydown;
        share.keyup = keyup;
        share.keypress = keypress;
        share.input = input;
        share.change = change;
        share.scroll = scroll;
        share.focusin = focusin;
        share.focusout = focusout;

    })(share);

    // .stream .removeStream .isStreamAvailable
    (function (share) {

        var version = null;

        var guid = 0;
        var handlers = {};
        var portal = {};
        var streamDomain;
        var streamPath;
        var instance;
        var isAvailable = -1;
        var isAvailable_ = -1;

        var serverRegisters = [];
        var registers = [];

        var timeToUpdate = 200;
        var timeOutID;

        var streamError = false;

        var stream = function (callback) {
            if ($.isFunction(callback)) {
                guid = share.lookup(guid, handlers);
                handlers[guid] = callback;
                return guid;
            }
        };
        var removeStream = function (id) {
            if (id == null) return;
            if (!$.isUndefined(handlers[id])) {
                delete handlers[id];
            }
        };
        var isStreamAvailable = function () {
            return isAvailable == 1;
        };
        var register = function (s) {
            if (arguments.length > 1) {
                $.each(arguments, function (ia, va) {
                    register(va);
                });
            }
            else if (Array.isArray(s)) {
                $.each(s, function (is, vs) {
                    register(vs);
                });
            }
            else if ($.isString(s)) {
                if (registers.indexOf(s) == -1) {
                    clearTimeout(timeOutID);
                    registers.push(s);
                    timeOutID = setTimeout(updateRegister, timeToUpdate);
                }
            }
        };
        var removeRegister = function (s) {
            if (arguments.length > 1) {
                $.each(arguments, function (ia, va) {
                    removeRegister(va);
                });
            }
            else if (Array.isArray(s)) {
                $.each(s, function (is, vs) {
                    removeRegister(vs);
                });
            }
            else if ($.isString(s)) {
                var idx = registers.indexOf(s);
                if (idx > -1) {
                    clearTimeout(timeOutID);
                    registers.splice(idx, 1);
                    timeOutID = setTimeout(updateRegister, timeToUpdate);
                }
            }
        };

        function updateRegister() {
            if (isStreamAvailable()) {

                var registerAdd = share.diff(registers, serverRegisters);
                var registerRemove = share.diff(serverRegisters, registers);

                var modifications = [];

                if (registerRemove.length > 0) {
                    $.each(registerRemove, function (ir, vr) {
                        modifications.push("-" + vr);
                    });
                }
                if (registerAdd.length > 0) {
                    $.each(registerAdd, function (ir, vr) {
                        modifications.push(vr);
                    });
                }

                if (modifications.length > 0)
                    share.post(10, { x: modifications.join() });

                serverRegisters = registers.slice();
            }
        };
        function start() {
            isAvailable = -1;
            instance = portal.open("", {
                credentials: true,
                transports: ["streamxhr"],
                urlBuilder: function () {
                    return share.protocol() + "://" + streamDomain + streamPath + "?c=" + share.client() + "&_=" + share.date().getTime();
                },
                sharing: false,
                streamParser: function (chunk) {
                    if (chunk == "") return [];
                    var lines = chunk.split("\n");
                    if (lines.length == 0) return [];
                    var relines = [];
                    for (var il = 0; il < lines.length; il++) {
                        var line;
                        if (lines[il] == "") continue;
                        if (lines[il].startsWith("for(;;); ")) line = lines[il].substr(9);
                        else line = lines[il];
                        if (line != "") relines.push(line);
                    }
                    return relines;
                },
                inbound: function (line) {
                    if (line == "" || line == null) return;
                    var data = JSON.parse(line);

                    if (data) {
                        var type = data.t;
                        var obj = data.d;

                        if (type == "heartbeat") {
                            if (streamError) {
                                streamError = false;
                                if (version != ver) {
                                    $.each(handlers, function (i, v) {
                                        v("streamrecovered");
                                    });
                                }
                            }
                            var ver = data.v;
                            if (version == null) version = ver;
                            else {
                                if (version != ver) {
                                    $.each(handlers, function (i, v) {
                                        v("update");
                                    });
                                }
                            }
                        }
                        else {
                            if (type == "available") {

                                isAvailable = 1;
                                if (isAvailable_ != isAvailable) {

                                    // send all registers to server
                                    if (registers.length > 0) {
                                        share.post(11, { x: registers.join() });
                                        serverRegisters = registers.slice();
                                    }

                                    isAvailable_ = 1;
                                    $.each(handlers, function (i, v) {
                                        if ($.isFunction(v)) v("online");
                                    });
                                }
                            }
                            else if (type == "unavailable" || type == "disconnected" || type == "pageend") {

                                instance.close();
                                instance = null;

                                isAvailable = 0;
                                if (isAvailable_ != isAvailable) {

                                    serverRegisters = [];

                                    isAvailable_ = 0
                                    $.each(handlers, function (i, v) {
                                        if ($.isFunction(v)) v("offline");
                                    });
                                }

                                if (type == "unavailable" || type == "disconnected")
                                    setTimeout(start, 5000); // try again
                            }
                            else {
                                if (type == "updatestreamdomain") {
                                    streamDomain = obj;
                                }
                                else if (type == "ping") {
                                }
                                else if (type == "continue") {
                                }
                                else if (type == "chat") {
                                }
                                else {
                                    $.each(handlers, function (i, v) {
                                        if ($.isFunction(v)) v(type, obj);
                                    });
                                }
                            }
                        }
                    }
                },
                outbound: function (event) {
                },
                onerror: function (err) {                    
                    if (streamError == false) {
                        streamError = true;
                        $.each(handlers, function (i, v) {
                            if ($.isFunction(v)) v("streamerror");
                        });
                    }
                }
            });
        };

        share.stream = stream;
        share.removeStream = removeStream;
        share.isStreamAvailable = isStreamAvailable;
        share.register = register;
        share.removeRegister = removeRegister;

        $(function () {
            streamDomain = share.system("streamDomain");
            streamPath = share.system("streamPath");
            if (streamDomain != null) {
                share.unload(function () { if (instance != null) instance.close(); });
                setTimeout(start, 500);
            }
        });

        (function (portal) {
            // Portal 1.0 http://github.com/flowersinthesand/portal - modded -> removing some common functions
            "use strict";

            var // A global identifier
                guid,
                // Is the unload event being processed?
                unloading,
                // Socket instances
                sockets = {},
                // Callback names for JSONP
                jsonpCallbacks = [],
                // Core prototypes
                toString = Object.prototype.toString,
                hasOwn = Object.prototype.hasOwnProperty,
                slice = Array.prototype.slice;

            // Convenience utilities
            // Most utility functions are borrowed from jQuery
            portal.support = {
                getAbsoluteURL: function (url) {
                    var div = document.createElement("div");

                    // Uses an innerHTML property to obtain an absolute URL
                    div.innerHTML = '<a href="' + url + '"/>';

                    // encodeURI and decodeURI are needed to normalize URL between IE and non-IE, 
                    // since IE doesn't encode the href property value and return it - http://jsfiddle.net/Yq9M8/1/
                    return encodeURI(decodeURI(div.firstChild.href));
                },
                iterate: function (fn) {
                    var timeoutId;

                    // Though the interval is 1ms for real-time application, there is a delay between setTimeout calls
                    // For detail, see https://developer.mozilla.org/en/window.setTimeout#Minimum_delay_and_timeout_nesting
                    (function loop() {
                        timeoutId = setTimeout(function () {
                            if (fn() === false) {
                                return;
                            }

                            loop();
                        }, 1);
                    })();

                    return function () {
                        clearTimeout(timeoutId);
                    };
                },
                extend: function (target) {
                    var i, options, name;

                    for (i = 1; i < arguments.length; i++) {
                        if ((options = arguments[i]) != null) {
                            for (name in options) {
                                target[name] = options[name];
                            }
                        }
                    }

                    return target;
                },
                param: function (params) {
                    var prefix,
                        s = [];

                    function add(key, value) {
                        value = $.isFunction(value) ? value() : (value == null ? "" : value);
                        s.push(encodeURIComponent(key) + "=" + encodeURIComponent(value));
                    }

                    function buildParams(prefix, obj) {
                        var name;

                        if (Array.isArray(obj)) {
                            $.each(obj, function (i, v) {
                                if (/\[\]$/.test(prefix)) {
                                    add(prefix, v);
                                } else {
                                    buildParams(prefix + "[" + (typeof v === "object" ? i : "") + "]", v);
                                }
                            });
                        } else if (toString.call(obj) === "[object Object]") {
                            for (name in obj) {
                                buildParams(prefix + "[" + name + "]", obj[name]);
                            }
                        } else {
                            add(prefix, obj);
                        }
                    }

                    for (prefix in params) {
                        buildParams(prefix, params[prefix]);
                    }

                    return s.join("&").replace(/%20/g, "+");
                },
                xhr: function () {
                    try {
                        return new window.XMLHttpRequest();
                    } catch (e1) {
                        try {
                            return new window.ActiveXObject("Microsoft.XMLHTTP");
                        } catch (e2) { }
                    }
                },
                browser: {},
                storage: !!(window.localStorage && window.StorageEvent)
            };
            portal.support.corsable = "withCredentials" in portal.support.xhr();
            guid = share.date();

            // Browser sniffing
            (function () {
                var ua = navigator.userAgent.toLowerCase(),
                    match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
                        /(webkit)[ \/]([\w.]+)/.exec(ua) ||
                        /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
                        /(msie) ([\w.]+)/.exec(ua) ||
                        ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) ||
                        [];

                portal.support.browser[match[1] || ""] = true;
                portal.support.browser.version = match[2] || "0";

                // The storage event of Internet Explorer and Firefox 3 works strangely
                if (portal.support.browser.msie || (portal.support.browser.mozilla && portal.support.browser.version.split(".")[0] === "1")) {
                    portal.support.storage = false;
                }
            })();

            // Finds the socket object which is mapped to the given url
            portal.find = function (url) {
                var i;

                // Returns the first socket in the document
                if (!arguments.length) {
                    for (i in sockets) {
                        if (sockets[i]) {
                            return sockets[i];
                        }
                    }
                    return null;
                }

                // The url is a identifier of this socket within the document
                return sockets[portal.support.getAbsoluteURL(url)] || null;
            };
            // Creates a new socket and connects to the given url 
            portal.open = function (url, options) {
                // Makes url absolute to normalize URL
                url = portal.support.getAbsoluteURL(url);
                sockets[url] = socket(url, options);

                return portal.find(url);
            };
            // Default options
            portal.defaults = {
                // Socket options
                transports: ["streamxhr"],
                timeout: false,
                heartbeat: false,
                lastEventId: 0,
                sharing: false,
                prepare: function (connect) {
                    connect();
                },
                reconnect: function (lastDelay) {
                    return 2 * (lastDelay || 250);
                },
                idGenerator: function () {
                    // Generates a random UUID 
                    // Logic borrowed from http://stackoverflow.com/questions/105034/how-to-create-a-guid-uuid-in-javascript/2117523#2117523
                    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
                        var r = Math.random() * 16 | 0,
                            v = c === "x" ? r : (r & 0x3 | 0x8);

                        return v.toString(16);
                    });
                },
                urlBuilder: function (url, params, when) {
                    return url + (/\?/.test(url) ? "&" : "?") + "when=" + when + "&" + portal.support.param(params);
                },
                inbound: JSON.parse,
                outbound: JSON.stringify,

                // Transport options
                credentials: false,
                notifyAbort: false,
                streamParser: function (chunk) {
                    // Chunks are formatted according to the event stream format 
                    // http://www.w3.org/TR/eventsource/#event-stream-interpretation
                    var reol = /\r\n|[\r\n]/g, lines = [], data = this.data("data"), array = [], i = 0,
                        match, line;

                    // Strips off the left padding of the chunk
                    // the first chunk of some streaming transports and every chunk for Android browser 2 and 3 has padding
                    chunk = chunk.replace(/^\s+/g, "");

                    // String.prototype.split is not reliable cross-browser
                    while (match = reol.exec(chunk)) {
                        lines.push(chunk.substring(i, match.index));
                        i = match.index + match[0].length;
                    }
                    lines.push(chunk.length === i ? "" : chunk.substring(i));

                    if (!data) {
                        data = [];
                        this.data("data", data);
                    }

                    // Processes the data field only
                    for (i = 0; i < lines.length; i++) {
                        line = lines[i];
                        if (!line) {
                            // Finish
                            array.push(data.join("\n"));
                            data = [];
                            this.data("data", data);
                        } else if (/^data:\s/.test(line)) {
                            // A single data field
                            data.push(line.substring("data: ".length));
                        } else {
                            // A fragment of a data field
                            data[data.length - 1] += line;
                        }
                    }

                    return array;
                },

                // Undocumented
                _heartbeat: 5000,
                longpollTest: true
                // method: null
                // initIframe: null
            };

            // Callback function
            function callbacks(deferred) {
                var list = [],
                    locked,
                    memory,
                    firing,
                    firingStart,
                    firingLength,
                    firingIndex,
                    fire = function (context, args) {
                        args = args || [];
                        memory = !deferred || [context, args];
                        firing = true;
                        firingIndex = firingStart || 0;
                        firingStart = 0;
                        firingLength = list.length;
                        for (; firingIndex < firingLength; firingIndex++) {
                            list[firingIndex].apply(context, args);
                        }
                        firing = false;
                    },
                    self = {
                        add: function (fn) {
                            var length = list.length;

                            list.push(fn);
                            if (firing) {
                                firingLength = list.length;
                            } else if (!locked && memory && memory !== true) {
                                firingStart = length;
                                fire(memory[0], memory[1]);
                            }
                        },
                        remove: function (fn) {
                            var i;

                            for (i = 0; i < list.length; i++) {
                                if (fn === list[i] || (fn.guid && fn.guid === list[i].guid)) {
                                    if (firing) {
                                        if (i <= firingLength) {
                                            firingLength--;
                                            if (i <= firingIndex) {
                                                firingIndex--;
                                            }
                                        }
                                    }
                                    list.splice(i--, 1);
                                }
                            }
                        },
                        fire: function (context, args) {
                            if (!locked && !firing && !(deferred && memory)) {
                                fire(context, args);
                            }
                        },
                        lock: function () {
                            locked = true;
                        },
                        locked: function () {
                            return !!locked;
                        },
                        unlock: function () {
                            locked = memory = firing = firingStart = firingLength = firingIndex = undefined;
                        }
                    };

                return self;
            }

            // Socket function
            function socket(url, options) {
                var	// Final options
                    opts,
                    // Transport
                    transport,
                    // The state of the connection
                    state,
                    // Event helpers
                    events = {},
                    eventId = 0,
                    // Reply callbacks
                    replyCallbacks = {},
                    // Buffer
                    buffer = [],
                    // Reconnection
                    reconnectTimer,
                    reconnectDelay,
                    reconnectTry,
                    // Map of the connection-scoped values
                    connection = {},
                    parts = /^([\w\+\.\-]+:)(?:\/\/([^\/?#:]*)(?::(\d+))?)?/.exec(url.toLowerCase()),
                    // Socket object
                    self = {
                        // Finds the value of an option
                        option: function (key, /* undocumented */ value) {
                            if (value === undefined) {
                                return opts[key];
                            }

                            opts[key] = value;

                            return this;
                        },
                        // Gets or sets a connection-scoped value
                        data: function (key, value) {
                            if (value === undefined) {
                                return connection[key];
                            }

                            connection[key] = value;

                            return this;
                        },
                        // Returns the state
                        state: function () {
                            return state;
                        },
                        // Adds event handler
                        on: function (type, fn) {
                            var event;

                            // Handles a map of type and handler
                            if (typeof type === "object") {
                                for (event in type) {
                                    self.on(event, type[event]);
                                }
                                return this;
                            }

                            // For custom event
                            event = events[type];
                            if (!event) {
                                if (events.message.locked()) {
                                    return this;
                                }

                                event = events[type] = callbacks();
                                event.order = events.message.order;
                            }

                            event.add(fn);

                            return this;
                        },
                        // Removes event handler
                        off: function (type, fn) {
                            var event = events[type];

                            if (event) {
                                event.remove(fn);
                            }

                            return this;
                        },
                        // Adds one time event handler
                        one: function (type, fn) {
                            function proxy() {
                                self.off(type, proxy);
                                fn.apply(self, arguments);
                            }

                            fn.guid = fn.guid || guid++;
                            proxy.guid = fn.guid;

                            return self.on(type, proxy);
                        },
                        // Fires event handlers
                        fire: function (type) {
                            var event = events[type];

                            if (event) {
                                event.fire(self, slice.call(arguments, 1));
                            }

                            return this;
                        },
                        // Establishes a connection
                        open: function () {
                            var type,
                                latch,
                                connect = function () {
                                    var candidates, type;

                                    if (!latch) {
                                        latch = true;
                                        candidates = connection.candidates = slice.call(opts.transports);
                                        while (!transport && candidates.length) {
                                            type = candidates.shift();
                                            connection.transport = type;
                                            connection.url = self.buildURL("open");
                                            transport = portal.transports[type](self, opts);
                                        }

                                        // Increases the number of reconnection attempts
                                        if (reconnectTry) {
                                            reconnectTry++;
                                        }

                                        // Fires the connecting event and connects
                                        if (transport) {
                                            self.fire("connecting");
                                            transport.open();
                                        } else {
                                            self.fire("close", "notransport");
                                        }
                                    }
                                },
                                cancel = function () {
                                    if (!latch) {
                                        latch = true;
                                        self.fire("close", "canceled");
                                    }
                                };

                            // Cancels the scheduled connection
                            if (reconnectTimer) {
                                clearTimeout(reconnectTimer);
                            }

                            // Resets the connection scope and event helpers
                            connection = {};
                            for (type in events) {
                                events[type].unlock();
                            }

                            // Chooses transport
                            transport = undefined;

                            // From null or waiting state
                            state = "preparing";

                            // Check if possible to make use of a shared socket
                            if (opts.sharing) {
                                connection.transport = "session";
                                transport = portal.transports.session(self, opts);
                            }

                            // Executes the prepare handler if a physical connection is needed
                            if (transport) {
                                connect();
                            } else {
                                opts.prepare.call(self, connect, cancel, opts);
                            }

                            return this;
                        },
                        // Sends an event to the server via the connection
                        send: function (type, data, doneCallback, failCallback) {
                            var event;

                            // Defers sending an event until the state become opened
                            if (state !== "opened") {
                                buffer.push(arguments);
                                return this;
                            }

                            // Outbound event
                            event = {
                                id: ++eventId,
                                socket: opts.id,
                                type: type,
                                data: data,
                                reply: !!(doneCallback || failCallback)
                            };

                            if (event.reply) {
                                // Shared socket needs to know the callback event name 
                                // because it fires the callback event directly instead of using reply event 
                                if (connection.transport === "session") {
                                    event.doneCallback = doneCallback;
                                    event.failCallback = failCallback;
                                } else {
                                    replyCallbacks[eventId] = { done: doneCallback, fail: failCallback };
                                }
                            }

                            // Delegates to the transport
                            transport.send($.isBinary(data) ? data : opts.outbound.call(self, event));

                            return this;
                        },
                        // Disconnects the connection
                        close: function () {
                            var script, head;

                            // Prevents reconnection
                            opts.reconnect = false;
                            if (reconnectTimer) {
                                clearTimeout(reconnectTimer);
                            }

                            // Fires the close event immediately for transport which doesn't give feedback on disconnection
                            if (unloading || !transport || !transport.feedback) {
                                self.fire("close", unloading ? "error" : "aborted");
                                if (opts.notifyAbort && connection.transport !== "session") {
                                    head = document.head || document.getElementsByTagName("head")[0] || document.documentElement;
                                    script = document.createElement("script");
                                    script.async = false;
                                    script.src = self.buildURL("abort");
                                    script.onload = script.onreadystatechange = function () {
                                        if (!script.readyState || /loaded|complete/.test(script.readyState)) {
                                            script.onload = script.onreadystatechange = null;
                                            if (script.parentNode) {
                                                script.parentNode.removeChild(script);
                                            }
                                        }
                                    };
                                    head.insertBefore(script, head.firstChild);
                                }
                            }

                            // Delegates to the transport
                            if (transport) {
                                transport.close();
                            }

                            return this;
                        },
                        // Broadcasts event to session sockets
                        broadcast: function (type, data) {
                            // TODO rename
                            var broadcastable = connection.broadcastable;
                            if (broadcastable) {
                                broadcastable.broadcast({ type: "fire", data: { type: type, data: data } });
                            }

                            return this;
                        },
                        // For internal use only
                        // fires events from the server
                        _fire: function (data, isChunk) {
                            var array;

                            if (isChunk) {
                                data = opts.streamParser.call(self, data);
                                while (data.length) {
                                    self._fire(data.shift());
                                }
                                return this;
                            }

                            if ($.isBinary(data)) {
                                array = [{ type: "message", data: data }];
                            } else {
                                array = opts.inbound.call(self, data);
                                array = array == null ? [] : !Array.isArray(array) ? [array] : array;
                            }

                            connection.lastEventIds = [];
                            $.each(array, function (i, event) {
                                var latch, args = [event.type, event.data];

                                opts.lastEventId = event.id;
                                connection.lastEventIds.push(event.id);
                                if (event.reply) {
                                    args.push(function (result) {
                                        if (!latch) {
                                            latch = true;
                                            self.send("reply", { id: event.id, data: result });
                                        }
                                    });
                                }

                                self.fire.apply(self, args).fire("_message", args);
                            });

                            return this;
                        },
                        // For internal use only
                        // builds an effective URL
                        buildURL: function (when, params) {
                            var p = when === "open" ?
							{
							    transport: connection.transport,
							    heartbeat: opts.heartbeat,
							    lastEventId: opts.lastEventId
							} :
                                    when === "poll" ?
							{
							    transport: connection.transport,
							    lastEventIds: connection.lastEventIds && connection.lastEventIds.join(","),
							    /* deprecated */lastEventId: opts.lastEventId
							} :
							{};

                            portal.support.extend(p, { id: opts.id, _: guid++ }, opts.params && opts.params[when], params);
                            return opts.urlBuilder.call(self, url, p, when);
                        }
                    };

                // Create the final options
                opts = portal.support.extend({}, portal.defaults, options);
                if (options) {
                    // Array should not be deep extended
                    if (options.transports) {
                        opts.transports = slice.call(options.transports);
                    }
                }
                // Saves original URL
                opts.url = url;
                // Generates socket id,
                opts.id = opts.idGenerator.call(self);
                opts.crossDomain = !!(parts &&
                    // protocol and hostname
                    (parts[1] != location.protocol || parts[2] != location.hostname ||
                    // port
                    (parts[3] || (parts[1] === "http:" ? 80 : 443)) != (location.port || (location.protocol === "http:" ? 80 : 443))));

                $.each(["connecting", "open", "message", "close", "waiting"], function (i, type) {
                    // Creates event helper
                    events[type] = callbacks(type !== "message");
                    events[type].order = i;

                    // Shortcuts for on method
                    var old = self[type],
                        on = function (fn) {
                            return self.on(type, fn);
                        };

                    self[type] = !old ? on : function (fn) {
                        return ($.isFunction(fn) ? on : old).apply(this, arguments);
                    };
                });

                // Initializes
                self.on({
                    connecting: function () {
                        // From preparing state
                        state = "connecting";

                        var timeoutTimer;

                        // Sets timeout timer
                        function setTimeoutTimer() {
                            timeoutTimer = setTimeout(function () {
                                transport.close();
                                self.fire("close", "timeout");
                            }, opts.timeout);
                        }

                        // Clears timeout timer
                        function clearTimeoutTimer() {
                            clearTimeout(timeoutTimer);
                        }

                        // Makes the socket sharable
                        function share() {
                            var traceTimer,
                                server,
                                name = "socket-" + url,
                                servers = {
                                    // Powered by the storage event and the localStorage
                                    // http://www.w3.org/TR/webstorage/#event-storage
                                    storage: function () {
                                        if (!portal.support.storage) {
                                            return;
                                        }

                                        var storage = window.localStorage;

                                        return {
                                            init: function () {
                                                function onstorage(event) {
                                                    // When a deletion, newValue initialized to null
                                                    if (event.key === name && event.newValue) {
                                                        listener(event.newValue);
                                                    }
                                                }

                                                // Handles the storage event 
                                                $.window.on("storage", onstorage);
                                                self.one("close", function () {
                                                    $.window.off("storage", onstorage);
                                                    // Defers again to clean the storage
                                                    self.one("close", function () {
                                                        storage.removeItem(name);
                                                        storage.removeItem(name + "-opened");
                                                        storage.removeItem(name + "-children");
                                                    });
                                                });
                                            },
                                            broadcast: function (obj) {
                                                var string = JSON.stringify(obj);
                                                storage.setItem(name, string);
                                                setTimeout(function () {
                                                    listener(string);
                                                }, 50);
                                            },
                                            get: function (key) {
                                                return JSON.parse(storage.getItem(name + "-" + key));
                                            },
                                            set: function (key, value) {
                                                storage.setItem(name + "-" + key, JSON.stringify(value));
                                            }
                                        };
                                    },
                                    // Powered by the window.open method
                                    // https://developer.mozilla.org/en/DOM/window.open
                                    windowref: function () {
                                        // Internet Explorer raises an invalid argument error
                                        // when calling the window.open method with the name containing non-word characters
                                        var neim = name.replace(/\W/g, ""),
                                            container = document.getElementById(neim),
                                            win;

                                        if (!container) {
                                            container = document.createElement("div");
                                            container.id = neim;
                                            container.style.display = "none";
                                            container.innerHTML = '<iframe name="' + neim + '" />';
                                            document.body.appendChild(container);
                                        }

                                        win = container.firstChild.contentWindow;

                                        return {
                                            init: function () {
                                                // Callbacks from different windows
                                                win.callbacks = [listener];
                                                // In IE 8 and less, only string argument can be safely passed to the function in other window
                                                win.fire = function (string) {
                                                    var i;

                                                    for (i = 0; i < win.callbacks.length; i++) {
                                                        win.callbacks[i](string);
                                                    }
                                                };
                                            },
                                            broadcast: function (obj) {
                                                if (!win.closed && win.fire) {
                                                    win.fire(JSON.stringify(obj));
                                                }
                                            },
                                            get: function (key) {
                                                return !win.closed ? win[key] : null;
                                            },
                                            set: function (key, value) {
                                                if (!win.closed) {
                                                    win[key] = value;
                                                }
                                            }
                                        };
                                    }
                                };

                            // Receives send and close command from the children
                            function listener(string) {
                                var command = JSON.parse(string), data = command.data;

                                if (!command.target) {
                                    if (command.type === "fire") {
                                        self.fire(data.type, data.data);
                                    }
                                } else if (command.target === "p") {
                                    switch (command.type) {
                                        case "send":
                                            self.send(data.type, data.data, data.doneCallback, data.failCallback);
                                            break;
                                        case "close":
                                            self.close();
                                            break;
                                    }
                                }
                            }

                            function propagateMessageEvent(args) {
                                server.broadcast({ target: "c", type: "message", data: args });
                            }

                            function leaveTrace() {
                                document.cookie = encodeURIComponent(name) + "=" +
                                    // Opera 12.00's parseFloat and JSON.stringify causes a strange bug with a number larger than 10 digit
                                    // JSON.stringify(parseFloat(10000000000) + 1).length === 11;
                                    // JSON.stringify(parseFloat(10000000000 + 1)).length === 10;
                                    encodeURIComponent(JSON.stringify({ ts: share.date() + 1, heir: (server.get("children") || [])[0] }));
                            }

                            // Chooses a server
                            server = servers.storage() || servers.windowref();
                            server.init();

                            // For broadcast method
                            connection.broadcastable = server;

                            // List of children sockets
                            server.set("children", []);
                            // Flag indicating the parent socket is opened
                            server.set("opened", false);

                            // Leaves traces
                            leaveTrace();
                            traceTimer = setInterval(leaveTrace, 1000);

                            self.on("_message", propagateMessageEvent)
                            .one("open", function () {
                                server.set("opened", true);
                                server.broadcast({ target: "c", type: "open" });
                            })
                            .one("close", function (reason) {
                                // Clears trace timer 
                                clearInterval(traceTimer);
                                // Removes the trace
                                document.cookie = encodeURIComponent(name) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT";
                                // The heir is the parent unless unloading
                                server.broadcast({ target: "c", type: "close", data: { reason: reason, heir: !unloading ? opts.id : (server.get("children") || [])[0] } });
                                self.off("_message", propagateMessageEvent);
                            });
                        }

                        if (opts.timeout > 0) {
                            setTimeoutTimer();
                            self.one("open", clearTimeoutTimer).one("close", clearTimeoutTimer);
                        }

                        // Share the socket if possible
                        if (opts.sharing && connection.transport !== "session") {
                            share();
                        }
                    },
                    open: function () {
                        // From connecting state
                        state = "opened";

                        var heartbeatTimer;

                        // Sets heartbeat timer
                        function setHeartbeatTimer() {
                            heartbeatTimer = setTimeout(function () {
                                self.send("heartbeat").one("heartbeat", function () {
                                    clearHeartbeatTimer();
                                    setHeartbeatTimer();
                                });

                                heartbeatTimer = setTimeout(function () {
                                    transport.close();
                                    self.fire("close", "error");
                                }, opts._heartbeat);
                            }, opts.heartbeat - opts._heartbeat);
                        }

                        // Clears heartbeat timer
                        function clearHeartbeatTimer() {
                            clearTimeout(heartbeatTimer);
                        }

                        if (opts.heartbeat > opts._heartbeat) {
                            setHeartbeatTimer();
                            self.one("close", clearHeartbeatTimer);
                        }

                        // Locks the connecting event
                        events.connecting.lock();

                        // Initializes variables related with reconnection
                        reconnectTimer = reconnectDelay = reconnectTry = null;

                        // Flushes buffer
                        while (buffer.length) {
                            self.send.apply(self, buffer.shift());
                        }
                    },
                    close: function () {
                        // From preparing, connecting, or opened state 
                        state = "closed";

                        var type, event, order = events.close.order;

                        // Locks event whose order is lower than close event
                        for (type in events) {
                            event = events[type];
                            if (event.order < order) {
                                event.lock();
                            }
                        }

                        // Schedules reconnection
                        if (opts.reconnect) {
                            self.one("close", function () {
                                reconnectTry = reconnectTry || 1;
                                reconnectDelay = opts.reconnect.call(self, reconnectDelay, reconnectTry);

                                if (reconnectDelay !== false) {
                                    reconnectTimer = setTimeout(function () {
                                        self.open();
                                    }, reconnectDelay);
                                    self.fire("waiting", reconnectDelay, reconnectTry);
                                }
                            });
                        }
                    },
                    waiting: function () {
                        // From closed state
                        state = "waiting";
                    },
                    reply: function (reply) {
                        var fn,
                            id = reply.id,
                            data = reply.data,
                            exception = reply.exception,
                            callback = replyCallbacks[id];

                        if (callback) {
                            fn = exception ? callback.fail : callback.done;
                            if (fn) {
                                if ($.isFunction(fn)) {
                                    fn.call(self, data);
                                } else {
                                    self.fire(fn, data).fire("_message", [fn, data]);
                                }

                                delete replyCallbacks[id];
                            }
                        }
                    }
                });

                return self.open();
            }

            // Transports
            portal.transports = {
                // HTTP Support
                httpbase: function (socket, options) {
                    var send,
                        sending,
                        queue = [];

                    function post() {
                        if (queue.length) {
                            send(options.url, queue.shift());
                        } else {
                            sending = false;
                        }
                    }

                    // The Content-Type is not application/x-www-form-urlencoded but text/plain on account of XDomainRequest
                    // See the fourth at http://blogs.msdn.com/b/ieinternals/archive/2010/05/13/xdomainrequest-restrictions-limitations-and-workarounds.aspx
                    send = !options.crossDomain || portal.support.corsable ?
                    function (url, data) {
                        var xhr = portal.support.xhr();

                        xhr.onreadystatechange = function () {
                            if (xhr.readyState === 4) {
                                post();
                            }
                        };

                        xhr.open("POST", url);
                        xhr.setRequestHeader("Content-Type", "text/plain; charset=UTF-8");
                        if (portal.support.corsable) {
                            xhr.withCredentials = options.credentials;
                        }

                        xhr.send("data=" + data);

                    } : window.XDomainRequest && options.xdrURL && options.xdrURL.call(socket, "t") ?
                    function (url, data) {
                        var xdr = new window.XDomainRequest();

                        xdr.onload = xdr.onerror = post;
                        xdr.open("POST", options.xdrURL.call(socket, url));
                        xdr.send("data=" + data);
                    } :
                    function (url, data) {
                        var iframe,
                            textarea,
                            form = document.createElement("form");

                        form.action = url;
                        form.target = "socket-" + (++guid);
                        form.method = "POST";
                        // IE 6 needs encoding property
                        form.enctype = form.encoding = "text/plain";
                        form.acceptCharset = "UTF-8";
                        form.style.display = "none";
                        form.innerHTML = '<textarea name="data"></textarea><iframe name="' + form.target + '"></iframe>';

                        textarea = form.firstChild;
                        textarea.value = data;

                        iframe = form.lastChild;
                        $(iframe).on("load", function () {
                            document.body.removeChild(form);
                            post();
                        });

                        document.body.appendChild(form);
                        form.submit();
                    };

                    return {
                        send: function (data) {
                            queue.push(data);

                            if (!sending) {
                                sending = true;
                                post();
                            }
                        }
                    };
                },
                // Streaming - XMLHttpRequest
                streamxhr: function (socket, options) {
                    var xhr;

                    if ((portal.support.browser.msie && +portal.support.browser.version < 10) || (options.crossDomain && !portal.support.corsable)) {
                        return;
                    }

                    return portal.support.extend(portal.transports.httpbase(socket, options), {
                        open: function () {
                            var stop;

                            xhr = portal.support.xhr();
                            xhr.onreadystatechange = function () {
                                function onprogress() {
                                    var index = socket.data("index"),
                                        length = xhr.responseText.length;

                                    if (!index) {
                                        socket.fire("open")._fire(xhr.responseText, true);
                                    } else if (length > index) {
                                        socket._fire(xhr.responseText.substring(index, length), true);
                                    }

                                    socket.data("index", length);
                                }

                                if (xhr.readyState === 3 && xhr.status === 200) {
                                    // Despite the change in response, Opera doesn't fire the readystatechange event
                                    if (portal.support.browser.opera && !stop) {
                                        stop = portal.support.iterate(onprogress);
                                    } else {
                                        onprogress();
                                    }
                                } else if (xhr.readyState === 4) {
                                    if (stop) {
                                        stop();
                                    }

                                    socket.fire("close", xhr.status === 200 ? "done" : "error");
                                }
                            };

                            xhr.onerror = function (err) {
                                if (options.onerror != null) {
                                    options.onerror(err);
                                }
                            };

                            xhr.open(options.method || "GET", socket.data("url"));
                            if (portal.support.corsable) {
                                xhr.withCredentials = options.credentials;
                            }

                            xhr.send(null);
                        },
                        close: function () {
                            xhr.abort();
                        }
                    });
                },
            };

            // Closes all sockets
            portal.finalize = function () {
                var url, socket;

                for (url in sockets) {
                    socket = sockets[url];
                    if (socket.state() !== "closed") {
                        socket.close();
                    }

                    // To run the test suite
                    delete sockets[url];
                }
            };

            share.unload(function () {
                // Check the unload event is fired by the browser
                unloading = true;
                // Closes all sockets when the document is unloaded 
                portal.finalize();
            });
            $.window.on("online", function () {
                var url, socket;
                for (url in sockets) {
                    socket = sockets[url];
                    // There is no reason to wait
                    if (socket.state() === "waiting") {
                        socket.open();
                    }
                }
            });
            $.window.on("offline", function () {
                var url, socket;
                for (url in sockets) {
                    socket = sockets[url];
                    // Closes sockets which cannot detect disconnection manually
                    if (socket.state() === "opened") {
                        socket.fire("close", "error");
                    }
                }
            });

            // Exposes portal to the global object
            //window.portal = portal;

        })(portal);

    })(share);

    // .debug (server debug);
    (function (share) {

        var _debug = function (msg) {
            share.get(1, { m: msg });
        };

        share.debug = _debug;

    })(share);

    // .load .marginTop .marginLeft .marginBottom .marginRight
    (function (share) {

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

        share.init = function (family, pageUrl, scriptUrl, cssUrl, content, title, titles, urls, data, pageData, stateData) {

            // init functions
            if (title == "") title = null;

            if (loaded == false) {
                loaded = true;

                // ajaxify                                     
                if (ajaxify && History.enabled == false) { // unfortunately, IE<10 is not supported
                    ajaxify = false;
                }
                
                // set container
                pagesContainer.css(margin);

                // resize
                share.resize("share", function (re) {

                    margin.top = parseInt(pagesContainer.css("top"));
                    margin.left = parseInt(pagesContainer.css("left"));
                    margin.right = parseInt(pagesContainer.css("right"));
                    margin.bottom = parseInt(pagesContainer.css("bottom"));

                    var topPage = pages.length > 0 ? pages[pages.length - 1] : null;
                    if (topPage != null) {
                        topPage.triggerResize(re);
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

                        History.replaceState({ family: family, data: null }, share.title(title), urlPrefix + pageUrl);
                        State = History.getState();

                        //debug("new: " + urlPrefix + pageUrl);
                        //debug(State.data);
                    }

                    if (State.data.data != null) {
                        stateData = State.data.data;
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

                        if (share.__nocallback == true) {
                            delete share.__nocallback;
                            nocallback = true;
                        }

                        if (topFamily != null) {

                            if (ofamily != topFamily) {  // different family, global link

                                if (!nocallback) {

                                    share.startLoading();

                                    $.getJSON(contentProviderUrl, { p: appHref }, function (d) {

                                        share.endLoading();

                                        share.init(d.f, appHref, d.s, d.u, d.c, d.t, d.y, d.z, d.d, d.x, stateData);
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
            if (data != null) {
                var key = null;
                $.each(data, function (i, v) { if (i % 2 == 0) key = v; else share.data(key, v); });
            }

            // create page object
            var data = {};
            if ($.isPlainObject(stateData)) {
                data = stateData;
            }
            var eventHandlers = [];
            var streamHandlers = [];
            var title = title;
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

            var resizeGuid = 0;
            var resizeHandlers = {};

            page.lastPage = last;
            page.family = family;
            page.titles = titles;
            page.urls = urls;

            page.url = function (arg1) {

                if ($.isString(arg1)) {
                    var purl = share.url(arg1);
                    var burl = null;
                    var eurl = null;

                    $.each(urls, function (i, v) {
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

                if (Array.isArray(o[0])) {
                    $.each(o[0], function (i, v) {
                        if ($.isNumber(v) || Array.isArray(v))
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
                else if (Array.isArray(arg1)) {
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
                else if (Array.isArray(d)) {
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

                        // cancel pending tooltip
                        share.cancelTooltipTimer();
                        share.closeTooltip();

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
                            share.__nocallback = true;
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
                    share.triggerResize("share", page);
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

                    $.each(resizeHandlers, function (ri, rv) {
                        page.removeResize(ri);
                    });

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
            page.error = function () { };
            page.debugEvents = function () {
                debug(eventHandlers);
            };
            page.resize = function (c, d) {
                if ($.isNumber(c) || $.isString(c)) {
                    if ($.isFunction(d)) {
                        if ($.isUndefined(resizeHandlers[c])) {
                            resizeHandlers[c] = d;
                            return c;
                        }
                    }
                }
                else if ($.isFunction(c)) {
                    resizeGuid = share.lookup(resizeGuid, resizeHandlers);
                    resizeHandlers[resizeGuid] = c;
                    return resizeGuid;
                }
            };
            page.removeResize = function () {
                var o = arguments;
                if ($.isNumber(o[0]) || $.isString(o[0])) {
                    var id = o[0];
                    if (!$.isUndefined(resizeHandlers[id])) {
                        delete resizeHandlers[id];
                    }
                }
                else if (Array.isArray(o[0])) {
                    $.each(o[0], function (i, v) {
                        removeUnload(v);
                    });
                }
            };
            page.triggerResize = function () {

                var o = arguments;

                if ($.isPlainObject(o[0])) {
                    $.each(resizeHandlers, function (ri, rv) {
                        rv(o[0]);
                    });
                }
                else if ($.isNumber(o[0]) || $.isString(o[0])) {
                    var id = o[0];
                    if (!$.isUndefined(resizeHandlers[id])) {
                        resizeHandlers[id]({ width: page.width(), height: page.height(), lastWidth: 884, lastHeight: 235, widthChanged: false, heightChanged: false });
                    }
                }
            };

            page.url(pageUrl);

            if (last != null) { last.next(page); }
            pages.push(page);
            page.css(cssUrl);

            var pageload = function () {
                var oscript = share.page(family);

                if (oscript != null) {
                    page = $.extend(true, page, oscript);
                }

                if (transferData != null) page.prototype.transfer = transferData;
                else page.prototype.transfer = null;
                transferData = null;

                var pageDataKey;
                $.each(pageData, function (pdi, pdv) {
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
                var dcontent = div(dpage).addClass("_CO").html(content);

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

            if (share.page(family) == null) {
                share.load(scriptUrl, function () {
                    pageload();
                });
            }
            else pageload();
        };
        share.page = function (name, object) {
            if ($.isPlainObject(object)) objects[name] = object;
            else if ($.isString(name)) {
                var object = objects[name];
                return $.isUndefined(object) ? null : $.extend(true, {}, object);
            }
        };
        share.script = function (name, object) {
            if ($.isFunction(object)) scripts[name] = object;
        }


        share.load = function (scriptUrl, complete) {
            $.ajax({
                url: scriptUrl, dataType: "script", cache: true, complete: function () {
                    if (complete != null) complete();
                }
            });
        };
        share.marginTop = function (s) {
            if (s == undefined) return margin.top;
            else {
                pagesContainer.css({ top: s });
                margin.top = s;
            }
        };
        share.marginBottom = function () {
            return margin.bottom;
        };
        share.marginLeft = function () {
            return margin.left;
        };
        share.marginRight = function (s, a) {
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
        share.isNew = function () {
            return _new;
        };
        share.remove = function () {
            var o = arguments;

            var v = o[0];

            if ($.isJQuery(v)) {

                v.children().each(function (ii, vv) {
                    remove($(vv));
                });

                var vo = v.data("share");

                if (vo != null) {
                    if (vo.type != "page") {
                        vo.remove();
                    }
                }
                else
                    v.remove();
            }
        };

        share.loadScript = function (name, callback) {

            if ($.isString(name) && $.isFunction(callback)) {
                var _script = scripts[name];

                if ($.isUndefined(_script)) {
                    $.getJSON(contentProviderUrl, { vk: name }, function (d) {
                        share.load(d.s, function () {
                            _script = scripts[name];
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
        share.width = function () {
            return share.page().width() + share.marginLeft() + share.marginRight();
        };
        share.height = function () {
            return share.page().height() + share.marginTop() + share.marginBottom();
        };
        share.topContainer = function () {
            return topContainer;
        };
        share.bottomContainer = function () {
            return bottomContainer;
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


        share(function () {
            ajaxify = share.system("ajaxify");
            contentProviderUrl = share.system("contentProviderUrl");
            urlPrefix = share.system("urlPrefix");

            console.log("%cCreated with love,\nby " + "%cAfis Herman Reza Devara", "font-size:20px;color:#ff0000", "font-size:20px;color:#ffcc00");
            console.log("%cNo, it's not created with a bootstrap, or whatever javascript framework thats' hot nowdays (angular, react, etc). This is created with my own front-end & back-end framework called " + "%cShare" + "%c that build together with Jovice project: https://github.com/afisd/jovice. If you are interested, just contact me, and I guess you probably know how to do so. Thanks.", "font-size:14px;color:#404040", "font-size:14px;color:#ff0000", "font-size:14px;color:#404040");
        });

    })(share);
    
    // .global
    // creates share global object that can be accessed through the application
    (function (share) {
        
        var global = function (onInit) {
            
            var page = null;
            var inited = false;

            return {
                // should be called every page in "init"
                init: function (p) {
                    page = p;

                    if (inited) {
                        return p;
                    }
                    inited = true;

                    if (onInit != null) onInit(page);

                    return page;
                }
            };
        };

        share.global = global;

    })(share);
    
    // .font .calculateFont .measureText
    (function (share) {

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
            //else if (t == "head") return headings;
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
                        var font = share.font(o.font);
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

            var f1 = share.font(f);
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

        share.font = font;
        share.measureText = measureText;
        share.parseFontFamily = parseFontFamily;

        share(function () {
            //headings = share.system("fontHeadings");
            body = share.system("fontBody");
        });

    })(share);

    // .box
    (function (share) {

        var tooltipBox = null, tooltipContents = null, tooltipArrowTop = null, tooltipArrowBot = null, tooltipTimer = null;

        var box = function (container, type) {

            if (container == null) return null;
            

            // CONTAINER ---
            var page = null, parent = null, parentBox = null,
                containerType = 0, boxEvents = [];

            var selfEventHandlers = [];

            function selfEvent() {
                var o = arguments;

                if (Array.isArray(o[0])) {
                    $.each(o[0], function (i, v) {
                        if ($.isNumber(v) || Array.isArray(v))
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
                else if (Array.isArray(arg1)) {
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
                else if (container.type == "box") { // container is a share.box descendant
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
            //var identifier = share.identifier(8);
            //var element = $("<div id=\"" + identifier + "\" class=\"_BO\" />");
            if ($.isUndefined(type)) type = "div";
            var element = $("<" + type + " class=\"_BO\" />");
            var box = function (a) {
                if ($.isPlainObject(a)) {
                    $.each(a, function (i, v) {
                        if ($.isFunction(box[i])) {
                            if (Array.isArray(v)) box[i].apply(box, v);
                            else box[i](v);
                        }
                    });
                };
                return box;
            };
            box.$ = element;
            $.data(element.get(0), "share", box);
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

                if (Array.isArray(o[0])) {
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
                        if ($.isPlainObject(a)) {
                            a.queue = false;
                            this.$.animate({ left: l }, a);
                        }
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
            box.right = function (r, a) {
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
                    else {
                        if ($.isPlainObject(a)) {
                            a.queue = false;
                            this.$.animate({ right: r }, a);
                        }
                        else
                            this.$.css({ left: "", right: r });
                    }
                }
            };
            box.leftRight = function (l, r, a) {
                if (!$.isUndefined(l) && !$.isUndefined(r)) {
                    this.right(r, a);
                    this.left(l, a);
                }
                else if (!$.isUndefined(l)) {
                    this.right(l, a);
                    this.left(l, a);
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
            box.opacity = function (o, a) {
                if ($.no(arguments)) return this.$.css("opacity");
                else {
                    if ($.isPlainObject(a))
                        this.$.animate({ opacity: o }, a);
                    else
                        this.$.css({ opacity: o });
                }
            };
            box.removeChildren = function () {
                if (box.isScroll()) {
                    var inner = $("._IB", this.$);
                    $.each(inner.children(), function (i, v) {
                        var cshare = $(this).data("share");
                        if (cshare != undefined) {
                            cshare.remove();
                        }
                    });
                    inner.empty();
                }
                else {
                    $.each(this.$.children(), function (i, v) {
                        var cshare = $(this).data("share");
                        if (cshare != undefined) {
                            cshare.remove();
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
                        this.onColor(share.color(this.onColor(), 0), o[1]);
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
                                s = share.color(o[0], o[1]);
                                if ($.isPlainObject(o[2])) a = o[2];
                            }
                            else {
                                s = share.color(o[0]);
                                if ($.isPlainObject(o[1])) a = o[1];
                            }
                        }
                    }
                    else if ($.isString(o[0])) {
                        if ($.isNumber(o[1])) {
                            s = share.color(o[0], o[1]);
                            if ($.isPlainObject(o[2])) a = o[2];
                        }
                        else {
                            s = share.color(o[0]);
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
                if (Array.isArray(e) || $.isNumber(e)) {
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
                    var handle = share.getEventHandle(v);
                    if (handle.type == e) {
                        share.triggerEvent(v, c);
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
                else if (Array.isArray(e)) {
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
                if ($.isUndefined(c.color)) c.color = 0;
                if (c == null) box.$.css(d, "");
                else box.$.css(d, c.size + "px solid " + share.color(c.color));

                if (border == null && borderLeft == null && borderTop == null && borderRight == null && borderBottom == null) box.$.css({ boxSizing: "" });
                else box.$.css({ boxSizing: c.outside == true ? "" : "border-box" });
            };

            box.noBorder = function (c) {
                if (c == false) {
                    box.$.css({ border: "" });
                }
                else {
                    border = null; borderLeft = null; borderTop = null; borderRight = null; borderBottom = null;
                    box.$.css({ border: 0 });
                }
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

            box.noOutline = function (c) {
                if (c == false) box.$.css({ outline: "" });
                else box.$.css({ outline: 0 });
            }

            box.margin = function () {

                var o = arguments;

                
                if (o.length >= 1) {

                    if (o.length == 2) {
                        box.marginLeft(o[1]);
                        box.marginRight(o[1]);
                        box.marginTop(o[0]);
                        box.marginBottom(o[0]);
                    }
                    else if (o.length == 4) {
                        box.marginLeft(o[0]);
                        box.marginRight(o[2]);
                        box.marginTop(o[1]);
                        box.marginBottom(o[3]);
                    }
                }
                else return [box.marginLeft(), box.marginTop(), box.marginRight(), box.marginBottom()];
            }

            box.marginTop = function (c) {
                if (c != undefined) {
                    if (c == null) c = "";
                    box.$.css({ marginTop: c });
                }
                else return box.$.css("marginTop");
            };
            box.marginBottom = function (c) {
                if (c != undefined) {
                    if (c == null) c = "";
                    box.$.css({ marginBottom: c });
                }
                else return box.$.css("marginBottom");
            };
            box.marginLeft = function (c) {
                if (c != undefined) {
                    if (c == null) c = "";
                    box.$.css({ marginLeft: c });
                }
                else return box.$.css("marginLeft");
            };
            box.marginRight = function (c) {
                if (c != undefined) {
                    if (c == null) c = "";
                    box.$.css({ marginRight: c });
                }
                else return box.$.css("marginRight");
            };

            var tooltipOver = null, tooltipOut = null;
            var tooltipText = null, tooltipColor = null;

            box.tooltip = function (t) {
                if (tooltipOver == null) {
                    tooltipOver = box.over(function () {

                        if (tooltipBox == null) {
                            tooltipBox = share.box(share.topContainer())({ z: 1000, hide: true });
                            tooltipContents = share.box(tooltipBox)({ color: 15 });
                            tooltipArrowTop = share.raphael(tooltipBox)({ size: [16, 16], top: 0, left: 20 });
                            tooltipArrowTop.paper().path("M0,8L16,8L8,0Z").attr({ fill: share.color(15), stroke: "none" });
                            tooltipArrowBot = share.raphael(tooltipBox)({ size: [16, 16], left: 20 });
                            tooltipArrowBot.paper().path("M0,8L16,8L8,16Z").attr({ fill: share.color(15), stroke: "none" });
                        }


                        if (tooltipTimer != null) clearTimeout(tooltipTimer);
                        tooltipTimer = setTimeout(function () {
                            tooltipBox.show();
                            tooltipBox.opacity(0);
                            tooltipBox.size(400, 250);
                            tooltipContents.size(400, 270);

                            if ($.isString(t)) {
                                tooltipText = share.text(tooltipContents)({ font: 11, position: [10, 10], color: 90, text: t });
                                tooltipText.text(t);
                                tooltipContents.width(tooltipText.computedWidth() + 20);
                                tooltipContents.height(tooltipText.computedHeight() + 20);
                                if (tooltipColor != null) {
                                    tooltipText.spanColor(tooltipColor[0], tooltipColor[1], tooltipColor[2], tooltipColor[3], tooltipColor[4]);
                                }
                            }
                            if ($.isFunction(t)) {

                            }

                            var w = tooltipContents.width();
                            var h = tooltipContents.height();
                            tooltipBox.width(w);
                            tooltipBox.height(h + 8);
                            tooltipBox.animate({ opacity: 1 }, { duration: 100 });

                            var abstop = box.absoluteTop();
                            var absleft = box.absoluteLeft();
                            var width = tooltipBox.width();
                            var height = tooltipBox.height();
                            var boxWidth = box.width();
                            var boxHeight = box.height();

                            if (absleft < 30) {
                                tooltipBox.left(10);
                            }
                            else if (absleft > (share.pageWidth() - 30)) {
                                tooltipBox.left(share.pageWidth() - width - 10);
                            }
                            else if ((absleft - 20 + width) > (share.pageWidth() - 10)) {
                                tooltipBox.left(share.pageWidth() - width - 10);
                                tooltipArrowBot.left(absleft - (share.pageWidth() - width - 10));
                                tooltipArrowTop.left(absleft - (share.pageWidth() - width - 10));
                            }
                            else {
                                tooltipBox.left(absleft - 20);
                                tooltipArrowBot.left(20);
                                tooltipArrowTop.left(20);
                            }

                            if ((abstop + height + boxHeight + 10) > (share.pageHeight() - 10)) {
                                tooltipArrowTop.hide();

                                if (absleft < 30 || absleft > (share.pageWidth() - 30)) tooltipArrowBot.hide();
                                else {
                                    tooltipArrowBot.show();
                                    tooltipArrowBot.top(height - 16);
                                }

                                tooltipContents.top(0);
                                tooltipBox.top(abstop - height);
                            }
                            else {
                                tooltipArrowBot.hide();

                                if (absleft < 30 || absleft > (share.pageWidth() - 30)) tooltipArrowTop.hide();
                                else {
                                    tooltipArrowTop.show();
                                }

                                tooltipContents.top(8);
                                tooltipBox.top(abstop + boxHeight);
                            }

                        }, 500);
                    });
                    tooltipOut = box.out(function () {
                        if (tooltipTimer != null) clearTimeout(tooltipTimer);
                        tooltipBox.hide();
                        tooltipContents.removeChildren();
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
                share.tooltipBox().hide();
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
                        var reshack = function () {
                            box.$.css({
                                top: (parentBox.height() - box.height()) / 2,
                                left: (parentBox.width() - box.width()) / 2
                            });
                        };
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

            var boxCurrentOrder = null;

            box.order = function (c) {

                var o = arguments;

                if (o.length > 0) {

                    var goodChild = [];
                    $.each(o, function (ci, cv) {
                        if (box.parentOf(cv)) {
                            goodChild.push(cv);
                        }
                    });

                    var same = true;

                    if (boxCurrentOrder != null) {
                        if (boxCurrentOrder.length != goodChild.length) same = false;
                        else {
                            $.each(boxCurrentOrder, function (ci, cv) {
                                if (goodChild[ci] == null || goodChild[ci] != cv) {
                                    same = false;
                                    return false;
                                }
                            });
                        }
                    }
                    else same = false;

                    if (same == true) return;

                    var oprev = null;
                    var oid = 0;

                    $.each(goodChild, function (gi, gv) {

                        if (oid > 0) {
                            gv.$.insertAfter(oprev.$);                            
                        }
                        oprev = gv;

                        oid++;
                    });

                    boxCurrentOrder = goodChild;

                }



            };

            box.parentOf = function (c) {
                if (c.$.parents(box.$).length > 0) {
                    return true;
                }
                else return false;
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
                    var cbox = vo.data("share");
                    if (cbox == null) return true;

                    children.push(cbox);
                });

                return children;
            };
            box.float = function (f) {
                if ($.isUndefined(f)) return this.$.css("float");
                else {
                    if (f == null) f = "";
                    this.$.css({ float: f });
                }
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
                    share(1, scrollCalculateProc);
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

                                scrollRightButton = share.box(element)({ hide: true, borderLeft: { size: 1, color: 65 }, height: "100%", width: 23, right: 0, dataTag: "right", cursor: "pointer", button: scrollButton });
                                var ric = share.icon(scrollRightButton, "arrow")({ size: [20, 20], color: 35, center: true });
                                ric.$.css({ marginRight: -3 });

                                scrollLeftButton = share.box(element)({ hide: true, borderRight: { size: 1, color: 65 }, left: 0, height: "100%", width: 23, cursor: "pointer", button: scrollButton });
                                var lic = share.icon(scrollLeftButton, "arrow")({ size: [20, 20], color: 35, center: true, flip: "horizontal" });
                                lic.$.css({ marginLeft: -3 });

                                if (!Array.isArray(scroll.horizontalStep) || scroll.horizontalStep.length == 0) scroll.horizontalStep = null;

                                if (scroll.horizontalStep != null) {
                                    scroll.horizontalStep.sort(function (a, b) { return a - b; });
                                }

                                if (scroll.horizontalStep != null) {
                                    scrollBeacon = share.box(inner)({ size: 1 });
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

                                scrollDownButton = share.box(element)({ hide: true, borderTop: { size: 1, color: 50 }, dataTag: "down", bottom: 0, width: "100%", height: 30, color: 90, opacity: 0.8, cursor: "pointer", button: scrollButton });
                                share.icon(scrollDownButton, "arrow")({ size: [25, 25], color: 35, center: true, rotation: 90 });
                                scrollUpButton = share.box(element)({ hide: true, borderBottom: { size: 1, color: 50 }, top: 0, width: "100%", height: 30, color: 90, opacity: 0.8, cursor: "pointer", button: scrollButton });
                                share.icon(scrollUpButton, "arrow")({ size: [25, 25], color: 35, center: true, rotation: 270 });
                            }
                        }
                        else {
                            var pRight = 0, pBottom = 0;
                            if (scroll.vertical && !Modernizr.touchevents) pRight = scrollSize;
                            if (scroll.horizontal && !Modernizr.touchevents) pBottom = scrollSize;
                            var vc = pRight > 0 ? $("<div class=\"_VB\" />").css({ bottom: pBottom, width: scrollSize }) : null;
                            var hc = pBottom > 0 ? $("<div class=\"_HB\" />").css({ right: pRight, height: scrollSize }) : null;
                            element.prepend(vc, hc);

                            scrollVerticalHandler = vc != null ? share.box(vc)({ width: "100%", height: 20, color: 65, opacity: 0.8, z: 5000 }) : null;
                            scrollHorizontalHandler = hc != null ? share.box(hc)({ height: "100%", width: 20, color: 65, opacity: 0.8, z: 5000 }) : null;
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

                        if (scroll.vertical) share.remove(vc);
                        if (scroll.horizontal) share.remove(hc);
                        share.remove(inner);
                        share.remove(outer);

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

                        var mx = containerType == 0 ? (share.pageWidth() - share.marginRight() - share.marginLeft()) :
                            containerType == 1 ? parentBox.isScroll() ? parentBox.scrollWidth() : parentBox.width() :
                                containerType == 2 ? parent.width() : 0;
                        var my = containerType == 0 ? (share.pageHeight() - share.marginBottom() - share.marginTop()) :
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

        share.box = box;
        share.cancelTooltipTimer = function () {
            if (tooltipTimer != null) clearTimeout(tooltipTimer);
        };
        share.closeTooltip = function () {
            if (tooltipBox != null) {
                tooltipBox.hide();
            }
        };

    })(share);

    // .textbase
    (function (share) {

        var textbase = function (container) {

            var d = share.box(container);
            if (d == null) return null;

            d.class("_FB");


            var tb = d;

            tb.refresh = function () {
            };
            tb.font = function () {
                var o = share.args(arguments, 1, "optional string font", "optional number size");

                if (o) {
                    if (o.font != null) {
                        var font = share.font(o.font);
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
                    var ff = share.parseFontFamily(d.$.css("fontFamily"));
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

                if (s == null) s = share.color(o[0], o[1]);
                if (s != null) {
                    d.$.css({ backgroundColor: s });
                }
            };
            tb.measureText = function (s) { // calculate specified text using current properties
                if (s != null) {
                    var fs = tb.font();
                    return share.measureText(s, fs.font, fs.size, tb.weight(), tb.italic());
                }
            };

            return tb;
        };

        share.textbase = textbase;

    })(share);

    // .text
    (function (share) {

        var text = function (container) {

            var d = share.textbase(container);
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
                                ca = share.color(b, c);
                                cb = d;
                            }
                            else {
                                ca = share.color(b);
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
                                if (Array.isArray(av)) ca = share.color(av[0], av[1], av[2]);
                                else ca = share.color(av);

                                if (ca == null) {
                                    if (Array.isArray(av)) ca = share.color(av[0], av[1]);
                                    else ca = share.color(av);

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
                                    var selection = window.getSelection();
                                    selection.removeAllRanges();
                                    selection.addRange(range);
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

        share.text = text;

    })(share);

    // .form
    (function (share) {

        var form = function () {
            var o = share.args(arguments, "reqsharered function page");

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

        share.form = form;

    })(share);

    // .inputbase
    (function (share) {

        var inputbase = function (container) {

            var d = share.box(container);
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
            ib.onChange = function (c) {
            };
            ib.change = function (c) {
                return d.onChange(c);
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

        share.inputbase = inputbase;

    })(share);

    // .textinput
    (function (share) {

        var textinput = function (container) {

            var d = share.inputbase(container);
            if (d == null) return null;

            var inputcon = $("<div class=\"_IC\" />");
            var input = $("<input class=\"_IT _FB\" spellcheck=\"false\" />");
            inputcon.append(input);
            d.$.append(inputcon);

            var gline = $("<div />");
            gline.css({ width: "100%", backgroundColor: share.color("accent"), height: 1, bottom: 0, position: "absolute" });
            d.$.append(gline);

            var textinput = d;

            var dheight = d.height;

            var csize;
            var pheight = null;
            function redraw(_this) {

                if (csize == null) {
                    var ff = share.parseFontFamily(input.css("fontFamily"));
                    csize = share.measureText("&nbsp;", (ff && ff.length > 0) ? ff[0] : null, size);
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
                    var ff = share.parseFontFamily(input.css("fontFamily"));
                    csize = share.measureText("&nbsp;", (ff && ff.length > 0) ? ff[0] : null, size);
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
                else {
                    input.val(v);
                    input.trigger("change");
                }
            };
            textinput.onChange = function (c) {
                return this.event(input, "change", c);
            };
            textinput.keydown = function (c) {
                return this.event(input, "keydown", c);
            };
            textinput.keyup = function (c) {
                return this.event(input, "keyup", c);
            };
            textinput.keypress = function (c) {
                return this.event(input, "keypress", c);
            };
            textinput.input = function (c) {
                return this.event(input, "input", c);
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
                return this.event(input, "focusin", d);
            };
            textinput.focusout = function (d) {
                return this.event(input, "focusout", d);
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

        share.textinput = textinput;

    })(share);

    // .textarea
    (function (share) {

        var textarea = function (container) {

            var d = share.inputbase(container);
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

        share.textarea = textarea;

    })(share);

    // .button
    (function (share) {

        var button = function (container) {

            var d = share.box(container, "button");
            if (d == null) return null;

            d({ noBorder: true, noOutline: true });

            var t = share.text(d);

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
                        d.cursor("pointer");
                        if (design == null) {
                            d.color("accent");
                        }
                        else if (design.stateChanged != null) design.stateChanged(d, t);
                    }
                    else {
                        d.cursor("default");
                        if (design == null) {
                            d.color(80);
                        }
                        else if (design.stateChanged != null) design.stateChanged(d, t);
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

            var design = null;
            button.design = function (c) {
                if ($.isPlainObject(c)) {
                    design = c;
                    if (design.normal != null) design.normal(d, t, true);
                }
                else if (c == null) {
                    design = null;
                    d.color("accent");
                    t.color(100);
                }
            };

            button.textColor = function (c, a) {
                return t.color(c, a);
            };

            d.resize(onresize);

            t({ color: 100, weight: "500", font: 14 });
            d({
                size: [300, 35], color: "accent", cursor: "pointer", button: {
                    normal: function () {
                        if (design == null) {
                            if (state == 1) d.color("accent");
                        }
                        else if (design.normal != null) design.normal(d, t);
                    },
                    over: function () {
                        if (design == null) {
                            if (state == 1) d.color("accent+25");
                        }
                        else if(design.over != null) design.over(d, t);
                    },
                    press: function () {
                        if (design == null) {
                            if (state == 1) d.color("accent+40");
                        }
                        else if (design.press != null) design.press(d, t);
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

        share.button = button;


    })(share);

    // .graphics .raphael .icon
    (function (share) {

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

                var d = share.box(container);
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
            var spath;

            switch (type) {
                case "powercut": spath = "M71.4,25.5L38.25,58.65l127.5,127.5v94.35h76.5V510l91.8-155.55L438.6,459l33.15-33.15L71.4,25.5z M420.75,204h-102l102-204h-255v56.1L382.5,272.85L420.75,204z"; break;
                case "warning": spath = "M15.789,13.982l-6.938-13C8.678,0.657,8.339,0.453,7.97,0.453H7.969c-0.369,0-0.707,0.203-0.881,0.528l-6.969,13c-0.166,0.312-0.157,0.686,0.023,0.986C0.323,15.268,0.649,15.453,1,15.453h13.906c0.352,0,0.677-0.184,0.858-0.486C15.945,14.666,15.954,14.292,15.789,13.982z M7.969,13.453c-0.552,0-1-0.448-1-1s0.448-1,1-1c0.551,0,1,0.448,1,1S8.521,13.453,7.969,13.453z M8.97,9.469c0,0.553-0.449,1-1,1c-0.552,0-1-0.447-1-1v-4c0-0.552,0.448-1,1-1 c0.551,0,1,0.448,1,1V9.469z"; break;
                case "search": spath = "M29.772,26.433l-7.126-7.126c0.96-1.583,1.523-3.435,1.524-5.421C24.169,8.093,19.478,3.401,13.688,3.399C7.897,3.401,3.204,8.093,3.204,13.885c0,5.789,4.693,10.481,10.484,10.481c1.987,0,3.839-0.563,5.422-1.523l7.128,7.127L29.772,26.433zM7.203,13.885c0.006-3.582,2.903-6.478,6.484-6.486c3.579,0.008,6.478,2.904,6.484,6.486c-0.007,3.58-2.905,6.476-6.484,6.484C10.106,20.361,7.209,17.465,7.203,13.885z"; break;
                case "tiles": spath = "M41.554,8.551v14.621H26.935V8.551H41.554z M8.446,23.172h14.62V8.551H8.446V23.172z M26.935,41.449h14.619V26.83H26.935	V41.449z M8.446,41.449h14.62V26.83H8.446V41.449z"; break;
                case "cross": spath = "M19,6.4 17.6,5 12,10.6 6.4,5 5,6.4 10.6,12 5,17.6 6.4,19 12,13.4 17.6,19 19,17.6 13.4,12 z"; break;
                case "arrow2": spath = "M16.714,33.301h66.799l-33.4,33.398L16.714,33.301z"; break;
                case "statusdown": spath = "M11.844,5.174L11.838,5.17C11.748,5.065,11.622,5,11.482,5H4.499c-0.143,0-0.27,0.069-0.361,0.176L4.136,5.174 c-0.195,0.217-0.195,0.569,0,0.786l3.499,3.877c0.195,0.218,0.511,0.218,0.706,0l0.012-0.021l3.491-3.856C12.039,5.743,12.039,5.391,11.844,5.174z"; break;
                case "refresh": spath = "M92.07,256.41H50l78.344,78.019l77.536-78.019h-39.825c-0.104-61.079,49.192-111.032,110.329-111.387c61.293-0.358,111.27,49.039,111.626,110.331c0.58,98.964-119.057,148.511-188.892,79.686l-51.929,52.687c116.154,114.483,315.773,32.415,314.809-132.804C461.4,152.769,378.105,70.441,275.952,71.037C173.955,71.632,91.725,154.47,92.07,256.41z"; break;
                case "back": spath = "M13.5,0C6.597,0,1,5.597,1,12.5S6.597,25,13.5,25S26,19.403,26,12.5S20.403,0,13.5,0ZM13.5,23.173C7.615,23.173,2.827,18.384999999999998,2.827,12.499999999999998C2.827,6.614999999999998,7.615,1.8269999999999982,13.5,1.8269999999999982C19.384999999999998,1.8269999999999982,24.173000000000002,6.614999999999998,24.173000000000002,12.499999999999998C24.173,18.385,19.385,23.173,13.5,23.173ZM14.742,18L9.242,12.5L6.383,12.5L11.883,18ZM6.383,12.5L11.883,7L14.742,7L9.242,12.5ZM10.4555,13.7L20,13.7L20,11.3L10.455,11.3L9.242,12.5Z"; break;
                case "clipboard": spath = "M432,64h-32v32h16v256H288v128H96V96h16V64H80c-8.801,0-16,7.2-16,16v416c0,8.8,7.199,16,16,16h252l116-116V80 C448,71.2,440.8,64,432,64z M320,480v-96h96L320,480z M384,64h-64V32c0-17.6-14.4-32-32-32h-64c-17.602,0-32,14.4-32,32v32h-64v64	h256V64z M288,64h-64V32.057c0.017-0.019,0.036-0.039,0.057-0.057h63.884c0.021,0.018,0.041,0.038,0.059,0.057V64z"; break;
                case "list": spath = "M41.166,9.166v5H18.105v-5H41.166L41.166,9.166z M18.105,23.055h23.061v-5H18.105V23.055z M18.105,31.943h23.061v-5H18.105 V31.943z M18.105,40.834h23.061v-5H18.105V40.834z M8.834,14.166h4.65v-5h-4.65V14.166z M8.834,23.055h4.65v-5h-4.65V23.055z M8.834,31.943h4.65v-5h-4.65V31.943z M8.834,40.834h4.65v-5h-4.65V40.834z"; break;
                case "forward": spath = "M31.416,12.568L14.429,0.38c-1.126-0.604-2.416-0.747-2.417,1.579v5.305 L2.417,0.38C1.291-0.225-0.031-0.243,0.001,2.021v23.942c0.032,2.045,1.384,2.341,2.417,1.673l9.595-6.885v5.243 c0.001,1.983,1.385,2.31,2.417,1.642l16.987-12.188C32.127,14.935,32.162,13.08,31.416,12.568z M14.014,25.208 c0-0.469,0-8.4,0-8.4l-12.012,8.4v-22.4l12.012,8.4c0,0,0-6.612,0-8.4l16.016,11.2L14.014,25.208z"; break;
                case "backtime": spath = "M53.131,0C30.902,0,12.832,17.806,12.287,39.976H0l18.393,20.5l18.391-20.5H22.506C23.045,23.468,36.545,10.25,53.131,10.25 c16.93,0,30.652,13.767,30.652,30.75S70.061,71.75,53.131,71.75c-6.789,0-13.059-2.218-18.137-5.966l-7.029,7.521 C34.904,78.751,43.639,82,53.131,82C75.703,82,94,63.645,94,41S75.703,0,53.131,0z M49.498,19v23.45l15.027,15.024l4.949-4.949 L56.5,39.55V19H49.498z"; break;
                case "timeline": spath = "M25.921,18.395c-0.965-1.358-2.853-1.681-4.215-0.713c-1.361,0.966-1.681,2.854-0.714,4.216l5.761,8.111 c-0.063,0.299-0.099,0.61-0.099,0.929c0,2.47,2,4.472,4.469,4.472c1.3,0,2.46-0.563,3.277-1.448h6.481 c1.67,0,3.023-1.354,3.023-3.023c0-1.669-1.353-3.023-3.023-3.023h-6.482c-0.694-0.752-1.638-1.254-2.703-1.392L25.921,18.395z M52.429,21.306c0.795,1.724,1.383,3.561,1.736,5.482c0.113,0.618,0.678,1.043,1.304,0.986l5.632-0.518 c0.336-0.03,0.643-0.199,0.849-0.466c0.207-0.268,0.291-0.608,0.236-0.941c-0.542-3.265-1.589-6.356-3.061-9.196 c-0.166-0.323-0.468-0.553-0.823-0.63c-0.355-0.075-0.725,0.01-1.01,0.236l-4.517,3.588C52.34,20.196,52.195,20.798,52.429,21.306z M48.224,48.299c-0.46-0.315-1.074-0.276-1.491,0.094c-3.634,3.226-8.28,5.334-13.399,5.777v-2.716	c0-1.114-0.902-2.015-2.015-2.015c-1.113,0-2.016,0.902-2.016,2.015v2.714C18.087,53.2,9.132,44.246,8.164,33.029h2.715 c1.113,0,2.015-0.901,2.015-2.015c0-1.114-0.902-2.015-2.015-2.015H8.156C9.066,18.462,17.031,9.93,27.292,8.131 c0.58-0.102,1.004-0.604,1.004-1.193V1.213c0-0.35-0.152-0.685-0.418-0.915c-0.265-0.23-0.617-0.335-0.965-0.285 C11.702,2.153,0,15.215,0,31.014c0,17.297,14.022,31.319,31.319,31.319c8.497,0,16.183-3.403,21.823-8.899 c0.26-0.254,0.393-0.61,0.362-0.972c-0.032-0.362-0.222-0.693-0.522-0.898L48.224,48.299z M35.346,8.129c0.792,0.138,1.57,0.315,2.331,0.531c0.583,0.165,1.194-0.124,1.439-0.677l2.298-5.182 c0.14-0.315,0.138-0.675-0.004-0.99C41.269,1.498,41,1.256,40.671,1.151c-1.597-0.506-3.247-0.894-4.943-1.138 c-0.348-0.049-0.702,0.055-0.967,0.285c-0.266,0.23-0.418,0.565-0.418,0.917v5.718C34.342,7.525,34.765,8.028,35.346,8.129z M47.822,14.677c0.435,0.443,1.132,0.484,1.618,0.099l4.438-3.527c0.267-0.211,0.432-0.526,0.456-0.866 c0.024-0.341-0.097-0.673-0.332-0.921c-1.615-1.697-3.418-3.21-5.377-4.511c-0.297-0.197-0.667-0.254-1.011-0.154 c-0.343,0.1-0.625,0.346-0.769,0.673l-2.32,5.23c-0.231,0.524-0.068,1.138,0.395,1.472C45.956,12.926,46.925,13.766,47.822,14.677z M62.054,33.694c-0.256-0.262-0.615-0.396-0.979-0.362l-5.755,0.529c-0.549,0.049-0.993,0.463-1.085,1.006 c-0.431,2.569-1.285,4.99-2.487,7.198c-0.297,0.545-0.135,1.228,0.378,1.579l4.671,3.206c0.28,0.193,0.628,0.26,0.959,0.183 c0.331-0.077,0.614-0.289,0.782-0.584c2.017-3.549,3.357-7.527,3.852-11.766C62.433,34.318,62.309,33.956,62.054,33.694z"; break;
                case "penbox": spath = "M497.944,14.059c18.75,18.75,18.719,49.141,0,67.891l-22.625,22.625l-67.906-67.891l22.625-22.625C448.787-4.675,479.194-4.691,497.944,14.059z M158.537,285.591l-22.609,90.5l90.5-22.625L452.694,127.2l-67.906-67.891 L158.537,285.591z M384.006,241.153v206.844h-320v-320h206.859l63.984-64H0.006v448h448V177.137L384.006,241.153z"; break;
                case "leftarrow": spath = "M20,11H7.8l5.6-5.6L12,4l-8,8l8,8l1.4-1.4L7.8,13H20V11z"; break;

                case "hex": spath = "M50.518,188.535h101.038l50.515-87.5l-50.515-87.5H50.518L0,101.035L50.518,188.535z M59.178,28.535h83.718l41.854,72.5l-41.854,72.5H59.178l-41.858-72.5L59.178,28.535z"; break;
                case "glass": spath = "M480.606,459.394L352.832,331.619c30.306-35.168,48.654-80.918,48.654-130.876C401.485,90.053,311.433,0,200.743,0S0,90.053,0,200.743s90.053,200.743,200.743,200.743c49.958,0,95.708-18.348,130.876-48.654l127.775,127.775L480.606,459.394zM30,200.743C30,106.595,106.595,30,200.743,30s170.743,76.595,170.743,170.743s-76.595,170.743-170.743,170.743S30,294.891,30,200.743z"; break;
                case "arrow": spath = "M10,6L8.6,7.4L13.2,12L8.6,16.6L10,18L16,12Z"; break;
                case "close": spath = "m22,0c-12.2,0-22,9.8-22,22s9.8,22 22,22 22-9.8 22-22-9.8-22-22-22zm3.2,22.4l7.5,7.5c0.2,0.2 0.3,0.5 0.3,0.7s-0.1,0.5-0.3,0.7l-1.4,1.4c-0.2,0.2-0.5,0.3-0.7,0.3-0.3,0-0.5-0.1-0.7-0.3l-7.5-7.5c-0.2-0.2-0.5-0.2-0.7,0l-7.5,7.5c-0.2,0.2-0.5,0.3-0.7,0.3-0.3,0-0.5-0.1-0.7-0.3l-1.4-1.4c-0.2-0.2-0.3-0.5-0.3-0.7s0.1-0.5 0.3-0.7l7.5-7.5c0.2-0.2 0.2-0.5 0-0.7l-7.5-7.5c-0.2-0.2-0.3-0.5-0.3-0.7s0.1-0.5 0.3-0.7l1.4-1.4c0.2-0.2 0.5-0.3 0.7-0.3s0.5,0.1 0.7,0.3l7.5,7.5c0.2,0.2 0.5,0.2 0.7,0l7.5-7.5c0.2-0.2 0.5-0.3 0.7-0.3 0.3,0 0.5,0.1 0.7,0.3l1.4,1.4c0.2,0.2 0.3,0.5 0.3,0.7s-0.1,0.5-0.3,0.7l-7.5,7.5c-0.2,0.1-0.2,0.5 3.55271e-15,0.7z"; break;

                default: spath = type;
            };

            var d = share.raphael(container);
            var paper = d.paper();
            var path = paper.path(spath).attr({ stroke: "none" });

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
                    path.attr({ stroke: share.color(c.color), "stroke-width": c.size });
                }
                else {
                    return { color: path.attr("stroke"), size: path.attr("stroke-width") };
                }
            };

            d.color("accent");

            return d;
        };
        share.raphael = function (container) {
            return graphics(container, "raphael");
        };
        share.graphics = graphics;
        share.icon = icon;

    })(share);

    // .performance
    (function (share) {

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

        share.performance = performance_;

    })(share);

    // .section
    (function (share) {


        var _section = function (container) {

            var article = null;
            var first = false;

            if ($.isJQuery(container.$)) {

                if (container.data("sectionArticle") == null) {

                    var articleContainer = share.box(container)({ width: "100%", height: "100%", scroll: { horizontal: false } });
                    article = share.box(articleContainer);
                    article.css({ margin: "0 auto" });
                    article.relative();
                    container.data("sectionArticle", article);
                    first = true;
                }
                else
                    article = container.data("sectionArticle");
            }
            
            if (article == null) return;

            var sectionLevel = 0;

            var containerSectionLevel = container.data("sectionLevel");
            if (containerSectionLevel != null) sectionLevel = containerSectionLevel + 1;

            if (!first) {
                var separator = $$.box(article)({
                    height: 1,
                    marginLeft: 30,
                    marginRight: 30,
                    color: $$.color(85),
                    relative: true,
                    marginTop: 20
                });
            }

            var title = share.text(article)({
                marginLeft: 30,
                marginRight: 30,
                marginTop: 20,
                relative: true
            });

            if (sectionLevel == 0) {
                title({ weight: "300", font: 35, color: 20 });
            }
            else if (sectionLevel == 1) {
                title({ font: 20, color: 30 });
            }

            var body = share.box(article)({ relative: true });

            var d = body;

            var section = d;
            section.title = function (c) {
                title.text(c);
            };
            section.content = function (ar) {
                var previous;
                $.each(ar, function (ci, cv) {

                    if ($.isString(cv)) {
                        //var p = $("<p>").html(cv);
                        //body.$.append(p);
                        var utext = share.text(body)({ text: cv, relative: true, marginTop: 10 });
                        previous = utext.$;
                    }
                    else if ($.isJQuery(cv.$)) {
                        cv.relative();
                        cv.marginTop(20);
                        cv.$.insertAfter(previous);
                        //body.$.append(cv.$);
                    }

                });
            };

            section.data("sectionLevel", sectionLevel);

            return d;

        };
        var _articleWidth = function (container, s) {
            var article = null;

            if ($.isJQuery(container.$) && container.data("sectionArticle") != null) {
                article = container.data("sectionArticle");
            }

            if (article == null) return;

            article.width(s);
        };


        share.section = _section;
        share.articleWidth = _articleWidth;


    })(share);


    // .startLoading, .endLoading
    (function (share) {

        var inited = false;
        var box;
        var loadingStarted = false;
        var endLoadingTimer = null;

        function boxExpand() {
            box.$.css({ width: "0%", left: "0%" }).animate({ width: "100%" }, { duration: 500, queue: false, complete: function () { boxCollapse(); } });
        };
        function boxCollapse() {
            box.$.css({ width: "100%", left: "0%" }).animate({ left: "100%", width: "0%" }, { duration: 500, queue: false, complete: function () { boxExpand(); } });
        };

        share.startLoading = function () {

            if (!inited) {
                box = share.box(share.topContainer())({ color: "accent", opacity: 0, height: 2, left: "0%", width: "0%", z: 600 });
                inited = true;
            }

            if (!loadingStarted) {
                loadingStarted = true;
                box.opacity(0.5);
                boxExpand();
            }
        };
        share.endLoading = function () {
            if (loadingStarted) {
                endLoadingTimer = setTimeout(function () {
                    box.$.animate({ opacity: 0 }, {
                        duration: 300, queue: false, complete: function () {
                            box.$.stop();
                            loadingStarted = false;
                        }
                    });
                }, 500);
            }
        };

    })(share);

    (function (share) {

        share.isVisible = function () {
            return !document.hidden;
        };
        share.isActive = function () {
            return document.hasFocus();
        };

    })(share);


    // .explore
    (function (share) {

        var exploreInited = false;

        var topBar, title;
        var searchBox, searchTextInput;

        var conf;

        var _explore = function (t) {

            if (t == null) t = {};
            conf = t;

            if (exploreInited) return;
            exploreInited = true;

            topBar = share.box(share.topContainer())({ color: 98, height: 48, width: "100%", borderBottom: { size: 1, color: 80 } });
            share.marginTop(48);
            
            var logoButton = share.box(topBar)({ left: 15, top: 4, size: 40, cursor: "pointer", click: function () { share.page().transfer("/"); } });
            var ileft = share.icon(logoButton, "hex")({ size: 18, color: 55, position: [3, 17], rotation: 90 });
            var iright = share.icon(logoButton, "hex")({ size: 18, color: 55, position: [16.25, 17], rotation: 90 });
            var itop = share.icon(logoButton, "hex")({ size: 18, color: 55, position: [9.5, 5.5], rotation: 90 });

            logoButton.hover(function () {
                itop.color(25, { duration: 166 });
                ileft.color("accent", { duration: 166 });
                iright.color(35, { duration: 166 });
            }, function () {
                itop.color(55, { duration: 166 });
                ileft.color(55, { duration: 166 });
                iright.color(55, { duration: 166 });
            });

            title = $$.text(topBar)({ text: t.title != null ? t.title : "aphysoft.share", position: [60, 12], font: 21, color: 45, weight: "300" });
            
            searchBox = share.box(topBar)({ color: 95, height: 34, width: 250, cursor: "text", top: 6, right: 90 });
            searchTextInput = share.textinput(searchBox)({ font: 14, leftRight: [60, 10], top: 0, height: 34, design: false });
            var searchGlassIcon = share.icon(searchBox, "glass")({ size: [24, 24], color: 55, position: [20, 5] });
            var searchPlaceholder = share.text(searchBox)({ noSelect: true, text: "Search", font: ["body", 14], color: 55, position: [60, 8] });

            var searchButton = share.button(topBar)({
                size: [34, 34], right: 70, top: 6, design: {
                    normal: function (d) { d.color("transparent"); },
                    over: function (d) { },
                    press: function (d) { }
                }, hide: true,
                click: function () {                    
                    if (!searchExpanded) {
                        searchExpanded = true;
                        searchButton.hide();
                        searchBox.show();
                        searchBox.color(102);
                        searchBox.width(null);
                        var wx = topBar.width();
                        searchBox.opacity(0);
                        searchBox.leftRight(wx / 2, wx / 2);
                        searchBox.leftRight(60, 60, { duration: 100 });
                        searchBox.opacity(1, { duration: 100 });
                        searchTextInput.focus();

                        accountArea.fadeOut(100);
                        accountArea.$.css({ x: 0 }).animate({ x: 20 }, { duration: 50 });

                        searchCloseButton.show();
                        searchCloseButton.$.css({ opacity: 0, x: -20 }).animate({ opacity: 1, x: 0 }, { duration: 100 });
                    }
                }
            });
            share.icon(searchButton, "glass")({ size: [24, 24], position: [5, 5], color: 55 });

            var searchCloseIcon;
            var searchCloseButton = share.button(topBar)({
                size: [34, 34], right: 20, top: 6, design: {
                    normal: function (d) {
                        d.color("transparent");
                        //searchCloseIcon.color(55, { duration: 100 });
                    },
                    over: function (d) {
                        //searchCloseIcon.color(75, { duration: 100 });
                    },
                    press: function (d) { }
                }, hide: true, click: function () {
                    searchExpanded = false;

                    var wx = topBar.width();
                    searchBox.opacity(0, { duration: 100 });
                    searchBox.leftRight(wx / 2, wx / 2, { duration: 100 });
                    searchButton.fadeIn(100);
                    accountArea.fadeIn(100);
                    accountArea.$.css({ x: 20 }).animate({ x: 0 }, { queue: false, duration: 50 });

                    searchCloseButton.$.css({ opacity: 1, x: 0 }).animate({ opacity: 0, x: -20 }, { duration: 100, complete: function () { searchCloseButton.hide(); } });
                }
            });
            searchCloseIcon = share.icon(searchCloseButton, "close")({ size: [28, 28], position: [3, 3], color: 55 });

            var accountArea = share.box(topBar)({
                width: 70, right: 0, height: 48
            });
            var accountSignIn = share.button(accountArea)({
                size: [60, 34], design: {
                    normal: function (d, t, init) {
                        if (init) d.color("transparent");
                        else {
                            d.color("transparent", { duration: 166 });
                        }
                    },
                    over: function (d) {
                        d.color(95, { duration: 166 });
                    },
                    press: function (d) {

                    }
                }, top: 6, right: 10, text: "SIGN IN", textColor: 80, disable: true
            });

            var close = share.box(share.topContainer())({ color: 95, width: "100%", z: 999 });

            var frontLogoDone = false;
            var frontLogoElements;
            var frontLogo = share.box(close)({ size: [200, 160] });

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
                var area = share.raphael(frontLogo)({ size: frontLogo.size() });
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
                var el1 = paper.path(paths1[0]).attr({ stroke: share.color(25), opacity: 0 });
                var el2 = paper.path(paths2[0]).attr({ stroke: share.color("accent"), opacity: 0 });
                var el3 = paper.path(paths3[0]).attr({ stroke: share.color(55), opacity: 0 });

                frontLogoElements = [el1, el2, el3];

                frontLogoHexaAnimRec(el1, paths1, 1);
                frontLogoHexaAnimRec(el2, paths2, 1);
                frontLogoHexaAnimRec(el3, paths3, 1);
                el1.animate({ opacity: 1 }, 516);
                el2.animate({ opacity: 1 }, 516);
                el3.animate({ opacity: 1 }, 516);
            };

            frontLogoHexaAnim();

            var frontTitle = share.text(close)({ weight: "400", font: 18, color: 60, hide: true });
            var frontAdditional = share.text(close)({ font: 13, color: 60, hide: true }); 
            
            var searchExpanded = false;

            searchTextInput.focusin(function () {
                if (!searchExpanded) {
                    searchExpanded = true;
                    searchBox.color(102, { duration: 100 });
                    searchBox.width(topBar.width() - 180, {
                        duration: 100, complete: function () {
                            searchBox.width(null);
                            searchBox.leftRight(90, 90);
                        }
                    });
                    if (showTitle) title.$.delay(100).fadeOut(50);
                }
            });
            searchTextInput.focusout(function () {
                if (share.isActive()) {                    
                    if (narrowSearch) {
                    }
                    else {
                        if (searchExpanded) {
                            searchExpanded = false;
                            searchBox.color(95, { duration: 100 });
                            searchBox.left(null);
                            searchBox.width(topBar.width() - 180);
                            searchBox.width(250, { duration: 100 });
                            if (showTitle) title.$.delay(100).fadeIn(50);
                            if (searchTextInput.value().trim() == "") {
                                searchTextInput.value(null);
                                searchPlaceholder.show();
                            }
                        }
                    }
                }



            });
            searchBox.down(function () { searchTextInput.focus(); });
            function checkPlaceholder() {
                var s = searchTextInput.value();
                if (s == "") searchPlaceholder.show();
                else searchPlaceholder.hide();
            }
            searchTextInput.input(checkPlaceholder);
            searchTextInput.change(checkPlaceholder);
            searchTextInput.keydown(function (c) {                
                if (c.key == 13) {
                    var s = searchTextInput.value();
                    s = s.replace(/ +(?= )/g, '');
                    s = s.replaceAll(' ', '+');
                    s = escape(s);
                    if (t.search != null) t.search(s);
                }
            });                        
           
            var resizeInited = false;

            var showTitle, narrowSearch;

            var closeLayoutRatio;

            var resizeID = share.resize(function (d) {

                var init = false;
                if (!resizeInited) init = true;

                var _showTitle = false;
                var _narrowSearch = false;

                if (d.width.gte(1000)) {
                    _showTitle = true;
                    _narrowSearch = false;
                }
                else if (d.width.between(720, 1000)) {
                    _showTitle = true;
                    _narrowSearch = false;
                }
                else {
                    _showTitle = false;
                    _narrowSearch = true;
                }

                if (showTitle != _showTitle) {
                    showTitle = _showTitle;
                                        
                    if (showTitle) {
                        if (init) title.show();
                        else {
                            if (!searchExpanded) {
                                title.fadeIn(50);
                            }
                        }
                    }
                    else {
                        if (init) title.hide();
                        else title.fadeOut(50);
                    }
                }
                if (narrowSearch != _narrowSearch) {
                    narrowSearch = _narrowSearch;

                    if (!narrowSearch) {
                        if (init) {
                            searchBox.show();
                            searchButton.hide();
                        }
                        else {
                            if (searchExpanded) {                                
                                if (!searchTextInput.isFocus()) {
                                    searchTextInput.focus();
                                }
                                searchBox.leftRight(90, 90, { duration: 100 });
                                accountArea.fadeIn(50);
                                accountArea.$.css({ x: 20 }).animate({ x: 0 }, { queue: false, duration: 50 });
                                searchCloseButton.$.css({ opacity: 1, x: 0 }).animate({ opacity: 0, x: -20 }, { duration: 100, complete: function () { searchCloseButton.hide(); } });
                            }
                            else {
                                searchBox.color(95);
                                searchBox.width(250);
                                searchBox.right(90);
                                searchBox.show();
                                searchBox.$.css({ opacity: 0 }).animate({ opacity: 1 }, { duration: 100 });                             
                                searchButton.fadeOut(50);
                            }
                        }
                    }
                    else {
                        if (init) {                            
                            searchBox.hide();
                            searchButton.show();
                        }
                        else {
                            if (searchExpanded) {
                                searchBox.leftRight(60, 60, { duration: 100 });
                                accountArea.fadeOut(50)
                                accountArea.$.css({ x: 0 }).animate({ x: 15 }, { queue: false, duration: 50 });
                                searchCloseButton.show();
                                searchCloseButton.$.css({ opacity: 0, x: -20 }).animate({ opacity: 1, x: 0 }, { duration: 100 });
                            }
                            else {
                                searchBox.fadeOut(50);
                                searchButton.fadeIn(50);
                            }
                        }
                    }
                }

                if (!resizeInited) resizeInited = true;

                if (close.isShown()) {
                    close.size(d.width, d.height);

                    frontLogo.left((close.width() - frontLogo.width()) / 2);

                    closeLayoutRatio = (d.width < 300 ? 0 : d.width > 1000 ? 700 : d.width - 300) / 700;
                    var oscale = 0.4 + (closeLayoutRatio * 0.6);
                    var ostrokew = 0.8 + (closeLayoutRatio * 1.2);

                    if (!narrowSearch && frontLogoDone) {
                        oscale = oscale / 1.8;
                        ostrokew = ostrokew - 0.4;
                    }

                    $.each(frontLogoElements, function (eli, el) {
                        el.transform("s" + oscale + "," + oscale + ",100,20");
                        el.attr("stroke-width", ostrokew);
                    });
                    frontTitle.font(17 + (closeLayoutRatio * 10));

                    if (d.width.gte(720)) {
                        frontLogo.top((close.height() - frontLogo.height()) / 2);
                        frontTitle.position((close.width() - frontTitle.width()) / 2, (close.height() - frontTitle.height()) / 2 - 20 + (closeLayoutRatio * 50));                        
                    }
                    else {
                        frontLogo.top(50);
                        frontTitle.position((close.width() - frontTitle.width()) / 2, 125 + (closeLayoutRatio * 75));                        
                    }
                    frontAdditional.position((close.width() - frontAdditional.width()) / 2, frontTitle.topHeight() + 5);
                }

            });

            var firstTime = true;

            share.stream(function (type, data) {

                if (type == "online" || type == "streamrecovered") {
                    if (firstTime) {
                        firstTime = false;
                        share(function () { }, 100, function () {
                            if (!frontLogoDone) return -1;
                            else close.fadeOut(100);
                        });
                    }
                    else
                        close.fadeOut(100);
                }
                else if (type == "offline" || type == "streamerror") {
                    if (firstTime) {
                        firstTime = false;

                        if (!narrowSearch) {
                            var oscale = (0.4 + (closeLayoutRatio * 0.6)) / 1.8;
                            var ostrokew = 0.8 + (closeLayoutRatio * 1.2) - 0.4;

                            $.each(frontLogoElements, function (eli, el) {
                                el.animate({
                                    transform: "s" + oscale + "," + oscale + ",100,20", "stroke-width": ostrokew, stroke: share.color(55) }, 500, "easeInOut");
                            });
                        }
                        else  $.each(frontLogoElements, function (eli, el) { el.animate({ stroke: share.color(55) }, 500, "easeInOut"); });

                        frontTitle({ text: "SERVER&nbsp;OFFLINE", show: true, opacity: 0 });

                        if (!narrowSearch) share(600, function () { frontTitle.opacity(1, { duration: 300 }); });
                        else  frontTitle.opacity(1, { duration: 300 });
                    }
                    else {
                        frontAdditional.hide();
                        close.fadeIn(100);
                        $.each(frontLogoElements, function (eli, el) { el.attr({ stroke: share.color(55) }); });
                        frontTitle({ text: "SERVER&nbsp;OFFLINE", show: true, opacity: 1 });

                        $(':focus').blur();
                    }

                    share.triggerResize(resizeID);
                }
                else if (type == "update") {
                    frontAdditional.show();
                    if (!close.isShown())
                        close.fadeIn(100);
                    
                    $.each(frontLogoElements, function (eli, el) { el.attr({ stroke: share.color(55) }); });
                    frontAdditional({ text: "Please refresh your browser" });
                    frontTitle({ text: "NEW&nbsp;UPDATE&nbsp;IS&nbsp;AVAILABLE", show: true, opacity: 1 });

                    $(':focus').blur();

                    share.triggerResize(resizeID);
                }
            });
        };
        var _setSearchText = function (t) {
            if (exploreInited) {
                searchTextInput.value(t);
            }
        };
        var _search = function (s) {
            conf.search(s);
        };

        share.explore = _explore;
        share.setSearchText = _setSearchText;
        share.search = _search;

    })(share);

})(window, jQuery);