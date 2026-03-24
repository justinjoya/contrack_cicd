const apiUrl = domain + '/TableDisplay/QuoteCompareList';
const element = document.querySelector('#TableQuoteCompareList');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    pageSize: 20,
    stateSave: false,
    columns: {
        comparisoncode: {
            title: 'comparisonid',
            render: (data, row) => {
                return `<span class="flex items-center grow1 gap-2.5">
                            <span class="flex flex-col gap-0.5">
                                <a class="text-primary btn-link1 text-sm1" style="border-bottom:0;" href="` + (row.menus.approval || row.menus.techapproval || row.menus.hodapproval ? ("/Purchase/QuoteApproval?refid=" + row.comparisonuuid + "") : ("/Purchase/CompareQuotes?refid=" + row.comparisonuuid + "")) + `">
                                    `+ row.comparisoncode + `
                                </a>
                                <span data-tooltip="#advanced_Agency_` + row.comparisonuuid + `" class="text-xs text-gray-700 font-normal ellipsis">
                                    ` + row.agencyname + `
                                </span>
                            </span>
                        </span>` +
                    `<div class="tooltip" id="advanced_Agency_` + row.comparisonuuid + `">
                            <div class="flex  items-center gap-1">` + row.agencyname + `<i class="ki-solid ki-information-5 text-lg text-warning">
                            </i>
                            </div>
                        </div>`;

            },
            createdCell(cell) {
                cell.classList.add('text-center1');
                cell.classList.add('nowrap');
            },
        },
        totalrfqs: {
            title: 'totalrfqs',
            render: (data, row) => {
                return '' + row.totalrfqs + (row.totalrfqs === 1 ? ' RFQ' : ' RFQs');
            },
            createdCell(cell) {
                cell.classList.add('text-center');
                cell.classList.add('nowrap');
            },
        },
        createdtime: {
            title: 'createdat',
            render: (data, row) => {
                return `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
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
                cell.classList.add('nowrap');
            },
        },
        approvedbyname: {
            title: 'approvedby',
            render: (data, row) => {
                if (row.status == 6 && row.approvedbyname != "") {
                    //return '<span class="text-danger">' + row.approvedbyname + '</span>';
                    return `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
                       
                        <span class="text-center1 text-2sm text-gray-600 font-normal">
                           Assigned to
                       </span>
                        <span class="flex items-center gap-1.5 leading-none font-normal text-sm text-danger">
                           (` + row.approvedbyname + `)
                        </span>
                     </div>
                </div>`;
                }
                else
                    return row.approvedbyname;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },
        },
        approvedbyname2: {
            title: 'approvedby',
            render: (data, row) => {
                if (row.status == 7 && row.approvedbyname2 != "") {
                    //return '<span class="text-danger">' + row.approvedbyname + '</span>';
                    return `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
                       
                        <span class="text-center1 text-2sm text-gray-600 font-normal">
                           Assigned to
                       </span>
                        <span class="flex items-center gap-1.5 leading-none font-normal text-sm text-danger">
                           (` + row.approvedbyname2 + `)
                        </span>
                     </div>
                </div>`;
                }
                else
                    return row.approvedbyname2;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },
        },
        statusfull: {
            title: 'statusfull',
            render: (data, row) => {
                return '<span>' + row.statusfull + '</span>';
            },
            createdCell(cell) {
                cell.classList.add('text-center');
                cell.classList.add('nowrap');
            },
        },
        status: {
            title: 'status',
            render: (data, row) => {
                var str = "";
                if (!row.menus.show) {
                    str = "-";
                }
                else {
                    str = `<div class="menu justify-center" data-menu="true" >
    <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
        <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
            <i class="ki-filled ki-dots-vertical">
            </i>
        </button>
        <div class="menu-dropdown menu-default w-full max-w-[220px]" data-menu-dismiss="true">`;
                    if (row.menus.approval) {
                        str += `<div class="menu-item">
			                <a class="menu-link" href="/Purchase/QuoteApproval?refid=`+ row.comparisonuuid + `">
			                    <span class="menu-icon">
				                <i class="ki-filled ki-check-circle">
				                </i>
			                    </span>
			                    <span class="menu-title text-start">
				                Approve
			                    </span>
			                </a>
		                    </div>`;
                    }
                    if (row.menus.techapproval) {
                        str += `<div class="menu-item">
			                <a class="menu-link" href="/Purchase/QuoteApproval?refid=`+ row.comparisonuuid + `">
			                    <span class="menu-icon">
				                <i class="ki-filled ki-check-circle">
				                </i>
			                    </span>
			                    <span class="menu-title text-start">
				                Approve Technical
			                    </span>
			                </a>
		                    </div>`;
                    }
                    if (row.menus.hodapproval) {
                        str += `<div class="menu-item">
			                <a class="menu-link" href="/Purchase/QuoteApproval?refid=`+ row.comparisonuuid + `">
			                    <span class="menu-icon">
				                <i class="ki-filled ki-check-circle">
				                </i>
			                    </span>
			                    <span class="menu-title text-start">
				                Approve HOD
			                    </span>
			                </a>
		                    </div>`;
                    }
                    if (row.menus.quote) {
                        str += `<div class="menu-item">
			                <a class="menu-link" href="/Quote/Create?comparisonref=`+ row.comparisonuuid + `">
			                    <span class="menu-icon">
				                <i class="ki-filled ki-file-added">
				                </i>
			                    </span>
			                    <span class="menu-title text-start">
				                Create Quote
			                    </span>
			                </a>
		                    </div>`;
                    }
                    if (row.menus.delete) {
                        str += `<div class="menu-item">
                                <a class="menu-link" href="javascript:void(0);" onclick="DeleteQuoteComparison('` + row.comparisonuuid + `');">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-trash">
                                        </i>
                                    </span>
                                    <span class="menu-title">
                                        Delete
                                    </span>
                                </a>
                            </div>`;
                    }
                    str += `</div>
                                </div>
                                </div> `;
                }
                return str;
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
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