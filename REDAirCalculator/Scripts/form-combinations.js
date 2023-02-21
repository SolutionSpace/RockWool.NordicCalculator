class FormCombinations {
    constructor() {
        this.insulationThicknessId = window.insulationThicknessId;
        this.baseRailSpacingId = window.baseRailSpacingId;
        this.frictionId = window.frictionCoefficientId;
        this.insulationThicknessValue = window.insulationThicknessValue;
        this.baseRailSpacingValue = window.baseRailSpacingValue;
        this.frictionValue = window.frictionCoefficientValue;
        this.windSpeedAreaValue = window.windSpeedAreaValue;
        this.systemInputName = window.systemTypeId;
        this.cityValue = window.cityValue;
        this.windSpeedOtherValue = window.windSpeedOtherValue;
        this.thicknessSelect = $('#' + window.insulationThicknessId);
        this.baseRailSpacingSelect = $('#' + window.baseRailSpacingId);
        this.anchorSelect = $('#' + window.anchorScrewId);
        this.systemSelect = $('#' + window.systemTypeId);
        this.frictionSelect = $('#' + window.frictionCoefficientId);
        this.anchorTypeSelect = $('#' + window.ownLCWTypeId);
        this.citySelect = $(`select#${window.cityFieldId}`);
        this.windSpeedAreaSelect = $(`#${window.windSpeedAreaFieldId}`);
        this.windSpeedAreaSelectId = window.windSpeedAreaFieldId;
        this.windSpeedSelect = $(`#${window.windSpeedFieldId}`);
        this.jsAnchorScrewList = window.jsAnchorScrewList;
        this.jsAnchorFrictionCombinations = window.anchorFrictionCombinations;
        this.lcwThicknessCombinations = window.lcwThicknessCombinations;
        this.baseRailSpacingList = window.BaseRailCombinationList;
        this.areaCityDictionary = window.AreaCityDictionary;
    }
    setAnchorThicknessCombination() {
        if (this.anchorSelect.val() === "" && this.baseRailSpacingSelect.val() === "" && this.anchorTypeSelect.val() === "" && this.frictionSelect.val() === "" && this.thicknessSelect.val() === "")
            return;
        this.thicknessSelect.selectpicker('refresh');
        if (this.thicknessSelect.val() != "") {
            this.thicknessSelect.find('[value=' + this.thicknessSelect.val() + ']').removeAttr("hidden");
        }
        for (var i = 100; i <= 350; i += 50) {
            this.thicknessSelect.find('[value=' + i + ']').removeAttr("hidden");
            this.thicknessSelect.find('[value=' + i + ']').removeAttr("style");
        }
        this.thicknessSelect.find('[value=' + 110 + ']').removeAttr("hidden");
        this.thicknessSelect.find('[value=' + 110 + ']').removeAttr("style");
        var system = $("input[name=" + this.systemInputName + " ]:checked").val();
        var length = this.jsAnchorScrewList.length;
        for (var i = 0; i < length - 1; i++) {
            if (this.jsAnchorScrewList[i].name == this.anchorSelect.val()) {
                var thicknesses = this.jsAnchorScrewList[i].disabledThicknesses;
                for (var i = 0; i < thicknesses.length; i++) {
                    if (system == "FLEX" && thicknesses[i] == "100") {
                        continue;
                    }
                    else {
                        this.thicknessSelect.find('[value=' + thicknesses[i] + ']').attr("hidden", "hidden");
                        if (this.thicknessSelect.val() == this.thicknessSelect.find('[value=' + thicknesses[i] + ']').val()) {
                            if (system == "FLEX") {
                                this.thicknessSelect.selectpicker('val', '100');
                                this.updateBaseRail(100);
                            }
                            else {
                                this.thicknessSelect.selectpicker('val', '150');
                                this.updateBaseRail(150);
                            }
                        }
                    }
                }
            }
        }
        if (this.anchorSelect.val() === this.jsAnchorScrewList[length - 1].name) {
            if (this.anchorTypeSelect.val() != "") {
                this.onChangeAnchorType();
            }
        }
        this.thicknessSelect.selectpicker('refresh');
        $('#' + this.insulationThicknessId + ' option').show();
    }
    setAnchorFrictionCombination(init) {
        let anchorSelectedValue = this.anchorSelect.val();
        if (anchorSelectedValue === "" && this.baseRailSpacingSelect.val() === "" && this.anchorTypeSelect.val() === "" && this.frictionSelect.val() === "" && this.thicknessSelect.val() === "")
            return;
        //if (init === false) {
        //    $('#' + this.frictionId + ' option').remove();
        //}
        let length = this.jsAnchorFrictionCombinations.length;
        let anchorScrewOwnValue = this.jsAnchorFrictionCombinations[length - 1].name;
        if (anchorSelectedValue !== anchorScrewOwnValue) {
            $('#' + this.frictionId + ' option').remove();
            for (var i = 0; i < length; i++) {
                if (this.jsAnchorFrictionCombinations[i].name === this.anchorSelect.val()) {
                    var frictionList = this.jsAnchorFrictionCombinations[i].combinationList;
                    for (var friction in frictionList) {
                        var option = new Option(frictionList[friction], frictionList[friction]);
                        this.frictionSelect.append(option);
                    }
                    break;
                }
            }
        }
        // update value of dropdown after page reload
        if (this.frictionValue !== "" && $.makeArray($("#" + window.frictionCoefficientId + " option").text().toString())[0].includes(this.frictionValue)) {
            this.frictionSelect.val(this.frictionValue);
        }
        this.frictionSelect.selectpicker('refresh');
    }
    onChangeAnchorScrew(init) {
        this.setAnchorFrictionCombination(init);
        setTimeout(() => { this.setAnchorThicknessCombination(); }, 1000);
    }
    onChangeInsulation(init) {
        if (this.anchorSelect.val() === "" && this.baseRailSpacingSelect.val() === "" && this.anchorTypeSelect.val() === "" && this.frictionSelect.val() === "" && this.thicknessSelect.val() === "" && !init)
            return;
        if (init === false) {
            $('#' + this.baseRailSpacingId + ' option').remove();
        }
        var insulationValue = this.thicknessSelect.val();
        var length = this.baseRailSpacingList.length;
        var resultValues = [];
        var resultNames = [];
        var allValues = [];
        for (var i = 0; i < length; i++) {
            if (this.baseRailSpacingList[i].value >= insulationValue * 2 || this.baseRailSpacingList[i].value === 0) {
                resultValues.push(this.baseRailSpacingList[i].value);
                resultNames.push(this.baseRailSpacingList[i].name);
            }
            allValues.push(this.baseRailSpacingList[i].value);
        }
        if (resultValues.length <= 2) {
            var maxValue = Math.max.apply(Math, allValues);
            var option = new Option(maxValue, maxValue);
            option.value = maxValue;
            this.baseRailSpacingSelect.append(option);
            this.baseRailSpacingValue = "";
            this.baseRailSpacingSelect.selectpicker('val', '600');
        }
        else {
            for (var i = 0; i < resultValues.length; i++) {
                var option = new Option(resultNames[i], resultValues[i]);
                option.value = resultNames[i];
                this.baseRailSpacingSelect.append(option);
            }
        }
        // update value of dropdown after page reload
        if (this.baseRailSpacingValue !== "") {
            this.baseRailSpacingSelect.val(this.baseRailSpacingValue.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2)));
        }
        this.baseRailSpacingSelect.selectpicker('refresh');
    }
    updateBaseRail(value) {
        $('#' + this.baseRailSpacingId + ' option').remove();
        var insulationValue = value;
        var length = this.baseRailSpacingList.length;
        var resultValues = [];
        var resultNames = [];
        var allValues = [];
        for (var i = 0; i < length; i++) {
            if (this.baseRailSpacingList[i].value >= insulationValue * 2 || this.baseRailSpacingList[i].value === 0) {
                resultValues.push(this.baseRailSpacingList[i].value);
                resultNames.push(this.baseRailSpacingList[i].name);
            }
            allValues.push(this.baseRailSpacingList[i].value);
        }
        for (var i = 0; i < resultValues.length; i++) {
            var option = new Option(resultNames[i], resultValues[i]);
            option.value = resultNames[i];
            this.baseRailSpacingSelect.append(option);
        }
        this.baseRailSpacingSelect.selectpicker('refresh');
    }
    onChangeAnchorType() {
        if (this.anchorSelect.val() === "" && this.baseRailSpacingSelect.val() === "" && this.anchorTypeSelect.val() === "" && this.frictionSelect.val() === "" && this.thicknessSelect.val() === "")
            return;
        $('#' + this.insulationThicknessId + ' option').attr("hidden", "hidden");
        var system = $("input[name=" + this.systemInputName + " ]:checked").val();
        for (var i = 0; i < this.lcwThicknessCombinations.length; i++) {
            if (this.lcwThicknessCombinations[i].name === this.anchorTypeSelect.val()) {
                var thicknesses = this.lcwThicknessCombinations[i].thicknesses;
                for (var i = 0; i < thicknesses.length; i++) {
                    if (system == "MULTI" && thicknesses[i] == "100" && thicknesses.length == 4) {
                        continue;
                    }
                    else {
                        this.thicknessSelect.find('[value=' + thicknesses[i] + ']').removeAttr("hidden");
                        this.thicknessSelect.find('[value=' + thicknesses[i] + ']').removeAttr("style");
                    }
                }
                if (thicknesses.indexOf(this.thicknessSelect.val().toString()) === -1) {
                    if (thicknesses.includes("100")) {
                        this.thicknessSelect.selectpicker('val', '150');
                    }
                    else {
                        this.thicknessSelect.selectpicker('val', '100');
                    }
                }
            }
        }
        // update value of dropdown after page reload
        if (this.insulationThicknessValue !== "" && $.makeArray($("#" + window.insulationThicknessId + " option").text().toString())[0].includes(this.insulationThicknessValue)) {
            this.thicknessSelect.val(this.insulationThicknessValue);
        }
        this.thicknessSelect.selectpicker('refresh');
    }
    setWindSpeedAreaCombinations() {
        if (Object.keys(this.areaCityDictionary).length === 0)
            return;
        for (var key in this.areaCityDictionary) {
            this.windSpeedAreaSelect.append(`<option value="${key}" ${this.windSpeedAreaSelect.next().attr("title").replace(/[\u00A0-\u9999<>\&]/gim, i => '&#' + i.charCodeAt(0) + ';') === key ? "selected" : ""}> ${key} </option>`);
        }
        // update value of dropdown after page reload
        if (this.windSpeedAreaValue !== "" && $.makeArray($("#" + this.windSpeedAreaSelectId + " option").text().toString())[0].includes(this.windSpeedAreaValue.replace("&lt;", "<").replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2)))) {
            this.windSpeedAreaSelect.val(this.windSpeedAreaValue.replace("&lt;", "<").replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2)));
            this.windSpeedAreaSelect.next().attr("title", this.windSpeedAreaValue.replace("&lt;", "<").replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2)));
        }
        this.windSpeedAreaSelect.selectpicker('refresh');
    }
    setCityCombinations(onreload) {
        for (var key in this.areaCityDictionary) {
            if (this.areaCityDictionary[key].length === 0)
                return;
            else {
                break;
            }
        }
        this.citySelect.find("option").remove();
        let citySelect = this.citySelect;
        if (this.windSpeedAreaSelect.val() === this.windSpeedOtherValue) {
            citySelect.append(`<option hidden value="${this.windSpeedOtherValue}"></option>`);
            this.citySelect.selectpicker('refresh');
            return;
        }
        else {
            this.citySelect.removeAttr('disabled');
        }
        let validatedWindSpeedAreaSelectValue = this.windSpeedAreaSelect.val().toString()
            .replace(/[\u00A0-\u9999<>\&]/gim, i => '&#' + i.charCodeAt(0) + ';');
        if (this.windSpeedAreaSelect.val() !== null && validatedWindSpeedAreaSelectValue !== undefined && validatedWindSpeedAreaSelectValue !== "") {
            this.areaCityDictionary[validatedWindSpeedAreaSelectValue].forEach(value => {
                citySelect.append(`<option value="${value}"> ${value} </option>`);
            });
        }
        let validatedCitySelectValue = this.citySelect.find("option").text().toString()
            .replace(/[\u00A0-\u9999<>\&]/gim, i => '&#' + i.charCodeAt(0) + ';');
        // update value of dropdown after page reload
        if (this.cityValue !== "" && onreload && $.makeArray(validatedCitySelectValue)[0].includes(this.cityValue)) {
            $("select#" + window.cityFieldId).val(this.cityValue.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2)));
            this.citySelect.next().attr("title", this.cityValue.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2)));
        }
        this.citySelect.selectpicker('refresh');
    }
    initWindspeedAreaCity() {
        this.setWindSpeedAreaCombinations();
        if (this.windSpeedAreaValue !== "") {
            this.setCityCombinations(true);
        }
        this.windSpeedAreaSelect.on("change", () => this.setCityCombinations(false));
        // get windspeed value
        this.windSpeedAreaSelect.on("change", (event) => this.getWindSpeedAreaValue(event));
        this.citySelect.on("change", (event) => this.getWindSpeedAreaValue(event));
    }
    getWindSpeedAreaValue(event) {
        event.preventDefault();
        let windSpeedSelect = this.windSpeedSelect;
        let area = this.windSpeedAreaSelect.val();
        let city = this.citySelect.val();
        if (area !== this.windSpeedOtherValue && city !== this.windSpeedOtherValue) {
            $.ajax({
                type: 'GET',
                url: '/umbraco/surface/windspeedarea/get',
                data: {
                    area,
                    city
                },
                dataType: 'json',
                async: false,
                success(response) {
                    windSpeedSelect.val(response);
                }
            });
        }
        if (area === this.windSpeedOtherValue) {
            this.windSpeedSelect.val(0);
        }
    }
    initCombinations() {
        $(this.anchorSelect).on("change", () => this.onChangeAnchorScrew(false));
        $(this.systemSelect).on("change", () => this.onChangeAnchorScrew(false));
        $(this.anchorTypeSelect).on("change", () => this.onChangeAnchorType());
        $(this.thicknessSelect).on("change", () => this.onChangeInsulation(false));
        this.initWindspeedAreaCity();
    }
    init() {
        this.initCombinations();
    }
}
let formCombinations = new FormCombinations();
$(document).ready(() => {
    formCombinations.init();
});
//# sourceMappingURL=form-combinations.js.map