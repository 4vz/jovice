(function () {

    var tooltip;
    var mapArea;
    var gmaps, gmapsOverlay;
    var style = [
        { featureType: "administrative", elementType: "labels", stylers: [{ visibility: "off" }] },
        { featureType: "landscape", elementType: "labels", stylers: [{ visibility: "off" }] },
        { featureType: "landscape", stylers: [{ saturation: -50 }] },
        { featureType: "road.highway", elementType: "geometry", stylers: [{ saturation: -100 }] },
        { featureType: "road", elementType: "labels.icon", stylers: [{ visibility: "off" }] },
        { featureType: "transit", elementType: "labels", stylers: [{ visibility: "off" }] },
        { featureType: "poi", stylers: [{ visibility: "off" }] },
        { featureType: "water", stylers: [{ color: ui.color(88) }] },
    ];

    var areaGroup = null, areaWitel = null, area = null;

    var areaGridsRequested = null;
    
    var areaGroupMarkers = null, areaWitelMarkers = null;

    var boundRequestTOID;
    var dayNight;
    var dayNightTimer;

    var validCenter = null;
    var validBounds = null;


    var uiPane, markerPane;

    function onGMapsScriptLoaded() {
        
        (function () {
            /**
             * Day/Night Overlay
             * Version 1.3
             *
             * @author kaktus621@gmail.com (Martin Matysiak)
             * @fileoverview This class provides a custom overlay which shows an
             * approximation of where the day/night line runs at any given date.
             */

            /**
             * @license Copyright 2011 — 2015 Martin Matysiak.
             *
             * Licensed under the Apache License, Version 2.0 (the "License");
             * you may not use this file except in compliance with the License.
             * You may obtain a copy of the License at
             *
             *     http://www.apache.org/licenses/LICENSE-2.0
             *
             * Unless required by applicable law or agreed to in writing, software
             * distributed under the License is distributed on an "AS IS" BASIS,
             * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
             * See the License for the specific language governing permissions and
             * limitations under the License.
             */

            /**
             * DayNightOverlayOptions
             *
             * {string} fillColor A color string that will be used when drawing
             * the night area.
             * {string} id A unique identifier which will be assigned to the
             * canvas on which we will draw.
             * {Date} date A specific point of time for which the day/night-
             * overview shall be calculated (UTC date is taken).
             * {google.maps.Map} map A handle to the Google Maps map on which the
             * overlay shall be shown.
             * {number} smallZoomThreshold A threshold for small zoom levels on
             * viewports with small dimensions.
             */
            (function (global, factory) {
                if (typeof module === 'object' && typeof module.exports === 'object') {
                    module.exports = factory();
                } else if (typeof define === 'function' && typeof define.amd === 'object') {
                    define(['goog!maps,3,other_params:[sensor=false&libraries=visualization]'], factory);
                } else {
                    if (typeof google !== 'object' || typeof google.maps !== 'object') {
                        throw new Error('DayNightOverlay requires google maps library');
                    }
                    global.DayNightOverlay = factory();
                }
            }(typeof window !== 'undefined' ? window : this, function () {

                /**
                 * The Class which represents the Overlay.
                 *
                 * @constructor
                 * @param {DayNightOverlayOptions=} opt_params A set of optional parameters.
                 * @extends {google.maps.OverlayView}
                 */
                var DayNightOverlay = function (opt_params) {
                    opt_params = opt_params || {};

                    /**
                     * The canvas on which we will draw later on.
                     * @type {?element}
                     * @private
                     */
                    this.canvas_ = null;

                    /**
                     * The color with which the night area shall be filled.
                     * @type {!string}
                     * @private
                     */
                    this.fillColor_ = opt_params.fillColor || 'rgba(0,0,0,0.5)';

                    /**
                     * If specified, this ID will be assigned to the Canvas element which will be
                     * created later on.
                     * @type {?string}
                     * @private
                     */
                    this.id_ = opt_params.id || null;

                    /**
                     * If specified, this fixed date will be drawn instead of the current time.
                     * The date should always be specified in UTC! Please not that not only the
                     * time, but also the day counts because of the sun's movement between the
                     * Solstices.
                     * @type {?Date} A date object that should be displayed
                     * @private
                     */
                    this.date_ = opt_params.date || null;

                    if (typeof opt_params.map != 'undefined') {
                        this.setMap(opt_params.map);
                    }

                    /**
                     * If maps zoom level is smaller than this threshold, then we adjust the
                     * viewport to the world's dimensions, extended by half a world width on
                     * the left and right.
                     * @type {number} A zoom level
                     */
                    this.smallZoomThreshold = opt_params.smallZoomThreshold || 3;
                };

                DayNightOverlay.prototype = new google.maps.OverlayView();


                /**
                 * A fixed reference to a very northern point on the world. Note: latitudes
                 * over 85 degrees result in a strange bug where the calculated pixel
                 * coordinates are _way_ outside the map. Therefore I use latitudes of +-85
                 * degrees which result in being placed very close to the visible borders
                 * of the map. I suppose this behaviour has to do with the mecrator projection.
                 *
                 * @type {google.maps.LatLng}
                 * @private
                 * @const
                 */
                DayNightOverlay.NORTH_ = new google.maps.LatLng(85, 0);


                /**
                 * A fixed reference to a very southern point on the world. Note: see
                 * DayNightOverlay.NORTH_.
                 *
                 * @type {google.maps.LatLng}
                 * @private
                 * @const
                 */
                DayNightOverlay.SOUTH_ = new google.maps.LatLng(-85, 0);


                /** @override */
                DayNightOverlay.prototype.onAdd = function () {
                    this.canvas_ = document.createElement('canvas');
                    this.canvas_.style.position = 'absolute';

                    if (this.id_) {
                        this.canvas_.id = this.id_;
                    }

                    this.getPanes().overlayLayer.appendChild(this.canvas_);
                };


                /** @override */
                DayNightOverlay.prototype.onRemove = function () {
                    this.canvas_.parentNode.removeChild(this.canvas_);
                    this.canvas_ = null;
                };


                /** @override */
                DayNightOverlay.prototype.draw = function () {

                    // Adjust the canvas to the current map's size
                    var projection = this.getProjection();
                    var worldDim = this.getWorldDimensions_(projection);
                    var visibleDim = this.getVisibleDimensions_(projection, 250);

                    // The viewport dimensions seem to be a bit buggy on small zoom levels.
                    // Therefore we adjust the viewport to the world's dimensions, extended by
                    // half a world width on the left and right
                    if (this.getMap().getZoom() < this.smallZoomThreshold) {
                        //visibleDim = worldDim;
                        visibleDim.x = worldDim.x - worldDim.width;
                        visibleDim.y = worldDim.y;
                        visibleDim.width = worldDim.width * 3;
                        visibleDim.height = worldDim.height;
                    }

                    // Resize canvas to current viewport
                    this.canvas_.style.left = visibleDim.x + 'px';
                    this.canvas_.style.top = visibleDim.y + 'px';
                    this.canvas_.style.width = visibleDim.width + 'px';
                    this.canvas_.style.height = visibleDim.height + 'px';
                    // Important: resize not only CSS dimensions, but also canvas dimensions
                    this.canvas_.width = visibleDim.width;
                    this.canvas_.height = visibleDim.height;

                    // Clear the current canvas
                    var ctx = this.canvas_.getContext('2d');
                    ctx.clearRect(0, 0, visibleDim.width, visibleDim.height);

                    // Redraw the wave which approximately describes where it's currently night
                    var terminator = this.createTerminatorFunc_(visibleDim, worldDim);
                    var northernSun = this.isNorthernSun_(this.date_ ? this.date_ : new Date());

                    ctx.fillStyle = this.fillColor_;
                    ctx.beginPath();
                    ctx.moveTo(0, northernSun ? visibleDim.height : 0);
                    for (var x = 0; x < visibleDim.width; x++) {
                        ctx.lineTo(x, terminator(x));
                    }
                    ctx.lineTo(visibleDim.width, northernSun ? visibleDim.height : 0);
                    ctx.fill();
                };


                /**
                 * Setter for date_.
                 *
                 * @param {?Date} date A specific point of time for which the day/night-
                 * overview shall be calculated (UTC date is taken) or null if the current
                 * time shall be taken.
                 */
                DayNightOverlay.prototype.setDate = function (date) {
                    this.date_ = date;

                    // Redraw the line if we're added to a maps canvas
                    if (this.canvas_ !== null) {
                        this.draw();
                    }
                };


                /**
                 * Returns the coordinates of the world map, based on the current maps view.
                 *
                 * @private
                 * @param {google.maps.MapCanvasProjection} projection The projection object for
                 * the current maps view.
                 * @return {Object} The dimensions, containing x and y coordinates of the upper
                 * left point as well as width and height of the rectangular world map.
                 */
                DayNightOverlay.prototype.getWorldDimensions_ = function (projection) {
                    var north = projection.fromLatLngToDivPixel(DayNightOverlay.NORTH_);
                    var south = projection.fromLatLngToDivPixel(DayNightOverlay.SOUTH_);
                    var width = projection.getWorldWidth();

                    return {
                        x: north.x - width / 2,
                        y: north.y,
                        width: width,
                        height: south.y - north.y
                    };
                };


                /**
                 * Returns the coordinates of the currently visible viewport plus a specifiable
                 * margin around it.
                 *
                 * @private
                 * @param {google.maps.MapCanvasProjection} projection The projection object for
                 * the current maps view.
                 * @param {number} margin The number of pixels by which the dimensions of the
                 * current viewport shall be increased.
                 * @return {Object} The dimensions, containing x and y coordinates of the upper
                 * left point as well as width and height of the rectangular viewport.
                 */
                DayNightOverlay.prototype.getVisibleDimensions_ = function (projection, margin) {
                    var ne = projection.fromLatLngToDivPixel(
                        this.getMap().getBounds().getNorthEast()
                        );
                    var sw = projection.fromLatLngToDivPixel(
                        this.getMap().getBounds().getSouthWest()
                        );

                    return {
                        x: sw.x - margin,
                        y: ne.y - margin,
                        width: ne.x - sw.x + 2 * margin,
                        height: sw.y - ne.y + 2 * margin
                    };
                };


                /**
                 * Generates a function which in turn calculates the day/night terminator curve
                 * based on the current viewport. The generated function awaits an x-coordinate
                 * in the scope of the current viewport (i.e. from 0 to viewport.width) and
                 * returns the corresponding y-coordinate of the curve (limited to the bounds
                 * of the current viewport).
                 *
                 * If you don't like mathematics, feel free to skip this code ;-) It's roughly
                 * based on http://www.geoastro.de/map/index.html
                 *
                 * This version uses a lot of variables to make the calculation a bit more
                 * understandable. You might want to inline them in a minified version.
                 *
                 * @private
                 * @param {Object} viewport The dimensions of the currently visible viewport.
                 * @param {Object} world The dimensions of the world as it's seen in every
                 * atlas (i.e. the rectangular area from (90°N,180°W) to (90°S,180°E)).
                 * @return {function(number): number} A function which calculates the day/night
                 * terminator curve.
                 */
                DayNightOverlay.prototype.createTerminatorFunc_ = function (viewport, world) {

                    var date = this.date_ ? this.date_ : new Date();

                    // Precalculate some constants to make the actual terminator function faster

                    var TWO_PI = 2 * Math.PI;

                    var WORLD_WIDTH = world.width;
                    var WORLD_HEIGHT = world.height;
                    var HALF_WORLD_HEIGHT = world.height / 2;

                    var VISIBLE_WIDTH = viewport.width;
                    var VISIBLE_HEIGHT = viewport.height;

                    var WORLD_OFFSET_X = viewport.x - world.x;
                    var WORLD_OFFSET_Y = viewport.y - world.y;


                    // Scaling factors

                    // Used for scaling the x-coordinate in the scope of the world with
                    // onto the range of [0, 2*PI)
                    var X_SCALE = TWO_PI / WORLD_WIDTH;

                    // Used for scaling the output of the crazy function below ([-PI/2, PI/2]) to
                    // the range of the world's height ([-world.height/2, world.height/2])
                    var Y_SCALE = WORLD_HEIGHT / Math.PI;


                    // Offset calculation

                    // The current (or specified) UTC time in seconds.
                    var TIME_SECS = date.getUTCHours() * 3600 +
                        date.getUTCMinutes() * 60 +
                        date.getUTCSeconds();

                    // Since the world's borders are at longitude +-180 degrees but we are
                    // are comparing to UTC time (which takes place at longitude 0 degrees),
                    // we have to shift the time by exactly 12 hours using NOON_SECS.
                    var NOON_SECS = 86400 / 2;

                    // We calculate the horizontal offset on the basis of seconds. Therefore we
                    // divide the maximum offset (2 * PI) by the amount of seconds in a day.
                    var PI_STEP = TWO_PI / 86400;

                    // Now let's add everything together... the offset is now in the
                    // range of [0, 2*PI)
                    var TIME_OFFSET_X = (TIME_SECS + NOON_SECS) * PI_STEP;

                    // And now the vertical offset... throughout the year, the sun's position
                    // varies between +-23.44 degrees around the equatorial line (it's exactly
                    // over the equator on the vernal and autumnal equinox, 23.44° north at the
                    // summer solstice and 23.44° south at the winter solstice). Between those
                    // dates, the sun moves on a sine wave.

                    // The first thing we do is calculating the sun's position by using the
                    // vernal equinox as a reference point.
                    var DAY_OF_YEAR = this.getDayOfYear_(date);
                    var VERNAL_EQUINOX = this.getDayOfYear_(
                        new Date(Date.UTC(date.getFullYear(), 2, 20))
                        );

                    var MAX_DECLINATION = 23.44 * Math.PI / 90;
                    var DECLINATION = Math.sin(TWO_PI * (DAY_OF_YEAR - VERNAL_EQUINOX) / 365) *
                        MAX_DECLINATION;

                    // The returned method first translates the viewport x to world x,
                    // calculates the world y and translates it back to the viewport y
                    return function (x) {
                        // x in range [0, visible_width]

                        // World x in the range [0, 2PI) ("longitude")
                        var worldX = (x + WORLD_OFFSET_X) * X_SCALE + TIME_OFFSET_X;

                        // World y in the range [-PI/2, PI/2] ("latitude")
                        // This is the main function for calculating the terminator line!!
                        var worldY = Math.atan(-Math.cos(worldX) / Math.tan(DECLINATION));

                        // Translate to range [0, world_height]
                        worldY = HALF_WORLD_HEIGHT + worldY * Y_SCALE;

                        // Crop to visible range
                        return Math.min(VISIBLE_HEIGHT, Math.max(0, worldY - WORLD_OFFSET_Y));
                    };
                };


                /**
                 * Returns true if sun is currently north of the equatorial line. That's
                 * basically always between the vernal and autumnal equinax (i.e. Mar - Sep)
                 *
                 * @private
                 * @param {Date} date The date for which the sun's position shall be determined.
                 * @return {boolean} true if the sun is north of the equatorial line, false
                 * otherwise.
                 */
                DayNightOverlay.prototype.isNorthernSun_ = function (date) {
                    var vernalEq = new Date(Date.UTC(date.getFullYear(), 2, 19));
                    var autumnalEq = new Date(Date.UTC(date.getFullYear(), 8, 18));

                    return (date.getTime() > vernalEq.getTime()) &&
                        (date.getTime() <= autumnalEq.getTime());
                };


                /**
                 * Calculates the day of year based on the given date. Method: the timestamp
                 * of the given date is substracted by the timestamp of the first day of the
                 * respective year. The resulting time difference is then divided by the number
                 * of milliseconds per day, which results in the day of the year.
                 *
                 * @private
                 * @param {Date} date A date for which the day of year shall be calculated.
                 * @return {number} The date's day of year.
                 */
                DayNightOverlay.prototype.getDayOfYear_ = function (date) {
                    // Yes, the month has to be zero thanks to JavaScript's great Date class.....
                    var firstDay = new Date(Date.UTC(date.getFullYear(), 0, 1));

                    return Math.ceil((date.getTime() - firstDay.getTime()) / 86400000);
                };

                return DayNightOverlay;
            }));
        })();

        gmaps = new google.maps.Map(mapArea.dom(), {
            //center: { lat: -1.548514, lng: 119.527694 },
            center: { lat: -6.175403, lng: 106.827114 },
            zoom: 10,
            disableDefaultUI: true
        });
        gmapsOverlay = new google.maps.OverlayView();
        gmapsOverlay.onAdd = function () {
            zoomChanged();
        };
        gmapsOverlay.onRemove = function () {
        };
        gmapsOverlay.draw = function () {
            if (uiPane == null)
                uiPane = $(gmapsOverlay.getPanes().overlayMouseTarget);
            if (markerPane == null)
                markerPane = $(gmapsOverlay.getPanes().markerLayer);

            var zoom = gmaps.getZoom();
            var ctr = gmaps.getCenter();
            var projection = gmapsOverlay.getProjection();

            if ($.isArray(areaGroup)) {
                if (zoom >= 5 && zoom <= 9) {
                    $.each(areaGroupMarkers, function (mi, mv) {
                        if (!mv.isShown()) mv.fadeIn();
                        var group = areaGroup[mi];
                        var label = mv.data("label");
                        var pos = projection.fromLatLngToDivPixel(new google.maps.LatLng(group[1], group[2]));
                        label.text(zoom == 5 ? group[4] : group[3]);
                        mv.width(label.width() + 15);
                        mv.position(pos.x - (mv.width() / 2), pos.y - (mv.height() / 2));
                    });
                }
                else {
                    $.each(areaGroupMarkers, function (mi, mv) {
                        if (mv.isShown()) mv.hide();
                    });
                }
            }
            if ($.isArray(areaWitel)) {
                if (zoom >= 6 && zoom <= 11) {                
                    $.each(areaWitelMarkers, function (mi, mv) {
                        if (!mv.isShown()) $$($$.random(100, 300), function () { mv.fadeIn(); });
                        var witel = areaWitel[mi];
                        var pos = projection.fromLatLngToDivPixel(new google.maps.LatLng(witel[1], witel[2]));
                        if (zoom <= 10) {
                            mv.size(20, 20);
                            mv.position(pos.x - 10, pos.y - 10);
                        }
                        else {
                            mv.size(40, 40);
                            mv.position(pos.x - 20, pos.y - 20);
                        }
                       
                    });
                }
                else {
                    $.each(areaWitelMarkers, function (mi, mv) {
                        if (mv.isShown()) mv.hide();
                    });
                }
            }
            if (zoom >= 10) {
                var grid = latLngToGrid(ctr);
                var x = grid.x;
                var y = grid.y;
                for (var ix = -1; ix <= 1; ix++) {
                    for (var iy = -1; iy <= 1; iy++) {
                        var fx = x + ix;
                        var fy = y + iy;
                        if (fx < 0) fx = 180 + ix;
                        if (fy < 0) fy = 90 + iy;


                        if (areaGridsRequested[fx][fy] != null) {
                            var gridBox = areaGridsRequested[fx][fy];
                            var lngMin = (fx * 2) - 180;
                            var lngMax = lngMin + 2;
                            var latMin = (fy * 2) - 90;
                            var latMax = latMin + 2;
                            var areaTopLeft = projection.fromLatLngToDivPixel(new google.maps.LatLng(latMax, lngMin));
                            var areaBottomRight = projection.fromLatLngToDivPixel(new google.maps.LatLng(latMin, lngMax));

                            gridBox.fadeIn();
                            gridBox.position(areaTopLeft.x, areaTopLeft.y);
                            gridBox.size(areaBottomRight.x - areaTopLeft.x, areaBottomRight.y - areaTopLeft.y);

                            var areaMarkers = gridBox.data("areaMarkers");

                            if (zoom >= 10 && zoom <= 12) {
                                if (areaMarkers != null) {
                                    $.each(areaMarkers, function (mi, mv) {
                                        var areaEdges = mv.data("areaEdges");
                                        if (!mv.isShown()) {
                                            mv.show();                                            
                                            $.each(areaEdges, function (ei, ev) {
                                                ev.show();
                                            });
                                        }
                                        var lat = mv.data("lat");
                                        var lng = mv.data("lng");
                                        var label = mv.data("label");

                                        var pos = projection.fromLatLngToDivPixel(new google.maps.LatLng(lat, lng));
                                        var x = pos.x - areaTopLeft.x;
                                        var y = pos.y - areaTopLeft.y;
                                        mv.position(x - 5, y - 5);
                                        label.position(x + 10, y - 2);

                                        $.each(areaEdges, function (ei, ev) {
                                            var de_lat = ev.data("lat");
                                            var de_lng = ev.data("lng");
                                            var path = ev.data("path");
                                            var dest = ev.data("dest");
                                            
                                            var depos = projection.fromLatLngToDivPixel(new google.maps.LatLng(de_lat, de_lng));
                                                                                       
                                            var dex = depos.x - areaTopLeft.x;
                                            var dey = depos.y - areaTopLeft.y;
                                            
                                            var canvX, canvY, canvW, canvH;











                                            if (dex < x) {
                                                canvX = dex;
                                                canvW = x - dex;
                                            }
                                            else {
                                                canvX = x;
                                                canvW = dex - x;
                                            }
                                            if (dey < y) {
                                                canvY = dey;
                                                canvH = y - dey;
                                            }
                                            else {
                                                canvY = y;
                                                canvH = dey - y;
                                            }

                                            var aLeftRight = 0;
                                            var aTopBottom = 0;

                                            if (canvW < 10) {
                                                aLeftRight = (10 - canvW) / 2;
                                                canvW = 10;
                                                canvX = canvX - aLeftRight;
                                            }
                                            if (canvH < 10) {
                                                aTopBottom = (10 - canvH) / 2;
                                                canvH = 10;
                                                canvY = canvY - aTopBottom;
                                            }
                                            
                                            ev.size(canvW, canvH);
                                            ev.position(canvX, canvY);

                                            if ((dex < x && dey < y) || (dex >= x && dey >= y))
                                                path.attr({ path: "M" + aLeftRight + "," + aTopBottom + "L" + (canvW - aLeftRight) + "," + (canvH - aTopBottom) });
                                            else if ((dex < x && dey >= y) || (dex >= x && dey < y))
                                                path.attr({ path: "M" + aLeftRight + "," + (canvH - aTopBottom) + "L" + (canvW - aLeftRight) + "," + aLeftRight });
                                        });
                                    });
                                }
                            }
                            else {
                                if (areaMarkers != null) {
                                    $.each(areaMarkers, function (mi, mv) {
                                        if (mv.isShown()) {
                                            mv.hide();
                                            var areaEdges = mv.data("areaEdges");
                                            $.each(areaEdges, function (ei, ev) {
                                                ev.hide();
                                            });
                                        }
                                    });
                                }
                            }

                        }
                    }
                }
            }
            else {
                for (var ix = 0; ix < 180; ix++) {
                    for (var iy = 0; iy < 90; iy++) {
                        if (areaGridsRequested[ix][iy] != null) {
                            var box = areaGridsRequested[ix][iy];
                            if (box.isShown()) box.hide();
                        }
                    }
                }
            }
        };
        gmapsOverlay.setMap(gmaps);

        gmaps.setOptions({ styles: style });
        gmaps.addListener("zoom_changed", zoomChanged);
        gmaps.addListener("center_changed", centerChanged);
        gmaps.addListener("bounds_changed", boundsChanged);

        validCenter = gmaps.getCenter();
        validBounds = new google.maps.LatLngBounds(
            new google.maps.LatLng(-10, 94),
            new google.maps.LatLng(5, 143)
        );

        areaGridsRequested = [];
        for (var i = 0; i < 180; i++) {
            areaGridsRequested[i] = [];
            for (var j = 0; j < 90; j++) {
                areaGridsRequested[i][j] = null;
            }
        }

        dayNight = new DayNightOverlay({
            map: gmaps,
            fillColor: 'rgba(0,0,0,0.1)',
        });
        dayNightTimer = $$.timer(function () {
            dayNight.setDate($$.date());
        });

        page.done();
    };
    function zoomChanged() {
        var zoom = gmaps.getZoom();

        $$(50, function () {
            if (zoom >= 5 && zoom <= 9) {
                if (areaGroup == null) {
                    areaGroup = "loading";
                    $$.get(15001, { ia: "G" }, function (d) {
                        areaGroup = d.po;

                        var idx = 0;
                        areaGroupMarkers = [];

                        $.each(areaGroup, function (gi, gv) {
                            areaGroupMarkers[idx] = ui.box(uiPane)({ hide: true, height: 25, color: 40, border: { size: 1, color: 0 }, z: 15 });
                            var label = ui.text(areaGroupMarkers[idx])({ font: 10, text: zoom == 5 ? gv[4] : gv[3], noBreak: true, left: 6, top: 5, color: 100 });
                            areaGroupMarkers[idx].data("label", label);
                            areaGroupMarkers[idx].width(label.width() + 15);
                            idx++;
                        });

                        gmapsOverlay.draw();

                    });
                }
            }
            if (zoom >= 6 && zoom <= 11) {
                if (areaWitel == null) {
                    areaWitel = "loading";
                    $$.get(15001, { ia: "W" }, function (d) {
                        areaWitel = d.po;

                        var idx = 0;
                        areaWitelMarkers = [];
                        $.each(areaWitel, function (wi, wv) {
                            areaWitelMarkers[idx] = ui.icon(uiPane, "M0,92.375l46.188-80h92.378l46.185,80l-46.185,80H46.188L0,92.375z")({
                                color: "accent",
                                colorOpacity: .5,
                                rotation: 90,
                                z: 14,
                                hide: true,
                                tooltip: wv[3],
                                stroke: { color: 0, size: 1 }
                            });
                            idx++;
                        });

                        gmapsOverlay.draw();

                    });
                }
            }
        });
    };
    function centerChanged() {
        var ctr = gmaps.getCenter();
        if (validBounds.contains(ctr)) {
            validCenter = ctr;
            return;
        }
        //gmaps.setCenter(validCenter);
    };
    function boundsChanged() {
        var zoom = gmaps.getZoom();
        if (zoom >= 10) {
            clearTimeout(boundRequestTOID);
            boundRequestTOID = setTimeout(boundRequest, 200);
        }
    };
    function latLngToGrid(loc) {
        var lat = loc.lat();
        var lng = loc.lng();

        var latGrid = Math.floor((lat + 90) / 2);
        var lngGrid = Math.floor((lng + 180) / 2);

        if (latGrid == 90) latGrid = 89;
        if (lngGrid == 180) lngGrid = 179;

        return { x: lngGrid, y: latGrid };
    };
    function boundRequest() {
        var ctr = gmaps.getCenter();
        var bnd = gmaps.getBounds();
        var grid = latLngToGrid(ctr);
                
        var x = grid.x;
        var y = grid.y;
        var nreq = [];

        for (var ix = -1; ix <= 1; ix++) {
            for (var iy = -1; iy <= 1; iy++) {
                var fx = x + ix;
                var fy = y + iy;
                if (fx < 0) fx = 180 + ix;
                if (fy < 0) fy = 90 + iy;

                if (areaGridsRequested[fx][fy] == null) {
                    areaGridsRequested[fx][fy] = ui.box(uiPane)({
                        hide: true,
                        //color: ui.color($$.random(0, 255), $$.random(0, 255), $$.random(0, 255)),
                        overflow: true
                    });
                    nreq.push(fx + "," + fy);
                }
            }
        }

        if (nreq.length > 0) {
            var sr = nreq.join(";");
            center.startLoading();
            $$.get(15001, { ia: "R", r: sr }, function (d) {
                center.endLoading();
                if (area == null) area = [];
                area = $.merge(area, d.po);

                var edges = [];
                $.each(d.eg, function (ai, av) {
                    edges.push([av[0], av[1], av[2], av[3]]);
                });

                $.each(d.po, function (ai, av) {
                    var grid = areaGridsRequested[av[3]][av[4]];

                    var areaMarker = ui.icon(grid, "M0,92.375l46.188-80h92.378l46.185,80l-46.185,80H46.188L0,92.375z")({
                        size: [10, 10],
                        color: "blue",
                        colorOpacity: .4,
                        rotation: 90,
                        z: 12,
                        stroke: { color: 0, size: 1 },
                        hide: true
                    });
                    var label = ui.text(grid)({ text: av[5], font: 9 });

                    areaMarker.data("label", label);
                    areaMarker.data("lat", av[1]);
                    areaMarker.data("lng", av[2]);

                    var areaMarkers = grid.data("areaMarkers");
                    if (areaMarkers == null) {
                        areaMarkers = [];
                        grid.data("areaMarkers", areaMarkers);
                    }
                    areaMarkers.push(areaMarker);

                    var areaEdges = areaMarker.data("areaEdges");
                    if (areaEdges == null) {
                        areaEdges = [];
                        areaMarker.data("areaEdges", areaEdges);
                    }

                    $.each(edges, function (ei, ev) {
                        if (ev[0] == av[0]) {
                            var areaEdge = ui.raphael(grid)({
                                hide: true,
                                //color: ui.color($$.random(0, 255), $$.random(0, 255), $$.random(0, 255)), opacity: 0.3
                            });
                            //debug(ev[0], ev[1], ev[2]);
                            areaEdge.data("lat", ev[1]);
                            areaEdge.data("lng", ev[2]);
                            var path = areaEdge.paper().path("");
                            path.attr({ "stroke-width": 1, "stroke": ui.color(50), "stroke-opacity": .9 });
                            areaEdge.data("path", path);
                            areaEdge.data("dest", ev[3]);
                            areaEdges.push(areaEdge);
                        }
                    });
                });

                gmapsOverlay.draw();
            });
        }
    };


    ui("jovice_network", {
        init: function (p) {
            page = center.init(p);
            mapArea = ui.box(p)({ size: ["100%", "100%"], z: 1 });

            center.loadGoogleMaps(onGMapsScriptLoaded);
        },
        start: function (p) {

            p.done();
        },
        unload: function (p) {

            if (areaGroupMarkers != null) {
                $.each(areaGroupMarkers, function (mi, mv) {
                    mv.remove();
                });
            }
            if (areaWitelMarkers != null) {
                $.each(areaWitelMarkers, function (mi, mv) {
                    mv.remove();
                });
            }

            areaGroup = null;
            areaWitel = null;
            areaGridsRequested = null;

            mapArea.remove();
            gmaps = null;
            gmapsOverlay = null;
            uiPane = null;
            markerPane = null;

            area = null;

            $$.removeTimer(dayNightTimer);
            p.done();
        }
    });

})();