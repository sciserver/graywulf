function initJobDetailsList() {
    $(".gw-details-button").each(initToggleJobDetails);
}

function toggleJobDetails(event) {
    var btn = event.target;
    var bar = $(btn).parents(".gw-details-container").first();
    var more = $(bar).find(".gw-details-panel");
    $(more).toggle();
    $(btn).toggleClass("glyphicon-chevron-down");
    $(btn).toggleClass("glyphicon-chevron-up");
}

function initToggleJobDetails(idx, btn) {
    $(btn).on("click", toggleJobDetails);
}

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(initJobDetailsList);

