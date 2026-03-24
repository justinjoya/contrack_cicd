
$(document).ready(function () {
    // Initial check
    if ($("#rfq_detail_table").length > 0) {
        $("#rfq_detail_table").find('tr').each(function () {
            var SupplQty = $(this).find('.SupplQty').val();
            var SupplyPrice = $(this).find('.SupplyAmount').val();
            var TAX = $(this).find('.TAX').val();

            if (SupplQty === "") SupplQty = "0";
            if (SupplyPrice === "") SupplyPrice = "0";
            if (TAX === "") TAX = "0";

            if (parseFloat(SupplQty) === 0 || parseFloat(SupplyPrice) === 0) {
                $(this).addClass('highlight-zero');
                $(this).next("tr").addClass('highlight-zero');
            }
        });

        // Update styling on input change
        $("#rfq_detail_table").find('tr').on('input', function () {
            const tr = $(this).closest('tr');
            var SupplQty = $(this).closest("tr").find('.SupplQty').val();
            var SupplyPrice = $(this).closest("tr").find('.SupplyAmount').val();
            var TAX = $(this).closest("tr").find('.TAX').val();
            if (SupplQty === "") SupplQty = "0";
            if (SupplyPrice === "") SupplyPrice = "0";
            if (TAX === "") TAX = "0";

            if (parseFloat(SupplQty) === 0 || parseFloat(SupplyPrice) === 0) {
                tr.addClass('highlight-zero');
                tr.next("tr").addClass('highlight-zero');
            } else {
                tr.removeClass('highlight-zero');
                tr.next("tr").removeClass('highlight-zero');
            }
        });
    }

    if ($('.rfqbuttons').length > 0) {
        $('.checksingle').change(function () {
            var anyChecked = $('.checksingle:checked').length > 0;
            if (anyChecked) {
                $('.rfqbuttons').prop('disabled', false);
            } else {
                $('.rfqbuttons').prop('disabled', true);
            }
        });
    }
});

function RFQExpandCollapse(JobTypeID) {
    if ($(".icon_" + JobTypeID).hasClass("ki-minus")) {
        $(".icon_" + JobTypeID).removeClass("ki-minus").addClass("ki-plus");
        $("#accordion_tr_" + JobTypeID).removeClass("expanded");
    }
    else {
        $(".icon_" + JobTypeID).removeClass("ki-plus").addClass("ki-minus");
        $("#accordion_tr_" + JobTypeID).addClass("expanded");
    }
}

$(document).on("keydown", "#rfq_detail_table tbody input", function (e) {
    const currentCell = $(this);
    const currentRow = currentCell.closest("tr");
    var textboxesclass = 0;
    if ($(currentCell).hasClass("SupplQty")) {
        textboxesclass = "SupplQty"; //Qty
    }
    else if ($(currentCell).hasClass("SupplyAmount")) {
        textboxesclass = "SupplyAmount"; //Amount
    }
    else if ($(currentCell).hasClass("TAX")) {
        textboxesclass = "TAX"; //Amount
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

$("html").on("keyup keypress change", "#rfq_detail_table .focus-input", function () {
    var qty = $(this).closest("tr").find(".SupplQty").val();
    var amount = $(this).closest("tr").find(".SupplyAmount").val();
    var TAX = $(this).closest("tr").find(".TAX").val();

    if (qty == "") qty = "0";
    if (amount == "") amount = "0";
    if (TAX == "") TAX = "0";


    let totalprice = parseInt(qty) * parseFloat(amount);
    totalprice = totalprice + (totalprice * TAX / 100);
    let formatted = totalprice.toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    $(this).closest("tr").find(".SupplyTotal").html(formatted);
    //calculatetotal();
});

function FillRFQQty() {
    $("#rfq_detail_table").find("tr").each(function () {
        var qty = $(this).find(".hdn_PIDetailQty").val();
        $(this).find(".SupplQty").val(qty).change();
    });
}


$('#frmPurchaseIntentRFQUpdate').on('submit', function (e) {
    var saveaction = "";
    const submitter = e.originalEvent?.submitter;

    if (submitter) {
        const $btn = $(submitter);
        saveaction = $btn.val();
    }

    if (saveaction === "Email") {
        return;
    }
    else {
        $('#btnRFQSave').prop('disabled', true);
        e.preventDefault();

        var validated = $("#frmPurchaseIntentRFQUpdate").Validate();
        if (validated) {
            blockui();
            var formdata = $("#frmPurchaseIntentRFQUpdate").serializeArray();
            $.ajax({
                url: '/Purchase/RFQ?saveaction=' + saveaction,
                data: formdata,
                type: 'POST',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $('#btnRFQSave').prop('disabled', false);
                    if (data.ResultId == 1) {
                        if (saveaction != "Download") {
                            SuccessMessage("RFQ Updated Successfully!");
                        }
                        setTimeout(
                            function () {
                                window.location.href = data.ResultMessage;
                            }, 500);

                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Updated RFQ");
                    $('#btnRFQSave').prop('disabled', false);
                }
            });
        }
        else {
            ErrorMessage("Some of required items needs to be filled");
            $('#btnRFQSave').prop('disabled', false);
        }
    }
});



function QuoteCompareFromRFQList(PurchaseIntentID) {
    var rfqList = $("#pi_rfq_list")
        .find("tbody input[type='checkbox']:checked")
        .map(function () {
            return $(this).val(); // or this.value
        })
        .get().join(',');

    CreateQuoteComparison(rfqList, PurchaseIntentID);
}

function ShowTaxInfo(current) {
    if ($(current).is(":checked")) {
        $("#txtTaxPercentage").removeAttr("disabled");
    }
    else {
        $("#txtTaxPercentage").attr("disabled", true);
    }
}

function FillTax(current) {

}

$("html").on("keyup keypress change", "#txtTaxPercentage", function () {
    $(".TAX").val($(this).val()).change();
});


function OpenRFQFileImport() {
    document.getElementById('ExcelFiles_RFQ').click();
}

function ImportRFQExcel() {
    var fileUpload = $("#ExcelFiles_RFQ").get(0);
    var files = fileUpload.files;
    if (files.length > 0) {
        // Create FormData object
        var fileData = new FormData();
        // Looping over all files and add it to FormData object    
        for (var i = 0; i < files.length; i++) {
            fileData.append('ExcelImport.xls', files[i]);
        }
        blockui();
        SaveMiscData();
        $.ajax({
            url: '/Purchase/ImportRFQExcel',
            type: "POST",
            contentType: false, // Not to set any content header    
            processData: false, // Not to process data    
            data: fileData,
            async: true,
            success: function (data) {
                unblockui();
                $("#tbody_RFQ_Detail").html(data);
            },
            error: function (err) {
                unblockui();
                ErrorMessage("Cannot upload files");
            }
        });

    }
}


function SendRFQEmails() {
    blockui("Sending mail...");
    var count = $("#pi_rfq_list").find("tbody input[type='checkbox']:checked").length;
    if (count == 1) {
        var refid = $("#pi_rfq_list").find("tbody input[type='checkbox']:checked").val();
        unblockui();
        window.location = "/Email/Compose?refid=" + refid + "&type=RFQ";
    }
    else {
        var runningcount = 0;
        $("#pi_rfq_list").find("tbody input[type='checkbox']:checked").each(function () {
            var uuid = $(this).val();
            $.ajax({
                url: '/EmailSchedular/RFQEmail?refid=' + uuid,
                type: "GET",
                async: true,
                success: function (data) {
                    runningcount++;
                    if (runningcount >= count) {
                        unblockui();
                        SuccessMessage("Email Sent Successfull!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }

                },
                error: function (err) {
                    runningcount++;
                    if (runningcount >= count) {
                        unblockui();
                        ErrorMessage("There is some problem in sending email");
                    }
                    //ErrorMessage("Cannot upload files");
                }
            });
        });
    }
}