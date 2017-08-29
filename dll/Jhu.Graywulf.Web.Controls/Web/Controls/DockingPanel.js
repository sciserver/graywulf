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

        $.fn.boxShadowSize = function () {
            var shadow = this.css("box-shadow");
            if (shadow) {
                // h-shadow v-shadow blur spread 
                var parts = shadow.match(/(-?\d+px)/g);
                if (parts) {
                    var h_shadow = parseInt(parts[0]);
                    var v_shadow = parseInt(parts[1]);
                    var blur = parseInt(parts[2]);
                    var spread = parseInt(parts[3]);
                    return {
                        left: Math.max(0, Math.abs(h_shadow - spread)),
                        top: Math.max(0, Math.abs(v_shadow - spread)),
                        right: Math.max(0, h_shadow + spread),
                        bottom: Math.max(0, v_shadow + spread)
                    };
                }
            }
            return { left: 0, top: 0, right: 0, bottom: 0 };
        };

    })(jQuery);

    (function ($) {

        $.fn.edge = function () {
            var m = this.margin();
            var b = this.borderWidth();
            var s = this.boxShadowSize();

            return {
                left: m.left + b.left + s.left,
                top: m.top + b.top + s.top,
                right: m.right + b.right + s.right,
                bottom: m.bottom + b.bottom + s.bottom
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
                        if (dockType != "container" && dockType != "scroll" && dockType != "hidden") {
                            vv.dockType = dockType;
                        }
                    }
                }
            }
        }

        var width = right - left - vv.edgeCache.left - vv.edgeCache.right;
        var height = bottom - top - vv.edgeCache.top - vv.edgeCache.bottom;

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
                vv.css({ "left": left, "max-height": height, "max-width": width });
                break;
            case "vcenter":
                vv.css({ "top": top, "max-height": height, "max-width": width });
                break;
            case "center":
                vv.css({ "left": left, "top": top, "max-height": height, "max-width": width });
                break;
        }

        if (vv.dockType) {
            vv.removeClass("dock-hidden");
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
