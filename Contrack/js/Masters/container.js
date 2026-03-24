let dataTable;
$(document).ready(function () {
    initContainerTable();
    $(".select2").select2({ width: '100%' });
    $(document).on('change', '.checkall', function () {
        var isChecked = $(this).prop('checked');
        $(".checksingle").prop('checked', isChecked);
        UpdateRecordMoveButton();
    });
    $(document).on('change', '.checksingle', function () {
        UpdateRecordMoveButton();
        if (!$(this).prop('checked')) {
            $('.checkall').prop('checked', false);
        }
    });
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
            checkbox: {
                title: 'checkbox',
                render: (data, row) => {
                    let containerId = row.containerid?.EncryptedValue || "";
                    let bookingId = row.bookingid?.EncryptedValue || "";
                    return ` <input type="checkbox" class="checkbox checkbox-sm checksingle cb-record-move"
                               data-datatable-row-check="true" data-containerid="${containerId}"
                               data-bookingid="${bookingId}"/>`;
                },
                createdCell(cell) {
                    cell.classList.add('text-center');
                    cell.classList.add('nowrap');
                },
            },
            equipmentno: {
                title: 'Equipment No',
                render: (data, row) => {
                    let containerId = row.containerid?.EncryptedValue || "";
                    let containeruuid = row.containeruuid || "";
                    let ageText = row.manufacturedate?.SubText || 'N/A';
                    var age = "/assets/img/agebar1.svg";
                    var agetext = "A+";
                    if (row.ageinyears >= 12) { age = "/assets/img/agebar5.svg"; agetext = "D"; }
                    else if (row.ageinyears >= 7) { age = "/assets/img/agebar4.svg"; agetext = "C"; }
                    else if (row.ageinyears >= 3) { age = "/assets/img/agebar3.svg"; agetext = "B"; }
                    else if (row.ageinyears >= 1) { age = "/assets/img/agebar2.svg"; agetext = "A"; }

                    return `
                    <div class='flex gap-3'>
                        <img src='${age}'/>
                        <div class="flex flex-col gap-0.5">
                            <a class="text-primary font-semibold hover:text-primary-active"
                               href="/Tracking/List?refid=${containeruuid}">${row.equipmentno}
                            </a>
                            <span class="text-gray-600 text-sm font-medium flex items-center gap-1.5">
                                 ${agetext} <span class="smalldot"></span> ${ageText}
                            </span>
                        </div>
                    </div>`;
                }
            },
            Damage: {
                title: 'Damage',
                render: (data, row) => row.isdamaged ? `<img src="/assets/img/explaim.svg" class="listdamage" />` : '<img style="opacity:0.2" src="/assets/img/explaim.svg" class="listdamage" />',
                createdCell(cell) { cell.classList.add('text-center'); }
            },
            status: {
                title: 'Status',
                render: (data, row) => {
                    let html = row.is_empty ? `<span data-tooltip="#advanced_Tracking_${row.containeruuid}" class="flex flex-col items-center"><i class="text-xl text-gray-400 ki-filled ki-cube-2"></i></span>` : `<span data-tooltip="#advanced_Tracking_${row.containeruuid}" class="flex flex-col items-center"><i class="text-xl text-info ki-filled ki-cube-2"></i></span>`;
                    return html + `<div class="tooltip" id="advanced_Tracking_${row.containeruuid}">
                                        <div class="flex items-center gap-1">
                                            ${row.is_empty ? `Empty` : `Full`}
                                            <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                                        </div>
                                    </div>`;
                },
                /*                render: (data, row) => row.isempty ? `<span class="flex flex-col items-center"><span class="badge-styling yellow flex gap-1">Empty</span></span>` : `<span class="flex flex-col items-center"><span class="badge-styling blue flex gap-1">Full</span></span>`,*/
                createdCell(cell) { cell.classList.add('text-center'); }
            },
            available: {
                title: 'Damage',
                render: (data, row) => {
                    let html = row.status_code == 2 ? `<span data-tooltip="#advanced_Status_${row.containeruuid}" class="booked-circle"></span>` : `<span data-tooltip="#advanced_Status_${row.containeruuid}" class="available-circle"></span>`;
                    return html + `<div class="tooltip" id="advanced_Status_${row.containeruuid}">
                                        <div class="flex items-center gap-1">
                                            ${row.status_code == 2 ? `Booked` : `Available`}
                                            <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                                        </div>
                                    </div>`;
                },
                createdCell(cell) { cell.classList.add('text-center'); }
            },
            model: {
                title: 'Model',
                render: (data, row) => {
                    const iso = row.model_iso_code || '-';
                    const details = (row.sizename || '') + ' ' + (row.type_name || '');
                    return `<div class="flex flex-col gap-0.5">
                                <span class="text-gray-900 font-semibold1 text-sm">${details}</span>
                                <span class="text-gray-600 text-sm">ISO: ${iso}</span>
                            </div>`;
                }
            },
            location: {
                title: 'Location',
                render: (data, row) => {
                    if (row.portname == "" && row.locationname != "")
                        return `<div class="flex items-center gap-3">
                                <span class="text-xl locationicon">  <i class="ki-outline text-2xl ki-ship"></i></span>
                                <div class="flex flex-col gap-0.5">
                                    <span class="text-gray-900 font-semibold1 text-sm">${row.locationname || ''}</span>
                                    <span class="text-gray-600 text-sm flex items-center gap-2">Vessel</span>
                                </div>
                            </div>`;
                    else
                        return `<div class="flex items-center gap-3">
                                <span class="text-xl locationicon">${row.locationicon}</span>
                                <div class="flex flex-col gap-0.5">
                                    <span class="text-gray-900 font-semibold1 text-sm">${row.locationname || ''}</span>
                                    <span class="text-gray-600 text-sm flex items-center gap-2">${row.portname}</span>
                                </div>
                            </div>`;
                }
            },

            booking: {
                title: 'Booking',
                render: (data, row) => {
                    if (!row.bookinguuid) {
                        return `<div class="flex items-center no-booking-indicator">
                                    <div class="circleicon"><i class="ki-filled ki-notification-status"></i></div>
                                    <div class="flex flex-col no-booking-content">
                                        <span class="no-booking-head text-gray-900">No Booking Linked</span>
                                        <span class="no-booking-subtext text-gray-500">Idle for last ${row.lastbookingdate?.SubText || ''}</span>
                                    </div>
                                </div>`;
                    }
                    return `<div class="flex flex-col items-center1">
                            <a href="${domain}/Booking/BookingDecider?refid=${row.bookinguuid}" 
                               class="text-primary font-semibold1 text-sm hover:text-primary-active mb-0.5">
                               ${row.bookingno || 'N/A'}
                            </a>
                            <span class="text-gray-600 text-sm">${row.customername || 'N/A'}</span>
                        </div>`;
                }
            },
            route: {
                title: 'POL/POD',
                render: (data, row) => {
                    if (!row.bookinguuid) return "";
                    let polFlagHtml = row.pol_flag ? `<img src="${row.pol_flag}" class="w-6 h-6 rounded-full flex-shrink-0 shadow-sm mr-1">` : '';
                    let podFlagHtml = row.pod_flag ? `<img src="${row.pod_flag}" class="w-6 h-6 rounded-full flex-shrink-0 shadow-sm mr-1">` : '';

                    return `<div class="flex flex-col items-center1 justify-center gap-1">
                                <div class="flex items-center gap-2 whitespace-nowrap">
                                    <div class="flex items-center">${polFlagHtml}<span class="text-gray-700 font-semibold1 text-sm">${row.polname || 'N/A'}</span></div>
                                    <i class="ki-filled ki-right"></i>
                                    <div class="flex items-center">${podFlagHtml}<span class="text-gray-700 font-semibold1 text-sm">${row.podname || 'N/A'}</span></div>
                                </div>
                               
                            </div>`;
                    /*<span class="text-gray-600 text-sm">Voyage: AE1 / 24N</span>*/
                },
                createdCell(cell, cellData, rowData) {
                    //if (!rowData.bookinguuid) cell.style.display = 'none';
                }
            },
            activity: {
                title: 'Last Move',
                render: (data, row) => {
                    if (row.lastmove) {
                        return `<div class="flex items-center gap-2 py-1">
                                    <span class="text-2xl text-gray-400">${row.moveicon || ''}</span>
                                    <div class="flex flex-col gap-0.5">
                                        <span class="text-gray-900 font-semibold1 text-sm">${row.lastmove}</span>
                                        <span  class="text-gray-600 text-2sm">${row.lastmovedatetime}</span>
                                    </div>
                                </div>`;
                    }
                    return ``;
                }
            },
            action: {
                title: 'Action',
                render: (data, row) => {
                    let containerId = row.containerid?.EncryptedValue || row.containeruuid;
                    let bookingId = row.bookingid?.EncryptedValue || "";
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
                                      <a class="menu-link" href="javascript:void(0);" data-containerid="${containerId}"
                                       data-bookingid="${bookingId}" onclick="TriggerSingleRecordMove(this)">
                                           <span class="menu-icon">
                                             <i class="ki-filled ki-route"></i>
                                           </span>
                                           <span class="menu-title">Record Move</span>
                                      </a>
                                </div>
                            </div>
                        </div>
                    </div>`;
                },
                createdCell(cell) { cell.classList.add('text-center'); }
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
            $('.checkall').prop('checked', false);
            UpdateRecordMoveButton();

            setTimeout(() => { if (typeof KTMenu !== 'undefined') KTMenu.createInstances(); }, 50);
        }
    });
    dataTable.on('drew', () => {
        for (var i = 0; i < dataTable._data.length; i++) {
            if (dataTable._data[i].isdamaged) {
                $("#TableContainers").find("tbody tr:nth-child(" + (i + 1) + ")").addClass("bg-red");
            }
        }
    });
}
function UpdateRecordMoveButton() {
    var checkedCount = $(".checksingle:checked").length;
    var btn = $("#btnRecordMove");
    if (checkedCount > 0) {
        btn.removeAttr("disabled");
    } else {
        btn.attr("disabled", "disabled");
    }
}
function RecordMove(ids) {
    if (ids === "") {
        var selectedIds = [];
        $(".checksingle:checked").each(function () {
            selectedIds.push($(this).val());
        });

        if (selectedIds.length === 0) {
            ErrorMessage("Please select at least one container.");
            return;
        }
        ids = selectedIds.join(",");
    }
    console.log("Record Move for: " + ids);
}
function OpenContainerModal(id) {
    blockui();
    $.ajax({
        url: '/Container/GetContainerModal?containerid=' + id,
        type: 'GET',
        cache: false,
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal('ModalAddContainer');
        },
        error: function () {
            unblockui();
            ErrorMessage("Error loading Container");
        }
    });
}
function SaveContainer() {
    var validated = $("#formContainer").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formContainer").serialize();
        $.ajax({
            url: '/Container/SaveContainer',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Container saved successfully");
                    CloseModal('ModalAddContainer');
                    if (dataTable) {
                        dataTable.reload();
                        location.reload();
                    }
                } else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot save container");
            }
        });
    }
}
function FilterStatus(status) {
    var val = status === 0 ? "" : status;
    $("#ddlStatus").val(val).trigger('change');
    $("#frmContainerList").submit();
}