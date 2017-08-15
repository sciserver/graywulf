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
                left: this.margin().left + this.borderWidth().left,
                top: this.margin().top + this.borderWidth().top,
                right: this.margin().right + this.borderWidth().right,
                bottom: this.margin().bottom + this.borderWidth().bottom
            };
        };

    })(jQuery);

    $(".dock-container").each(dockContents);
}

function dockContents(idx, element) {

    var left, top, right, bottom;
    var leftedge, topedge;

    var ee = $(element);

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

            if ($(vv).attr("class")) {
                var classList = $(vv).attr("class").split(/\s+/);
                for (var j = 0; j < classList.length; j++) {
                    if (classList[j].startsWith("dock-")) {
                        var dockType = classList[j].substring(5);
                        if (dockType != "container" && dockType != "scroll") {
                            vv.dockType = dockType;
                        }
                    }
                }
            }
        }

        var width = 0;
        var height = 0;

        if (vv.dockType == "top" || vv.dockType == "bottom" || vv.dockType == "fill" || vv.dockType == "hfill") {
            width = right - left - vv.edgeCache.left - vv.edgeCache.right;
        }

        if (vv.dockType == "left" || vv.dockType == "right" || vv.dockType == "fill" || vv.dockType == "vfill") {
            height = bottom - top - vv.edgeCache.top - vv.edgeCache.bottom;
        }

        if (vv.dockType == "hcenter" || vv.dockType == "center" || vv.dockType == "vfill") {
            left = (right - left - vv.width()) / 2;
        }

        if (vv.dockType == "vcenter" || vv.dockType == "center" || vv.dockType == "hfill") {
            top = (bottom - top - vv.height()) / 2.5;
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
            case "vfill":
                vv.css({ "top": top, "left": left, "height": height });
                break;
            case "hfill":
                vv.css({ "top": top, "left": left, "width": width });
                break;
            case "fill":
                vv.css({ "top": top, "left": left, "width": width, "height": height });
                break;
            case "hcenter":
                vv.css({ "left": left });
                break;
            case "vcenter":
                vv.css({ "top": top });
                break;
            case "center":
                vv.css({ "left": left, "top": top });
                break;
        }
    }
}


function pageLoad() {
    resizePanels();
    $(".dock-container").each(dockContents);
}

$(document).ready(function () {
    $(window).resize(function () {
        $(".dock-container").each(dockContents);
    });
});
