let debounceTimer;
$(document).ready(function () {
    //$("#ddlPortFilter").portSelect2();
    //$('#txtSearchDisplay').on('input', function () {
    //    var val = $(this).val();
    //    $("#hdnSearchText").val(val);
    //});
    initLocationTable();
});
const apiUrl = domain + '/TableDisplay/Locations';
const element = document.querySelector('#TableLocations');
let tableData = [];


function initLocationTable() {
    if (!element) return;

    const dataTableOptions = {
        apiEndpoint: apiUrl,
        pageSize: 20,
        stateSave: false,
        columns: {

            locationname: {
                title: 'Location',
                render: (data, row) => {
                    return `<div class="flex items-center gap-3">
                                <span class="text-xl locationicon">${row.location.LocationType.Icon.icon}</span>
                                <div class="flex flex-col gap-0.5">
                                        <span class="text-gray-900 font-semibold1 text-sm">${row.location.LocationName}</span>
                                        <span class="text-gray-600 text-sm flex items-center gap-2">${row.location.LocationCode}<span class="smalldot"></span>${row.location.LocationType.LocationTypeName}</span>
                                    </div>
                            </div>`;
                }
            },

            portname: {
                title: 'Port Name',
                render: (data, row) => {
                    let flagHtml = '';
                    if (row.location.CountryFlag && typeof flagpath !== 'undefined') {
                        flagHtml = `<img src="${flagpath}${row.location.CountryFlag}" class="w-8 h-8 rounded-full shadow-sm" onerror="this.style.display='none'">`;
                    }
                    return `<div class="flex gap-3 items-center port-cell-content" data-port-id="${row.location.PortID?.EncryptedValue || row.location.PortName}">
                                ${flagHtml}
                                <div class="flex flex-col gap-0.5">
                                    <span class="text-gray-900 font-semibold1 text-sm">${row.location.PortName}</span>
                                    <span class="text-gray-600 text-sm ">${row.location.CountryName}</span>
                                </div>
                            </div>`;
                }
            },
            total: {
                title: 'Total',
                render: (data, row) => `<span class="statcount ${row.location.TotalCount == 0 ? 'empty' : ''}">${row.location.TotalCount}</span>`,
                createdCell(cell) {
                    cell.classList.add('text-center');
                }
            },
            available: {
                title: 'Available',
                render: (data, row) => datawithprogressbar(row.location.TotalCount, row.location.AvailableCount, "statcount green"),// `<span class="statcount green ${row.location.AvailableCount == 0 ? 'empty' : ''}">${row.location.AvailableCount}</span>`,
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            blocked: {
                title: 'Blocked',
                render: (data, row) => datawithprogressbar(row.location.TotalCount, row.location.BlockedCount, "statcount gray"),// `<span class="statcount gray ${row.location.BlockedCount == 0 ? 'empty' : ''}">${row.location.BlockedCount}</span>`,
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            booked: {
                title: 'Booked',
                render: (data, row) => datawithprogressbar(row.location.TotalCount, row.location.BookedCount, "statcount yellow"),// `<span class="statcount yellow ${row.location.BookedCount == 0 ? 'empty' : ''}">${row.location.BookedCount}</span>`,
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            repair: {
                title: 'Repair',
                render: (data, row) => datawithprogressbar(row.location.TotalCount, row.location.DamagedCount, "statcount red"),// `<span class="statcount red ${row.location.DamagedCount == 0 ? 'empty' : ''}">${row.location.DamagedCount}</span>`,
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            action: {
                title: 'Action',
                render: (data, row) => {
                    if (row.menu && row.menu.edit) {
                        return `
                    <div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">

                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="OpenLocationModal('${row.location.LocationUUID}')">
                                        <span class="menu-icon">
                                            <i class="ki-filled ki-notepad-edit"></i>
                                        </span>
                                        <span class="menu-title">
                                            Edit
                                        </span>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>`;
                    }
                    return '<span class="text-gray-400">-</span>';
                },
                createdCell(cell) {
                    cell.classList.add('text-center');
                }
            }
        }
    };
    const dataTable = new KTDataTable(element, dataTableOptions);
    dataTable.on('draw', () => {
        if (dataTable._data.length === 0) {
            $('.custom-dt').hide();
            $('.emptydatatable').show();
        } else {
            $('.emptydatatable').hide();
            $('.custom-dt').show();
        }
    });
}

function PrepareFilterData(isBlocking) {
    if (isBlocking) {
        blockui();
    }
    $("#hdnSearchText").val($("#txtSearchDisplay").val());
    $("#hdnPortID").val($("#ddlPortFilter").val());
    var types = [];
    $(".chk-type:checked").each(function () {
        types.push($(this).val());
    });
    $("#hdnLocationTypeID").val(types.join(","));
}
function OpenLocationModal(refid) {
    blockui();
    $.ajax({
        url: '/Location/GetLocationModal?refid=' + (refid || ''),
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            $("#modalsection .select2").select2();
            $("#ddlPort").portSelect2();
            OpenModal('ModalAddLocation');
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in getting Location details.");
        }
    });
}
function SaveLocation() {
    var validated = $("#formLocation").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formLocation").serializeArray();
        $.ajax({
            url: '/Location/SaveLocation',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                unblockui();
                if (data.ResultId == "1") {
                    SuccessMessage(data.ResultMessage);
                    const modal = KTModal.getInstance(document.querySelector('#ModalAddLocation'));
                    if (modal) modal.hide();
                    setTimeout(function () { window.location.reload(); }, 500);
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot Save Location!");
            }
        });
    } else {
        ErrorMessage("Please fill all required fields.");
    }
}
function DisableLocation(encryptedId, uuid) {
    ShowErrorConfirmation({
        title: 'Disable Location?',
        message: 'Are you sure you want to disable this location?',
        confirmtext: 'Yes, Disable',
        canceltext: 'Cancel',
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Location/DisableLocation',
                type: 'POST',
                data: {
                    location: {
                        LocationID: { EncryptedValue: encryptedId },
                        LocationUUID: uuid
                    }
                },
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Location disabled successfully!");
                        setTimeout(function () { window.location.reload(); }, 500);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot disable location.");
                }
            });
        }
    });
}