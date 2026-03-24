const apiUrl = domain + '/TableDisplay/PurchaseIntentList';
const element = document.querySelector('#TablePurchaseIntentList');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 20,
    stateSave: false,
    columns: {
        //Expand: {
        //    title: 'Expand',
        //    render: (data, row) => {
        //        return `<button class="btn btn-xs btn-icon btn-clear btn-light"><i class="ki-outline ki-down"></i></button>`;
        //    },
        //    createdCell(cell) {
        //        cell.classList.add('nowrap');
        //        /*cell.classList.add('text-center');*/
        //    },
        //},
        PIDateTime: {
            title: 'PIDateTime',
            render: (data, row) => {
                return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                ` + row.PIDateTime + `
                                      </span>
                            <span class="text-center1 text-2sm text-gray-600 font-normal">
                                ` + row.timeago + `
                            </span>
                        </div>
                    </div>`;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
                /*cell.classList.add('text-center');*/
            },

        },
        PurchaseIntentCode: {
            title: 'PurchaseIntentCode',
            render: (data, row) => {
                return `<span class="flex items-center grow1 gap-2.5">
                            <span class="flex flex-col gap-0.5">
                                <a class="text-primary btn-link1 text-sm1" style="border-bottom:0;" href="` + ("/Purchase/CreatePI?refid=" + row.PurchaseIntentUUIDID + "") + `">
                                    `+ row.PurchaseIntentCode + `
                                </a>
                                <span data-tooltip="#advanced_Agency_` + row.PurchaseIntentUUIDID + `" class="text-xs text-gray-700 font-normal ellipsis">
                                    ` + row.AgencyName + `
                                </span>
                            </span>
                        </span>` +
                    `<div class="tooltip" id="advanced_Agency_` + row.PurchaseIntentUUIDID + `">
                            <div class="flex  items-center gap-1">` + row.AgencyName + `<i class="ki-solid ki-information-5 text-lg text-warning">
                            </i>
                            </div>
                        </div>`;
                return '<span class="flex flex-col items-center gap-0.5"><a class="btn btn-link" href="/Purchase/CreatePI?refid=' + row.PurchaseIntentUUIDID + '">' +
                    row.PurchaseIntentCode + '</a>' +
                    //(row.PurchaseIntentCode == "PI-SYN-04-25-005" ? `<span class="text-center1 text-2sm text-danger font-normal">
                    //            In Draft
                    //        </span>` : "") +
                    `</span>`;
                ;
            },
            createdCell(cell) {
                cell.classList.add('text-center1');
                cell.classList.add('nowrap');
            },
        },
        //comments: {
        //    title: 'comments',
        //    render: (data, row) => {

        //        return `<a class="btn btn-sm btn-icon btn-icon-lg btn-clear btn-light" href="#">
        //          <i class="ki-filled ki-message-text-2"></i>
        //         </a>`;
        //    },
        //    createdCell(cell) {
        //        cell.classList.add('nowrap');
        //        cell.classList.add('text-center');
        //    },

        //},
        VesselName: {
            title: 'VesselName',
            render: (data, row) => {
                var vessel = row.VesselName;
                if (vessel == "") {
                    vessel = "No Vessel";
                }
                var vesselcorrected = "";
                var JobTypes = row.JobTypes;
                var vesselList = vessel.split(",");

                var jobtypesummary = "";
                if (JobTypes != "") {
                    var JobTypesList = JobTypes.split(",");
                    for (var i = 0; i < JobTypesList.length; i++) {
                        if (i < 2) {
                            jobtypesummary = jobtypesummary + `<span class="flex items-center gap-1">
                            <i class="ki-filled ki-cube-2 text-sm text-gray-600">
                            </i>
                            ` + JobTypesList[i] + (i == 1 ? ", ..." : "") +
                                `</span>`;
                        }
                        //else {
                        //    jobtypesummary = jobtypesummary + "<span>+" + (JobTypesList.length - 2) + "</span>";
                        //}
                    }
                }
                //if (jobtypesummary != "") {
                //    jobtypesummary = `<span class="flex items-center gap-2 text-xs text-gray-600 font-normal">` + jobtypesummary + `</span>`;
                //}
                //else {
                //    jobtypesummary = `<span class="flex items-center gap-2 text-xs text-gray-500 font-normal">No Items added</span>`;
                //}
                var remarks = row.Remarks;
                if (remarks == "")
                    jobtypesummary = "";
                else
                    jobtypesummary = `<span class="flex items-center gap-2 text-xs text-gray-600 font-normal ellipsis-2" data-tooltip="#advanced_tooltip_` + row.PurchaseIntentUUIDID + `">` + remarks + `</span>`;

                if (vesselList.length > 1) {
                    vesselcorrected = vesselList.slice(0, 1).join(",") + " <span class='text-primary'>+" + (vesselList.length - 1) + "</span>";
                    return '<div class="flex flex-col gap-0.5"><span class="ellipsis text-sm text-gray-900" data-tooltip="#advanced_tooltip_' + row.PurchaseIntentUUIDID + '">' +
                        vesselcorrected +
                        '</span>' +
                        '<div class="tooltip" id="advanced_tooltip_' + row.PurchaseIntentUUIDID + '">' +
                        '<div class="flex  items-center gap-1">' +
                        vessel + (jobtypesummary != "" ? "<br/>Remarks: " + remarks : "") +
                        '<i class="ki-solid ki-information-5 text-lg text-warning">' +
                        '</i>' +
                        '</div>' +
                        '</div>' + jobtypesummary +
                        `</div>`;
                } else {
                    vesselcorrected = vesselList.join(";");
                    if (vessel == "")
                        return "";
                    else
                        return `<div class='flex flex-col gap-0.5'><span class='ellipsis text-sm text-gray-900' data-tooltip="#advanced_tooltip_` + row.PurchaseIntentUUIDID + `">` + vessel + `</span>
                            <div class="tooltip" id="advanced_tooltip_` + row.PurchaseIntentUUIDID + `">
                        <div class="flex  items-center gap-1">` +
                            vessel + (jobtypesummary != "" ? "<br/>Remarks: " + remarks : "") +
                            `<i class="ki-solid ki-information-5 text-lg text-warning">
                        </i>
                        </div>
                        </div>` + jobtypesummary + `
                        </div>`;
                }
            },
        },
        ClientName: {
            title: 'ClientName',
            render: (data, row) => {

                var JobTypes = row.JobTypes;

                var jobtypesummary = "";
                if (JobTypes != "") {
                    var JobTypesList = JobTypes.split(",");
                    for (var i = 0; i < JobTypesList.length; i++) {
                        if (i < 2) {
                            jobtypesummary = jobtypesummary + `<span class="flex items-center gap-1">
                            <i class="ki-filled ki-cube-2 text-sm text-gray-600">
                            </i>
                            ` + JobTypesList[i] + (i == 1 ? ", ..." : "") +
                                `</span>`;
                        }
                        //else {
                        //    jobtypesummary = jobtypesummary + "<span>+" + (JobTypesList.length - 2) + "</span>";
                        //}
                    }
                }
                if (jobtypesummary != "") {
                    jobtypesummary = `<span class="flex items-center gap-2 text-xs text-gray-600 font-normal">` + jobtypesummary + `</span>`;
                }
                else {
                    jobtypesummary = `<span class="flex items-center gap-2 text-xs text-gray-500 font-normal">No Items added</span>`;
                }
                var client = row.ClientName;
                if (client == "")
                    client = "No Client";

                return `<div class='flex flex-col gap-0.5'><span class='ellipsis text-sm text-gray-900' data-tooltip="#advanced_client_` + row.PurchaseIntentUUIDID + `">` + client + `</span>
                            <div class="tooltip" id="advanced_client_` + row.PurchaseIntentUUIDID + `">
                        <div class="flex  items-center gap-1">` +
                    client +
                    `<i class="ki-solid ki-information-5 text-lg text-warning">
                        </i>
                        </div>
                        </div>` + jobtypesummary + `
                        </div>`;

            },
            //render: (data, row) => {
            //    var client = row.ClientName;
            //    if (client == "")
            //        return "";
            //    else
            //        return '<span data-tooltip="#advanced_client_' + row.PurchaseIntentUUIDID + '">' +
            //            client +
            //            '</span>' +
            //            '<div class="tooltip" id="advanced_client_' + row.PurchaseIntentUUIDID + '">' +
            //            '<div class="flex items-center gap-1">' +
            //            client +
            //            '<i class="ki-solid ki-information-5 text-lg text-warning">' +
            //            '</i>' +
            //            '</div>' +
            //            '</div>';


            //},
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
                            <span class="flex  items-center gap-1.5 leading-none font-medium"><span data-tooltip="#advanced_Port_` + row.PurchaseIntentUUIDID + `" class="text-sm ellipsis-sm text-gray-900">
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

                //return '<span data-tooltip="#advanced_Port_' + row.PurchaseIntentUUIDID + '">' +
                //    port +
                //    '</span>' +
                //    '<div class="tooltip" id="advanced_Port_' + row.PurchaseIntentUUIDID + '">' +
                //    '<div class="flex items-center gap-1">' +
                //    port +
                //    '<i class="ki-solid ki-information-5 text-lg text-warning">' +
                //    '</i>' +
                //    '</div>' +
                //    '</div>';


            },
            createdCell(cell) {
                cell.classList.add('ellipsis-sm1');
            },
        },
        DeliveryDate: {
            title: 'DeliveryDate',
            render: (data, row) => {

                if (row.DeliveryDate == "")
                    return "";
                else {
                    return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-1">
                            <span class="flex items-center gap-1.5 leading-none font-medium1 text-sm text-gray-700">
                                ` + row.DeliveryDate + `
                                      </span>
                            <span class="text-center1 text-xs ` + (row.ETADays < 0 ? "text-danger" : "text-gray-700") + ` font-normal">
                                ` + row.ETADays + ` Days
                            </span>
                        </div>
                    </div>`;
                }
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
                /*cell.classList.add('text-center');*/
            },

        },
        CreatedByUser: {
            title: 'CreatedByUser',
        },
        StatusName: {
            title: 'StatusName',
            render: (data, row) => {
                var statustooltip = '<div class="tooltip" id="advanced_Status_' + row.PurchaseIntentUUIDID + '">' +
                    '<div class="flex items-center gap-1">' +
                    row.StatusName +
                    '<i class="ki-solid ki-information-5 text-lg text-warning">' +
                    '</i>' +
                    '</div>' +
                    '</div>'
                //switch (row.Status) {
                //    case 1:
                //        return '<span data-tooltip="#advanced_Status_' + row.PurchaseIntentUUIDID + '" class="badge py-1 badge-fixed badge-blue badge-outline">' + row.StatusName + '</span>' + statustooltip;
                //    case 2:
                //        return '<span data-tooltip="#advanced_Status_' + row.PurchaseIntentUUIDID + '" class="badge py-1 badge-fixed badge-yellow badge-outline">' + row.StatusName + '</span>' + statustooltip;
                //    case 3:
                //        return '<span data-tooltip="#advanced_Status_' + row.PurchaseIntentUUIDID + '" class="badge py-1 badge-fixed badge-green badge-outline">' + row.StatusName + '</span>' + statustooltip;
                //    default:
                //        return '<span data-tooltip="#advanced_Status_' + row.PurchaseIntentUUIDID + '" class="badge py-1 badge-fixed badge-red badge-outline">' + row.StatusName + '</span>' + statustooltip;
                //}

                return '<span data-tooltip="#advanced_Status_' + row.PurchaseIntentUUIDID + '" class="">' + row.StatusNameStyling + '</span>' + statustooltip;

            },
            createdCell(cell) {
                cell.classList.add('text-center');
                cell.classList.add('nowrap');
            },
        },
        //action: {
        //    title: 'action',
        //    render: (data, row) => {
        //        return `<div class="menu flex-inline" data-menu="true">
        //        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
        //         <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
        //          <i class="ki-filled ki-dots-vertical">
        //          </i>
        //         </button>

        //         <div class="menu-dropdown menu-default w-full max-w-[175px]" data-menu-dismiss="true">
        //          <div class="menu-item">
        //           <a class="menu-link" href="#">
        //            <span class="menu-icon">
        //             <i class="ki-filled ki-search-list">
        //             </i>
        //            </span>
        //            <span class="menu-title">
        //             View
        //            </span>
        //           </a>
        //          </div>
        //          <div class="menu-item">
        //           <a class="menu-link" href="#">
        //            <span class="menu-icon">
        //             <i class="ki-filled ki-file-up">
        //             </i>
        //            </span>
        //            <span class="menu-title">
        //             Export
        //            </span>
        //           </a>
        //          </div>
        //          <div class="menu-separator">
        //          </div>
        //          <div class="menu-item">
        //           <a class="menu-link" href="#">
        //            <span class="menu-icon">
        //             <i class="ki-filled ki-pencil">
        //             </i>
        //            </span>
        //            <span class="menu-title">
        //             Edit
        //            </span>
        //           </a>
        //          </div>
        //          <div class="menu-item">
        //           <a class="menu-link" href="#">
        //            <span class="menu-icon">
        //             <i class="ki-filled ki-copy">
        //             </i>
        //            </span>
        //            <span class="menu-title">
        //             Make a copy
        //            </span>
        //           </a>
        //          </div>
        //          <div class="menu-separator">
        //          </div>
        //          <div class="menu-item">
        //           <a class="menu-link" href="#">
        //            <span class="menu-icon">
        //             <i class="ki-filled ki-trash">
        //             </i>
        //            </span>
        //            <span class="menu-title">
        //             Remove
        //            </span>
        //           </a>
        //          </div>
        //         </div>
        //        </div>
        //       </div>`;
        //    }

        //},

    },
};

const dataTable = new KTDataTable(element, dataTableOptions);

dataTable.on('draw', () => {
    if (dataTable._data.length === 0) {
        // No rows found
        $('.carddatatable').hide();   // hide datatable
        $('.emptydatatable').show();       // show custom empty state
    } else {
        $('.emptydatatable').hide();
        $('.carddatatable').show();
    }
});
