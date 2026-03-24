const apiUrl = domain + '/TableDisplay/BookingLists';
const element = document.querySelector('#TableContainerBooking');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    /*pageSizes: [5, 10, 20, 30, 50],*/
    pageSize: 20,
    columns: {
        bookingno: {
            title: 'bookingno',
            render: (data, row) => {
                return `<div class="flex flex-col gap-0.5">
                <a class="flex gap-3 items-center text-primary primary-col font-semibold1 text-sm"href="/Booking/BookingDecider?refid=${row.containerbookinglist.bookinguuid}">
                      ${row.containerbookinglist.bookingno}
               </a>
              <span data-tooltip="#tooltip_Agency_` + row.containerbookinglist.bookinguuid + `" class="text-xs text-gray-700 font-normal ellipsis">${row.containerbookinglist.agencyname} </span
              </div>
              <div class="tooltip" id="tooltip_Agency_` + row.containerbookinglist.bookinguuid + `">
                <div class="flex  items-center gap-1">` + row.containerbookinglist.agencyname + `</div>
              </div>`;
            }
        },
        customername: {
            title: 'Customer',
            render: (data, row) => {
                const item = row.containerbookinglist;
                const tooltipId = `cust_tooltip_${item.bookinguuid}`;

                return `
                    <div class="flex flex-col min-w-0">
                        <span class="ellipsis text-gray-900 font-semibold1 text-sm cursor-pointer" 
                              data-tooltip="#${tooltipId}">
                            ${item.customername}
                        </span>
                        <span class="text-gray-600 text-2sm">
                           ${item.customertypename}
                        </span>

                        <div class="tooltip hidden" id="${tooltipId}">
                            <div class="flex items-center gap-1">
                                ${item.customername}
                                <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                            </div>
                        </div>
                    </div>
                `;
            },
            createdCell(cell) {
                cell.classList.add('max-w-[200px]');
            }
        },

        pol: {
            title: 'POL',

            render: (data, row) => {
                const pol = row.containerbookinglist;
                let flagHtml = '';

                if (pol.pol_countryflag && typeof flagpath !== 'undefined') {
                    flagHtml = ` <img src="${flagpath}${pol.pol_countryflag}"class="w-8 h-8 rounded-full shadow-sm" onerror="this.style.display='none'">`;
                }
                const tooltipId = `pol_tooltip_${pol.pol?.EncryptedValue || row.row_index}`;
                return `<div class="flex gap-2.5 items-center">
                 ${flagHtml}
              <div class="flex flex-col gap-0.5 min-w-0">
                   <span  class="ellipsis text-sm text-gray-900 font-semibold1"data-tooltip="#${tooltipId}">${pol.pol_portname}</span>
                    <span class="ellipsis text-2sm text-gray-600" data-tooltip="#${tooltipId}"> ${pol.pol_countryname}</span>
             </div>
             <div class="tooltip hidden" id="${tooltipId}" style="z-index:100">
                <div class="flex flex-col gap-0.5">
                    <span class="font-semibold1">${pol.pol_portname}</span>
                    <span class="text-xs text-gray-500">${pol.pol_countryname}</span>
               </div>
             </div>

             </div>`;
            },

            createdCell(cell) {
                cell.classList.add('max-w-[220px]', 'align-top');
            }
        },

        pod: {
            title: 'POD',

            render: (data, row) => {
                const pod = row.containerbookinglist;
                let flagHtml = '';

                if (pod.pod_countryflag && typeof flagpath !== 'undefined') {
                    flagHtml = ` <img src="${flagpath}${pod.pod_countryflag}"class="w-8 h-8 rounded-full shadow-sm" onerror="this.style.display='none'">`;
                }
                const tooltipId = `pod_tooltip_${pod.pod?.EncryptedValue || row.row_index}`;
                return `<div class="flex gap-2.5 items-center">
                 ${flagHtml}
              <div class="flex flex-col gap-0.5 min-w-0">
                   <span  class="ellipsis text-sm text-gray-900 font-semibold1"data-tooltip="#${tooltipId}">${pod.pod_portname}</span>
                    <span class="ellipsis text-2sm text-gray-600"data-tooltip="#${tooltipId}"> ${pod.pod_countryname}</span>
             </div>
             <div class="tooltip hidden" id="${tooltipId}" style="z-index:100">
                <div class="flex flex-col gap-0.5">
                    <span class="font-semibold1">${pod.pod_portname}</span>
                    <span class="text-xs text-gray-500">${pod.pod_countryname}</span>
               </div>
             </div>

             </div>`;
            },

            createdCell(cell) {
                cell.classList.add('max-w-[220px]', 'align-top');
            }
        },

        voyagename: {
            title: 'Voyage',
            render: (data, row) => {
                const item = row.containerbookinglist;
                const tooltipId = `voyage_tooltip_${item.bookinguuid}`;

                return `
                    <div class="flex flex-col gap-0.5 min-w-0">
                        <span class="ellipsis text-gray-900 font-semibold1 text-sm cursor-pointer" 
                              data-tooltip="#${tooltipId}">
                            ${item.voyagenumber}
                        </span>
                        <span class="text-gray-600 text-2sm ellipsis">
                            ${item.vesselname}
                        </span>

                        <div class="tooltip hidden" id="${tooltipId}">
                            <div class="flex flex-col gap-0.5">
                                <div class="flex items-center gap-1">
                                     ${item.voyagenumber}
                                     <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                                </div>
                                <div class="text-xs text-gray-400">${item.vesselname}</div>
                            </div>
                        </div>
                    </div>`;
            },
            createdCell(cell) {
                cell.classList.add('max-w-[150px]');
            }
        },


        containerInfo: {
            title: 'Containers / Qty',
            render: (data, row) => {

                var details = row.containerbookinglist.booking_details || "";
                if (details == "")
                    return "";
                var list = details.split(",").map(x => x.trim());
                var tooltipId = "container_tooltip_" + row.containerbookinglist.bookinguuid;

                var firstItem = list[0];
                var extraCount = list.length - 1;

                var html = `
              <div class="flex items-center gap-2 justify-center1 whitespace-nowrap">
                 <div  class="py-1 px-2 text-gray-900 text-xs font-semibold1 badge badge-outline badge-secondary flex items-center gap-1.5"
                   style=""
                   title="${firstItem.replace('*', ' x ')}">
                    <i class="ki-filled ki-cube-2 text-sm flex-shrink-0"></i>
                    <span class="truncate">
                      ${firstItem.replace('*', ' x ')}
                    </span>
                 </div>
               ${extraCount > 0 ? `<div class="text-primary text-xs font-semibold1 flex-shrink-0">+${extraCount}</div>` : ""}
              </div>`;

                var tooltipHtml = list.map(item => `
                <div class="flex items-center gap-1 text-xs">
                <i class="ki-filled ki-cube-2 text-sm"></i>
                ${item.replace('*', ' x ')}
               </div>
        `).join("");

                if (list.length > 1) {
                    return `
                <span data-tooltip="#${tooltipId}">
                    ${html}
                </span>
                <div class="tooltip" id="${tooltipId}">
                    <div class="flex flex-col gap-1">
                        ${tooltipHtml}
                        <i class="ki-solid ki-information-5 text-lg text-warning"></i>
                    </div>
                </div>`;
                }

                return html;
            }
        },

        createdat: {
            title: 'Created At / By',
            render: (data, row) => {
                return `
            <div class="flex flex-col gap-0.5">
                <span class="text-gray-900 font-semibold1 text-sm">
                    ${row.containerbookinglist.createdat.Text}
                </span>
                <span class="text-gray-600 text-2sm">
                    By ${row.containerbookinglist.createdbyusername}
                </span>
            </div>
        `;
            },
        },
        status: {
            title: 'Status',
            render: (data, row) => {
                return `${row.containerbookinglist.status.SubText}`;
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },

        //action: {
        //    title: 'Action',
        //    render: (data, row) => {
        //        if (row.menu && row.menu.edit) {
        //            return `
        //            <div class="menu justify-center" data-menu="true">
        //                <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
        //                    <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
        //                        <i class="ki-filled ki-dots-vertical"></i>
        //                    </button>
        //                    <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">
        //                        <div class="menu-item">
        //                            <a class="menu-link" href="javascript:void(0);" onclick="DisableLocation('${row.containerbookinglist.bookingid.EncryptedValue}', '${row.containerbookinglist.bookinguuid}')">
        //                            <span class="menu-icon">
        //                                     <i class="ki-filled ki-trash text-danger"></i>
        //                                </span>
        //                                <span class="menu-title text-danger">
        //                                   Delete
        //                                </span>
        //                            </a>
        //                        </div>

        //                    </div>
        //                </div>
        //            </div>`;
        //        }
        //        return '<span class="text-gray-400">-</span>';
        //    },
        //    createdCell(cell) {
        //        cell.classList.add('text-center');
        //    }
        //}

        action: {
            title: 'Action',
            render: (data, row) => {
                if (row.menu && row.menu.edit) {
                    const encryptedId = row.containerbookinglist.bookingid.EncryptedValue;
                    const bookingUuid = row.containerbookinglist.bookinguuid;
                    // Generate SHIPMENT code
                    const generatedCode = "SHIPMENT_" + crypto.randomUUID();
                    const shipmentUrl = `/Download/PDFDownload?refid=${bookingUuid}&type=shipmentconfirm&code=${generatedCode}&Download=0`;
                    return `
                <div class="menu justify-center" data-menu="true">
                    <div class="menu-item menu-item-dropdown"
                         data-menu-item-offset="0, 10px"
                         data-menu-item-placement="bottom-end"
                         data-menu-item-placement-rtl="bottom-start"
                         data-menu-item-toggle="dropdown"
                         data-menu-item-trigger="click|lg:click">

                        <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                            <i class="ki-filled ki-dots-vertical"></i>
                        </button>

                        <div class="menu-dropdown menu-default w-full max-w-[250px]" data-menu-dismiss="true">

                            <!-- SUMMARY -->                                
                            <div class="menu-item">
                                <a class="menu-link"
                                   href="javascript:void(0);"
                                   onclick="OpenBookingSummaryModal('${bookingUuid}')">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-document"></i>
                                    </span>
                                    <span class="menu-title">
                                        Download Summary
                                    </span>
                                </a>
                            </div>

                           

                            <!-- SHIPMENT CONFIRM -->
                            <div class="menu-item">
                                <a class="menu-link"
                                   href="/Booking/ShipmentConfirmation?refid=${bookingUuid}"
                                   target="_blank">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-document"></i>
                                    </span>
                                    <span class="menu-title text-left">
                                        Download Shipment Confirmation & Cabotage
                                    </span>
                                </a>
                            </div>

                            <!-- DELETE -->
                            <div class="menu-item">
                                <a class="menu-link"
                                   href="javascript:void(0);"
                                   onclick="DisableLocation('${encryptedId}', '${bookingUuid}')">
                                    <span class="menu-icon">
                                        <i class="ki-filled ki-trash text-danger"></i>
                                    </span>
                                    <span class="menu-title text-danger">
                                        Delete
                                    </span>
                                </a>
                            </div>

                        </div>
                    </div>
                </div>`;
                }

                return '<span class="text-gray-400">-</span>';
            },

            createdCell(cell) {
                cell.classList.add('text-center');
            }
        }

    },
};

const dataTable = new KTDataTable(element, dataTableOptions);

$(document).ready(function () {

    $("#ddlPOLFilter").portSelect2();
    $("#ddlPODFilter").portSelect2();

});

// Modal
window.ckClassicEditors = window.ckClassicEditors || {};
function initClassicEditor(editorId) {
    const el = document.getElementById(editorId);
    if (!el) return;

    if (window.ckClassicEditors[editorId]) {
        window.ckClassicEditors[editorId]
            .destroy()
            .then(() => {
                delete window.ckClassicEditors[editorId];
                createEditor();
            })
            .catch(error => {
                console.error("Error destroying editor:", error);
                createEditor();
            });
    } else {
        createEditor();
    }

    function createEditor() {
        ClassicEditor
            .create(el)
            .then(editor => {
                window.ckClassicEditors[editorId] = editor;
            })
            .catch(error => {
                console.error("Error initializing editor:", error);
            });
    }
}

function OpenBookingSummaryModal(bookinguuid) {
    blockui();
    $.ajax({
        url: '/Booking/GetBookingSummaryModal',
        type: 'GET',
        data: { bookinguuid: bookinguuid },
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalBookingSummary");
            setTimeout(function () {
                initClassicEditor('editor1');
            }, 100);
        },
        error: function () {
            unblockui();
            ErrorMessage("Failed to load booking summary.");
        }
    });
}

function toggleNA(checkbox, fieldId) {
    var wrapper = document.getElementById("wrap_" + fieldId);
    if (!wrapper) return;

    if (checkbox.checked) {
        wrapper.style.display = "none";
        var textarea = document.getElementById(fieldId);
        if (textarea) textarea.value = "";
    }
    else {
        wrapper.style.display = "block";
    }
}

function SaveBookingSummary() {
    var validated = $("#formBookingSummary").Validate();
    if (!validated) {
        ErrorMessage("Please fill required fields");
        return;
    }

    $("#formBookingSummary textarea").each(function () {
        if ($(this).val() === "N/A") {
            $(this).val("");
        }
    });

    if (window.ckClassicEditors["editor1"]) {
        var editorData = window.ckClassicEditors["editor1"].getData();
        $("#editor1").val(editorData);
    }
    blockui();
    var formData = $("#formBookingSummary").serialize();
    $.ajax({
        url: '/Booking/SaveBookingSummary',
        type: 'POST',
        data: formData,
        success: function (data) {
            unblockui();
            if (data.ResultId === 1) {
                CloseModal("ModalBookingSummary");
                LoadingMessage("Downloading. Please wait...");
                setTimeout(function () {
                    window.location.href = data.ResultMessage;
                }, 300);
            } else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Something went wrong");
        }
    });
}

function GetCabatoge(bookinguuid) {
    blockui();
    $.ajax({
        url: '/Booking/GetCabatoge?refid=' + bookinguuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal('ModalCabotage');
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}
