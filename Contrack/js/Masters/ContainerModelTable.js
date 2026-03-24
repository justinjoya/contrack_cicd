let debounceTimer;

$(document).ready(function () {
    initContainerModelTable();
});

const apiUrl = domain + '/TableDisplay/ContainerModels';
const element = document.querySelector('#TableContainerModels');
let dataTable;

function initContainerModelTable() {
    if (!element) return;

    const dataTableOptions = {
        apiEndpoint: apiUrl,
        pageSize: 20,
        stateSave: false,
        sortable: true,
        columns: {

            typename: {
                title: 'Type',
                sortable: true,
                sortField: 'typename',
                render: (data, row) => `
                    <span class='flex gap-3 items-center'>
                        <span class="text-gray-900  font-semibold">
                            ${row.model.typename}
                        </span>
                    </span>`
            },
            sizename: {
                title: 'Size',
                sortable: true,
                sortField: 'sizename',
                render: (data, row) => `
                        <span class="mx-2 py-1 badge badge-outline badge-primary badge-md">
                            ${row.model.sizename}
                        </span>`,
                createdCell(cell) {
                    cell.classList.add('text-center');
                },
            },
            iso_code: {
                title: 'ISO Code',
                sortable: true,
                sortField: 'iso_code',
                render: (data, row) =>
                    `<span class="text-gray-700 font-semibold sans-font1">
                        ${row.model.iso_code}
                    </span>`
            },
            total: {
                title: 'Total',
                sortable: true,
                sortField: 'total',
                render: (data, row) => `<span class="statcount ${row.model.TotalCount == 0 ? 'empty' : ''}"> ${row.model.TotalCount}  </span>`,
                createdCell(cell) {
                    cell.classList.add('text-center');
                }
            },
            available: {
                title: 'Available',
                sortable: true,
                sortField: 'available',
                render: (data, row) => datawithprogressbar(row.model.TotalCount, row.model.AvailableCount, "statcount green"),
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            blocked: {
                title: 'Blocked',
                sortable: true,
                sortField: 'blocked',
                render: (data, row) => datawithprogressbar(row.model.TotalCount, row.model.BlockedCount, "statcount gray"),
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            booked: {
                title: 'Booked',
                sortable: true,
                sortField: 'booked',
                render: (data, row) => datawithprogressbar(row.model.TotalCount, row.model.BookedCount, "statcount yellow"),
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            repair: {
                title: 'Repair',
                sortable: true,
                sortField: 'repair',
                render: (data, row) => datawithprogressbar(row.model.TotalCount, row.model.RepairCount, "statcount red"),
                createdCell(cell) {
                    cell.classList.add('text-end');
                }
            },
            action: {
                title: 'Action',
                sortable: false,
                render: (data, row) => {

                    if (row.menu && row.menu.edit) {
                        return `
                    <div class="menu justify-center" data-menu="true">
                        <div class="menu-item menu-item-dropdown" data-menu-item-offset="0, 10px" data-menu-item-placement="bottom-end" data-menu-item-placement-rtl="bottom-start" data-menu-item-toggle="dropdown" data-menu-item-trigger="click|lg:click">
                            <button class="menu-toggle btn btn-sm btn-icon btn-light btn-clear">
                                <i class="ki-filled ki-dots-vertical"></i>
                            </button>
                            <div class="menu-dropdown menu-default w-full max-w-[200px]" data-menu-dismiss="true">
                                
                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="OpenContainerModelModal('${row.model.modelid.EncryptedValue}')">
                                        <span class="menu-icon">
                                            <i class="ki-filled ki-notepad-edit"></i>
                                        </span>
                                        <span class="menu-title">
                                            Edit
                                        </span>
                                    </a>
                                </div>

                                <div class="menu-item">
                                    <a class="menu-link" href="javascript:void(0);" onclick="DeleteContainerModel('${row.model.modelid.EncryptedValue}')">
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
                    cell.classList.add('text-center', 'nowrap');
                }
            }
        }
    };

    dataTable = new KTDataTable(element, dataTableOptions);

    dataTable.on('draw', () => {
        if (dataTable._data.length === 0) {
            $('.custom-dt').hide();
            $('.emptydatatable').show();
        } else {
            $('.emptydatatable').hide();
            $('.custom-dt').show();
        }
    });
}

function toggleActionMenu(btn) {
    $('.action-menu').addClass('hidden');
    $(btn).siblings('.action-menu').toggleClass('hidden');
}

$(document).on('click', function (e) {
    if (!$(e.target).closest('.action-menu, .btn-icon').length) {
        $('.action-menu').addClass('hidden');
    }
});

function OpenContainerModelModal(modelid = '', typeid = '', sizeid = '') {
    blockui();

    $.ajax({
        url: '/ContainerModel/GetContainerModelModal',
        type: 'GET',
        data: {
            modelid: modelid,
            typeid: typeid,
            sizeid: sizeid
        },
        success: function (html) {
            unblockui();
            $('#ModalContainerModel').html(html);
            OpenModal('ModalAddContainerModel');
        },
        error: function () {
            unblockui();
            ErrorMessage('Unable to load container model.');
        }
    });
}

function SaveContainerModel() {
    var validated = $("#formContainerModel").Validate();
    if (!validated) return;
    blockui();
    var formdata = $("#formContainerModel").serialize();
    $.ajax({
        url: '/ContainerModel/SaveContainerModel',
        type: 'POST',
        data: formdata,
        dataType: "json",
        success: function (data) {
            unblockui();
            if (data.ResultId == "1") {
                SuccessMessage('Container Model Updates successfully.');
                CloseModal("ModalAddContainerModel");
                setTimeout(function () {
                    window.location.href = window.location;
                }, 400);
            } else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage('Cannot delete container model.');
        }
    });
}

function DeleteContainerModel(encryptedId) {
    ShowErrorConfirmation({
        title: 'Delete Container Model?',
        message: 'Are you sure you want to delete this container model?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: function () {

            blockui();

            $.ajax({
                url: '/ContainerModel/DeleteContainerModel',
                type: 'POST',
                data: { containermodelid: encryptedId },
                dataType: "json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == "1") {
                        SuccessMessage('Container Model deleted successfully.');
                        setTimeout(function () {
                            window.location.href = window.location;
                        }, 400);
                    } else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage('Cannot delete container model.');
                }
            });
        }
    });
}
