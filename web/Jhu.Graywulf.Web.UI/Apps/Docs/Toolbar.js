function initDocLists() {
    $("div.toolbar div select").each(initDocList);
}

function initDocList(idx, list) {
    $(list).on("change", function (e) {
        if (this.value != null && this.value != "")
            window.location.href = this.value;
    });
}

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(initDocLists);