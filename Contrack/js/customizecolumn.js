
$(document).ready(function () {
    $(document).on("click", ".customize-save-btn", function () {
        const selectedMenuIds = $(".field-switch").map(function () {
            const $el = $(this);
            return ($el.is(":checked") || $el.is(":disabled")) ? $el.data("menuid") : null;
        }).get().filter(id => id !== null);

        if (selectedMenuIds.length === 0) {
            ErrorMessage("Please select at least one column.");
            return;
        }
        blockui();
        $.ajax({
            type: "POST",
            url: "/CustomizeColumn/SaveMenus",
            data: JSON.stringify({ menuIds: selectedMenuIds }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                unblockui();
                if (response.ResultId === 1) {
                    SuccessMessage(response.ResultMessage);
                    setTimeout(() => location.reload(), 200);
                } else {
                    ErrorMessage(response.ResultMessage);
                }
            },
            error: function (xhr, status, error) {
                unblockui();
                ErrorMessage("AJAX Error: " + error);
            }
        });
    });
    $(document).on("click", ".customize-cancel-btn", function () {
        $("#dropdown .dropdown-content").hide();
    });
    function applyColumnVisibility() {
        $(".field-switch").each(function () {
            const isChecked = $(this).is(":checked");
            const columnIndex = parseInt($(this).closest(".menu-item").find(".index").val());

            if (!isNaN(columnIndex)) {
                $("table tr").each(function () {
                    $(this).find("td, th").eq(columnIndex).toggle(isChecked);
                });
            }
        });
    }

    applyColumnVisibility();
    const observer = new MutationObserver(() => {
        applyColumnVisibility();
    });

    $("table").each(function () {
        observer.observe(this, { childList: true, subtree: true });
    });
    $(document).on("change", ".field-switch", function () {
        applyColumnVisibility();
    });
    $(document).on("input", "#dropdown_search", function () {
        var searchText = $(this).val().toLowerCase();
        $("#table_columns").find(".menu-item").each(function () {
            var menuTitle = $(this).find(".menu-title").text().toLowerCase();
            $(this).toggle(menuTitle.includes(searchText));
        });
    });
    $(document).on("change", ".select-all-switch", function () {
        const checked = $(this).is(":checked");
        $(".field-switch:not(:disabled)").prop("checked", checked);
        applyColumnVisibility();
    });
});
