const dataTableOptionsVoyage = {
    apiEndpoint: domain + '/TableDisplay/Voyage',
    pageSize: 10,
    stateSave: false,

    columns: {
        expand: {
            title: "",
            render: () => {
                return `
            <button class="btn btn-sm btn-icon  btn-light original toggle-row">
                <i class="ki-filled ki-plus"></i>
            </button>
            <span class='horizontallinetop'></span>
        `;
            }
        },

        voyagenumber: {
            title: "Voyage Number",
            render: (d, row) =>
                `<span class="flex items-center grow1 gap-2.5">
            <span class="flex flex-col gap-1">
                <a class="voyageno-link " style="border-bottom:0;" href="#">
                    ${row.VoyageDTO.VoyageNumber} ${row.VoyageDTO.Status}
                </a>
                <span class="voyageno-vessel">
                    <i class="ki-filled ki-ship"></i> ${row.VoyageDTO.Vesselname}
                </span>
            </span>
        </span>`
        },

        originport: {
            title: "Origin",
            render: (d, row) => {
                const details = row.VoyageDTO.VoyageDetails;
                if (!details || details.length === 0) return '--';
                const origin = details[0];
                const destination = details[details.length - 1];
                const minDate = row.VoyageDTO.minDate;
                const maxDate = row.VoyageDTO.maxDate;
                return `
        <div class="voyage-ports">
            <div class="vport left">
                <img src="${flagpath}${origin.CountryFlag}" class="w-9 h-9 rounded-full">
                <div class="flex flex-col textnames">
                    <span class="textport">${origin.portname}, ${origin.CountryName}</span>
                    <span class="textcountry">${minDate.Text}</span>
                </div>
            </div>
            <img src="/assets/img/arrow-right.svg">
            <div class="vport right">               
                <div class="flex flex-col textnames">
                    <span class="textport">${destination.portname}, ${destination.CountryName}</span>
                    <span class="textcountry">${maxDate.Text}</span>
                </div>
                <img src="${flagpath}${destination.CountryFlag}" class="w-9 h-9 rounded-full">
            </div>           
        </div>`;
            }
        },

        date: {
            title: "date",
            render: (d, row) => {

                return `
                <div class="flex flex-col items-end gap-0.5">
                    <span class="text-xs text-gray-600">Transit Days</span>
                    <div class="voyage-days">
                        ${row.VoyageDTO.NoOfDays} Days
                    </div>
                </div>`;
            }
        },


        action: {
            title: 'Action',
            render: (data, row) => {
                return `
                    <div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">
                                
                                <div class="menu-item">
                                   <a class="menu-link"
                                   href="javascript:void(0);"
                                    onclick="OpenVoyageModal({
                                        voyageId: '${row.VoyageDTO.VoyageId.EncryptedValue}',
                                        mode: 'addport'
                                    })">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-plus"></i>
                                    </span>
                                    <span class="menu-title">Add Port</span>
                                </a>

                                </div>
                               <div class="menu-item">
                                <a class="menu-link"
                                   href="javascript:void(0);"
                                   onclick="OpenVoyageModal({
                                       voyageId: '${row.VoyageDTO.VoyageId.EncryptedValue}',
                                       mode: 'editvoyage'
                                   })">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-notepad-edit"></i>
                                    </span>
                                    <span class="menu-title">Edit Voyage</span>
                                </a>
                            </div>
                            </div>
                        </div>
                    </div>`;
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            }
        }
    }
};
const dataTableVoyage = new KTDataTable(
    document.querySelector("#TableVessels"),
    dataTableOptionsVoyage
);

dataTableVoyage.on('drew', () => {

    const $tableBody = $("#TableVessels tbody");
    $tableBody.find("tr").each(function () {
        const $row = $(this);

        if (!$row.hasClass("main-row")) {
            $row.addClass("main-row");
        }

        if (!$row.next().hasClass("expand-row-spacer")) {
            $row.after(
                '<tr class="expand-row-spacer"><td class="none" colspan="10"></td></tr>'
            );
        }
    });
    setTimeout(() => {
        $("#TableVessels .toggle-row").each(function () {

            const $btn = $(this);
            const $row = $btn.closest("tr");
            if (!$row.next().hasClass("expand-row")) {
                $btn.trigger("click");
            }
        });
    }, 0);
});
function ExpandCollapse() {
    $("#TableVessels").find(".toggle-row").trigger("click");

}
(function () {

    const TABLE = "#TableVessels";
    let expandedOnce = false;

    function getColCount() {
        return document.querySelectorAll(`${TABLE} thead th`).length || 5;
    }

    function buildExpandRowHtml(details) {
        return `
        <tr class="expand-row relative">
            <td class='none verticleline'></td>
            <td colspan="${getColCount()}" style='padding:0 20px 20px 20px !important;' class="p-0">
                <div class="voyage-details-container">
                    ${buildVoyageDetails(details)}
                </div>
            </td>
        </tr>
        
        `;
    }

    $(TABLE).on("click", ".toggle-row", function () {

        const $btn = $(this);
        const $row = $btn.closest("tr");
        const $next = $row.next();
        if ($next.hasClass("expand-row")) {
            $next.remove();
            $btn.html('<i class="ki-filled ki-plus"></i>');
            return;
        }
        const $dataRows = $row
            .closest("tbody")
            .children("tr.main-row");

        const rowIndex = $dataRows.index($row);
        const data = dataTableVoyage._data?.[rowIndex];
        if (!data || !data.VoyageDTO) return;
        const details = data.VoyageDTO.VoyageDetails || [];
        $row.after(buildExpandRowHtml(details));

        if ($row.next().next('.expand-row-spacer').length === 0) {
            $row.next().after(
                '<tr class="expand-row-spacer"><td class="none" colspan="10"></tr>'
            );
        }

        $btn.html('<i class="ki-filled ki-minus"></i>');
    });
    function autoExpandAll() {
        if (expandedOnce) return;

        const $buttons = $(`${TABLE} .toggle-row`);
        if (!$buttons.length) return;

        expandedOnce = true;
        $buttons.each(function () {
            $(this).trigger("click");
        });
    }

    const observer = new MutationObserver(() => {
        autoExpandAll();
    });

    observer.observe(document.querySelector(TABLE), {
        childList: true,
        subtree: true
    });

})();


function buildVoyageDetails(details) {
    if (!details || details.length === 0) {
        return `<div class="p-4 text-gray-500">No voyage details found.</div>`;
    }

    let html = `
        <table class="table table-auto table-border align-middle text-gray-700 font-medium vdetail text-sm w-100">
            <thead>
                <tr>

                    <th>Port</th>
                    <th class='text-center' style='width:150px;'>Status</th>
                    <th class="w-[250px]" style='width:225px;'>Arrival</th>
                    <th class="w-[250px]" style='width:225px;'>Departure</th>
                  
                    <th class="w-[50px] text-center">Action</th>
                </tr>
            </thead>
            <tbody>
    `;

    const fDate = (val) => {
        if (!val) return "-";

        const match = val.match(/\d+/);
        if (!match) return "-";

        const timestamp = parseInt(match[0]);
        const dt = new Date(timestamp);

        return typeof formatDateOnly === "function"
            ? formatDateOnly(dt)
            : dt.toLocaleDateString();
    };

    details.forEach(d => {

        const flag = d.CountryFlag
            ? `${flagpath}${d.CountryFlag}`
            : ``;

        var arrivallabel = "E";
        var arrivallabelclass = "expect";
        var arrival = d.ETA.Text;
        var arrivaltext = d.ETA.SubText;
        var arrivalnumeric = d.ETA.NumericValue;
        var arrivalred = "";

        var departurelabel = "E";
        var departurelabelclass = "expect";
        var departure = d.ETD.Text;
        var departuretext = d.ETD.SubText;
        var departurenumeric = d.ETD.NumericValue;
        var departurered = "";


        if (d.ATA.Text != "") {
            arrivallabel = "A";
            arrival = d.ATA.Text;
            arrivaltext = d.ATA.SubText;
            arrivallabelclass = "arrive";
            arrivalnumeric = d.ATA.NumericValue;
        }

        if (d.ATD.Text != "") {
            departurelabel = "A";
            departure = d.ATD.Text;
            departuretext = d.ATD.SubText;
            departurenumeric = d.ATD.NumericValue;
            departurelabelclass = "arrive";
        }
        if (arrivalnumeric < 0) {
            arrivalred = "red";
        }
        if (departurenumeric < 0) {
            departurered = "red";
        }
        let statusClass = "";

        if (d.PortStatus) {
            const status = d.PortStatus.toLowerCase();

            if (status.includes("scheduled")) {
                statusClass = "status-scheduled";
            }
            else if (status.includes("arrived")) {
                statusClass = "status-arrived";
            }
            else if (status.includes("departed")) {
                statusClass = "status-departed";
            }
        }

        html += `
            <tr class="${statusClass}">
                <td class="relative">
                    <span class='horizontalline'></span>
                    <div class="flex vdt-port-group items-center">
                            <img src="${flag}">
                            <div class="flex flex-col vdt-port">
                                <span class="vdt-port-name">
                                    <span class="vdt-port-name-text">${d.portname}</span>
                                    <span class="dot"></span>
                                    <span class="vdt-port-name-text">${d.Terminal}</span>
                                </span>
                                <span class="vdt-port-country">${d.CountryName}</span>
                            </div>
                        </div>
                </td>
                 <td class='text-center'>${d.PortStatus}</td>
                <td>

                    <div class="flex vdt-date-group ${arrivallabelclass}">
                        <span class='badgespan'>${arrivallabel}</span>
                        <div class="flex flex-col vdt-date">
                            <span class="vdt-date-text datespan">${arrival}</span>
                            <span class="vdt-date-subtext ${arrivalred}">${arrivaltext}</span>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="flex vdt-date-group ${departurelabelclass}">
                        <span class='badgespan'>${departurelabel}</span>
                        <div class="flex flex-col vdt-date">
                            <span class="vdt-date-text datespan">${departure}</span>
                            <span class="vdt-date-subtext ${departurered}">${departuretext}</span>
                        </div>
                    </div>
              
                </td>
                
               
                <td><div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">                                
                                <div class="menu-item">
                                <a class="menu-link"
                                   href="javascript:void(0);"
                                  onclick="OpenVoyageModal({
                                        voyageDetailId: '${d.VoyageDetailId.EncryptedValue}' 
                                    })">
                                        <span class="menu-icon">
                                            <i class="ki-filled ki-notepad-edit"></i>
                                        </span>
                                        <span class="menu-title">
                                            Edit
                                        </span>
                                    </a>
                                </div>
                                 <div class="menu-item">
                                        <a class="menu-link"
                                           href="javascript:void(0);"
                                          onclick="DisableVoyage('${d.VoyageDetailId.EncryptedValue}',
                                                           '${d.VoyageId.EncryptedValue}')">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-trash text-danger"></i>
                                    </span>

                            <span class="menu-title text-danger">
                                Delete
                            </span>
                            </a>
                        </div>
                        <div class="menu-item">
                                        <a class="menu-link"
                                           href="javascript:void(0);"
                                        onclick="OpenArrived({
                                        voyageId: '${d.VoyageId.EncryptedValue}',
                                        voyageDetailId: '${d.VoyageDetailId.EncryptedValue}'})">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-notepad-edit"></i>
                                    </span>
                            <span class="menu-title text-danger">
                                Mark As Arrived
                            </span>
                            </a>
                        </div>
                        <div class="menu-item">
                                        <a class="menu-link"
                                           href="javascript:void(0);"
                                          onclick="OpenDepatured({
                                            voyageId: '${d.VoyageId.EncryptedValue}',
                                            voyageDetailId: '${d.VoyageDetailId.EncryptedValue}' })">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-notepad-edit"></i>
                                    </span>
                            <span class="menu-title text-danger">
                                Mark As Departured
                            </span>
                            </a>
                        </div>
                          </div>
                        </div>
                    </div></td>
            </tr>
        `;
    });

    html += `
            </tbody>
        </table>
    `;

    return html;
}
$(document).ready(function () {
    if ($("#ddlVesselfilterID").length) {
        InitializeVesselDropdown($("#ddlVesselfilterID"), '');
    }
});
//function OpenVoyageModal(options = {}, agencyid) {
//    const defaults = {
//        voyageId: "",
//        voyageDetailId: "",
//        mode: ""
//    };
//    options = { ...defaults, ...options };

//    blockui();

//    $.get('/Voyage/Create', {
//        voyageId: options.voyageId,
//        voyageDetailId: options.voyageDetailId,
//        mode: options.mode
//    })
//        .done(function (html) {

//            unblockui();
//            const $modal = $("#modalsection").html(html);
//            if ($("#ddlVesselDetailID").length) {
//                InitializeVesselDropdown("#ddlVesselDetailID", '');
//            }
//            if ($("#ddlPOL").length) {
//                $("#ddlPOL").portSelect2({
//                    multiline: true,
//                    placeholder: $("#ddlPOL").data("placeholder-main"),
//                    subtextplaceholder: $("#ddlPOL").data("placeholder-sub"),
//                    icon: flagpath + 'pol.png'
//                });
//            }
//            if ($("#ddlVoyage").length && options.mode !== "addport") {
//                $("#ddlVoyage").voyageSelect2({
//                    placeholder: $("#ddlVoyage").data("placeholder-main")
//                });
//                if (!options.mode || options.mode === "create") {
//                    bindVoyageChange();
//                }
//                if ($modal.find(".datetimepicker-here").length > 0) {
//                    $modal.find(".datetimepicker-here").each(function () {
//                        var datatarget = $(this).data("trigger");
//                        var dataclear = $(this).data("clear");

//                        var datepicker = $(this).flatpickr({
//                            enableTime: true,
//                            dateFormat: "Y-m-d H:i",
//                            altInput: true,
//                            altFormat: "M j, Y h:i K"
//                        });
//                        if (datatarget != undefined) {
//                            document.getElementById(datatarget).addEventListener("click", function () {
//                                datepicker.open(); // Manually open the Flatpickr
//                            });
//                        }
//                        if (dataclear != undefined) {
//                            document.getElementById(dataclear).addEventListener("click", function () {
//                                datepicker.clear();
//                            });
//                        }
//                    }

//            //$modal.find(".datetimepicker-here").each(function () {
//            //    const $el = $(this);
//            //    const picker = flatpickr(this, {
//            //        enableTime: true,
//            //        dateFormat: "Y-m-d H:i",
//            //        altFormat: "M j, Y h:i K",
//            //        defaultDate: $el.val() ? new Date($el.val()) : null,
//            //        allowInput: true
//            //    });
//            //});
//            OpenModal("ModalAddVoyage");
//        })
//        .fail(function () {
//            unblockui();
//            ErrorMessage("Error loading voyage");
//        });
//}

function OpenVoyageModal(options = {}, agencyid) {
    const defaults = {
        voyageId: "",
        voyageDetailId: "",
        mode: ""
    };
    options = { ...defaults, ...options };

    blockui();

    $.get('/Voyage/Create', {
        voyageId: options.voyageId,
        voyageDetailId: options.voyageDetailId,
        mode: options.mode
    })
        .done(function (html) {

            unblockui();

            const $modal = $("#modalsection").html(html);

            if ($("#ddlVesselDetailID").length) {
                InitializeVesselDropdown("#ddlVesselDetailID", '');
            }

            if ($("#ddlPOL").length) {
                $("#ddlPOL").portSelect2({
                    multiline: true,
                    placeholder: $("#ddlPOL").data("placeholder-main"),
                    subtextplaceholder: $("#ddlPOL").data("placeholder-sub"),
                    icon: flagpath + 'pol.png'
                });
            }

            if ($("#ddlVoyage").length && options.mode !== "addport") {

                $("#ddlVoyage").voyageSelect2({
                    placeholder: $("#ddlVoyage").data("placeholder-main")
                });

                if (!options.mode || options.mode === "create") {
                    bindVoyageChange();
                }
            }

            if ($modal.find(".datetimepicker-here").length > 0) {
                $modal.find(".datetimepicker-here").each(function () {

                    const datatarget = $(this).data("trigger");
                    const dataclear = $(this).data("clear");
                    var mindatefield = $(this).data("min-date-field");


                    const datepicker = flatpickr(this, {
                        enableTime: true,
                        dateFormat: "Y-m-d H:i",
                        altInput: true,
                        altFormat: "M j, Y h:i K",
                        allowInput: true,
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

                    if (datatarget) {
                        document.getElementById(datatarget)?.addEventListener("click", function () {
                            datepicker.open();
                        });
                    }

                    if (dataclear) {
                        document.getElementById(dataclear)?.addEventListener("click", function () {
                            datepicker.clear();
                        });
                    }
                });
            }

            OpenModal("ModalAddVoyage");
        })
        .fail(function () {
            unblockui();
            ErrorMessage("Error loading voyage");
        });
}
//(function ($) {

//    $.fn.voyageSelect2 = function () {
//        return this.each(function () {
//            const $el = $(this);
//            $el.select2({
//                placeholder: "Search Voyage",
//                minimumInputLength: 1,
//                closeOnSelect: true,
//                selectOnClose: true,
//                tags: false,

//                ajax: {
//                    url: "/Voyage/GetVoyageSearch",
//                    dataType: "json",
//                    delay: 300,
//                    data: function (params) {
//                        return { search: params.term };
//                    },
//                    processResults: function (data) {
//                        return {
//                            results: data
//                        };
//                    },

//                    cache: true
//                },

//                templateResult: item => item.text,
//                templateSelection: item => item.text
//            });
//        });
//    };

//    if ($("#ddlVoyage").length) {
//        $("#ddlVoyage").voyageSelect2();
//        bindVoyageChange();
//    }

//})(jQuery);
function bindVoyageChange() {

    const $voyage = $("#ddlVoyage");
    const $vessel = $("#ddlVesselDetailID");
    const $comments = $("#txtComments");
    const $hiddenVoyage = $("#VoyageId_EncryptedValue");

    $voyage.off("select2:select");

    $voyage.on("select2:select", function (e) {

        const data = e.params.data;
        if (data.number && Number(data.number) > 0) {
            $hiddenVoyage.val(data.id);
            $vessel.prop("disabled", true);
            if (data.vesselId) {
                const option = new Option(
                    data.vesselName,
                    data.vesselId,
                    true,
                    true
                );

                $vessel
                    .empty()
                    .append(option)
                    .trigger("change");
            }
            $comments
                .val(data.comments || "")
                .prop("readonly", true);
        }
        else {
            $hiddenVoyage.val("");
            $vessel
                .prop("disabled", false)
                .val(null)
                .trigger("change");
            $comments
                .val("")
                .prop("readonly", false);
        }
    });
}

function SaveVoyage() {
    if (!$("#formVoyage").Validate()) {
        ErrorMessage("Some of the required fields are not filled");
        return;
    }
    const isEdit = $("#IsEditMode").val() === "true";
    const encryptedVoyageId = $("#VoyageId_EncryptedValue").val() || "";
    if (!isEdit) {
        const data = $("#ddlVoyage").select2("data");
        let voyageText = data[0].displaytext || "";
        $("#VoyageExtendedDTO_VoyageNumber").val(voyageText);
    }
    let url = "";
    if (isEdit) {
        url = "/Voyage/SaveVoyage";
    } else if (encryptedVoyageId !== "") {
        url = "/Voyage/AddIntermediatePort";
    } else {
        url = "/Voyage/SaveVoyage";
    }
    blockui();
    $.ajax({
        url: url,
        type: "POST",
        data: $("#formVoyage").serialize(),
        success: function (res) {
            unblockui();

            if (res?.ResultId === 1) {

                SuccessMessage(
                    url.includes("AddIntermediatePort")
                        ? "Intermediate port saved successfully!"
                        : "Voyage saved successfully!"
                );

                CloseModal("ModalAddVoyage");
                setTimeout(
                    function () {
                        window.location.href = window.location;
                    }, 500);

            } else {
                ErrorMessage(res?.ResultMessage || "Save failed");
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong!");
        }
    });
}
function DisableVoyage(detailId, voyageId) {

    ShowErrorConfirmation({
        title: 'Delete Voyage Details?',
        message: 'Are you sure you want to delete this voyage Details?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: () => {

            blockui();

            $.ajax({
                url: '/Voyage/DisableVoyage',
                type: 'POST',
                data: {
                    VoyageDetailId: { EncryptedValue: detailId },
                    VoyageId: { EncryptedValue: voyageId }
                },
                success: function (data) {
                    unblockui();

                    if (data.ResultId == "1") {
                        SuccessMessage("Voyage Details deleted successfully!");
                        setTimeout(
                            function () {
                                window.location.href = window.location;
                            }, 500);                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot delete voyage Details.");
                }
            });
        }
    });
}

function ExpandRow(current) {
    console.log(current);
}
function InitializeVesselDropdown(selector, agencyId) {
    const $el = $(selector);
    const multiple = $el.attr("multiple") === "multiple";

    $el.select2({
        placeholder: 'Enter Vessel Name to search',
        minimumInputLength: 2,
        ajax: {
            url: `/Vessel/GetVesselDropdownList?AgencyDetailID=${agencyId}&multiple=${multiple ? 1 : 0}`,
            dataType: 'json',
            delay: 250,
            processResults: function (data) {
                return { results: data };
            },
            cache: true
        }
    });

    const existingText = $el.attr("datatext");
    const existingValue = $el.attr("datavalue");
    if (existingText && existingValue) {
        const option = new Option(existingText, existingValue, true, true);
        $el.append(option).trigger('change');
    }
}
