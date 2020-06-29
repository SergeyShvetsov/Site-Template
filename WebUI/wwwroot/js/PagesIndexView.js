$(function () {
    /* Confirm Page deletion */
    $("a.delete").click(function () {
        if (!confirm("Confirm Page deletion")) return false;
    });
    /*---------------------------------------*/
    /* Sorting script */
    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/Admin/Pages/ReorderPages";

            $.post(url, { ids });
        }
    });
});