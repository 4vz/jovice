(function () {

    var warnaTextHeading = "#440044";

    $$.page("main", {
        init: function (p) {
            page = center.init(p);

            var heading = $$.box(page)({
                color: warnaTextHeading
            });

            var icondragonball = $$.svg("asdasdasdasdadasdasdasd")({
                x: 50,
                y: 20,
                color: "red"
            });

            icondragonball.x(50);
            icondragonball.y(20);
            icondragonball.color("red");

            $$.get(5000, { namaku: "afis" }, function (respon) {
                alert(respon.afis);
            });

            p.done();
        },
        start: function (p) {
            p.done();
        }
    });
})();