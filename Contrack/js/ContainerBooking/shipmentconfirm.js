$(document).on("keydown", "#shipment_detail_table tbody input", function (e) {
    const currentCell = $(this);
    const currentRow = currentCell.closest("tr");
    var textboxesclass = 0;
    if ($(currentCell).hasClass("stamp")) {
        textboxesclass = "stamp"; //Qty
    }
    else if ($(currentCell).hasClass("SupplyAmount")) {
        textboxesclass = "SupplyAmount"; //Amount
    }
    else if ($(currentCell).hasClass("weight")) {
        textboxesclass = "weight"; //Amount
    }

    if (e.key === "ArrowDown" || e.key === 'Enter') {
        e.preventDefault();
        const nextRow = currentRow.nextAll("tr.mainrow").first();
        if (nextRow.length) {
            nextRow.find("." + textboxesclass).focus();
        }
    } else if (e.key === "ArrowUp") {
        e.preventDefault();
        const prevRow = currentRow.prevAll("tr.mainrow").first();
        if (prevRow.length) {
            prevRow.find("." + textboxesclass).focus();
        }
    }
});


$('#frmShipmentConfirmation').on('submit', function (e) {
    var saveaction = "";
    const submitter = e.originalEvent?.submitter;

    if (submitter) {
        const $btn = $(submitter);
        saveaction = $btn.val();
    }
    $('.actionbutton').prop('disabled', true);
    e.preventDefault();
    blockui();
    var formdata = $("#frmShipmentConfirmation").serialize();
    $.ajax({
        url: "/Booking/ShipmentConfirmation?buttonaction=" + saveaction,
        type: "POST",
        data: formdata,
        dataType: "json",
        success: function (data) {
            unblockui();
            if (data.ResultId == "1") {
                //SuccessMessage("Saved Successfully!");
                LoadingMessage("Downloading. Please wait...");
                setTimeout(
                    function () {
                        window.location = data.ResultMessage;
                    }, 100);
            } else {
                ErrorMessage(data.ResultMessage);
            }
            $('.actionbutton').prop('disabled', false);
        },
        error: function (xhr, status, error) {
            unblockui();
            ErrorMessage("Something went wrong while saving Shipment Confirmation.");
            $('.actionbutton').prop('disabled', false);
        }
    });

});

function EnableShipmentCheckboxAll(current) {
    $(".checkbox").prop("checked", $(current).is(":checked"));
}
function EnableShipmentCheckbox(current, headingvalue) {
    $(".check_" + headingvalue).prop("checked", $(current).is(":checked"));
}