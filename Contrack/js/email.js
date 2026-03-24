var ckClassicEditor;
ClassicEditor
    .create(
        document.querySelector('#editor1')
    ).then(editor => {
        ckClassicEditor = editor; // Store the editor instance
    })
    .catch(error => {
        console.error(error);
    });

if ($(".searchableemail").length > 0) {
    $('.searchableemail').each(function () {
        var current = $(this);
        EmailSearch(current);
    });

}

function EmailSearch(current) {
    current.select2({
        placeholder: 'Enter Email to search',
        minimumInputLength: 1,
        ajax: {
            url: '/Email/GetEmailAddress',
            dataType: 'json',
            delay: 250,
            processResults: function (data) {
                return {
                    results: data,
                    pagination: false
                };
            },
            cache: true
        }
    });
}



$(function () {
    $('#frmEmailCompose').on('submit', function (e) {

        var saveaction = "";
        const submitter = e.originalEvent?.submitter;

        if (submitter) {
            const $btn = $(submitter);
            saveaction = $btn.val();
        }

        if (saveaction === "Download" || saveaction === "Email") {
            return;
        }
        else {
            $('#btnSendEmail').prop('disabled', true);
            e.preventDefault();

            var validated = $("#frmEmailCompose").Validate();
            if (validated) {
                blockui();
                var formdata = $("#frmEmailCompose").serializeArray();
                $.ajax({
                    url: '/Email/Compose?saveaction=' + saveaction,
                    data: formdata,
                    type: 'POST',
                    //dataType: "json",
                    //contentType: "application/json",
                    success: function (data) {
                        unblockui();
                        if (data.ResultId == 1) {
                            SuccessMessage("Email Sent Successfully!");
                            setTimeout(
                                function () {
                                    window.location.href = data.ResultMessage;
                                }, 500);
                        }
                        else {
                            ErrorMessage(data.ResultMessage);
                        }
                        $('#btnSendEmail').prop('disabled', false);
                    },
                    error: function (data) {
                        unblockui();
                        ErrorMessage("Cannot Save Email");
                        $('#btnSendEmail').prop('disabled', false);
                    }
                });
            }
            else {
                ErrorMessage("Some of required items needs to be filled");
                $('#btnSendEmail').prop('disabled', false);
            }
        }
    });
});