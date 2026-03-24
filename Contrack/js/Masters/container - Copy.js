let debounceTimer;
$(document).ready(function () {
    initContainerTable();
});
const apiUrl = domain + '/TableDisplay/Containers';
const element = document.querySelector('#TableContainers');
function initContainerTable() {
    if (!element) return;

    const dataTableOptions = {
        apiEndpoint: apiUrl,
        pageSize: 20,
        stateSave: false,
        columns: {
            equipmentno: {
                title: 'Equipment No',
                render: (data, row) => {
                    return `
                        <div class="equip-group">
                            <a class="equip-no">
                                ${row.equipmentno}
                            </a>
                            <span class="equip-operator-group">
                                <span class='equip-operator'>
                                    <i class="ki-filled ki-user"></i>
                                    ${row.operatorname}
                                </span>
                                <span class='equip-damage'>
                                    <img src='/assets/img/explaim.svg'/>
                                    Damaged
                                 </span>
                            </span>
                        </div>`;
                }
            },
            model: {
                title: 'Model',
                render: (data, row) => {
                    const iso = row.model_iso_code || '-';
                    const details = (row.type_name || '') + ' ' + (row.sizename || '');
                    return `<div class="equip-type-group">
                                <span class="equip-type">${details}</span>
                                <span class="equip-iso">ISO: ${iso}</span>
                            </div>`;
                }
            },
            location: {
                title: 'Location',
                render: (data, row) => {
                    return `<div class="equip-location-group">
                                <i class="ki-filled ki-geolocation"></i>
                                <div class="equip-type-group">
                                    <span class="equip-type">${row.locationname}</span>
                                    <span class="equip-iso">${row.portname}</span>
                                </div>
                             </div>`;
                }
            },
            booking: {
                title: 'Booking',
                render: (data, row) => {
                    if (!row.bookingno) {
                        return `<div class="no-booking-indicator">
                                    <i class="ki-outline ki-information-2"></i>
                                    <span>NO BOOKING</span>
                                </div>`;
                    }
                    const bookingNo = row.bookingno || 'N/A';
                    const customer = row.customername || 'N/A';
                    return `<div class="flex flex-col">
                                <span class="text-gray-800 font-semibold text-sm">${bookingNo}</span>
                                <span class="text-gray-500 text-xs mt-0.5">${customer}</span>
                            </div>`;
                },
                createdCell(cell, cellData, rowData) {
                    if (!rowData.bookingno) {
                        cell.setAttribute('colspan', '2');
                        cell.classList.add('no-booking-cell');
                    }
                }
            },
            route: {
                title: 'POL/POD',
                render: (data, row) => {
                    if (!row.bookingno) return '';
                    const pol = row.polname || 'N/A';
                    const pod = row.podname || 'N/A';

                    let polFlagHtml = row.pol_flag
                        ? `<img src="${row.pol_flag}"class="w-7 h-7 rounded-full flex-shrink-0 shadow-sm" style="margin-right: 8px;" onerror="this.style.display='none'">`
                        : '';

                    let podFlagHtml = row.pod_flag
                        ? `<img src="${row.pod_flag}" class="w-7 h-7 rounded-full flex-shrink-0 shadow-sm" style="margin-right: 8px;" onerror="this.style.display='none'">`
                        : '';

                    return `<div style="display:flex;align-items:center;flex-wrap:nowrap;white-space:nowrap;">
                                <div style="display:flex;align-items:center;flex-wrap:nowrap;">
                                    ${polFlagHtml}
                                    <span class="text-gray-700 font-semibold text-sm" style="white-space:nowrap;">${pol}</span>
                                </div>
                                <span class="text-gray-400" style="margin: 0 12px; flex: 0 0 auto;">→</span>
                                <div style="display:flex;align-items:center;flex-wrap:nowrap;">
                                    ${podFlagHtml}
                                    <span class="text-gray-700 font-semibold text-sm" style="white-space:nowrap;">${pod}</span>
                                </div>
                            </div>`;
                },
                createdCell(cell, cellData, rowData) {
                    if (!rowData.bookingno) {
                        cell.style.display = 'none';
                        return;
                    }
                    cell.style.whiteSpace = 'nowrap';
                }
            },
            action: {
                title: 'Action',
                render: (data, row) => {
                    let containerId = "";
                    if (row.containerid && row.containerid.EncryptedValue) {
                        containerId = row.containerid.EncryptedValue;
                    } else {
                        containerId = row.containeruuid;
                    }
                    return `
                    <div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">
                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="OpenContainerModal('${containerId}')">
                                        <span class="menu-icon"><i class="ki-filled ki-notepad-edit"></i></span>
                                        <span class="menu-title">Edit</span>
                                    </a>
                                </div>
                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="DeleteContainer('${containerId}')">
                                        <span class="menu-icon"><i class="ki-filled ki-trash text-danger"></i></span>
                                        <span class="menu-title text-danger">Delete</span>
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
    const dataTable = new KTDataTable(element, dataTableOptions);
    dataTable.on('draw', () => {
        if (dataTable._data.length === 0) {
            $('.custom-dt').hide();
            $('.emptydatatable').show();
        } else {
            $('.emptydatatable').hide();
            $('.custom-dt').show();
        }
        KTMenu.init();
    });
}
function OpenContainerModal(id) {
    if (typeof blockui === 'function') blockui();
    const safeId = id ? encodeURIComponent(id) : '';
    $.ajax({
        url: '/Container/GetContainerModal?containerid=' + safeId,
        type: 'GET',
        success: function (data) {
            if (typeof unblockui === 'function') unblockui();
            $("#modalsection").html(data);
            $("#modalsection .select2").select2({
                minimumResultsForSearch: 5,
                width: '100%',
                dropdownParent: $('#ModalAddContainer')
            });
            if (typeof OpenModal === 'function') {
                OpenModal('ModalAddContainer');
            } else {
                const modalEl = document.querySelector('#ModalAddContainer');
                if (modalEl && typeof KTModal !== 'undefined') {
                    const modal = new KTModal(modalEl);
                    modal.show();
                }
            }
        },
        error: function () {
            if (typeof unblockui === 'function') unblockui();
            ErrorMessage("Error loading modal");
        }
    });
}
function SaveContainer(e) {
    if (e) e.preventDefault();
    var equipment = $("#equipmentno").val();
    var model = $("#containermodeluuid").val();
    var operator = $("#operatorid_EncryptedValue").val();
    var location = $("#currentlocationid_EncryptedValue").val();
    if (!equipment) { ErrorMessage("Please enter Equipment No"); return; }
    if (!model || model === "0" || model === "") { ErrorMessage("Please select a Model"); return; }
    if (!operator || operator === "0" || operator === "") { ErrorMessage("Please select an Operator"); return; }
    if (!location || location === "0" || location === "") { ErrorMessage("Please select a Location"); return; }
    if (typeof blockui === 'function') blockui();
    var formdata = $("#formContainer").serializeArray();
    $.ajax({
        url: '/Container/SaveContainer',
        data: formdata,
        type: 'POST',
        dataType: "json",
        success: function (data) {
            if (typeof unblockui === 'function') unblockui();
            var resultId = data.ResultId || data.resultId;
            var resultMessage = data.ResultMessage || data.resultMessage;
            if (resultId == "1") {
                SuccessMessage(resultMessage);
                const modalEl = document.querySelector('#ModalAddContainer');
                if (modalEl && typeof KTModal !== 'undefined') {
                    const modal = KTModal.getInstance(modalEl);
                    if (modal) modal.hide();
                }
                setTimeout(function () { window.location.reload(); }, 500);
            } else {
                ErrorMessage(resultMessage);
            }
        },
        error: function () {
            if (typeof unblockui === 'function') unblockui();
            ErrorMessage("Error communicating with server.");
        }
    });
}
function DeleteContainer(encryptedId) {
    ShowErrorConfirmation({
        title: 'Delete Container?',
        message: 'Are you sure you want to delete this container?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: () => {
            if (typeof blockui === 'function') blockui();
            $.ajax({
                url: '/Container/DeleteContainer',
                type: 'POST',
                data: { containerid: encryptedId },
                success: function (data) {
                    if (typeof unblockui === 'function') unblockui();
                    var resultId = data.ResultId || data.resultId;
                    if (resultId == "1") {
                        SuccessMessage(data.ResultMessage || "Deleted Successfully");
                        setTimeout(function () { window.location.reload(); }, 500);
                    } else {
                        ErrorMessage(data.ResultMessage || data.resultMessage);
                    }
                },
                error: function () {
                    if (typeof unblockui === 'function') unblockui();
                    ErrorMessage("Cannot delete container.");
                }
            });
        }
    });
}
