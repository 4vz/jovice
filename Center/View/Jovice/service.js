(function () {

    var page;
    var transfer = null;

    var topbox, serviceID, customerName, serviceType;


    $$("jovice_service", {
        init: function (p) {
            page = center.init(p);
            transfer = p.transfer();



            //debug(p.transfer());
            //debug(page.endUrl());

            topbox = ui.box(p)({ color: "accent", position: [20, 20], size: [30, 3] });
            serviceID = ui.text(p)({ position: [20, 35], text: "SID", font: ["body", 15], color: "accent", weight: "600", noBreak: true, clickToSelect: true, cursor: "copy" });
            customerName = ui.text(p)({ position: [20, 60], text: "CUSTOMER_NAME", font: ["body", 20], color: 25, weight: "600", noBreak: true, truncate: true });
            serviceType = ui.text(p)({ position: [20, 90], text: "SERVICE_TYPE", font: ["body", 15], color: 25 });

            p.done();
        },
        start: function (p) {

            if (transfer != null) {
                serviceID.text(transfer.serviceID);
                customerName.text(transfer.customerName);
                serviceType.text(transfer.serviceType);

                topbox.$.css({ y: 20, opacity: 0 }).transition({ y: 0, opacity: 1, duration: 400 });
                serviceID.$.css({ y: 20, opacity: 0 }).transition({ y: 0, opacity: 1, duration: 400 });
                customerName.$.delay(100).css({ x: -20, opacity: 0 }).transition({ x: 0, opacity: 1, duration: 400 });
                serviceType.$.delay(200).css({ x: -20, opacity: 0 }).transition({ x: 0, opacity: 1, duration: 400 });

                //debug(transfer.entries);

                $$(800, function () {
                    p.done();
                });
            }
            else p.done();

            
        },
        resize: function (p) {

        },
        local: function (p) {

        },
        unload: function (p) {
            p.done();
        }
    });
})();