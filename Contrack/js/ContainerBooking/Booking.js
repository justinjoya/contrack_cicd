const bulkConfig = {
    shipper_email: {
        input: "#txt_shipperemailtemp",
        hidden: "#hdn_shipper_Email",
        list: "#email_bulksummary",
        cls: "emailtext"
    },
    shipper_phone: {
        input: "#txt_shipperphonetemp",
        hidden: "#hdn_shipper_Phone",
        list: "#phone_bulksummary",
        cls: "phonetext"
    },
    consignee_email: {
        input: "#txt_Consigneeemailtemp",
        hidden: "#hdn_consignee_Email",
        list: "#email_bulksummary_consignee",
        cls: "emailtext"
    },
    consignee_phone: {
        input: "#txt_Consigneephonetemp",
        hidden: "#hdn_consignee_Phone",
        list: "#phone_bulksummary_consignee",
        cls: "phonetext"
    }
};


if ($(".customerselect2").length > 0) {
    var $ddlCustomer = $(".customerselect2");
    $ddlCustomer.customerSelect2({ multiline: true, minLength: 1 });
}

if ($("#ddlquotationfor").length > 0) {
    var $ddlCustomer = $("#ddlquotationfor");
    $ddlCustomer.customerSelect2({ multiline: false, minLength: 1 });
}

if ($("#ddlPOL").length > 0) {
    var $ddlPOL = $("#ddlPOL");
    var placeholder = $ddlPOL.data("placeholder-main");
    var subplaceholder = $ddlPOL.data("placeholder-sub");
    $("#ddlPOL").portSelect2({
        multiline: true,
        placeholder: placeholder,
        subtextplaceholder: subplaceholder,
        icon: flagpath + 'pol.png',
        /*  minLength: 0,*/
    });
}
if ($("#ddlPOD").length > 0) {
    var $ddlPOD = $("#ddlPOD");
    var placeholder = $ddlPOD.data("placeholder-main");
    var subplaceholder = $ddlPOD.data("placeholder-sub");
    $("#ddlPOD").portSelect2({
        multiline: true,
        placeholder: placeholder,
        subtextplaceholder: subplaceholder
    });
}

$(function () {
    $('#frmCustomerSelection').on('submit', function (e) {
        $('#btnCustomerSelection').prop('disabled', true);
        e.preventDefault();
        SaveCustomerStep();
    });

    $('#frmLocationSelection').on('submit', function (e) {
        $('#btnLocationSelection').prop('disabled', true);
        e.preventDefault();
        SaveLocationStep();
    });

    if ($("#frmCreateQuotation").length > 0) {
        $('.reloadpricing').on('change', function (e) {
            GetPricingByBookingUUID();
        });
    }
});

function LoadCustomerInfo(current) {
    var id = $(current).attr("id");
    switch (id) {
        case "ddlCustomer":
            GetCustomerInfoWithStats($(current).val());
            break;
        case "ddlShipper":
            GetShipperConsigneeInfo($(current).val(), 1);
            break;
        case "ddlConsignee":
            GetShipperConsigneeInfo($(current).val(), 2);
            break;
        default:
            break;
    }
}

function GetShipperConsigneeInfo(clientdetailid, type) {
    blockui();
    $.ajax({
        url: '/Client/GetShipperConsigneeInfo?clientdetailid=' + clientdetailid + '&type=' + type,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == undefined) { // html
                switch (type) {
                    case 1:
                        $("#shipper_grid").html(data).find(".select2").select2();
                        LoadShipperDataByClientId(clientdetailid);
                        break;
                    case 2:
                        $("#consignee_grid").html(data).find(".select2").select2();
                        LoadConsigneeDataByClientId(clientdetailid);
                        break;
                    default:
                        break;
                }
            }
            else {
                ErrorMessage(data.ResultMessage)
            }
            //RoloadSummary();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting customer info")
        }
    });
}
function RoloadSummary() {
    $.ajax({
        url: '/Booking/ReloadSummary',
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#summarycard").html(data);
        },
        error: function (data) {
        }
    });
}
function GetCustomerInfoWithStats(clientdetailid) {
    blockui();
    $.ajax({
        url: '/Client/GetClientWithEnquiryStats?clientdetailid=' + clientdetailid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#customer-card").html(data).find(".select2").select2();
            if ($("#customer-card").find(".datetimepicker-here").length > 0) {
                $("#customer-card").find(".datetimepicker-here").each(function () {
                    var datatarget = $(this).data("trigger");
                    var dataclear = $(this).data("clear");

                    var datepicker = $(this).flatpickr({
                        enableTime: true,
                        dateFormat: "Y-m-d H:i",
                        altInput: true,
                        altFormat: "M j, Y h:i K"
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
            RoloadSummary();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting customer info")
        }
    });
}
function ChooseCustomerType(current) {
    blockui();
    $.ajax({
        url: '/Booking/SetCustomerType?customertype=' + $(current).val(),
        type: 'POST',
        type: 'POST',
        data: {
            customertype: $(current).val()
        },
        success: function (data) {
            unblockui();
            if (data.ResultId == 1)
                RoloadSummary();
        },
        error: function (data) {
            unblockui();
        }
    });
}


function AddBulkItemByType(type) {
    const cfg = bulkConfig[type];
    if (!cfg) return;

    let value = $(cfg.input).val().trim();
    if (value === "") return;

    let existing = $(cfg.hidden).val();
    existing = existing ? existing + ";" + value : value;
    $(cfg.hidden).val(existing);

    let parts = value.includes(",") ? value.split(",") : value.split(";");

    parts.forEach(v => {
        const html = `
            <li>
                <span class="${cfg.cls}">${v.trim()}</span>
                <a class="btn btn-sm btn-icon btn-clear" href="javascript:void(0);"
                   onclick="RemoveBulkItemByType(this, '${type}')">
                    <i class="ki-filled ki-cross"></i>
                </a>
            </li>`;

        $(cfg.list).append(html);
    });

    $(cfg.input).val("");
    $(cfg.list).show();
}

function RemoveBulkItemByType(current, type) {
    const cfg = bulkConfig[type];
    if (!cfg) return;

    let text = $(current).closest("li").find("." + cfg.cls).text().trim();
    let existing = $(cfg.hidden).val();

    existing = existing
        .split(";")
        .filter(x => x.trim() !== text)
        .join(";");

    $(cfg.hidden).val(existing);
    $(current).closest("li").remove();

    if (existing === "") {
        $(cfg.list).hide();
    } else {
        $(cfg.list).show();
    }
}
function SaveCustomerStep() {
    if ($("#ddlCustomer").val() == "") {
        $('#btnCustomerSelection').prop('disabled', false);
        ErrorMessage("Please choose customer");
        return;
    }
    var validated = $("#frmCustomerSelection").Validate();
    if (!validated) {
        $('#btnCustomerSelection').prop('disabled', false);
        ErrorMessage("Some of required items needs to be filled");
        return;
    }
    blockui();
    var formdata = $("#frmCustomerSelection").serialize();
    $.ajax({
        url: "/Booking/CustomerSelection",
        data: formdata,
        type: "POST",
        dataType: "json",
        success: function (data) {
            unblockui();
            if (data.ResultId == "1") {
                SuccessMessage("Saved Successfully!");
                setTimeout(
                    function () {
                        window.location.href = data.ResultMessage;
                    }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
            $('#btnCustomerSelection').prop('disabled', false);
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong.");
            $('#btnCustomerSelection').prop('disabled', false);
        }
    });
}

function GoBackToCustomer() {
    var bookingUUID = $("#hdnBookingUUID").val();
    if (bookingUUID === null || bookingUUID === "") {
        ErrorMessage("Booking reference missing.");
        return;
    }
    window.location.href = "/Booking/CustomerSelection?refid=" + bookingUUID;
}

var isChangeVoyage = false;
$(document).on("change", "#ddlPOL, #ddlPOD", function () {
    isChangeVoyage = false;
    //$("#hdnVoyageUUID").val("");
    LoadVoyageOptions(false);
});

$(document).on("change", ".voyage-radio", function () {
    $("#hdnVoyageUUID").val($(this).val());
});

$(document).on("click", "#btnChangeVoyage", function () {
    isChangeVoyage = true;
    //$("#hdnVoyageUUID").val("");
    LoadVoyageOptions(false);
});

function LoadVoyageOptions(isEditLoad) {
    var originPortId = $("#ddlPOL").val();
    var destinationPortId = $("#ddlPOD").val();
    var selectedVoyageUuid = $("#hdnVoyageUUID").val();
    if (!originPortId || !destinationPortId) {
        $("#voyageContainer").html(
            '<div class="text-muted p-4">Please select both ports</div>'
        );
        return;
    }
    $.ajax({
        url: "/Booking/GetDirectVoyageSearch",
        type: "POST",
        data: {
            originPortId: originPortId,
            destinationPortId: destinationPortId,
            selectedVoyageUuid: selectedVoyageUuid,
            isChange: isChangeVoyage
        },
        beforeSend: function () {
            $("#voyageContainer").html(
                '<div class="p-4">Loading voyage options...</div>'
            );
        },
        //success: function (html) {
        //    $("#voyageContainer").html(html);
        //    isChangeVoyage = false;
        //    if (isEditLoad && selectedVoyageUuid) {
        //        $("#hdnVoyageUUID").val(selectedVoyageUuid);
        //    }
        //},
        success: function (html) {
            $("#voyageContainer").html(html);
            isChangeVoyage = false;
            if ($("#voyageContainer").find(".voyage-radio").length === 0) {
                $("#hdnVoyageUUID").val("");
            }
            else if (isEditLoad && selectedVoyageUuid) {
                $("#hdnVoyageUUID").val(selectedVoyageUuid);
            }
        },
        error: function () {
            ErrorMessage("Failed to load voyage options");
        }
    });
}
$(document).ready(function () {
    var originPortId = $("#ddlPOL").val();
    var destinationPortId = $("#ddlPOD").val();

    if (originPortId && destinationPortId) {
        isChangeVoyage = false;
        LoadVoyageOptions(true);
    }
});

function SaveLocationStep() {
    /* To add Emails and phone to hidden if no add button is pressed */
    AddBulkItemByType('shipper_email');
    AddBulkItemByType('shipper_phone');
    AddBulkItemByType('consignee_email');
    AddBulkItemByType('consignee_phone');
    /* To add Emails and phone to hidden if no add button is pressed */

    var validated = $("#frmLocationSelection").Validate();
    if (!validated) {
        $('#btnLocationSelection').prop('disabled', false);
        ErrorMessage("Some of required items needs to be filled");
        return;
    }
    blockui();
    var formdata = $("#frmLocationSelection").serialize();
    $.ajax({
        url: "/Booking/LocationSelection",
        type: "POST",
        data: formdata,
        dataType: "json",
        success: function (data) {
            unblockui();
            if (data.ResultId == "1") {
                SuccessMessage("Saved Successfully!");
                setTimeout(
                    function () {
                        window.location.href = data.ResultMessage;
                    }, 500);
            } else {
                ErrorMessage(data.ResultMessage);
            }
            $('#btnLocationSelection').prop('disabled', false);
        },
        error: function (xhr, status, error) {
            unblockui();
            ErrorMessage("Something went wrong while saving location.");
            $('#btnLocationSelection').prop('disabled', false);
        }
    });
}

//function LoadShipperDataByClientId(clientId) {
//    if (!clientId) return;

//    $.ajax({
//        url: '/Booking/LoadShipperPIC',
//        type: 'GET',
//        data: { clientdetailid: clientId },
//        success: function (html) {

//            $("#shipper_pic_container").html(html);
//            var hasOptions = $("#ddlShipperPIC option").length > 1;

//            if (hasOptions) {
//                $("#shipper_pic_textbox").hide();
//                $("#shipper_pic_container").show().find(".select2").select2();
//            } else {
//                $("#shipper_pic_container").hide();
//                $("#shipper_pic_textbox").show();
//            }
//        }
//    });
//}

function LoadShipperDataByClientId(clientId) {
    if (!clientId) return;

    $.ajax({
        url: '/Booking/LoadShipperPIC',
        type: 'GET',
        data: { clientdetailid: clientId },
        success: function (html) {
            $("#shipper_pic_container").html(html);
            var hasOptions = $("#ddlShipperPIC option").length > 1;
            if (hasOptions) {
                $("#shipper_pic_textbox").hide();
                $("#shipper_pic_container").show();

                $("#ddlShipperPIC").select2();
            } else {
                $("#shipper_pic_container").hide();
                $("#shipper_pic_textbox").show();
            }
        }
    });
}


//function LoadConsigneeDataByClientId(clientId) {
//    if (!clientId) return;
//    $.ajax({
//        url: '/Booking/LoadConsigneePIC',
//        type: 'GET',
//        data: { clientdetailid: clientId },
//        success: function (html) {
//            $("#consignee_pic_container").html(html);
//            var hasOptions = $("#ddlConsigneePIC option").length > 1;
//            if (hasOptions) {
//                $("#consignee_pic_textbox").hide();
//                $("#consignee_pic_container").show().find(".select2").select2();
//            }
//            else {
//                $("#consignee_pic_container").hide();
//                $("#consignee_pic_textbox").show();
//            }
//        }
//    });
//}

function LoadConsigneeDataByClientId(clientId) {
    if (!clientId) return;
    $.ajax({
        url: '/Booking/LoadConsigneePIC',
        type: 'GET',
        data: { clientdetailid: clientId },
        success: function (html) {
            $("#consignee_pic_container").html(html);
            var hasOptions = $("#ddlConsigneePIC option").length > 1;
            if (hasOptions) {
                $("#consignee_pic_textbox").hide();
                $("#consignee_pic_container").show();
                $("#ddlConsigneePIC").select2();
            } else {
                $("#consignee_pic_container").hide();
                $("#consignee_pic_textbox").show();
            }
        }
    });
}



$(document).on("change", "#ddlcontainertype, #ddlcontainersize", function () {
    LoadContainerModelByTypeAndSize();
});

function LoadContainerModelByTypeAndSize() {
    var typeId = $("#ddlcontainertype").val();
    var sizeId = $("#ddlcontainersize").val();

    if (!typeId || !sizeId) {
        return;
    }

    $.ajax({
        url: "/Booking/LoadContainerModel",
        type: "GET",
        data: {
            typeid: typeId,
            sizeid: sizeId
        },
        success: function (html) {
            $("#containermodel_container").html(html);
            $("#containermodel_container").show().find(".select2").select2();
        },
        error: function () {
            ErrorMessage("Failed to load container models");
        }
    });
}

function SaveBookingContainer() {
    var validated = $("#formAddBookingContainer").Validate();
    if (!validated) {
        ErrorMessage("Please fill required fields");
        return;
    }

    var qty = parseInt($("#txt_Qty").val(), 10);
    if (isNaN(qty) || qty <= 0) {
        ErrorMessage("Quantity must be greater than 0");
        $("#txt_Qty").focus();
        return;
    }

    var emptyFullValue = $("#ddlEmptyOrFull").val();
    var commodity = $("#txt_Commodity").val().trim();
    if (emptyFullValue === "F" && commodity === "") {
        ErrorMessage("Commodity is required when container is Full.");
        $("#txt_Commodity").focus();
        return;
    }
    if ($("#bookingdetailuuid").val() != "") {
        blockui();
        $.ajax({
            url: '/Booking/ValidateContainerDetail?bookinguuid=' + $("#hdnbookinguuid").val() + '&bookingdetailuuid=' + $("#bookingdetailuuid").val() + "&qty=" + $("#txt_Qty").val(),
            type: 'GET',
            success: function (data) {
                unblockui();
                if (data.ResultId) {
                    SaveBookingContainerInner();
                }
                else {
                    ShowErrorMessageOnly({
                        title: 'Change Container Qty?',
                        message: data.ResultMessage,
                        confirmtext: 'Close',
                        canceltext: 'Cancel',
                        onConfirm: () => {
                            //SaveBookingContainerInner();
                        }
                    });
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Unable to validate");
            }
        });
    }
    else {
        SaveBookingContainerInner();
    }

}

function SaveBookingContainerInner() {
    blockui();
    $.ajax({
        url: '/Booking/SaveBookingContainer',
        type: 'POST',
        data: $("#formAddBookingContainer").serialize(),
        dataType: 'json',
        success: function (data) {
            unblockui();
            if (data.ResultId == "1") {
                SuccessMessage("Container saved successfully!");
                setTimeout(() => {
                    if (data.ResultMessage) {
                        window.location.href = data.ResultMessage;
                    } else {
                        location.reload();
                    }
                }, 300);

            } else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong");
        }
    });
}

function DeleteBookingContainer(id) {
    ShowErrorConfirmation({
        title: 'Delete Container?',
        message: 'Are you sure you want to delete this container?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Booking/DeleteBookingContainer',
                type: 'POST',
                data: { bookingdetailuuid: id },
                dataType: 'json',
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Container deleted successfully!");
                        setTimeout(() => {
                            window.location.href = data.ResultMessage;
                        }, 300);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                }
            });
        }
    });
}


$(document).ready(function () {

    initTab($("#polTab"));

    $(".additional-service-row").each(function () {
        toggleRow($(this));
    });
});

function initTab(container) {
    container.find(".additional-service-uom:not(.select2-hidden-accessible)").select2({
        width: "100%"
    });
}

function toggleRow(row) {
    var checkbox = row.find(".additional-service-check");
    var inputs = row.find(".service-input");
    var select = row.find(".additional-service-uom");

    if (checkbox.is(":checked")) {
        inputs.prop("disabled", false);
        select.prop("disabled", false).trigger("change.select2");
        initTab(row);
    } else {
        inputs.prop("disabled", true);
        select.prop("disabled", true).trigger("change.select2");
    }
}


$(document).on("change", ".additional-service-check", function () {
    toggleRow($(this).closest(".additional-service-row"));
});

$(document).on("click", ".tab-btn", function () {
    var tabId = $(this).data("tab");

    $(".tab-btn").removeClass("active");
    $(this).addClass("active");

    $(".tab-content").removeClass("active");
    $("#" + tabId).addClass("active");

    setTimeout(function () {
        initTab($("#" + tabId));
    }, 50);
});


function SaveBookingAdditionalServices() {
    var isValid = true;
    $(".additional-service-row").each(function () {
        var row = $(this);
        var checkbox = row.find(".additional-service-check");
        var uom = row.find(".additional-service-uom");
        if (checkbox.is(":checked")) {
            if (!uom.val()) {
                ErrorMessage("Please select UOM");
                uom.focus();
                isValid = false;
                return false;
            }
        }
    });

    if (!isValid) return;

    $(".additional-service-row").each(function () {
        var row = $(this);
        if (row.find(".additional-service-check").is(":checked")) {
            row.find(".service-input").prop("disabled", false);
        } else {
            row.find(".service-input").prop("disabled", true);
        }
    });
    blockui();
    var formData = $("#frmContainerSelection").serialize();
    $.ajax({
        url: '/Booking/SaveBookingAdditionalServices',
        type: 'POST',
        data: formData,
        success: function (data) {
            unblockui();
            if (data.ResultId === 1) {
                SuccessMessage("Additional services saved successfully!");
                setTimeout(function () {
                    window.location.href = data.ResultMessage;
                }, 300);
            } else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong");
        }
    });
}

function FillDescription(ctrl) {
    var selectedText = $(ctrl).find("option:selected").text().trim();
    if (selectedText && selectedText !== "Select") {
        $("#txtDescription").val(selectedText);
    } else {
        $("#txtDescription").val("");
    }
    GetPricingByLineItem();
}

$(document).ready(function () {
    var ddl = $("#ddlJobTypeDetailUUID");
    if (ddl.val()) {
        FillDescription(ddl);
    }
});

function OpenQuotationLineItemModal(detailuuid) {
    blockui();
    $.ajax({
        url: '/Booking/QuotationLineItem',
        type: 'GET',
        data: { quotationdetailuuid: detailuuid },
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const modalEl = document.querySelector('#ModalQuotationLineItem');
            if (modalEl) {
                let modal = KTModal.getInstance(modalEl);
                if (!modal) {
                    modal = new KTModal(modalEl);
                }
                modal.show();
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Unable to load line item");
        }
    });
}

// to bind the selected value's text
$(document).on('select2:select', '#TypeSizeCombinedValue', function (e) {
    var selectedText = e.params.data.text;
    $('#TypeSizeCombinedText').val(selectedText);
});


window.ckClassicEditors = window.ckClassicEditors || {};

function initClassicEditor(editorId) {
    if (ckClassicEditors[editorId]) {
        return;
    }
    const el = document.getElementById(editorId);
    if (!el) return;
    ClassicEditor.create(el).then(editor => { ckClassicEditors[editorId] = editor; }).catch(error => {
        console.error(error);
    });
}
initClassicEditor('editor1');

$(document).on("change", "#ddlAgency", function () {
    var agencyId = $(this).val();
    if (!agencyId) {
        return;
    }
    $.ajax({
        url: '/Booking/GetTermsAndConditionsByAgency',
        type: 'GET',
        data: {
            Agency: agencyId
        },
        success: function (response) {
            if (!response.success) return;
            var editor = ckClassicEditors['editor1'];
            if (editor && typeof editor.setData === "function") {
                editor.setData(response.termsText || '');
            } else {
                $("#editor1").val(response.termsText || '');
            }
        },
        error: function () {
            console.error("Failed to load Terms & Conditions");
        }
    });
});

function loadBillToByClient(client) {
    var detailid = typeof client === "string" ? client : $(client).val();
    if (!detailid) {
        $("#txt_BillTo").val("");
        $("#ddlCurrency").val("").select2();
        return;
    }
    blockui();
    $.ajax({
        url: "/Booking/GetBillToByClient",
        type: "POST",
        data: { clientdetailid: detailid },
        success: function (res) {
            unblockui();
            if (!res || !res.success) {
                $("#txt_BillTo").val("");
                $("#ddlCurrency").val("").select2();
                return;
            }
            $("#txt_BillTo").val(res.billTo || "");
            $("#ddlCurrency").val(res.currency).select2();
        },
        error: function () {
            unblockui();
            $("#txt_BillTo").val("");
            $("#ddlCurrency").val("").select2();
        }
    });
}
function loadBillToByCustomerType(source) {
    var customerType = typeof source === "string" ? source : $(source).val();
    if (!customerType) {
        $("#txt_BillTo").val("");
        return;
    }
    $.ajax({
        url: "/Booking/GetBillToByCustomerType",
        type: "POST",
        data: { customertype: customerType },
        success: function (res) {
            if (!res || !res.success) {
                $("#txt_BillTo").val("");
                return;
            }
            $("#txt_BillTo").val(res.billTo || "");
        },
        error: function () {
            $("#txt_BillTo").val("");
        }
    });
}


function SaveQuotationLineItem(saveflag) {
    var validated = $("#formQuotationLineItem").Validate();
    if (!validated) {
        ErrorMessage("Please fill required fields");
        return;
    }
    blockui();
    var form = document.getElementById('formQuotationLineItem');
    var formdata = new FormData(form);
    $.ajax({
        url: '/Booking/SaveQuotationLineItem',
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,

        success: function (html) {
            unblockui();
            $("#quotationLineItemsTable").html(html);
            calculateFooterTotals();
            if (saveflag === "1") {
                $("#formQuotationLineItem").ResetForm();
                $("#hdn_quotationdetailuuid").val("");
                $("#txt_description").focus();
            }
            else {
                const modalEl = document.querySelector('#ModalQuotationLineItem');
                const modal = KTModal.getInstance(modalEl);
                modal.hide();
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Cannot save quotation line item");
        }
    });
}

//let ckClassicEditors = {};

//ClassicEditor
//    .create(document.querySelector('#editor1'))
//    .then(editor => { ckClassicEditors['editor1'] = editor; })
//    .catch(error => { });

$('#frmCreateQuotation').on('submit', function (e) {
    var saveaction = "";
    const submitter = e.originalEvent?.submitter;

    if (submitter) {
        const $btn = $(submitter);
        saveaction = $btn.val();
    }
    e.preventDefault();
    var validated = $("#frmCreateQuotation").Validate();
    if (!validated) return;
    blockui();
    var form = document.getElementById("frmCreateQuotation");
    var formdata = new FormData(form);
    $.ajax({
        url: '/Booking/CreateQuotation',
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                if (saveaction == "Approval") {
                    var uuid = $("#hdn_quotationuuid").val();
                    if (uuid == "") {
                        uuid = data.TargetUUID;
                        $("#hdn_quotationuuid").val(data.TargetUUID);
                    }
                    OpenSendQuotationForApproval(uuid);
                }
                else {
                    SuccessMessage("Quotation saved successfully!");
                    setTimeout(function () {
                        window.location.href = data.ResultMessage;
                    }, 300);
                }

            } else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong");
        }
    });
});


function DeleteQuotationLineItem(detailuid) {
    ShowErrorConfirmation({
        title: 'Delete Quotation Line Item?',
        message: 'Are you sure you want to delete this quotation line item?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Booking/DeleteQuotationLineItem',
                type: 'POST',
                data: { quotationdetailuuid: detailuid },
                success: function (data) {
                    unblockui();
                    $("#quotationLineItemsTable").html(data);
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Error deleting quotation detail");
                }
            });
        }
    });
}

function DeleteQuotation(quotationuuid) {
    ShowErrorConfirmation({
        title: 'Delete Quotation?',
        message: 'Are you sure you want to delete this quotation?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Booking/DeleteQuotationByUUID',
                type: 'POST',
                data: { quotationuuid: quotationuuid },
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("Quotation Deleted successfully!");
                        setTimeout(function () {
                            window.location.href = window.location;
                        }, 300);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Error deleting quotation");
                }
            });
        }
    });
}


function OpenSendQuotationForApproval(quoteuuid) {
    blockui();
    $.ajax({
        url: '/Booking/GetQuoteApproval?QuoteUUID=' + quoteuuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == undefined) {
                $("#modalsection").html(data).find(".select2").select2();
                OpenModal('ModalQuoteSendApproval');
            }
            else {
                if (data.ResultId == 1) {
                    SuccessMessage("Approved Successfully!");
                    setTimeout(
                        function () {
                            window.location = data.ResultMessage;
                        }, 500);
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}

function SaveApprovalAction() {
    if ($("#formQuoteApprovalAction").Validate()) {
        blockui();
        var formdata = $("#formQuoteApprovalAction").serializeArray();
        $.ajax({
            url: '/Booking/SaveApprovalAction',
            data: formdata,
            type: 'POST',
            //dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    SuccessMessage("Saved Successfully!");
                    setTimeout(
                        function () {
                            window.location = data.ResultMessage;
                        }, 500);
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Send for Approval");
            }
        });
    }
}



function OpenApproveReject(quoteuuid, status) {
    blockui();
    $.ajax({
        url: '/Booking/GetApproveReject?QuoteUUID=' + quoteuuid + '&ApprovalStatus=' + status,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            OpenModal('ModalQuotationApprovaReject');
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}


function GetPricingByLineItem() {
    blockui();
    var BookingUUID = $("#hdn_bookinguuid").val();
    var ClientDetailID = $("#ddlquotationfor").val();
    var Currency = $("#ddlCurrency").val();
    var LineItemUUID = $("#ddlJobTypeDetailUUID").val();
    var TypeID = "";
    var SizeID = "";
    var FullEmpty = $("#ddlFullEmpty").val();

    var ddlTypeSizeCombinedValue = $("#ddlTypeSizeCombinedValue").val();
    if (ddlTypeSizeCombinedValue != "") {
        var splitarray = ddlTypeSizeCombinedValue.split("@@");
        TypeID = splitarray[0];
        SizeID = splitarray[1];
    }
    $.ajax({
        url: "/Booking/GetPricingByLineItem",
        type: "POST",
        data: {
            BookingUUID: BookingUUID, ClientDetailID: ClientDetailID, Currency: Currency,
            LineItemUUID: LineItemUUID, TypeID: TypeID, SizeID: SizeID, FullEmpty: FullEmpty
        },
        success: function (res) {
            unblockui();
            $("#ddlAmount").val(res.Amount);
            $("#hdn_templateprice").val(res.Amount);
            $("#ddlUOM").val(res.UOM);
        },
        error: function () {
            unblockui();
            $("#ddlAmount").val("0");
            $("#hdn_templateprice").val("0");
        }
    });
}


function GetPricingByBookingUUID() {
    blockui();
    var BookingUUID = $("#hdn_bookinguuid").val();
    var ClientDetailID = $("#ddlquotationfor").val();
    var Currency = $("#ddlCurrency").val();

    $.ajax({
        url: "/Booking/GetPricingByBookingUUID",
        type: "POST",
        data: {
            BookingUUID: BookingUUID, ClientDetailID: ClientDetailID, Currency: Currency
        },
        success: function (res) {
            unblockui();
            $("#quotationLineItemsTable").html(res).find(".select2").select2();
        },
        error: function () {
            unblockui();
        }
    });
}
function ShowTaxInfo(current) {
    if ($(current).is(":checked")) {
        $("#txtTaxPercentage").removeAttr("disabled");
    }
    else {
        $("#txtTaxPercentage").attr("disabled", true);
    }
}

$("html").on("keyup keypress change", "#txtTaxPercentage", function () {
    $(".TAX").val($(this).val()).change();
});

$("#additionalservicetab .tab-btn").click(function (e) {
    var currenttab = $(this).data("tab");
    $(".tabcontent").hide();
    $("#" + currenttab).show();
});


$(document).on("change", ".type-checkbox", function () {
    var type = $(this).data("type");
    var isChecked = $(this).is(":checked");
    $(".row-checkbox[data-type='" + type + "']")
        .prop("checked", isChecked);
});

$(document).on("change", ".row-checkbox", function () {
    var type = $(this).data("type");
    var total = $(".row-checkbox[data-type='" + type + "']").length;
    var checked = $(".row-checkbox[data-type='" + type + "']:checked").length;
    $(".type-checkbox[data-type='" + type + "']")
        .prop("checked", total === checked);
});

$("#btnSave").click(function () {
    blockui();
    var model = {
        booking: {
            bookinguuid: '@Model.booking.bookinguuid',
            details: []
        }
    };
    $(".row-checkbox:checked").each(function () {
        var detail = {
            bookingdetailuuid: $(this).data("detailuuid")
        };
        model.booking.details.push(detail);
    });

    if (model.booking.details.length === 0) {
        unblockui();
        ErrorMessage("Please select at least one container");
        return;
    }

    $.ajax({
        url: '/Booking/SaveContainerBookingPickSelection',
        data: JSON.stringify(model),
        type: 'POST',
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                SuccessMessage("Saved Successfully!");
                setTimeout(function () {
                    window.location = data.ResultMessage;
                }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Cannot Save Container Selection");
        }
    });
});


// updating the amount on change for Quotation line item
$(document).on("input", ".qty, .amount, .TAX", function () {
    var row = $(this).closest("tr");
    var qty = parseFloat(row.find(".qty").val()) || 0;
    var amount = parseFloat(row.find(".amount").val()) || 0;
    var taxPercent = parseFloat(row.find(".TAX").val()) || 0;
    var subTotal = qty * amount;
    var taxAmount = subTotal * taxPercent / 100;
    var total = subTotal + taxAmount;

    // Update row total
    row.find(".line-total").text(total.toFixed(2));
    row.find(".line-tax").text("(Incl. Tax : " + taxAmount.toFixed(2) + ")");

    if (taxPercent > 0)
        row.find(".line-tax").css("opacity", "1");
    else
        row.find(".line-tax").css("opacity", "0.4");
    calculateFooterTotals();
});

function calculateFooterTotals() {
    var subTotalSum = 0;
    var taxSum = 0;
    $("#quotation_detail_table tbody tr.mainrow").each(function () {
        var qty = parseFloat($(this).find(".qty").val()) || 0;
        var amount = parseFloat($(this).find(".amount").val()) || 0;
        var taxPercent = parseFloat($(this).find(".TAX").val()) || 0;
        var rowSubTotal = qty * amount;
        var rowTax = rowSubTotal * taxPercent / 100;
        subTotalSum += rowSubTotal;
        taxSum += rowTax;
    });
    var grandTotal = subTotalSum + taxSum;
    $("#lblSubTotal").text(subTotalSum.toFixed(2));
    $("#lblTaxAmount").text(taxSum.toFixed(2));
    $("#lblGrandTotal").text(grandTotal.toFixed(2));
}

// Recalculate on page load
$(document).ready(function () {
    calculateFooterTotals();
});


// toggle for booking summary
function toggleNA(checkbox, fieldId) {
    var wrapper = document.getElementById("wrap_" + fieldId);
    if (!wrapper) return;

    if (checkbox.checked) {
        wrapper.style.display = "none";
        var textarea = document.getElementById(fieldId);
        if (textarea) textarea.value = "";
    }
    else {
        wrapper.style.display = "block";
    }
}


// Booking Summary Terms & Conditions
window.ckClassicEditors = window.ckClassicEditors || {};
function initClassicEditor(editorId) {
    const el = document.getElementById(editorId);
    if (!el) return;

    if (window.ckClassicEditors[editorId]) {
        window.ckClassicEditors[editorId]
            .destroy()
            .then(() => {
                delete window.ckClassicEditors[editorId];
                createEditor();
            })
            .catch(error => {
                console.error("Error destroying editor:", error);
                createEditor();
            });
    } else {
        createEditor();
    }

    function createEditor() {
        ClassicEditor
            .create(el)
            .then(editor => {
                window.ckClassicEditors[editorId] = editor;
            })
            .catch(error => {
                console.error("Error initializing editor:", error);
            });
    }
}

function OpenOtherFeesModal(otherfeeuuid) {
    blockui();
    $.ajax({
        url: '/Booking/OtherFeesLineItem',
        type: 'GET',
        data: { otherfeeuuid: otherfeeuuid },
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const modalEl = document.querySelector('#ModalOtherFeeLineItem');
            if (modalEl) {
                let modal = KTModal.getInstance(modalEl);
                if (!modal) {
                    modal = new KTModal(modalEl);
                }
                modal.show();
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Unable to load line item");
        }
    });
}

function SaveOtherFeesLineItem(saveflag) {
    var validated = $("#ModalOtherFeeLineItem").Validate();
    if (!validated) {
        ErrorMessage("Please fill required fields");
        return;
    }
    blockui();
    var form = document.getElementById('formQuotationLineItem');
    if (!form) {
        unblockui();
        console.error("Form not found!");
        return;
    }
    var formdata = new FormData(form);
    $.ajax({
        url: '/Booking/SaveOtherFeesLineItem',
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (html) {
            unblockui();
            $("#otherFeesContainerTable").html(html);
            if (saveflag === "1") {
                $("#formQuotationLineItem")[0].reset();
                $("#otherfeeuuid").val("");
            }
            else {
                const modalEl = document.querySelector('#ModalOtherFeeLineItem');
                const modal = KTModal.getInstance(modalEl);
                if (modal) modal.hide();
            }
        },
        error: function (err) {
            unblockui();
            console.error(err);
            ErrorMessage("Cannot save line item");
        }
    });
}

function DeleteOtherFee(otherfeeuuid) {
    ShowErrorConfirmation({
        title: 'Delete Other Fee Line Item?',
        message: 'Are you sure you want to delete this other fee line item?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Booking/DeleteOtherFeesLineItem',
                type: 'POST',
                data: { otherfeeuuid: otherfeeuuid },
                success: function (data) {
                    unblockui();
                    $("#otherFeesContainerTable").html(data);
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Error deleting other fee line item");
                }
            });
        }
    });
}

function SaveBookingSummary() {
    var validated = $("#formBookingSummary").Validate();
    if (!validated) {
        ErrorMessage("Please fill required fields");
        return;
    }
    $("#formBookingSummary textarea").each(function () {
        if ($(this).val() === "N/A") {
            $(this).val("");
        }
    });
    if (window.ckClassicEditors && window.ckClassicEditors["editor1"]) {
        var editorData = window.ckClassicEditors["editor1"].getData();
        $("#editor1").val(editorData);
    }
    blockui();
    var formData = $("#formBookingSummary").serialize();
    $.ajax({
        url: '/Booking/SaveBookingSummaryDetails',
        type: 'POST',
        data: formData,
        success: function (data) {
            unblockui();
            if (data.ResultId === 1) {
                SuccessMessage("Saved successfully");
                setTimeout(function () {
                    window.location = data.ResultMessage;
                }, 500);

            } else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong");
        }
    });
}