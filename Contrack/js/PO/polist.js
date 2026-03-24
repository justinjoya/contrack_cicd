$(document).ready(function () {
    if ($('.pobuttons').length > 0) {
        $('.checksingle').change(function () {
            var anyChecked = $('.checksingle:checked').length > 0;
            if (anyChecked) {
                $('.pobuttons').prop('disabled', false);
            } else {
                $('.pobuttons').prop('disabled', true);
            }
        });
    }
});


function SendPOEmails() {
    blockui("Sending mail...");
    var count = $("#pi_po_list").find("tbody input[type='checkbox']:checked").length;
    if (count == 1) {
        var refid = $("#pi_po_list").find("tbody input[type='checkbox']:checked").val();
        unblockui();
        window.location = "/Email/Compose?refid=" + refid + "&type=PO";
    }
    else {
        var runningcount = 0;
        $("#pi_po_list").find("tbody input[type='checkbox']:checked").each(function () {
            var uuid = $(this).val();
            $.ajax({
                url: '/EmailSchedular/POEmail?refid=' + uuid,
                type: "GET",
                async: true,
                success: function (data) {
                    runningcount++;
                    if (runningcount >= count) {
                        unblockui();
                        SuccessMessage("Email Sent Successfull!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }

                },
                error: function (err) {
                    runningcount++;
                    if (runningcount >= count) {
                        unblockui();
                        ErrorMessage("There is some problem in sending email");
                    }
                    //ErrorMessage("Cannot upload files");
                }
            });
        });
    }
}