const apiUrl = domain + '/TableDisplay/PurchaseOrderList';
const element = document.querySelector('#TablePurchaseOrderList');


const dataTableOptions = {
    apiEndpoint: apiUrl,
    pageSize: 20,
    stateSave: false,
    columns: {
        vendor_name: {
            title: 'vendor_name',
            render: (data, row) => {
                const vendor_name = row.vendor_name || 'N/A';
                return '<a class="text-sm font-medium text-gray-900 hover:text-primary-active" href="#">' +
                    vendor_name + '</a> <br> <span class="text-xs font-normal text-gray-700"> No Ratings </span > ';
            },
        },
        pocode: {
            title: 'pocode',
            render: (data, row) => {
                const typeBadge = row.isdirectpo ? `<span class="text-warning text-xs font-bold ml-1">(D)</span>` : ``;
                return `<span class="flex items-center grow1 gap-2.5">
                            <span class="flex flex-col gap-0.5">
                                <span class="flex gap-1">
                                <a class="text-primary btn-link text-sm" style="border-bottom:0;" href="/PurchaseOrder/Create?refid=${row.pouuid}">
                                    ${row.pocode || 'Draft'}
                                </a>
                                ${typeBadge}
                                </span>
                                <span class="text-xs text-gray-700 font-normal ellipsis">
                                    ${row.agencyname || 'N/A'}
                                </span>
                            </span>
                        </span>`;
            },
            createdCell(cell) {
                cell.classList.add('text-center1');
            },
        },
        no_of_items: {
            title: 'no_of_items',
            render: (data, row) => {
                return '<div class="flex flex-col gap-0.5"><span class="text-sm font-medium">' + (row.no_of_items || 0) + ' Items</span></div>';
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
        issuedate: {
            title: 'issuedate',
            render: (data, row) => {
                if (!row.issuedate) return '';
                return `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
                        <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                           ${row.issuedate.Text || ''}
                        </span>
                        <span class="text-center1 text-2sm text-gray-600 font-normal">
                           ${row.issuedate.SubText || ''}
                       </span>
                     </div>
                </div>`;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },
        },
        createdby_fullname: {
            title: 'createdby_fullname',
            render: (data, row) => {
                return `<span>${row.createdby_fullname || 'N/A'}</span>`;
            }
        },
        status: {
            title: 'Status',
            render: (data, row) => {
                return `${row.status.SubText}`;
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
        total_amount: {
            title: 'total_amount',
            render: (data, row) => {
                const cur = row.currency || 'USD';
                const amt = row.total_amount ? parseFloat(row.total_amount).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '0.00';
                return `<div class="flex flex-col gap-0.5">
                        <span class="font-medium text-md text-gray-900">
                            <span class="text-xs text-gray-600">${cur}</span> ${amt}
                        </span>
                        </div>`;
            },
            createdCell(cell) {
                cell.classList.add('text-end');
                cell.classList.add('nowrap');
            },
        },
    },
};

const dataTable = new KTDataTable(element, dataTableOptions);
dataTable.on('draw', () => {
    if (dataTable._data.length === 0) {
        $('.carddatatable').hide();
        $('.emptydatatable').show();
    } else {
        $('.emptydatatable').hide();
        $('.carddatatable').show();
    }
});