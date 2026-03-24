var elementUser;
if (viewpage == "AgencyClients") {
    elementUser = document.querySelector('#TableClientsAgency');
}
else {
    elementUser = document.querySelector('#TableClients');
}

const dataTableOptionsUser = {
    apiEndpoint: domain + '/TableDisplay/Clients',
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 10,
    /*stateSave: false,*/
    columns: {
        clientname: {
            title: 'clientname',
            render: (data, row) => {
                var clientname = row.client.clientname;
                //if (clientname.length > 30) {
                //    clientname = clientname.substring(0, 30) + "...";
                //}
                return `<div class='flex gap-3 items-center'>
                      <span class="colorbox" style="color:` + row.client.extras.clientcolor + `;background-color:` + row.client.extras.clientbgcolor + `">` + row.client.extras.clientshortcode + `</span>
                      <div class="flex items-center gap-4">
                        <div class="flex flex-col gap-0.5">
                          <span class="leading-none font-medium text-sm text-gray-900">
                            <a class="text-primary" href="/Client/Details?refid=${row.client.clientuuid}">
                              ${clientname}
                            </a>
                          </span>
                          <span class="text-2sm text-gray-700 font-normal">
                            ${row.client.imono}
                            <!-- <span>•</span> ${row.Phone} -->
                          </span>
                        </div>
                      </div>
                      </div>
                    `;
            },

        },
        email: {
            title: 'Email',
            render: (data, row) => {
                const email = row.client.email;
                if (!email) return "";
                const emailList = email.split(";");
                if (emailList.length > 1) {
                    const emailcorrected = emailList.slice(0, 1).join(";") + ` <span class='text-primary'>+${emailList.length - 1}</span>`;
                    const tooltipId = 'tooltip_' + row.client.clientuuid;
                    return `<span data-tooltip="#${tooltipId}">${emailcorrected}</span>
                                <div class="tooltip" id="${tooltipId}">
                                    <div class="flex items-center gap-1">${email}<i class="ki-solid ki-information-5 text-lg text-warning"></i></div>
                                </div>`;
                }
                return email;
            },
        },

        picname: {
            title: 'picname',
            render: (data, row) => {
                var clientname = row.client.clientname;
                //if (clientname.length > 30) {
                //    clientname = clientname.substring(0, 30) + "...";
                //}
                if (row.client.picname == "")
                    return "";
                else
                    return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                ` + row.client.picname + `
                                      </span>
                            <span class="text-2sm text-gray-700 font-normal">
                                ` + row.client.picdesignation + `
                            </span>
                        </div>
                    </div>`;
            },
        },
        cc: {
            title: 'fullname1',
            render: (data, row) => {
                if (row.client.agency.agencyid.NumericValue > 0) {
                    return '<div class="flex items-center gap-4">' +
                        '<div class="leading-none w-9 shrink-0">' +
                        '<img src="/assets/icons/agency.svg"/>' +
                        '</div>' +
                        '<div class="flex flex-col gap-0.5">' +
                        '<span class="leading-none font-medium text-sm text-gray-900">' +
                        'Agency' +
                        '</span>' +
                        '<span class="text-2sm text-gray-700 font-normal">' +
                        '' + row.client.agency.agencyname + '' +
                        '</span>' +
                        '</div>' +
                        '</div>';
                }
                else {
                    return '<div class="flex items-center gap-4">' +
                        '<div class="leading-none w-9 shrink-0">' +
                        '<img src="/assets/icons/hub.svg"/>' +
                        '</div>' +
                        '<div class="flex flex-col gap-0.5">' +
                        '<span class="leading-none font-medium text-sm text-gray-900">' +
                        'Hub' +
                        '</span>' +
                        '<span class="text-2sm text-gray-700 font-normal">' +
                        '' + row.client.agency.agencyname + '' +
                        '</span>' +
                        '</div>' +
                        '</div>';
                }

            }
        },
        hcreatedat: {
            title: 'hcreatedat',
            render: (data, row) => {
                let jsonDate = row.client.hcreatedat; // .NET JSON format
                let timestamp = parseInt(jsonDate.match(/\d+/)[0]); // Extract the number
                let jsDate = new Date(timestamp);

                //console.log(jsDate); // Correct Date object
                //return jsDate.toLocaleString();
                return formatDateOnly(jsDate);
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
        //uuid: {
        //    title: 'Status',
        //    render: (data, row) => {
        //        return row.menus.edit ? '<a onclick="OpenClientModal(\'' + row.client.clientuuid + '\');" class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">' +
        //            '<i class="ki-filled ki-notepad-edit"></i></a> <span class="text-gray-300">|</span> ' +
        //            '<a onclick="DeleteClient(\'' + row.client.clientid + '\',\'' + row.client.clientuuid + '\');" class="btn btn-sm btn-icon btn-clear btn-light " href="javascript:void(0);">' +
        //            '<i class="ki-filled ki-trash"></i></a>' : "-";
        //    },
        //    createdCell(cell) {
        //        cell.classList.add('text-center');
        //        cell.classList.add('nowrap');
        //    },
        //},
        uuid: {
            title: 'Actions',
            render: (data, row) => {
                const client = row.client;
                const numericId = client.clientid?.NumericValue || 0;
                const encryptedId = client.clientid?.EncryptedValue || "";
                const clientuuid = client.clientuuid;
                const canEdit = row.menus?.edit;

                if (!canEdit) return "-";

                return `
            <a onclick="OpenClientModal('${clientuuid}');" 
               class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">
                <i class="ki-filled ki-notepad-edit"></i>
            </a>
            <span class="text-gray-300">|</span>
            <a onclick="DeleteClient('${encryptedId}', '${clientuuid}');" 
               class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">
                <i class="ki-filled ki-trash"></i>
            </a>`;
            },
            createdCell(cell) {
                cell.classList.add('text-center', 'nowrap');
            },
        },
    },
    /*order: [[0, 'desc']],*/
};

const dataTableUser = new KTDataTable(elementUser, dataTableOptionsUser);

function OpenClientModal(refid, agencyid) {
    blockui();
    $.ajax({
        url: '/Client/GetClientModal?refid=' + refid + '&agencyid=' + agencyid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalAddClient");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Client")
        }
    });
}


function AddClientEMail() {
    var email = $("#txt_emailtemp").val().trim();
    var existingemail = $("#hdn_Client_Email").val();
    if (email != "") {
        if (existingemail == "") {
            existingemail = email;
        }
        else {
            existingemail = existingemail + ";" + email;
        }
        $("#hdn_Client_Email").val(existingemail);
        var emailsplit = [];
        if (email.indexOf(",") >= 0) {
            emailsplit = email.split(',');
        }
        else {
            emailsplit = email.split(';');
        }
        for (var i = 0; i < emailsplit.length; i++) {
            var innerhtml = `<tr>
            <td class="py-2 text-gray-800 font-normal text-sm emailtext">
                ${emailsplit[i]}
            </td>
            <td class="py-2 text-center" style="padding-right:0;">
                <a class="btn btn-sm btn-icon btn-clear btn-danger" href="javascript:void(0);" onclick="RemoveClientEMail(this);">
                    <i class="ki-filled ki-trash"></i>
                </a>
            </td></tr>`;
            $("#tbody_Client_Email").append(innerhtml);
        }
        $("#txt_emailtemp").val("");
        $("#emaillist_client").show();
    }
}

function RemoveClientEMail(current) {
    var emailtext = $(current).closest("tr").find(".emailtext").text();
    var existingemail = $("#hdn_Client_Email").val();
    existingemail = existingemail
        .split(";")
        .filter(email => email.trim() !== emailtext.trim())
        .join(";");

    $("#hdn_Client_Email").val(existingemail);
    $(current).closest("tr").remove();
    if (existingemail == "") {
        $("#emaillist_client").hide();
    }
    else {
        $("#emaillist_client").show();
    }

}

function AddAccountsClientEMail() {
    var email = $("#txt_accountsemailtemp").val().trim();
    var existingemail = $("#hdn_Account_Client_Email").val();
    if (email != "") {
        if (existingemail == "") {
            existingemail = email;
        }
        else {
            existingemail = existingemail + ";" + email;
        }
        $("#hdn_Account_Client_Email").val(existingemail);

        var emailsplit = [];
        if (email.indexOf(",") >= 0) {
            emailsplit = email.split(',');
        }
        else {
            emailsplit = email.split(';');
        }
        for (var i = 0; i < emailsplit.length; i++) {
            var innerhtml = `<tr>
        <td class="py-2 text-gray-800 font-normal text-sm emailtext">
            ${emailsplit[i]}
        </td>
        <td class="py-2 text-center" style="padding-right:0;">
            <a class="btn btn-sm btn-icon btn-clear btn-danger" href="javascript:void(0);" onclick="RemoveAccClientEMail(this);">
                <i class="ki-filled ki-trash"></i>
            </a>
        </td></tr>`;
        }
        $("#tbody_acc_Client_Email").append(innerhtml);
        $("#txt_accountsemailtemp").val("");
        $("#accounts_emaillist_client").show();
    }
}

function RemoveAccClientEMail(current) {
    var emailtext = $(current).closest("tr").find(".emailtext").text();
    var existingemail = $("#hdn_Account_Client_Email").val();
    existingemail = existingemail
        .split(";")
        .filter(email => email.trim() !== emailtext.trim())
        .join(";");

    $("#hdn_Account_Client_Email").val(existingemail);
    $(current).closest("tr").remove();
    if (existingemail == "") {
        $("#accounts_emaillist_client").hide();
    }
    else {
        $("#accounts_emaillist_client").show();
    }

}



function SaveClient() {
    var validated = $("#formClient").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formClient").serializeArray();
        $.ajax({
            url: '/Client/SaveClient',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Client updated successfully!");
                    CloseModal("ModalAddClient");
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
                ErrorMessage("Cannot Save client!");
            }
        });
    }
}



function OpenClientCustomAttribute(refid) {
    blockui();
    $.ajax({
        url: '/Client/OpenClientKeyValuePair?refid=' + refid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            OpenModal("ModalCustomKeyValue");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Client")
        }
    });
}


function SaveCustomAttribute() {
    var validated = $("#formCustomKeyValue").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formCustomKeyValue").serializeArray();
        $.ajax({
            url: '/Client/UpdateClientCustomAttribute',
            data: formdata,
            type: 'POST',
            /*dataType: "json",*/
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_client_customattribute").html(data);
                SuccessMessage("Updated successfully!");
                CloseModal("ModalCustomKeyValue");
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Client!");
            }
        });
    }
}

function DeleteClientCustomAttribute(refid) {
    ShowErrorConfirmation({
        title: 'Delete Custom attribute?',
        message: `Are you sure you want to delete the custom attribute?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Client/DeleteClientCustomAttribute?refid=' + refid,
                type: 'GET',
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_client_customattribute").html(data);
                    SuccessMessage("Updated successfully!");
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Error in Deleting Custom attribute")
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}


function OpenClientBankAccount(refid) {
    blockui();
    $.ajax({
        url: '/Client/OpenClientBankAccount?refid=' + refid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalBankAccount");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Bank Account")
        }
    });
}


function SaveBankAccount() {
    var validated = $("#formBankAccount").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formBankAccount").serializeArray();
        $.ajax({
            url: '/Client/UpdateClientBankAccount',
            data: formdata,
            type: 'POST',
            /*dataType: "json",*/
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_client_bankaccount").html(data);
                SuccessMessage("Updated successfully!");
                CloseModal("ModalBankAccount");
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Ban account!");
            }
        });
    }
}
function ClearBankAccount(current) {
    $(current).closest("div").find("input[type='text']").val("");
    $(current).closest("div").find("textarea").val("");
}


function DeleteClientBankAccount(refid) {
    ShowErrorConfirmation({
        title: 'Delete Bank Account?',
        message: `Are you sure you want to delete this bank account?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Client/DeleteClientBankAccount?refid=' + refid,
                type: 'GET',
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_client_bankaccount").html(data);
                    SuccessMessage("Updated successfully!");
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Error in Deleting bank")
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}



function OpenClientAddressModal(refid, clientid) {
    blockui();
    $.ajax({
        url: '/Client/GetAddressModal?refid=' + refid + '&clientid=' + clientid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalClientAddress");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Address")
        }
    });
}


function SaveAddress() {
    var validated = $("#formClientAddress").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formClientAddress").serializeArray();
        $.ajax({
            url: '/Client/SaveAddress',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Address updated successfully!");
                    CloseModal("ModalClientAddress");
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
                ErrorMessage("Cannot Save Address!");
            }
        });
    }
}

function MakeAddressPrimary(refid, clientid) {

    blockui();
    $.ajax({
        url: '/Client/MakeAddressPrimary?refid=' + refid + '&clientid=' + clientid,
        type: 'GET',
        /*dataType: "json",*/
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#section_client_Address").html(data);
            SuccessMessage("Updated successfully!");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Save Address!");
        }
    });

}

function DeleteAddress(refid, clientid) {
    ShowErrorConfirmation({
        title: 'Delete Address?',
        message: `Are you sure you want to delete this address?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Client/DeleteAddress?refid=' + refid + '&clientid=' + clientid,
                type: 'GET',
                /*dataType: "json",*/
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_client_Address").html(data);
                    SuccessMessage("Deleted successfully!");
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Delete Address!");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}

//function DeleteClient(client_id, ref_id) {

//    Swal.fire({
//        title: "Are you sure?",
//        text: "You won't be able to revert this!",
//        icon: "warning",
//        showCancelButton: true,
//        confirmButtonColor: "#3085d6",
//        cancelButtonColor: "#d33",
//        confirmButtonText: "Yes, delete it!"
//    }).then((result) => {
//        if (result.isConfirmed) {
//            blockui();
//            $.ajax({
//                url: '/Client/DeleteClient',
//                type: 'POST',
//                data: 'clientid=' + client_id + '&clientuuid=' + ref_id,
//                success: function (data) {
//                    unblockui();
//                    if (data.ResultId == "1") {
//                        SuccessMessage("Client deleted successfully!");
//                        window.location.href = '/Client/List';
//                    }
//                    else {
//                        ErrorMessage(data.ResultMessage);
//                    }
//                },
//                error: function () {
//                    unblockui();
//                    ErrorMessage("Cannot delete client");
//                }
//            });
//        }
//    });
//}

function DeleteClient(client_id_encrypted, client_uuid) {
    ShowErrorConfirmation({
        title: 'Delete Client?',
        message: `Are you sure you want to delete this client?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Client/DeleteClient',
                type: 'POST',
                data: {
                    clientid: client_id_encrypted,
                    clientuuid: client_uuid
                },
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Client deleted successfully!");
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                    setTimeout(function () {
                        window.location.href = window.location;
                    }, 400);
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot delete client");
                    setTimeout(function () {
                        window.location.href = window.location;
                    }, 400);
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });

}

function ShowHideEntity(type) {
    $(".entity").hide();
    $("#Entiry_" + type).show();
}