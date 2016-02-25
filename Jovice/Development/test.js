(function () {

    ui("test", {
        init: function (p) {
            
            var color = "#ff0055";

            color = $$.color(color, 0.5);

            debug(color);


            p.done();
        },
        start: function (p) {

            p.done();
        },
        resize: function (p) {

        }
    });

})();