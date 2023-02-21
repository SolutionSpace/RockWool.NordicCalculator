class AdminTable {
    constructor() {
        this.selectorOptions = {
            showTick: false,
            dropdownAlignRight: true
        };
        this.calculatorSelector = $(".calculator-selector");
        this.wrapper = $(".wrapper");
        this.anchorAdminSelect = $('#AnchorScrewDesign');
        this.frictionAdminSelect = $('#FrictionCoef');
        this.jsAnchorScrewAdminList = window.jsAnchorScrewAdminList;
        this.jsFrictionCoefAdminList = window.jsFrictionAdminList;
    }
    initCalcOptions() {
        this.calculatorSelector.selectpicker(this.selectorOptions);
    }
    initTableWrapper() {
        this.wrapper.addClass("table-wrapper");
    }
    onAnchorOwnValue() {
        var length = this.jsAnchorScrewAdminList.length;
        if (this.anchorAdminSelect.val() != this.jsAnchorScrewAdminList[length - 1]) {
            $("#anchorOwnType").hide();
            $("#anchorOwnTypeValue").val("0");
        }
        else {
            $("#anchorOwnType").show();
        }
    }
    onFrictionOwnValue() {
        var length = this.jsFrictionCoefAdminList.length;
        if (this.frictionAdminSelect.val() != this.jsFrictionCoefAdminList[length - 1]) {
            $("#frictionOwnType").hide();
            $("#frictionOwnTypeValue").val("0");
        }
        else {
            $("#frictionOwnType").show();
        }
    }
    initChanges() {
        $(this.anchorAdminSelect).on("change", () => this.onAnchorOwnValue());
        $(this.frictionAdminSelect).on("change", () => this.onFrictionOwnValue());
    }
    init() {
        this.initTableWrapper();
        this.initCalcOptions();
        this.initChanges();
    }
}
let adminTable = new AdminTable();
$(document).ready(() => {
    adminTable.init();
});
//# sourceMappingURL=admin-tables.js.map