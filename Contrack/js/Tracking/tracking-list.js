let dataTable;

$(document).ready(function () {
    initTrackingTable();
    $(".select2").select2({ width: '100%' });
});

const apiUrl = domain + '/TableDisplay/Trackings';
const element = document.querySelector('#TableTrackings');

function initTrackingTable() {
    if (!element) return;
    var containerUUID = $("#hdnContainerUUID").val()?.trim() || "";

    const dataTableOptions = {
        apiEndpoint: apiUrl,
        pageSize: 20,
        stateSave: false,
        params: {
            containeruuid: containerUUID
        },
        columns: {
            //recorddatetime: {
            //    title: 'Date & Time',
            //    render: (data, row) => {
            //        const date = row.recorddatetime?.Text || '-';
            //        const timeAgo = row.recorddatetime?.SubText || '';
            //        return `<div class="flex flex-col">
            //                    <span class="text-gray-900 font-semibold text-sm">${date}</span>
            //                    <span class="text-gray-600 text-sm ">${timeAgo}</span>
            //                </div>`;
            //    }
            //},
            move: {
                title: 'Move',
                render: (data, row) => {
                    return `<div class="flex items-center gap-2.5">
                                    ${row.move_icon}
                                     <div class="flex flex-col gap-0.5">
                                        <span class="text-gray-900 font-semibold text-sm">${row.movesname}</span>
                                        <span class="text-gray-600 text-sm">${row.recorddatetime.Text}</span>
                                    </div>
                                </div>`;
                }
            },
            location: {
                title: 'Location',
                render: (data, row) => {
                    let flagHtml = row.location_countryflag ? `<img src="${flagpath + row.location_countryflag}" class="w-7 h-7 rounded-full flex-shrink-0 shadow-sm"  onerror="this.style.display='none'">` : '';
                    if (row.locationuuid == "" && row.locationname != "")
                        return `<div class="flex items-center gap-2">
                                <span class="text-xl locationicon">  <i class="ki-outline text-2xl ki-ship"></i></span>
                                <div class="flex flex-col gap-0.5">
                                    <span class="text-gray-900 font-semibold text-sm">${row.locationname || ''}</span>
                                    <span class="text-gray-600 text-sm flex items-center gap-2">Vessel</span>
                                </div>
                            </div>`;
                    else
                        return `<div class="flex items-center gap-2">
                               ${flagHtml}
                                <div class="flex flex-col gap-0.5">
                                    <span class="text-gray-900 font-semibold text-sm">${row.locationname || ''}</span>
                                    <span class="text-gray-600 text-sm">${row.location_portcode || ''}</span>
                                </div>
                            </div>`;
                }
            },
            //container: {
            //    title: 'Container',
            //    render: (data, row) => {
            //        return `<div class="flex flex-col">
            //                    <span class="text-gray-900 font-bold text-sm">${row.containerno || 'N/A'}</span>
            //                    <span class="text-gray-600 text-sm ">${row.containersizetype || ''}</span>
            //                </div>`;
            //    }
            //},
            status: {
                title: 'Status',
                render: (data, row) => {
                    let html = row.isempty ? `<span data-tooltip="#advanced_Tracking_${row.trackinguuid}" class="flex flex-col items-center"><i class="text-xl text-gray-400 ki-filled ki-cube-2"></i></span>` : `<span data-tooltip="#advanced_Tracking_${row.trackinguuid}" class="flex flex-col items-center"><i class="text-xl text-info ki-filled ki-cube-2"></i></span>`;
                    return html + `<div class="tooltip" id="advanced_Tracking_${row.trackinguuid}">
                                        <div class="flex items-center gap-1">
                                            ${row.isempty ? `Empty` : `Full`}
                                            <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                                        </div>
                                    </div>`;
                },
                /*                render: (data, row) => row.isempty ? `<span class="flex flex-col items-center"><span class="badge-styling yellow flex gap-1">Empty</span></span>` : `<span class="flex flex-col items-center"><span class="badge-styling blue flex gap-1">Full</span></span>`,*/
                createdCell(cell) { cell.classList.add('text-center'); }
            },
            damage: {
                title: 'Damage',
                render: (data, row) => row.isdamaged ? `<img src="/assets/img/explaim.svg" class="listdamage" />` : '<span></span>',
                createdCell(cell) { cell.classList.add('text-center'); }
            },
            booking: {
                title: 'Booking',
                render: (data, row) => {
                    if (row.bookinguuid && row.bookingno) {
                        return `<div class="flex flex-col gap-0.5">
                        <a href="${domain}/Booking/BookingDecider?refid=${row.bookinguuid}" 
                           class="text-primary font-semibold text-sm hover:text-primary-active mb-0.5">
                           ${row.bookingno}
                        </a>
                        <span class="text-gray-600 text-sm ">${row.customername || ''}</span>
                    </div>`;
                    }
                    return '';
                }
            },

            nextmove: {
                title: 'Next Activity',
                render: (data, row) => {
                    if (!row.nextmovename) return '';
                    return `<div class="flex flex-col  gap-0.5">
                                <div class="flex items-center gap-1">
                                    <span class="text-primary text-sm font-semibold">${row.nextmovename}</span>
                                    <span class="text-gray-700">@</span>
                                    <span class="text-gray-900 font-semibold text-sm">${row.nextlocationname || ''}</span>
                                </div>
                                <span class="text-gray-600 text-sm ">${row.nextdatetime?.Text || ''}</span>
                            </div>`;
                }
            },
            action: {
                title: 'Action',
                render: (data, row) => {
                    let trackingUuid = row.trackinguuid || "";

                    if (!row.canedit) {
                        return `<span class="text-gray-400">-</span>`;
                    }
                    return `
                    <div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[175px]" data-menu-dismiss="true">
                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="OpenRecordMoveModalByUuid('${trackingUuid}')">
                                        <span class="menu-icon"><i class="ki-filled ki-notepad-edit"></i></span>
                                        <span class="menu-title">Edit</span>
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
            setTimeout(() => { if (typeof KTMenu !== 'undefined') KTMenu.createInstances(); }, 50);
        }
    });

    dataTable.on('drew', () => {
        for (var i = 0; i < dataTable._data.length; i++) {
            if (dataTable._data[i].isdamaged) {
                $("#TableTrackings").find("tbody tr:nth-child(" + (i + 1) + ")").addClass("bg-red");
            }
        }
    });
}

