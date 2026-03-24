function OpenJobTypeDetailModal(refid, jobtypeid) {
    blockui();
    $.ajax({
        url: '/JobType/GetJobTypeDetailModal?refid=' + refid + '&jobtypeid=' + jobtypeid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();

            const jobDetailsModal = document.querySelector('#ModalAddJobTypeDetail');
            const modal = new KTModal(jobDetailsModal);
            modal.show();
        },
        error: function () {
            unblockui();
            ErrorMessage("Error in Getting Job Type Detail");
        }
    });
}

function SaveJobTypeDetail() {
    var validated = $("#formJobTypeDetail").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formJobTypeDetail").serialize();
        $.ajax({
            url: '/JobType/SaveJobTypeDetails',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Job Type Details Updated Successfully!");

                    const ModalAddJobTypeDetails = document.querySelector('#ModalAddJobTypeDetail');
                    const modal = KTModal.getInstance(ModalAddJobTypeDetails);
                    if (modal) { 
                        modal.hide();
                    }

                    setTimeout(function () {
                        window.location.href = data.ResultMessage;
                    }, 500);
                } else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Job Type Details!");
            }
        });
    }
}

$(document).ready(function () {
    $('#jobTypeDetailsTable').on('click', '.delete-detail-btn', function () {
        const $button = $(this);
        const refid = $button.data('refid');
        const parentRefId = $button.data('parent-refid');

        ShowErrorConfirmation({
            title: 'Delete Job type?',
            message: `Are you sure you want to delete this Job type?`,
            confirmtext: `Yes, Delete`,
            canceltext: `Cancel`,
            onConfirm: () => {
                blockui();
                $.ajax({
                    url: '/JobType/DeleteJobTypeDetails',
                    type: 'POST',
                    data: {
                        detailRefId: refid,
                        parentRefId: parentRefId
                    },
                    success: function (data) {
                        unblockui();
                        if (data.ResultId === 1 || data.ResultId === "1") {
                            SuccessMessage("Item deleted successfully!");
                            $button.closest('tr').fadeOut(400, function () {
                                $(this).remove();
                                if ($('#jobTypeDetailsTable tbody tr').length === 0) {
                                    const noDetailsRow = '<tr id="no-details-row"><td colspan="2" class="text-center text-gray-500 p-4">No details have been added.</td></tr>';
                                    $('#jobTypeDetailsTable tbody').append(noDetailsRow);
                                }
                            });
                        } else {
                            ErrorMessage(data.ResultMessage);
                        }
                    },
                    error: function () {
                        unblockui();
                        ErrorMessage("Cannot Delete Job Type Detail");
                    }
                });
            },
            onCancel: () => {
                //$(current).prop('checked', !$(current).prop('checked'));
            }
        });
    });
});