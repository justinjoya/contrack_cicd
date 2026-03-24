function OpenSmtpConfigModal(refid) {
    blockui();
    $.ajax({
        url: '/SmtpConfig/GetSmtpConfigModal?refid=' + refid,
        type: 'GET',
        success: function (data) {
            unblockui();
            // Load the returned partial view into modal section
            $("#modalsection").html(data).find(".select2").select2();
            // Initialize modal         
            OpenModal("ModalAddSmtpConfig");
        },

        error: function () {
            unblockui();
            ErrorMessage("Error in Getting SMTP Configuration");
        }
    });
}

function ShowHideEntity(type) {
    $(".entity").hide();
    $("#Entiry_" + type).show();
}

function SaveSmtpConfig() {
    var validated = $("#formSmtpConfig").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formSmtpConfig").serializeArray();
        $.ajax({
            url: '/SmtpConfig/SaveSmtpConfig',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("SMTP Configuration updated successfully!");
                    CloseModal("ModalAddSmtpConfig");

                    setTimeout(
                        function () {
                            window.location.href = window.location;
                        }, 500);
                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);

                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save SMTP Configuration!");
            }
        });
    }
}

function DeleteSmtpConfig(refid) {

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
                url: '/SmtpConfig/DeleteSmtpConfig',
                type: 'POST',
                data: 'smtpconfig.smtp_id.EncryptedValue=' + refid,
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("SMTP Configuration Deleted Successfully!");
                        setTimeout(
                            function () {
                                window.location.href = window.location;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot Delete SMTP Configuration");
                }
            });
        }
    });
}

const apiUrl = domain + '/TableDisplay/SmtpConfigurations';
const element = document.querySelector('#TableSmtpConfigurations');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 10,
    columns: {
        smtp_host: {
            title: 'smtp_host',
            render: (data, row) => {
                const badge = row.smtpconfig.is_default
                    ? " <span class='mx-1 py-1 badge badge-outline badge-primary badge-sm '>Default</span>"
                    : "";
                return '' + row.smtpconfig.smtp_host + '' + badge;
            }
        },
        smtp_username: {
            title: 'SMTP Username',
            render: (data, row) => {
                return '' + row.smtpconfig.smtp_username + ''
            }
        },
        smtp_port: {
            title: 'SMTP Port',
            render: (data, row) => {
                return '' + row.smtpconfig.smtp_port + ''                   
            }
        },
        agency: {
            title: 'created_for',
            render: (data, row) => {
                if (row.smtpconfig.agency.agencyname != "") {
                    return '<div class="flex items-center gap-4">' +
                        '<div class="leading-none w-9 shrink-0">' +
                        '<img src="/assets/icons/agency.svg"/>' +
                        '</i>' +
                        '</div>' +
                        '<div class="flex flex-col gap-0.5">' +
                        '<span class="leading-none font-medium text-sm text-gray-900">' +
                        'Agency' +
                        '</span>' +
                        '<span class="text-2sm text-gray-700 font-normal">' +
                        '' + row.smtpconfig.agency.agencyname + '' +
                        '</span>' +
                        '</div>' +
                        '</div>';
                }
                else {
                    return '<div class="flex items-center gap-4">' +
                        '<div class="leading-none w-9 shrink-0">' +
                        '<img src="/assets/icons/hub.svg"/>' +
                        '</i>' +
                        '</div>' +
                        '<div class="flex flex-col gap-0.5">' +
                        '<span class="leading-none font-medium text-sm text-gray-900">' +
                        'Hub' +
                        '</span>' +
                        '<span class="text-2sm text-gray-700 font-normal">' +
                        '' + row.smtpconfig.HubName + '' +
                        '</span>' +
                        '</div>' +
                        '</div>';
                }
            }
        },
        created_at: {
            title: 'created_at',
            render: (data, row) => {
                let jsonDate = row.smtpconfig.created_at; // .NET JSON format
                let timestamp = parseInt(jsonDate.match(/\d+/)[0]); // Extract the number
                let jsDate = new Date(timestamp);
                return formatDateOnly(jsDate);
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
        smtpid: {
            title: 'Actions',
            render: function (data, row) {
                return row.menus.edit ? '<a onclick="OpenSmtpConfigModal(\'' + row.smtpconfig.smtp_id.EncryptedValue + '\');" class="btn btn-sm btn-icon btn-clear btn-light">' +
                    '<i class="ki-filled ki-notepad-edit"></i>' +
                    '</a>' +
                    '<span class="text-gray-300">|</span>' +
                    '<a onclick="DeleteSmtpConfig(\'' + row.smtpconfig.smtp_id.EncryptedValue + '\');" class="btn btn-sm btn-icon btn-clear btn-light">' +
                    '<i class="ki-filled ki-trash"></i>' +
                    '</a>' : "-";
            },
            createdCell: function (cell) {
                cell.classList.add('text-center', 'nowrap');
            }
        },
    },
};

const dataTable = new KTDataTable(element, dataTableOptions);