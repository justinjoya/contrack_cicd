const apiUrl = domain + '/TableDisplay/JobTypes';
const element = document.querySelector('#TableJobTypeList');

const dataTableOptions = {
    apiEndpoint: apiUrl,
    pageSize: 10,
    columns: {
        jobtypename: {
            title: 'jobtypename',
            render: (data, row) => {
                if (row.jobtype.useasmaster) {
                    return '<a class="text-primary" href="/JobType/JobTypeDetails?refid=' + row.jobtype.jobtypeid.EncryptedValue + '">' + row.jobtype.jobtypename + '</a>';
                }
                return row.jobtype.jobtypename;
            },
        },
        useasmaster: {
            title: 'useasmaster',
            render: (data, row) => {
                if (row.jobtype.useasmaster) {
                    return `<span class="badge py-1 badge-fixed badge-primary badge-outline">Yes</span>`;
                } else {
                    return `<span class="badge py-1 badge-fixed badge-secondary badge-outline">No</span>`;
                }
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            },
        },
        lineitemcount: {
            title: 'lineitemcount',
            render: (data, row) => {
                return row.jobtype.useasmaster ? row.jobtype.lineitemcount : '-';
            },
            createdCell(cell) {
                cell.classList.add('text-center');
            }
        },
        jobtypeid: {
            title: 'Actions',
            render: (data, row) => {
                return row.menu.edit ? '<a onclick="OpenJobTypeModal(\'' + row.jobtype.jobtypeid.EncryptedValue + '\');" class="btn btn-sm btn-icon btn-clear btn-light" href="javascript:void(0);">' +
                    '<i class="ki-filled ki-notepad-edit"></i></a>' : "-";
            },
            createdCell: function (cell) {
                cell.classList.add('text-center', 'nowrap');
            }
        },
    },
};

const dataTable = new KTDataTable(element, dataTableOptions);
function OpenJobTypeModal(refid) {
    blockui();
    $.ajax({
        url: '/JobType/GetJobTypeModal?refid=' + refid,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const modalEl = document.querySelector('#ModalAddJobType');
            const modal = new KTModal(modalEl);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Job Type Details");
        }
    });
}

function SaveJobType() {
    var validated = $("#formJobType").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formJobType").serializeArray();
        $.ajax({
            url: '/JobType/SaveJobType',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                unblockui();
                if (data.ResultId == "1") {
                    SuccessMessage("Job Type saved successfully!");
                    const modalEl = document.querySelector('#ModalAddJobType');
                    const modal = KTModal.getInstance(modalEl);
                    modal.hide();
                    if (dataTable) {
                        dataTable.reload();
                    }
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Save Job Type!");
            }
        });
    }
}