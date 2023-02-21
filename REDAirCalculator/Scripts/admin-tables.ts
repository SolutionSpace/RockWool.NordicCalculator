class AdminTable{
    private selectorOptions: ICalculatorSelectorOptions;

    private wrapper: JQuery;
    private calculatorSelector: JQuery;
    private anchorAdminSelect: JQuery;
    private frictionAdminSelect: JQuery;

    private jsAnchorScrewAdminList: string[];
    private jsFrictionCoefAdminList: string[];

    constructor() {
        this.selectorOptions = {
            showTick: false,
            dropdownAlignRight: true
        }
        this.calculatorSelector = $(".calculator-selector");
        this.wrapper = $(".wrapper");
        this.anchorAdminSelect = $('#AnchorScrewDesign');
        this.frictionAdminSelect = $('#FrictionCoef');
        this.jsAnchorScrewAdminList = (<any>window).jsAnchorScrewAdminList;
        this.jsFrictionCoefAdminList = (<any>window).jsFrictionAdminList;
    }
    public initCalcOptions(): void {
        this.calculatorSelector.selectpicker(this.selectorOptions);
    }

    public initTableWrapper(): void {
        this.wrapper.addClass("table-wrapper");
    }

    public onAnchorOwnValue(): void {
        var length = this.jsAnchorScrewAdminList.length;
        if (this.anchorAdminSelect.val() != this.jsAnchorScrewAdminList[length - 1]) {
            $("#anchorOwnType").hide();
            $("#anchorOwnTypeValue").val("0");
        } else {
            $("#anchorOwnType").show();
        }
    }

    public onFrictionOwnValue(): void {
        var length = this.jsFrictionCoefAdminList.length;
        if (this.frictionAdminSelect.val() != this.jsFrictionCoefAdminList[length - 1]) {
            $("#frictionOwnType").hide();
            $("#frictionOwnTypeValue").val("0");
        } else {
            $("#frictionOwnType").show();
        }
    }
    public initChanges(): void {
        $(this.anchorAdminSelect).on("change", () => this.onAnchorOwnValue());
        $(this.frictionAdminSelect).on("change", () => this.onFrictionOwnValue());
    }
    public init(): void {
        this.initTableWrapper();
        this.initCalcOptions();
        this.initChanges();
    }
}

let adminTable = new AdminTable();

$(<any>document).ready(() => {
    adminTable.init();
});