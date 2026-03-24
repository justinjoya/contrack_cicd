const apiUrl = domain + '/TableDisplay/PurchaseIntentList';
const element = document.querySelector('#TablePurchaseIntentList');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 10,
    columns: {
        //PIDateTime: {
        //    title: 'PIDateTime',
        //},
        PIDateTime: {
            title: 'PIDateTime',
            render: (data, row) => {


                return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                ` + row.PIDateTime + `
                                      </span>
                            <span class="text-2sm text-gray-700 font-normal">
                                ` + row.timeago + `
                            </span>
                        </div>
                    </div>`;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },

        },
        PurchaseIntentCode: {
            title: 'PurchaseIntentCode',
            render: (data, row) => {
                return '<a class="btn btn-link" href="/Purchase/CreatePI?refid=' + row.PurchaseIntentUUIDID + '">' +
                    row.PurchaseIntentCode + '</a> ';
            },
            createdCell(cell) {

                cell.classList.add('nowrap');
            },
        },
        //VesselName: {
        //    title: 'VesselName',
        //},
        VesselName: {
            title: 'VesselName',
            render: (data, row) => {
                var vessel = row.VesselName;
                var vesselcorrected = "";

                var vesselList = vessel.split(",");

                if (vesselList.length > 1) {
                    vesselcorrected = vesselList.slice(0, 1).join(",") + " <span class='text-primary'>+" + (vesselList.length - 1) + "</span>";
                    return '<div class="flex flex-col gap-0.5"><span class="ellipsis text-sm text-gray-900" data-tooltip="#advanced_tooltip_' + row.PurchaseIntentUUIDID + '">' +
                        vesselcorrected +
                        '</span>' +
                        '<div class="tooltip" id="advanced_tooltip_' + row.PurchaseIntentUUIDID + '">' +
                        '<div class="flex  items-center gap-1">' +
                        vessel +
                        '<i class="ki-solid ki-information-5 text-lg text-warning">' +
                        '</i>' +
                        '</div>' +
                        '</div>' +
                        `<span class="flex items-center gap-2 text-xs text-gray-600 font-normal">
                           <span class="flex items-center gap-1">
                            <i class="ki-filled ki-files text-sm text-gray-500">
                            </i>
                            8 Stores
                           </span>
                           <span class="border-r border-r-gray-300 h-4">
                           </span>
                           <span class="flex items-center gap-1">
                            <i class="ki-filled ki-files text-sm text-gray-500">
                            </i>
                            10 Spares
                           </span>
                          </span></div>`;
                } else {
                    vesselcorrected = vesselList.join(";");
                    if (vessel == "")
                        return "";
                    else
                        return `<div class='flex flex-col gap-0.5'><span class='ellipsis text-sm text-gray-900' data-tooltip="#advanced_tooltip_` + row.PurchaseIntentUUIDID + `">` + vessel + `</span>
                            <div class="tooltip" id="advanced_tooltip_` + row.PurchaseIntentUUIDID + `">
                        <div class="flex  items-center gap-1">` +
                            vessel +
                            `<i class="ki-solid ki-information-5 text-lg text-warning">
                        </i>
                        </div>
                        </div>
            <span class="flex items-center gap-2 text-xs text-gray-600 font-normal">
                   <span class="flex items-center gap-1">
                    <i class="ki-filled ki-files text-sm text-gray-500">
                    </i>
                    8 Stores
                   </span>
                   <span class="border-r border-r-gray-300 h-4">
                   </span>
                   <span class="flex items-center gap-1">
                    <i class="ki-filled ki-files text-sm text-gray-500">
                    </i>
                    10 Spares
                   </span>
                  </span></div>`;
                }
            },
        },
        ClientName: {
            title: 'ClientName',
            render: (data, row) => {
                var client = row.ClientName;
                if (client == "")
                    return "";
                else
                    return '<span data-tooltip="#advanced_client_' + row.PurchaseIntentUUIDID + '">' +
                        client +
                        '</span>' +
                        '<div class="tooltip" id="advanced_client_' + row.PurchaseIntentUUIDID + '">' +
                        '<div class="flex items-center gap-1">' +
                        client +
                        '<i class="ki-solid ki-information-5 text-lg text-warning">' +
                        '</i>' +
                        '</div>' +
                        '</div>';


            },
            createdCell(cell) {
                cell.classList.add('ellipsis');
            },
        },
        Port: {
            title: 'Port',
            render: (data, row) => {
                var port = row.Port;
                if (port == "")
                    return "";
                else {
                    var splitarry = port.split('-');
                    if (splitarry.length > 1) {
                        return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="flex  items-center gap-1.5 leading-none font-medium"><span data-tooltip="#advanced_Port_` + row.PurchaseIntentUUIDID + `" class="text-sm ellipsis text-gray-900">
                                ` + splitarry[0].trim() + `
                                      </span>

<div class="tooltip" id="advanced_Port_` + row.PurchaseIntentUUIDID + `">
                            <div class="flex items-center gap-1">` +
                            port +
                            `<i class="ki-solid ki-information-5 text-lg text-warning">
                            </i>
                            </div>
                            </div>
                                </span>
                            <span class="text-2sm text-gray-700 font-normal">
                                ` + splitarry[1].trim() + `
                            </span>
                        </div>
                    </div>`;
                    }
                    else {
                        return '<span data-tooltip="#advanced_Port_' + row.PurchaseIntentUUIDID + '">' +
                            port +
                            '</span>' +
                            '<div class="tooltip" id="advanced_Port_' + row.PurchaseIntentUUIDID + '">' +
                            '<div class="flex items-center gap-1">' +
                            port +
                            '<i class="ki-solid ki-information-5 text-lg text-warning">' +
                            '</i>' +
                            '</div>' +
                            '</div>';
                    }
                }

                return '<span data-tooltip="#advanced_Port_' + row.PurchaseIntentUUIDID + '">' +
                    port +
                    '</span>' +
                    '<div class="tooltip" id="advanced_Port_' + row.PurchaseIntentUUIDID + '">' +
                    '<div class="flex items-center gap-1">' +
                    port +
                    '<i class="ki-solid ki-information-5 text-lg text-warning">' +
                    '</i>' +
                    '</div>' +
                    '</div>';


            },
            createdCell(cell) {
                cell.classList.add('ellipsis-sm1');
            },
        },
        DeliveryDate: {
            title: 'DeliveryDate',
            createdCell(cell) {
                cell.classList.add('nowrap');
            },

        },
        CreatedByUser: {
            title: 'CreatedByUser',
        },
        StatusName: {
            title: 'StatusName',
            render: (data, row) => {
                switch (row.Status) {
                    case 1:
                        return '<span class="badge py-1 badge-primary badge-outline">' + row.StatusName + '</span>';
                    case 2:
                        return '<span class="badge py-1 badge-warning badge-outline">' + row.StatusName + '</span>';
                    case 3:
                        return '<span class="badge py-1 badge-success badge-outline">' + row.StatusName + '</span>';
                    default:
                        return '<span class="badge py-1 badge-dark badge-outline">' + row.StatusName + '</span>';
                }

            },
            createdCell(cell) {
                cell.classList.add('text-center1');
            },
        },
        action: {
            title: 'action',
            render: (data, row) => {
                return `<div class="menu flex-inline" data-menu="true">
                <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                 <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                  <i class="ki-filled ki-dots-vertical">
                  </i>
                 </button>
                 <div class="menu-dropdown menu-default w-full max-w-[175px]" data-menu-dismiss="true">
                  <div class="menu-item">
                   <a class="menu-link" href="#">
                    <span class="menu-icon">
                     <i class="ki-filled ki-search-list">
                     </i>
                    </span>
                    <span class="menu-title">
                     View
                    </span>
                   </a>
                  </div>
                  <div class="menu-item">
                   <a class="menu-link" href="#">
                    <span class="menu-icon">
                     <i class="ki-filled ki-file-up">
                     </i>
                    </span>
                    <span class="menu-title">
                     Export
                    </span>
                   </a>
                  </div>
                  <div class="menu-separator">
                  </div>
                  <div class="menu-item">
                   <a class="menu-link" href="#">
                    <span class="menu-icon">
                     <i class="ki-filled ki-pencil">
                     </i>
                    </span>
                    <span class="menu-title">
                     Edit
                    </span>
                   </a>
                  </div>
                  <div class="menu-item">
                   <a class="menu-link" href="#">
                    <span class="menu-icon">
                     <i class="ki-filled ki-copy">
                     </i>
                    </span>
                    <span class="menu-title">
                     Make a copy
                    </span>
                   </a>
                  </div>
                  <div class="menu-separator">
                  </div>
                  <div class="menu-item">
                   <a class="menu-link" href="#">
                    <span class="menu-icon">
                     <i class="ki-filled ki-trash">
                     </i>
                    </span>
                    <span class="menu-title">
                     Remove
                    </span>
                   </a>
                  </div>
                 </div>
                </div>
               </div>`;
            },

        },

    },
};

const dataTable = new KTDataTable(element, dataTableOptions);

