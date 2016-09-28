function resizePanels() {
    (function ($) {

        $.fn.padding = function () {
            return {
                left: parseInt(this.css("padding-left")),
                top: parseInt(this.css("padding-top")),
                right: parseInt(this.css("padding-right")),
                bottom: parseInt(this.css("padding-bottom"))
            };
        };

    })(jQuery);

    (function ($) {

        $.fn.margin = function () {
            return {
                left: parseInt(this.css("margin-left")),
                top: parseInt(this.css("margin-top")),
                right: parseInt(this.css("margin-right")),
                bottom: parseInt(this.css("margin-bottom"))
            };
        };

    })(jQuery);

    (function ($) {

        $.fn.borderWidth = function () {
            return {
                left: parseInt(this.css("border-left-width")),
                top: parseInt(this.css("border-top-width")),
                right: parseInt(this.css("border-right-width")),
                bottom: parseInt(this.css("border-bottom-width"))
            };
        };

    })(jQuery);

    (function ($) {

        $.fn.edge = function () {
            return {
                left: this.margin().left + this.borderWidth().left + this.padding().left,
                top: this.margin().top + this.borderWidth().top + this.padding().top,
                right: this.margin().right + this.borderWidth().right + this.padding().right,
                bottom: this.margin().bottom + this.borderWidth().bottom + this.padding().bottom
            };
        };

    })(jQuery);

    $(".dock-container").each(dockContents);
}

function dockContents(idx, element) {

    var left, top, right, bottom;
    var leftedge, topedge;

    var ee = $(element);

    //ee.css({ "border": "1px solid red" });

    left = ee.padding().left;
    top = ee.padding().top;
    right = ee.innerWidth() - ee.padding().right;
    bottom = ee.innerHeight() - ee.padding().bottom;

    var ch = ee.children();

    for (var i = 0; i < ch.length; i++) {
        var vv = $(ch[i]);

        if (vv.visitedForLayout == null) {
            vv.visitedForLayout = 1;
            vv.edgeCache = vv.edge();
            vv.css({ "position": "absolute" });

            if (vv.hasClass("dock-top")) {
                vv.dockType = "top";
            } else if (vv.hasClass("dock-bottom")) {
                vv.dockType = "bottom";
            } else if (vv.hasClass("dock-left")) {
                vv.dockType = "left";
            } else if (vv.hasClass("dock-right")) {
                vv.dockType = "right";
            } else if (vv.hasClass("dock-fill")) {
                vv.dockType = "fill";
            }
        }

        var width = 0;
        var height = 0;

        switch (vv.dockType) {
            case "top":
            case "bottom":
                width = right - left - vv.edgeCache.left - vv.edgeCache.right;
                break;
            case "left":
            case "right":
                height = bottom - top - vv.edgeCache.top - vv.edgeCache.bottom;
                break;
            case "fill":
                width = right - left - vv.edgeCache.left - vv.edgeCache.right;
                height = bottom - top - vv.edgeCache.top - vv.edgeCache.bottom;
        }

        switch (vv.dockType) {
            case "top":
                vv.css({ "top": top, "left": left, "width": width });
                top += vv.outerHeight(true);
                break;
            case "bottom":
                bottom -= vv.outerHeight(true);
                vv.css({ "top": bottom, "left": left, "width": width });
                break;
            case "left":
                vv.css({ "top": top, "left": left, "height": height });
                left += vv.outerWidth(true);
                break;
            case "right":
                right -= vv.outerWidth(true);
                vv.css({ "top": top, "left": right, "height": height });
                break;
            case "fill":
                vv.css({ "top": top, "left": left, "width": width, "height": height });
                break;
        }
    }
}

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(resizePanels);

$(document).ready(function () {
    $(window).resize(function () {
        $(".dock-container").each(dockContents);
    });
});

