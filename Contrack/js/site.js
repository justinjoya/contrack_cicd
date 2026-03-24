var counter1 = 0;
var documentuploadedcount = 0;
function blockui(message) {
    if (message == undefined || message == null || message == "") {
        message = "Loading...";
    }
    $.blockUI({
        message: '<svg class="animate-spin -ml-1 h-5 w-5 text-gray-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">' +
            '<circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="3"></circle>' +
            '<path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>' +
            '</svg>' + message,
    });
}
$(document).ready(function () {
    //$('.focus-input').focus(function () {
    //    $(this).select();
    //});

    $("html").on("focus", '.focus-input', function () {
        $(this).select();
    });

    $("html").on("change", '.checkall', function () {
        $(".checksingle").prop("checked", $(this).is(":checked")).trigger('change');
    });
    //setInterval(function () {
    //    $.ajax({
    //        url: '/EmailSchedular/GetEmailNotification',
    //        type: 'GET',
    //        //contentType: "application/json",
    //        success: function (data) {
    //            $("#div_EmailNotification").html(data);
    //        },
    //        error: function (data) {
    //        }
    //    });
    //}, 2000);

    if ($('.checkedbuttons').length > 0) {
        $("html").on("change", '.checksingle', function () {
            var anyChecked = $('.checksingle:checked').length > 0;
            if (anyChecked) {
                $('.checkedbuttons').prop('disabled', false);
            } else {
                $('.checkedbuttons').prop('disabled', true);
            }
        });
    }
});

$(document).ajaxStart(function () {
    $.ajax({
        url: '/Account/VerifySession',
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            if (data.ResultId == 0)
                window.location.href = "/Account/login";
        },
        error: function (data) {
        }
    });
});

if ($(".date-mask").length > 0) {
    var cleavedate = new Cleave('.date-mask', {
        date: true,
        delimiter: '/',
        datePattern: ['m', 'd', 'Y']
    });
}


if ($(".select2").length > 0) {
    $(".select2").select2();
}

if ($(".datepicker-here").length > 0) {
    $(".datepicker-here").each(function () {
        var datatarget = $(this).data("trigger");
        var dataclear = $(this).data("clear");
        var mindatefield = $(this).data("min-date-field");

        var datepicker = $(this).flatpickr({
            dateFormat: "Y-m-d",
            altInput: true,
            altFormat: "M j, Y",
            onChange: function (selectedDates) {
                if ($("#" + mindatefield).length > 0) {
                    if (selectedDates.length > 0) {
                        let min = new Date(selectedDates[0]);
                        min.setDate(min.getDate() + 1);   // force next day
                        $("#" + mindatefield)[0]._flatpickr.set("minDate", min);
                    }
                }
            }
        });


        if (datatarget != undefined) {
            document.getElementById(datatarget).addEventListener("click", function () {
                datepicker.open(); // Manually open the Flatpickr
            });
        }

        if (dataclear != undefined) {
            document.getElementById(dataclear).addEventListener("click", function () {
                datepicker.clear();
            });
        }
    });

}


if ($(".datetimepicker-here").length > 0) {
    $(".datetimepicker-here").each(function () {
        var datatarget = $(this).data("trigger");
        var dataclear = $(this).data("clear");

        var datepicker = $(this).flatpickr({
            enableTime: true,
            dateFormat: "Y-m-d H:i",
            altInput: true,
            altFormat: "M j, Y h:i K"
        });
        if (datatarget != undefined) {
            document.getElementById(datatarget).addEventListener("click", function () {
                datepicker.open(); // Manually open the Flatpickr
            });
        }
        if (dataclear != undefined) {
            document.getElementById(dataclear).addEventListener("click", function () {
                datepicker.clear();
            });
        }
    });

}

if ($(".searchablevessel").length > 0) {
    //dataagencyid
    $('.searchablevessel').each(function () {
        var current = $(this);
        var AgencyDetailID = current.data("agencyid");
        VesselSearch(current, AgencyDetailID);
    });

}

//if ($(".searchableponumber").length > 0) {
//    //dataagencyid
//    $('.searchableponumber').each(function () {
//        var current = $(this);
//        var AgencyDetailID = current.data("agencyid");
//        VesselSearch(current, AgencyDetailID);
//    });

//}

function OpenDatePicker(target) {
    $("#" + target).click();
}

function unblockui() {
    $.unblockUI();
}

function ClearDataTableStates(datatableobj) {
    datatableobj.goPage(1);
}

function GoBack() {
    window.history.back();
}


function SuccessMessage(message) {
    const node = document.createElement('div');
    node.innerHTML = `
        <div class='toast-box flex items-center'>
           <img src='/assets/icons/tick-circle.svg' class='icon'/>
            <span>${message}</span>
        </div>
    `;
    Toastify({
        node: node,
        /* text: message,*/
        duration: 3000,
        newWindow: true,
        //close: true,
        gravity: "top", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        //stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "#0AA739",
            borderRadius: "39px",
            padding: "10px 16px",
            boxShadow: " 0 11px 48px 0 rgba(29, 29, 29, 0.38);"

        },
    }).showToast();
    setTimeout(() => {
        const el = document.querySelector(".toastify.on");
        if (el) el.style.top = "62px"; // move it 40px from top
    }, 10);
}

function ErrorMessage(message) {
    const node = document.createElement('div');
    node.innerHTML = `
        <div class='toast-box flex items-center'>
           <img src='/assets/icons/close-circle.svg' class='icon'/>
            <span>${message}</span>
        </div>
    `;
    Toastify({
        node: node,
        /* text: message,*/
        duration: 3000,
        newWindow: true,
        //close: true,
        gravity: "top", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        //stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "#EA2640",
            borderRadius: "39px",
            padding: "10px 16px",
            boxShadow: "0 11px 48px 0 rgba(29, 29, 29, 0.38)"

        },
    }).showToast();

    setTimeout(() => {
        const el = document.querySelector(".toastify.on");
        if (el) el.style.top = "62px"; // move it 40px from top
    }, 10);
}

function LoadingMessage(message) {
    const node = document.createElement('div');
    node.innerHTML = `
        <div class='toast-box flex items-center'>
            <span>${message}</span>
        </div>
    `;
    Toastify({
        node: node,
        /* text: message,*/
        duration: 3000,
        newWindow: true,
        //close: true,
        gravity: "top", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        //stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "#3157FF",
            borderRadius: "39px",
            padding: "10px 16px",
            boxShadow: "0 11px 48px 0 rgba(29, 29, 29, 0.38)"

        },
    }).showToast();

    setTimeout(() => {
        const el = document.querySelector(".toastify.on");
        if (el) el.style.top = "62px"; // move it 40px from top
    }, 10);
}

function formatDate(date) {
    const day = String(date.getDate()).padStart(2, '0'); // Ensures 2-digit day
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    const month = monthNames[date.getMonth()];
    const year = date.getFullYear();

    let hours = date.getHours();
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12; // Convert 24-hour format to 12-hour

    return `${day} ${month} ${year} ${String(hours).padStart(2, '0')}:${minutes}:${seconds} ${ampm}`;
}

function formatDateOnly(date) {
    const day = String(date.getDate()).padStart(2, '0'); // Ensures 2-digit day
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    const month = monthNames[date.getMonth()];
    const year = date.getFullYear();

    let hours = date.getHours();
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12; // Convert 24-hour format to 12-hour

    return `${day} ${month} ${year}`;
}


$("html").on("keyup keypress change", '.CustomePlaceholderKey', function () {
    var txtval = $(this).val();
    if (txtval == "")
        txtval = "Field value";
    $(this).closest("div").find("textarea").attr("placeholder", "Enter " + txtval);
});


function OpenClientContactModal(refid, clientid) {
    blockui();
    $.ajax({
        url: '/Client/GetClientContactModal?refid=' + refid + '&clientid=' + clientid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalClientContact = document.querySelector('#ModalClientContact');
            const modal = KTModal.getInstance(ModalClientContact);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Contact")
        }
    });
}

function OpenVesselContactModal(refid, vassignmentid) {
    blockui();
    $.ajax({
        url: '/Vessel/GetVesselContactModal?refid=' + refid + '&vassignmentid=' + vassignmentid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalVesselContact = document.querySelector('#ModalVesselContact');
            const modal = KTModal.getInstance(ModalVesselContact);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Contact")
        }
    });
}

function OpenVendorContactModal(refid, vendorid) {
    blockui();
    $.ajax({
        url: '/Vendor/GetVendorContactModal?refid=' + refid + '&vendorid=' + vendorid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalVendorContact = document.querySelector('#ModalVendorContact');
            const modal = KTModal.getInstance(ModalVendorContact);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Contact")
        }
    });
}



function SaveClientContact(dontreload) {
    var validated = $("#formClientcontact").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formClientcontact").serializeArray();
        $.ajax({
            url: '/Client/SaveContact',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Contact updated successfully!");
                    const ModalClientContact = document.querySelector('#ModalClientContact');
                    const modal = KTModal.getInstance(ModalClientContact);
                    modal.hide();
                    if (dontreload != "1") {
                        setTimeout(
                            function () {
                                window.location.href = window.location;
                            }, 400);
                    }

                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);

                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Contact!");
            }
        });
    }
}



function SaveVesselContact(dontreload) {
    var validated = $("#formVesselcontact").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formVesselcontact").serializeArray();
        $.ajax({
            url: '/Vessel/SaveContact',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Contact updated successfully!");
                    const ModalVesselContact = document.querySelector('#ModalVesselContact');
                    const modal = KTModal.getInstance(ModalVesselContact);
                    modal.hide();
                    if (dontreload != "1") {
                        setTimeout(
                            function () {
                                window.location.href = window.location;
                            }, 400);
                    }

                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);

                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Contact!");
            }
        });
    }
}


function SaveVendorContact(dontreload) {
    var validated = $("#formVendorcontact").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formVendorcontact").serializeArray();
        $.ajax({
            url: '/Vendor/SaveVendorContact',
            data: formdata,
            type: 'POST',
            dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Contact updated successfully!");
                    const ModalVendorContact = document.querySelector('#ModalVendorContact');
                    const modal = KTModal.getInstance(ModalVendorContact);
                    modal.hide();
                    if (dontreload != "1") {
                        setTimeout(
                            function () {
                                window.location.href = window.location;
                            }, 400);
                    }

                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);

                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Contact!");
            }
        });
    }
}

function timeAgo(date) {
    const now = new Date();
    const past = new Date(date);
    const diffInSeconds = Math.floor((now - past) / 1000);

    const units = [
        { label: "year", seconds: 31536000 },
        { label: "month", seconds: 2592000 },
        { label: "day", seconds: 86400 },
        { label: "hour", seconds: 3600 },
        { label: "min", seconds: 60 },
        { label: "sec", seconds: 1 }
    ];

    for (let unit of units) {
        const value = Math.floor(diffInSeconds / unit.seconds);
        if (value >= 1) {
            return `${value} ${unit.label}${value > 1 ? 's' : ''} ago`;
        }
    }

    return 'Just now';
}

function serializeFormToObject(formSelector) {
    let obj = {};
    let form = $(formSelector).serializeArray();

    form.forEach(function (item) {
        if (obj[item.name]) {
            // Handle multiple values (e.g., checkboxes)
            if (!Array.isArray(obj[item.name])) {
                obj[item.name] = [obj[item.name]];
            }
            obj[item.name].push(item.value);
        } else {
            obj[item.name] = item.value;
        }
    });

    return obj;
}

function ValidateAgency(current) {
    counter1 = 0;
    var targetcount = 2;
    if ($("#vendor_placeholder").length > 0) {
        targetcount = 3;
    }
    blockui();
    var clientcountrolname = "ClientDetailID";
    var portcountrolname = "Port";
    var vendorcountrolname = "vendordetailid";

    if ($("#frmCreatePO").length > 0) {
        clientcountrolname = "PI.ClientDetailID";
        portcountrolname = "PI.Port";
    }

    GenerateClient($(current).val(), "client_placeholder", targetcount, clientcountrolname);
    GeneratePort($(current).val(), "deliveryport_placeholder", targetcount, portcountrolname);
    if ($("#vendor_placeholder").length > 0) {
        GenerateVendor($(current).val(), "vendor_placeholder", targetcount, vendorcountrolname);
    }
    GenerateVessel($(current).val(), "ddlVesselAssignmentID");
}

function ValidateVICompany(current) {
    counter1 = 0;
    blockui();
    GenerateVendor($(current).val(), "vendor_placeholder", 1, "VendorDetailID");
    GenerateVessel($(current).val(), "ddlVesselAssignmentID");
}
function GenerateClient(agencydetailid, targetid, countertarget, name) {
    $.ajax({
        url: '/Master/GetClientDropdown?AgencyDetailID=' + agencydetailid + '&name=' + name,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
            $("#" + targetid).html(data).find(".select2").select2();
        },
        error: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
        }
    });
}

function GeneratePort(agencydetailid, targetid, countertarget, name) {
    $.ajax({
        url: '/Master/GetPortDropdown?AgencyDetailID=' + agencydetailid + '&name=' + name,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
            $("#" + targetid).html(data).find(".select2").select2();
        },
        error: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
        }
    });
}

function GenerateVendor(agencydetailid, targetid, countertarget, name) {
    var ddlVendor = "";
    if ($("#ddlVendor").length > 0) {
        ddlVendor = $("#ddlVendor").val();
    }
    $.ajax({
        url: '/Dropdown/GetVendorDropdown?AgencyDetailID=' + agencydetailid + '&name=' + name,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
            $("#" + targetid).html(data).find(".select2").select2();
            if ($("#ddlVendor").length > 0 && ddlVendor != "") {
                if ($("#ddlVendor option[value='" + ddlVendor + "']").length > 0) {
                    $("#ddlVendor").val(ddlVendor).select2();
                }
            }
        },
        error: function (data) {
            counter1++;
            if (countertarget == counter1) {
                unblockui();
            }
        }
    });
}

function GenerateVessel(agencydetailid, targetid) {
    var current = $("#" + targetid);
    var AgencyDetailID = agencydetailid;
    VesselSearch(current, AgencyDetailID);
}

function VesselSearch(current, AgencyDetailID) {
    var multiple = $(current).attr("multiple") == "multiple" ? "1" : "";
    current.select2({
        placeholder: 'Enter Vessel Name to search',
        minimumInputLength: 2,
        ajax: {
            url: '/Vessel/GetVesselDropdownList?AgencyDetailID=' + AgencyDetailID + "&multiple=" + multiple,
            dataType: 'json',
            delay: 250,
            processResults: function (data) {
                return {
                    results: data,
                    pagination: false
                };
            },
            cache: true
        }
    });
    if (multiple == "") {
        current.each(function () {
            if ($(this).attr("datatext") != "") {
                var newOption = new Option($(this).attr("datatext"), $(this).attr("datavalue"), true, true);
                $(this).append(newOption); //.trigger('change')
            }
        });
    }
}

$(document).on("keydown", ".navigation-table tbody input", function (e) {
    const currentCell = $(this);
    const currentRow = currentCell.closest("tr");
    var data_nav = $(currentCell).data("nav")

    if (e.key === "ArrowDown" || e.key === 'Enter') {
        e.preventDefault();
        const nextRow = currentRow.next("tr");
        if (nextRow.length) {
            nextRow.find("." + data_nav).focus();
        }
    } else if (e.key === "ArrowUp") {
        e.preventDefault();
        const prevRow = currentRow.prev("tr");
        if (prevRow.length) {
            prevRow.find("." + data_nav).focus();
        }
    }
});

function CreateQuoteComparison(rfqlist, piid) {
    blockui();
    var rfqList = rfqlist.split(",");  // Replace with actual UUIDs
    var purchaseIntentId = piid;
    $.ajax({
        url: '/Purchase/CreateQuoteComparison',
        data: {
            rfqlist: rfqList,
            purchaseintentid: purchaseIntentId
        },
        type: 'POST',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                //SuccessMessage("Quote Comparison created Successfully!");
                setTimeout(
                    function () {
                        window.location.href = data.ResultMessage;
                    }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Create Comparison");
        }
    });

}


function displayprice(price) {
    const formatted = price.toLocaleString('en-IN', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
    return formatted;  // Default locale (e.g., "1,234,567.89")
}

function OpenDocumentModal(p_documenttypeid, p_parentuuid, p_targetuuid, p_targetid) {
    documentuploadedcount = 0;
    blockui();
    $.ajax({
        url: '/Document/GetDocumentModal?documenttypeid=' + p_documenttypeid + "&parentuuid=" + p_parentuuid + "&targetuuid=" + p_targetuuid + "&targetid=" + p_targetid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalDocumentUpload = document.querySelector('#ModalDocumentUpload');
            const modal = KTModal.getInstance(ModalDocumentUpload);
            modal.show();
        },
        error: function (data) {
            unblockui();
            Swal.fire({
                title: 'Error in Getting Documents',
                text: '',
                icon: 'error',
                allowOutsideClick: true,
            });
        }
    });
}

function refreshdocount() {
    try {
        documentuploadedcount = $("#div_Document_inner tbody").find("tr").length;
        var targetuuid = $("#hdn_targetuuid").val();
        switch ($("#hdn_documenttypeid").val()) {
            case "2":

                if (documentuploadedcount > 0) {
                    $("#attach_item_" + targetuuid).text("Attach (" + documentuploadedcount + ")");
                    $("#attach_item_" + targetuuid).removeClass("text-gray-600");
                }
                else {
                    $("#attach_item_" + targetuuid).text("Attach");
                    $("#attach_item_" + targetuuid).addClass("text-gray-600");
                }
                break;
            default:
                break;
        }
    } catch (e) { }
}
function UploadAttachmentFiles() {

    if ($('#AttachmentFileInput')[0].files.length === 0) {
        ErrorMessage("Please select file");
    }
    else {
        blockui();
        //var formdata = $("#formPurchaseIntentLineItem").serializeArray();
        var form = document.getElementById('formDocumentUpload');
        var formdata = new FormData(form);
        $.ajax({
            url: '/Document/UploadFiles',
            data: formdata,
            type: 'POST',
            processData: false,  // Prevent jQuery from converting the data to a string
            contentType: false,  // Prevent jQuery from setting the Content-Type header
            //dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                if (data.ResultId == 1) {
                    SuccessMessage("Uploaded Successfully!");
                    RefreshAttachment();
                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Line Item");
            }
        });
    }
}


function RefreshAttachment() {
    var form = document.getElementById('formDocumentUpload');
    var formdata = new FormData(form);
    $.ajax({
        url: '/Document/RefreshDocument',
        data: formdata,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (data) {
            unblockui();
            $("#div_Document_inner").html(data);
            refreshdocount();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot refresh the document list");
        }
    });

}

function DeleteDocument(docuuid, current) {

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
                url: '/Document/DeleteDocument',
                type: 'POST',
                data: 'docuuid=' + docuuid,
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Deleted successfully!");
                        $(current).closest("tr").remove();
                        refreshdocount();
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot delete Document");
                }
            });
        }
    });
}

function OpenClientInfoDetailID(current, targetid, targetkey) {
    blockui();
    var detailid = $(current).val();
    $.ajax({
        url: '/Master/GetClientInfoByDetailID?ClientDetailID=' + detailid,
        type: 'GET',
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#" + targetid).val(data[targetkey])
        },
        error: function (data) {
            unblockui();
        }
    });
}

function OpenCommentModal() {
    const ModalChatHistory = document.querySelector('#ModalChatHistory');
    const modal = KTModal.getInstance(ModalChatHistory);
    modal.show();
}

function isUUID(str) {
    const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    return uuidRegex.test(str);
}

function OpenModal(id) {
    const ModalContainer = document.querySelector('#' + id);
    const modal = KTModal.getInstance(ModalContainer);
    modal.show();
}

function CloseModal(id) {
    const ModalContainer = document.querySelector('#' + id);
    const modal = KTModal.getInstance(ModalContainer);
    modal.hide();
}


function LoadLogs(p_uuid, p_type) {
    $.ajax({
        url: '/Logs/GetLogList?UUID=' + p_uuid + "&type=" + p_type,
        type: 'GET',
        success: function (data) {
            $("#logsection").html(data);
        },
        error: function (data) {
        }
    });
}

function ShowConfirmation({ title, message, confirmtext, canceltext, onConfirm, onCancel }) {
    if (confirmtext == "" || confirmtext == undefined || confirmtext == null) {
        confirmtext = "Confirm";
    }
    if (canceltext == "" || canceltext == undefined || canceltext == null) {
        confirmtext = "Cancel";
    }
    Swal.fire({
        html: `
          <div class="custom-confirmation-popup">
            <img class="bg" src='/assets/icons/blue-confirm.svg'/>
            <div class="custom-body flex flex-col">
                <span class="title">${title || ''}</span>
                <span class="message">${message || ''}</span>
            </div>
            <div class="custom-buttons flex">
              <button class="btn btn-light btn-cancel" id="btnCancel">${canceltext}</button>
              <button class="btn btn-primary  btn-confirm" id="btnConfirm">${confirmtext}</button>
            </div>
          </div>
        `,
        showConfirmButton: false,
        background: 'transparent',
        didOpen: () => {
            document.getElementById('btnConfirm').addEventListener('click', () => {
                Swal.close();
                if (typeof onConfirm === 'function') onConfirm();
            });

            document.getElementById('btnCancel').addEventListener('click', () => {
                Swal.close();
                if (typeof onCancel === 'function') onCancel();
            });
        }
    });
}

function ShowErrorConfirmation({ title, message, confirmtext, canceltext, onConfirm, onCancel }) {
    if (confirmtext == "" || confirmtext == undefined || confirmtext == null) {
        confirmtext = "Confirm";
    }
    if (canceltext == "" || canceltext == undefined || canceltext == null) {
        confirmtext = "Cancel";
    }
    Swal.fire({
        html: `
          <div class="custom-confirmation-popup error">
            <img class="bg" src='/assets/icons/red-confirm.svg'/>
            <div class="custom-body flex flex-col">
                <span class="title">${title || ''}</span>
                <span class="message">${message || ''}</span>
            </div>
            <div class="custom-buttons flex">
              <button class="btn btn-light btn-cancel" id="btnCancel">${canceltext}</button>
              <button class="btn btn-danger  btn-confirm" id="btnConfirm">${confirmtext}</button>

            </div>
          </div>
        `,
        showConfirmButton: false,
        background: 'transparent',
        didOpen: () => {
            document.getElementById('btnConfirm').addEventListener('click', () => {
                Swal.close();
                if (typeof onConfirm === 'function') onConfirm();
            });

            document.getElementById('btnCancel').addEventListener('click', () => {
                Swal.close();
                if (typeof onCancel === 'function') onCancel();
            });
        }
    });
}
function datawithprogressbar(total, count, color, text) {
    var percentage = total > 0 ? parseInt((count / total) * 100) : 0;
    return `<div class="flex flex-col gap-1.5">
        <div class="flex justify-between items-center text-sm font-bold uppercase tracking-tight">
        <span class="${color} ${count == 0 ? "empty" : ""} font-semibold text-sm">${count}</span>
        <span class="text-gray-500 text-xs">${percentage}%</span>
        </div>
        <div class="progress">
        <div class="progress-bar ${color}" style="width: ${percentage}%"></div>
        </div>
        </div>`
}



function ShowErrorMessageOnly({ title, message, confirmtext, canceltext, onConfirm, onCancel }) {
    if (confirmtext == "" || confirmtext == undefined || confirmtext == null) {
        confirmtext = "Confirm";
    }
    if (canceltext == "" || canceltext == undefined || canceltext == null) {
        confirmtext = "Cancel";
    }
    Swal.fire({
        html: `
          <div class="custom-confirmation-popup error">
            <img class="bg" src='/assets/icons/red-confirm.svg'/>
            <div class="custom-body flex flex-col">
                <span class="title">${title || ''}</span>
                <span class="message">${message || ''}</span>
            </div>
            <div class="custom-buttons flex">
              <button class="btn btn-danger  btn-confirm" id="btnConfirm">${confirmtext}</button>

            </div>
          </div>
        `,
        showConfirmButton: false,
        background: 'transparent',
        showCancelButton: false,
        didOpen: () => {
            document.getElementById('btnConfirm').addEventListener('click', () => {
                Swal.close();
                if (typeof onConfirm === 'function') onConfirm();
            });

            document.getElementById('btnCancel').addEventListener('click', () => {
                Swal.close();
                if (typeof onCancel === 'function') onCancel();
            });
        }
    });
}