$(".SelectedCount").on("input", function () {
    this.value = this.value.replace(/[^0-9]/g, '');
    var maxValue = parseInt($(this).data("max"));
    if (maxValue == undefined)
        maxValue = 0;
    // Enforce max value
    if (this.value !== "" && parseInt(this.value) > maxValue) {
        this.value = maxValue;
    }
});

function AllocateQty(current) {
    blockui();
    var ReserveID = $(current).closest(".model-card").find(".ReservationID").val();
    var LocationID = $(current).closest(".allocation-card").find(".LocationId").val();
    var ModelID = $(current).closest(".model-card").find(".ContainerModelId").val();
    var Qty = $(current).closest(".model-card").find(".SelectedCount").val();
    var BookingID = $("#bookingid").val();
    var DetailID = $("#bookingdetailid").val();
    $.ajax({
        url: '/Reserve/AllocateQty',
        type: 'POST',
        data: {
            BookingID: BookingID,
            BookingDetailID: DetailID,
            ReserveID: ReserveID,
            LocationID: LocationID,
            ModelID: ModelID,
            Qty: Qty
        },
        success: function (data) {
            unblockui();

            if (data.ResultId == "1") {
                SuccessMessage("Allocated Successfully!");
                setTimeout(
                    function () {
                        window.location = window.location.href;
                    }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function () {
            unblockui();
            ErrorMessage("Cannot allocate container.");
        }
    });
}


function DeleteAllocateQty(current) {
    ShowErrorConfirmation({
        title: 'Delete Allocation?',
        message: 'Are you sure you want to delete the allocated qty?',
        confirmtext: 'Yes, Delete',
        canceltext: 'Cancel',
        onConfirm: () => {
            blockui();
            var ReserveID = $(current).closest(".model-card").find(".ReservationID").val();
            var BookingID = $("#bookingid").val();
            var DetailID = $("#bookingdetailid").val();
            var LocationID = $(current).closest(".allocation-card").find(".LocationId").val();
            var ModelID = $(current).closest(".model-card").find(".ContainerModelId").val();

            $.ajax({
                url: '/Reserve/DeleteAllocateQty',
                type: 'POST',
                data: {
                    BookingID: BookingID,
                    BookingDetailID: DetailID,
                    ReserveID: ReserveID,
                    LocationID: LocationID,
                    ModelID: ModelID
                },
                success: function (data) {
                    unblockui();

                    if (data.ResultId == "1") {
                        SuccessMessage("Deleted Successfully!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function () {
                    unblockui();
                    ErrorMessage("Cannot allocate container.");
                }
            });
        }
    });
}