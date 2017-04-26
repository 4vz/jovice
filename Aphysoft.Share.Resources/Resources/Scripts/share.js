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
        var isLoading = function () {
            return (state < 2) ? true : false;
        };

        var version = "1.0";
        share.prototype = new Object();
        share.prototype.share = "Aphysoft Share " + version + " by Afis Herman Reza Devara";
        share.toString = function () {
            return "Aphysoft Share " + version;
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

        share.isLoading = isLoading;
        $.toString = function () {
            return "jQuery " + $.prototype.jquery;
        };        

        window.share = share;
        window.$$ = share;

        $(function () {
            state = 1;
        });

        $.window.on("load", function () {
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
            $$(1, c);
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
                agespan = $$.date() - d;
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

            if (agespansec < 30) ret = $$.format(mas, $$.format(lang.s, Math.round(agespansec)));
            else if (agespansec < 90) ret = $$.format(mas, lang.m);
            else if (agespansec < 3600) ret = $$.format(mas, $$.format(lang.mm, Math.round(agespansec / 60)));
            else if (agespansec < 5400) ret = $$.format(mas, lang.h);
            else if (agespansec < 84600) ret = $$.format(mas, $$.format(lang.hh, Math.round(agespansec / 3600)));
            else if (agespansec < 129600) ret = p == -1 ? lang.dn : lang.dp;
            else if (agespansec < 2547900) ret = $$.format(mas, $$.format(lang.dd, Math.round(agespansec / 86400)));
            else if (agespansec < 3888000) ret = p == -1 ? lang.Mn : lang.Mp;
            else if (agespansec < 29808000) ret = $$.format(mas, $$.format(lang.MM, Math.round(agespansec / 2592000)));
            else if (agespansec < 47304000) ret = p == -1 ? lang.yn : lang.yp;
            else ret = $$.format(mas, $$.format(lang.yy, Math.round(agespansec / 31536000)));

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

            var sw = $$.stopwatch();

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

    // .isWebFont .isContainWebFont
    (function (share) {

        var preloadfonts = [];
        var webfonts = [];

        $.each(document.styleSheets[0].cssRules, function (i, v) {

            if (v instanceof CSSFontFaceRule) {
                var t = v.cssText ? v.cssText : v.style.cssText;

                var family, weight;
                $.each(t.substring(t.indexOf("{") + 1, t.indexOf("}")).split(";"), function (tsi, tsv) {
                    if (tsv.indexOf(":") > -1) {
                        var ps = tsv.split(":");
                        var psk = ps[0].trim();
                        if (psk == "src") return 1;
                        var psv = ps[1].trim();
                        if (psk == "font-family") family = psv;
                        else if (psk == "font-weight") weight = psv;
                    }
                });

                if (family != null) {
                    var av = $("<div />");
                    $(document.body).append(av);
                    av.html("loading");
                    av.css("visibility", "hidden");
                    av.css("font-family", family);

                    if (weight != null) av.css("font-weight", weight);

                    preloadfonts.push(av);
                    webfonts.push({
                        family: family.substring(1, family.length - 1),
                        weight: weight != null ? weight : null
                    });
                }
            }
        });

        var isWebFont = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                var iswf = false;
                $.each(webfonts, function (i, v) {
                    if (v.family == o[0]) {
                        iswf = true;
                        return false;
                    }
                });
                return iswf;
            }
            else return false;
        };
        var isContainWebFont = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                var iswf = false;
                $.each(webfonts, function (i, v) {
                    if (o[0].indexOf(v.family) > -1) {
                        iswf = true;
                        return false;
                    }
                });
                return iswf;
            }
            else return false;
        };

        share.isWebFont = isWebFont;
        share.isContainWebFont = isContainWebFont;

        share(function () {
            $.each(preloadfonts, function (i, v) { v.remove(); });
        });

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

        var color = function () {
            var o = arguments;

            if ($.isString(o[0])) {
                var hex = null, com;
                var s = o[0].toLowerCase().trim();
                if (colors[s] != null) hex = colors[s];
                else if (s.length == 3 && $.isColor("#" + s)) hex = "#" + s;
                else if (s.length == 4 && s[0] == "#" && s.isColor(s)) hex = s;
                else if (s.length == 6 && $.isColor("#" + s)) hex = "#" + s;
                else if (s.length == 7 && s[0] == "#" && $.isColor(s)) hex = s;
                else if (com = s.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/)) hex = "#" + dec2hex(com[1]) + dec2hex(com[2]) + dec2hex(com[3]);

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
            else if ($.isNumber(o[0]) && $.isNumber(o[1]) && $.isNumber(o[2])) {
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

        share.color = color;

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

        var sizeGroups;

        var group = -1;
        var lastGroup = null;

        var _sizing = false;

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
                o[0]({ width: ww, height: wh, lastWidth: ww, lastHeight: wh, widthChanged: false, heightChanged: false });
                return guid;
            }
        };
        var doResize = function () {
            var o = arguments;

            if ($.isNumber(o[0]) || $.isString(o[0])) {
                var id = o[0];
                if (o.length == 1) {
                    if (handlers[id] != null) handlers[id]();
                }
                else {
                    if (handlers[id] != null) handlers[id](o[1]);
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

        var sizing = function () {
            return _sizing;
        };
        var sizeGroup = function () {
            if (group == -1) return false;
            else if (group >= 0) return group;
        };
        var minimumSize = function () {
            return sizeGroups[0];
        };

        var pendingForSizing = false;

        function refresh() {
            lastWidth = width;
            lastHeight = height;
            width = $.window.width();
            height = $.window.height();
            _isFullScreen = width == screen.width && height >= (screen.height - 1/*firefox 1px bug*/);

            group = false;
            //debug(sizeGroups);
            //debug(width);
            $.each(sizeGroups, function (i, v) {
                //debug(width + " <= " + v + " ?");
                if (width <= v) {
                    group = i;
                    //debug("ok group: " + group);
                    return false;
                }
            });

            //if (group === false)
            //    debug("ok group: false");

            if (lastGroup !== null && lastGroup !== group) _sizing = true;
            else _sizing = false;

            //debug("lastgroup: " + lastGroup + ", sizing: " + _sizing);

            lastGroup = group;
            pendingForSizing = true;

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
        share.doResize = doResize;
        share.removeResize = removeResize;
        share.isFullScreen = isFullScreen;
        share.pageWidth = pageWidth;
        share.pageHeight = pageHeight;

        share.sizing = sizing;
        share.sizeGroup = sizeGroup;
        share.minimumSize = minimumSize;

        $.window.resize(function (a) {
            var obj = refresh();
            $.each(handlers, function (bi, bv) {
                bv(obj);
            });
        });

        $(function () {
            sizeGroups = share.system("sizeGroups");
            sizeGroups.sort(function (a, b) { return a - b });
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
                        o.type == "keydown" || o.type == "keyup" || o.type == "keypress" ||
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
                                        $$.removeResize(data.window);
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
                        $$.removeResize(data.window);
                    }
                    data.window = $$.resize(function (v) {
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
                                    $$.removeResize(data.window);
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

        // 1: keydown, 2: keyup, 3: keypress
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

                var registerAdd = $$.diff(registers, serverRegisters);
                var registerRemove = $$.diff(serverRegisters, registers);

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
                    $$.post(10, { x: modifications.join() });

                serverRegisters = registers.slice();
            }
        };
        function start() {
            isAvailable = -1;
            instance = portal.open("", {
                credentials: true,
                transports: ["streamxhr"],
                urlBuilder: function () {
                    var urlHead = share.protocol() + "://" + streamDomain;
                    return urlHead + streamPath + "?c=" + share.client() + "&_=" + share.date().getTime();
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
                                        $$.post(11, { x: registers.join() });
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
            $$.get(1, { m: msg });
        };

        share.debug = _debug;

    })(share);

})(window, jQuery);