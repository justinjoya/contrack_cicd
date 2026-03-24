var elementUser;
if (viewpage == "AgencyVendors") {
    elementUser = document.querySelector('#TableVendorsAgency');
}
else {
    elementUser = document.querySelector('#TableVendors');
}

const dataTableOptionsUser = {
    apiEndpoint: domain + '/TableDisplay/Vendors',
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 10,
    stateSave: false,
    columns: {
        vendorname: {
            title: 'legalname',
            render: function (data, row) {
                var vendor = row.vendor || {};
                var extras = vendor.extras || { vendorcolor: '#000', vendorbgcolor: '#fff', vendorshortcode: '' };
                var vendorname = vendor.legalname || '';
                var vendorcode = vendor.vendorcode || '';

                return '<div class="flex gap-3 items-center">' +
                    '<span class="colorbox" style="color:' + extras.vendorcolor + ';background-color:' + extras.vendorbgcolor + '">' + extras.vendorshortcode + '</span>' +
                    '<div class="flex items-center gap-4">' +
                    '<div class="flex flex-col gap-0.5">' +
                    '<span class="leading-none font-medium text-sm text-gray-900">' +
                    '<a class="text-primary" href="/Vendor/Details?refid=' + vendor.vendoruuid + '">' +
                    vendorname +
                    '</a>' +
                    '</span>' +
                    '<span class="text-2sm text-gray-700 font-normal">' +
                    vendorcode +
                    '</span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }
        },
        email: {
            title: 'Email',
            render: (data, row) => {
                const vendor = row.vendor || {};
                const email = vendor.contactemail || "";
                const agencyId = vendor.agency?.agencyid?.NumericValue || "";

                if (!email) return "";

                const emailList = email.split(";").map(e => e.trim()).filter(e => e);

                if (emailList.length > 1) {
                    const emailcorrected = `${emailList[0]} <span class='text-primary'>+${emailList.length - 1}</span>`;
                    return `
                <span data-tooltip="#advanced_tooltip_${agencyId}">
                    ${emailcorrected}
                </span>
                <div class="tooltip" id="advanced_tooltip_${agencyId}">
                    <div class="flex items-center gap-1">
                        ${emailList.join("; ")}
                        <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                    </div>
                </div>
            `;
                } else {
                    return emailList[0];
                }
            },
        },
        phone: {
            title: 'Phone',
            render: (data, row) => {
                const vendor = row.vendor || {};
                const phone = vendor.phone || "";
                if (!phone) return "";
                return phone;
            }
        },
        picname: {
            title: 'picname',
            render: (data, row) => {
                var vendorname = row.vendor.legalname;


                if (row.vendor.picname == "")
                    return "";
                else
                    return `<div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="flex items-center gap-1.5 leading-none font-medium text-sm text-gray-900">
                                ` + row.vendor.picname + `
                                      </span>
                            <span class="text-2sm text-gray-700 font-normal">
                                ` + row.vendor.picdesignation + `
                            </span>
                        </div>
                    </div>`;
            },
        },
        cc: {
            title: 'fullname1',
            render: (data, row) => {
                if (row.vendor.agency.agencyid.NumericValue > 0) {
                    return '<div class="flex items-center gap-4">' +
                        '<div class="leading-none w-9 shrink-0">' +
                        '<img src="/assets/icons/agency.svg"/>' +
                        '</div>' +
                        '<div class="flex flex-col gap-0.5">' +
                        '<span class="leading-none font-medium text-sm text-gray-900">' +
                        'Agency' +
                        '</span>' +
                        '<span class="text-2sm text-gray-700 font-normal">' +
                        '' + row.vendor.agency.agencyname + '' +
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
                        '' + row.vendor.agency.agencyname + '' +
                        '</span>' +
                        '</div>' +
                        '</div>';
                }

            }
        },
        hcreatedat: {
            title: 'createdat',
            render: (data, row) => {
                const jsonDate = row.vendor?.createdat || ""; // safe fallback
                if (!jsonDate) return ""; // return empty if missing

                const match = jsonDate.match(/\d+/);
                if (!match) return ""; // return empty if regex fails

                const timestamp = parseInt(match[0]);
                const jsDate = new Date(timestamp);
                return formatDateOnly(jsDate);
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
        uuid: {
            title: 'Status',
            render: (data, row) => {

                const vendoruuid = row.vendor.vendoruuid || '';
                const agencyEnc = row.vendor.agency?.agencyid?.EncryptedValue || '';

                return row.menus.edit
                    ? `<a onclick="OpenVendorModal('${vendoruuid}', '${agencyEnc}');"
                  class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">
                    <i class="ki-filled ki-notepad-edit"></i>
               </a>
               <span class="text-gray-300">|</span>
               <a onclick="DeleteVendor('${row.vendor.vendorid.EncryptedValue}', '${vendoruuid}');"
                  class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">
                    <i class="ki-filled ki-trash"></i>
               </a>`
                    : "-";
            }
        }

    },

};

const dataTableUser = new KTDataTable(elementUser, dataTableOptionsUser);
function OpenVendorModal(refid, agencyid) {
    blockui();
    $.ajax({
        url: '/Vendor/GetVendorModal?refid=' + refid + '&agencyid=' + agencyid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();

            $("#modalsection").html(data).find(".select2").select2();
            const ModalAddVendor = document.querySelector('#ModalAddVendor');
            const modal = KTModal.getInstance(ModalAddVendor);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Vendor")
        }
    });
}
function AddVendorEMail() {
    var email = $("#txt_emailtemp").val().trim();
    var existingemail = $("#hdn_Vendor_Email").val();
    if (email != "") {
        if (existingemail == "") {
            existingemail = email;
        }
        else {
            existingemail = existingemail + ";" + email;
        }
        $("#hdn_Vendor_Email").val(existingemail);
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
                <a class="btn btn-sm btn-icon btn-clear btn-danger" href="javascript:void(0);" onclick="RemoveVendorEMail(this);">
                    <i class="ki-filled ki-trash"></i>
                </a>
            </td></tr>`;
            $("#tbody_Vendor_Email").append(innerhtml);
        }
        $("#txt_emailtemp").val("");
        $("#emaillist_Vendor").show();
    }
}
function RemoveVendorEMail(current) {
    var emailtext = $(current).closest("tr").find(".emailtext").text();
    var existingemail = $("#hdn_Vendor_Email").val();
    existingemail = existingemail
        .split(";")
        .filter(email => email.trim() !== emailtext.trim())
        .join(";");

    $("#hdn_Vendor_Email").val(existingemail);
    $(current).closest("tr").remove();
    if (existingemail == "") {
        $("#emaillist_Vendor").hide();
    }
    else {
        $("#emaillist_Vendor").show();
    }

}
function AddAccountsVendorEMail() {
    var email = $("#txt_accountsemailtemp").val().trim();
    var existingemail = $("#hdn_Account_Vendor_Email").val();
    if (email != "") {
        if (existingemail == "") {
            existingemail = email;
        }
        else {
            existingemail = existingemail + ";" + email;
        }
        $("#hdn_Account_Vendor_Email").val(existingemail);

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
            <a class="btn btn-sm btn-icon btn-clear btn-danger" href="javascript:void(0);" onclick="RemoveAccVendorEMail(this);">
                <i class="ki-filled ki-trash"></i>
            </a>
        </td></tr>`;
        }
        $("#tbody_acc_Vendor_Email").append(innerhtml);
        $("#txt_accountsemailtemp").val("");
        $("#accounts_emaillist_Vendor").show();
    }
}
function RemoveAccVendorEMail(current) {
    var emailtext = $(current).closest("tr").find(".emailtext").text();
    var existingemail = $("#hdn_Account_Vendor_Email").val();
    existingemail = existingemail
        .split(";")
        .filter(email => email.trim() !== emailtext.trim())
        .join(";");

    $("#hdn_Account_Vendor_Email").val(existingemail);
    $(current).closest("tr").remove();
    if (existingemail == "") {
        $("#accounts_emaillist_Vendor").hide();
    }
    else {
        $("#accounts_emaillist_Vendor").show();
    }

}
function SaveVendor() {
    AddVendorEMail();
    AddAccountsVendorEMail();
    var validated = $("#formVendor").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formVendor").serializeArray();
        $.ajax({
            url: '/Vendor/SaveVendor',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Vendor updated successfully!");
                    const ModalAddVendor = document.querySelector('#ModalAddVendor');
                    const modal = KTModal.getInstance(ModalAddVendor);
                    modal.hide();

                    setTimeout(
                        function () {
                            if (isUUID(data.ResultMessage)) {
                                window.location.href = "/Vendor/Vendor?refid=" + data.ResultMessage;
                            }
                            else {
                                window.location.href = window.location;
                            }
                        }, 400);

                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);

                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save vendor!");
            }
        });
    }
}
function ShowHideEntity(type) {
    $(".entity").hide();
    $("#Entiry_" + type).show();
}
function OpenVendorCustomAttribute(refid) {
    blockui();
    $.ajax({
        url: '/Vendor/OpenVendorKeyValuePair?refid=' + refid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalAddAgencyCustom = document.querySelector('#ModalCustomKeyValue');
            const modal = KTModal.getInstance(ModalAddAgencyCustom);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Vendor")
        }
    });
}
function SaveCustomAttribute() {
    var validated = $("#formCustomKeyValue").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formCustomKeyValue").serializeArray();
        $.ajax({
            url: '/Vendor/UpdateVendorCustomAttribute',
            data: formdata,
            type: 'POST',
            /*dataType: "json",*/
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_vendor_customattribute").html(data);
                SuccessMessage("Updated successfully!");
                const ModalCustomKeyValue = document.querySelector('#ModalCustomKeyValue');
                const modal = KTModal.getInstance(ModalCustomKeyValue);
                modal.hide();
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Vendor!");
            }
        });
    }
}
function DeleteVendorCustomAttribute(refid) {
    ShowErrorConfirmation({
        title: 'Delete Custom attribute?',
        message: `Are you sure you want to delete this Custom attribute?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Vendor/DeleteVendorCustomAttribute?refid=' + refid,
                type: 'GET',
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_vendor_customattribute").html(data);
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
function OpenVendorBankAccount(refid) {
    blockui();
    $.ajax({
        url: '/Vendor/OpenVendorBankAccount?refid=' + refid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalBankAccount = document.querySelector('#ModalBankAccount');
            const modal = KTModal.getInstance(ModalBankAccount);
            modal.show();
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
            url: '/Vendor/UpdateVendorBankAccount',
            data: formdata,
            type: 'POST',
            /*dataType: "json",*/
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_vendor_bankaccount").html(data);
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
function DeleteVendorBankAccount(refid) {
    ShowErrorConfirmation({
        title: 'Delete Bank Account?',
        message: `Are you sure you want to delete this Bank Account?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Vendor/DeleteVendorBankAccount?refid=' + refid,
                type: 'GET',
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_vendor_bankaccount").html(data);
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
function DeleteVendor(vendor_id_encrypted, ref_id) {
    ShowErrorConfirmation({
        title: 'Delete Bank Account?',
        message: `Are you sure you want to delete this Bank Account?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Vendor/DeleteVendor',
                type: 'POST',
                data: {
                    //vendorid: vendor_id_encrypted,
                    //vendoruuid: ref_id  
                    vendor: {
                        vendorid: {
                            EncryptedValue: vendor_id_encrypted
                        },
                        vendoruuid: ref_id,
                    },
                },
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Vendor deleted successfully!");
                        setTimeout(function () {
                            window.location.href = '/Vendor/List';
                        }, 400);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot delete vendor");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}
