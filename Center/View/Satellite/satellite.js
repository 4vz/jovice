(function () {


    var page;
    $$.page("satellite", {
        init: function (p) {
            page = center.init(p);

            $$.setDomain("SATELLITE OPERATION");

            var pageBox = $$.box(page)({ scroll: { horizontal: false }, size: "100%" });


            var satBox = $$.box(pageBox)({ height: 120, width: "100%", top: 0, color: 92, relative: true });

            var satTitle = $$.text(satBox)({ text: "SATELLITES", weight: "500", font: 20, color: 40, position: [20, 40] });

            var satVect = "M609.6,316.6l-14-64.6c-0.4-1.9-2.3-3-4.1-2.6l-252.3,62.4c-1.9,0.5-3,2.3-2.6,4.2l2.3,10l-23.2,5.1l-24.1,15.2l-0.8-2.2h1.3v-3.2	h-2.3l-1.1-2.3l-3.2-3l-6.6-14.2l1.9-1.1l-1.3-3.2h-2l-1.1-2.1l-5,3.9c-1.3-0.4-2.9-0.7-3.6-0.1v-1.3c0,0,2-0.4,1.4-2.5l3.8-1.7	l-3.5-16.8c4.4-0.7,8.2-3,10.6-6.7c5-7.8,1.8-18.8-7.3-24.7c-9-5.9-20.4-4.4-25.5,3.3c-5,7.8-1.8,18.8,7.3,24.7	c4,2.6,8.5,3.8,12.7,3.5l3.1,15.7l-1.9,0.5c0,0-2.5-2.1-4.8,0c0,0-3.8,4.4,2.5,5.7l0.3,1.1c0,0-1.3,1.1-2.1,2.5l-4.5,0.7l-1.3-4	h-1.9l0.4,3.3c-0.8-1.1-2.3-1.6-2.3-1.6l-4.4,1.3c-3.8,2-2.5,4.8-1.3,6.5l-5.3,1.6l-0.4-1.7l-4.7,1.5l-0.2-0.1	c1.4-1.9,2.9-5.2-2.7-7.6c0,0-7-2.1-6.7,4.2l-6.8-5.5c0.1-0.1,0.2-0.3,0.3-0.4c8.1-11.5,5-27.7-7-36.2c-12-8.4-28.3-5.9-36.4,5.6	s-5,27.7,7,36.2c6.5,4.5,14.2,5.9,21.2,4.4c-2,0.7-5.3,2.3-3.2,6.3l-4.5,0.9l-2.7,12.5l3.9,6.2l-5.4,5.3L200,372h-12.6l-17.8,7.5	l-1.7-7.9c-0.4-1.9-2.2-2.8-3.9-2.1L3.3,434c-1.8,0.7-3,2.8-2.7,4.7l6.7,44c0.3,1.9,2,2.9,3.8,2.2l164.5-60.1	c1.8-0.7,2.9-2.7,2.6-4.6l-1.8-8.5l20-7.2l12.7-12.1l11.2,25l-1.7,2.7l0.2,3.3h2.7v1.3h4v-2.3l4.2-1.9l3.2,14l9.7-2.6	c5.3,11.4,17.3,17.2,27,12.9c9.9-4.4,13.7-17.4,8.5-29.2c-0.4-0.8-0.8-1.7-1.3-2.5l0,0l3.5-0.2v-4.7h-2.3l-1.9-10.3l8.5-8.1l1,3	l2.1-0.7l-1.3-4.2l2-1.9c0.8,1.4,2,1.9,2,1.9c7,1.9,5.9-3,5.9-3h3l4.4,7.4c-8.1,6.6-10.8,19.3-5.7,30.9	c5.9,13.5,20.1,20.3,31.6,15.2s16.1-20.1,10.2-33.6c-5.9-13.5-20.1-20.3-31.6-15.2c-0.8,0.4-1.6,0.8-2.3,1.2l-6.2-8.2h-4.2l5.9-6.6	l-1-1.3l1-0.9l-0.7-1.6c1-1.1,0.7-2.6,0.7-2.6c-0.6-1.6-1.5-1.7-2.3-1.4l-2.9-6.8l30.4,4.8L347,361l2.3,9.9c0.4,1.9,2.3,3,4.2,2.7	l253.5-52.8C608.8,320.3,610,318.5,609.6,316.6z M188.1,374.6l6,26.5l-2.6,1l-6.6-26L188.1,374.6z M170.5,382.7l11.4-5.3l5.9,25.9	l-12.1,4.4L170.5,382.7z M196.6,399l-5.6-24.6l10.4,0.5l6,13.5L196.6,399z M220.7,330.1l-2.7,6.6l-3.4-0.4c0,0,0.1-5.9-5.8-6l0,0	c4.8-1.3,9.2-4,12.6-8l7.1,6.5L220.7,330.1z M320.2,334l2.7-0.7l6.7,28.9l-3,0.8L320.2,334z M294,356.4v-7.9l22-13.4l0.1-0.1	l5.9,27.6l-27.1-3.8L294,356.4z M333.1,361.5l-6-29l12.6-3l6.7,28.6L333.1,361.5z";

            var t2lon = $$.text(satBox)({ text: "156.9702 deg", weight: "500", font: 15, color: "blue-20", position: [235, 85] })
            var t3slon = $$.text(satBox)({ text: "157.0063 deg", weight: "500", font: 15, color: "blue-20", position: [535, 85] })


            $$.icon(satBox, satVect)({ size: 100, color: 45, left: 220, top: -10 });
            $$.text(satBox)({ text: "TELKOM-2", weight: "600", font: 15, color: 40, position: [235, 65] });
            $$.text(satBox)({ text: "NOMINAL", font: 25, weight: "500", color: "green+20", position: [350, 40] });
            
            $$.icon(satBox, satVect)({ size: 100, color: 45, left: 520, top: -10 });
            $$.text(satBox)({ text: "TELKOM-3S", weight: "600", font: 15, color: 40, position: [535, 65] });
            $$.text(satBox)({ text: "NOMINAL", font: 25, weight: "500", color: "green+20", position: [650, 40] });


            var mcsBox = $$.box(pageBox)({ height: 320, width: "100%", color: 89, relative: true });
            
            var mcsTitle = $$.text(mcsBox)({ text: "CIBINONG", weight: "500", font: 20, color: 40, position: [20, 60] });
            var mcsTime = $$.text(mcsBox)({ text: "00:00 WIB", weight: "300", font: 15, color: 30, position: [20, 40] });            

            var bcsBox = $$.box(pageBox)({ height: 320, width: "100%", color: 85, relative: true });



            var tlmBox = $$.box(pageBox)({ height: 600, width: "100%", relative: true, color: 82 });

            var tlmTitle = $$.text(tlmBox)({ text: "TELEMETRIES", weight: "500", font: 20, color: 40, position: [20, 40] });

            var bcsTitle = $$.text(bcsBox)({ text: "BANJARMASIN", weight: "500", font: 20, color: 40, position: [20, 60] });
            var bcsTime = $$.text(bcsBox)({ text: "00:00 WITA", weight: "300", font: 15, color: 30, position: [20, 40] });

            var dishFMA = $$.img(mcsBox, "satellite_png_dish")({ size: [67, 100], position: [220, 130] });
            var dishT2 = $$.img(mcsBox, "satellite_png_dish")({ size: [67, 100], position: [420, 130] });
            var dishT3SC = $$.img(mcsBox, "satellite_png_dish")({ size: [67, 100], position: [620, 130] });
            var dishT3SKU = $$.img(mcsBox, "satellite_png_dish")({ size: [67, 100], position: [820, 130] });

            $$.img(mcsBox, "satellite_png_sine1")({ size: 80, position: [220, 50], opacity: 0.3, backgroundScroll: [0, 2] });
            $$.img(mcsBox, "satellite_png_sine1")({ size: 80, position: [420, 50], opacity: 0.3, backgroundScroll: [0, 2] });
            $$.img(mcsBox, "satellite_png_sine1")({ size: 80, position: [620, 50], opacity: 0.3, backgroundScroll: [0, 2] });
            //$$.img(mcsBox, "satellite_png_sine1")({ size: 80, position: [820, 50], opacity: 0.3 });
            $$.img(bcsBox, "satellite_png_sine1")({ size: 80, position: [220, 50], opacity: 0.3, backgroundScroll: [0, 2] });

            var textFMA = $$.text(mcsBox)({ text: "11M FMA", font: 18, color: 30, position: [215, 235] });
            var textT2 = $$.text(mcsBox)({ text: "9M T2", font: 18, color: 30, position: [430, 235] });
            var textT3SC = $$.text(mcsBox)({ text: "9M T3S C", font: 18, color: 30, position: [610, 235] });
            var textT3SKU = $$.text(mcsBox)({ text: "9M T3S KU", font: 18, color: 30, position: [810, 235] });

            $$.text(mcsBox)({ text: "AZ/EL", weight: "700", font: 15, color: 10, position: [155, 265] });
            $$.text(bcsBox)({ text: "AZ/EL", weight: "700", font: 15, color: 10, position: [155, 265] });

            var azelFMA = $$.text(mcsBox)({ text: "83.22 deg, 39.162 deg", weight: "500", font: 15, color: "green-10", position: [215, 265] });
            var azelT2 = $$.text(mcsBox)({ text: "84.6 deg, 32.3 deg", weight: "500", font: 15, color: "green-10", position: [420, 265] });
            var azelT3SC = $$.text(mcsBox)({ text: "60.152 deg, 75.38 deg", weight: "500", font: 15, color: "green-10", position: [610, 265] });
            var azelT3SKU = $$.text(mcsBox)({ text: "60.152 deg, 75.38 deg", weight: "500", font: 15, color: "green-10", position: [810, 265] });

            //$$.text(mcsBox)({ text: "OK", font: 35, weight: "500", color: "green+20", position: [230, 60] });
            //$$.text(mcsBox)({ text: "OK", font: 35, weight: "500", color: "green+20", position: [380, 60] });
            //$$.text(mcsBox)({ text: "OK", font: 35, weight: "500", color: "green+20", position: [530, 60] });
            //$$.text(mcsBox)({ text: "OK", font: 35, weight: "500", color: "green+20", position: [680, 60] });

            var dishTHA = $$.img(bcsBox, "satellite_png_dish")({ size: [67, 100], position: [220, 130] });

            var textTHA = $$.text(bcsBox)({ text: "9M THA", font: 18, color: 30, position: [215, 235] });
            var azelTHA = $$.text(bcsBox)({ text: "N/A deg, N/Adeg", weight: "500", font: 15, color: "green-10", position: [215, 265] });

            //$$.text(bcsBox)({ text: "OK", font: 35, weight: "500", color: "green+20", position: [230, 60] });

            debug($$.date());
            $$.timer(function (d) {
                mcsTime.text($$.date("{HH}:{mm}:{ss}", { timeZone: 7 }) + " WIB");
                bcsTime.text($$.date("{HH}:{mm}:{ss}", { timeZone: 8 }) + " WITA");

                var ra1, ra2;

                ra1 = 0.0052 * $$.random();
                ra2 = 0.0011 * $$.random();
                azelFMA.text((83.22 + ra1).toFixed(3) + " deg, " + (39.162 + ra2).toFixed(3) + " deg");
                ra1 = 0.0042 * $$.random();
                ra2 = 0.0031 * $$.random();
                azelT3SC.text((60.152 + ra1).toFixed(3) + " deg, " + (75.38 + ra2).toFixed(3) + " deg");
                ra1 = 0.0032 * $$.random();
                ra2 = 0.0091 * $$.random();
                azelT3SKU.text((60.152 + ra1).toFixed(3) + " deg, " + (75.38 + ra2).toFixed(3) + " deg");

                ra1 = 0.0052 * $$.random();
                ra2 = 0.0011 * $$.random();
                azelTHA.text((86.571 + ra1).toFixed(3) + " deg, " + (40.623 + ra2).toFixed(3) + " deg");

                //azelT2.text(azelTHA.text());
                //azelT2.text("0 deg, 0 deg");

                t2lon.text((156.9821).toFixed(4) + " deg");
                ra1 = 0.0005;// * $$.random();
                t3slon.text((117.9781 + ra1).toFixed(4) + " deg");
            });

            p.done();
        },
        start: function (p) {
            p.done();
        },
        unload: function (p) {
            //$$.setDomain(null);
            p.done();
        }
    });
})();