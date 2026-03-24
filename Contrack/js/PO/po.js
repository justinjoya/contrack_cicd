let ckClassicEditors = {};

$(document).ready(function () {
    $('#ddlAgencies').on('change', function () {
        ValidatePOAgency(this);
    });
    var selectedAgency = $('#ddlAgencies').val();
    if (selectedAgency && $('#vendor_placeholder select option').length <= 1) {
        ValidatePOAgency($('#ddlAgencies')[0]);
    }
});

ClassicEditor
    .create(document.querySelector('#editor1'))
    .then(editor => { ckClassicEditors['editor1'] = editor; })
    .catch(error => { });

ClassicEditor
    .create(document.querySelector('#editor2'))
    .then(editor => { ckClassicEditors['editor2'] = editor; })
    .catch(error => { });

$(function () {
    $('#frmCreatePO').on('submit', function (e) {
        var saveaction = "";
        const submitter = e.originalEvent?.submitter;
        if (submitter) {
            saveaction = $(submitter).val();
        }

        if (saveaction === "Download" || saveaction === "Email") {
            return;
        }

        e.preventDefault();

        const $saveButton = $(submitter) || $('#btnSavePO');
        $saveButton.prop('disabled', true);

        var validated = $("#frmCreatePO").Validate();
        if (validated) {
            blockui();
            var formdata = $("#frmCreatePO").serialize();
            $.ajax({
                url: '/PurchaseOrder/Create',
                data: formdata + '&saveaction=' + saveaction,
                type: 'POST',
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("PO Saved Successfully!");
                        setTimeout(function () { window.location.href = data.ResultMessage; }, 500);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                    $saveButton.prop('disabled', false);
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Save PO. A server error occurred.");
                    $saveButton.prop('disabled', false);
                }
            });
        } else {
            ErrorMessage("Please fill all required fields.");
            $saveButton.prop('disabled', false);
        }
    });

    // START: Updated calculation logic for PO table rows
    $(document).on('keyup change', '#po_detail_table .po-calc-inline', function () {
        const $row = $(this).closest('tr');
        const quantity = parseFloat($row.find('input[name$=".quantity"]').val()) || 0;
        const price = parseFloat($row.find('input[name$=".vendorprice"]').val()) || 0;
        const taxPercent = parseFloat($row.find('input[name$=".tax"]').val()) || 0;

        const subtotal = quantity * price;
        const taxAmount = subtotal * (taxPercent / 100);
        const totalAmount = subtotal + taxAmount;
        // Function to format number as currency
        const formatCurrency = (num) => {
            return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        };
        // Update the total and tax amount display
        $row.find('.po-total-price .total-price-val').text(formatCurrency(totalAmount));
        $row.find('.po-total-price .tax-price-val').text(`(Incl. Tax : ${formatCurrency(taxAmount)})`);
    });
    // END: Updated calculation logic

    // START: New functions for global tax controls
    // Applies the global tax percentage to all line items in the table
    $(document).on('keyup change', '#txtGlobalPOTax', function () {
        const globalTaxValue = $(this).val();
        if (globalTaxValue === '' || isNaN(parseFloat(globalTaxValue))) {
            return; // Don't apply if input is empty or invalid
        }

        $('#po_detail_table tbody tr').each(function () {
            const $row = $(this);
            const $taxInput = $row.find('input[name$=".tax"]');
            if ($taxInput.length) {
                $taxInput.val(globalTaxValue);
                // Trigger change to recalculate the row total using the logic above
                $taxInput.trigger('change');
            }
        });
    });
    // END: New functions

    $(document).on('keyup change', '#formPOLineItem .po-calc', function () {
        CalculatePOAmount();
    });

    $(document).on('change', '#formPOLineItem .desc-dropdown', function () {
        var selectedText = $(this).find('option:selected').text();
        if ($(this).val()) {
            $('.po-new-item-fields textarea[name="itemname"]').val(selectedText);
        } else {
            $('.po-new-item-fields textarea[name="itemname"]').val('');
        }
    });

    $(document).on('shown.bs.modal', '#ModalPOLineItem', function () {
        $('input[name="ItemType"]:checked').trigger('click');
        CalculatePOAmount();
    });
});
// START: New function to handle the Tax toggle checkbox
function TogglePOTaxInfo(checkbox) {
    const taxInput = $('#txtGlobalPOTax');
    if ($(checkbox).is(':checked')) {
        taxInput.prop('disabled', false);
        taxInput.focus();
        taxInput.select();
    } else {
        taxInput.prop('disabled', true);
        taxInput.val('0.00');
        // Trigger change to apply 0% tax to all rows
        taxInput.trigger('change');
    }
}
// END: New function
function CalculatePOAmount() {
    var visibleContainer = $('.po-new-item-fields').is(':visible')
        ? $('.po-new-item-fields')
        : $('.po-existing-item-fields');
    var quantity = parseFloat(visibleContainer.find('input[name="quantity"]').val()) || 0;
    var price = parseFloat(visibleContainer.find('input[name="vendorprice"]').val()) || 0;
    var tax = parseFloat(visibleContainer.find('input[name="tax"]').val()) || 0;
    var totalAmount = quantity * price * (1 + tax / 100);
    visibleContainer.find('.txt-total-amount').val(totalAmount.toFixed(2));
}
function ShowPOItemType(type) {
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
function SavePOLineItem(isSaveAndNext) {
    var validated = $("#formPOLineItem").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formPOLineItem").serialize();
        $.ajax({
            url: '/PurchaseOrder/SavePOLineItem',
            data: formdata,
            type: 'POST',
            success: function (data) {
                unblockui();
                $("#section_po_details").html(data);
                //SuccessMessage("Line item saved successfully!");
                if (isSaveAndNext) {
                    var selectvalue = $('input[name="ItemType"]:checked').val();
                    $('#formPOLineItem').ResetForm();
                    $("#formPOLineItem").find(".select2").select2();
                    $("#formPOLineItem").find("#hdn_podetailuuid").val("");
                    $("#formPOLineItem").find("#txt_Description").focus();
                    $('input[name="ItemType"][value="' + selectvalue + '"]').prop('checked', true).trigger('click');
                    CalculatePOAmount();
                } else {
                    CloseModal("ModalPOLineItem");
                }
            },
            error: function (jqXHR) {
                unblockui();
                ErrorMessage("Cannot Save Line Item");
            }
        });
    }
}

function OpenPOLineItemModal(podetailuuid, current) {
    blockui();
    var requestData = { "podetailuuid": podetailuuid };
    $.ajax({
        url: '/PurchaseOrder/GetPOLineItemModal',
        type: 'GET',
        data: requestData,
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            $("#modalsection").find(".select2").select2();
            OpenModal("ModalPOLineItem");
        },
        error: function (xhr, status, error) {
            unblockui();
            console.error(xhr.responseText);
            ErrorMessage("Error in Processing Request: " + error);
        }
    });
}
//function OpenPOLineItemModal(pouuid, podetailuuid) {
//    blockui();
//    $.ajax({
//        url: '/PO/GetPOLineItemModal?pouuid=' + pouuid + '&podetailuuid=' + podetailuuid,
//        type: 'GET',
//        success: function (data) {
//            unblockui();
//            $("#modalsection").html(data);
//            const modal = new KTModal(document.querySelector('#ModalPOLineItem'));
//            modal.show();
//        },
//        error: function () {
//            unblockui();
//            ErrorMessage("Error getting line item form.");
//        }
//    });
//}
function ValidatePOAgency(current) {
    var agencydetailid = $(current).val();
    if (!agencydetailid) return;
    blockui();
    var name = "ContainerPO.PO.vendordetailid";
    $.ajax({
        url: '/Dropdown/GetVendorDropdown?AgencyDetailID=' + agencydetailid + '&name=' + name,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#vendor_placeholder").html(data).find(".select2").select2();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Load Vendors");
        }
    });
}

function DeletePOLineItem(podetailuuid) {
    ShowErrorConfirmation({
        title: 'Delete Line Item?',
        message: `Are you sure you want to delete this line item?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/PurchaseOrder/DeletePOLineItem',
                type: 'POST',
                data: {
                    podetailuuid: podetailuuid
                },
                success: function (data) {
                    unblockui();
                    $("#section_po_details").html(data);
                    SuccessMessage("Line item deleted successfully!");
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Error deleting line item.");
                }
            });
        },
        onCancel: () => {
        }
    });
}