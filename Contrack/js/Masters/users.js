var elementUser;
if (viewpage == "AgencyUsers") {
    elementUser = document.querySelector('#TableUserLoginAgency');
}
else {
    elementUser = document.querySelector('#TableUserLogin');
}
const dataTableOptionsUser = {
    apiEndpoint: domain + '/TableDisplay/Users',
    pageSize: 10,
    columns: {
        Name: {
            title: 'Name',
            render: (data, row) => {
                return `<div class="flex items-center gap-3">
                            <span class="colorbox" style="color: ${row.user.extras.color}; background-color: ${row.user.extras.bgcolor};">${row.user.extras.shortcode}</span>
                            <div class="flex flex-col gap-0.5">
                                <span class="leading-none font-medium text-sm text-gray-900">${row.user.Name}</span>
                                <span class="text-2sm text-gray-700 font-normal">${row.user.Email}</span>
                            </div>
                        </div>`;
            },
        },
        UserName: {
            title: 'User Name',
            render: (data, row) => {
                const roleIcon = row.user.RoleIcon || '<i class="ki-filled ki-shield-slash"></i>';
                const roleName = row.user.RoleName || 'N/A';
                return `<div class="flex items-center gap-2">
                            <div class="leading-none w-5 shrink-0">
                                <span class="text-xl text-primary">${roleIcon}</span>
                            </div>
                            <div class="flex flex-col gap-1">
                                <span class="leading-none font-medium text-sm text-gray-900">${row.user.UserName}</span>
                                <span class="text-2sm text-gray-700 font-normal">${roleName}</span>
                            </div>
                        </div>`;
            },
        },
        Phone: {
            title: 'Phone',
            render: (data, row) => row.user.Phone || '-',
        },
        TypeName: {
            title: 'Created For',
            render: (data, row) => {
                const entityname = row.user.EntityName || '';
                const entityList = entityname.split(',');
                const entitynamecorrected = entityList.length > 1
                    ? `${entityList[0]} <span class='text-primary'>+${entityList.length - 1}</span>`
                    : entityname;

                const userType = row.user.Type.NumericValue;
                let htmlOutput = '';

                if (userType === 2) {
                    htmlOutput = `
                        <div class="flex items-center gap-4">
                            <div class="leading-none w-9 shrink-0">
                                <img src="/assets/icons/agency.svg"/>
                            </div>
                            <div class="flex flex-col gap-0.5">
                                <span class="leading-none font-medium text-sm text-gray-900">Agency</span>
                                <span class="text-2sm text-gray-700 font-normal" data-tooltip="#tooltip_${row.user.UserID.NumericValue}">${entitynamecorrected}</span>
                            </div>
                        </div>`;
                } else if (userType === 1) {
                    htmlOutput = `
                        <div class="flex items-center gap-4">
                            <div class="leading-none w-9 shrink-0">
                                <img src="/assets/icons/hub.svg"/>
                            </div>
                            <div class="flex flex-col gap-0.5">
                                <span class="leading-none font-medium text-sm text-gray-900">Hub</span>
                                <span class="text-2sm text-gray-700 font-normal" data-tooltip="#tooltip_${row.user.UserID.NumericValue}">${entitynamecorrected}</span>
                            </div>
                        </div>`;
                } else {
                    htmlOutput = `
                         <div class="flex items-center gap-4">
                            <div class="leading-none w-9 shrink-0">
                                <i class="ki-filled ki-user text-gray-500 text-2xl"></i>
                            </div>
                            <div class="flex flex-col gap-0.5">
                                <span class="leading-none font-medium text-sm text-gray-900">${row.user.TypeName}</span>
                                <span class="text-2sm text-gray-700 font-normal">${entitynamecorrected}</span>
                            </div>
                        </div>`;
                }

                return htmlOutput + `<div class="tooltip" id="tooltip_${row.user.UserID.NumericValue}">${entityname}</div>`;
            }
        },
        DateTimeCreated: {
            title: 'Created Date',
            render: (data, row) => {
                let jsonDate = row.user.DateTimeCreated;
                let timestamp = parseInt(jsonDate.match(/\d+/)[0]);
                return formatDateOnly(new Date(timestamp));
            },
            createdCell: cell => cell.classList.add('text-center'),
        },
        Status: {
            title: 'Active',
            render: (data, row) => {
                if (row.menus.edit) {
                    const isChecked = row.user.Status === 1 ? 'checked' : '';
                    const newStatus = row.user.Status === 1 ? 0 : 1;
                    return `<div class="switch switch-sm" style="justify-content: center;">
                                <input ${isChecked} type="checkbox" onclick="ActiveDeactive(this, '${row.user.UserID.EncryptedValue}', ${newStatus})">
                            </div>`;
                }
                return `<div class="switch switch-sm" style="justify-content: center;"><input ${row.user.Status === 1 ? 'checked' : ''} type="checkbox" disabled></div>`;
            },
            createdCell: cell => cell.classList.add('text-center', 'nowrap'),
        },
        Action: {
            title: 'Action',
            render: (data, row) => {
                if (row.menus.edit) {
                    return `<a onclick="OpenChangePassword('${row.user.UserID.EncryptedValue}');" class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);" title="Change Password"><i class="ki-filled ki-password-check"></i></a> 
                            <span class="text-gray-300">|</span>
                            <a onclick="OpenUserModal('${row.user.UserID.EncryptedValue}');" class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);" title="Edit User"><i class="ki-filled ki-notepad-edit"></i></a>`;
                }
                return "-";
            },
            createdCell: cell => cell.classList.add('text-center', 'nowrap'),
        },
    },
};

const dataTableUser = new KTDataTable(elementUser, dataTableOptionsUser);

dataTableUser.on('drew', () => {
    var opacity = "0.7";

    for (var i = 0; i < dataTableUser._data.length; i++) {
        if (dataTableUser._data[i].Status == 0) {
            if (viewpage == "AgencyUsers") {
                $("#TableUserLoginAgency").find("tbody tr:nth-child(" + (i + 1) + ")").css("opacity", opacity);
            }
            else {
                $("#TableUserLogin").find("tbody tr:nth-child(" + (i + 1) + ")").css("opacity", opacity);
            }
        }
    }
});

function clearuserstate() {
    if (dataTableUser && dataTableUser.state) {
        dataTableUser.state.clear();
        window.location.reload();
    }
}

function OpenUserModal(userid, type, entityid, entityname) {
    blockui();
    $.ajax({
        url: '/User/GetUserModal?userid=' + userid + '&type=' + type + '&entityid=' + entityid + '&entityname=' + entityname,
        type: 'GET',
        success: function (data) {
            unblockui();
            //$("#modalsection").html(data).find(".select2").select2();
            $("#modalsection").html(data);
            $("#modalsection").find(".select2").not(".customerselect2").select2({
                dropdownParent: $("#ModalAddUser")
            });
            $("#ddlCustomer").customerSelect2({
                dropdownParent: $("#ModalAddUser")
            });
            OpenModal("ModalAddUser");
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in Getting User");
        }
    });
}

function OpenChangePassword(userid) {
    blockui();
    $.ajax({
        url: '/User/OpenChangePassword?userid=' + userid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            OpenModal("ModalChangePassword");
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in getting password update form");
        }
    });
}

function Checkavailability() {
    var userName = $("#txt_username").val();
    if (!userName) {
        ErrorMessage("Please enter a User Name first.");
        return;
    }
    blockui();
    $.ajax({
        url: '/User/CheckAvailability?Username=' + userName,
        type: 'GET',
        dataType: "json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                $("#btn_save_user").prop("disabled", false);
                $("#formUser").find("input:not([type=hidden]), select").prop("disabled", false);
                $("#formUser").find(".select2").trigger('change');
                SuccessMessage("Username is available");
            } else {
                $("#btn_save_user").prop("disabled", true);
                ErrorMessage("Username is not available. Please try another name.");
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Could not check username availability.");
        }
    });
}

function SaveUser() {
    if ($("#formUser").Validate()) {
        blockui();
        $.ajax({
            url: '/User/SaveUser',
            data: $("#formUser").serialize(),
            type: 'POST',
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    SuccessMessage("User saved successfully!");
                    CloseModal("ModalAddUser");
                    dataTableUser.reload();
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot save user!");
            }
        });
    }
}

function SavePassword() {
    if ($("#formChangePassword").Validate()) {
        blockui();
        $.ajax({
            url: '/User/SavePassword',
            data: $("#formChangePassword").serialize(),
            type: 'POST',
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    SuccessMessage("Password updated successfully!");
                    CloseModal("ModalChangePassword");
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot save password");
            }
        });
    }
}

function ActiveDeactive(current, userid, active) {
    ShowConfirmation({
        title: 'Change Status?',
        message: `Do you really want to change the status to ${active === 1 ? "active" : "inactive"}?`,
        confirmtext: `Yes, Change`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/User/UpdateUserStatus',
                type: 'POST',
                data: {
                    UserIDEncrypted: userid,
                    Status: active
                },
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("Status updated successfully!");
                        dataTableUser.reload();
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                        $(current).prop('checked', !$(current).prop('checked'));
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot update status");
                    $(current).prop('checked', !$(current).prop('checked'));
                }
            });
        },
        onCancel: () => {
            $(current).prop('checked', !$(current).prop('checked'));
        }
    });
}
//function ShowHideEntity(type) {
//    $(".entity").hide();
//    $("#Entity_" + type).show();
//}

function ShowHideEntity(type) {
    $(".entity").hide();
    $("#Entity_" + type).show();
}
(function ($) {
    $.fn.customerSelect2 = function () {
        return this.each(function () {
            var $ddl = $(this);
            if ($ddl.hasClass("select2-hidden-accessible")) {
                $ddl.select2('destroy');
            }
            $ddl.select2({
                dropdownParent: $("#ModalAddUser"),
                width: "100%",
                placeholder: "Search Customer",
                minimumInputLength: 1,
                allowClear: false,
                ajax: {
                    url: "/Client/GetCustomers",
                    dataType: "json",
                    delay: 250,
                    data: function (params) {
                        return {
                            q: params.term
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data.results
                        };
                    },
                    cache: true
                }
            });
        });
    };
})(jQuery);