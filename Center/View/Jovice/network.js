(function () {

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

    var mtArea;
    var mtCanvas;


    getVisibleDimensions_ = function (projection, margin) {
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

    function onGMapsScriptLoaded() {
        
        (function () {
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

                var DayNightOverlay = function (opt_params) {
                    opt_params = opt_params || {};
                    this.canvas_ = null;
                    this.fillColor_ = opt_params.fillColor || 'rgba(0,0,0,0.5)';
                    this.id_ = opt_params.id || null;
                    this.date_ = opt_params.date || null;

                    if (typeof opt_params.map != 'undefined') {
                        this.setMap(opt_params.map);
                    }

                    this.smallZoomThreshold = opt_params.smallZoomThreshold || 3;
                };

                DayNightOverlay.prototype = new google.maps.OverlayView();
                DayNightOverlay.NORTH_ = new google.maps.LatLng(85, 0);
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

                DayNightOverlay.prototype.setDate = function (date) {
                    this.date_ = date;

                    // Redraw the line if we're added to a maps canvas
                    if (this.canvas_ !== null) {
                        this.draw();
                    }
                };
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
                DayNightOverlay.prototype.createTerminatorFunc_ = function (viewport, world) {

                    var date = this.date_ ? this.date_ : new Date();
                    var TWO_PI = 2 * Math.PI;

                    var WORLD_WIDTH = world.width;
                    var WORLD_HEIGHT = world.height;
                    var HALF_WORLD_HEIGHT = world.height / 2;

                    var VISIBLE_WIDTH = viewport.width;
                    var VISIBLE_HEIGHT = viewport.height;

                    var WORLD_OFFSET_X = viewport.x - world.x;
                    var WORLD_OFFSET_Y = viewport.y - world.y;

                    var X_SCALE = TWO_PI / WORLD_WIDTH;
                    var Y_SCALE = WORLD_HEIGHT / Math.PI;

                    var TIME_SECS = date.getUTCHours() * 3600 +
                        date.getUTCMinutes() * 60 +
                        date.getUTCSeconds();

                    var NOON_SECS = 86400 / 2;
                    var PI_STEP = TWO_PI / 86400;
                    var TIME_OFFSET_X = (TIME_SECS + NOON_SECS) * PI_STEP;
                    var DAY_OF_YEAR = this.getDayOfYear_(date);
                    var VERNAL_EQUINOX = this.getDayOfYear_(
                        new Date(Date.UTC(date.getFullYear(), 2, 20))
                        );

                    var MAX_DECLINATION = 23.44 * Math.PI / 90;
                    var DECLINATION = Math.sin(TWO_PI * (DAY_OF_YEAR - VERNAL_EQUINOX) / 365) *
                        MAX_DECLINATION;

                    return function (x) {
                        var worldX = (x + WORLD_OFFSET_X) * X_SCALE + TIME_OFFSET_X;
                        var worldY = Math.atan(-Math.cos(worldX) / Math.tan(DECLINATION));
                        worldY = HALF_WORLD_HEIGHT + worldY * Y_SCALE;
                        return Math.min(VISIBLE_HEIGHT, Math.max(0, worldY - WORLD_OFFSET_Y));
                    };
                };


                DayNightOverlay.prototype.isNorthernSun_ = function (date) {
                    var vernalEq = new Date(Date.UTC(date.getFullYear(), 2, 19));
                    var autumnalEq = new Date(Date.UTC(date.getFullYear(), 8, 18));

                    return (date.getTime() > vernalEq.getTime()) &&
                        (date.getTime() <= autumnalEq.getTime());
                };


                DayNightOverlay.prototype.getDayOfYear_ = function (date) {
                    // Yes, the month has to be zero thanks to JavaScript's great Date class.....
                    var firstDay = new Date(Date.UTC(date.getFullYear(), 0, 1));

                    return Math.ceil((date.getTime() - firstDay.getTime()) / 86400000);
                };

                return DayNightOverlay;
            }));
        })();

        gmaps = new google.maps.Map(mapArea.dom(), {
            center: { lat: -1.548514, lng: 119.527694 },
            //center: { lat: -6.175403, lng: 106.827114 },
            zoom: 5,
            disableDefaultUI: true
        });

        gmapsOverlay = new google.maps.OverlayView();

        gmapsOverlay.onAdd = function () {

            mtArea = ui.box($(gmapsOverlay.getPanes().overlayMouseTarget));
            mtArea.size(200, 200);
            mtArea.opacity(0.5);
            mtArea.color("red");            

            //canvasMouseTarget
            //this.canvas_ = document.createElement('canvas');
            //this.canvas_.style.position = 'absolute';

            //if (this.id_) {
            //    this.canvas_.id = this.id_;
            //}

            //this.getPanes().overlayLayer.appendChild(this.canvas_);
        };
        gmapsOverlay.onRemove = function () {
        };
        gmapsOverlay.draw = function () {

            var projection = gmapsOverlay.getProjection();
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


        };

        gmapsOverlay.setMap(gmaps);
        gmaps.setOptions({ styles: style });

        var dayNight = new DayNightOverlay({
            map: gmaps,
            fillColor: 'rgba(0,0,0,0.1)',
        });
        $$.timer(function () {
            dayNight.setDate($$.date());
        });

        page.done();
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

            mapArea.remove();
            gmaps = null;
            gmapsOverlay = null;

            $$.removeTimer(dayNightTimer);
            p.done();
        }
    });

})();