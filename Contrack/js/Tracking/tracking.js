function ShowHideDamage(current) {
    if ($(current).is(":checked")) {
        $(current).closest(".damage-container").addClass("checked");
    }
    else {
        $(current).closest(".damage-container").removeClass("checked");
    }
}

$("html").on("change", 'input[name="Side"]', function () {
    $(".damage_side_container").hide();
    $("#damage_side_container_" + $(this).val()).show();
});

function OpenBulkRecordMoveModal() {
    blockui();
    let selections = [];
    $(".cb-record-move:checked").each(function () {
        let containerId = $(this).data("containerid") || "";
        let bookingId = $(this).data("bookingid") || "";

        selections.push({
            ContainerId: {
                EncryptedValue: containerId
            },
            BookingId: {
                EncryptedValue: bookingId
            }
        });
    });

    if (selections.length === 0) {
        unblockui();
        ErrorMessage("Cannot Save Record Move!");
    }

    OpenRecordMoveModal(selections);
}

function TriggerSingleRecordMove(element) {

    blockui();

    let containerId = $(element).data("containerid") || "";
    let bookingId = $(element).data("bookingid") || "";

    let selections = [{
        ContainerId: { EncryptedValue: containerId },
        BookingId: { EncryptedValue: bookingId }
    }];

    OpenRecordMoveModal(selections);
}

function OpenRecordMoveModal(selections) {

    blockui();
    $.ajax({
        url: '/Tracking/GetRecordMoveModal',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ ContainerBookingSelection: selections }),
        success: function (res) {
            unblockui();
            if (typeof res === "object") {
                ErrorMessage(res.Message || "Unable to load record move");
                return;
            }

            $("#modalsection").html(res).find(".select2").select2();
            $("#modalsection").find("#ddlVoyage").voyageSelect2({ multiline: true, minLength: 1, createnew: false });
            $("#modalsection").find("#ddlNextVoyage").voyageSelect2({ multiline: true, minLength: 1, createnew: false });
            initFlatpickr("#modalsection");

            OpenModal("ModalRecordMove");
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong. Please try again.");
        }
    });
}

function SaveRecordMove() {
    var validated = $("#formrecordmove").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formrecordmove").serializeArray();
        $.ajax({
            url: '/Tracking/SaveRecordMove',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Record Move Saved Successfully!");
                    setTimeout(
                        function () {
                            window.location.href = window.location;
                        }, 500);
                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Tracking!");
            }
        });
    }
}

$(document).ready(function () {
    initFlatpickr();
});

$("html").on("change", ".movestype", function () {

    var showVoyage = $(this).attr("data-showvoyage") === "true";
    var isOut = $(this).attr("data-inout") === "OUT";

    var $voyageSection = $("#voyageSection");
    var $locationSection = $("#locationSection");
    var $fields = $("#ddlNextMoves, #ddlNextLocation, #ddlNextDateTime, #ddlNextVoyage");

    if (showVoyage) {
        $voyageSection.show();
        $locationSection.hide();
    } else {
        $voyageSection.hide();
        $locationSection.show();
    }

    $fields.attr("datarequired", isOut ? "yes" : "no");
    $fields.attr("datavalidate", isOut ? "yes" : "no");
    toggleStar(".nextmove", isOut);

});

function toggleStar(labelSelector, isRequired) {

    var $label = $(labelSelector);

    if (isRequired) {
        $label.find(".nextreq").html('<span class="text-danger ps-2 required-star">*</span>');

    } else {
        $label.find(".nextreq").html('<span class="ps-2 text-gray-500">(Optional)</span>');

    }
}

function initFlatpickr(container) {
    container = container || document;

    $(container).find(".datetimepicker-here").each(function () {

        if (this._flatpickr) return;

        var datatarget = $(this).data("trigger");
        var dataclear = $(this).data("clear");
        var mindatefield = $(this).data("min-date-field");

        var datepicker = $(this).flatpickr({
            enableTime: true,
            dateFormat: "Y-m-d H:i",
            altInput: true,
            altFormat: "M j, Y h:i K",
            /*allowInput: true,*/
            onChange: function (selectedDates) {
                if ($("#" + mindatefield).length > 0) {
                    if (selectedDates.length > 0) {
                        let min = new Date(selectedDates[0]);
                        min.setDate(min.getDate() + 1);   // force next day
                        $("#" + mindatefield)[0]._flatpickr.set("minDate", min);
                    }
                }
            }
        });

        if (datatarget) {
            document.getElementById(datatarget)?.addEventListener("click", function () {
                datepicker.open();
            });
        }

        if (dataclear) {
            document.getElementById(dataclear)?.addEventListener("click", function () {
                datepicker.clear();
            });
        }
    });
}

$("html").on("change", ".nextmovestype", function () {

    var showVoyage = $(this).find(":selected").attr("data-shownextvoyage") === "true";

    var $nextVoyageSection = $("#nextVoyageSection");
    var $nextLocationSection = $("#nextLocationSection");

    if (showVoyage) {
        $nextVoyageSection.show();
        $nextLocationSection.hide();
    } else {
        $nextVoyageSection.hide();
        $nextLocationSection.show();
    }
});

function OpenRecordMoveModalByUuid(trackingUuid) {
    blockui();
    $.ajax({
        url: '/Tracking/GetRecordMoveModalByUuid?trackingUuid=' + trackingUuid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalRecordMove");
            initFlatpickr("#modalsection");
            $("#modalsection").find("#ddlVoyage").voyageSelect2({ multiline: true, minLength: 1, createnew: false });
            $("#modalsection").find("#ddlNextVoyage").voyageSelect2({ multiline: true, minLength: 1, createnew: false });
            $("#modalsection").find("#Damage").each(function () {
                ShowHideDamage(this);
            });
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in Getting Record Move Modal");
        }
    });
}


