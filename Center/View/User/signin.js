(function () {

    var page;
    ui("user_signin", {
        init: function (p) {
            page = center.init(p);

            center.hideToolbar();

            var boleft = ui.box(p)({
                width: "50%",
                left: "50%",
                height: "100%",
                color: 91
            });
            
            var b = ui.text(p)({ text: "SIGN IN", font: ["body", 25], color: 30, left: 100, top: 50 });
            

            //center.showSignIn();

            p.done();
        },
        start: function (p) {
            center.setSearchBoxValue(null);
            p.done();
        },
        leave: function(p){
            center.showToolbar();
            p.done();
        },
        resize: function (p) {
        }
    });
})();