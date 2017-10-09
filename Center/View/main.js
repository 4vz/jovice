(function () {

    var page;
    $$.page("main", {
        init: function (p) {
            page = center.init(p);
            p.done();
        },
        start: function (p) {
            p.done();
        }
    });
})();