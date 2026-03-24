let dataTable;
$(document).ready(function () {
    initPricingTable();
    if ($("#ddlCustomerPricing").length > 0) {
        var $ddlCustomer = $("#ddlCustomerPricing");
        $ddlCustomer.pricingcustomerSelect2({ multiline: true, minLength: 1, templateid: $("#hdn_pricinguuid").val() });
    }
});

const apiUrl = domain + '/TableDisplay/Pricings';
const element = document.querySelector('#TablePricing');

function initPricingTable() {
    if (!element) return;

    const dataTableOptions = {
        apiEndpoint: apiUrl,
        pageSize: 20,
        stateSave: false,
        columns: {
            description: {
                title: 'Template Details',
                render: (data, row) => {
                    let pricingId = row.PricingID?.NumericValue || "";
                    let refId = row.PricingUUID || "";
                    let desc = row.Description || 'No Description';
                    let tNo = row.TemplateNo || '-';

                    return `
        <div class="flex flex-col">
            <a href="/Pricing/Template?refid=${refId}" data-tooltip="#tooltip_desc_${pricingId}" class="text-primary font-semibold text-sm ellipsis-full-2 max-w-[180px]">
                ${desc}
            </a>

        </div>
        <div class="tooltip" id="tooltip_desc_${pricingId}">
            <div class="flex items-center gap-1">
                ${desc} <i class="ki-solid ki-information-5 text-sm text-warning"></i>
            </div>
        </div>`;
                }
            },
            pol: {
                title: 'POL',
                render: (data, row) => {
                    let flagBase = '/assets/Flags/';
                    let polFlag = row.POL.Flag ? `<img src="${flagBase}${row.POL.Flag}" class="w-6 h-6 rounded-full flex-shrink-0 shadow-sm mr-1" onerror="this.style.display='none'">` : '';
                    let polText = row.POL.PortName || 'N/A';
                    return `<div class="flex items-center">
                                ${polFlag}
                                <span class="text-gray-700 font-semibold text-sm">${polText}</span>
                            </div>`;
                }
            },
            pod: {
                title: 'POD',
                render: (data, row) => {
                    let flagBase = '/assets/Flags/';
                    let podFlag = row.POD.Flag ? `<img src="${flagBase}${row.POD.Flag}" class="w-6 h-6 rounded-full flex-shrink-0 shadow-sm mr-1" onerror="this.style.display='none'">` : '';
                    let podText = row.POD.PortName || 'N/A';
                    return `<div class="flex items-center">
                                ${podFlag}
                                <span class="text-gray-700 font-semibold text-sm">${podText}</span>
                            </div>`;
                }
            },
            Currency: {
                title: 'Currency',
                render: (data, row) => {
                    return `<span class="text-gray-700 font-semibold text-sm">${row.Currency || '-'}</span>`;
                },
                createdCell(cell) {
                    cell.classList.add('text-center');
                }
            },
            clients: {
                title: 'No of Clients',
                render: (data, row) => {
                    let count = row.ClientCount || 0;
                    return `<span class="text-gray-700 font-semibold text-sm">${count} Clients</span>`;
                },
                createdCell(cell) {
                    cell.classList.add('text-center');
                }
            },
            createdby: {
                title: 'Created By',
                render: (data, row) => {
                    return `<span class="text-gray-700 font-semibold text-sm">${row.CreatedByName || '-'}</span>`;
                }
            },
            created: {
                title: 'Created Date',
                render: (data, row) => {
                    const date = row.CreatedAt?.Text || '-';
                    const timeAgo = row.CreatedAt?.SubText || '';
                    return `
                    <div class="flex flex-col gap-0.5">
                        <span class="text-gray-700 font-semibold text-sm">${date}</span>
                        <span class="text-gray-600 text-sm">${timeAgo}</span>
                    </div>`;
                }
            },
            action: {
                title: 'Action',
                render: (data, row) => {
                    let pId = row.PricingID?.EncryptedValue || "";
                    return `
                    <div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">
                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="OpenPricingTemplate('${row.PricingUUID}','')">
                                        <span class="menu-icon"><i class="ki-filled ki-notepad-edit"></i></span>
                                        <span class="menu-title">Edit</span>
                                    </a>
                                </div>
                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="OpenClonePricingHeader('${pId}')">
                                        <span class="menu-icon"><i class="ki-filled ki-copy"></i></span>
                                        <span class="menu-title">Clone Template</span>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>`;
                },
                createdCell(cell) {
                    cell.classList.add('text-center');
                }
            }
        }
    };
    dataTable = new KTDataTable(element, dataTableOptions);
    dataTable.on('draw', () => {
        if (dataTable._data.length === 0) {
            $('.carddatatable').hide();
            $('.emptydatatable').show();
        } else {
            $('.emptydatatable').hide();
            $('.carddatatable').show();
            setTimeout(() => { if (typeof KTMenu !== 'undefined') KTMenu.createInstances(); }, 50);
        }
    });
}
function OpenPricingTemplate(pricinguuid) {
    blockui();
    $.ajax({
        url: '/Pricing/GetPricingModal?pricinguuid=' + pricinguuid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            $("#modalsection .select2").select2();
            $(".ddlPort").portSelect2();
            OpenModal('ModalPricingHeader');
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in getting Template details.");
        }
    });
}


function SavePricingTemplate() {
    var validated = $("#formPricingTemplate").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formPricingTemplate").serializeArray();
        $.ajax({
            url: '/Pricing/SavePricingTemplate',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                unblockui();
                if (data.ResultId == "1") {
                    CloseModal("ModalPricingHeader");
                    SuccessMessage("Saved Successfully!");
                    setTimeout(function () { window.location = data.ResultMessage; }, 500);
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot Save Template!");
            }
        });
    } else {
        ErrorMessage("Please fill all required fields.");
    }
}

function OpenPricingType(pricinguuid, typeid) {
    blockui();
    $.ajax({
        url: '/Pricing/OpenPricingType?pricinguuid=' + pricinguuid + '&typeid=' + typeid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            $("#modalsection .select2").select2();
            OpenModal('ModalPricingType');
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in getting Template details.");
        }
    });
}

function SavePricingType() {
    var validated = $("#formPricingType").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formPricingType").serializeArray();
        $.ajax({
            url: '/Pricing/SavePricingType',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                unblockui();
                if (data.ResultId == "1") {
                    CloseModal("ModalPricingType");
                    SuccessMessage("Saved Successfully!");
                    setTimeout(function () { window.location = data.ResultMessage; }, 500);
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot Save Template!");
            }
        });
    } else {
        ErrorMessage("Please fill all required fields.");
    }
}

function OpenPricingLineItem(typeid, detailid) {
    blockui();
    $.ajax({
        url: '/Pricing/OpenPricingLineItem?typeid=' + typeid + '&detailid=' + detailid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            $("#modalsection .select2").select2();
            OpenModal('ModalPricingDetail');
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in getting Line Item details.");
        }
    });
}
function DeleteLineItem(TypeUUID, DetailUUID) {
    ShowErrorConfirmation({
        title: 'Delete Line Item?',
        message: `Are you sure you want to delete this Line Item?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Pricing/DeletePricingLineItem?TypeUUID=' + TypeUUID + '&DetailUUID=' + DetailUUID,
                type: 'GET',
                success: function (data) {
                    unblockui();
                    SuccessMessage("Delete Successfully!");
                    setTimeout(function () { window.location = window.location.href; }, 500);

                },
                error: function () {
                    unblockui();
                    ErrorMessage("Error in delete Line Item details.");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });

}


function SavePricingDetail() {
    var validated = $("#formPricingDetail").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formPricingDetail").serializeArray();
        $.ajax({
            url: '/Pricing/SavePricingDetail',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                unblockui();
                if (data.ResultId == "1") {
                    CloseModal("ModalPricingDetail");
                    SuccessMessage("Saved Successfully!");
                    setTimeout(function () { window.location = window.location.href; }, 500);
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot Save Line Item!");
            }
        });
    } else {
        ErrorMessage("Please fill all required fields.");
    }
}

function OpenClonePricingCurrency(pricinguuid, sourcecurrencyid) {
    ShowConfirmation({
        title: 'Clone Currency?',
        message: `Are you sure you want to clone this currency?`,
        confirmtext: `Yes, Clone`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Pricing/OpenClonePricingCurrency?pricinguuid=' + pricinguuid + '&sourcecurrencyid=' + sourcecurrencyid,
                type: 'GET',
                success: function (data) {
                    unblockui();
                    $("#modalsection").html(data);
                    $("#modalsection .select2").select2();
                    OpenModal('ModalPricingCurrency');
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Error in getting Template details.");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}


function OpenClonePricingHeader(sourcepricingid) {
    ShowConfirmation({
        title: 'Clone Template?',
        message: `Are you sure you want to clone this template?`,
        confirmtext: `Yes, Clone`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Pricing/OpenClonePricingHeader?sourcepricingid=' + sourcepricingid,
                type: 'GET',
                success: function (data) {
                    unblockui();
                    $("#modalsection").html(data);
                    $("#modalsection .select2").select2();
                    $("#modalsection .ddlPort").portSelect2();
                    OpenModal('ModalPricingHeader');
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Error in getting Template details.");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}

function AddCustomerToList(pricingid, current) {
    blockui();
    var clientid = $(current).val();
    $.ajax({
        url: '/Pricing/AddClientToPricing?pricingid=' + pricingid + '&clientid=' + clientid,
        type: 'GET',
        success: function (data) {
            unblockui();
            var pricinguuid = $("#hdn_pricinguuid").val();
            $(current).val("").pricingcustomerSelect2({ multiline: true, minLength: 1, templateid: $("#hdn_pricinguuid").val() });
            ReloadClients(pricingid, pricinguuid);
            SuccessMessage("Client Added!");
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in adding client.");
        }
    });
}

function ReloadClients(pricingid, pricinguuid) {
    $.ajax({
        url: '/Pricing/ReloadClient?pricingid=' + pricingid + '&pricinguuid=' + pricinguuid,
        type: 'GET',
        success: function (data) {
            $("#client_list").html(data);
        },
        error: function () {
        }
    });
} 