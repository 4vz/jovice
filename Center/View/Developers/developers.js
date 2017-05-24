(function () {

    var page, doc;
    ui("developers", {
        init: function (p) {
            page = center.init(p);

            doc = ui.doc(p, {
                title: "We're Just Getting Started",


            });

            doc.content([
                "Cent",
                "The Nearby Messages API has the potential to be battery-intensive due to the way it uses Bluetooth and other device resources to detect and communicate with nearby devices. To ensure that users are in control of the experience, an opt-in dialog is presented the first time the user connects to the Nearby Messages API. The user must provide consent for Nearby to utilize the required device resources.",
            ]);


            




            

            p.done();
        },
        start: function (p) {
            p.done();
        },
        resize: function (p) {
        }
    });
})();