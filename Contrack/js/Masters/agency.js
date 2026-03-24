const apiUrl = domain + '/TableDisplay/Agencies';
const element = document.querySelector('#TableAgencies');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 10,
    columns: {
        agencyname: {
            title: 'agencyname',
            render: (data, row) => {
                return '<a class="flex gap-3 items-center text-primary primary-col" href="/Agency/Details?refid=' + row.agency.uuid + '"><span class="colorbox" style="color:' + row.agency.extras.agencycolor + ';background-color:' + row.agency.extras.agencybgcolor + '">' + row.agency.extras.agencyshortcode + '</span>' +
                    row.agency.agencyname + '</a> ';
            },

        },
        email: {
            title: 'email',
            render: (data, row) => {
                var email = row.agency.email;
                if (!email) return '';

                var emailcorrected = "";
                var emailList = email.split(";");

                if (emailList.length > 1) {
                    emailcorrected = emailList.slice(0, 1).join(";") + " <span class='text-primary'>+" + (emailList.length - 1) + "</span>";

                    const tooltipId = 'advanced_tooltip_' + row.agency.uuid;

                    return `<span data-tooltip="#${tooltipId}">${emailcorrected}</span>` +
                        `<div class="tooltip" id="${tooltipId}">` +
                        '<div class="flex items-center gap-1">' +
                        email +
                        '<i class="ki-solid ki-information-5 text-lg text-warning"></i>' +
                        '</div>' +
                        '</div>';
                } else {
                    return email;
                }
            },
        },
        phone: {
            title: 'Phone',
            render: (data, row) => {
                return row.agency.phone  ;
            }
        },
        hcreatedat: {
            title: 'hcreatedat',
            render: (data, row) => {
                let jsonDate = row.agency.hcreatedat; // .NET JSON format
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
        uuid: {
            title: 'Status',
            render: (data, row) => {
                return row.menu.edit ? '<a onclick="OpenAgencyModal(\'' + row.agency.uuid + '\');" class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">' +
                    '<i class="ki-filled ki-notepad-edit"></i></a> <span class="text-gray-300">|</span> ' +
                    '<a onclick="DeleteAgency(\'' + row.agency.agencyid.EncryptedValue + '\',\'' + row.agency.uuid + '\');" class="btn btn-sm btn-icon btn-clear btn-light " href="javascript:void(0);">' +
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


function OpenAgencyModal(refid) {
    blockui();
    $.ajax({
        url: '/Agency/GetAgencyModal?refid=' + refid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalAddAgency")
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Company")
        }
    });
}


function DeleteAgency(agency_id, ref_id) {
    ShowErrorConfirmation({
        title: 'Delete Company?',
        message: `Are you sure you want to delete this company?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Agency/DeleteAgency',
                type: 'POST',
                data: {
                    agency: {
                        agencyid: {
                            EncryptedValue: agency_id
                        },
                        uuid: ref_id,
                    },
                },
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Company deleted successfully!");
                        setTimeout(
                            function () {
                                window.location.href = '/Agency/List';
                            }, 500);

                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot delete Company");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}



function SaveAgency() {
    var validated = $("#formAgency").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formAgency").serializeArray();
        $.ajax({
            url: '/Agency/SaveAgency',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Company updated successfully!");
                    CloseModal("ModalAddAgency");
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
                ErrorMessage("Cannot Save Company!");
            }
        });
    }
    else {
        ErrorMessage("Some of required items needs to be filled");
    }
}

function OpenAgencyCustomAttribute(refid) {
    blockui();
    $.ajax({
        url: '/Agency/OpenKeyValuePair?refid=' + refid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            OpenModal("ModalCustomKeyValue");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Company")
        }
    });
}


function SaveCustomAttribute() {
    var validated = $("#formCustomKeyValue").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formCustomKeyValue").serializeArray();
        $.ajax({
            url: '/Agency/UpdateCustomAttribute',
            data: formdata,
            type: 'POST',
            /*dataType: "json",*/
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_agency_customattribute").html(data);
                SuccessMessage("Updated successfully!");
                CloseModal("ModalCustomKeyValue");
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Company!");
            }
        });
    }
}

function DeleteAgencyCustomAttribute(refid) {
    ShowErrorConfirmation({
        title: 'Delete Custom attribute?',
        message: `Are you sure you want to delete the custom attribute?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Agency/DeleteAgencyCustomAttribute?refid=' + refid,
                type: 'GET',
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_agency_customattribute").html(data);
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


function OpenAgencyBankAccount(refid) {
    blockui();
    $.ajax({
        url: '/Agency/OpenBankAccount?refid=' + refid,
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
            url: '/Agency/UpdateBankAccount',
            data: formdata,
            type: 'POST',
            /*dataType: "json",*/
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_agency_bankaccount").html(data);
                CloseModal("ModalBankAccount");
                SuccessMessage("Updated successfully!");
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


function DeleteAgencyBankAccount(refid) {
    ShowErrorConfirmation({
        title: 'Delete Bank Account?',
        message: `Are you sure you want to delete this bank account?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Agency/DeleteBankAccount?refid=' + refid,
                type: 'GET',
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_agency_bankaccount").html(data);
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
function AddAgencyEMail() {
    var email = $("#txt_emailtemp").val().trim();
    var existingemail = $("#hdn_Agency_Email").val();
    if (email != "") {
        if (existingemail == "") {
            existingemail = email;
        }
        else {
            existingemail = existingemail + ";" + email;
        }
        $("#hdn_Agency_Email").val(existingemail);
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
                <a class="btn btn-sm btn-icon btn-clear btn-danger" href="javascript:void(0);" onclick="RemoveAgencyEMail(this);">
                    <i class="ki-filled ki-trash"></i>
                </a>
            </td></tr>`;
            $("#tbody_Agency_Email").append(innerhtml);
        }
        $("#txt_emailtemp").val("");
        $("#emaillist_agency").show();
    }
}

function RemoveAgencyEMail(current) {
    var emailtext = $(current).closest("tr").find(".emailtext").text();
    var existingemail = $("#hdn_Agency_Email").val();
    existingemail = existingemail
        .split(";")
        .filter(email => email.trim() !== emailtext.trim())
        .join(";");

    $("#hdn_Agency_Email").val(existingemail);
    $(current).closest("tr").remove();
    if (existingemail == "") {
        $("#emaillist_agency").hide();
    }
    else {
        $("#emaillist_agency").show();
    }

}

function AddAccountsAgencyEMail() {
    var email = $("#txt_accountsemailtemp").val().trim();
    var existingemail = $("#hdn_Account_Agency_Email").val();
    if (email != "") {
        if (existingemail == "") {
            existingemail = email;
        }
        else {
            existingemail = existingemail + ";" + email;
        }
        $("#hdn_Account_Agency_Email").val(existingemail);

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
            <a class="btn btn-sm btn-icon btn-clear btn-danger" href="javascript:void(0);" onclick="RemoveAccountAgencyEMail(this);">
                <i class="ki-filled ki-trash"></i>
            </a>
        </td></tr>`;
        }
        $("#tbody_acc_Agency_Email").append(innerhtml);
        $("#txt_accountsemailtemp").val("");
        $("#accounts_emaillist_agency").show();
    }
}

function RemoveAccountAgencyEMail(current) {
    var emailtext = $(current).closest("tr").find(".emailtext").text();
    var existingemail = $("#hdn_Account_Agency_Email").val();
    existingemail = existingemail
        .split(";")
        .filter(email => email.trim() !== emailtext.trim())
        .join(";");

    $("#hdn_Account_Agency_Email").val(existingemail);
    $(current).closest("tr").remove();
    if (existingemail == "") {
        $("#accounts_emaillist_agency").hide();
    }
    else {
        $("#accounts_emaillist_agency").show();
    }

}



function FilterPorts(current) {
    //var selectedCountry = $(current).val();
    //$('#ddlPortAgency option').closest('.dummy').show();

    ////$("#ddlPortAgency option[value='']").show(); // Keep the default option
    //for (var i = 0; i < selectedCountry.length; i++) {
    //    $("#ddlPortAgency option").each(function () {
    //        if ($(this).val().indexOf(selectedCountry[i] + "-") < 0) {
    //            //$(this).show(); // Show options where data-country contains the selectedCountry value
    //            //$(this).prop("disabled", false); // Disable matching options
    //            $(this).wrap('<span class=\'dummy\' style="display: none;"></span>');


    //        }
    //    });
    //}
    //$("#ddlPortAgency").select2();
}
