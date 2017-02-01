(function () {

    var page;
    ui("user_signin", {
        init: function (p) {
            page = center.init(p);
            
            var b = ui.text(p)({ text: "SIGN IN", position: [100, 100] });


            p.done();
        },
        start: function (p) {
            center.setSearchBoxValue(null);
            p.done();
        },
        resize: function (p) {
        }
    });
})();