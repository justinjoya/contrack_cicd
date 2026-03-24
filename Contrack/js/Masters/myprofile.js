function OpenEditProfileModal() {
    blockui();
    $.ajax({
        url: '/Profile/GetEditProfileModal',
        type: 'GET',
        success: function (result) {
            unblockui();
            $('#modalsection').html(result);

            const editModalEl = document.querySelector('#EditProfileModal');
            if (editModalEl) {
                const modal = new KTModal(editModalEl);
                attachModalDismissListeners(editModalEl, modal);
                modal.show();
            }
        },
        error: function () {
            unblockui();
            ErrorMessage('Failed to load the edit profile form.');
        }
    });
}

function SaveChanges() {
    var validated = $("#EditProfileForm").Validate();
    if (validated) {
        blockui();
        $.ajax({
            url: '/Profile/UpdateProfile',
            type: 'POST',
            data: $("#EditProfileForm").serialize(),
            success: function (response) {
                unblockui();
                const myModalEl = document.getElementById('EditProfileModal');
                const modal = KTModal.getInstance(myModalEl);
                if (modal) {
                    modal.hide();
                }

                if (response.success) {
                    SuccessMessage(response.message);
                    setTimeout(function () {
                        location.reload();
                    }, 1500);

                } else {
                    ErrorMessage(response.message);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("An unexpected error occurred. Please try again.");
            }
        });
    }
}

function OpenChangePasswordModal() {
    blockui();
    $.ajax({
        url: '/Profile/GetChangePasswordForm',
        type: 'GET',
        success: function (data) {
            unblockui();
            $('#modalsection').html(data);

            const modalEl = document.querySelector('#ModalChangePassword');
            if (modalEl) {
                const modal = new KTModal(modalEl);
                attachModalDismissListeners(modalEl, modal);
                modal.show();
            }
        },
        error: function (xhr, status, error) {
            unblockui();
            ErrorMessage("Could not load the change password form.");
            console.error("Error loading form: ", error);
        }
    });
}


function UpdatePassword() {
   
    var newPassword = $("#NewPassword").val();
    if (newPassword && newPassword.length < 6) {
        $("#NewPassword").next('span.errordisp').remove();
        $("#NewPassword").after('<span class="errordisp">Password must be at least 6 characters long.</span>');
        return;
    }

    var validated = $("#frmChangePassword").Validate();

    if (validated) {
        blockui();
        var form = document.getElementById('frmChangePassword');
        $.ajax({
            url: $(form).attr('action'),
            type: 'POST',
            data: new FormData(form),
            processData: false,
            contentType: false,
            success: function (result) {
                unblockui();
                if (result.ResultId == "1") {
                    SuccessMessage("Password updated successfully!");
                    CloseModal("ModalChangePassword")
                    setTimeout(function () {
                        window.location.reload();
                    }, 1500);
                } else {
                    ErrorMessage(result.ResultMessage || "An error occurred.");
                }
            },
            error: function () { unblockui(); ErrorMessage("A network error occurred."); }
        });
    }
}

function attachModalDismissListeners(modalEl, modalInstance) {
    const dismissButtons = modalEl.querySelectorAll('[data-bs-dismiss="modal"], [data-modal-dismiss="true"]');
    dismissButtons.forEach(button => {
        button.addEventListener('click', e => {
            e.preventDefault();
            modalInstance.hide();
        });
    });
    modalEl.addEventListener('hidden.bs.modal', function () { $(this).remove(); }, { once: true });
}