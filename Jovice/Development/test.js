(function () {

    ui("test", {
        init: function (p) {
            
            var v = jovice.vselect(p);

            v.add("powercut", "DISCONNECTED");
            v.add("search", "SEARCH");
            v.add("refresh", "REFRESH");

            p.done();
        },
        start: function (p) {

            p.done();
        },
        resize: function (p) {

        }
    });

})();