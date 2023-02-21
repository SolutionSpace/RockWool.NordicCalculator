class Calculator {
    constructor() {
        this.calculatorForm = $(".umbraco-forms-Calculator").find("form");
        this.selectorOptions = {
            showTick: true,
            dropdownAlignRight: true
        };
        this.calculatorBtnClass = "btn-calculator";
        this.calculatorSelectorWrapper = "calculator-selector-wrapper";
        this.calculatorSelector = $(".calculator-selector");
        this.agreeCheckBox = $(".agree-checkbox input");
        this.calculatorBtn = $("." + this.calculatorBtnClass);
        this.plankDepthInput = $(".plankDepth");
        this.calculatorFieldsWrapper = ".calculator-fields";
        this.calculatorFormLine = ".calculator-form-line";
        this.formInput = this.calculatorFieldsWrapper + " .form-control";
        this.formControl = " .form-control";
        this.formLabel = ".calculator-field-label";
        this.fieldWrapper = ".calculator-field-wrapper";
        this.formGroup = ".bmd-form-group";
        this.formSelect = "select";
        this.formRadio = 'input[type="radio"]';
        this.radioButtonList = ".radiobutton-list";
        this.notCheckedString = "checkbox-checked=false";
        this.formCheckbox = ".form-checkbox-check";
        this.formCheckboxWrapper = ".form-checkbox";
        this.inputValidationError = ".input-validation-error";
        this.agreeCheckboxCheck = $(".agree-checkbox-check");
        this.checkedString = "checkbox-checked=true";
        this.notCheckedString = "checkbox-checked=false";
        this.projectsApiPath = window.projectsApiPath;
        this.hasCalculationError = (window.hasCalculationError === "True");
        this.projectsType = window.projectsType;
        this.PlanksValidationMessageMin = window.PlanksValidationMessageMin;
        this.PlanksValidationMessageMax = window.PlanksValidationMessageMax;
    }
    initCalcOptions() {
        this.calculatorSelector.closest(this.fieldWrapper).addClass(this.calculatorSelectorWrapper);
        this.calculatorSelector.selectpicker(this.selectorOptions);
    }
    initAnimationLine() {
        this.animationLine = $(".dropdown-toggle");
        this.animationLine.on("click", (evt) => this.setActiveLine(evt));
        $(this.formInput).on("click", (evt) => this.setActiveLine(evt));
        $(this.formControl).on("click", (evt) => this.setActiveLine(evt));
        $(this.formLabel).on("click", (evt) => this.setActiveLine(evt));
        $(this.formCheckboxWrapper).on("click", (evt) => this.setActiveLine(evt));
        $(this.radioButtonList).on("click", (evt) => this.setActiveLine(evt));
        $("body").on('click focus', (evt) => this.refreshActiveLines(evt));
        $(this.formInput).on("focus", (evt) => this.setFieldFocus(evt));
    }
    changeFormCheckBoxValue() {
        $(this.formCheckbox).on("change", function () {
            var value;
            if (!$(this).is(':checked')) {
                $(this).removeAttr("checked");
                value = false;
            }
            else {
                $(this).attr("checked", "checked");
                value = true;
            }
            $(this).val((value).toString());
        });
    }
    disableActiveLines(evt) {
        $(this.formInput).removeClass("active");
        $(this.formControl).removeClass("active");
        $(this.formLabel).removeClass("active");
        $(this.fieldWrapper).removeClass("active");
        $(this.radioButtonList).removeClass("active");
        $(this.formCheckboxWrapper).removeClass("active");
    }
    refreshActiveLines(evt) {
        this.selectedField = window.getSelection();
        if (this.selectedField != null) {
            this.selectedText = this.selectedField.toString();
        }
        if ($(evt.target).hasClass(this.calculatorBtnClass)) {
            return;
        }
        if ($(evt.target).closest(this.calculatorFieldsWrapper).length > 0 || this.selectedText !== "") {
            return;
        }
        this.disableActiveLines(evt);
    }
    setActiveLine(evt) {
        this.disableActiveLines(evt);
        $(evt.target).parents(this.calculatorFormLine).find(this.formLabel).addClass("active");
        if ($(evt.target).parents(this.calculatorFormLine).has(this.formSelect).length > 0) {
            $(evt.target).parents(this.calculatorFormLine).find(this.fieldWrapper).addClass("active");
        }
        if ($(evt.target).parents(this.calculatorFormLine).has(this.formRadio).length > 0) {
            $(evt.target).parents(this.calculatorFormLine).find(this.radioButtonList).addClass("active");
        }
        if ($(evt.target).parents(this.calculatorFormLine).has(this.formCheckbox).length > 0) {
            $(evt.target).parents(this.calculatorFormLine).find(this.formCheckboxWrapper).addClass("active");
        }
        if ($(evt.target).parents(this.calculatorFormLine).has(this.formControl).length > 0) {
            $(evt.target).parents(this.calculatorFormLine).find(this.formControl).addClass("active");
        }
    }
    setFieldFocus(evt) {
        $(evt.target).parents(this.calculatorFormLine).find(this.formLabel).addClass("active");
    }
    getCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
    initCheckboxCookie() {
        if (this.getCookie("checkbox-checked") == "true") {
            this.agreeCheckboxCheck.prop("checked", true);
        }
        else {
            this.agreeCheckboxCheck.prop("checked", false);
        }
        this.agreeCheckboxCheck.on('click', () => {
            if (this.agreeCheckboxCheck.prop('checked') === true) {
                document.cookie = this.checkedString;
            }
            else {
                document.cookie = this.notCheckedString;
            }
        });
    }
    initAgreeBox() {
        this.agreeCheckBox.on("click", () => this.validateForm());
    }
    validateForm() {
        this.agreeCheckBox.on("click", (evt) => this.setActiveLine(evt));
        if (!this.agreeCheckBox.is(':checked')) {
            this.calculatorBtn.attr("disabled", "");
        }
        else {
            this.calculatorBtn.removeAttr("disabled");
        }
    }
    initUpdateAfterError() {
        if (this.hasCalculationError) {
            this.calculatorForm.attr("action", this.projectsApiPath + "/update?t=" + this.projectsType);
        }
    }
    initCalculations() {
        this.calculatorBtn.on("click", (evt) => this.validateCalculations(evt));
    }
    validateCalculations(evt) {
        this.disableActiveLines(evt);
        // add animation line to errors
        this.validationError = $(".field-validation-error");
        this.validationError.parents(this.calculatorFormLine).find(this.formLabel).addClass("active");
        this.validationError.parents(this.calculatorFormLine).find(this.fieldWrapper).addClass("active");
        let fieldWrapper = this.validationError.closest(this.fieldWrapper);
        fieldWrapper.addClass("is-focused");
        fieldWrapper.find(this.formGroup).find(this.inputValidationError).focus();
    }
    initFieldsTab() {
        $(this.calculatorFieldsWrapper).on('keydown', this.calculatorFormLine, evt => {
            let keyCode = evt.keyCode || evt.which;
            let line = $(evt.target).closest(this.calculatorFormLine);
            if (keyCode === 9) {
                evt.preventDefault();
                this.moveField(evt, 1, line);
            }
            if (evt.shiftKey && keyCode === 9) {
                this.moveField(evt, -1, line);
            }
        });
    }
    // move to next/previos field by tab/shift+tab
    moveField(evt, stepIndex, targetField) {
        evt.preventDefault();
        let fields = $(this.calculatorFieldsWrapper).find(this.calculatorFormLine + ":visible");
        let index = fields.index(targetField);
        let moveIndex = index + stepIndex;
        if (index > -1 && moveIndex < fields.length) {
            this.makeStep(moveIndex, fields);
        }
        if (moveIndex === fields.length) {
            this.makeStep(0, fields);
        }
    }
    // step action for tab/shift+tab
    makeStep(nextStepIndex, fields) {
        fields.eq(nextStepIndex).find("label").trigger("click");
    }
    submitTooltipShow() {
        if (this.calculatorBtn.prop('disabled')) {
            $('[data-toggle="tooltipSubmit"]').tooltip('enable');
        }
        else {
            $('[data-toggle="tooltipSubmit"]').tooltip('disable');
        }
    }
    initTooltips() {
        $(document).ready(() => {
            $('[data-toggle="tooltip"]').tooltip();
            this.submitTooltipShow();
        });
    }
    changeValidationMessage() {
        $.extend($.validator.messages, {
            min: `${this.PlanksValidationMessageMin}`,
            max: `${this.PlanksValidationMessageMax}`,
        });
        $(this.plankDepthInput).valid();
    }
    init() {
        this.initCalcOptions();
        this.initAnimationLine();
        this.initAgreeBox();
        this.initUpdateAfterError();
        this.initCalculations();
        this.initCheckboxCookie();
        this.initFieldsTab();
        this.initTooltips();
        this.validateForm();
        this.changeFormCheckBoxValue();
        $(this.agreeCheckBox).on("click", () => this.submitTooltipShow());
        this.plankDepthInput.on("input", () => this.changeValidationMessage());
        if (this.plankDepthInput.length > 0) {
            this.calculatorBtn.on("click", () => this.changeValidationMessage());
        }
    }
}
let calculator = new Calculator();
$(document).ready(() => {
    calculator.init();
});
//# sourceMappingURL=calculator.js.map