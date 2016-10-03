Type.registerNamespace("Graywulf");

Graywulf.CodeMirror = function (element, indentUnit, lineNumbers, matchBrackets, mode, theme, width, height) {
    Graywulf.CodeMirror.initializeBase(this, [element]);

    this._codeMirror = null;

    this._indentUnit = indentUnit;
    this._lineNumbers = lineNumbers;
    this._matchBrackets = matchBrackets;
    this._mode = mode;
    this._theme = theme;
    this._width = width;
    this._height = height;
}

Graywulf.CodeMirror.prototype = {
    initialize: function () {
        Graywulf.CodeMirror.callBaseMethod(this, "initialize");

        this._text = $get(this.get_element().id + "_text");
        this._selectionCoords = $get(this.get_element().id + "_selectionCoords");
        this._selectedText = $get(this.get_element().id + "_selectedText");

        // Initialize codemirror
        this.set_CodeMirror(
            CodeMirror.fromTextArea(
                this._text,
                {
                    lineNumbers: this.get_LineNumbers(),
                    matchBrackets: this.get_MatchBrackets(),
                    indentUnit: this.get_IndentUnit(),
                    mode: this.get_Mode(),
                    theme: this.get_Theme()
                }));

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(Function.createDelegate(this, this.init));
    },

    dispose: function () {
        Graywulf.CodeMirror.callBaseMethod(this, 'dispose');
    },

    get_CodeMirror: function () {
        return this._codeMirror;
    },

    set_CodeMirror: function (value) {
        this._codeMirror = value;
    },

    get_Text: function () {
        return this._text.value;
    },

    set_Text: function (value) {
        this._text.value = value;
    },

    get_SelectedText: function () {
        return this._selectedText.value;
    },

    set_SelectedText: function (value) {
        this._selectedText.value = value;
    },

    get_SelectionCoords: function () {
        return this._selectionCoords.value;
    },

    set_SelectionCoords: function (value) {
        this._selectionCoords.value = value;
    },

    get_IndentUnit: function () {
        return this._indentUnit;
    },

    set_IndentUnit: function (value) {
        this._indentUnit = value;
    },

    get_LineNumbers: function () {
        return this._lineNumbers;
    },

    set_LineNumbers: function (value) {
        this._lineNumbers = value;
    },

    get_MatchBrackets: function () {
        return this._matchBrackets;
    },

    set_MatchBrackets: function (value) {
        this._matchBrackets = value;
    },

    get_Mode: function () {
        return this._mode;
    },

    set_Mode: function (value) {
        this._mode = value;
    },

    get_Theme: function () {
        return this._theme;
    },

    set_Theme: function (value) {
        this._theme = value;
    },

    get_Width: function () {
        return this._width;
    },

    set_Width: function (value) {
        this._width = value;
    },

    get_Height: function () {
        return this._height;
    },

    set_Height: function (value) {
        this._height = value;
    },

    onBeforeSubmit: function () {
        this.saveSelection();
    },

    init: function () {
        try {
            this._codeMirror.setSize(this._width, this._height);
        } catch (ex) { }

        try {
            this._codeMirror.refresh();
        } catch (ex) { }

        try {
            this.loadSelection();
        } catch (ex) { }

    },

    saveSelection: function () {
        var cm = this.get_CodeMirror();

        this.set_SelectionCoords(
            cm.getCursor(true).line + "," + cm.getCursor(true).ch + "," +
            cm.getCursor(false).line + "," + cm.getCursor(false).ch);

        if (cm.somethingSelected()) {
            this.set_SelectedText(cm.getSelection());
        }
        else {
            this.set_SelectedText(cm.getValue());
        }

        this.set_Text(cm.getValue());
    },

    loadSelection: function () {
        var sc = this.get_SelectionCoords();
        if (sc) {
            var coords = sc.split(",");

            var st = { line: parseInt(coords[0]), ch: parseInt(coords[1]) };
            var en = { line: parseInt(coords[2]), ch: parseInt(coords[3]) };

            this.get_CodeMirror().setSelection(st, en);
        }
    }
}

Graywulf.CodeMirror.registerClass('Graywulf.CodeMirror', Sys.UI.Behavior);

// Notify ScriptManager that this is the end of the script.
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
