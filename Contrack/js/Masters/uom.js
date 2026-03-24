const apiUrl = domain + '/TableDisplay/UoM';
const element = document.querySelector('#TableUnitofMeasure');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    pageSize: 10,
    columns: {
        uomname: {
            title: 'uomname',
        },
        uomid: {
            title: 'Actions',
            render: (data, row) => {
                console.log('Row data:', row);
                const encryptedId = row?.uomid?.EncryptedValue ?? '';
                return `<a onclick="DeleteUoM('${encryptedId}');" 
                           class="btn btn-sm btn-icon btn-clear btn-light" 
                           href="javascript:void(0);">
                            <i class="ki-filled ki-trash"></i>
                        </a>`;
            },
            createdCell: function (cell) {
                cell.classList.add('text-center', 'nowrap');
            }
        },
    },
};

const dataTable = new KTDataTable(element, dataTableOptions);
function OpenUoMModal() {
    blockui();
    $.ajax({
        url: '/UoM/GetUoMModal',
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalUoM");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Unit of Measure");
        }
    });
}

function SaveUoM() {
    var validated = $("#formUoM").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formUoM").serializeArray();
        $.ajax({
            url: '/UoM/SaveUoM',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Unit of Measure saved successfully!");
                    CloseModal("ModalUoM");
                    setTimeout(
                        function () {
                            window.location.href = data.ResultMessage;
                        }, 200);
                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);

                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Unit of Measure!");
            }
        });
    }
}

function DeleteUoM(refid) {
    ShowErrorConfirmation({
        title: 'Delete UOM?',
        message: `Are you sure you want to delete this UOM?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/UoM/DeleteUoM',
                type: 'POST',
                data: { encryptedId: refid },
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage("Unit of Measure Deleted Successfully!");
                        setTimeout(() => {
                            window.location.href = '/UoM/List';
                        }, 200);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot Delete Unit of Measure");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}


