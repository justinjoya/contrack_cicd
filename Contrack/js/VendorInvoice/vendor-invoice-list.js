const apiUrl = domain + '/TableDisplay/VendorInvoiceList';
const element = document.querySelector('#TableVendorInvoiceList');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 20,
    stateSave: false,
    columns: {
        checkbox: {
            title: 'checkbox',
            render: (data, row) => {
                if (row.menus.checkbox) {
                    return `<input class="checkbox checkbox-sm checksingle" data-datatable-row-check="true" type="checkbox" value="` + row.vendor_invoice_uuid + `">`;
                }
                else {
                    return "-";
                }
            },
            createdCell(cell) {
                cell.classList.add('text-center');
                cell.classList.add('nowrap');
            },
        },
        vendor_name: {
            title: 'vendor_name',
            render: (data, row) => {
                const vendor_name = row.vendor_name;
                /*<span class='text-gray-500'>N/A</span>*/
                //return '<a class="text-sm font-medium text-gray-900 hover:text-primary-actives" href="#">' +
                //    vendor_name + '</a> <br> <span class="text-xs font-normal text-gray-700">' + row.remarks + '</span > ';

                return `<span class="flex items-center grow1 gap-2.5">
                            <span class="flex flex-col gap-0.5">
                                <span class="text-sm font-medium text-gray-900">
                                    `+ vendor_name + `
                                </span>
                                ` + (row.remarks == "" ? "" : `<span data-tooltip-placement="right" data-tooltip="#advanced_Comments_` + row.vendor_invoice_uuid + `" class="text-2sm italic1 text-gray-700 font-normal ellipsis-2">
                                    `  + row.remarks + `
                                </span>`) + `
                                
                            </span>
                        </span>` +
                    `<div class="tooltip" id="advanced_Comments_` + row.vendor_invoice_uuid + `">
                            <div class="flex  items-center gap-1">` + row.remarks + `<i class="ki-solid ki-information-5 text-lg text-warning">
                            </i>
                            </div>
                        </div>`;
            },
        },
        vendor_invoice_no: {
            title: 'vendorinvoiceno',
            render: (data, row) => {
                const vendor_invoice_no = row.vendor_invoice_no;
                return `<span class="flex items-center grow1 gap-2.5">
                            <span class="flex flex-col gap-0.5">
                                <a class="text-primary btn-link1 text-sm1" style="border-bottom:0;" href="` + (row.menus.approval ? ("/VendorInvoice/Approval?refid=" + row.vendor_invoice_uuid + "") : ("/VendorInvoice/Create?refid=" + row.vendor_invoice_uuid + "")) + `">
                                    `+ vendor_invoice_no + `
                                </a>
                                <span data-tooltip="#advanced_Agency_` + row.vendor_invoice_uuid + `" class="text-xs text-gray-700 font-normal ellipsis">
                                    ` + row.agency_name + `
                                </span>
                            </span>
                        </span>` +
                    `<div class="tooltip" id="advanced_Agency_` + row.vendor_invoice_uuid + `">
                            <div class="flex  items-center gap-1">` + row.agency_name + `<i class="ki-solid ki-information-5 text-lg text-warning">
                            </i>
                            </div>
                        </div>`;
                //    return '<a class="text-primary btn-link" href="/VendorInvoice/Create?refid=' + row.vendor_invoice_uuid + '">' +
                //        vendor_invoice_no + '</a> ';
            },
            createdCell(cell) {
                cell.classList.add('text-center1');
            },
        },

        invoice_date_format: {
            title: 'invoicedate',
            render: (data, row) => {
                return `<div class="flex items-center gap-2.5">
                      <div class="flex flex-col gap-0.5">
                        <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                           ` + row.invoice_date_format + `
                        </span>
                        <span class="text-center1 text-2sm text-gray-600 font-normal">
                           ` + row.timeago + `
                       </span>
                     </div>
                </div>`;
            },
            createdCell(cell) {
                cell.classList.add('nowrap');
            },
        },
        invoice_amount: {
            title: 'invoice_amount',
            render: (data, row) => {
                var output = "";
                output = `<div class="flex flex-col gap-0.5">
                        <span class="font-medium text-md text-gray-900">
                            <span class="text-xs text-gray-600">`
                    + row.currency +
                    `</span> ` + row.invoice_amount_formatted + `
                        </span>`;

                if (row.invoice_amount - row.paidamount > 0) {
                    output += `<span class="text-xs text-gray-700 font-normal">
                                (Balance : ` + row.remaining_amount_formatted + `)
                            </span>`;
                }
                output += `</div>`;
                return output;
            },
            createdCell(cell) {
                cell.classList.add('text-end');
                cell.classList.add('nowrap');
            },
        },
        due_date_format: {
            title: 'duedate',
            render: (data, row) => {
                if (row.due_date_format == "")
                    return "";
                else {
                    return `<div class="flex items-center gap-2.5">
                          <div class="flex flex-col gap-0.5">
                             <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                               ` + row.due_date_format + `
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
            },
        },
        po_code: {
            title: 'po_code',
            render: (data, row) => {
                var result = "";
                if (row.po_code != "" || row.jobid != "") {
                    result = `<td class="nowrap">
                            <div class="flex items-center gap-2.5">
                                <div class="flex flex-col gap-1">
                                    <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                        ` + (row.po_code == "" ? "N/A" : row.po_code) + `
                                    </span>`;
                    if (row.jobid != "") {
                        result += `<span class="text-xs font-normal text-gray-700">
                                        Job ID : ` + row.jobid + `
                                    </span>`;
                    }

                    result += `</div>
                            </div>
                        </td>`;
                }
                return result;
            },
            //createdCell(cell) {
            //    cell.classList.add('text-center');
            //},
        },
        //approved_name1: {
        //    title: 'approved1name',
        //},
        //approved_name2: {
        //    title: 'approved2name',
        //},
        status_desc_style: {
            title: 'status_desc_style',
            render: (data, row) => {
                var statusresult = '';
                return `<span class="flex items-center justify-center grow1 gap-2.5">
                            <span class="flex flex-col gap-1.5">
                                `+ row.status_desc_style + `
                                ` + ((row.status == 2 && row.approved_name1 != "") || (row.status == 3 && row.approved_name2 != "") ? `<span data-tooltip="#advanced_AppName_` + row.vendor_invoice_uuid + `" class="text-2xs text-danger font-semibold ellipsis-sm">
                                    ` + (row.status == 2 ? row.approved_name1 : row.approved_name2) + `
                                </span>`: ``) + `
                                
                            </span>
                        </span>` +
                    ((row.status == 2 && row.approved_name1 != "") || (row.status == 3 && row.approved_name2 != "") ? `<div class="tooltip" id="advanced_AppName_` + row.vendor_invoice_uuid + `">
                            <div class="flex  items-center gap-1">` + (row.status == 2 ? row.approved_name1 : row.approved_name2) + `<i class="ki-solid ki-information-5 text-lg text-warning">
                            </i>
                            </div>
                        </div>`: ``);

                //    return '<span>' + row.status_desc_style + '</span>';
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
			                <a class="menu-link" href="/VendorInvoice/Approval?refid=`+ row.vendor_invoice_uuid + `">
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
                    if (row.menus.payable) {
                        str += `<div class="menu-item">
			                <a class="menu-link" href="javascript:void(0);" onclick="RecordPayment('`+ row.vendor_invoice_uuid + `');">
			                    <span class="menu-icon">
				                <i class="ki-filled ki-dollar">
				                </i>
			                    </span>
			                    <span class="menu-title text-start">
				                Record Payment
			                    </span>
			                </a>
		                    </div>`;
                    }
                    if (row.menus.delete) {
                        str += `<div class="menu-item">
                                <a class="menu-link" href="javascript:void(0);" onclick="DeleteVendorInvoice('` + row.vendor_invoice_uuid + `');">
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

function RecordPayment(invoiceuuid) {
    blockui();
    $.ajax({
        url: '/VendorInvoice/GetRecordPayment?invoiceuuid=' + invoiceuuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalVIRecordPayment = document.querySelector('#ModalVIRecordPayment');
            const modal = KTModal.getInstance(ModalVIRecordPayment);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}


function SaveVendorPayment() {
    var validated = $("#formVIRecordPayment").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formVIRecordPayment").serializeArray();
        $.ajax({
            url: '/VendorInvoice/SaveRecordPayment',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Updated successfully!");
                    const ModalVIRecordPayment = document.querySelector('#ModalVIRecordPayment');
                    const modal = KTModal.getInstance(ModalVIRecordPayment);
                    modal.hide();

                    setTimeout(
                        function () {
                            window.location.href = window.location;
                        }, 400);

                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);

                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot update payment");
            }
        });
    }
}

function DeleteVendorInvoice(invoiceuuid) {
    Swal.fire({
        title: "Are you sure to delete?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            blockui();
            $.ajax({
                url: '/VendorInvoice/DeleteVendorInvoice?invoiceuuid=' + invoiceuuid,
                type: 'GET',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("Deleted Successfully!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Delete Vendor Invoice");
                }
            });
        }
    });
}


function BulkApprove() {
    var checkedlength = $("#TableVendorInvoiceList").find('.checksingle:checked').length;
    if (checkedlength == 0) {
        ErrorMessage("Please select atleast one invoice to approve");
        return;
    }
    var invoiceList = $("#TableVendorInvoiceList")
        .find("tbody input[type='checkbox']:checked")
        .map(function () {
            return $(this).val(); // or this.value
        })
        .get().join(',');

    CreateBulkApprovalReject(invoiceList, true);
}

function BulkReject() {
    var checkedlength = $("#TableVendorInvoiceList").find('.checksingle:checked').length;
    if (checkedlength == 0) {
        ErrorMessage("Please select atleast one invoice to reject");
        return;
    }
    var invoiceList = $("#TableVendorInvoiceList")
        .find("tbody input[type='checkbox']:checked")
        .map(function () {
            return $(this).val(); // or this.value
        })
        .get().join(',');

    CreateBulkApprovalReject(invoiceList, false);
}

function CreateBulkApprovalReject(invoicelist, approve_reject) {
    blockui();
    var invoiceList = invoicelist.split(",");  // Replace with actual UUIDs
    $.ajax({
        url: '/VendorInvoice/BulkApprovalReject',
        data: {
            invoiceList: invoiceList,
            approve: approve_reject
        },
        type: 'POST',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 0) {
                ErrorMessage(data.ResultMessage);
            }
            else {
                $("#modalsection").html(data);
                OpenModal("ModalVInvoiceSendBulkApproval");
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Create Comparison");
        }
    });

}

function BulkApproveVI(invoiceuuid) {
    var data = {
        "invoiceuuid": invoiceuuid,
        "isaccept": true,
        "comments": $("#ModalVInvoiceSendBulkApproval").find("#txt_Comments").val(),
        "invoiceid": ""
    };
    BulkApproveRejectVI(data);
}

function BulkRejectVI(invoiceuuid) {
    if ($("#formBulkApprovalReject").Validate()) {
        var data = {
            "invoiceuuid": invoiceuuid,
            "isaccept": false,
            "comments": $("#ModalVInvoiceSendBulkApproval").find("#txt_Comments").val(),
            "invoiceid": ""
        };
        BulkApproveRejectVI(data);
    }
}

function BulkApproveRejectVI(data) {
    blockui();
    let formdata = JSON.stringify(data);
    $.ajax({
        url: '/VendorInvoice/BulkApproveReject',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                SuccessMessage("Updated Successfully!");
                setTimeout(
                    function () {
                        window.location = window.location.href;
                    }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Update Vendor Invoice");
        }
    });
}
