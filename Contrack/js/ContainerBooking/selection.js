var lastselectedate;
var containerselected = [];
var current_bookinguuid = $("#hdn_bookinguuid").val();
$("html").on("change", '.selectioncheckbox', function () {
    handleSelectionChange(this);
});
$("html").on("keyup keypress change", '.searchcontainer', function () {
    var value = $(this).val().toLowerCase();

    $(this).closest(".selection-model-group").find(".grid .custom-selection-check").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });
});


setInterval(function () {
    var now = new Date();
    if (now - lastselectedate >= 3 * 1000) {
        console.log(now);
        var notProcessing = containerselected.filter(c => c.processing === 0);
        if (notProcessing.length > 0) {
            AcquireProcessing();
        }
    }
    else {
        console.log("Not expired");
    }
}, 1000)
function handleSelectionChange(elem) {
    lastselectedate = new Date();
    var $checkbox = $(elem);
    var model = $checkbox.data("model");
    var $group = $checkbox.closest(".selection-model-group");

    updateGroupState($group);
    updateOverallProgress();

    processgroup(model);

    var info = getContainerInfo($checkbox);
    AcquireContainer(info.containerid, info.modeluuid, info.locationuuid, $checkbox.is(":checked"));
}

function updateGroupState($group) {
    var checkedlength = $group.find(".selectioncheckbox:checked").length;
    var RequiredCount = $group.find(".RequiredCount").val();
    if (checkedlength >= RequiredCount) {
        $group.find(".selectioncheckbox").not(":checked").prop("disabled", true);
    }
    else {
        $group.find(".selectioncheckbox:not(:checked)").filter(function () {
            return !$(this).parent().find(".locked").length;
        }).prop("disabled", false);
    }
    $group.find(".selected").html(checkedlength);
}

function updateOverallProgress() {
    var allcheckedlength = $(document).find(".selectioncheckbox:checked").length;
    var requiredall = parseInt($("#requiredall").html());
    $("#selectedall").html(allcheckedlength);
    var percentage = allcheckedlength == 0 ? 0 : ((allcheckedlength / requiredall) * 100);
    var $progress = $("#progressall").closest(".progress");
    $progress.removeClass("progress-success");
    $progress.removeClass("progress-warning");
    $progress.removeClass("progress-danger");
    if (percentage >= 75) {
        $progress.addClass("progress-success");
    }
    else if (percentage >= 50) {
        $progress.addClass("progress-warning");
    }
    else {
        $progress.addClass("progress-danger");
    }
    $("#progressall").css("width", percentage + "%");
}

function getContainerInfo($checkbox) {
    return {
        containerid: $checkbox.closest("label").find(".ContainerID").val(),
        modeluuid: $checkbox.closest(".contmodel").find(".ContainerModelUuid").val(),
        locationuuid: $checkbox.closest(".location").find(".LocationUuid").val()
    };
}

function processgroup(groupid) {
    var $group = $(".model-group-" + groupid);
    var allcheckedlength = $group.find(".selectioncheckbox:checked").length;
    var requiredall = parseInt($("#required_" + groupid).html());
    $("#selected_" + groupid).html(allcheckedlength);
    var percentage = allcheckedlength == 0 ? 0 : ((allcheckedlength / requiredall) * 100);
    var $progress = $("#progress_" + groupid).closest(".progress");
    $progress.removeClass("progress-success");
    $progress.removeClass("progress-warning");
    $progress.removeClass("progress-danger");
    if (percentage >= 75) {
        $progress.addClass("progress-success");
    }
    else if (percentage >= 50) {
        $progress.addClass("progress-warning");
    }
    else {
        $progress.addClass("progress-danger");
    }
    $("#progress_" + groupid).css("width", percentage + "%");
}

function ExpandCollapseHeader(current) {
    $group = $(current).closest(".selection-model-group");
    if ($(current).find("i").hasClass("ki-minus")) {
        $group.find(".grid").hide();
        $(current).find("i").removeClass("ki-minus");
        $(current).find("i").addClass("ki-plus");
    }
    else {
        $group.find(".grid").show();
        $(current).find("i").removeClass("ki-plus");
        $(current).find("i").addClass("ki-minus");
    }
}

function ExpandCollapseAll(current) {
    $(current).closest(".allocation-card").find(".expandcollapse").click();
}


function AcquireContainer(containerid, modeluuid, locationuuid, ischecked) {
    var existing = containerselected.find(x => x.containerid === containerid);
    if (existing) {
        existing.isdeleted = !ischecked;
        existing.processing = 0;
    } else {
        containerselected.push({ containerid: containerid, modeluuid: modeluuid, locationuuid: locationuuid, isdeleted: !ischecked, processing: 0 });
    }
    var notProcessing = containerselected.filter(c => c.processing === 0);
    if (notProcessing.length >= 5) {
        AcquireProcessing();
    }
}

function AcquireProcessing(calledfromsave) {
    if (!calledfromsave)
        calledfromsave = false;
    var grouped = GroupByLocationModel(containerselected);
    Object.keys(grouped).forEach(function (key) {

        var group = grouped[key];
        // mark as processing to avoid double submit
        containerselected.forEach(function (c) {
            if (c.locationuuid === group.locationuuid &&
                c.modeluuid === group.modeluuid &&
                c.processing === 0) {
                c.processing = 1;
            }
        });

        $.ajax({
            url: '/Reserve/AcquireContainer',
            type: 'POST',
            data: {
                bookinguuid: current_bookinguuid,
                locationuuid: group.locationuuid,
                modeluuid: group.modeluuid,
                containerids: group.containerids
            },
            async: !calledfromsave,
            //traditional: true, // 🔥 REQUIRED for List<string>
            success: function (data) {
                console.log("Server response:", data);
                data.forEach(function (item) {
                    processResponse(item);
                });
                group.containerids.forEach(function (cid) {
                    var c = containerselected.find(x => x.containerid === cid.containerid && x.isdeleted === false && x.processing === 1);
                    if (c) {

                        $("#chk_" + cid.containerid)
                            .prop("checked", false)
                            .prop("disabled", true);
                        $("#chk_" + cid.containerid).closest("label").find(".equipname")
                            .addClass("locked");
                    }
                });
            },
            error: function () {
            }
        });

    });
}
function processResponse(item) {
    var containerId = item.ContainerID.EncryptedValue;
    var bookinguuid = item.AllocationBookingUUID;
    if (bookinguuid == current_bookinguuid) {
        // update local array
        var c = containerselected.find(x => x.containerid === containerId);
        if (c) {
            c.processing = 2; // confirmed
        }
        // update UI
        $("#chk_" + containerId).prop("checked", true)
            .prop("disabled", false);

        $("#chk_" + containerId).closest("label").find(".equipname")
            .addClass("allotted");
    }
}
function GroupByLocationModel(list) {
    return list.reduce(function (groups, item) {

        if (item.processing !== 0) return groups;

        var key = item.locationuuid + "|" + item.modeluuid;

        if (!groups[key]) {
            groups[key] = {
                locationuuid: item.locationuuid,
                modeluuid: item.modeluuid,
                containerids: []
            };
        }

        groups[key].containerids.push({ containerid: item.containerid, isdeleted: item.isdeleted });
        return groups;

    }, {});
}

$('#frmContainerSelection').on('submit', function (e) {
    var saveaction = "";
    const submitter = e.originalEvent?.submitter;

    if (submitter) {
        const $btn = $(submitter);
        saveaction = $btn.val();
    }
    if (saveaction == "Confirm") {
        e.preventDefault();
        ShowConfirmation({
            title: 'Confirm container selection?',
            message: `Are you sure you want to assign these containers to this booking?`,
            confirmtext: `Yes, Confirm`,
            canceltext: `Cancel`,
            onConfirm: () => {
                processContainerSelection(saveaction, e);
            },
            onCancel: () => {
                //$(current).prop('checked', !$(current).prop('checked'));
            }
        });
    }
    else {
        processContainerSelection(saveaction, e);
    }
});


function processContainerSelection(saveaction, e) {
    if (saveaction === "Download") {
        return;
    }
    else {
        $('.actionbutton').prop('disabled', true);
        e.preventDefault();
        var validated = true;
        if (validated) {
            blockui();
            AcquireProcessing(true); //Process unprocessed
            var formdata = $("#frmContainerSelection").serialize();
            $.ajax({
                url: "/Reserve/ContainerSelection?saveaction=" + saveaction,
                type: "POST",
                data: formdata,
                dataType: "json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Saved Successfully!");
                        setTimeout(
                            function () {
                                window.location = data.ResultMessage;
                            }, 500);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                    $('.actionbutton').prop('disabled', false);
                },
                error: function (xhr, status, error) {
                    unblockui();
                    ErrorMessage("Something went wrong while saving container selection.");
                    $('.actionbutton').prop('disabled', false);
                }
            });
        }
        else {
            ErrorMessage("Some of required items needs to be filled");
            $('.actionbutton').prop('disabled', false);
        }
    }
}


function DeleteContainerFromBooking(bookinguuid, containeruuid) {
    var formdata = {
        bookinguuid: bookinguuid,
        containeruuid: containeruuid
    };
    ShowErrorConfirmation({
        title: 'Delete Container?',
        message: `Are you sure you want to delete the container from this booking?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            $.ajax({
                url: "/Reserve/DeleteContainer",
                type: "POST",
                data: formdata,
                dataType: "json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Deleted Successfully!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (xhr, status, error) {
                    unblockui();
                    ErrorMessage("Something went wrong while deleting container.");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}
