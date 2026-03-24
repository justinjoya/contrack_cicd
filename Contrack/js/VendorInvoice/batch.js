
$(document).ready(function () {
    $('#formVIBatch').on('submit', function (e) {
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
            $('.vinvoicebuttons').prop('disabled', true);
            e.preventDefault();
            var validated = $("#formVIBatch").Validate();
            if (validated) {
                blockui();
                var form = document.getElementById('formVIBatch');
                var formdata = new FormData(form);
                //var formdata = $("#frmCreateVendorInvoice").serializeArray();
                $.ajax({
                    url: '/Batch/SaveBatch?saveaction=' + saveaction,
                    data: formdata,
                    type: 'POST',
                    processData: false,  // Prevent jQuery from converting the data to a string
                    contentType: false,  // Prevent jQuery from setting the Content-Type header
                    //dataType: "json",
                    //contentType: "application/json",
                    success: function (data) {
                        unblockui();
                        if (data.ResultId == 1) {
                            if (saveaction == "Approve") {
                                OpenConfirmation($("#hdn_BatchUUID").val());
                            }
                            else {
                                SuccessMessage("Batch Saved Successfully!");
                                setTimeout(
                                    function () {
                                        window.location.href = data.ResultMessage;
                                    }, 500);
                            }
                        }
                        else {
                            ErrorMessage(data.ResultMessage);
                        }
                        $('.vinvoicebuttons').prop('disabled', false);
                    },
                    error: function (data) {
                        unblockui();
                        ErrorMessage("Cannot Save Batch");
                        $('.vinvoicebuttons').prop('disabled', false);
                    }
                });
            }
            else {
                ErrorMessage("Some of required items needs to be filled");
                $('.vinvoicebuttons').prop('disabled', false);
            }
        }
    });
});


function OpenConfirmation(batchuuid) {
    blockui();
    $.ajax({
        url: '/Batch/GetApproval?BatchUUID=' + batchuuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalBatchVIApproval = document.querySelector('#ModalBatchVIApproval');
            const modal = KTModal.getInstance(ModalBatchVIApproval);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}


function SendForApproval(batchuuid) {
    if ($("#formBatchVIApproval").Validate()) {
        blockui();
        var data = { "approver": $("#ddlApprovalUser").val(), "message_text": $("#txt_Comments").val(), "target_id": $("#hdn_BatchIDEnc").val() };

        let formdata = JSON.stringify(data);

        $.ajax({
            url: '/Batch/SendForApproval?batchuuid=' + batchuuid,
            data: formdata,
            type: 'POST',
            //dataType: "json",
            contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    SuccessMessage("Sent for approval Successfully!");
                    setTimeout(
                        function () {
                            if (data.ResultMessage == "") {
                                window.location = window.location.href;
                            }
                            else {
                                window.location = data.ResultMessage;
                            }
                        }, 500);
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Send for approval");
            }
        });
    }
}

function ValidateBatchCompany(current) {
    counter1 = 0;
    blockui();
    GenerateVendorByUUID($(current).val(), "vendor_placeholder", 2);
    GenerateBank($(current).val(), "bank_placeholder", 2);
}

function GenerateVendorByUUID(agencyuuid, targetid, countertarget) {
    $.ajax({
        url: '/Master/GetMultiVendorDropdownByUUID?AgencyUUID=' + agencyuuid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
            $("#" + targetid).html(data).find(".select2").select2();
        },
        error: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
        }
    });
}

function GenerateBank(agencyuuid, targetid, countertarget) {
    $.ajax({
        url: '/Master/GetBankDropdownByUUID?AgencyUUID=' + agencyuuid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
            $("#" + targetid).html(data).find(".select2").select2();
            $("#bank_info").hide();
        },
        error: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
        }
    });
}


function OpenCreateBatch(refid) {
    blockui();
    $.ajax({
        url: '/Batch/GetVIBatch?refid=' + refid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            if ($("#modalsection").find(".datepicker-here").length > 0) {
                $("#modalsection").find(".datepicker-here").each(function () {
                    var datatarget = $(this).data("trigger");
                    var dataclear = $(this).data("clear");

                    var datepicker = $(this).flatpickr({
                        dateFormat: "Y-m-d",
                        altInput: true,
                        altFormat: "M j, Y"
                    });
                    if (datatarget != undefined) {
                        document.getElementById(datatarget).addEventListener("click", function () {
                            datepicker.open(); // Manually open the Flatpickr
                        });
                    }

                    if (dataclear != undefined) {
                        document.getElementById(dataclear).addEventListener("click", function () {
                            datepicker.clear();
                        });
                    }
                });

            }
            const ModalCreateBatch = document.querySelector('#ModalCreateBatch');
            const modal = KTModal.getInstance(ModalCreateBatch);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Batch")
        }
    });
}

function GetAgencyBankInfo(current) {
    blockui();
    var agencyuuid = $("#ddlAgencies").val();
    var bankuuid = $(current).val();
    $.ajax({
        url: '/Master/GetAgencyBankAccount?AgencyUUID=' + agencyuuid + '&BankUUID=' + bankuuid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#bank_info").html(data);
            if ($("#bank_info").find(".bankinfo").length == 0) {
                $("#bank_info").hide();
            }
            else {
                $("#bank_info").show();
            }
        },
        error: function (data) {
            unblockui();
        }
    });
}


function SearchBatchVendorInvoice() {
    if ($("#formVIBatch").Validate()) {
        blockui();
        var formdata = $("#formVIBatch").serializeArray();
        $.ajax({
            url: '/Batch/GetVIForBatch',
            data: formdata,
            type: 'POST',
            //dataType: "json",
            /*contentType: "application/json",*/
            success: function (data) {
                unblockui();
                if ($("#ModalCreateBatch").length > 0) {
                    CloseModal("ModalCreateBatch");
                }
                $("#modalsection").html(data);
                OpenModal("ModalBatchSearchInvoice");

            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Create Batch");
            }
        });
    }
    else {
        ErrorMessage("Some of required items needs to be filled");
    }
}


function ChooseVendorInvoice() {
    blockui();
    var formdata = $("#frmCreateBatchVendorInvoice").serializeArray();
    $.ajax({
        url: '/Batch/GetVIForBatchModal',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalBatchSearchInvoice = document.querySelector('#ModalBatchSearchInvoice');
            const modal = KTModal.getInstance(ModalBatchSearchInvoice);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Create Batch");
        }
    });
}



function EnableVICheckbox(current, vendoruuid, currency) {
    if ($(current).is(":checked")) {
        /*$(".check-group").prop("checked", true);*/
        if (currency == undefined || currency == "") {
            $(".check-group[datavendor='" + vendoruuid + "']").prop("checked", true);
        }
        else {
            $(".check-group[datavendor='" + vendoruuid + "'][datacurrency='" + currency + "']").prop("checked", true);
        }

    }
    else {
        if (currency == undefined || currency == "") {
            $(".check-group[datavendor='" + vendoruuid + "']").prop("checked", false);
        }
        else {
            $(".check-group[datavendor='" + vendoruuid + "'][datacurrency='" + currency + "']").prop("checked", false);
        }
    }
}
function EnableVICheckboxAll(current) {
    if ($(current).is(":checked")) {
        /*$(".check-group").prop("checked", true);*/
        $(".check-group").prop("checked", true);

    }
    else {
        $(".check-group").prop("checked", false);
    }
}


function CreateBatch() {
    blockui();
    var formdata = $("#formBatchSearchInvoice").serializeArray();
    $.ajax({
        url: '/Batch/SaveBatch',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {

                SuccessMessage("Batch Saved Successfully!");
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
            ErrorMessage("Cannot Create Batch");
        }
    });
}

function AddToBatch() {
    blockui();
    var formdata = $("#formBatchSearchInvoice").serializeArray();
    $.ajax({
        url: '/Batch/AddToBatch',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            SuccessMessage("Updated Successfully!");
            setTimeout(
                function () {
                    window.location = window.location.href;
                }, 500);

        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Create Batch");
        }
    });
}

function UpdateConversions() {
    blockui();
    var formdata = $("#formBatchConversions").serializeArray();
    $.ajax({
        url: '/Batch/UpdateConversions',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            SuccessMessage("Updated Successfully!");
            setTimeout(
                function () {
                    window.location = window.location.href;
                }, 500);
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Update Conversions");
        }
    });
}

function FillBalance() {
    $("#batch_vendor_invoice_detail_table").find("tr").each(function () {
        var balance = parseFloat($(this).find(".balanceamount").val()).toFixed(2);
        $(this).find(".amounttopay").val(balance);
    });
    CalculateTotal();
}

function DeleteBatchVendorInvoiceItem(vendorinvoiceid) {
    Swal.fire({
        title: "Are you sure?",
        text: "You want to delete the this invoice?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, do it!"
    }).then((result) => {
        if (result.isConfirmed) {
            blockui();
            $.ajax({
                url: '/Batch/DeleteBatchVendorInvoiceItem?vendorinvoiceid=' + vendorinvoiceid,
                type: 'GET',
                /*contentType: "application/json",*/
                success: function (data) {
                    unblockui();
                    $("#section_vendorinvoice_list").html(data);
                    CalculateTotal();
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Error in Deleting Vendor Invoice")
                }
            });
        }

    });
}

$(document).on("keyup keypress change", "#batch_vendor_invoice_detail_table tbody input", function (e) {
    CalculateTotal();
});
function CalculateTotal() {

    let amountopay = 0;
    $("#batch_vendor_invoice_detail_table tbody").find('tr').each(function () {
        var amounttopay = $(this).find(".amounttopay").val();
        if (amounttopay == "" || amounttopay == undefined) amounttopay = "0";
        amountopay += parseFloat(amounttopay);
    });

    let formattedamountopay = amountopay.toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    $("#labelAmounttopay").html(formattedamountopay);

}


function OpenApproveReject(batchuuid, isapprove) {
    blockui();
    $.ajax({
        url: '/Batch/GetApproveReject?batchuuid=' + batchuuid + '&accept=' + (isapprove ? 1 : 2),
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalBatchApprovaReject = document.querySelector('#ModalBatchApprovaReject');
            const modal = KTModal.getInstance(ModalBatchApprovaReject);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}

function ApproveBatch(batchuuid) {
    var data = {
        "batchuuid": batchuuid,
        "isaccept": true,
        "comments": $("#ModalBatchApprovaReject").find("#txt_Comments").val(),
        "batchid": $("#hdn_BatchIDEnc").val()
    };
    ApproveRejectBatch(data);
}

function RejectBatch(batchuuid) {
    if ($("#formBatchApprovalReject").Validate()) {
        var data = {
            "batchuuid": batchuuid,
            "isaccept": false,
            "comments": $("#ModalBatchApprovaReject").find("#txt_Comments").val(),
            "batchid": $("#hdn_BatchIDEnc").val()
        };
        ApproveRejectBatch(data);
    }
}

function ApproveRejectBatch(data) {
    blockui();
    let formdata = JSON.stringify(data);
    $.ajax({
        url: '/Batch/ApproveReject',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                SuccessMessage("Updated Successfully!");
                setTimeout(
                    function () {
                        if (data.ResultMessage == "") {
                            window.location = window.location.href;
                        }
                        else {
                            window.location = data.ResultMessage;
                        }
                    }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Update Batch");
        }
    });
}



$(document).on("keyup keypress change", "#formBatchConversions .convrate", function (e) {
    var convvalue = $(this).val();
    $(this).closest("tr").find(".converted").html(convvalue);
});



function MarkasPaid(batchuuid) {

    Swal.fire({
        title: "Are you sure?",
        text: "You want to mark this batch as paid?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, Mark as paid!"
    }).then((result) => {
        if (result.isConfirmed) {
            blockui();
            $.ajax({
                url: '/Batch/MarkAsPaid?batchuuid=' + batchuuid,
                type: 'GET',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("Updated Successfully!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot update");
                }
            });
        }

    });
}

function ExpandCollapseVendor(current, vendoruuid) {
    if ($(current).find(".iconexpand").hasClass("ki-minus")) {
        $(current).find(".iconexpand").removeClass("ki-minus").addClass("ki-plus");
        $(".vendor_" + vendoruuid).removeClass("expanded");

    }
    else {
        $(current).find(".iconexpand").removeClass("ki-plus").addClass("ki-minus");
        $(".vendor_" + vendoruuid).addClass("expanded");
    }
}

function ExpandCollapseVendorAll(current) {
    if ($(current).find(".iconexpand").hasClass("ki-minus")) {
        $(current).find(".iconexpand").removeClass("ki-minus").addClass("ki-plus");
        $(".vendorgroup").find(".iconexpand").removeClass("ki-minus").addClass("ki-plus");
        $(".vendorsection").removeClass("expanded");

    }
    else {
        $(current).find(".iconexpand").removeClass("ki-plus").addClass("ki-minus");
        $(".vendorgroup").find(".iconexpand").removeClass("ki-plus").addClass("ki-minus");
        $(".vendorsection").addClass("expanded");
    }
}

