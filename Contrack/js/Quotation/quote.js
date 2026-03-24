$("html").on("keyup keypress change", "#txtSellPercentage", function () {
    var markupvalue = $(this).val();
    if (markupvalue == "" || markupvalue == undefined)
        markupvalue = "0";
    var markup = parseFloat(markupvalue);

    $(".quotelineitem").each(function () {
        var Qty = $(this).find(".Qty").val();
        var buyamount = parseFloat($(this).find(".buyamount").val());
        var sellamount = buyamount + (buyamount * markup / 100);
        if (isNaN(sellamount))
            sellamount = 0;
        $(this).find(".sellamount").val(parseFloat(sellamount).toFixed(2));

        $(this).find(".selllabel").html(displayprice(parseFloat(sellamount) * parseFloat(Qty)));

    });
    calculatequotetotal();
});

function calculatequotetotal() {
    $(".sellamount").each(function () {
        adjustarrows(this);
    });
}

$("html").on("keyup keypress change", ".sellamount", function () {
    adjustarrows(this);
});


function adjustarrows(current) {
    var buyamount = parseFloat($(current).closest("tr").find(".buyamount").val());
    var sellamount = parseFloat($(current).val());
    var closesttd = $(current).closest("td");
    if (buyamount > sellamount) {
        closesttd.find(".btn").removeClass("btn-success");
        closesttd.find(".btn").addClass("btn-danger");
        closesttd.find(".btn>i").removeClass("ki-arrow-up-refraction")
            .removeClass("ki-right-left");
        closesttd.find(".btn>i").addClass("ki-arrow-down-refraction");
    }
    else if (buyamount < sellamount) {
        closesttd.find(".btn").removeClass("btn-danger");
        closesttd.find(".btn").addClass("btn-success");
        closesttd.find(".btn>i").removeClass("ki-arrow-down-refraction")
            .removeClass("ki-right-left");
        closesttd.find(".btn>i").addClass("ki-arrow-up-refraction");
    }
    else {
        closesttd.find(".btn").removeClass("btn-success").removeClass("btn-danger");
        closesttd.find(".btn").addClass("btn-light");
        closesttd.find(".btn>i").removeClass("ki-arrow-up-refraction")
            .removeClass("ki-arrow-down");
        closesttd.find(".btn>i").addClass("ki-right-left");

    }
}


$(function () {
    $('#frmCreateQuote').on('submit', function (e) {

        var saveaction = "";
        const submitter = e.originalEvent?.submitter;

        if (submitter) {
            const $btn = $(submitter);
            saveaction = $btn.val();
        }

        if (saveaction === "Download") {
            return;
        }
        else {
            $('#btnSaveQuote').prop('disabled', true);
            $('#btnCreatePO').prop('disabled', true);
            e.preventDefault();

            var validated = $("#frmCreateQuote").Validate();
            if (validated) {
                blockui();
                var formdata = $("#frmCreateQuote").serializeArray();
                $.ajax({
                    url: '/Quote/Create?saveaction=' + saveaction,
                    data: formdata,
                    type: 'POST',
                    //dataType: "json",
                    //contentType: "application/json",
                    success: function (data) {
                        unblockui();
                        if (data.ResultId == 1) {
                            SuccessMessage("Quotation Saved Successfully!");
                            setTimeout(
                                function () {
                                    window.location.href = data.ResultMessage;
                                }, 500);
                        }
                        else {
                            ErrorMessage(data.ResultMessage);
                        }
                        $('#btnSaveQuote').prop('disabled', false);
                        $('#btnCreatePO').prop('disabled', false);
                    },
                    error: function (data) {
                        unblockui();
                        ErrorMessage("Cannot Save Quote");
                        $('#btnSaveQuote').prop('disabled', false);
                        $('#btnCreatePO').prop('disabled', false);
                    }
                });
            }
            else {
                ErrorMessage("Some of required items needs to be filled");
                $('#btnSaveQuote').prop('disabled', false);
                $('#btnCreatePO').prop('disabled', false);
            }
        }
    });
});


$(document).on('keydown', function (e) {
    if ($("#frmCreateQuote").length > 0) {
        if (e.ctrlKey && (e.key === 's' || e.key === 'S' || e.key === 'Enter')) {
            if ($("#ModalPurchaseIntentLineItem:visible").length > 0) {
                var hdn_Save = $("#ModalPurchaseIntentLineItem").find("#hdn_Save").val();
                e.preventDefault(); // Prevent browser save
                if (hdn_Save == "0")
                    SaveLineItemAndNext();
                else
                    SaveLineItemAndClose();
            }
            else if ($("#frmCreateQuote:visible").length > 0) {
                e.preventDefault(); // Prevent browser save
                $('#btnSaveQuote').click();
            }
        }
        else if (e.ctrlKey && (e.key === 'p' || e.key === 'P')) {
            e.preventDefault(); // Prevent browser save
            $('#btnCreatePO').click();

        }
    }
});