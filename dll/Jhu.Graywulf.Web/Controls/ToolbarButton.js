Type.registerNamespace("Graywulf");

Graywulf.ToolbarButton = function (element, clientOnClick, cssClass, cssClassDisabled, cssClassHover, navigateUrl, enabled) {
    Graywulf.ToolbarButton.initializeBase(this, [element]);

    this._clientOnClick = clientOnClick;
    this._cssClass = cssClass,
    this._cssClassDisabled = cssClassDisabled;
    this._cssClassHover = cssClassHover;
    this._navigateUrl = navigateUrl;
    this._enabled = enabled;
}

Graywulf.ToolbarButton.prototype = {
    initialize: function () {
        Graywulf.ToolbarButton.callBaseMethod(this, 'initialize');

        if (!this.get_Enabled()) {
            var element = this.get_element();
            $(element).addClass(this.get_CssClassDisabled());
        }

        $addHandlers(
            this.get_element(),
            {
                click: this.event_OnClick,
                mouseover: this.event_OnMouseOver,
                mouseout: this.event_OnMouseOut
            },
            this);
    },

    dispose: function () {
        $clearHandlers(this.get_element());

        Graywulf.ToolbarButton.callBaseMethod(this, 'dispose');
    },

    get_ClientOnClick: function () {
        return this._clientOnClick;
    },

    set_ClientOnClick: function (value) {
        this._clientOnClick = value;
    },

    get_CssClass: function () {
        return this._cssClass;
    },

    set_CssClass: function (value) {
        this._cssClass = value;
    },

    get_CssClassDisabled: function () {
        return this._cssClassDisabled;
    },

    set_CssClassDisabled: function (value) {
        this._cssClassDisabled = value;
    },

    get_CssClassHover: function () {
        return this._cssClassHover;
    },

    set_CssClassHover: function (value) {
        this._cssClassHover = value;
    },

    get_NavigateUrl: function () {
        return this._navigateUrl;
    },

    set_NavigateUrl: function (value) {
        this._navigateUrl = value;
    },

    get_Enabled: function () {
        return this._enabled;
    },

    set_Enabled: function (value) {
        this._enabled = value;
    },

    event_OnClick: function (e) {
        if (this.get_Enabled()) {
            var click = this.get_ClientOnClick();
            if (click && click != "") {
                eval(this.get_ClientOnClick());
            }

            var url = this.get_NavigateUrl();
            if (url && url != "") {
                window.location = url;
            }
            else {
                __doPostBack(this.get_element().id, "");
            }
        }
    },

    event_OnMouseOver: function (e) {
        if (this.get_Enabled()) {
            var element = this.get_element();
            $(element).removeClass(this.get_CssClass());
            $(element).addClass(this.get_CssClassHover());
        }
    },

    event_OnMouseOut: function (e) {
        if (this.get_Enabled()) {
            var element = this.get_element();
            $(element).removeClass(this.get_CssClassHover());
            $(element).addClass(this.get_CssClass());
        }
    }
}

Graywulf.ToolbarButton.registerClass('Graywulf.ToolbarButton', Sys.UI.Behavior);

// Notify ScriptManager that this is the end of the script.
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
