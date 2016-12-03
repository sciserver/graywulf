function initMessageBar(idx, bar) {
    var btn = $(bar).find("span.gw-messagebar-button").first();
    $(btn).on("click", function (event) {
        var btn = event.target;
        var bar = $(btn).parent("div").first();
        $(bar).hide();

        if (dockContents) {
            $(".dock-container").each(dockContents);
        }
    });
}

function pageLoad() {
    $("div.gw-messagebar").each(initMessageBar);
}