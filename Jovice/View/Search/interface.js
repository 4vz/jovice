
if (create) {
    b({
        height: 80, top: ei * 80,
    });
    r.nodeName = ui.text(b)({
        font: ["head", 18], color: "accent", top: 18, left: 20, nobreak: true, text: "PE-D2-CKA"
    });
    r.nodeName.resize = function () {

    };
    r.interfaceName = ui.text(b)({
        font: ["head", 17], color: 25, top: 19, left: 210, nobreak: true, text: "Gi1/2/3.4567"
    });
    r.description = ui.text(b)({
        font: ["body", 17], color: 25, top: 45, leftRight: [20, 20], nobreak: true, truncate: true, text: "Deskripsi disini"
    });

    r.statusIcon = ui.icon(b, "arrow2")({
        size: [15, 15], top: 21, left: 400
    });
    r.statusText = ui.text(b)({
        font: ["body", 15], color: 25, top: 18, text: "ADMIN DOWN", left: 430
    });
    r.protocolIcon = ui.icon(b, "arrow2")({
        size: [15, 15], top: 21, left: 590
    });
    r.protocolText = ui.text(b)({
        font: ["body", 15], color: 25, top: 18, text: "ADMIN DOWN", left: 620
    });

    r.resize = function (d) {

    };
}

var status = e("I_Status");
var protocol = e("I_Protocol");

r.nodeName.text(e("NO_Name"));
r.interfaceName.text(formatInterfaceName(e("I_Name"), e("NO_Manufacture")));
r.interfaceName.color(protocol ? 25 : 55);
r.description.text(e("I_Description"));

r.statusIcon({ rotation: status ? 180 : 0, color: status ? "accent" : 25 });
r.statusText({ text: status ? "UP" : "ADMIN DOWN", color: status ? 25 : 55 });

r.protocolIcon({ rotation: protocol ? 180 : 0, color: protocol ? "accent" : 25 });
r.protocolText({ text: protocol ? "UP" : "DOWN", color: protocol ? 25 : 55 });