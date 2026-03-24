function OpenRoleModal() {
    blockui();
    $.ajax({
        url: '/Role/GetRoleModal',
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const modalEl = document.querySelector('#ModalRole');
            const modal = new KTModal(modalEl);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Error in Getting Role Modal");
        }
    });
}