Type.registerNamespace("Graywulf");

Graywulf.MultiSelectListView = function (element, selectionMode, selectionCheckboxID, selectionElementID, cssClassSelected) {
    Graywulf.MultiSelectListView.initializeBase(this, [element]);

    this._items = null;
    this._selectionMode = selectionMode;
    this._selectionCheckboxID = selectionCheckboxID;
    this._selectionElementID = selectionElementID;
    this._cssClassSelected = cssClassSelected;
}

Graywulf.MultiSelectListView.prototype = {
    initialize: function () {
        Graywulf.MultiSelectListView.callBaseMethod(this, 'initialize');

        // Initialize event handlers that restrict selection to a single item
        var element = this.get_element();
        var query = "input[type=checkbox][id*=" + this.get_SelectionCheckboxID() + "]";

        this._items = $(element).find(query);

        for (i = 0; i < this._items.length; i++) {
            $addHandlers(this._items[i], { click: this.updateSelection }, this);
            if (this._items[i].checked) {
            }
        }
    },

    dispose: function () {
        for (i = 0; i < this._items.length; i++) {
            $clearHandlers(this._items[i]);
        }

        Graywulf.MultiSelectListView.callBaseMethod(this, 'dispose');
    },

    get_SelectionMode: function () {
        return this._selectionMode;
    },

    set_SelectionMode: function (value) {
        this._selectionMode = value;
    },

    get_SelectionCheckboxID: function () {
        return this._selectionCheckboxID;
    },

    set_SelectionCheckboxID: function (value) {
        this._selectionCheckboxID = value;
    },

    get_SelectionElementID: function () {
        return this._selectionElementID;
    },

    set_SelectionElementID: function (value) {
        this._selectionElementID = value;
    },

    get_CssClassSelected: function () {
        return this._cssClassSelected;
    },

    set_CssClassSelected: function (value) {
        this._cssClassSelected = value;
    },

    updateSelection: function (e) {
        if (this.get_SelectionMode == 0) {
            for (i = 0; i < this._items.length; i++) {
                this._items[i].checked = (this._items[i] == e.target);
                this.applyHighlight(this._items[i]);
            }
        }
        else {
            this.applyHighlight(e.target);
        }
    },

    applyHighlight: function (e) {
        var query = "*[id*=" + this.get_SelectionElementID() + "]";

        if (e.checked) {
            $(e).parentsUntil(this.get_element(), query).addClass(this.get_CssClassSelected());
        }
        else {
            $(e).parentsUntil(this.get_element(), query).removeClass(this.get_CssClassSelected());
        }
    }
}

Graywulf.MultiSelectListView.registerClass('Graywulf.MultiSelectListView', Sys.UI.Behavior);

// Notify ScriptManager that this is the end of the script.
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
