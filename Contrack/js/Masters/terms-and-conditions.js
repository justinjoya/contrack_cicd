function ShowHideEntity(type) {
    $(".entity").hide();
    $("#Entiry_" + type).show();
}



$(function () {
    $('#frmTermsAndConditions').on('submit', function (e) {
        e.preventDefault();
        var validated = $("#frmTermsAndConditions").Validate();
        if (validated) {
            blockui();
            var formdata = $("#frmTermsAndConditions").serializeArray();
            $.ajax({
                url: '/TermsandConditions/SaveTermsandConditions',
                data: formdata,
                type: 'POST',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Terms and Conditions Saved Successfully!");
                        setTimeout(
                            function () {
                                window.location.href = window.location;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Terms and Conditions");
                }
            });
        }
        else {
            ErrorMessage("Some of required terms needs to be filled");
        }
    });
});

function DeleteTermsAndConditions(refid, userid) {

    Swal.fire({
        title: "Are you sure?",
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
                url: '/TermsandConditions/DeleteTermsandConditions',
                type: 'POST',
                data: 'TermUuid=' + refid + '&DeletedBy=' + userid,
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Terms and Conditions Deleted Successfully!");
                        setTimeout(
                            function () {
                                window.location.href = '/TermsandConditions/List';
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot Delete Terms and Conditions");
                }
            });
        }
    });
}

const apiUrl = domain + '/TableDisplay/TermsAndConditions';
const element = document.querySelector('#TableTerms');

const typeLabels = {
    RFQ: "RFQ T&C",
    Quotation: "Quotation T&C",
    PO: "PO T&C",
    Invoice: "PO Invoicing Instructions"
};

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 10,
    columns: {
        Type: {
            title: 'type',
            render: (data, row) => {
                const displayText = typeLabels[row.termsandConditions.Type] || row.termsandConditions.Type;
                return '<a class="text-primary" href="/TermsandConditions/CreateTermsAndConditions?refid=' + row.termsandConditions.TermUuid + '">' +
                    displayText + '</a> ';
            },
        },
        agency: {
            title: 'created_for',
            render: (data, row) => {
                if (row.termsandConditions.agency.agencyname != "") {
                    return '<div class="flex items-center gap-4">' +
                        '<div class="leading-none w-5 shrink-0">' +
                        '<i class="ki-filled ki-users text-warning text-2xl">' +
                        '</i>' +
                        '</div>' +
                        '<div class="flex flex-col gap-0.5">' +
                        '<span class="leading-none font-medium text-sm text-gray-900">' +
                        'Agency' +
                        '</span>' +
                        '<span class="text-2sm text-gray-700 font-normal">' +
                        '' + row.termsandConditions.agency.agencyname + '' +
                        '</span>' +
                        '</div>' +
                        '</div>';
                }
                else {
                    return '<div class="flex items-center gap-4">' +
                        '<div class="leading-none w-5 shrink-0">' +
                        '<i class="ki-filled ki-people text-primary text-2xl">' +
                        '</i>' +
                        '</div>' +
                        '<div class="flex flex-col gap-0.5">' +
                        '<span class="leading-none font-medium text-sm text-gray-900">' +
                        'Hub' +
                        '</span>' +
                        '<span class="text-2sm text-gray-700 font-normal">' +
                        '' + row.termsandConditions.HubName + '' +
                        '</span>' +
                        '</div>' +
                        '</div>';
                }
            }
        },
        UserName: {
            title: 'created_by',
            render: (data, row) => {
                return '' + row.termsandConditions.UserName + ''
            }
        },




        CreatedAt: {
            title: 'created_at',
            render: (data, row) => {
                let jsonDate = row.termsandConditions.CreatedAt; // .NET JSON format
                let timestamp = parseInt(jsonDate.match(/\d+/)[0]); // Extract the number
                let jsDate = new Date(timestamp);
                return formatDateOnly(jsDate);
            },

            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
        TermUuid: {
            title: 'Status',
            render: (data, row) => {
                return row.menu.edit ? '<a href="/TermsandConditions/CreateTermsandConditions?refid=' + row.termsandConditions.TermUuid + '" class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">' +
                    '<i class="ki-filled ki-notepad-edit"></i></a> <span class="text-gray-300">|</span> ' +
                    '<a onclick="DeleteTermsAndConditions(\'' + row.termsandConditions.TermUuid + '\',\'' + row.termsandConditions.CreatedBy + '\');" class="btn btn-sm btn-icon btn-clear btn-light " href="javascript:void(0);">' +
                    '<i class="ki-filled ki-trash"></i></a>' : "-";
            },
            createdCell(cell) {
                cell.classList.add('text-center');
                cell.classList.add('nowrap');
            },
        },
    },
};

const dataTable = new KTDataTable(element, dataTableOptions);

