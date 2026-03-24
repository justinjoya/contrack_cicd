
function MakePrimary(refid, clientid) {

    blockui();
    $.ajax({
        url: '/Client/MakeContactPrimary?refid=' + refid + '&clientid=' + clientid,
        type: 'GET',
        /*dataType: "json",*/
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#section_client_contacts").html(data);
            SuccessMessage("Updated successfully!");
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Save Contact!");
        }
    });

}

function DeleteContact(refid, clientid) {
    ShowErrorConfirmation({
        title: 'Delete Contact?',
        message: `Are you sure you want to delete this contact?`,
        confirmtext: `Yes, Delete`,
        canceltext: `Cancel`,
        onConfirm: () => {
            blockui();
            $.ajax({
                url: '/Client/DeleteContact?refid=' + refid + '&clientid=' + clientid,
                type: 'GET',
                /*dataType: "json",*/
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    $("#section_client_contacts").html(data);
                    SuccessMessage("Deleted successfully!");
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Delete Contact!");
                }
            });
        },
        onCancel: () => {
            //$(current).prop('checked', !$(current).prop('checked'));
        }
    });
}