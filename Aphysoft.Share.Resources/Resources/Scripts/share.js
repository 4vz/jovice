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
            else if ($.isArray(o[0])) {
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
            else if ($.isArray(o[0])) {
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
            else if ($.isArray(o[0])) {
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
            if ($.isArray(array1) && $.isArray(array2)) {
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

            if ($.isArray(s)) {

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
            else if ($.isArray(o[0])) {
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

})(window, jQuery);