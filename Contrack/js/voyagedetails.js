
function OpenArrived(options = {}) {

    const defaults = {
        voyageId: "",
        voyageDetailId: ""
    };

    options = { ...defaults, ...options };

    blockui();

    $.get('/Voyage/MarkAsArrived', {
        voyageId: options.voyageId,
        voyageDetailId: options.voyageDetailId
    })
        .done(function (html) {

            unblockui();

            const $modal = $("#modalsection").html(html);

            $modal.find(".datetimepicker-here").each(function () {

                const $el = $(this);
                const value = $el.val(); 

                flatpickr(this, {
                    enableTime: true,
                    dateFormat: "Y-m-d H:i",     
                    altInput: true,
                    altFormat: "M j, Y h:i K",  
                    defaultDate: value
                        ? flatpickr.parseDate(value, "M d, Y h:i K")
                        : null,
                    allowInput: false
                });
            });

            OpenModal("ModalAddArrived");
        })
        .fail(function () {
            unblockui();
            ErrorMessage("Error loading voyage");
        });
}


function SaveArrived() {
    var validated = $("#formArrived").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formArrived").serializeArray();
        $.ajax({
            url: '/Voyage/MarkAsArrived',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                unblockui();
                if (data.ResultId == "1") {
                    SuccessMessage(data.ResultMessage);
                    const modal = KTModal.getInstance(document.querySelector('#ModalAddArrived'));
                    if (modal) modal.hide();
                    setTimeout(function () { window.location.reload(); }, 500);
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot Save Arrived!");
            }
        });
    } else {
        ErrorMessage("Please fill all required fields.");
    }
}

function OpenDepatured(options = {}) {

    const defaults = {
        voyageId: "",
        voyageDetailId: ""
    };

    options = { ...defaults, ...options };

    blockui();

    $.get('/Voyage/MarkAsDepartured', {
        voyageId: options.voyageId,
        voyageDetailId: options.voyageDetailId
    })
        .done(function (html) {

            unblockui();

            const $modal = $("#modalsection").html(html);
            $modal.find(".datetimepicker-here").each(function () {

                const $el = $(this);
                const value = $el.val(); 

                flatpickr(this, {
                    enableTime: true,
                    dateFormat: "Y-m-d H:i",   
                    altInput: true,
                    altFormat: "M j, Y h:i K",  
                    defaultDate: value
                        ? flatpickr.parseDate(value, "M d, Y h:i K")
                        : null,
                    allowInput: false
                });
            });

            OpenModal("ModalAddDepartured");
        })
        .fail(function () {
            unblockui();
            ErrorMessage("Error loading voyage");
        });
}

function SaveDepatured() {
    var validated = $("#formDepature").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formDepature").serializeArray();
        $.ajax({
            url: '/Voyage/MarkAsDepartured',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                unblockui();
                if (data.ResultId == "1") {
                    SuccessMessage(data.ResultMessage);
                    const modal = KTModal.getInstance(document.querySelector('#ModalAddDepartured'));
                    if (modal) modal.hide();
                    setTimeout(function () { window.location.reload(); }, 500);
                } else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot Save Arrived!");
            }
        });
    } else {
        ErrorMessage("Please fill all required fields.");
    }
}
