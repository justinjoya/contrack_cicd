const apiUrl = domain + '/TableDisplay/QuotationLists';
const element = document.querySelector('#TableQuotation');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    pageSize: 10,
    columns: {
        quotationno: {
            title: 'Quotation No',
            render: (data, row) => {
                const q = row.QuotationLists || {};
                const quotationNo = q.quotationno || '-';
                const agencyName = q.agencyname || '-';
                const uuid = q.quotationuuid || '';

                return `
            <span class="flex items-center grow1 gap-2.5">
                <span class="flex flex-col gap-0.5">
                    <span class="flex gap-1">
                        <a class="text-primary btn-link1 text-sm1" style="border-bottom:0;"
                          href="/Booking/CreateQuotation?refid=${uuid}">
                            ${quotationNo}
                        </a>
                    </span>
                    <span data-tooltip="#advanced_Agency_${uuid}"
                          class="text-xs text-gray-700 font-normal ellipsis">
                        ${agencyName}
                    </span>
                </span>
            </span>

            <div class="tooltip" id="advanced_Agency_${uuid}">
                <div class="flex items-center gap-1">
                    ${agencyName}
                    <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                </div>
            </div>
        `;
            },
            createdCell(cell) {
                cell.classList.add('text-center1');
            },
        },
        quoteDate: {
            title: 'Quote Date',
            render: (data, row) => {
                return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                ` + row.QuotationLists.quotedateOnly + `
                                      </span>
                            <span class="text-center1 text-2sm text-gray-600 font-normal">
                                ` + row.QuotationLists.timeago + `
                            </span>
                        </div>
                    </div>`;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
                /*cell.classList.add('text-center');*/
            },

        },
        //billto: {
        //    title: 'Bill To',
        //    render: (data, row) => {

        //        let billTo = row.QuotationLists.billto || "";

        //        let firstLine = billTo.split(/\r?\n/)[0];

        //        return `
        //    <div class="flex flex-col">
        //        <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
        //            ${firstLine}
        //        </span>
        //    </div>
        //`;
        //    },
        //},
        customername: {
            title: 'Customer',
            render: (data, row) => {

                const customerName = row.QuotationLists.customername || '';
                const customerType = (row.QuotationLists.customertypename || '').trim();

                return `<div class="flex flex-col">
                <a class="flex gap-3 items-center text-gray-900 primary-col font-semibold text-sm"href="/Booking/CustomerSelection?refid=${row.QuotationLists.bookinguuid}">
                      ${customerName}
               </a>
              

              </div>`;

            }
        },

        totalamount: {
            title: 'Total Amount',
            render: (data, row) => {
                var output = "";
                output = `<div class="flex flex-col gap-0.5">
                        <span class="font-medium text-md text-gray-900">
                            <span class="text-xs text-gray-600">`
                    + row.QuotationLists.currency +
                    `</span> ` + row.QuotationLists.totalamountformatted + `
                        </span>`;
                output += `</div>`;
                return output;
            },
            createdCell(cell) {
                cell.classList.add('text-end');
                cell.classList.add('nowrap');
            },
        },
        expiryDate: {
            title: 'ExpiryDate',
            render: (data, row) => {

                if (row.QuotationLists.expirydateOnly == "")
                    return "";
                else {
                    return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-1">
                            <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                ` + row.QuotationLists.expirydateOnly + `
                                      </span>
                            <span class="text-center1 text-xs ` + (row.QuotationLists.ETADays < 0 ? "text-danger" : "text-gray-700") + ` font-normal">
                                ` + row.QuotationLists.ETADays + ` Days
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
        createdby: {
            title: 'Created At / By',
            render: (data, row) => {
                return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                ` + row.QuotationLists.createdat.Text + `
                                      </span>
                            <span class="text-center1 text-2sm text-gray-600 font-normal">
                                By ` + row.QuotationLists.createdBy + `
                            </span>
                        </div>
                    </div>`;
                return `
            <div class="flex flex-col">
                
                <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                     ${row.QuotationLists.createdBy}
                </span>
            </div>
        `;
            },
        },
        status: {
            title: 'Status',
            render: (data, row) => {
                return row.QuotationLists.status.SubText;
            },
            createdCell(cell) {
                cell.classList.add('text-center', 'nowrap');
            }
        },
        action: {
            title: 'Action',
            render: (data, row) => {
                if (row.menu.delete || row.menu.approve) {
                    return `
                    <div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">
                                ` + (row.menu.approve ? `<div class="menu-item">
                                    <a class="menu-link" href="/Booking/ApproveQuotation?refid=${row.QuotationLists.quotationuuid}">
                                        <span class="menu-title text-danger">
                                           Approve
                                        </span>
                                    </a>
                                </div>`: ``) + `

                                ` + (row.menu.delete ? `<div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" ')">

                                        <span class="menu-title text-danger">
                                           Delete
                                        </span>
                                    </a>
                                </div>`: ``) + `
                               
                                
                            </div>
                        </div>
                    </div>`;
                }
                return '-';
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            }
        }
    }
};

const dataTable = new KTDataTable(element, dataTableOptions);