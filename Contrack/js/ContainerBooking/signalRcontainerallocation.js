
var hub = $.connection.containerHub;
$.connection.hub.start().done(function () {
    $(".LocationUuid").each(function () {
        var locationuuid = $(this).val();
        $(this).parent().find(".ContainerModelUuid").each(function () {
            var modeluuid = $(this).val();
            hub.server.joinGroup(locationuuid, modeluuid);
            console.log("joinGroup");
        });
    });
});
// -- containerBooked Notification
hub.client.containerBooked = function (cid, bookinguuid) {
    console.log("containerBooked EVENT RECEIVED", cid, bookinguuid);
    if (bookinguuid != current_bookinguuid) {
        $("#chk_" + cid)
            .prop("checked", false)
            .prop("disabled", true);
        $("#chk_" + cid).closest("label").find(".equipname")
            .addClass("locked");
    }
};

hub.client.containerReleased = function (cid) {
    console.log("containerReleased EVENT RECEIVED", cid);
    $("#chk_" + cid)
        .prop("checked", false)
        .prop("disabled", false);
    $("#chk_" + cid).closest("label").find(".equipname")
        .removeClass("locked").removeClass("allotted");
};
