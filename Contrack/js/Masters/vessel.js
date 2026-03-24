var elementUser;
if (viewpage == "AgencyVessels") {
    elementUser = document.querySelector('#TableVesselsAgency');
}
else {
    elementUser = document.querySelector('#TableVessels');
}

const dataTableOptionsUser = {
    apiEndpoint: domain + '/TableDisplay/Vessels',
    pageSize: 10,
    stateSave: false,

    columns: {
        vesselname: {
            title: 'Vessel Name',
            render: (data, row) => {
                const v = row.vessel ?? {};
                const name = v.vesselname ?? 'N/A';
                const imono = v.imono ?? '';

                const uuid = v.vassignment?.assignmentuuid ?? '';

                return `<div class='flex gap-3 items-center'>
                      <span class="colorbox" style="color:` + v.extras.color + `;background-color:` + v.extras.bgcolor + `">` + v.extras.shortcode + `</span>
                    <div class="flex items-center gap-4">
                        <div class="flex flex-col gap-0.5">
                            <span class="leading-none font-medium text-sm text-gray-900">
                                <a class="text-primary" href="/Vessel/Details?refid=${uuid}">
                                    ${name}
                                </a>
                            </span>
                            <span class="text-2sm text-gray-700 font-normal">
                                ${imono}
                            </span>
                        </div>
                    </div>
                    </div>`;
            }
        },

        vesseltype: {
            title: 'Type',
            render: (data, row) => {
                const v = row.vessel ?? {};
                if (!v.vesseltype && !v.vesselsubtype)
                    return '';

                return `
                    <div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="leading-none font-medium text-sm text-gray-900">
                                ${v.vesseltype ?? ''}
                            </span>
                            <span class="text-2sm text-gray-700 font-normal">
                                ${v.vesselsubtype ?? ''}
                            </span>
                        </div>
                    </div>`;
            }
        },

        picname: {
            title: 'PIC',
            render: (data, row) => {
                const v = row.vessel ?? {};
                if (!v.vesselpicname && !v.vesselpicposition)
                    return '';

                return `
                    <div class="flex items-center gap-2.5">
                        <div class="flex flex-col gap-0.5">
                            <span class="leading-none font-medium text-sm text-gray-900">
                                ${v.vesselpicname ?? ''}
                            </span>
                            <span class="text-2sm text-gray-700 font-normal">
                                ${v.vesselpicposition ?? ''}
                            </span>
                        </div>
                    </div>`;
            }
        },

        cc: {
            title: 'Created For',
            render: (data, row) => {
                const agency = row.vessel?.vassignment;
                const agencyId = agency?.agencyid?.NumericValue ?? 0;
                const agencyName = agency?.agencyname ?? '';

                if (agencyId > 0) {
                    return `
                <div class="flex items-center gap-4">
                    <div class="leading-none w-9 shrink-0">
                        <img src="/assets/icons/agency.svg"/>
                    </div>
                    <div class="flex flex-col gap-0.5">
                        <span class="leading-none font-medium text-sm text-gray-900">Agency</span>
                        <span class="text-2sm text-gray-700 font-normal">${agencyName}</span>
                    </div>
                </div>
            `;
                } else {
                    return `
                <div class="flex items-center gap-4">
                    <div class="leading-none w-9 shrink-0">
                        <img src="/assets/icons/hub.svg"/>
                    </div>
                    <div class="flex flex-col gap-0.5">
                        <span class="leading-none font-medium text-sm text-gray-900">Hub</span>
                        <span class="text-2sm text-gray-700 font-normal">${agencyName}</span>
                    </div>
                </div>
            `;
                }
            }
        },

        hcreatedat: {
            title: 'Created Date',
            render: (data, row) => {
                const v = row.vessel ?? {};
                const raw = v.createdat ?? '';

                if (!raw)
                    return '<span class="text-gray-500">N/A</span>';

                const match = raw.match(/\d+/);
                if (!match)
                    return '<span class="text-gray-500">Invalid</span>';

                const timestamp = parseInt(match[0]);
                const jsDate = new Date(timestamp);

                return typeof formatDateOnly === 'function'
                    ? formatDateOnly(jsDate)
                    : jsDate.toLocaleDateString();
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            }
        },

        uuid: {
            title: 'Actions',
            render: (data, row) => {
                const vessel = row.vessel ?? {};
                const assignment = vessel.vassignment ?? {};

                // Extract EncryptedValue & UUID
                const encryptedId = assignment.vesselassignmentid?.EncryptedValue || "";
                const assignmentUuid = assignment.assignmentuuid || "";
                const canEdit = row.menus?.edit;

                if (!canEdit) return "-";

                return `
            <a onclick="OpenVesselModal('${assignmentUuid}');" 
               class="btn btn-sm btn-icon btn-clear btn-light" 
               href="javascript:void(0);" 
               title="Edit Vessel">
                <i class="ki-filled ki-notepad-edit"></i>
            </a>
            <span class="text-gray-300">|</span>
            <a onclick="DeleteVessel('${encryptedId}', '${assignmentUuid}');" 
               class="btn btn-sm btn-icon btn-clear btn-light" 
               href="javascript:void(0);" 
               title="Delete Vessel">
                <i class="ki-filled ki-trash"></i>
            </a>
        `;
            },
            createdCell(cell) {
                cell.classList.add('text-center', 'nowrap');
            }
        },
    }
};

const dataTableUser = new KTDataTable(elementUser, dataTableOptionsUser);

function OpenVesselModal(refid, agencyid) {
    blockui();
    $.ajax({
        url: '/Vessel/GetVesselModal?refid=' + refid + '&agencyid=' + agencyid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalAddVessel");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Vessel")
        }
    });
}
function SaveVessel() {
    var validated = $("#formVessel").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formVessel").serializeArray();
        $.ajax({
            url: '/Vessel/SaveVessel',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Vessel updated successfully!");
                    CloseModal("ModalAddVessel");

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
                ErrorMessage("Cannot Save Vessel!");
            }
        });
    }
}
function ResetVessel() {
    $(".assignment").hide();
    $("#btn_Vessel").attr("disabled", true);
    EnableDisableVesselControls(1);
}

function ShowHideEntity(type) {
    $(".entity").hide();
    $("#Entiry_" + type).show();
}
function EnableDisableVesselControls(enable) {
    $("#div_existingvessels").html("");
    if (enable == 1) {
        $("#btn_reset").hide();
        $("#txt_vesselname").removeAttr("readonly");
        $("#txt_IMOno").removeAttr("readonly");
        $("#txt_mmsino").removeAttr("readonly");
    }
    else {
        $("#btn_reset").show();
        $("#txt_vesselname").attr("readonly", true);
        $("#txt_IMOno").attr("readonly", true);
        $("#txt_mmsino").attr("readonly", true);
    }
}
function ValidateVessel() {
    if (!$("#formVessel").Validate()) {
        return false;
    }
    if ($("#txt_IMOno").val().trim() === "" && $("#txt_mmsino").val().trim() === "") {
        ErrorMessage("Please enter IMO or MMSI No");
        return false;
    }
    blockui();
    $.ajax({
        url: '/Vessel/ValidateVessel',
        type: 'POST',
        data: {
            vesselname: $("#txt_vesselname").val(),
            imono: $("#txt_IMOno").val(),
            mmsino: $("#txt_mmsino").val()
        },
        success: function (data) {
            unblockui();
            EnableDisableVesselControls(0);

            if (data.ResultId == 1) {
                SuccessMessage('Vessel is available');
                $(".assignment").show();
                $("#btn_Vessel").attr("disabled", false);
            } else {
                $("#div_existingvessels").html(data);
            }
        },
        error: function (xhr) {
            unblockui();
            console.error("Error in ValidateVessel:", xhr.responseText);
            ErrorMessage("Cannot Get Vessel Info");
        }
    });
}
//function ValidateVessel() {
//    if (!$("#formVessel").Validate()) {
//        return false;
//    }

//    if ($("#txt_IMOno").val().trim() == "" && $("#txt_mmsino").val().trim() == "") {
//        ErrorMessage("Please enter IMO or MMSI No");
//        return false;
//    }

//    $.ajax({
//        url: '/Vessel/ValidateVessel',
//        data: 'vesselname=' + $("#txt_vesselname").val() + '&imono=' + $("#txt_IMOno").val() + '&mmsino=' + $("#txt_mmsino").val(),
//        type: 'POST',
//        //dataType: "json",
//        //contentType: "application/json",
//        success: function (data) {
//            unblockui();
//            EnableDisableVesselControls(0);
//            if (data.ResultId == 1) {
//                SuccessMessage('Vessel is available');
//                $(".assignment").show();
//                $("#btn_Vessel").attr("disabled", false);
//                //unblockui();

//                //Swal.fire({
//                //    title: "Vessel Exists!",
//                //    text: "This vessel is already exists. Do you want to create vessel again?",
//                //    icon: "warning",
//                //    showCancelButton: true,
//                //    showDenyButton: true,
//                //    confirmButtonColor: "#3085d6",
//                //    cancelButtonColor: "#919191",
//                //    denyButtonColor: "#d33",
//                //    confirmButtonText: "Use Existing",
//                //    denyButtonText: `Create New`,

//                //}).then((result) => {
//                //    if (result.isConfirmed) {
//                //        $.each(data, function (key, value) {
//                //            $("[name='" + key + "']").val(value);
//                //            if ($("[name='" + key + "']").hasClass("select2")) {
//                //                $("[name='" + key + "']").select2();
//                //            }
//                //        });
//                //        $(".assignment").show();
//                //        $("#btn_Vessel").attr("disabled", false);
//                //    } else if (result.isDenied) {
//                //        $(".assignment").show();
//                //        $("#btn_Vessel").attr("disabled", false);
//                //    }
//                //});

//            }
//            else {
//                $("#div_existingvessels").html(data);
//            }
//        },
//        error: function (data) {
//            unblockui();
//            ErrorMessage("Cannot Get Vessel Info");
//        }
//    });

//    //$(".assignment").show();
//    //$("#btn_Vessel").attr("disabled", false);
//}

function DeleteVessel(encryptedId, assignmentUuid) {
    ShowErrorConfirmation({
        title: 'Delete Vessel?',
        message: `Are you sure you want to delete this vessel?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Vessel/DeleteVessel',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    vesselassignmentid: {
                        EncryptedValue: encryptedId,
                        NumericValue: 0
                    },
                    assignmentuuid: assignmentUuid
                }),
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Vessel deleted successfully!");
                        window.location.href = '/Vessel/List';
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (xhr, status, error) {
                    unblockui();
                    ErrorMessage("Error deleting vessel: " + error);
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}

function CreateNewVessel() {
    $("#div_existingvessels").html("");
    $(".assignment").show();
    $("#btn_Vessel").attr("disabled", false);
}


function MoveVesselToHub(assignmentid, assignmentuuid) {
    ShowConfirmation({
        title: 'Add Vessel?',
        message: `Are you sure you want to add this vessel?`,
        confirmtext: `Yes, Add`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Vessel/MoveVesselToHub',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    vassignment: {
                        vesselassignmentid: {
                            EncryptedValue: assignmentid,
                            NumericValue: 0
                        },
                        assignmentuuid: assignmentuuid
                    },
                }),
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Vessel moved successfully!");
                        window.location.href = '/Vessel/List';
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot move Vessel");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}

