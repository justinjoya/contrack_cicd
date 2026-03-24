const apiUrl = domain + '/TableDisplay/BatchList';
const element = document.querySelector('#TableBatchList');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 20,
    stateSave: false,
    columns: {
        //AgencyName: {
        //    title: 'AgencyName',
        //},
        BatchNo: {
            title: 'BatchNo',
            render: (data, row) => {
                const BatchNo = row.BatchNo;
                return `<span class="flex items-center grow1 gap-2.5">
                            <span class="flex flex-col gap-0.5">
                                <a class="text-primary btn-link text-sm" style="border-bottom:0;" href="` + (row.menus.approval ? ("/Batch/Approval?refid=" + row.BatchUUID + "") : ("/Batch/Create?refid=" + row.BatchUUID + "")) + `">
                                    `+ BatchNo + `
                                </a>
                                <span data-tooltip="#advanced_Agency_` + row.BatchUUID + `" class="text-xs text-gray-700 font-normal ellipsis">
                                    ` + row.AgencyName + `
                                </span>
                            </span>
                        </span>` +
                    `<div class="tooltip" id="advanced_Agency_` + row.BatchUUID + `">
                            <div class="flex  items-center gap-1">` + row.AgencyName + `<i class="ki-solid ki-information-5 text-lg text-warning">
                            </i>
                            </div>
                        </div>`;

                return '<a class="text-primary btn-link" href="/Batch/Create?refid=' + row.BatchUUID + '">' +
                    BatchNo + '</a> ';
            },
            createdCell(cell) {
                cell.classList.add('text-center1');
                cell.classList.add('nowrap');
            },
        },

        PaymentDateFormatted: {
            title: 'PaymentDateFormatted',
        },
        PaymentBank: {
            title: 'PaymentBank',
            render: (data, row) => {
                var result = "";
                result = `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
                        <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                           ` + row.PaymentBank + `
                        </span>`;

                if (row.PaymentMethod != "") {
                    result += `<span class="text-center1 text-2sm text-gray-600 font-normal">
                        via ` + row.PaymentMethod + `
                    </span>`;
                }
                result += `</div></div>`;

                return result;
            },
        },
        TotalAmount: {
            title: 'TotalAmount',
            render: (data, row) => {
                var output = "";
                output = `<div class="flex flex-col gap-0.5">
                        <span class="font-medium text-md text-gray-900">
                            <span class="text-xs text-gray-600">`
                    + row.PaymentCurrency +
                    `</span> ` + row.TotalAmountFormatted + `
                        </span>`;

                //output += `<span class="flex justify-end items-center gap-2 text-xs text-gray-600 font-normal"><span class="flex items-center gap-1">
                //            <i class="ki-filled ki-user text-sm text-gray-600">
                //            </i>
                //            ` + row.NoOfVendors + ` Vendors 
                //            </span><span class="flex items-center gap-1">
                //            <i class="ki-filled ki-cheque text-sm text-gray-600">
                //            </i>
                //             ` + row.NoOfInvoices + ` Invoices 
                //             </span></span>`;

                output += `</div>`;
                return output;
            },
            createdCell(cell) {
                cell.classList.add('text-end');
                cell.classList.add('nowrap');
            },
        },
        CreatedAtFormat: {
            title: 'CreatedAtFormat',
            render: (data, row) => {
                return `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
                        <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                           ` + row.CreatedAtFormat + `
                        </span>
                        <span class="text-center1 text-2sm text-gray-600 font-normal">
                           By ` + row.CreatedBy + `
                       </span>
                     </div>
                </div>`;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },
        },
        //ApprovedBy: {
        //    title: 'ApprovedBy',
        //},
        ApprovedBy: {
            title: 'ApprovedBy',
            render: (data, row) => {
                if (row.Status == 4 && row.ApprovedBy != "") {
                    //return '<span class="text-danger">' + row.approvedbyname + '</span>';
                    return `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
                       
                        <span class="text-center1 text-2sm text-gray-600 font-normal">
                           Assigned to
                       </span>
                        <span class="flex items-center gap-1.5 leading-none font-normal text-sm text-danger">
                           (` + row.ApprovedBy + `)
                        </span>
                     </div>
                </div>`;
                }
                else
                    return row.ApprovedBy;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },
        },

        StatusName: {
            title: 'StatusName',
            render: (data, row) => {
                return '<span>' + row.StatusName + '</span>';
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
                    var str = `<div class="menu justify-center" data-menu="true" >
    <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
        <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
            <i class="ki-filled ki-dots-vertical">
            </i>
        </button>
        <div class="menu-dropdown menu-default w-full max-w-[220px]" data-menu-dismiss="true">`;
                    if (row.menus.approval) {
                        str += `<div class="menu-item">
			                <a class="menu-link" href="/Batch/Approval?refid=`+ row.BatchUUID + `">
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
                    if (row.menus.paid) {
                        str += `<div class="menu-item">
			                <a class="menu-link" href="javascript:void(0);" onclick="MarkasPaid('`+ row.BatchUUID + `');">
			                    <span class="menu-icon">
				                <i class="ki-filled ki-check-circle">
				                </i>
			                    </span>
			                    <span class="menu-title text-start">
				                Mark as Paid
			                    </span>
			                </a>
		                    </div>`;
                    }

                    str += `</div>
                        </div>
                    </div > `;
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