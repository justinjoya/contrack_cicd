
function MakePrimary(refid, vendorid) {
    blockui();
    $.ajax({
        url: '/Vendor/MakeContactPrimary?refid=' + refid + '&vendorid=' + vendorid,
        type: 'GET',
        /*dataType: "json",*/
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#section_vendor_contacts").html(data);
            SuccessMessage("Updated successfully!");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Save Contact!");
        }
    });

}


function DeleteContact(refid, vendorid) {
    ShowErrorConfirmation({
        title: 'Delete Contact?',
        message: `Are you sure you want to delete this Contact?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Vendor/DeleteContact',
                type: 'GET',
                data: {
                    refid: refid,
                    vendorid: vendorid
                },
                success: function (data) {
                    unblockui();

                    $("#section_vendor_contacts").html(data);
                    SuccessMessage("Deleted successfully!");
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot delete contact!");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
    
}
