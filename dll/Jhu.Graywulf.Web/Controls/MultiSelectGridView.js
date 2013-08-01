Type.registerNamespace("Graywulf");

Graywulf.MultiSelectGridView = function (element, selectionMode, selectionCheckboxID, cssClass, selectedRowCssClass) {
    Graywulf.MultiSelectGridView.initializeBase(this, [element]);

    this._items = null;
    this._selectionMode = selectionMode;
    this._selectionCheckboxID = selectionCheckboxID;
    this._cssClass = cssClass;
    this._selectedRowCssClass = selectedRowCssClass;
}

Graywulf.MultiSelectGridView.prototype = {
    initialize: function () {
        Graywulf.MultiSelectGridView.callBaseMethod(this, 'initialize');

        // Initialize event handlers that restrict selection to a single item
        var element = this.get_element();

        // Apply css class to containing div
        $(element).closest("div").addClass(this.get_CssClass());

        var query = "input[type=checkbox][id*=" + this.get_SelectionCheckboxID() + "]";
        this._items = $(element).find(query);

        for (i = 0; i < this._items.length; i++) {
            $addHandlers(this._items[i], { click: this.updateSelection }, this);
            if (this._items[i].checked) {
                this.applyHighlight(this._items[i]);
            }
        }
    },

    dispose: function () {
        for (i = 0; i < this._items.length; i++) {
            $clearHandlers(this._items[i]);
        }

        Graywulf.MultiSelectGridView.callBaseMethod(this, 'dispose');
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

    get_CssClass: function () {
        return this._cssClass;
    },

    set_CssClass: function (value) {
        this._cssClass = value;
    },

    get_SelectedRowCssClass: function () {
        return this._selectedRowCssClass;
    },

    set_SelectedRowCssClass: function (value) {
        this._selectedRowCssClass = value;
    },

    updateSelection: function (e) {
        if (this.get_SelectionMode() == 0) {
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
        //var query = "*[id*=" + this.get_SelectionElementID() + "]";
        var tr = $(e).closest("tr");
        var cssclass = this.get_SelectedRowCssClass();

        if (e.checked) {
            tr.addClass(cssclass);
        }
        else {
            tr.removeClass(cssclass);
        }
    }
}

Graywulf.MultiSelectGridView.registerClass('Graywulf.MultiSelectGridView', Sys.UI.Behavior);

// Notify ScriptManager that this is the end of the script.
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
