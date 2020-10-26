
(function () {

    $$.page("stats", {

        init: function (p) {

            var boxatas = $$.box(p)({ size: ["100%", 50], color: 50 });

            var ins = $$.text(boxatas)({
                text: "iNecrow Stats",
                position: [10, 10],
                font: 25,
                color: 100
            });

            var live = $$.text(boxatas)({ text: "LIVE", color: "lightgreen", right: 10, top: 15 });
            var timedate = $$.text(boxatas)({ text: "30 JULY 2020 00:00:00 WIB", color: 85, attach: [ins, "right", 13, 8] });

            $$.timer(function (d) {
                timedate.text($$.date("{DD} {MMMM} {YYYY} {HH}:{mm}:{ss}", { timeZone: 0 }) + " UTC");
            });

            var boxpage = $$.box(p)({ width: "100%", topBottom: [50, 0], scroll: {}, color: 100 });



            var boxsa = $$.box(boxpage)({ size: ["100%", 100], color: 80 });
            $$.text(boxsa)({ text: "Server Status", font: 20, position: [10, 10] });

            var boxsa1 = $$.box(boxsa)({ size: [150, 60], color: 85, top: 40, left: 0 });
            var boxsa2 = $$.box(boxsa)({ size: [150, 60], color: 88, attach: [boxsa1, "right", 0, 0] });
            var boxsa3 = $$.box(boxsa)({ size: [150, 60], color: 85,  attach: [boxsa2, "right", 0, 0] });
            var boxsa4 = $$.box(boxsa)({ size: [150, 60], color: 88, attach: [boxsa3, "right", 0, 0] });
            var boxsa5 = $$.box(boxsa)({ size: [150, 60], color: 88, attach: [boxsa4, "right", 0, 0] });
            var boxsa6 = $$.box(boxsa)({ size: [150, 60], color: 88, attach: [boxsa5, "right", 0, 0] });

            $$.text(boxsa1)({ text: "GAIA", color: "green", weight: "500", position: [10, 10] });
            $$.text(boxsa1)({ text: "10.62.175.94", weight: "500", position: [10, 30] });
            $$.text(boxsa2)({ text: "TERRA", color: "green",  weight: "500", position: [10, 10] });
            $$.text(boxsa2)({ text: "10.62.175.91", weight: "500", position: [10, 30] });
            $$.text(boxsa3)({ text: "CHIKYU", color: "green", weight: "500", position: [10, 10] });
            $$.text(boxsa3)({ text: "10.62.166.183", weight: "500", position: [10, 30] });
            $$.text(boxsa4)({ text: "ARD", color: "green", weight: "500", position: [10, 10] });
            $$.text(boxsa4)({ text: "10.60.166.86", weight: "500", position: [10, 30] });
            $$.text(boxsa5)({ text: "MIDGARD", color: "green", weight: "500", position: [10, 10] });
            $$.text(boxsa5)({ text: "10.62.164.134", weight: "500", position: [10, 30] });
            $$.text(boxsa6)({ text: "EARTH", color: "green", weight: "500", position: [10, 10] });
            $$.text(boxsa6)({ text: "10.62.166.53", weight: "500", position: [10, 30] });

            var boxoa = $$.box(boxpage)({ size: ["100%", 160], color: 90, attach: [boxsa, "bottom",  0, 0] });
            $$.text(boxoa)({ text: "Overall Activity", font: 20, position: [10, 10]  });

            var boxoa1 = $$.box(boxoa)({ size: [350, 120], color: 95, top: 40, left: 0 });
            var boxoa3 = $$.box(boxoa)({ size: [350, 120], color: 92, top: 40, left: 0, attach: [boxoa1, "right", 0, 0] });

            $$.text(boxoa1)({ text: "UNIQUE API CONNECTIONS MTD", weight: "300", position: [10, 10] });
            var oaconnmtd = $$.text(boxoa1)({ text: "...", font: 51, left: 10, top: 30 });
            $$.text(boxoa3)({ text: "UNIQUE API CONNECTIONS YTD", weight: "300", position: [10, 10] });
            var oaconnytd = $$.text(boxoa3)({ text: "...", font: 51, left: 10, top: 30 });

            var boxtr = $$.box(boxpage)({ size: ["100%", 600], color: 85, attach: [boxoa, "bottom", 0, 0] });
            $$.text(boxtr)({ text: "2020 Trends", font: 20, position: [10, 10] });

            var textm1 = $$.text(boxtr)({ text: "JANUARY", font: 12, color: 30, position: [10, 55] });
            var boxm1 = $$.box(boxtr)({ size: [10 + (9194 / 50000) * 240, 20], color: 50, position: [80, 50] });
            var valm1 = $$.text(boxtr)({ text: "9194", font: 12, color: 30, attach: [boxm1, "right", 10, 5] });

            var textm2 = $$.text(boxtr)({ text: "FEBRUARY", font: 12, color: 30, position: [10, 95] });
            var boxm2 = $$.box(boxtr)({ size: [10 + (10828 / 50000) * 240, 20], color: 50, position: [80, 90] });
            var valm2 = $$.text(boxtr)({ text: "10828", font: 12, color: 30, attach: [boxm2, "right", 10, 5] });

            var textm3 = $$.text(boxtr)({ text: "MARCH", font: 12, color: 30, position: [10, 135] });
            var boxm3 = $$.box(boxtr)({ size: [10 + (12543 / 50000) * 240, 20], color: 50, position: [80, 130] });
            var valm3 = $$.text(boxtr)({ text: "12543", font: 12, color: 30, attach: [boxm3, "right", 10, 5] });

            var textm4 = $$.text(boxtr)({ text: "APRIL", font: 12, color: 30, position: [10, 175] });
            var boxm4 = $$.box(boxtr)({ size: [10 + (12264 / 50000) * 240, 20], color: 50, position: [80, 170] });
            var valm4 = $$.text(boxtr)({ text: "12264", font: 12, color: 30, attach: [boxm4, "right", 10, 5] });

            var textm5 = $$.text(boxtr)({ text: "MAY", font: 12, color: 30, position: [10, 215] });
            var boxm5 = $$.box(boxtr)({ size: [10 + (22919 / 50000) * 240, 20], color: 50, position: [80, 210] });
            var valm5 = $$.text(boxtr)({ text: "22919", font: 12, color: 30, attach: [boxm5, "right", 10, 5] });

            var textm6 = $$.text(boxtr)({ text: "JUNE", font: 12, color: 30, position: [10, 255] });
            var boxm6 = $$.box(boxtr)({ size: [10 + (40582 / 50000) * 240, 20], color: 50, position: [80, 250] });
            var valm6 = $$.text(boxtr)({ text: "40582", font: 12, color: 30, attach: [boxm6, "right", 10, 5] });

            var textm7 = $$.text(boxtr)({ text: "JULY", font: 12, color: 30, position: [10, 295] });
            var boxm7 = $$.box(boxtr)({ size: [10 + (49008 / 50000) * 240, 20], color: 50, position: [80, 290] });
            var valm7 = $$.text(boxtr)({ text: "49008", font: 12, color: 30, attach: [boxm7, "right", 10, 5] });

            var textm8 = $$.text(boxtr)({ text: "AUGUST", font: 12, color: 30, position: [10, 335] });
            var boxm8 = $$.box(boxtr)({ size: [10 + (60251 / 50000) * 240, 20], color: 50, position: [80, 330] });
            var valm8 = $$.text(boxtr)({ text: "60251", font: 12, color: 30, attach: [boxm8, "right", 10, 5] });

            var textm9 = $$.text(boxtr)({ text: "SEPTEMBER", font: 12, color: 30, position: [10, 375] });
            var boxm9 = $$.box(boxtr)({ size: [10 + (109104 / 50000) * 240, 20], color: 50, position: [80, 370] });
            var valm9 = $$.text(boxtr)({ text: "109104", font: 12, color: 30, attach: [boxm9, "right", 10, 5] });

            var textm10 = $$.text(boxtr)({ text: "OCTOBER", font: 12, color: 30, position: [10, 415] });
            var boxm10 = $$.box(boxtr)({ size: [10, 20], color: 70, position: [80, 410] });
            var valm10 = $$.text(boxtr)({ text: "...", font: 12, color: 30, attach: [boxm10, "right", 10, 5] });


            $$(function () {
                $$.get(55001, {}, function (d) {
                    oaconnmtd.text(d.connmtd);
                    oaconnytd.text(d.connytd);
                    //oaapimtd.text(d.quemtd);
                    //oaapiytd.text(d.queytd);
                    //oaahcmtd.text(d.apihc);
                    boxm10.width(10 + (d.connmtd / 50000) * 240);
                    valm10.text(d.connmtd);
                });
            }, 5000, -2);

            /*

            var box1 = $$.box(p)({ size: ["100%", 80], color: 85, attach: [boxatas, "bottom", 0, 0] });

            var text1 = $$.text(box1)({ text: "Telkom-2", font: 30, position: [10, 20], color: 40 });

            $$.icon(box1, satVect)({ attach: [text1, "right", 20, -30], color: 50, size: 100 });

            var box2 = $$.box(p)({ size: ["100%", 80], color: 90, attach: [box1, "bottom", 0, 0] });

            var text2 = $$.text(box2)({ text: "Telkom-3S", font: 30, position: [10, 20], color: 40 });

            $$.icon(box2, satVect)({ attach: [text2, "right", 20, -30], color: 50, size: 100 });

            var box3 = $$.box(p)({ size: ["100%", 80], color: 85, attach: [box2, "bottom", 0, 0] });

            var text3 = $$.text(box3)({ text: "Merah Putih", font: 30, position: [10, 20], color: 40 });

            $$.icon(box3, satVect)({ attach: [text3, "right", 20, -30], color: 50, size: 100 });

            var boxtengah = $$.box(p)({ size: ["100%", 50], color: 50, attach: [box3, "bottom", 0, 0] });

            $$.text(boxtengah)({
                text: "Payload Capacity",
                position: [10, 10],
                font: 25,
                color: 255
            });*/

            p.done();
        }

    });

})();

