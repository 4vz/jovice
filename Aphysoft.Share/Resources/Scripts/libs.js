/*! History.js jQuery Adapter (updated 6/1/2014) */
(function (window, undefined) {
    "use strict";

    // Localise Globals
    var
		History = window.History = window.History || {},
		jQuery = window.jQuery;

    // Check Existence
    if (typeof History.Adapter !== 'undefined') {
        throw new Error('History.js Adapter has already been loaded...');
    }

    // Add the Adapter
    History.Adapter = {
        /**
		 * History.Adapter.bind(el,event,callback)
		 * @param {Element|string} el
		 * @param {string} event - custom and standard events
		 * @param {function} callback
		 * @return {void}
		 */
        bind: function (el, event, callback) {
            jQuery(el).bind(event, callback);
        },

        /**
		 * History.Adapter.trigger(el,event)
		 * @param {Element|string} el
		 * @param {string} event - custom and standard events
		 * @param {Object=} extra - a object of extra event data (optional)
		 * @return {void}
		 */
        trigger: function (el, event, extra) {
            jQuery(el).trigger(event, extra);
        },

        /**
		 * History.Adapter.extractEventData(key,event,extra)
		 * @param {string} key - key for the event data to extract
		 * @param {string} event - custom and standard events
		 * @param {Object=} extra - a object of extra event data (optional)
		 * @return {mixed}
		 */
        extractEventData: function (key, event, extra) {
            // jQuery Native then jQuery Custom
            var result = (event && event.originalEvent && event.originalEvent[key]) || (extra && extra[key]) || undefined;

            // Return
            return result;
        },

        /**
		 * History.Adapter.onDomLoad(callback)
		 * @param {function} callback
		 * @return {void}
		 */
        onDomLoad: function (callback) {
            jQuery(callback);
        }
    };

    // Try and Initialise History
    if (typeof History.init !== 'undefined') {
        History.init();
    }

})(window);

/*! History.js Core (updated 6/1/2014) */
(function (window, undefined) {
    "use strict";

    // ========================================================================
    // Initialise

    // Localise Globals
    var
		console = window.console || undefined, // Prevent a JSLint complain
		document = window.document, // Make sure we are using the correct document
		navigator = window.navigator, // Make sure we are using the correct navigator
		sessionStorage = false, // sessionStorage
		setTimeout = window.setTimeout,
		clearTimeout = window.clearTimeout,
		setInterval = window.setInterval,
		clearInterval = window.clearInterval,
		JSON = window.JSON,
		alert = window.alert,
		History = window.History = window.History || {}, // Public History Object
		history = window.history; // Old History Object

    try {
        sessionStorage = window.sessionStorage; // This will throw an exception in some browsers when cookies/localStorage are explicitly disabled (i.e. Chrome)
        sessionStorage.setItem('TEST', '1');
        sessionStorage.removeItem('TEST');
    } catch (e) {
        sessionStorage = false;
    }

    // MooTools Compatibility
    JSON.stringify = JSON.stringify || JSON.encode;
    JSON.parse = JSON.parse || JSON.decode;

    // Check Existence
    if (typeof History.init !== 'undefined') {
        throw new Error('History.js Core has already been loaded...');
    }

    // Initialise History
    History.init = function (options) {
        // Check Load Status of Adapter
        if (typeof History.Adapter === 'undefined') {
            return false;
        }

        // Check Load Status of Core
        if (typeof History.initCore !== 'undefined') {
            History.initCore();
        }

        // Check Load Status of HTML4 Support
        if (typeof History.initHtml4 !== 'undefined') {
            History.initHtml4();
        }

        // Return true
        return true;
    };


    // ========================================================================
    // Initialise Core

    // Initialise Core
    History.initCore = function (options) {
        // Initialise
        if (typeof History.initCore.initialized !== 'undefined') {
            // Already Loaded
            return false;
        }
        else {
            History.initCore.initialized = true;
        }


        // ====================================================================
        // Options

        /**
		 * History.options
		 * Configurable options
		 */
        History.options = History.options || {};

        /**
		 * History.options.hashChangeInterval
		 * How long should the interval be before hashchange checks
		 */
        History.options.hashChangeInterval = History.options.hashChangeInterval || 100;

        /**
		 * History.options.safariPollInterval
		 * How long should the interval be before safari poll checks
		 */
        History.options.safariPollInterval = History.options.safariPollInterval || 500;

        /**
		 * History.options.doubleCheckInterval
		 * How long should the interval be before we perform a double check
		 */
        History.options.doubleCheckInterval = History.options.doubleCheckInterval || 500;

        /**
		 * History.options.disableSuid
		 * Force History not to append suid
		 */
        History.options.disableSuid = History.options.disableSuid || false;

        /**
		 * History.options.storeInterval
		 * How long should we wait between store calls
		 */
        History.options.storeInterval = History.options.storeInterval || 1000;

        /**
		 * History.options.busyDelay
		 * How long should we wait between busy events
		 */
        History.options.busyDelay = History.options.busyDelay || 250;

        /**
		 * History.options.debug
		 * If true will enable debug messages to be logged
		 */
        History.options.debug = History.options.debug || false;

        /**
		 * History.options.initialTitle
		 * What is the title of the initial state
		 */
        History.options.initialTitle = History.options.initialTitle || document.title;

        /**
		 * History.options.html4Mode
		 * If true, will force HTMl4 mode (hashtags)
		 */
        History.options.html4Mode = History.options.html4Mode || false;

        /**
		 * History.options.delayInit
		 * Want to override default options and call init manually.
		 */
        History.options.delayInit = History.options.delayInit || false;


        // ====================================================================
        // Interval record

        /**
		 * History.intervalList
		 * List of intervals set, to be cleared when document is unloaded.
		 */
        History.intervalList = [];

        /**
		 * History.clearAllIntervals
		 * Clears all setInterval instances.
		 */
        History.clearAllIntervals = function () {
            var i, il = History.intervalList;
            if (typeof il !== "undefined" && il !== null) {
                for (i = 0; i < il.length; i++) {
                    clearInterval(il[i]);
                }
                History.intervalList = null;
            }
        };


        // ====================================================================
        // Debug

        /**
		 * History.debug(message,...)
		 * Logs the passed arguments if debug enabled
		 */
        History.debug = function () {
            if ((History.options.debug || false)) {
                History.log.apply(History, arguments);
            }
        };

        /**
		 * History.log(message,...)
		 * Logs the passed arguments
		 */
        History.log = function () {
            // Prepare
            var
				consoleExists = !(typeof console === 'undefined' || typeof console.log === 'undefined' || typeof console.log.apply === 'undefined'),
				textarea = document.getElementById('log'),
				message,
				i, n,
				args, arg
            ;

            // Write to Console
            if (consoleExists) {
                args = Array.prototype.slice.call(arguments);
                message = args.shift();
                if (typeof console.debug !== 'undefined') {
                    console.debug.apply(console, [message, args]);
                }
                else {
                    console.log.apply(console, [message, args]);
                }
            }
            else {
                message = ("\n" + arguments[0] + "\n");
            }

            // Write to log
            for (i = 1, n = arguments.length; i < n; ++i) {
                arg = arguments[i];
                if (typeof arg === 'object' && typeof JSON !== 'undefined') {
                    try {
                        arg = JSON.stringify(arg);
                    }
                    catch (Exception) {
                        // Recursive Object
                    }
                }
                message += "\n" + arg + "\n";
            }

            // Textarea
            if (textarea) {
                textarea.value += message + "\n-----\n";
                textarea.scrollTop = textarea.scrollHeight - textarea.clientHeight;
            }
                // No Textarea, No Console
            else if (!consoleExists) {
                alert(message);
            }

            // Return true
            return true;
        };


        // ====================================================================
        // Emulated Status

        /**
		 * History.getInternetExplorerMajorVersion()
		 * Get's the major version of Internet Explorer
		 * @return {integer}
		 * @license Public Domain
		 * @author Benjamin Arthur Lupton <contact@balupton.com>
		 * @author James Padolsey <https://gist.github.com/527683>
		 */
        History.getInternetExplorerMajorVersion = function () {
            var result = History.getInternetExplorerMajorVersion.cached =
					(typeof History.getInternetExplorerMajorVersion.cached !== 'undefined')
				? History.getInternetExplorerMajorVersion.cached
				: (function () {
				    var v = 3,
                            div = document.createElement('div'),
                            all = div.getElementsByTagName('i');
				    while ((div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->') && all[0]) { }
				    return (v > 4) ? v : false;
				})()
            ;
            return result;
        };

        /**
		 * History.isInternetExplorer()
		 * Are we using Internet Explorer?
		 * @return {boolean}
		 * @license Public Domain
		 * @author Benjamin Arthur Lupton <contact@balupton.com>
		 */
        History.isInternetExplorer = function () {
            var result =
				History.isInternetExplorer.cached =
				(typeof History.isInternetExplorer.cached !== 'undefined')
					? History.isInternetExplorer.cached
					: Boolean(History.getInternetExplorerMajorVersion())
            ;
            return result;
        };

        /**
		 * History.emulated
		 * Which features require emulating?
		 */

        if (History.options.html4Mode) {
            History.emulated = {
                pushState: true,
                hashChange: true
            };
        }

        else {

            History.emulated = {
                pushState: !Boolean(
					window.history && window.history.pushState && window.history.replaceState
					&& !(
						(/ Mobile\/([1-7][a-z]|(8([abcde]|f(1[0-8]))))/i).test(navigator.userAgent) /* disable for versions of iOS before version 4.3 (8F190) */
						|| (/AppleWebKit\/5([0-2]|3[0-2])/i).test(navigator.userAgent) /* disable for the mercury iOS browser, or at least older versions of the webkit engine */
					)
				),
                hashChange: Boolean(
					!(('onhashchange' in window) || ('onhashchange' in document))
					||
					(History.isInternetExplorer() && History.getInternetExplorerMajorVersion() < 8)
				)
            };
        }

        /**
		 * History.enabled
		 * Is History enabled?
		 */
        History.enabled = !History.emulated.pushState;

        /**
		 * History.bugs
		 * Which bugs are present
		 */
        History.bugs = {
            /**
			 * Safari 5 and Safari iOS 4 fail to return to the correct state once a hash is replaced by a `replaceState` call
			 * https://bugs.webkit.org/show_bug.cgi?id=56249
			 */
            setHash: Boolean(!History.emulated.pushState && navigator.vendor === 'Apple Computer, Inc.' && /AppleWebKit\/5([0-2]|3[0-3])/.test(navigator.userAgent)),

            /**
			 * Safari 5 and Safari iOS 4 sometimes fail to apply the state change under busy conditions
			 * https://bugs.webkit.org/show_bug.cgi?id=42940
			 */
            safariPoll: Boolean(!History.emulated.pushState && navigator.vendor === 'Apple Computer, Inc.' && /AppleWebKit\/5([0-2]|3[0-3])/.test(navigator.userAgent)),

            /**
			 * MSIE 6 and 7 sometimes do not apply a hash even it was told to (requiring a second call to the apply function)
			 */
            ieDoubleCheck: Boolean(History.isInternetExplorer() && History.getInternetExplorerMajorVersion() < 8),

            /**
			 * MSIE 6 requires the entire hash to be encoded for the hashes to trigger the onHashChange event
			 */
            hashEscape: Boolean(History.isInternetExplorer() && History.getInternetExplorerMajorVersion() < 7)
        };

        /**
		 * History.isEmptyObject(obj)
		 * Checks to see if the Object is Empty
		 * @param {Object} obj
		 * @return {boolean}
		 */
        History.isEmptyObject = function (obj) {
            for (var name in obj) {
                if (obj.hasOwnProperty(name)) {
                    return false;
                }
            }
            return true;
        };

        /**
		 * History.cloneObject(obj)
		 * Clones a object and eliminate all references to the original contexts
		 * @param {Object} obj
		 * @return {Object}
		 */
        History.cloneObject = function (obj) {
            var hash, newObj;
            if (obj) {
                hash = JSON.stringify(obj);
                newObj = JSON.parse(hash);
            }
            else {
                newObj = {};
            }
            return newObj;
        };


        // ====================================================================
        // URL Helpers

        /**
		 * History.getRootUrl()
		 * Turns "http://mysite.com/dir/page.html?asd" into "http://mysite.com"
		 * @return {String} rootUrl
		 */
        History.getRootUrl = function () {
            // Create
            var rootUrl = document.location.protocol + '//' + (document.location.hostname || document.location.host);
            if (document.location.port || false) {
                rootUrl += ':' + document.location.port;
            }
            rootUrl += '/';

            // Return
            return rootUrl;
        };

        /**
		 * History.getBaseHref()
		 * Fetches the `href` attribute of the `<base href="...">` element if it exists
		 * @return {String} baseHref
		 */
        History.getBaseHref = function () {
            // Create
            var
				baseElements = document.getElementsByTagName('base'),
				baseElement = null,
				baseHref = '';

            // Test for Base Element
            if (baseElements.length === 1) {
                // Prepare for Base Element
                baseElement = baseElements[0];
                baseHref = baseElement.href.replace(/[^\/]+$/, '');
            }

            // Adjust trailing slash
            baseHref = baseHref.replace(/\/+$/, '');
            if (baseHref) baseHref += '/';

            // Return
            return baseHref;
        };

        /**
		 * History.getBaseUrl()
		 * Fetches the baseHref or basePageUrl or rootUrl (whichever one exists first)
		 * @return {String} baseUrl
		 */
        History.getBaseUrl = function () {
            // Create
            var baseUrl = History.getBaseHref() || History.getBasePageUrl() || History.getRootUrl();

            // Return
            return baseUrl;
        };

        /**
		 * History.getPageUrl()
		 * Fetches the URL of the current page
		 * @return {String} pageUrl
		 */
        History.getPageUrl = function () {
            // Fetch
            var
				State = History.getState(false, false),
				stateUrl = (State || {}).url || History.getLocationHref(),
				pageUrl;

            // Create
            pageUrl = stateUrl.replace(/\/+$/, '').replace(/[^\/]+$/, function (part, index, string) {
                return (/\./).test(part) ? part : part + '/';
            });

            // Return
            return pageUrl;
        };

        /**
		 * History.getBasePageUrl()
		 * Fetches the Url of the directory of the current page
		 * @return {String} basePageUrl
		 */
        History.getBasePageUrl = function () {
            // Create
            var basePageUrl = (History.getLocationHref()).replace(/[#\?].*/, '').replace(/[^\/]+$/, function (part, index, string) {
                return (/[^\/]$/).test(part) ? '' : part;
            }).replace(/\/+$/, '') + '/';

            // Return
            return basePageUrl;
        };

        /**
		 * History.getFullUrl(url)
		 * Ensures that we have an absolute URL and not a relative URL
		 * @param {string} url
		 * @param {Boolean} allowBaseHref
		 * @return {string} fullUrl
		 */
        History.getFullUrl = function (url, allowBaseHref) {
            // Prepare
            var fullUrl = url, firstChar = url.substring(0, 1);
            allowBaseHref = (typeof allowBaseHref === 'undefined') ? true : allowBaseHref;

            // Check
            if (/[a-z]+\:\/\//.test(url)) {
                // Full URL
            }
            else if (firstChar === '/') {
                // Root URL
                fullUrl = History.getRootUrl() + url.replace(/^\/+/, '');
            }
            else if (firstChar === '#') {
                // Anchor URL
                fullUrl = History.getPageUrl().replace(/#.*/, '') + url;
            }
            else if (firstChar === '?') {
                // Query URL
                fullUrl = History.getPageUrl().replace(/[\?#].*/, '') + url;
            }
            else {
                // Relative URL
                if (allowBaseHref) {
                    fullUrl = History.getBaseUrl() + url.replace(/^(\.\/)+/, '');
                } else {
                    fullUrl = History.getBasePageUrl() + url.replace(/^(\.\/)+/, '');
                }
                // We have an if condition above as we do not want hashes
                // which are relative to the baseHref in our URLs
                // as if the baseHref changes, then all our bookmarks
                // would now point to different locations
                // whereas the basePageUrl will always stay the same
            }

            // Return
            return fullUrl.replace(/\#$/, '');
        };

        /**
		 * History.getShortUrl(url)
		 * Ensures that we have a relative URL and not a absolute URL
		 * @param {string} url
		 * @return {string} url
		 */
        History.getShortUrl = function (url) {
            // Prepare
            var shortUrl = url, baseUrl = History.getBaseUrl(), rootUrl = History.getRootUrl();

            // Trim baseUrl
            if (History.emulated.pushState) {
                // We are in a if statement as when pushState is not emulated
                // The actual url these short urls are relative to can change
                // So within the same session, we the url may end up somewhere different
                shortUrl = shortUrl.replace(baseUrl, '');
            }

            // Trim rootUrl
            shortUrl = shortUrl.replace(rootUrl, '/');

            // Ensure we can still detect it as a state
            if (History.isTraditionalAnchor(shortUrl)) {
                shortUrl = './' + shortUrl;
            }

            // Clean It
            shortUrl = shortUrl.replace(/^(\.\/)+/g, './').replace(/\#$/, '');

            // Return
            return shortUrl;
        };

        /**
		 * History.getLocationHref(document)
		 * Returns a normalized version of document.location.href
		 * accounting for browser inconsistencies, etc.
		 *
		 * This URL will be URI-encoded and will include the hash
		 *
		 * @param {object} document
		 * @return {string} url
		 */
        History.getLocationHref = function (doc) {
            doc = doc || document;

            // most of the time, this will be true
            if (doc.URL === doc.location.href)
                return doc.location.href;

            // some versions of webkit URI-decode document.location.href
            // but they leave document.URL in an encoded state
            if (doc.location.href === decodeURIComponent(doc.URL))
                return doc.URL;

            // FF 3.6 only updates document.URL when a page is reloaded
            // document.location.href is updated correctly
            if (doc.location.hash && decodeURIComponent(doc.location.href.replace(/^[^#]+/, "")) === doc.location.hash)
                return doc.location.href;

            if (doc.URL.indexOf('#') == -1 && doc.location.href.indexOf('#') != -1)
                return doc.location.href;

            return doc.URL || doc.location.href;
        };


        // ====================================================================
        // State Storage

        /**
		 * History.store
		 * The store for all session specific data
		 */
        History.store = {};

        /**
		 * History.idToState
		 * 1-1: State ID to State Object
		 */
        History.idToState = History.idToState || {};

        /**
		 * History.stateToId
		 * 1-1: State String to State ID
		 */
        History.stateToId = History.stateToId || {};

        /**
		 * History.urlToId
		 * 1-1: State URL to State ID
		 */
        History.urlToId = History.urlToId || {};

        /**
		 * History.storedStates
		 * Store the states in an array
		 */
        History.storedStates = History.storedStates || [];

        /**
		 * History.savedStates
		 * Saved the states in an array
		 */
        History.savedStates = History.savedStates || [];

        /**
		 * History.noramlizeStore()
		 * Noramlize the store by adding necessary values
		 */
        History.normalizeStore = function () {
            History.store.idToState = History.store.idToState || {};
            History.store.urlToId = History.store.urlToId || {};
            History.store.stateToId = History.store.stateToId || {};
        };

        /**
		 * History.getState()
		 * Get an object containing the data, title and url of the current state
		 * @param {Boolean} friendly
		 * @param {Boolean} create
		 * @return {Object} State
		 */
        History.getState = function (friendly, create) {
            // Prepare
            if (typeof friendly === 'undefined') { friendly = true; }
            if (typeof create === 'undefined') { create = true; }

            // Fetch
            var State = History.getLastSavedState();

            // Create
            if (!State && create) {
                State = History.createStateObject();
            }

            // Adjust
            if (friendly) {
                State = History.cloneObject(State);
                State.url = State.cleanUrl || State.url;
            }

            // Return
            return State;
        };

        /**
		 * History.getIdByState(State)
		 * Gets a ID for a State
		 * @param {State} newState
		 * @return {String} id
		 */
        History.getIdByState = function (newState) {

            // Fetch ID
            var id = History.extractId(newState.url),
				str;

            if (!id) {
                // Find ID via State String
                str = History.getStateString(newState);
                if (typeof History.stateToId[str] !== 'undefined') {
                    id = History.stateToId[str];
                }
                else if (typeof History.store.stateToId[str] !== 'undefined') {
                    id = History.store.stateToId[str];
                }
                else {
                    // Generate a new ID
                    while (true) {
                        id = (new Date()).getTime() + String(Math.random()).replace(/\D/g, '');
                        if (typeof History.idToState[id] === 'undefined' && typeof History.store.idToState[id] === 'undefined') {
                            break;
                        }
                    }

                    // Apply the new State to the ID
                    History.stateToId[str] = id;
                    History.idToState[id] = newState;
                }
            }

            // Return ID
            return id;
        };

        /**
		 * History.normalizeState(State)
		 * Expands a State Object
		 * @param {object} State
		 * @return {object}
		 */
        History.normalizeState = function (oldState) {
            // Variables
            var newState, dataNotEmpty;

            // Prepare
            if (!oldState || (typeof oldState !== 'object')) {
                oldState = {};
            }

            // Check
            if (typeof oldState.normalized !== 'undefined') {
                return oldState;
            }

            // Adjust
            if (!oldState.data || (typeof oldState.data !== 'object')) {
                oldState.data = {};
            }

            // ----------------------------------------------------------------

            // Create
            newState = {};
            newState.normalized = true;
            newState.title = oldState.title || '';
            newState.url = History.getFullUrl(oldState.url ? oldState.url : (History.getLocationHref()));
            newState.hash = History.getShortUrl(newState.url);
            newState.data = History.cloneObject(oldState.data);

            // Fetch ID
            newState.id = History.getIdByState(newState);

            // ----------------------------------------------------------------

            // Clean the URL
            newState.cleanUrl = newState.url.replace(/\??\&_suid.*/, '');
            newState.url = newState.cleanUrl;

            // Check to see if we have more than just a url
            dataNotEmpty = !History.isEmptyObject(newState.data);

            // Apply
            if ((newState.title || dataNotEmpty) && History.options.disableSuid !== true) {
                // Add ID to Hash
                newState.hash = History.getShortUrl(newState.url).replace(/\??\&_suid.*/, '');
                if (!/\?/.test(newState.hash)) {
                    newState.hash += '?';
                }
                newState.hash += '&_suid=' + newState.id;
            }

            // Create the Hashed URL
            newState.hashedUrl = History.getFullUrl(newState.hash);

            // ----------------------------------------------------------------

            // Update the URL if we have a duplicate
            if ((History.emulated.pushState || History.bugs.safariPoll) && History.hasUrlDuplicate(newState)) {
                newState.url = newState.hashedUrl;
            }

            // ----------------------------------------------------------------

            // Return
            return newState;
        };

        /**
		 * History.createStateObject(data,title,url)
		 * Creates a object based on the data, title and url state params
		 * @param {object} data
		 * @param {string} title
		 * @param {string} url
		 * @return {object}
		 */
        History.createStateObject = function (data, title, url) {
            // Hashify
            var State = {
                'data': data,
                'title': title,
                'url': url
            };

            // Expand the State
            State = History.normalizeState(State);

            // Return object
            return State;
        };

        /**
		 * History.getStateById(id)
		 * Get a state by it's UID
		 * @param {String} id
		 */
        History.getStateById = function (id) {
            // Prepare
            id = String(id);

            // Retrieve
            var State = History.idToState[id] || History.store.idToState[id] || undefined;

            // Return State
            return State;
        };

        /**
		 * Get a State's String
		 * @param {State} passedState
		 */
        History.getStateString = function (passedState) {
            // Prepare
            var State, cleanedState, str;

            // Fetch
            State = History.normalizeState(passedState);

            // Clean
            cleanedState = {
                data: State.data,
                title: passedState.title,
                url: passedState.url
            };

            // Fetch
            str = JSON.stringify(cleanedState);

            // Return
            return str;
        };

        /**
		 * Get a State's ID
		 * @param {State} passedState
		 * @return {String} id
		 */
        History.getStateId = function (passedState) {
            // Prepare
            var State, id;

            // Fetch
            State = History.normalizeState(passedState);

            // Fetch
            id = State.id;

            // Return
            return id;
        };

        /**
		 * History.getHashByState(State)
		 * Creates a Hash for the State Object
		 * @param {State} passedState
		 * @return {String} hash
		 */
        History.getHashByState = function (passedState) {
            // Prepare
            var State, hash;

            // Fetch
            State = History.normalizeState(passedState);

            // Hash
            hash = State.hash;

            // Return
            return hash;
        };

        /**
		 * History.extractId(url_or_hash)
		 * Get a State ID by it's URL or Hash
		 * @param {string} url_or_hash
		 * @return {string} id
		 */
        History.extractId = function (url_or_hash) {
            // Prepare
            var id, parts, url, tmp;

            // Extract

            // If the URL has a #, use the id from before the #
            if (url_or_hash.indexOf('#') != -1) {
                tmp = url_or_hash.split("#")[0];
            }
            else {
                tmp = url_or_hash;
            }

            parts = /(.*)\&_suid=([0-9]+)$/.exec(tmp);
            url = parts ? (parts[1] || url_or_hash) : url_or_hash;
            id = parts ? String(parts[2] || '') : '';

            // Return
            return id || false;
        };

        /**
		 * History.isTraditionalAnchor
		 * Checks to see if the url is a traditional anchor or not
		 * @param {String} url_or_hash
		 * @return {Boolean}
		 */
        History.isTraditionalAnchor = function (url_or_hash) {
            // Check
            var isTraditional = !(/[\/\?\.]/.test(url_or_hash));

            // Return
            return isTraditional;
        };

        /**
		 * History.extractState
		 * Get a State by it's URL or Hash
		 * @param {String} url_or_hash
		 * @return {State|null}
		 */
        History.extractState = function (url_or_hash, create) {
            // Prepare
            var State = null, id, url;
            create = create || false;

            // Fetch SUID
            id = History.extractId(url_or_hash);
            if (id) {
                State = History.getStateById(id);
            }

            // Fetch SUID returned no State
            if (!State) {
                // Fetch URL
                url = History.getFullUrl(url_or_hash);

                // Check URL
                id = History.getIdByUrl(url) || false;
                if (id) {
                    State = History.getStateById(id);
                }

                // Create State
                if (!State && create && !History.isTraditionalAnchor(url_or_hash)) {
                    State = History.createStateObject(null, null, url);
                }
            }

            // Return
            return State;
        };

        /**
		 * History.getIdByUrl()
		 * Get a State ID by a State URL
		 */
        History.getIdByUrl = function (url) {
            // Fetch
            var id = History.urlToId[url] || History.store.urlToId[url] || undefined;

            // Return
            return id;
        };

        /**
		 * History.getLastSavedState()
		 * Get an object containing the data, title and url of the current state
		 * @return {Object} State
		 */
        History.getLastSavedState = function () {
            return History.savedStates[History.savedStates.length - 1] || undefined;
        };

        /**
		 * History.getLastStoredState()
		 * Get an object containing the data, title and url of the current state
		 * @return {Object} State
		 */
        History.getLastStoredState = function () {
            return History.storedStates[History.storedStates.length - 1] || undefined;
        };

        /**
		 * History.hasUrlDuplicate
		 * Checks if a Url will have a url conflict
		 * @param {Object} newState
		 * @return {Boolean} hasDuplicate
		 */
        History.hasUrlDuplicate = function (newState) {
            // Prepare
            var hasDuplicate = false,
				oldState;

            // Fetch
            oldState = History.extractState(newState.url);

            // Check
            hasDuplicate = oldState && oldState.id !== newState.id;

            // Return
            return hasDuplicate;
        };

        /**
		 * History.storeState
		 * Store a State
		 * @param {Object} newState
		 * @return {Object} newState
		 */
        History.storeState = function (newState) {
            // Store the State
            History.urlToId[newState.url] = newState.id;

            // Push the State
            History.storedStates.push(History.cloneObject(newState));

            // Return newState
            return newState;
        };

        /**
		 * History.isLastSavedState(newState)
		 * Tests to see if the state is the last state
		 * @param {Object} newState
		 * @return {boolean} isLast
		 */
        History.isLastSavedState = function (newState) {
            // Prepare
            var isLast = false,
				newId, oldState, oldId;

            // Check
            if (History.savedStates.length) {
                newId = newState.id;
                oldState = History.getLastSavedState();
                oldId = oldState.id;

                // Check
                isLast = (newId === oldId);
            }

            // Return
            return isLast;
        };

        /**
		 * History.saveState
		 * Push a State
		 * @param {Object} newState
		 * @return {boolean} changed
		 */
        History.saveState = function (newState) {
            // Check Hash
            if (History.isLastSavedState(newState)) {
                return false;
            }

            // Push the State
            History.savedStates.push(History.cloneObject(newState));

            // Return true
            return true;
        };

        /**
		 * History.getStateByIndex()
		 * Gets a state by the index
		 * @param {integer} index
		 * @return {Object}
		 */
        History.getStateByIndex = function (index) {
            // Prepare
            var State = null;

            // Handle
            if (typeof index === 'undefined') {
                // Get the last inserted
                State = History.savedStates[History.savedStates.length - 1];
            }
            else if (index < 0) {
                // Get from the end
                State = History.savedStates[History.savedStates.length + index];
            }
            else {
                // Get from the beginning
                State = History.savedStates[index];
            }

            // Return State
            return State;
        };

        /**
		 * History.getCurrentIndex()
		 * Gets the current index
		 * @return (integer)
		*/
        History.getCurrentIndex = function () {
            // Prepare
            var index = null;

            // No states saved
            if (History.savedStates.length < 1) {
                index = 0;
            }
            else {
                index = History.savedStates.length - 1;
            }
            return index;
        };

        // ====================================================================
        // Hash Helpers

        /**
		 * History.getHash()
		 * @param {Location=} location
		 * Gets the current document hash
		 * Note: unlike location.hash, this is guaranteed to return the escaped hash in all browsers
		 * @return {string}
		 */
        History.getHash = function (doc) {
            var url = History.getLocationHref(doc),
				hash;
            hash = History.getHashByUrl(url);
            return hash;
        };

        /**
		 * History.unescapeHash()
		 * normalize and Unescape a Hash
		 * @param {String} hash
		 * @return {string}
		 */
        History.unescapeHash = function (hash) {
            // Prepare
            var result = History.normalizeHash(hash);

            // Unescape hash
            result = decodeURIComponent(result);

            // Return result
            return result;
        };

        /**
		 * History.normalizeHash()
		 * normalize a hash across browsers
		 * @return {string}
		 */
        History.normalizeHash = function (hash) {
            // Prepare
            var result = hash.replace(/[^#]*#/, '').replace(/#.*/, '');

            // Return result
            return result;
        };

        /**
		 * History.setHash(hash)
		 * Sets the document hash
		 * @param {string} hash
		 * @return {History}
		 */
        History.setHash = function (hash, queue) {
            // Prepare
            var State, pageUrl;

            // Handle Queueing
            if (queue !== false && History.busy()) {
                // Wait + Push to Queue
                //History.debug('History.setHash: we must wait', arguments);
                History.pushQueue({
                    scope: History,
                    callback: History.setHash,
                    args: arguments,
                    queue: queue
                });
                return false;
            }

            // Log
            //History.debug('History.setHash: called',hash);

            // Make Busy + Continue
            History.busy(true);

            // Check if hash is a state
            State = History.extractState(hash, true);
            if (State && !History.emulated.pushState) {
                // Hash is a state so skip the setHash
                //History.debug('History.setHash: Hash is a state so skipping the hash set with a direct pushState call',arguments);

                // PushState
                History.pushState(State.data, State.title, State.url, false);
            }
            else if (History.getHash() !== hash) {
                // Hash is a proper hash, so apply it

                // Handle browser bugs
                if (History.bugs.setHash) {
                    // Fix Safari Bug https://bugs.webkit.org/show_bug.cgi?id=56249

                    // Fetch the base page
                    pageUrl = History.getPageUrl();

                    // Safari hash apply
                    History.pushState(null, null, pageUrl + '#' + hash, false);
                }
                else {
                    // Normal hash apply
                    document.location.hash = hash;
                }
            }

            // Chain
            return History;
        };

        /**
		 * History.escape()
		 * normalize and Escape a Hash
		 * @return {string}
		 */
        History.escapeHash = function (hash) {
            // Prepare
            var result = History.normalizeHash(hash);

            // Escape hash
            result = window.encodeURIComponent(result);

            // IE6 Escape Bug
            if (!History.bugs.hashEscape) {
                // Restore common parts
                result = result
					.replace(/\%21/g, '!')
					.replace(/\%26/g, '&')
					.replace(/\%3D/g, '=')
					.replace(/\%3F/g, '?');
            }

            // Return result
            return result;
        };

        /**
		 * History.getHashByUrl(url)
		 * Extracts the Hash from a URL
		 * @param {string} url
		 * @return {string} url
		 */
        History.getHashByUrl = function (url) {
            // Extract the hash
            var hash = String(url)
				.replace(/([^#]*)#?([^#]*)#?(.*)/, '$2')
            ;

            // Unescape hash
            hash = History.unescapeHash(hash);

            // Return hash
            return hash;
        };

        /**
		 * History.setTitle(title)
		 * Applies the title to the document
		 * @param {State} newState
		 * @return {Boolean}
		 */
        History.setTitle = function (newState) {
            // Prepare
            var title = newState.title,
				firstState;

            // Initial
            if (!title) {
                firstState = History.getStateByIndex(0);
                if (firstState && firstState.url === newState.url) {
                    title = firstState.title || History.options.initialTitle;
                }
            }

            // Apply
            try {
                document.getElementsByTagName('title')[0].innerHTML = title.replace('<', '&lt;').replace('>', '&gt;').replace(' & ', ' &amp; ');
            }
            catch (Exception) { }
            document.title = title;

            // Chain
            return History;
        };


        // ====================================================================
        // Queueing

        /**
		 * History.queues
		 * The list of queues to use
		 * First In, First Out
		 */
        History.queues = [];

        /**
		 * History.busy(value)
		 * @param {boolean} value [optional]
		 * @return {boolean} busy
		 */
        History.busy = function (value) {
            // Apply
            if (typeof value !== 'undefined') {
                //History.debug('History.busy: changing ['+(History.busy.flag||false)+'] to ['+(value||false)+']', History.queues.length);
                History.busy.flag = value;
            }
                // Default
            else if (typeof History.busy.flag === 'undefined') {
                History.busy.flag = false;
            }

            // Queue
            if (!History.busy.flag) {
                // Execute the next item in the queue
                clearTimeout(History.busy.timeout);
                var fireNext = function () {
                    var i, queue, item;
                    if (History.busy.flag) return;
                    for (i = History.queues.length - 1; i >= 0; --i) {
                        queue = History.queues[i];
                        if (queue.length === 0) continue;
                        item = queue.shift();
                        History.fireQueueItem(item);
                        History.busy.timeout = setTimeout(fireNext, History.options.busyDelay);
                    }
                };
                History.busy.timeout = setTimeout(fireNext, History.options.busyDelay);
            }

            // Return
            return History.busy.flag;
        };

        /**
		 * History.busy.flag
		 */
        History.busy.flag = false;

        /**
		 * History.fireQueueItem(item)
		 * Fire a Queue Item
		 * @param {Object} item
		 * @return {Mixed} result
		 */
        History.fireQueueItem = function (item) {
            return item.callback.apply(item.scope || History, item.args || []);
        };

        /**
		 * History.pushQueue(callback,args)
		 * Add an item to the queue
		 * @param {Object} item [scope,callback,args,queue]
		 */
        History.pushQueue = function (item) {
            // Prepare the queue
            History.queues[item.queue || 0] = History.queues[item.queue || 0] || [];

            // Add to the queue
            History.queues[item.queue || 0].push(item);

            // Chain
            return History;
        };

        /**
		 * History.queue (item,queue), (func,queue), (func), (item)
		 * Either firs the item now if not busy, or adds it to the queue
		 */
        History.queue = function (item, queue) {
            // Prepare
            if (typeof item === 'function') {
                item = {
                    callback: item
                };
            }
            if (typeof queue !== 'undefined') {
                item.queue = queue;
            }

            // Handle
            if (History.busy()) {
                History.pushQueue(item);
            } else {
                History.fireQueueItem(item);
            }

            // Chain
            return History;
        };

        /**
		 * History.clearQueue()
		 * Clears the Queue
		 */
        History.clearQueue = function () {
            History.busy.flag = false;
            History.queues = [];
            return History;
        };


        // ====================================================================
        // IE Bug Fix

        /**
		 * History.stateChanged
		 * States whether or not the state has changed since the last double check was initialised
		 */
        History.stateChanged = false;

        /**
		 * History.doubleChecker
		 * Contains the timeout used for the double checks
		 */
        History.doubleChecker = false;

        /**
		 * History.doubleCheckComplete()
		 * Complete a double check
		 * @return {History}
		 */
        History.doubleCheckComplete = function () {
            // Update
            History.stateChanged = true;

            // Clear
            History.doubleCheckClear();

            // Chain
            return History;
        };

        /**
		 * History.doubleCheckClear()
		 * Clear a double check
		 * @return {History}
		 */
        History.doubleCheckClear = function () {
            // Clear
            if (History.doubleChecker) {
                clearTimeout(History.doubleChecker);
                History.doubleChecker = false;
            }

            // Chain
            return History;
        };

        /**
		 * History.doubleCheck()
		 * Create a double check
		 * @return {History}
		 */
        History.doubleCheck = function (tryAgain) {
            // Reset
            History.stateChanged = false;
            History.doubleCheckClear();

            // Fix IE6,IE7 bug where calling history.back or history.forward does not actually change the hash (whereas doing it manually does)
            // Fix Safari 5 bug where sometimes the state does not change: https://bugs.webkit.org/show_bug.cgi?id=42940
            if (History.bugs.ieDoubleCheck) {
                // Apply Check
                History.doubleChecker = setTimeout(
					function () {
					    History.doubleCheckClear();
					    if (!History.stateChanged) {
					        //History.debug('History.doubleCheck: State has not yet changed, trying again', arguments);
					        // Re-Attempt
					        tryAgain();
					    }
					    return true;
					},
					History.options.doubleCheckInterval
				);
            }

            // Chain
            return History;
        };


        // ====================================================================
        // Safari Bug Fix

        /**
		 * History.safariStatePoll()
		 * Poll the current state
		 * @return {History}
		 */
        History.safariStatePoll = function () {
            // Poll the URL

            // Get the Last State which has the new URL
            var
				urlState = History.extractState(History.getLocationHref()),
				newState;

            // Check for a difference
            if (!History.isLastSavedState(urlState)) {
                newState = urlState;
            }
            else {
                return;
            }

            // Check if we have a state with that url
            // If not create it
            if (!newState) {
                //History.debug('History.safariStatePoll: new');
                newState = History.createStateObject();
            }

            // Apply the New State
            //History.debug('History.safariStatePoll: trigger');
            History.Adapter.trigger(window, 'popstate');

            // Chain
            return History;
        };


        // ====================================================================
        // State Aliases

        /**
		 * History.back(queue)
		 * Send the browser history back one item
		 * @param {Integer} queue [optional]
		 */
        History.back = function (queue) {
            //History.debug('History.back: called', arguments);

            // Handle Queueing
            if (queue !== false && History.busy()) {
                // Wait + Push to Queue
                //History.debug('History.back: we must wait', arguments);
                History.pushQueue({
                    scope: History,
                    callback: History.back,
                    args: arguments,
                    queue: queue
                });
                return false;
            }

            // Make Busy + Continue
            History.busy(true);

            // Fix certain browser bugs that prevent the state from changing
            History.doubleCheck(function () {
                History.back(false);
            });

            // Go back
            history.go(-1);

            // End back closure
            return true;
        };

        /**
		 * History.forward(queue)
		 * Send the browser history forward one item
		 * @param {Integer} queue [optional]
		 */
        History.forward = function (queue) {
            //History.debug('History.forward: called', arguments);

            // Handle Queueing
            if (queue !== false && History.busy()) {
                // Wait + Push to Queue
                //History.debug('History.forward: we must wait', arguments);
                History.pushQueue({
                    scope: History,
                    callback: History.forward,
                    args: arguments,
                    queue: queue
                });
                return false;
            }

            // Make Busy + Continue
            History.busy(true);

            // Fix certain browser bugs that prevent the state from changing
            History.doubleCheck(function () {
                History.forward(false);
            });

            // Go forward
            history.go(1);

            // End forward closure
            return true;
        };

        /**
		 * History.go(index,queue)
		 * Send the browser history back or forward index times
		 * @param {Integer} queue [optional]
		 */
        History.go = function (index, queue) {
            //History.debug('History.go: called', arguments);

            // Prepare
            var i;

            // Handle
            if (index > 0) {
                // Forward
                for (i = 1; i <= index; ++i) {
                    History.forward(queue);
                }
            }
            else if (index < 0) {
                // Backward
                for (i = -1; i >= index; --i) {
                    History.back(queue);
                }
            }
            else {
                throw new Error('History.go: History.go requires a positive or negative integer passed.');
            }

            // Chain
            return History;
        };


        // ====================================================================
        // HTML5 State Support

        // Non-Native pushState Implementation
        if (History.emulated.pushState) {
            /*
			 * Provide Skeleton for HTML4 Browsers
			 */

            // Prepare
            var emptyFunction = function () { };
            History.pushState = History.pushState || emptyFunction;
            History.replaceState = History.replaceState || emptyFunction;
        } // History.emulated.pushState

            // Native pushState Implementation
        else {
            /*
			 * Use native HTML5 History API Implementation
			 */

            /**
			 * History.onPopState(event,extra)
			 * Refresh the Current State
			 */
            History.onPopState = function (event, extra) {
                // Prepare
                var stateId = false, newState = false, currentHash, currentState;

                // Reset the double check
                History.doubleCheckComplete();

                // Check for a Hash, and handle apporiatly
                currentHash = History.getHash();
                if (currentHash) {
                    // Expand Hash
                    currentState = History.extractState(currentHash || History.getLocationHref(), true);
                    if (currentState) {
                        // We were able to parse it, it must be a State!
                        // Let's forward to replaceState
                        //History.debug('History.onPopState: state anchor', currentHash, currentState);
                        History.replaceState(currentState.data, currentState.title, currentState.url, false);
                    }
                    else {
                        // Traditional Anchor
                        //History.debug('History.onPopState: traditional anchor', currentHash);
                        History.Adapter.trigger(window, 'anchorchange');
                        History.busy(false);
                    }

                    // We don't care for hashes
                    History.expectedStateId = false;
                    return false;
                }

                // Ensure
                stateId = History.Adapter.extractEventData('state', event, extra) || false;

                // Fetch State
                if (stateId) {
                    // Vanilla: Back/forward button was used
                    newState = History.getStateById(stateId);
                }
                else if (History.expectedStateId) {
                    // Vanilla: A new state was pushed, and popstate was called manually
                    newState = History.getStateById(History.expectedStateId);
                }
                else {
                    // Initial State
                    newState = History.extractState(History.getLocationHref());
                }

                // The State did not exist in our store
                if (!newState) {
                    // Regenerate the State
                    newState = History.createStateObject(null, null, History.getLocationHref());
                }

                // Clean
                History.expectedStateId = false;

                // Check if we are the same state
                if (History.isLastSavedState(newState)) {
                    // There has been no change (just the page's hash has finally propagated)
                    //History.debug('History.onPopState: no change', newState, History.savedStates);
                    History.busy(false);
                    return false;
                }

                // Store the State
                History.storeState(newState);
                History.saveState(newState);

                // Force update of the title
                History.setTitle(newState);

                // Fire Our Event
                History.Adapter.trigger(window, 'statechange');
                History.busy(false);

                // Return true
                return true;
            };
            History.Adapter.bind(window, 'popstate', History.onPopState);

            /**
			 * History.pushState(data,title,url)
			 * Add a new State to the history object, become it, and trigger onpopstate
			 * We have to trigger for HTML4 compatibility
			 * @param {object} data
			 * @param {string} title
			 * @param {string} url
			 * @return {true}
			 */
            History.pushState = function (data, title, url, queue) {
                //History.debug('History.pushState: called', arguments);

                // Check the State
                if (History.getHashByUrl(url) && History.emulated.pushState) {
                    throw new Error('History.js does not support states with fragement-identifiers (hashes/anchors).');
                }

                // Handle Queueing
                if (queue !== false && History.busy()) {
                    // Wait + Push to Queue
                    //History.debug('History.pushState: we must wait', arguments);
                    History.pushQueue({
                        scope: History,
                        callback: History.pushState,
                        args: arguments,
                        queue: queue
                    });
                    return false;
                }

                // Make Busy + Continue
                History.busy(true);

                // Create the newState
                var newState = History.createStateObject(data, title, url);

                // Check it
                if (History.isLastSavedState(newState)) {
                    // Won't be a change
                    History.busy(false);
                }
                else {
                    // Store the newState
                    History.storeState(newState);
                    History.expectedStateId = newState.id;

                    // Push the newState
                    history.pushState(newState.id, newState.title, newState.url);

                    // Fire HTML5 Event
                    History.Adapter.trigger(window, 'popstate');
                }

                // End pushState closure
                return true;
            };

            /**
			 * History.replaceState(data,title,url)
			 * Replace the State and trigger onpopstate
			 * We have to trigger for HTML4 compatibility
			 * @param {object} data
			 * @param {string} title
			 * @param {string} url
			 * @return {true}
			 */
            History.replaceState = function (data, title, url, queue) {
                //History.debug('History.replaceState: called', arguments);

                // Check the State
                if (History.getHashByUrl(url) && History.emulated.pushState) {
                    throw new Error('History.js does not support states with fragement-identifiers (hashes/anchors).');
                }

                // Handle Queueing
                if (queue !== false && History.busy()) {
                    // Wait + Push to Queue
                    //History.debug('History.replaceState: we must wait', arguments);
                    History.pushQueue({
                        scope: History,
                        callback: History.replaceState,
                        args: arguments,
                        queue: queue
                    });
                    return false;
                }

                // Make Busy + Continue
                History.busy(true);

                // Create the newState
                var newState = History.createStateObject(data, title, url);

                // Check it
                if (History.isLastSavedState(newState)) {
                    // Won't be a change
                    History.busy(false);
                }
                else {
                    // Store the newState
                    History.storeState(newState);
                    History.expectedStateId = newState.id;

                    // Push the newState
                    history.replaceState(newState.id, newState.title, newState.url);

                    // Fire HTML5 Event
                    History.Adapter.trigger(window, 'popstate');
                }

                // End replaceState closure
                return true;
            };

        } // !History.emulated.pushState


        // ====================================================================
        // Initialise

        /**
		 * Load the Store
		 */
        if (sessionStorage) {
            // Fetch
            try {
                History.store = JSON.parse(sessionStorage.getItem('History.store')) || {};
            }
            catch (err) {
                History.store = {};
            }

            // Normalize
            History.normalizeStore();
        }
        else {
            // Default Load
            History.store = {};
            History.normalizeStore();
        }

        /**
		 * Clear Intervals on exit to prevent memory leaks
		 */
        History.Adapter.bind(window, "unload", History.clearAllIntervals);

        /**
		 * Create the initial State
		 */
        History.saveState(History.storeState(History.extractState(History.getLocationHref(), true)));

        /**
		 * Bind for Saving Store
		 */
        if (sessionStorage) {
            // When the page is closed
            History.onUnload = function () {
                // Prepare
                var currentStore, item, currentStoreString;

                // Fetch
                try {
                    currentStore = JSON.parse(sessionStorage.getItem('History.store')) || {};
                }
                catch (err) {
                    currentStore = {};
                }

                // Ensure
                currentStore.idToState = currentStore.idToState || {};
                currentStore.urlToId = currentStore.urlToId || {};
                currentStore.stateToId = currentStore.stateToId || {};

                // Sync
                for (item in History.idToState) {
                    if (!History.idToState.hasOwnProperty(item)) {
                        continue;
                    }
                    currentStore.idToState[item] = History.idToState[item];
                }
                for (item in History.urlToId) {
                    if (!History.urlToId.hasOwnProperty(item)) {
                        continue;
                    }
                    currentStore.urlToId[item] = History.urlToId[item];
                }
                for (item in History.stateToId) {
                    if (!History.stateToId.hasOwnProperty(item)) {
                        continue;
                    }
                    currentStore.stateToId[item] = History.stateToId[item];
                }

                // Update
                History.store = currentStore;
                History.normalizeStore();

                // In Safari, going into Private Browsing mode causes the
                // Session Storage object to still exist but if you try and use
                // or set any property/function of it it throws the exception
                // "QUOTA_EXCEEDED_ERR: DOM Exception 22: An attempt was made to
                // add something to storage that exceeded the quota." infinitely
                // every second.
                currentStoreString = JSON.stringify(currentStore);
                try {
                    // Store
                    sessionStorage.setItem('History.store', currentStoreString);
                }
                catch (e) {
                    if (e.code === DOMException.QUOTA_EXCEEDED_ERR) {
                        if (sessionStorage.length) {
                            // Workaround for a bug seen on iPads. Sometimes the quota exceeded error comes up and simply
                            // removing/resetting the storage can work.
                            sessionStorage.removeItem('History.store');
                            sessionStorage.setItem('History.store', currentStoreString);
                        } else {
                            // Otherwise, we're probably private browsing in Safari, so we'll ignore the exception.
                        }
                    } else {
                        throw e;
                    }
                }
            };

            // For Internet Explorer
            History.intervalList.push(setInterval(History.onUnload, History.options.storeInterval));

            // For Other Browsers
            History.Adapter.bind(window, 'beforeunload', History.onUnload);
            History.Adapter.bind(window, 'unload', History.onUnload);

            // Both are enabled for consistency
        }

        // Non-Native pushState Implementation
        if (!History.emulated.pushState) {
            // Be aware, the following is only for native pushState implementations
            // If you are wanting to include something for all browsers
            // Then include it above this if block

            /**
			 * Setup Safari Fix
			 */
            if (History.bugs.safariPoll) {
                History.intervalList.push(setInterval(History.safariStatePoll, History.options.safariPollInterval));
            }

            /**
			 * Ensure Cross Browser Compatibility
			 */
            if (navigator.vendor === 'Apple Computer, Inc.' || (navigator.appCodeName || '') === 'Mozilla') {
                /**
				 * Fix Safari HashChange Issue
				 */

                // Setup Alias
                History.Adapter.bind(window, 'hashchange', function () {
                    History.Adapter.trigger(window, 'popstate');
                });

                // Initialise Alias
                if (History.getHash()) {
                    History.Adapter.onDomLoad(function () {
                        History.Adapter.trigger(window, 'hashchange');
                    });
                }
            }

        } // !History.emulated.pushState


    }; // History.initCore

    // Try to Initialise History
    if (!History.options || !History.options.delayInit) {
        History.init();
    }

})(window);

//*! jQuery mousewheel */
(function ($) {

    (function (factory) {
        if (typeof define === 'function' && define.amd) {
            // AMD. Register as an anonymous module.
            define(['jquery'], factory);
        } else if (typeof exports === 'object') {
            // Node/CommonJS style for Browserify
            module.exports = factory;
        } else {
            // Browser globals
            factory(jQuery);
        }
    }(function ($) {

        var toFix = ['wheel', 'mousewheel', 'DOMMouseScroll', 'MozMousePixelScroll'];
        var toBind = 'onwheel' in document || document.documentMode >= 9 ? ['wheel'] : ['mousewheel', 'DomMouseScroll', 'MozMousePixelScroll'];
        var lowestDelta, lowestDeltaXY;

        if ($.event.fixHooks) {
            for (var i = toFix.length; i;) {
                $.event.fixHooks[toFix[--i]] = $.event.mouseHooks;
            }
        }

        $.event.special.mousewheel = {
            setup: function () {
                if (this.addEventListener) {
                    for (var i = toBind.length; i;) {
                        this.addEventListener(toBind[--i], handler, false);
                    }
                } else {
                    this.onmousewheel = handler;
                }
            },

            teardown: function () {
                if (this.removeEventListener) {
                    for (var i = toBind.length; i;) {
                        this.removeEventListener(toBind[--i], handler, false);
                    }
                } else {
                    this.onmousewheel = null;
                }
            }
        };

        $.fn.extend({
            mousewheel: function (fn) {
                return fn ? this.bind("mousewheel", fn) : this.trigger("mousewheel");
            },

            unmousewheel: function (fn) {
                return this.unbind("mousewheel", fn);
            }
        });


        function handler(event) {
            var orgEvent = event || window.event,
                args = [].slice.call(arguments, 1),
                delta = 0,
                deltaX = 0,
                deltaY = 0,
                absDelta = 0,
                absDeltaXY = 0,
                fn;
            event = $.event.fix(orgEvent);
            event.type = "mousewheel";

            // Old school scrollwheel delta
            if (orgEvent.wheelDelta) { delta = orgEvent.wheelDelta; }
            if (orgEvent.detail) { delta = orgEvent.detail * -1; }

            // New school wheel delta (wheel event)
            if (orgEvent.deltaY) {
                deltaY = orgEvent.deltaY * -1;
                delta = deltaY;
            }
            if (orgEvent.deltaX) {
                deltaX = orgEvent.deltaX;
                delta = deltaX * -1;
            }

            // Webkit
            if (orgEvent.wheelDeltaY !== undefined) { deltaY = orgEvent.wheelDeltaY; }
            if (orgEvent.wheelDeltaX !== undefined) { deltaX = orgEvent.wheelDeltaX * -1; }

            // Look for lowest delta to normalize the delta values
            absDelta = Math.abs(delta);
            if (!lowestDelta || absDelta < lowestDelta) { lowestDelta = absDelta; }
            absDeltaXY = Math.max(Math.abs(deltaY), Math.abs(deltaX));
            if (!lowestDeltaXY || absDeltaXY < lowestDeltaXY) { lowestDeltaXY = absDeltaXY; }

            // Get a whole value for the deltas
            fn = delta > 0 ? 'floor' : 'ceil';
            delta = Math[fn](delta / lowestDelta);
            deltaX = Math[fn](deltaX / lowestDeltaXY);
            deltaY = Math[fn](deltaY / lowestDeltaXY);

            // Add event and delta to the front of the arguments
            args.unshift(event, delta, deltaX, deltaY);

            return ($.event.dispatch || $.event.handle).apply(this, args);
        }

    }));

})(jQuery);

/*! jQuery Color Animation https://github.com/jquery/jquery-color/ */
(function ($) {
    /*!
 * jQuery Color Animations v2.1.2
 * https://github.com/jquery/jquery-color
 *
 * Copyright 2013 jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 *
 * Date: Wed Jan 16 08:47:09 2013 -0600
 */
    (function (jQuery, undefined) {

        var stepHooks = "backgroundColor borderBottomColor borderLeftColor borderRightColor borderTopColor color columnRuleColor outlineColor textDecorationColor textEmphasisColor",

        // plusequals test for += 100 -= 100
        rplusequals = /^([\-+])=\s*(\d+\.?\d*)/,
        // a set of RE's that can match strings and generate color tuples.
        stringParsers = [{
            re: /rgba?\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*(?:,\s*(\d?(?:\.\d+)?)\s*)?\)/,
            parse: function (execResult) {
                return [
					execResult[1],
					execResult[2],
					execResult[3],
					execResult[4]
                ];
            }
        }, {
            re: /rgba?\(\s*(\d+(?:\.\d+)?)\%\s*,\s*(\d+(?:\.\d+)?)\%\s*,\s*(\d+(?:\.\d+)?)\%\s*(?:,\s*(\d?(?:\.\d+)?)\s*)?\)/,
            parse: function (execResult) {
                return [
					execResult[1] * 2.55,
					execResult[2] * 2.55,
					execResult[3] * 2.55,
					execResult[4]
                ];
            }
        }, {
            // this regex ignores A-F because it's compared against an already lowercased string
            re: /#([a-f0-9]{2})([a-f0-9]{2})([a-f0-9]{2})/,
            parse: function (execResult) {
                return [
					parseInt(execResult[1], 16),
					parseInt(execResult[2], 16),
					parseInt(execResult[3], 16)
                ];
            }
        }, {
            // this regex ignores A-F because it's compared against an already lowercased string
            re: /#([a-f0-9])([a-f0-9])([a-f0-9])/,
            parse: function (execResult) {
                return [
					parseInt(execResult[1] + execResult[1], 16),
					parseInt(execResult[2] + execResult[2], 16),
					parseInt(execResult[3] + execResult[3], 16)
                ];
            }
        }, {
            re: /hsla?\(\s*(\d+(?:\.\d+)?)\s*,\s*(\d+(?:\.\d+)?)\%\s*,\s*(\d+(?:\.\d+)?)\%\s*(?:,\s*(\d?(?:\.\d+)?)\s*)?\)/,
            space: "hsla",
            parse: function (execResult) {
                return [
					execResult[1],
					execResult[2] / 100,
					execResult[3] / 100,
					execResult[4]
                ];
            }
        }],

        // jQuery.Color( )
        color = jQuery.Color = function (color, green, blue, alpha) {
            return new jQuery.Color.fn.parse(color, green, blue, alpha);
        },
        spaces = {
            rgba: {
                props: {
                    red: {
                        idx: 0,
                        type: "byte"
                    },
                    green: {
                        idx: 1,
                        type: "byte"
                    },
                    blue: {
                        idx: 2,
                        type: "byte"
                    }
                }
            },

            hsla: {
                props: {
                    hue: {
                        idx: 0,
                        type: "degrees"
                    },
                    saturation: {
                        idx: 1,
                        type: "percent"
                    },
                    lightness: {
                        idx: 2,
                        type: "percent"
                    }
                }
            }
        },
        propTypes = {
            "byte": {
                floor: true,
                max: 255
            },
            "percent": {
                max: 1
            },
            "degrees": {
                mod: 360,
                floor: true
            }
        },
        support = color.support = {},

        // element for support tests
        supportElem = jQuery("<p>")[0],

        // colors = jQuery.Color.names
        colors,

        // local aliases of functions called often
        each = jQuery.each;

        // determine rgba support immediately
        supportElem.style.cssText = "background-color:rgba(1,1,1,.5)";
        support.rgba = supportElem.style.backgroundColor.indexOf("rgba") > -1;

        // define cache name and alpha properties
        // for rgba and hsla spaces
        each(spaces, function (spaceName, space) {
            space.cache = "_" + spaceName;
            space.props.alpha = {
                idx: 3,
                type: "percent",
                def: 1
            };
        });

        function clamp(value, prop, allowEmpty) {
            var type = propTypes[prop.type] || {};

            if (value == null) {
                return (allowEmpty || !prop.def) ? null : prop.def;
            }

            // ~~ is an short way of doing floor for positive numbers
            value = type.floor ? ~~value : parseFloat(value);

            // IE will pass in empty strings as value for alpha,
            // which will hit this case
            if (isNaN(value)) {
                return prop.def;
            }

            if (type.mod) {
                // we add mod before modding to make sure that negatives values
                // get converted properly: -10 -> 350
                return (value + type.mod) % type.mod;
            }

            // for now all property types without mod have min and max
            return 0 > value ? 0 : type.max < value ? type.max : value;
        }

        function stringParse(string) {
            var inst = color(),
                rgba = inst._rgba = [];

            string = string.toLowerCase();

            each(stringParsers, function (i, parser) {
                var parsed,
                    match = parser.re.exec(string),
                    values = match && parser.parse(match),
                    spaceName = parser.space || "rgba";

                if (values) {
                    parsed = inst[spaceName](values);

                    // if this was an rgba parse the assignment might happen twice
                    // oh well....
                    inst[spaces[spaceName].cache] = parsed[spaces[spaceName].cache];
                    rgba = inst._rgba = parsed._rgba;

                    // exit each( stringParsers ) here because we matched
                    return false;
                }
            });

            // Found a stringParser that handled it
            if (rgba.length) {

                // if this came from a parsed string, force "transparent" when alpha is 0
                // chrome, (and maybe others) return "transparent" as rgba(0,0,0,0)
                if (rgba.join() === "0,0,0,0") {
                    jQuery.extend(rgba, colors.transparent);
                }
                return inst;
            }

            // named colors
            return colors[string];
        }

        color.fn = jQuery.extend(color.prototype, {
            parse: function (red, green, blue, alpha) {
                if (red === undefined) {
                    this._rgba = [null, null, null, null];
                    return this;
                }
                if (red.jquery || red.nodeType) {
                    red = jQuery(red).css(green);
                    green = undefined;
                }

                var inst = this,
                    type = jQuery.type(red),
                    rgba = this._rgba = [];

                // more than 1 argument specified - assume ( red, green, blue, alpha )
                if (green !== undefined) {
                    red = [red, green, blue, alpha];
                    type = "array";
                }

                if (type === "string") {
                    return this.parse(stringParse(red) || colors._default);
                }

                if (type === "array") {
                    each(spaces.rgba.props, function (key, prop) {
                        rgba[prop.idx] = clamp(red[prop.idx], prop);
                    });
                    return this;
                }

                if (type === "object") {
                    if (red instanceof color) {
                        each(spaces, function (spaceName, space) {
                            if (red[space.cache]) {
                                inst[space.cache] = red[space.cache].slice();
                            }
                        });
                    } else {
                        each(spaces, function (spaceName, space) {
                            var cache = space.cache;
                            each(space.props, function (key, prop) {

                                // if the cache doesn't exist, and we know how to convert
                                if (!inst[cache] && space.to) {

                                    // if the value was null, we don't need to copy it
                                    // if the key was alpha, we don't need to copy it either
                                    if (key === "alpha" || red[key] == null) {
                                        return;
                                    }
                                    inst[cache] = space.to(inst._rgba);
                                }

                                // this is the only case where we allow nulls for ALL properties.
                                // call clamp with alwaysAllowEmpty
                                inst[cache][prop.idx] = clamp(red[key], prop, true);
                            });

                            // everything defined but alpha?
                            if (inst[cache] && jQuery.inArray(null, inst[cache].slice(0, 3)) < 0) {
                                // use the default of 1
                                inst[cache][3] = 1;
                                if (space.from) {
                                    inst._rgba = space.from(inst[cache]);
                                }
                            }
                        });
                    }
                    return this;
                }
            },
            is: function (compare) {
                var is = color(compare),
                    same = true,
                    inst = this;

                each(spaces, function (_, space) {
                    var localCache,
                        isCache = is[space.cache];
                    if (isCache) {
                        localCache = inst[space.cache] || space.to && space.to(inst._rgba) || [];
                        each(space.props, function (_, prop) {
                            if (isCache[prop.idx] != null) {
                                same = (isCache[prop.idx] === localCache[prop.idx]);
                                return same;
                            }
                        });
                    }
                    return same;
                });
                return same;
            },
            _space: function () {
                var used = [],
                    inst = this;
                each(spaces, function (spaceName, space) {
                    if (inst[space.cache]) {
                        used.push(spaceName);
                    }
                });
                return used.pop();
            },
            transition: function (other, distance) {
                var end = color(other),
                    spaceName = end._space(),
                    space = spaces[spaceName],
                    startColor = this.alpha() === 0 ? color("transparent") : this,
                    start = startColor[space.cache] || space.to(startColor._rgba),
                    result = start.slice();

                end = end[space.cache];
                each(space.props, function (key, prop) {
                    var index = prop.idx,
                        startValue = start[index],
                        endValue = end[index],
                        type = propTypes[prop.type] || {};

                    // if null, don't override start value
                    if (endValue === null) {
                        return;
                    }
                    // if null - use end
                    if (startValue === null) {
                        result[index] = endValue;
                    } else {
                        if (type.mod) {
                            if (endValue - startValue > type.mod / 2) {
                                startValue += type.mod;
                            } else if (startValue - endValue > type.mod / 2) {
                                startValue -= type.mod;
                            }
                        }
                        result[index] = clamp((endValue - startValue) * distance + startValue, prop);
                    }
                });
                return this[spaceName](result);
            },
            blend: function (opaque) {
                // if we are already opaque - return ourself
                if (this._rgba[3] === 1) {
                    return this;
                }

                var rgb = this._rgba.slice(),
                    a = rgb.pop(),
                    blend = color(opaque)._rgba;

                return color(jQuery.map(rgb, function (v, i) {
                    return (1 - a) * blend[i] + a * v;
                }));
            },
            toRgbaString: function () {
                var prefix = "rgba(",
                    rgba = jQuery.map(this._rgba, function (v, i) {
                        return v == null ? (i > 2 ? 1 : 0) : v;
                    });

                if (rgba[3] === 1) {
                    rgba.pop();
                    prefix = "rgb(";
                }

                return prefix + rgba.join() + ")";
            },
            toHslaString: function () {
                var prefix = "hsla(",
                    hsla = jQuery.map(this.hsla(), function (v, i) {
                        if (v == null) {
                            v = i > 2 ? 1 : 0;
                        }

                        // catch 1 and 2
                        if (i && i < 3) {
                            v = Math.round(v * 100) + "%";
                        }
                        return v;
                    });

                if (hsla[3] === 1) {
                    hsla.pop();
                    prefix = "hsl(";
                }
                return prefix + hsla.join() + ")";
            },
            toHexString: function (includeAlpha) {
                var rgba = this._rgba.slice(),
                    alpha = rgba.pop();

                if (includeAlpha) {
                    rgba.push(~~(alpha * 255));
                }

                return "#" + jQuery.map(rgba, function (v) {

                    // default to 0 when nulls exist
                    v = (v || 0).toString(16);
                    return v.length === 1 ? "0" + v : v;
                }).join("");
            },
            toString: function () {
                return this._rgba[3] === 0 ? "transparent" : this.toRgbaString();
            }
        });
        color.fn.parse.prototype = color.fn;

        // hsla conversions adapted from:
        // https://code.google.com/p/maashaack/source/browse/packages/graphics/trunk/src/graphics/colors/HUE2RGB.as?r=5021

        function hue2rgb(p, q, h) {
            h = (h + 1) % 1;
            if (h * 6 < 1) {
                return p + (q - p) * h * 6;
            }
            if (h * 2 < 1) {
                return q;
            }
            if (h * 3 < 2) {
                return p + (q - p) * ((2 / 3) - h) * 6;
            }
            return p;
        }

        spaces.hsla.to = function (rgba) {
            if (rgba[0] == null || rgba[1] == null || rgba[2] == null) {
                return [null, null, null, rgba[3]];
            }
            var r = rgba[0] / 255,
                g = rgba[1] / 255,
                b = rgba[2] / 255,
                a = rgba[3],
                max = Math.max(r, g, b),
                min = Math.min(r, g, b),
                diff = max - min,
                add = max + min,
                l = add * 0.5,
                h, s;

            if (min === max) {
                h = 0;
            } else if (r === max) {
                h = (60 * (g - b) / diff) + 360;
            } else if (g === max) {
                h = (60 * (b - r) / diff) + 120;
            } else {
                h = (60 * (r - g) / diff) + 240;
            }

            // chroma (diff) == 0 means greyscale which, by definition, saturation = 0%
            // otherwise, saturation is based on the ratio of chroma (diff) to lightness (add)
            if (diff === 0) {
                s = 0;
            } else if (l <= 0.5) {
                s = diff / add;
            } else {
                s = diff / (2 - add);
            }
            return [Math.round(h) % 360, s, l, a == null ? 1 : a];
        };

        spaces.hsla.from = function (hsla) {
            if (hsla[0] == null || hsla[1] == null || hsla[2] == null) {
                return [null, null, null, hsla[3]];
            }
            var h = hsla[0] / 360,
                s = hsla[1],
                l = hsla[2],
                a = hsla[3],
                q = l <= 0.5 ? l * (1 + s) : l + s - l * s,
                p = 2 * l - q;

            return [
                Math.round(hue2rgb(p, q, h + (1 / 3)) * 255),
                Math.round(hue2rgb(p, q, h) * 255),
                Math.round(hue2rgb(p, q, h - (1 / 3)) * 255),
                a
            ];
        };


        each(spaces, function (spaceName, space) {
            var props = space.props,
                cache = space.cache,
                to = space.to,
                from = space.from;

            // makes rgba() and hsla()
            color.fn[spaceName] = function (value) {

                // generate a cache for this space if it doesn't exist
                if (to && !this[cache]) {
                    this[cache] = to(this._rgba);
                }
                if (value === undefined) {
                    return this[cache].slice();
                }

                var ret,
                    type = jQuery.type(value),
                    arr = (type === "array" || type === "object") ? value : arguments,
                    local = this[cache].slice();

                each(props, function (key, prop) {
                    var val = arr[type === "object" ? key : prop.idx];
                    if (val == null) {
                        val = local[prop.idx];
                    }
                    local[prop.idx] = clamp(val, prop);
                });

                if (from) {
                    ret = color(from(local));
                    ret[cache] = local;
                    return ret;
                } else {
                    return color(local);
                }
            };

            // makes red() green() blue() alpha() hue() saturation() lightness()
            each(props, function (key, prop) {
                // alpha is included in more than one space
                if (color.fn[key]) {
                    return;
                }
                color.fn[key] = function (value) {
                    var vtype = jQuery.type(value),
                        fn = (key === "alpha" ? (this._hsla ? "hsla" : "rgba") : spaceName),
                        local = this[fn](),
                        cur = local[prop.idx],
                        match;

                    if (vtype === "undefined") {
                        return cur;
                    }

                    if (vtype === "function") {
                        value = value.call(this, cur);
                        vtype = jQuery.type(value);
                    }
                    if (value == null && prop.empty) {
                        return this;
                    }
                    if (vtype === "string") {
                        match = rplusequals.exec(value);
                        if (match) {
                            value = cur + parseFloat(match[2]) * (match[1] === "+" ? 1 : -1);
                        }
                    }
                    local[prop.idx] = value;
                    return this[fn](local);
                };
            });
        });

        // add cssHook and .fx.step function for each named hook.
        // accept a space separated string of properties
        color.hook = function (hook) {
            var hooks = hook.split(" ");
            each(hooks, function (i, hook) {
                jQuery.cssHooks[hook] = {
                    set: function (elem, value) {
                        var parsed, curElem,
                            backgroundColor = "";

                        if (value !== "transparent" && (jQuery.type(value) !== "string" || (parsed = stringParse(value)))) {
                            value = color(parsed || value);
                            if (!support.rgba && value._rgba[3] !== 1) {
                                curElem = hook === "backgroundColor" ? elem.parentNode : elem;
                                while (
                                    (backgroundColor === "" || backgroundColor === "transparent") &&
                                    curElem && curElem.style
                                ) {
                                    try {
                                        backgroundColor = jQuery.css(curElem, "backgroundColor");
                                        curElem = curElem.parentNode;
                                    } catch (e) {
                                    }
                                }

                                value = value.blend(backgroundColor && backgroundColor !== "transparent" ?
                                    backgroundColor :
                                    "_default");
                            }

                            value = value.toRgbaString();
                        }
                        try {
                            elem.style[hook] = value;
                        } catch (e) {
                            // wrapped to prevent IE from throwing errors on "invalid" values like 'auto' or 'inherit'
                        }
                    }
                };
                jQuery.fx.step[hook] = function (fx) {
                    if (!fx.colorInit) {
                        fx.start = color(fx.elem, hook);
                        fx.end = color(fx.end);
                        fx.colorInit = true;
                    }
                    jQuery.cssHooks[hook].set(fx.elem, fx.start.transition(fx.end, fx.pos));
                };
            });

        };

        color.hook(stepHooks);

        jQuery.cssHooks.borderColor = {
            expand: function (value) {
                var expanded = {};

                each(["Top", "Right", "Bottom", "Left"], function (i, part) {
                    expanded["border" + part + "Color"] = value;
                });
                return expanded;
            }
        };

        // Basic color names only.
        // Usage of any of the other color names requires adding yourself or including
        // jquery.color.svg-names.js.
        colors = jQuery.Color.names = {
            // 4.1. Basic color keywords
            aqua: "#00ffff",
            black: "#000000",
            blue: "#0000ff",
            fuchsia: "#ff00ff",
            gray: "#808080",
            green: "#008000",
            lime: "#00ff00",
            maroon: "#800000",
            navy: "#000080",
            olive: "#808000",
            purple: "#800080",
            red: "#ff0000",
            silver: "#c0c0c0",
            teal: "#008080",
            white: "#ffffff",
            yellow: "#ffff00",

            // 4.2.3. "transparent" color keyword
            transparent: [null, null, null, 0],

            _default: "#ffffff"
        };

    })(jQuery);

    /*!
     * jQuery Color Animations v2.1.2 - SVG Color Names
     * https://github.com/jquery/jquery-color
     *
     * Remaining HTML/CSS color names per W3C's CSS Color Module Level 3.
     * http://www.w3.org/TR/css3-color/#svg-color
     *
     * Copyright 2013 jQuery Foundation and other contributors
     * Released under the MIT license.
     * http://jquery.org/license
     *
     * Date: Wed Jan 16 08:47:09 2013 -0600
     */
    jQuery.extend(jQuery.Color.names, {
        // 4.3. Extended color keywords (minus the basic ones in core color plugin)
        aliceblue: "#f0f8ff",
        antiquewhite: "#faebd7",
        aquamarine: "#7fffd4",
        azure: "#f0ffff",
        beige: "#f5f5dc",
        bisque: "#ffe4c4",
        blanchedalmond: "#ffebcd",
        blueviolet: "#8a2be2",
        brown: "#a52a2a",
        burlywood: "#deb887",
        cadetblue: "#5f9ea0",
        chartreuse: "#7fff00",
        chocolate: "#d2691e",
        coral: "#ff7f50",
        cornflowerblue: "#6495ed",
        cornsilk: "#fff8dc",
        crimson: "#dc143c",
        cyan: "#00ffff",
        darkblue: "#00008b",
        darkcyan: "#008b8b",
        darkgoldenrod: "#b8860b",
        darkgray: "#a9a9a9",
        darkgreen: "#006400",
        darkgrey: "#a9a9a9",
        darkkhaki: "#bdb76b",
        darkmagenta: "#8b008b",
        darkolivegreen: "#556b2f",
        darkorange: "#ff8c00",
        darkorchid: "#9932cc",
        darkred: "#8b0000",
        darksalmon: "#e9967a",
        darkseagreen: "#8fbc8f",
        darkslateblue: "#483d8b",
        darkslategray: "#2f4f4f",
        darkslategrey: "#2f4f4f",
        darkturquoise: "#00ced1",
        darkviolet: "#9400d3",
        deeppink: "#ff1493",
        deepskyblue: "#00bfff",
        dimgray: "#696969",
        dimgrey: "#696969",
        dodgerblue: "#1e90ff",
        firebrick: "#b22222",
        floralwhite: "#fffaf0",
        forestgreen: "#228b22",
        gainsboro: "#dcdcdc",
        ghostwhite: "#f8f8ff",
        gold: "#ffd700",
        goldenrod: "#daa520",
        greenyellow: "#adff2f",
        grey: "#808080",
        honeydew: "#f0fff0",
        hotpink: "#ff69b4",
        indianred: "#cd5c5c",
        indigo: "#4b0082",
        ivory: "#fffff0",
        khaki: "#f0e68c",
        lavender: "#e6e6fa",
        lavenderblush: "#fff0f5",
        lawngreen: "#7cfc00",
        lemonchiffon: "#fffacd",
        lightblue: "#add8e6",
        lightcoral: "#f08080",
        lightcyan: "#e0ffff",
        lightgoldenrodyellow: "#fafad2",
        lightgray: "#d3d3d3",
        lightgreen: "#90ee90",
        lightgrey: "#d3d3d3",
        lightpink: "#ffb6c1",
        lightsalmon: "#ffa07a",
        lightseagreen: "#20b2aa",
        lightskyblue: "#87cefa",
        lightslategray: "#778899",
        lightslategrey: "#778899",
        lightsteelblue: "#b0c4de",
        lightyellow: "#ffffe0",
        limegreen: "#32cd32",
        linen: "#faf0e6",
        mediumaquamarine: "#66cdaa",
        mediumblue: "#0000cd",
        mediumorchid: "#ba55d3",
        mediumpurple: "#9370db",
        mediumseagreen: "#3cb371",
        mediumslateblue: "#7b68ee",
        mediumspringgreen: "#00fa9a",
        mediumturquoise: "#48d1cc",
        mediumvioletred: "#c71585",
        midnightblue: "#191970",
        mintcream: "#f5fffa",
        mistyrose: "#ffe4e1",
        moccasin: "#ffe4b5",
        navajowhite: "#ffdead",
        oldlace: "#fdf5e6",
        olivedrab: "#6b8e23",
        orange: "#ffa500",
        orangered: "#ff4500",
        orchid: "#da70d6",
        palegoldenrod: "#eee8aa",
        palegreen: "#98fb98",
        paleturquoise: "#afeeee",
        palevioletred: "#db7093",
        papayawhip: "#ffefd5",
        peachpuff: "#ffdab9",
        peru: "#cd853f",
        pink: "#ffc0cb",
        plum: "#dda0dd",
        powderblue: "#b0e0e6",
        rosybrown: "#bc8f8f",
        royalblue: "#4169e1",
        saddlebrown: "#8b4513",
        salmon: "#fa8072",
        sandybrown: "#f4a460",
        seagreen: "#2e8b57",
        seashell: "#fff5ee",
        sienna: "#a0522d",
        skyblue: "#87ceeb",
        slateblue: "#6a5acd",
        slategray: "#708090",
        slategrey: "#708090",
        snow: "#fffafa",
        springgreen: "#00ff7f",
        steelblue: "#4682b4",
        tan: "#d2b48c",
        thistle: "#d8bfd8",
        tomato: "#ff6347",
        turquoise: "#40e0d0",
        violet: "#ee82ee",
        wheat: "#f5deb3",
        whitesmoke: "#f5f5f5",
        yellowgreen: "#9acd32"
    });
})(jQuery);

/*! jQuery Additional Easing 1.3 http://gsgd.co.uk/sandbox/jquery/easing/ */
(function(jQuery){
    // t: current time, b: begInnIng value, c: change In value, d: duration
    jQuery.easing['jswing'] = jQuery.easing['swing'];
    jQuery.extend( jQuery.easing,
    {
	    def: 'easeOutQuad',
	    swing: function (x, t, b, c, d) {
		    //alert(jQuery.easing.default);
		    return jQuery.easing[jQuery.easing.def](x, t, b, c, d);
	    },
	    easeInQuad: function (x, t, b, c, d) {
		    return c*(t/=d)*t + b;
	    },
	    easeOutQuad: function (x, t, b, c, d) {
		    return -c *(t/=d)*(t-2) + b;
	    },
	    easeInOutQuad: function (x, t, b, c, d) {
		    if ((t/=d/2) < 1) return c/2*t*t + b;
		    return -c/2 * ((--t)*(t-2) - 1) + b;
	    },
	    easeInCubic: function (x, t, b, c, d) {
		    return c*(t/=d)*t*t + b;
	    },
	    easeOutCubic: function (x, t, b, c, d) {
		    return c*((t=t/d-1)*t*t + 1) + b;
	    },
	    easeInOutCubic: function (x, t, b, c, d) {
		    if ((t/=d/2) < 1) return c/2*t*t*t + b;
		    return c/2*((t-=2)*t*t + 2) + b;
	    },
	    easeInQuart: function (x, t, b, c, d) {
		    return c*(t/=d)*t*t*t + b;
	    },
	    easeOutQuart: function (x, t, b, c, d) {
		    return -c * ((t=t/d-1)*t*t*t - 1) + b;
	    },
	    easeInOutQuart: function (x, t, b, c, d) {
		    if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
		    return -c/2 * ((t-=2)*t*t*t - 2) + b;
	    },
	    easeInQuint: function (x, t, b, c, d) {
		    return c*(t/=d)*t*t*t*t + b;
	    },
	    easeOutQuint: function (x, t, b, c, d) {
		    return c*((t=t/d-1)*t*t*t*t + 1) + b;
	    },
	    easeInOutQuint: function (x, t, b, c, d) {
		    if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
		    return c/2*((t-=2)*t*t*t*t + 2) + b;
	    },
	    easeInSine: function (x, t, b, c, d) {
		    return -c * Math.cos(t/d * (Math.PI/2)) + c + b;
	    },
	    easeOutSine: function (x, t, b, c, d) {
		    return c * Math.sin(t/d * (Math.PI/2)) + b;
	    },
	    easeInOutSine: function (x, t, b, c, d) {
		    return -c/2 * (Math.cos(Math.PI*t/d) - 1) + b;
	    },
	    easeInExpo: function (x, t, b, c, d) {
		    return (t==0) ? b : c * Math.pow(2, 10 * (t/d - 1)) + b;
	    },
	    easeOutExpo: function (x, t, b, c, d) {
		    return (t==d) ? b+c : c * (-Math.pow(2, -10 * t/d) + 1) + b;
	    },
	    easeInOutExpo: function (x, t, b, c, d) {
		    if (t==0) return b;
		    if (t==d) return b+c;
		    if ((t/=d/2) < 1) return c/2 * Math.pow(2, 10 * (t - 1)) + b;
		    return c/2 * (-Math.pow(2, -10 * --t) + 2) + b;
	    },
	    easeInCirc: function (x, t, b, c, d) {
		    return -c * (Math.sqrt(1 - (t/=d)*t) - 1) + b;
	    },
	    easeOutCirc: function (x, t, b, c, d) {
		    return c * Math.sqrt(1 - (t=t/d-1)*t) + b;
	    },
	    easeInOutCirc: function (x, t, b, c, d) {
		    if ((t/=d/2) < 1) return -c/2 * (Math.sqrt(1 - t*t) - 1) + b;
		    return c/2 * (Math.sqrt(1 - (t-=2)*t) + 1) + b;
	    },
	    easeInElastic: function (x, t, b, c, d) {
		    var s=1.70158;var p=0;var a=c;
		    if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
		    if (a < Math.abs(c)) { a=c; var s=p/4; }
		    else var s = p/(2*Math.PI) * Math.asin (c/a);
		    return -(a*Math.pow(2,10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )) + b;
	    },
	    easeOutElastic: function (x, t, b, c, d) {
		    var s=1.70158;var p=0;var a=c;
		    if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
		    if (a < Math.abs(c)) { a=c; var s=p/4; }
		    else var s = p/(2*Math.PI) * Math.asin (c/a);
		    return a*Math.pow(2,-10*t) * Math.sin( (t*d-s)*(2*Math.PI)/p ) + c + b;
	    },
	    easeInOutElastic: function (x, t, b, c, d) {
		    var s=1.70158;var p=0;var a=c;
		    if (t==0) return b;  if ((t/=d/2)==2) return b+c;  if (!p) p=d*(.3*1.5);
		    if (a < Math.abs(c)) { a=c; var s=p/4; }
		    else var s = p/(2*Math.PI) * Math.asin (c/a);
		    if (t < 1) return -.5*(a*Math.pow(2,10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )) + b;
		    return a*Math.pow(2,-10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )*.5 + c + b;
	    },
	    easeInBack: function (x, t, b, c, d, s) {
		    if (s == undefined) s = 1.70158;
		    return c*(t/=d)*t*((s+1)*t - s) + b;
	    },
	    easeOutBack: function (x, t, b, c, d, s) {
		    if (s == undefined) s = 1.70158;
		    return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
	    },
	    easeInOutBack: function (x, t, b, c, d, s) {
		    if (s == undefined) s = 1.70158; 
		    if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525))+1)*t - s)) + b;
		    return c/2*((t-=2)*t*(((s*=(1.525))+1)*t + s) + 2) + b;
	    },
	    easeInBounce: function (x, t, b, c, d) {
		    return c - jQuery.easing.easeOutBounce (x, d-t, 0, c, d) + b;
	    },
	    easeOutBounce: function (x, t, b, c, d) {
		    if ((t/=d) < (1/2.75)) {
			    return c*(7.5625*t*t) + b;
		    } else if (t < (2/2.75)) {
			    return c*(7.5625*(t-=(1.5/2.75))*t + .75) + b;
		    } else if (t < (2.5/2.75)) {
			    return c*(7.5625*(t-=(2.25/2.75))*t + .9375) + b;
		    } else {
			    return c*(7.5625*(t-=(2.625/2.75))*t + .984375) + b;
		    }
	    },
	    easeInOutBounce: function (x, t, b, c, d) {
		    if (t < d/2) return jQuery.easing.easeInBounce (x, t*2, 0, c, d) * .5 + b;
		    return jQuery.easing.easeOutBounce (x, t*2-d, 0, c, d) * .5 + c*.5 + b;
	    }
    });
})(jQuery);

/*! jQuery Transit http://ricostacruz.com/jquery.transit */
(function ($) {
    (function (t, e) { if (typeof define === "function" && define.amd) { define(["jquery"], e) } else if (typeof exports === "object") { module.exports = e(require("jquery")) } else { e(t.jQuery) } })(this, function (t) { t.transit = { version: "0.9.12", propertyMap: { marginLeft: "margin", marginRight: "margin", marginBottom: "margin", marginTop: "margin", paddingLeft: "padding", paddingRight: "padding", paddingBottom: "padding", paddingTop: "padding" }, enabled: true, useTransitionEnd: false }; var e = document.createElement("div"); var n = {}; function i(t) { if (t in e.style) return t; var n = ["Moz", "Webkit", "O", "ms"]; var i = t.charAt(0).toUpperCase() + t.substr(1); for (var r = 0; r < n.length; ++r) { var s = n[r] + i; if (s in e.style) { return s } } } function r() { e.style[n.transform] = ""; e.style[n.transform] = "rotateY(90deg)"; return e.style[n.transform] !== "" } var s = navigator.userAgent.toLowerCase().indexOf("chrome") > -1; n.transition = i("transition"); n.transitionDelay = i("transitionDelay"); n.transform = i("transform"); n.transformOrigin = i("transformOrigin"); n.filter = i("Filter"); n.transform3d = r(); var a = { transition: "transitionend", MozTransition: "transitionend", OTransition: "oTransitionEnd", WebkitTransition: "webkitTransitionEnd", msTransition: "MSTransitionEnd" }; var o = n.transitionEnd = a[n.transition] || null; for (var u in n) { if (n.hasOwnProperty(u) && typeof t.support[u] === "undefined") { t.support[u] = n[u] } } e = null; t.cssEase = { _default: "ease", "in": "ease-in", out: "ease-out", "in-out": "ease-in-out", snap: "cubic-bezier(0,1,.5,1)", easeInCubic: "cubic-bezier(.550,.055,.675,.190)", easeOutCubic: "cubic-bezier(.215,.61,.355,1)", easeInOutCubic: "cubic-bezier(.645,.045,.355,1)", easeInCirc: "cubic-bezier(.6,.04,.98,.335)", easeOutCirc: "cubic-bezier(.075,.82,.165,1)", easeInOutCirc: "cubic-bezier(.785,.135,.15,.86)", easeInExpo: "cubic-bezier(.95,.05,.795,.035)", easeOutExpo: "cubic-bezier(.19,1,.22,1)", easeInOutExpo: "cubic-bezier(1,0,0,1)", easeInQuad: "cubic-bezier(.55,.085,.68,.53)", easeOutQuad: "cubic-bezier(.25,.46,.45,.94)", easeInOutQuad: "cubic-bezier(.455,.03,.515,.955)", easeInQuart: "cubic-bezier(.895,.03,.685,.22)", easeOutQuart: "cubic-bezier(.165,.84,.44,1)", easeInOutQuart: "cubic-bezier(.77,0,.175,1)", easeInQuint: "cubic-bezier(.755,.05,.855,.06)", easeOutQuint: "cubic-bezier(.23,1,.32,1)", easeInOutQuint: "cubic-bezier(.86,0,.07,1)", easeInSine: "cubic-bezier(.47,0,.745,.715)", easeOutSine: "cubic-bezier(.39,.575,.565,1)", easeInOutSine: "cubic-bezier(.445,.05,.55,.95)", easeInBack: "cubic-bezier(.6,-.28,.735,.045)", easeOutBack: "cubic-bezier(.175, .885,.32,1.275)", easeInOutBack: "cubic-bezier(.68,-.55,.265,1.55)" }; t.cssHooks["transit:transform"] = { get: function (e) { return t(e).data("transform") || new f }, set: function (e, i) { var r = i; if (!(r instanceof f)) { r = new f(r) } if (n.transform === "WebkitTransform" && !s) { e.style[n.transform] = r.toString(true) } else { e.style[n.transform] = r.toString() } t(e).data("transform", r) } }; t.cssHooks.transform = { set: t.cssHooks["transit:transform"].set }; t.cssHooks.filter = { get: function (t) { return t.style[n.filter] }, set: function (t, e) { t.style[n.filter] = e } }; if (t.fn.jquery < "1.8") { t.cssHooks.transformOrigin = { get: function (t) { return t.style[n.transformOrigin] }, set: function (t, e) { t.style[n.transformOrigin] = e } }; t.cssHooks.transition = { get: function (t) { return t.style[n.transition] }, set: function (t, e) { t.style[n.transition] = e } } } p("scale"); p("scaleX"); p("scaleY"); p("translate"); p("rotate"); p("rotateX"); p("rotateY"); p("rotate3d"); p("perspective"); p("skewX"); p("skewY"); p("x", true); p("y", true); function f(t) { if (typeof t === "string") { this.parse(t) } return this } f.prototype = { setFromString: function (t, e) { var n = typeof e === "string" ? e.split(",") : e.constructor === Array ? e : [e]; n.unshift(t); f.prototype.set.apply(this, n) }, set: function (t) { var e = Array.prototype.slice.apply(arguments, [1]); if (this.setter[t]) { this.setter[t].apply(this, e) } else { this[t] = e.join(",") } }, get: function (t) { if (this.getter[t]) { return this.getter[t].apply(this) } else { return this[t] || 0 } }, setter: { rotate: function (t) { this.rotate = b(t, "deg") }, rotateX: function (t) { this.rotateX = b(t, "deg") }, rotateY: function (t) { this.rotateY = b(t, "deg") }, scale: function (t, e) { if (e === undefined) { e = t } this.scale = t + "," + e }, skewX: function (t) { this.skewX = b(t, "deg") }, skewY: function (t) { this.skewY = b(t, "deg") }, perspective: function (t) { this.perspective = b(t, "px") }, x: function (t) { this.set("translate", t, null) }, y: function (t) { this.set("translate", null, t) }, translate: function (t, e) { if (this._translateX === undefined) { this._translateX = 0 } if (this._translateY === undefined) { this._translateY = 0 } if (t !== null && t !== undefined) { this._translateX = b(t, "px") } if (e !== null && e !== undefined) { this._translateY = b(e, "px") } this.translate = this._translateX + "," + this._translateY } }, getter: { x: function () { return this._translateX || 0 }, y: function () { return this._translateY || 0 }, scale: function () { var t = (this.scale || "1,1").split(","); if (t[0]) { t[0] = parseFloat(t[0]) } if (t[1]) { t[1] = parseFloat(t[1]) } return t[0] === t[1] ? t[0] : t }, rotate3d: function () { var t = (this.rotate3d || "0,0,0,0deg").split(","); for (var e = 0; e <= 3; ++e) { if (t[e]) { t[e] = parseFloat(t[e]) } } if (t[3]) { t[3] = b(t[3], "deg") } return t } }, parse: function (t) { var e = this; t.replace(/([a-zA-Z0-9]+)\((.*?)\)/g, function (t, n, i) { e.setFromString(n, i) }) }, toString: function (t) { var e = []; for (var i in this) { if (this.hasOwnProperty(i)) { if (!n.transform3d && (i === "rotateX" || i === "rotateY" || i === "perspective" || i === "transformOrigin")) { continue } if (i[0] !== "_") { if (t && i === "scale") { e.push(i + "3d(" + this[i] + ",1)") } else if (t && i === "translate") { e.push(i + "3d(" + this[i] + ",0)") } else { e.push(i + "(" + this[i] + ")") } } } } return e.join(" ") } }; function c(t, e, n) { if (e === true) { t.queue(n) } else if (e) { t.queue(e, n) } else { t.each(function () { n.call(this) }) } } function l(e) { var i = []; t.each(e, function (e) { e = t.camelCase(e); e = t.transit.propertyMap[e] || t.cssProps[e] || e; e = h(e); if (n[e]) e = h(n[e]); if (t.inArray(e, i) === -1) { i.push(e) } }); return i } function d(e, n, i, r) { var s = l(e); if (t.cssEase[i]) { i = t.cssEase[i] } var a = "" + y(n) + " " + i; if (parseInt(r, 10) > 0) { a += " " + y(r) } var o = []; t.each(s, function (t, e) { o.push(e + " " + a) }); return o.join(", ") } t.fn.transition = t.fn.transit = function (e, i, r, s) { var a = this; var u = 0; var f = true; var l = t.extend(true, {}, e); if (typeof i === "function") { s = i; i = undefined } if (typeof i === "object") { r = i.easing; u = i.delay || 0; f = typeof i.queue === "undefined" ? true : i.queue; s = i.complete; i = i.duration } if (typeof r === "function") { s = r; r = undefined } if (typeof l.easing !== "undefined") { r = l.easing; delete l.easing } if (typeof l.duration !== "undefined") { i = l.duration; delete l.duration } if (typeof l.complete !== "undefined") { s = l.complete; delete l.complete } if (typeof l.queue !== "undefined") { f = l.queue; delete l.queue } if (typeof l.delay !== "undefined") { u = l.delay; delete l.delay } if (typeof i === "undefined") { i = t.fx.speeds._default } if (typeof r === "undefined") { r = t.cssEase._default } i = y(i); var p = d(l, i, r, u); var h = t.transit.enabled && n.transition; var b = h ? parseInt(i, 10) + parseInt(u, 10) : 0; if (b === 0) { var g = function (t) { a.css(l); if (s) { s.apply(a) } if (t) { t() } }; c(a, f, g); return a } var m = {}; var v = function (e) { var i = false; var r = function () { if (i) { a.unbind(o, r) } if (b > 0) { a.each(function () { this.style[n.transition] = m[this] || null }) } if (typeof s === "function") { s.apply(a) } if (typeof e === "function") { e() } }; if (b > 0 && o && t.transit.useTransitionEnd) { i = true; a.bind(o, r) } else { window.setTimeout(r, b) } a.each(function () { if (b > 0) { this.style[n.transition] = p } t(this).css(l) }) }; var z = function (t) { this.offsetWidth; v(t) }; c(a, f, z); return this }; function p(e, i) { if (!i) { t.cssNumber[e] = true } t.transit.propertyMap[e] = n.transform; t.cssHooks[e] = { get: function (n) { var i = t(n).css("transit:transform"); return i.get(e) }, set: function (n, i) { var r = t(n).css("transit:transform"); r.setFromString(e, i); t(n).css({ "transit:transform": r }) } } } function h(t) { return t.replace(/([A-Z])/g, function (t) { return "-" + t.toLowerCase() }) } function b(t, e) { if (typeof t === "string" && !t.match(/^[\-0-9\.]+$/)) { return t } else { return "" + t + e } } function y(e) { var n = e; if (typeof n === "string" && !n.match(/^[\-0-9\.]+/)) { n = t.fx.speeds[n] || t.fx.speeds._default } return b(n, "ms") } t.transit.getTransitionValue = d; return t });
})(jQuery);

