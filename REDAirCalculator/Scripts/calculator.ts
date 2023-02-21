class Calculator {
    private selectorOptions: ICalculatorSelectorOptions;

    private calculatorSelectorWrapper: string;
    private calculatorBtnClass: string;

    private calculatorForm: JQuery;
    private calculatorSelector: JQuery;
    private animationLine: JQuery;
    private agreeCheckBox: JQuery;
    private calculatorBtn: JQuery;
    private agreeCheckboxCheck: JQuery;
    private validationError: JQuery;
    private plankDepthInput: JQuery;

    private selectedField: Selection;

    private calculatorFieldsWrapper: string;
    private calculatorFormLine: string;
    private formLabel: string;
    private formSelect: string;
    private fieldWrapper: string;
    private formGroup: string;
    private formRadio: string;
    private radioButtonList: string;
    private formInput: string;
    private formControl: string;
    private formCheckbox: string;
    private formCheckboxWrapper: string;
    private inputValidationError: string;

    private PlanksValidationMessageMin: string;
    private PlanksValidationMessageMax: string;

    private checkedString: string;
    private notCheckedString: string;
    private projectsType: string;

    private selectedText: string;

    private projectsApiPath: number;
    private hasCalculationError: boolean;

    constructor() {
        this.calculatorForm = $(".umbraco-forms-Calculator").find("form");

        this.selectorOptions = {
            showTick: true,
            dropdownAlignRight: true
        }

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

        this.projectsApiPath = (<any>window).projectsApiPath;
        this.hasCalculationError = ((<any>window).hasCalculationError === "True");
        this.projectsType = (<any>window).projectsType;
        this.PlanksValidationMessageMin = (<any>window).PlanksValidationMessageMin;
        this.PlanksValidationMessageMax = (<any>window).PlanksValidationMessageMax;
    }

    public initCalcOptions(): void {
        this.calculatorSelector.closest(this.fieldWrapper).addClass(this.calculatorSelectorWrapper);
        this.calculatorSelector.selectpicker(this.selectorOptions);
    }


    public initAnimationLine(): void {
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

    public changeFormCheckBoxValue(): void {
        $(this.formCheckbox).on("change", function () {
            var value;
            if (!$(this).is(':checked')) {
                $(this).removeAttr("checked");
                value = false;
            } else {

                $(this).attr("checked", "checked");
                value = true;
            }
            $(this).val((value).toString());
        });
    }

    public disableActiveLines(evt: any): void {
        $(this.formInput).removeClass("active");
        $(this.formControl).removeClass("active");
        $(this.formLabel).removeClass("active");
        $(this.fieldWrapper).removeClass("active");
        $(this.radioButtonList).removeClass("active");
        $(this.formCheckboxWrapper).removeClass("active");
    }

    public refreshActiveLines(evt: any): void {
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

    public setActiveLine(evt: any): void {
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

    public setFieldFocus(evt: any): void {
        $(evt.target).parents(this.calculatorFormLine).find(this.formLabel).addClass("active");
    }

    public getCookie(cname: string): string {
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

    public initCheckboxCookie(): void {
        if (this.getCookie("checkbox-checked") == "true") {
            this.agreeCheckboxCheck.prop("checked", true);
        } else {
            this.agreeCheckboxCheck.prop("checked", false);
        }

        this.agreeCheckboxCheck.on('click',
            () => {
                if (this.agreeCheckboxCheck.prop('checked') === true) {
                    document.cookie = this.checkedString;
                } else {
                    document.cookie = this.notCheckedString;
                }
            });
    }

    public initAgreeBox(): void {
        this.agreeCheckBox.on("click", () => this.validateForm());
    }

    public validateForm(): void {
        this.agreeCheckBox.on("click", (evt) => this.setActiveLine(evt));
        if (!this.agreeCheckBox.is(':checked')) {
            this.calculatorBtn.attr("disabled", "");
        } else {
            this.calculatorBtn.removeAttr("disabled");
        }
    }

    public initUpdateAfterError(): void {
        if (this.hasCalculationError) {
            this.calculatorForm.attr("action", this.projectsApiPath + "/update?t=" + this.projectsType);
        }
    }

    public initCalculations(): void {
        this.calculatorBtn.on("click", (evt) => this.validateCalculations(evt));
    }

    public validateCalculations(evt: any): void {
        this.disableActiveLines(evt);

        // add animation line to errors
        this.validationError = $(".field-validation-error");
        this.validationError.parents(this.calculatorFormLine).find(this.formLabel).addClass("active");
        this.validationError.parents(this.calculatorFormLine).find(this.fieldWrapper).addClass("active");
        let fieldWrapper = this.validationError.closest(this.fieldWrapper);
        fieldWrapper.addClass("is-focused");
        fieldWrapper.find(this.formGroup).find(this.inputValidationError).focus();
    }

    public initFieldsTab(): void {
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
    public moveField(evt: any, stepIndex: number, targetField: JQuery): void {
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
    public makeStep(nextStepIndex, fields) {
        fields.eq(nextStepIndex).find("label").trigger("click");
    }
    public submitTooltipShow() {
        if (this.calculatorBtn.prop('disabled')) {
            ($('[data-toggle="tooltipSubmit"]') as any).tooltip('enable');
        } else {
            ($('[data-toggle="tooltipSubmit"]') as any).tooltip('disable');
        }
    }
    public initTooltips(): void {
        $(<any>document).ready(() => {
            ($('[data-toggle="tooltip"]') as any).tooltip();
            this.submitTooltipShow();
        });
    }

    public changeValidationMessage(): void {
        $.extend(($ as any).validator.messages, {
            min: `${this.PlanksValidationMessageMin}`,
            max: `${this.PlanksValidationMessageMax}`,
        });
        ($(this.plankDepthInput) as any).valid();
    }

    public init(): void {
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

$(<any>document).ready(() => {
    calculator.init();
});