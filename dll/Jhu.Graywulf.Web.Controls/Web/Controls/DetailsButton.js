function initDetailsButtons() {
    $(".gw-details-button").each(initToggleDetails);
}

function toggleDetails(event) {
    event.preventDefault();
    var btn = event.target;
    var bar = $(btn).parents(".gw-details-container").first();
    var more = $(bar).find(".gw-details-panel");
    $(more).toggle();
    $(btn).toggleClass("glyphicon-chevron-down");
    $(btn).toggleClass("glyphicon-chevron-up");

    if (dockContents) {
        $(btn).parents(".dock-container").first().each(dockContents);
    }
}

function initToggleDetails(idx, btn) {
    $(btn).on("click", toggleDetails);
}

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(initDetailsButtons);

