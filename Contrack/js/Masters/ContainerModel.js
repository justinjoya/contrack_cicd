function OpenContainerTypeModal(ContainerTypeId) {
    blockui();
    $.ajax({
        url: '/ContainerType/GetContainerTypeModal?ContainerTypeId=' + ContainerTypeId,
        type: 'GET',
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            OpenModal("ModalAddContainerType");
        },
        error: function () {
            unblockui();
            ErrorMessage("Error loading Container Type details");
        }
    });
}


function SaveContainerType() {
    var validated = $("#formContainerType").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formContainerType").serializeArray();
        $.ajax({
            url: '/ContainerType/SaveContainerType',
            data: formdata,
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Container Type saved successfully!");
                    CloseModal("ModalAddContainerType");
                    setTimeout(function () {
                        window.location.href = window.location;
                    }, 400);
                } else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot save Container Type!");
            }
        });
    }
}


function OpenContainerModelModal(modelid, typeid, sizeid) {
    blockui();
    var url = '/ContainerModel/GetContainerModelModal?';
    if (modelid) url += 'modelid=' + encodeURIComponent(modelid) + '&';
    if (typeid) url += 'typeid=' + encodeURIComponent(typeid) + '&';
    if (sizeid) url += 'sizeid=' + encodeURIComponent(sizeid) + '&';
    $.ajax({
        url: url,
        type: 'GET',
        success: function (html) {
            unblockui();
            $("#modalsection").html(html);
            $("#modalsection").find(".select2").select2();
            OpenModal("ModalAddContainerModel");
        },
        error: function () {
            unblockui();
            ErrorMessage("Error loading modal");
        }
    });
}

function SaveContainerModel() {
    var validated = $("#formContainerModel").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formContainerModel").serializeArray();
        $.ajax({
            url: '/ContainerModel/SaveContainerModel',
            data: formdata,
            type: 'POST',
            dataType: "json",
            success: function (data) {
                if (data.ResultId == "1") {
                    unblockui();
                    SuccessMessage("Container Model saved successfully!");
                    CloseModal("ModalAddContainerModel");
                    setTimeout(function () {
                        window.location.href = window.location;
                    }, 400);
                }
                else {
                    unblockui();
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function () {
                unblockui();
                ErrorMessage("Cannot save Container Model!");
            }
        });
    }
}

function LoadFilteredModels() {
    $.ajax({
        url: "/ContainerModel/ListPartial",
        type: "GET",
        success: function (html) {
            $("#ContainerModelBody").html(html);
        }
    });
}


let searchTimer = null;

$("#txtSearch").on("keyup", function () {
    $(".clear-search").toggle($(this).val().length > 0);
    if (searchTimer) clearTimeout(searchTimer);
    searchTimer = setTimeout(function () {
        SaveFilters(false);
    }, 400);
});

function SaveFilters(redirect) {
    var types = $("input[name='ctype']:checked")
        .map(function () { return $(this).val(); }).get();
    var sizes = $("input[name='csize']:checked")
        .map(function () { return $(this).val(); }).get();
    $.ajax({
        url: "/ContainerModel/SaveFilter",
        type: "POST",
        data: {
            containertypeid: types.join(","),
            sizeid: sizes.join(","),
            search: $("#txtSearch").val().trim()
        },
        success: function () {
            LoadFilteredModels();
            if (redirect === true) {
                window.location.href = "/ContainerModel/List";
            }
        }
    });
}

function ApplyFilters() {
    SaveFilters(true);
}

function ResetFilters() {
    $.ajax({
        url: "/ContainerModel/ResetFilter",
        type: "POST",
        success: function () {
            window.location.href = "/ContainerModel/List";
        }
    });
}

function ClearSearch() {
    $("#txtSearch").val("");
    $(".clear-search").hide();
    SaveFilters(true);
}
