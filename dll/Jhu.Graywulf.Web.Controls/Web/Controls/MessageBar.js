function initMessageBars() {
    $("div.gw-messagebar").each(initMessageBar);
}

function initMessageBar(idx, bar) {
    var btn = $(bar).find("span.gw-messagebar-button").first();
    $(btn).on("click", closeMessageBar);
}

function closeMessageBar(event) {
    var btn = event.target;
    var bar = $(btn).parent("div").first();
    $(bar).hide();

    if (dockContents) {
        $(".dock-container").each(dockContents);
    }
}

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(initMessageBars);