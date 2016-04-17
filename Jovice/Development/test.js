(function () {

    ui("test", {
        init: function (p) {
            
            ui.textarea(p)({
                size: [200, 200],
                readonly: true
            });


            p.done();
        },
        start: function (p) {

            p.done();
        },
        resize: function (p) {

        }
    });

})();