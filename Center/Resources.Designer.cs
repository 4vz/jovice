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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
        ///   Looks up a localized string similar to 
        ///
        ////*! Center Main */
        ///(function () {
        ///
        ///    var center = $$.global(function (page) {
        ///
        ///        // install explore
        ///        $$.explore({
        ///            title: &quot;telkom.center&quot;,
        ///            search: function (s) {
        ///                page.transfer(&quot;/search/&quot; + escape(s));
        ///            }
        ///        });
        ///
        ///    });
        ///
        ///    center.icon = function (name) {
        ///        if (name == &quot;hex&quot;) return &quot;M50.518,188.535h101.038l50.515-87.5l-50.515-87.5H50.518L0,101.035L50.518,188.535z M59.178,28.535h83.718l41.854,72.5l-41.854,72.5H5 [rest of string was truncated]&quot;;.
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
        ///    $$.page(&quot;coba&quot;, {
        ///
        ///        init: function (p) {
        ///
        ///            var satVect = &quot;M609.6,316.6l-14-64.6c-0.4-1.9-2.3-3-4.1-2.6l-252.3,62.4c-1.9,0.5-3,2.3-2.6,4.2l2.3,10l-23.2,5.1l-24.1,15.2l-0.8-2.2h1.3v-3.2	h-2.3l-1.1-2.3l-3.2-3l-6.6-14.2l1.9-1.1l-1.3-3.2h-2l-1.1-2.1l-5,3.9c-1.3-0.4-2.9-0.7-3.6-0.1v-1.3c0,0,2-0.4,1.4-2.5l3.8-1.7	l-3.5-16.8c4.4-0.7,8.2-3,10.6-6.7c5-7.8,1.8-18.8-7.3-24.7c-9-5.9-20.4-4.4-25.5,3.3c-5,7.8-1.8,18.8,7.3,24.7	c4,2.6,8.5,3.8,12.7,3.5l3.1,15.7l-1.9,0.5c0,0-2.5- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string coba {
            get {
                return ResourceManager.GetString("coba", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (function () {
        ///
        ///    var mapArea;
        ///    var gmaps, gmapsOverlay;
        ///    var style = [
        ///        { featureType: &quot;administrative&quot;, elementType: &quot;labels&quot;, stylers: [{ visibility: &quot;off&quot; }] },
        ///        { featureType: &quot;landscape&quot;, elementType: &quot;labels&quot;, stylers: [{ visibility: &quot;off&quot; }] },
        ///        { featureType: &quot;landscape&quot;, stylers: [{ saturation: -50 }] },
        ///        { featureType: &quot;road.highway&quot;, elementType: &quot;geometry&quot;, stylers: [{ saturation: -100 }] },
        ///        { featureType: &quot;road&quot;, elementType: &quot;labels.icon&quot;,  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string jovice_network {
            get {
                return ResourceManager.GetString("jovice_network", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///(function () {
        ///
        ///    $$.script(&quot;search_jovice_interface&quot;, function (b, r, f, p, t) {
        ///        f.setSize(100);
        ///
        ///        //--- entry values
        ///        var interfaceName = f.column(&quot;I_Name&quot;);
        ///        var nodeName = f.column(&quot;NO_Name&quot;);
        ///        var interfaceType = f.column(&quot;I_Type&quot;);
        ///        var interfaceDesc = f.column(&quot;I_Desc&quot;);
        ///        
        ///        if (f.create) {
        ///            r.nodeName = $$.text(b)({ font: [&quot;body&quot;, 15], color: 25, weight: &quot;600&quot;, top: 13, left: 20, noBreak: true, clickToSelect: true, cu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string jovice_search_interface {
            get {
                return ResourceManager.GetString("jovice_search_interface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///(function () {
        ///
        ///
        ///
        ///    $$.script(&quot;search_jovice_service&quot;, function (b, r, f, p, t) {
        ///
        ///        //--- match properties
        ///        /*
        ///        f.setButton(function () {
        ///            p.transfer(&quot;/network/service/&quot; + serviceID, {
        ///                transferData: {
        ///                    serviceID: r.serviceID.text(),
        ///                    customerName: r.customerName.text(),
        ///                    serviceType: r.serviceType.text(),
        ///                    entries: t.entries
        ///                },
        ///                leave: [rest of string was truncated]&quot;;.
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
        ///    $$(&quot;jovice_service&quot;, {
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
        ///    var warnaTextHeading = &quot;#440044&quot;;
        ///
        ///    $$.page(&quot;main&quot;, {
        ///        init: function (p) {
        ///            page = center.init(p);
        ///
        ///            var heading = $$.box(page)({
        ///                color: warnaTextHeading
        ///            });
        ///
        ///            var icondragonball = $$.svg(&quot;asdasdasdasdadasdasdasd&quot;)({
        ///                x: 50,
        ///                y: 20,
        ///                color: &quot;red&quot;
        ///            });
        ///
        ///            icondragonball.x(50);
        ///            icondragonball.y(20);
        ///            icondragonb [rest of string was truncated]&quot;;.
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
        ///   Looks up a localized string similar to 
        ///(function () {
        ///
        ///    $$.page(&quot;coba&quot;, {
        ///
        ///        init: function (p) {
        ///
        ///            var satVect = &quot;M609.6,316.6l-14-64.6c-0.4-1.9-2.3-3-4.1-2.6l-252.3,62.4c-1.9,0.5-3,2.3-2.6,4.2l2.3,10l-23.2,5.1l-24.1,15.2l-0.8-2.2h1.3v-3.2	h-2.3l-1.1-2.3l-3.2-3l-6.6-14.2l1.9-1.1l-1.3-3.2h-2l-1.1-2.1l-5,3.9c-1.3-0.4-2.9-0.7-3.6-0.1v-1.3c0,0,2-0.4,1.4-2.5l3.8-1.7	l-3.5-16.8c4.4-0.7,8.2-3,10.6-6.7c5-7.8,1.8-18.8-7.3-24.7c-9-5.9-20.4-4.4-25.5,3.3c-5,7.8-1.8,18.8,7.3,24.7	c4,2.6,8.5,3.8,12.7,3.5l3.1,15.7l-1.9,0.5c0,0-2.5- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string stats {
            get {
                return ResourceManager.GetString("stats", resourceCulture);
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
        ///            center.hideToolbar();
        ///
        ///            var boleft = ui.box(p)({
        ///                width: &quot;50%&quot;,
        ///                left: &quot;50%&quot;,
        ///                height: &quot;100%&quot;,
        ///                color: 91
        ///            });
        ///            
        ///            var b = ui.text(p)({ text: &quot;SIGN IN&quot;, font: [&quot;body&quot;, 25], color: 30, left: 100, top: 50 });
        ///            
        ///
        ///            //center.showSignIn();
        ///
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string user_signin {
            get {
                return ResourceManager.GetString("user_signin", resourceCulture);
            }
        }
    }
}
