function AddEditBookingContainer(bookingdetailuuid) {
    blockui();
    $.ajax({
        url: '/Booking/GetContainerModal?bookingdetailuuid=' + bookingdetailuuid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            //$("#modalsection").html(data).find(".select2").select2();
            //$("#modalsection").find("#ddlcontainermodel").containermodelSelect2({ minLength: 1 });
            //$("#modalsection").find("#ddlcontainertype").containermodelSelect2({ minLength: 1 });
            //$("#modalsection").find("#ddlcontainersize").containermodelSelect2({ multiline: true, minLength: 1 });
            //$("#modalsection").find("#ddlPackageType").containermodelSelect2({ minLength: 1 });
            $("#modalsection").html(data).find(".select2").select2();
            if ($("#modalsection").find(".datepicker-here").length > 0) {
                $("#modalsection").find(".datepicker-here").each(function () {
                    var datatarget = $(this).data("trigger");
                    var dataclear = $(this).data("clear");
                    var existingValue = $(this).val();

                    var datepicker = $(this).flatpickr({
                        enableTime: false,
                        dateFormat: "Y-m-d",
                        altInput: true,
                        altFormat: "d/m/Y",
                        defaultDate: existingValue || null,
                        allowInput: false
                    });
                    if (datatarget != undefined) {
                        document.getElementById(datatarget).addEventListener("click", function () {
                            datepicker.open(); // Manually open the Flatpickr
                        });
                    }

                    //if (dataclear != undefined) {
                    //    document.getElementById(dataclear).addEventListener("click", function () {
                    //        datepicker.clear();
                    //    });
                    //}
                });
            }
            OpenModal("ModalAddBookingContainer")
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Container")
        }
    });
}

// Increase
$(document).on("click", "#btnClearDeliveryDate", function () {
    var qtyInput = $("#txt_Qty");
    var currentVal = parseInt(qtyInput.val()) || 0;
    qtyInput.val(currentVal + 1);
});

// Decrease
$(document).on("click", "#btnDeliveryDate", function () {
    var qtyInput = $("#txt_Qty");
    var currentVal = parseInt(qtyInput.val()) || 0;

    if (currentVal > 1) {
        qtyInput.val(currentVal - 1);
    }
});

// Commodity Required
$(document).on("change", "#ddlEmptyOrFull", function () {
    toggleCommodityRequired();
});
function toggleCommodityRequired() {
    var emptyFullValue = $("#ddlEmptyOrFull").val();
    if (emptyFullValue === "F") {
        $("#txt_Commodity").attr("required", "required");
        $("#commodityRequiredStar").show();
    }
    else {
        $("#txt_Commodity").removeAttr("required");
        $("#commodityRequiredStar").hide();
    }
}