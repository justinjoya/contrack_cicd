
$(document).ready(function () {
    GeneratePO($("#ddlVendor").val(), "ddlPONumber");
    $('#frmCreateVendorInvoice').on('submit', function (e) {
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
            var validated = $("#frmCreateVendorInvoice").Validate();
            if (validated && saveaction == "Approve") {
                var trlengh = $("#vendor_invoice_detail_table").find("tbody>tr").length;
                if (trlengh == 0) {
                    ErrorMessage("Please add at least one line item to proceed with invoice approval");
                    $('.vinvoicebuttons').prop('disabled', false);
                    return;
                }
            }
            if (validated) {
                blockui();
                var form = document.getElementById('frmCreateVendorInvoice');
                var formdata = new FormData(form);
                //var formdata = $("#frmCreateVendorInvoice").serializeArray();
                $.ajax({
                    url: '/VendorInvoice/Create?saveaction=' + saveaction,
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
                                var uuid = $("#hdn_InvoiceUUID").val();
                                if (uuid == "") {
                                    uuid = data.TargetUUID;
                                    $("#hdn_InvoiceUUID").val(data.TargetUUID);
                                    $("#hdn_InvoiceIDInc").val(data.TargetID);
                                }
                                OpenConfirmation(uuid, $("#ddlAgencies").val());
                            }
                            else {
                                SuccessMessage("Invoice Saved Successfully!");
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
                        ErrorMessage("Cannot Save Invoice");
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

    $(document).on("keydown", "#vendor_invoice_detail_table tbody input", function (e) {
        const currentCell = $(this);
        const currentRow = currentCell.closest("tr");
        var textboxesclass = 0;
        if ($(currentCell).hasClass("sellqty")) {
            textboxesclass = "sellqty"; //Qty
        }
        else if ($(currentCell).hasClass("sellamount")) {
            textboxesclass = "sellamount"; //Amount
        }
        //else if ($(currentCell).hasClass("TAX")) {
        //    textboxesclass = "TAX"; //Amount
        //}

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

    $("html").on("keyup keypress change", ".sellamount,.sellqty", function () {
        var qty = $(this).closest("tr").find(".sellqty").val();
        var amount = $(this).closest("tr").find(".sellamount").val();

        if (qty == "") qty = "0";
        if (amount == "") amount = "0";

        let totalprice = parseInt(qty) * parseFloat(amount);
        let formatted = totalprice.toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });

        $(this).closest("tr").find(".selllabel").html(formatted);
        CalculateTotal();
    });


    $("html").on("keyup keypress change", "#txt_InvoiceAmount", function () {
        CalculateTotal();
    });

    $("html").on("change", "#ddlVendor", function () {
        GeneratePODropdown($(this));
    });

});

function CalculateTotal(current) {
    let invoiceprice = 0;
    let poprice = 0;
    let invoiceheaderprice = 0;
    $("#vendor_invoice_detail_table tbody").find('tr').each(function () {
        var qty = $(this).closest("tr").find(".sellqty").val();
        var amount = $(this).closest("tr").find(".sellamount").val();
        var buyqty = $(this).closest("tr").find(".buyqty").val();
        var buyamount = $(this).closest("tr").find(".buyamount").val();

        if (qty == "") qty = "0";
        if (amount == "") amount = "0";

        if (buyqty == "") buyqty = "0";
        if (buyamount == "") buyamount = "0";

        invoiceprice += parseInt(qty) * parseFloat(amount);
        poprice += parseInt(buyqty) * parseFloat(buyamount);
    });
    //difference = invoiceprice - poprice;
    invoiceheaderprice = $("#txt_InvoiceAmount").val();
    if (Number.isNaN(invoiceheaderprice)) {
        invoiceheaderprice = 0;
    }
    else {
        invoiceheaderprice = parseFloat(invoiceheaderprice);
    }

    let formattedinvoice = invoiceprice.toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
    let formattedpo = poprice.toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
    let formattedheaderprice = invoiceheaderprice.toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
    $("#labelInvoiceLineitemAmount").html(formattedinvoice);
    $("#labelPOAmount").html(formattedpo);
    $("#labelInvoiceAmount").html(formattedheaderprice);

    if (poprice >= invoiceprice) {
        $("#labelPOAmount").removeClass("text-danger").addClass("text-success");
    }
    else {
        $("#labelPOAmount").removeClass("text-success").addClass("text-danger");
    }

    if (invoiceprice == invoiceheaderprice) {
        $("#labelInvoiceLineitemAmount").removeClass("text-danger").addClass("text-success");
    }
    else {
        $("#labelInvoiceLineitemAmount").removeClass("text-success").addClass("text-danger");
    }

}

function GeneratePODropdown(current) {
    GeneratePO($(current).val(), "ddlPONumber");
}
function GeneratePO(vendordetailid, targetid) {
    var current = $("#" + targetid);
    var VendorDetailID = vendordetailid;
    POSearch(current, VendorDetailID);
}

function POSearch(current, VendorDetailID) {
    var multiple = $(current).attr("multiple") == "multiple" ? "1" : "";
    var currentpo = current.attr("datavalue");
    var PurchaseIntentUUIDID = $("#hdn_VI_PurchaseIntentUUIDID").val();
    if (PurchaseIntentUUIDID == undefined)
        PurchaseIntentUUIDID = "";
    current.select2({
        placeholder: 'Enter PO No to search',
        minimumInputLength: 0,
        ajax: {
            url: '/PO/GetPODropdownList?VendorDetailID=' + VendorDetailID + '&PO=' + currentpo + '&piuuid=' + PurchaseIntentUUIDID,
            dataType: 'json',
            delay: 250,
            processResults: function (data) {
                return {
                    results: data,
                    pagination: false
                };
            },
            cache: true
        }
    });

    current.each(function () {
        if ($(this).attr("datatext") != "") {
            var newOption = new Option($(this).attr("datatext"), $(this).attr("datavalue"), true, true);
            $(this).append(newOption); //.trigger('change')
        }
    });


}

function GetPOInfo(current) {
    blockui();
    $.ajax({
        url: '/PO/GetPOInfo?refid=' + $(current).val(),
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            $("#ddlCurrency").val(data.vendorcurrency).select2();
            $("#txt_InvoiceAmount").val(data.totalamount);
            if (data.pouuid != "") {
                $("#div_VI_Vessel").hide();
                $.ajax({
                    url: '/VendorInvoice/GetPIInfoLite?refid=' + $(current).val(),
                    type: 'GET',
                    //contentType: "application/json",
                    success: function (data) {
                        $("#div_vendorinvoice_purchase_intent").html(data);
                        LoadVIDetail();
                    },
                    error: function (data) {
                        unblockui();
                        //ErrorMessage("Error in Getting Contact")
                    }
                });
            }
            else {
                unblockui();
                $("#div_VI_Vessel").show();
            }
        },
        error: function (data) {
            unblockui();
            //ErrorMessage("Error in Getting Contact")
        }
    });
}

function LoadVIDetail() {
    $.ajax({
        url: '/VendorInvoice/GetVendorDetail',
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            $("#section_vendorinvoice_details").html(data);
            CalculateTotal();
            unblockui();
        },
        error: function (data) {
            unblockui();
        }
    });
}


$("html").on("keyup keypress change", ".sellamount", function () {
    adjustarrows(this);
});


function adjustarrows(current) {
    var buyamount = parseFloat($(current).closest("tr").find(".buyamount").val());
    var sellamount = parseFloat($(current).val());
    var closesttd = $(current).closest("td");
    if (buyamount < sellamount) {
        closesttd.find(".btn").removeClass("btn-success");
        closesttd.find(".btn").addClass("btn-danger");
        closesttd.find(".btn>i").removeClass("ki-arrow-down-refraction")
            .removeClass("ki-right-left");
        closesttd.find(".btn>i").addClass("ki-arrow-up-refraction");
    }
    else if (buyamount > sellamount) {
        closesttd.find(".btn").removeClass("btn-danger");
        closesttd.find(".btn").addClass("btn-success");
        closesttd.find(".btn>i").removeClass("ki-arrow-up-refraction")
            .removeClass("ki-right-left");
        closesttd.find(".btn>i").addClass("ki-arrow-down-refraction");
    }
    else {
        closesttd.find(".btn").removeClass("btn-success").removeClass("btn-danger");
        closesttd.find(".btn").addClass("btn-light");
        closesttd.find(".btn>i").removeClass("ki-arrow-up-refraction")
            .removeClass("ki-arrow-down");
        closesttd.find(".btn>i").addClass("ki-right-left");

    }
}


$("html").on("keyup keypress change", ".sellqty", function () {
    adjustqtyarrows(this);
});


function adjustqtyarrows(current) {
    var buyamount = parseFloat($(current).closest("tr").find(".buyqty").val());
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
        closesttd.find(".btn").removeClass("btn-success");
        closesttd.find(".btn").addClass("btn-danger");
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

function OpenVendorInvoiceLineItemModal(detailuid, current) {
    blockui();
    $.ajax({
        url: '/VendorInvoice/VendorInvoiceModal?detailuid=' + detailuid,
        type: 'POST',
        data: current == undefined ? { "InvoiceDetailUUID": "" } : {
            "InvoiceQty": $(current).closest("tr").find(".sellqty").val(),
            "InvoicePrice": $(current).closest("tr").find(".sellamount").val(),
            "POQty": $(current).closest("tr").find(".buyqty").val(),
            "POPrice": $(current).closest("tr").find(".buyamount").val()
        },
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalVendorInvoiceLineItem = document.querySelector('#ModalVendorInvoiceLineItem');
            const modal = KTModal.getInstance(ModalVendorInvoiceLineItem);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Processing Request")
        }
    });
}



function SaveInvoiceItemAndNext() {
    SaveInvoiceItem("1");

}

function SaveInvoiceItemAndClose() {
    SaveInvoiceItem("2");
}


function SaveInvoiceItem(saveflag) {
    var validated = $("#formVendorInvoiceLineItem").Validate();
    if (validated) {
        blockui();
        SaveMiscData();
        //var formdata = $("#formPurchaseIntentLineItem").serializeArray();
        var form = document.getElementById('formVendorInvoiceLineItem');
        var formdata = new FormData(form);
        $.ajax({
            url: '/VendorInvoice/SaveVendorInvoiceLineItem',
            data: formdata,
            type: 'POST',
            processData: false,  // Prevent jQuery from converting the data to a string
            contentType: false,  // Prevent jQuery from setting the Content-Type header
            //dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_vendorinvoice_details").html(data);
                //RefreshSticky();
                if (saveflag == "1")// Save and Next
                {
                    var selectvalue = $('input[name="TypeEncrypted"]:checked').val();
                    $("#formVendorInvoiceLineItem").ResetForm();
                    $("#formVendorInvoiceLineItem").find(".select2").select2();
                    $("#formVendorInvoiceLineItem").find("#hdn_InvoiceDetailUUID").val("");
                    $("#formPurchaseIntentLineItem").find("#txt_Description").focus();
                    $('input[name="TypeEncrypted"][value="' + selectvalue + '"]').prop('checked', true).trigger('click');
                }
                else// if (saveflag == "2")// Save and Close
                {
                    const ModalVendorInvoiceLineItem = document.querySelector('#ModalVendorInvoiceLineItem');
                    const modal = KTModal.getInstance(ModalVendorInvoiceLineItem);
                    modal.hide();
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Line Item");
            }
        });
    }
}


$('#btn_VI_Upload').on('click', function () {
    $('#VI_fileInput').click();
});

$('#VI_fileInput').on('change', function () {
    selectedFile = this.files[0];
    if (!selectedFile) {
        ErrorMessage("Please select a file first.");
        return;
    }
    const formData = new FormData();
    formData.append("file", selectedFile);
    $.ajax({
        url: '/VendorInvoice/UploadVendorInvoice?invoiceuuid=' + $("#hdn_InvoiceUUID").val(),
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.ResultId == 1) {
                SuccessMessage("Uploaded Successfully!");
                setTimeout(
                    function () {
                        window.location = window.location.href;
                    }, 500);
            }
            else {
                unblockui();
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function (xhr, status, error) {
            unblockui();
            ErrorMessage("Cannot Upload vendor invoice");
        }
    });
});



function DeleteVendorInvoiceItem(detailuid) {
    blockui();
    SaveMiscData();
    $.ajax({
        url: '/VendorInvoice/DeleteVendorInvoiceItem?detailuid=' + detailuid,
        type: 'GET',
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            $("#section_vendorinvoice_details").html(data);
            CalculateTotal();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Deleting Request")
        }
    });
}
function ShowHideVendorInvoice() {
    $("#section_vendorinvoice_pdf").toggleClass("nowidth");
}

function OpenConfirmation(invoiceuuid, agencydetailid) {
    blockui();
    if (agencydetailid == "" || agencydetailid == undefined)
        agencydetailid = $("#ddlAgencies").val();
    $.ajax({
        url: '/VendorInvoice/GetApproval?invoiceuuid=' + invoiceuuid + '&agencydetailid=' + agencydetailid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalVInvoiceSendApproval = document.querySelector('#ModalVInvoiceSendApproval');
            const modal = KTModal.getInstance(ModalVInvoiceSendApproval);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}

function OpenApproveReject(invoiceuuid, isapprove) {
    blockui();
    $.ajax({
        url: '/VendorInvoice/GetApproveReject?invoiceuuid=' + invoiceuuid + '&accept=' + (isapprove ? 1 : 2),
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalApprovaReject = document.querySelector('#ModalApprovaReject');
            const modal = KTModal.getInstance(ModalApprovaReject);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}

function SendForApproval(invoiceuuid) {
    if ($("#formVISendApproval").Validate()) {
        blockui();
        var data = { "approver": $("#ddlApprovalUser").val(), "message_text": $("#txt_Comments").val(), "target_id": $("#hdn_InvoiceIDInc").val() };

        let formdata = JSON.stringify(data);

        $.ajax({
            url: '/VendorInvoice/SendForApproval?invoiceuuid=' + invoiceuuid,
            data: formdata,
            type: 'POST',
            //dataType: "json",
            contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    SuccessMessage("Saved Successfully!");
                    setTimeout(
                        function () {
                            if (data.ResultMessage == "") {
                                window.location = "/VendorInvoice/VIList";
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
                ErrorMessage("Cannot Order RFQs");
            }
        });
    }
}

function ApproveQuote(invoiceuuid) {
    var data = {
        "invoiceuuid": invoiceuuid,
        "isaccept": true,
        "comments": $("#ModalApprovaReject").find("#txt_Comments").val(),
        "invoiceid": $("#hdn_InvoiceIDInc").val()
    };
    ApproveRejectQuote(data);
}

function RejectQuote(invoiceuuid) {
    if ($("#formApprovalReject").Validate()) {
        var data = {
            "invoiceuuid": invoiceuuid,
            "isaccept": false,
            "comments": $("#ModalApprovaReject").find("#txt_Comments").val(),
            "invoiceid": $("#hdn_InvoiceIDInc").val()
        };
        ApproveRejectQuote(data);
    }
}

function ApproveRejectQuote(data) {
    blockui();
    let formdata = JSON.stringify(data);
    $.ajax({
        url: '/VendorInvoice/ApproveReject',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                SuccessMessage("Saved Successfully!");
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
            ErrorMessage("Cannot Update Vendor Invoice");
        }
    });
}

function ShowJobType(type) {
    if (type == 1) {
        $(".existing").show();
        $(".newitem").hide();
    }
    else {
        $(".existing").hide();
        $(".newitem").show();
    }
}

function FillDescription(current) {
    $("#txt_Description").val($(current).find("option:selected").text());
}

function VITabSwitch(current, jobtype) {
    $("#vitabs .btn-md").removeClass("active");
    $(".VICard").hide();
    $("#VI_Card_" + jobtype).show();
    $(current).addClass("active");
}
