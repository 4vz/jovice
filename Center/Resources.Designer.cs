﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Center {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Center.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///
        ///    var center;
        ///
        ///    // center
        ///    (function (window) {
        ///
        ///        var inited = false;
        ///        var page = null;
        ///        var onmain = false;
        ///
        ///        var shade, close, closeThings, offline, update, frontLogo, frontLogoElements, frontLogoDone = false;
        ///        var ctop;
        ///        var nbox;
        ///        var asea;
        ///
        ///        var tbox;
        ///        var ttitle;
        ///
        ///        var loadingStarted = false;
        ///        var lbox;
        ///
        ///        var ogbox;
        ///
        ///
        ///        // sign in
        ///        var signinArea = null;
        ///
        /// </summary>
        internal static string center {
            get {
                return ResourceManager.GetString("center", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///(function () {
        ///
        ///    function getSection(ars, key) {
        ///        var ar = null;
        ///        $.each(ars, function (ai, av) {
        ///            if (av[0] == key) {
        ///                ar = av;
        ///                return false;
        ///            }
        ///        });
        ///        return ar;
        ///    };
        ///
        ///    ui(&quot;search_jovice_service&quot;, function (b, r, f, p, t) {
        ///
        ///        //--- match properties
        ///        f.setButton(function () {
        ///            p.transfer(&quot;/network/service/&quot; + serviceID, {
        ///                transferData: {
        ///                    s [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string jovice_search_service {
            get {
                return ResourceManager.GetString("jovice_search_service", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///
        ///    var page;
        ///    var transfer = null;
        ///
        ///    var topbox, serviceID, customerName, serviceType;
        ///
        ///
        ///    ui(&quot;jovice_service&quot;, {
        ///        init: function (p) {
        ///            page = center.init(p);
        ///            transfer = p.transfer();
        ///
        ///
        ///
        ///            //debug(p.transfer());
        ///            //debug(page.endUrl());
        ///
        ///            topbox = ui.box(p)({ color: &quot;accent&quot;, position: [20, 20], size: [30, 3] });
        ///            serviceID = ui.text(p)({ position: [20, 35], text: &quot;SID&quot;, font: [&quot;body&quot;, 15],  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string jovice_service {
            get {
                return ResourceManager.GetString("jovice_service", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///
        ///    var page;
        ///    ui(&quot;main&quot;, {
        ///        init: function (p) {
        ///            page = center.init(p);
        ///
        ///            (function() {
        ///                //var paper = logo.paper();
        ///                /*
        ///                Raphael.registerFont({ &quot;w&quot;: 113, &quot;face&quot;: { &quot;font-family&quot;: &quot;Keep Calm Cufonized&quot;, &quot;font-weight&quot;: 500, &quot;font-stretch&quot;: &quot;normal&quot;, &quot;units-per-em&quot;: &quot;360&quot;, &quot;panose-1&quot;: &quot;2 0 0 0 0 0 0 0 0 0&quot;, &quot;ascent&quot;: &quot;288&quot;, &quot;descent&quot;: &quot;-72&quot;, &quot;x-height&quot;: &quot;5&quot;, &quot;bbox&quot;: &quot;-18 -316 413 101.626&quot;, &quot;underline-thick [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string main {
            get {
                return ResourceManager.GetString("main", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///
        ///    var uipage;
        ///
        ///    // functions
        ///    var enterSearchResult, setResults, setFilters, clearSearchResult, setRelated;
        ///
        ///    // features
        ///    var animTransferedClick;
        ///
        ///    var isfiltersexists = false, ispagingexists = false, isnomatchexists = false;
        ///    var necrowonline = false;
        ///
        ///    var searchJQXHR;
        ///    var search, columns, results, sortList, sortBy, sortType, page, npage, mpage, count, type, subType, searchid = null, filters;
        ///    var registerstream = {};
        ///      
        ///    function sea [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string search {
            get {
                return ResourceManager.GetString("search", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///
        ///    var page;
        ///    ui(&quot;user_signin&quot;, {
        ///        init: function (p) {
        ///            page = center.init(p);
        ///            
        ///            var b = ui.text(p)({ text: &quot;SIGN IN&quot;, position: [100, 100] });
        ///
        ///
        ///            p.done();
        ///        },
        ///        start: function (p) {
        ///            center.setSearchBoxValue(null);
        ///            p.done();
        ///        },
        ///        resize: function (p) {
        ///        }
        ///    });
        ///})();.
        /// </summary>
        internal static string user_signin {
            get {
                return ResourceManager.GetString("user_signin", resourceCulture);
            }
        }
    }
}