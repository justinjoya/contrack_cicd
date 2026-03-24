let lastupdateddatetime;
$(function () {
    RefreshSticky();
    $('#frmPurchaseIntent').on('submit', function (e) {
        $('#btnSavePI').prop('disabled', true);
        e.preventDefault();
        var validated = $("#frmPurchaseIntent").Validate();
        if (validated) {
            blockui();
            var formdata = $("#frmPurchaseIntent").serializeArray();
            $.ajax({
                url: '/Purchase/CreatePI',
                data: formdata,
                type: 'POST',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("Purchase Intent Saved Successfully!");
                        setTimeout(
                            function () {
                                window.location.href = data.ResultMessage;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                    $('#btnSavePI').prop('disabled', false);
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Save Purchase Intent");
                    $('#btnSavePI').prop('disabled', false);
                }
            });
        }
        else {
            ErrorMessage("Some of required items needs to be filled");
            $('#btnSavePI').prop('disabled', false);
        }
    });
});

function RefreshSticky() {
    $('.stickheader').removeClass('sticky-row');
    $('.stickheader').each(function () {
        var $menu = $(this);
        var offsetTop = $menu.offset().top - 150;
        if (offsetTop < 0)
            offsetTop = $("#scrollable_content").scrollTop() + offsetTop;
        $("#scrollable_content").on('scroll', function () {
            //var offsetTop = $menu.data("offset");

            if ($("#scrollable_content").scrollTop() > offsetTop) {
                $menu.addClass('sticky-row');
            } else {
                $menu.removeClass('sticky-row');
            }
        });
    });
}

$(document).ready(function () {
    function AutoSavePurchaseIntent() {
        var formdata = $("#frmPurchaseIntent").serializeArray();
        $.ajax({
            url: '/Purchase/SavePIDraft',
            data: formdata,
            type: 'POST',
            dataType: "json",
            async: true,
            //contentType: "application/json",
            success: function (data) {
                lastupdateddatetime = new Date();
                $(".edited").removeClass("edited");
            },
            error: function (data) {
                console.log("Cannot Save Draft");
            }
        });
    }

    // Autosave every 30 seconds
    setInterval(
        function () {
            if ($("#chk_AutoSave:checked").length > 0) {
                AutoSavePurchaseIntent();
            }
        }, 30000);

    setInterval(
        function () {
            if (lastupdateddatetime != undefined) {
                var timeago = timeAgo(lastupdateddatetime);
                $("#span_last_updated").text(timeago);
            }
        }, 3000);

});

function SaveMiscData() {
    var formdata = $("#frmPurchaseIntent").serializeArray();
    $.ajax({
        url: '/Purchase/SaveMiscData',
        data: formdata,
        type: 'POST',
        dataType: "json",
        async: false,
        //contentType: "application/json",
        success: function (data) {
            console.log("Misc Success");
        },
        error: function (data) {
            console.log("Cannot Save Misc Data");
        }
    });
}

$("html").on("change", '.purchaseintent input,.purchaseintent select', function () {
    if (!$(this).hasClass("check-single") && !$(this).hasClass("check-group")) {
        $(this).closest("tr").addClass("edited");
    }
});

$("html").on("change", '.checkbox', function () {
    let count = $('.check-single:checked').length;
    if (count > 0) {
        $('.selectedcount').text("(" + count + ")");
        $('.selectedcount').removeClass("hide");
        //$('.checkenabledbuttons').removeProp("disabled");
        $('.checkenabledbuttons').prop("disabled", false);
    }
    else {
        $('.selectedcount').addClass("hide");
        $('.checkenabledbuttons').prop("disabled", true);
    }
});

$(document).on('keydown', function (e) {
    if ($("#frmPurchaseIntent").length > 0) {
        if (e.ctrlKey && (e.key === 's' || e.key === 'S' || e.key === 'Enter')) {
            if ($("#ModalPurchaseIntentLineItem:visible").length > 0) {
                var hdn_Save = $("#ModalPurchaseIntentLineItem").find("#hdn_Save").val();
                e.preventDefault(); // Prevent browser save
                if (hdn_Save == "0")
                    SaveLineItemAndNext();
                else
                    SaveLineItemAndClose();
            }
            else if ($("#frmPurchaseIntent:visible").length > 0) {
                e.preventDefault(); // Prevent browser save
                $('#btnSavePI').click();
            }
        }
        else if (e.ctrlKey && (e.key === 'x' || e.key === 'X')) {
            e.preventDefault();
            ExpandCollapseAll();
        }
    }
});
function OpenPurchaseIntentModal(jobtype, detailuid, current) {
    blockui();
    $.ajax({
        url: '/Purchase/PurchaseIntentModal?jobtype=' + jobtype + '&detailuid=' + detailuid,
        type: 'POST',
        data: current == undefined ? { "PIDetailUUID": "" } : { "UOM": $(current).closest("tr").find(".UOM").val(), "Qty": $(current).closest("tr").find(".Qty").val(), "Mandatory": $(current).closest("tr").find(".Mandatory").is(":checked") },
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalPurchaseIntentLineItem = document.querySelector('#ModalPurchaseIntentLineItem');
            const modal = KTModal.getInstance(ModalPurchaseIntentLineItem);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Processing Request")
        }
    });
}

function DeletePurchaseIntentModal(jobtype, detailuid) {
    blockui();
    SaveMiscData();
    $.ajax({
        url: '/Purchase/DeletePurchaseIntent?jobtype=' + jobtype + '&detailuid=' + detailuid,
        type: 'GET',
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            $("#section_pi_job_details").html(data).find(".select2").select2();
            RefreshSticky();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Deleting Request")
        }
    });
}


function SaveLineItemAndNext() {
    SavePurchaseLineItem("1");

}

function SaveLineItemAndClose() {
    SavePurchaseLineItem("2");
}


function SavePurchaseLineItem(saveflag) {
    var validated = $("#formPurchaseIntentLineItem").Validate();
    if (validated) {
        blockui();
        SaveMiscData();
        //var formdata = $("#formPurchaseIntentLineItem").serializeArray();
        var form = document.getElementById('formPurchaseIntentLineItem');
        var formdata = new FormData(form);
        $.ajax({
            url: '/Purchase/SavePurchaseLineItem',
            data: formdata,
            type: 'POST',
            processData: false,  // Prevent jQuery from converting the data to a string
            contentType: false,  // Prevent jQuery from setting the Content-Type header
            //dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_pi_job_details").html(data).find(".select2").select2();
                RefreshSticky();
                if (saveflag == "1")// Save and Next
                {
                    $("#formPurchaseIntentLineItem").ResetForm();
                    $("#formPurchaseIntentLineItem").find(".select").val("").select2();
                    $("#formPurchaseIntentLineItem").find("#hdn_PIDetailUUID").val("");
                    $("#formPurchaseIntentLineItem").find("#txt_Description").focus();
                    $("#formPurchaseIntentLineItem").find('input[type="file"]').val("");
                }
                else// if (saveflag == "2")// Save and Close
                {
                    const ModalPurchaseIntentLineItem = document.querySelector('#ModalPurchaseIntentLineItem');
                    const modal = KTModal.getInstance(ModalPurchaseIntentLineItem);
                    modal.hide();
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Line Item");
            }
        });
    }
}
function OpenFileImport(jobtypeid) {
    document.getElementById('ExcelFiles_' + jobtypeid).click();
}

function ImportExcel(jobtypeid) {
    var fileUpload = $("#ExcelFiles_" + jobtypeid).get(0);
    var files = fileUpload.files;
    if (files.length > 0) {
        // Create FormData object
        var fileData = new FormData();
        // Looping over all files and add it to FormData object    
        for (var i = 0; i < files.length; i++) {
            fileData.append('ExcelImport.xls', files[i]);
        }
        blockui();
        SaveMiscData();
        $.ajax({
            url: '/Purchase/ImportExcel?JobTypeID=' + jobtypeid,
            type: "POST",
            contentType: false, // Not to set any content header    
            processData: false, // Not to process data    
            data: fileData,
            async: true,
            success: function (data) {
                unblockui();
                $("#section_pi_job_details").html(data).find(".select2").select2();
                RefreshSticky();
            },
            error: function (err) {
                unblockui();
                ErrorMessage("Cannot upload files");
            }
        });

    }
}

function EnableCheckbox(current, jobtype, servicename) {
    if ($(current).is(":checked")) {
        //$(".check-job-" + jobtype).attr("checked", true);
        if (servicename == "") {
            $(".check-job-" + jobtype).prop("checked", true);
        }
        else {
            $(".check-job-" + jobtype + ".check-service-" + servicename.replace(" ", "").trim()).prop("checked", true);
        }
    }
    else {
        //$(".check-job-" + jobtype + ".check-service-" + servicename.replace(" ", "").trim()).removeProp("checked");
        if (servicename == "") {
            $(".check-job-" + jobtype).prop("checked", false);
        }
        else {
            $(".check-job-" + jobtype + ".check-service-" + servicename.replace(" ", "").trim()).prop("checked", false);
        }
        //$(".check-job-" + jobtype).removeAttr("checked");
        //$(".check-service-" + servicename.replace(" ", "").trim()).removeAttr("checked");
    }
}

function BulkDelete() {
    var checkedcount = $('.check-single:checked').length;
    if (checkedcount > 0) {
        Swal.fire({
            title: "Are you sure?",
            text: "You want to delete the selected items?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, do it!"
        }).then((result) => {
            if (result.isConfirmed) {
                blockui();
                SaveMiscData();
                let arrayData = [];

                $('.check-single:checked').each(function (e) {
                    arrayData.push({ "PIDetailUUID": $(this).closest("tr").find(".hdn_PIDetailUUID").val(), "JobTypeID": $(this).attr("datatypeid") });
                });
                //{ "PIDetailUUID": "" }

                $.ajax({
                    url: '/Purchase/DeleteMultiplePurchaseIntent',
                    type: 'POST',
                    data: JSON.stringify(arrayData),
                    contentType: "application/json",
                    success: function (data) {
                        unblockui();
                        $("#section_pi_job_details").html(data).find(".select2").select2();
                        RefreshSticky();
                        $('.selectedcount').addClass("hide");
                        $('.checkenabledbuttons').prop("disabled", true);

                    },
                    error: function (data) {
                        unblockui();
                        ErrorMessage("Error in Deleting multiple items")
                    }
                });
            }

        });
    }
    else {
        ErrorMessage("Please select atleast one item to delete");
    }
}

function OpenPurchaseIntentBulkUpdateModal() {
    blockui();
    $.ajax({
        url: '/Purchase/BulkUpdatePurchaseIntentModal',
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalPurchaseIntentBulkUpdate = document.querySelector('#ModalPurchaseIntentBulkUpdate');
            const modal = KTModal.getInstance(ModalPurchaseIntentBulkUpdate);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Processing Request")
        }
    });
}

function EableDisableBulk(current, targetid) {
    if ($(current).is(":checked")) {
        $("#" + targetid).find("input").prop("disabled", false);
        $("#" + targetid).find("select").prop("disabled", false);
    }
    else {
        $("#" + targetid).find("input").prop("disabled", true);
        $("#" + targetid).find("select").prop("disabled", true);
    }

}

function BulkUpdate() {
    var checkedcount = $('.check-single:checked').length;
    if (checkedcount > 0) {

        blockui();
        let arrayData = [];
        let listData = [];

        $('.check-single:checked').each(function (e) {
            listData.push({ "PIDetailUUID": $(this).closest("tr").find(".hdn_PIDetailUUID").val(), "JobTypeID": $(this).attr("datatypeid") });
        });
        //arrayData.push({ "List": listData });
        //arrayData.push({ "Bulk": $("#formPurchaseIntentBulkUpdate").serializeArray() });
        SaveMiscData();
        $.ajax({
            url: '/Purchase/UpdateMultiplePurchaseIntent',
            type: 'POST',
            data: JSON.stringify({ "List": listData, "Bulk": serializeFormToObject("#formPurchaseIntentBulkUpdate") }),
            contentType: "application/json",
            success: function (data) {
                unblockui();
                $("#section_pi_job_details").html(data).find(".select2").select2();
                RefreshSticky();
                $('.selectedcount').addClass("hide");
                $('.checkenabledbuttons').prop("disabled", true);

                const ModalPurchaseIntentBulkUpdate = document.querySelector('#ModalPurchaseIntentBulkUpdate');
                const modal = KTModal.getInstance(ModalPurchaseIntentBulkUpdate);
                modal.hide();
                //SuccessMessage("Updated");

            },
            error: function (data) {
                unblockui();
                ErrorMessage("Error in Deleting multiple items");
            }
        });

    }
    else {
        ErrorMessage("Please select atleast one item to update");
    }
}

function MoveUpDown(JobTypeID, ServiceName, Direction) {
    blockui();
    SaveMiscData();
    $.ajax({
        url: '/Purchase/MoveService?JobTypeID=' + JobTypeID + '&ServiceName=' + ServiceName + '&Direction=' + Direction,
        type: 'GET',
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == undefined) {
                $("#section_pi_job_details").html(data).find(".select2").select2();
                RefreshSticky();
                $('.selectedcount').addClass("hide");
                $('.checkenabledbuttons').prop("disabled", true);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }

        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Moving multiple items")
        }
    });
}

function MoveLineItems() {
    var checkedcount = $('.check-single:checked').length;
    if (checkedcount > 0) {
        blockui();
        var TargetIndex = $("#txt_MoveNumber").val();
        let arrayData = [];

        $('.check-single:checked').each(function (e) {
            arrayData.push({ "PIDetailUUID": $(this).closest("tr").find(".hdn_PIDetailUUID").val(), "JobTypeID": $(this).attr("datatypeid") });
        });
        //{ "PIDetailUUID": "" }
        SaveMiscData();
        $.ajax({
            url: '/Purchase/MoveMultipleItems?TargetIndex=' + TargetIndex,
            type: 'POST',
            data: JSON.stringify(arrayData),
            contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == undefined) {
                    $("#section_pi_job_details").html(data).find(".select2").select2();
                    RefreshSticky();
                    $('.selectedcount').addClass("hide");
                    $('.checkenabledbuttons').prop("disabled", true);
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }

            },
            error: function (data) {
                unblockui();
                ErrorMessage("Error in Moving multiple items")
            }
        });
    }
    else {
        ErrorMessage("Please select atleast one item to move");
    }
}

function ExpandCollapse(JobTypeID) {
    if ($(".icon_" + JobTypeID).hasClass("ki-minus")) {
        $(".icon_" + JobTypeID).removeClass("ki-minus").addClass("ki-plus");
        $("#accordion_content_" + JobTypeID).removeClass("expanded");
        $("#accordion_tr_" + JobTypeID).removeClass("expanded");
        $("#hdn_Misc_JobExpand_" + JobTypeID).val("1");
    }
    else {
        $(".icon_" + JobTypeID).removeClass("ki-plus").addClass("ki-minus");
        $("#accordion_content_" + JobTypeID).addClass("expanded");
        $("#accordion_tr_" + JobTypeID).addClass("expanded");
        $("#hdn_Misc_JobExpand_" + JobTypeID).val("");
    }
}
function ExpandCollapseHeader() {
    if ($(".iconmain").hasClass("ki-minus")) {
        $(".iconmain").removeClass("ki-minus").addClass("ki-plus");
        $(".maininfo").removeClass("expanded");
        $("#hdn_Misc_BasicExpand").val("1");
    }
    else {
        $(".iconmain").removeClass("ki-plus").addClass("ki-minus");
        $(".maininfo").addClass("expanded");
        $("#hdn_Misc_BasicExpand").val("");
    }
}


function ExpandCollapseAll() {
    if ($(".iconexpand").hasClass("ki-minus")) {
        $(".iconexpand").removeClass("ki-minus").addClass("ki-plus");
        $(".accordion-content").removeClass("expanded");
        $(".jobheader").removeClass("expanded");

        $(".iconmain").removeClass("ki-minus").addClass("ki-plus");
        $(".maininfo").removeClass("expanded")

        $("#hdn_Misc_BasicExpand").val("1");
        $(".jobheader").each(function () {
            var id = $(this).attr("id");
            var JobTypeID = id.replace("accordion_tr_", "");
            $("#hdn_Misc_JobExpand_" + JobTypeID).val("1");
        });

    }
    else {
        $(".iconexpand").removeClass("ki-plus").addClass("ki-minus");
        $(".accordion-content").addClass("expanded");
        $(".jobheader").addClass("expanded");

        $(".iconmain").removeClass("ki-plus").addClass("ki-minus");
        $(".maininfo").addClass("expanded")

        $("#hdn_Misc_BasicExpand").val("");
        $(".jobheader").each(function () {
            var id = $(this).attr("id");
            var JobTypeID = id.replace("accordion_tr_", "");
            $("#hdn_Misc_JobExpand_" + JobTypeID).val("");
        });
    }
}

function FilterJobType(current, jobtype) {
    if (jobtype == "")
        $(".sectiongroup").removeClass("hide");
    else {
        $(".sectiongroup").addClass("hide");
        $(".rowheader_" + jobtype).removeClass("hide");
        $("#accordion_content_" + jobtype).removeClass("hide");
    }
    $("#jobtypecategory").find(".btn").removeClass("active");
    $(current).addClass("active");
    $("#hdn_Misc_JobTypeFilter").val(jobtype);
}


function OpenRFQModal() {
    var checkedcount = $('.check-single:checked').length;
    if (checkedcount > 0) {

        blockui();
        let arrayData = [];
        let listData = [];
        var AgencyDetailID = $("#ddlAgencies").val();
        var PurchaseIntentID = $("#hdn_PurchaseIntentIDEnc").val();
        var PurchaseIntentUUID = $("#hdn_PurchaseIntentUUIDID").val();

        $('.check-single:checked').each(function (e) {
            listData.push($(this).closest("tr").find(".hdn_PIDetailID").val());
        });
        //arrayData.push({ "List": listData });
        //arrayData.push({ "Bulk": $("#formPurchaseIntentBulkUpdate").serializeArray() });
        SaveMiscData();
        $.ajax({
            url: '/Purchase/GetRFQModal?AgencyDetailID=' + AgencyDetailID,
            type: 'POST',
            data: JSON.stringify({ "PurchaseIntentID": PurchaseIntentID, "PurchaseIntentUUID": PurchaseIntentUUID, "SelectedIDs": listData }),
            contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == undefined) {
                    $("#modalsection").html(data).find(".select2").select2();
                    const ModalCreateRFQ = document.querySelector('#ModalCreateRFQ');
                    const modal = KTModal.getInstance(ModalCreateRFQ);
                    modal.show();
                }
                else
                    ErrorMessage(data.ResultMessage);
                //SuccessMessage("Updated");

            },
            error: function (data) {
                unblockui();
                ErrorMessage("Error in Creating RFQ");
            }
        });

    }
    else {
        ErrorMessage("Please select atleast one item to Create RFQ");
    }
}


function CreateRFQ() {
    blockui();
    var formdata = $("#formCreateRFQModal").serializeArray();
    $.ajax({
        url: '/Purchase/CreateRFQ',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == "1") {
                if ($("#chk_ShowRFQList").is(":checked")) {
                    SuccessMessage("RFQ Created Successfully!");
                    setTimeout(
                        function () {
                            window.location.href = data.ResultMessage;
                        }, 500);
                }
                else {
                    const ModalCreateRFQ = document.querySelector('#ModalCreateRFQ');
                    const modal = KTModal.getInstance(ModalCreateRFQ);
                    modal.hide();
                    SuccessMessage("Created Successfully!");
                }
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Create RFQ");
        }
    });
}

function OpenJobTypeManageModal() {
    blockui();
    $.ajax({
        url: '/Purchase/ManageJobType',
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);


            const ModalReOrderRFQ = document.querySelector('#ModalManageJobType');
            const modal = KTModal.getInstance(ModalManageJobType);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open Manage jobtype");
        }
    });
}


function SaveJobType() {
    blockui();
    var selectedValues = [];
    $("input[name='PIJobType']:checked").each(function () {
        selectedValues.push($(this).val());
    });

    $.ajax({
        url: '/Purchase/SaveJobType',
        data: { PIJobType: selectedValues },
        traditional: true,
        type: 'POST',
        //dataType: "json",
        /*contentType: "application/json",*/
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
            ErrorMessage("Cannot Save Jobtype");
        }
    });

}