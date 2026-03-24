const apiUrl = domain + '/TableDisplay/AwaitingApprovalsList';
const element = document.querySelector('#TableAwaitingApprovalsList');

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
        comparisoncode: {
            title: 'comparisoncode',
            render: (data, row) => {


                return `<div class="flex items-center grow gap-2.5">
                            <div class="flex flex-col gap-0.5">
                                <a class="text-sm font-medium text-primary hover:text-primary-actives" href="/Purchase/QuoteApproval?refid=` + row.comparisonuuid + `">
                                    ` + row.comparisoncode + `
                                </a>
                                <span class="text-xs font-normal text-gray-700">
                                    ` + row.totalrfqs + ` RFQs added
                                </span>
                            </div>
                        </div>`;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
                /*cell.classList.add('text-center');*/
            },

        },
        picode: {
            title: 'picode',
            createdCell(cell) {
                cell.classList.add('text-center');
                cell.classList.add('nowrap');
            },
            //render: (data, row) => {
            //    return '<span class="flex flex-col items-center gap-0.5"><a class="btn btn-link text-gray-900" href="/Purchase/CreatePI?refid=' + row.piuuid + '">' +
            //        row.picode + '</a>' +
            //        //(row.PurchaseIntentCode == "PI-SYN-04-25-005" ? `<span class="text-center1 text-2sm text-danger font-normal">
            //        //            In Draft
            //        //        </span>` : "") +
            //        `</span>`;
            //    ;
            //},
            //createdCell(cell) {
            //    cell.classList.add('text-center');
            //    cell.classList.add('nowrap');
            //},
        },
        createdat: {
            title: 'createdat',
            render: (data, row) => {
                return `<div class="flex items-center gap-2.5">
                            <div class="flex flex-col items-start gap-0.5">
                                <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                    ` + row.createdtime + `
                                </span>
                                <span class="text-center1 text-2sm text-gray-600 font-normal">
                                    ` + row.createdtimeago + `
                                </span>
                            </div>
                        </div>`;
            },
            createdCell(cell) {
                cell.classList.add('text-center');
                cell.classList.add('nowrap');
            },
        },
        approveddatetime: {
            title: 'approveddatetime',
            render: (data, row) => {
                if (row.approvedtime != "-") {
                    return `<div class="flex items-center gap-2.5">
                            <div class="flex flex-col items-start gap-0.5">
                                <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                    ` + row.approvedtime + `
                                </span>
                                <span class="text-center1 text-2sm text-gray-600 font-normal">
                                    ` + row.approvedtimeago + `
                                </span>
                            </div>
                        </div>`;
                }
                else
                    return "";
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },
        },

        approvedbyname: {
            title: 'approvedbyname',
        },

        statusfull: {
            title: 'statusfull',
            render: (data, row) => {
                return row.statusfull;
            },
        },
        action: {
            title: 'action',
            render: (data, row) => {
                var result = ``;
                result = `<div class="menu justify-center" data-menu="true">
                            <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                                <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                    <i class="ki-filled ki-dots-vertical">
                                    </i>
                                </button>
                                <div class="menu-dropdown menu-default w-full max-w-[175px]" data-menu-dismiss="true">`;

                if (row.status == 2) {
                    result = result + `<div class="menu-item">
                                            <a class="menu-link" href="/Purchase/QuoteApproval?refid=` + row.comparisonuuid + `">
                                                <span class="menu-icon">
                                                    <i class="ki-filled ki-check-circle">
                                                    </i>
                                                </span>
                                                <span class="menu-title">
                                                    Approve
                                                </span>
                                            </a>
                                        </div>`;
                }
                else if (row.status == 3) {
                    result = result + `<div class="menu-item">
                                            <a class="menu-link" href="/Purchase/QuoteApproval?comparisonref=` + row.comparisonuuid + `">
                                                <span class="menu-icon">
                                                    <i class="ki-filled ki-file-added"></i>

                                                </span>
                                                <span class="menu-title">
                                                    Create Quote
                                                </span>
                                            </a>
                                        </div>`;
                }
                result = result + ` <div class="menu-item">
                                        <a class="menu-link" href="#">
                                            <span class="menu-icon">
                                                <i class="ki-filled ki-trash">
                                                </i>
                                            </span>
                                            <span class="menu-title">
                                                Delete
                                            </span>
                                        </a>
                                    </div>`

                result = result + ` </div>
                            </div>
                        </div>`;

                return result;
            }

        },

    },
};

const dataTable = new KTDataTable(element, dataTableOptions);