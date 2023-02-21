class OpeningTypes {
    constructor() {
        this.selectorOptions = {
            showTick: true,
            dropdownAlignRight: false
        };
        this.calculatorForm = $(".umbraco-forms-Calculator").find("form");
        this.openingTypeSelect = $('#' + window.openingTypesId);
        this.calculatorBtn = $(".btn-calculator");
        this.calculatorFormId = "#calculator-form";
        this.widthInput = `input.width`;
        this.heightInput = `input.height`;
        this.openingTypesContainer = $(".opening-types-container");
        this.fieldValidationErrorClass = "field-validation-error";
        this.openingTypeErrors = ".opening-types-errors";
        this.openingTypesSelector = ".opening-types-selector";
        this.tableBodySelector = "table tbody";
        this.rowSelector = "tr";
        this.errorSelector = "span";
        this.defaultRowSelector = `${this.tableBodySelector} ${this.rowSelector}:first-child`;
        this.defaultErrorSelector = `${this.openingTypeErrors} > ${this.errorSelector}:first-child`;
        this.addTypeBtn = ".add-opening-type-btn";
        this.removeTypeBtn = ".remove-opening-type-btn";
        this.nameWidth = "Width";
        this.nameHeight = "Height";
        this.areTypesCalculated = false;
        this.translateWindow = window.windowType;
        this.translateDoor = window.doorType;
        this.translateWidth = window.translateWidth;
        this.translateHeight = window.translateHeight;
        this.translateShouldBeGreater = window.translateShouldBeGreater;
        this.translateShouldBeLess = window.translateShouldBeLess;
        this.translateIsRequired = window.translateIsRequired;
        this.translateType = window.translateType;
        let translateWindowType = this.translateWindow.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2));
        let translateDoorType = this.translateDoor.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2));
        this.openingTypes = [
            {
                type: translateWindowType,
                row: `${translateWindowType} ${this.translateType}`
            },
            {
                type: translateDoorType,
                row: `${translateDoorType} ${this.translateType}`
            }
        ];
        this.openingTypeDefaultValues = {
            type: this.openingTypes[0].type,
            width: 1000,
            height: 1000,
            amount: 1,
            isWindow: true
        };
        this.openingTypesList = window.openingTypesList;
        this.minValue = parseInt($(this.defaultRowSelector).find("td:nth-child(3)").find("input").attr("min"));
        this.maxValue = parseInt($(this.defaultRowSelector).find("td:nth-child(3)").find("input").attr("max"));
    }
    checkRowsLength() {
        let rowsCount = this.openingTypesContainer.find(this.tableBodySelector).find(this.rowSelector).length;
        let firstRowRemoveBtn = $(this.defaultRowSelector).find(this.removeTypeBtn);
        if (rowsCount > 1) {
            firstRowRemoveBtn.show();
        }
        else {
            firstRowRemoveBtn.hide();
        }
    }
    updateTypeNumbers() {
        let counter = 1;
        this.openingTypesContainer.find(this.tableBodySelector).find(this.rowSelector).each((index, row) => {
            $(row).find(".type-number").text(counter++);
        });
    }
    showOnResulter() {
        $.each(this.openingTypesList, (index, typeItem) => {
            let openingItem = {
                type: typeItem.type,
                width: typeItem.width,
                height: typeItem.height,
                amount: typeItem.amount,
                isWindow: typeItem.isWindow
            };
            this.add(openingItem, true, index);
        });
    }
    getTypeValues(evt) {
        // if already clicked or it is not calculator form do not call event
        if (this.areTypesCalculated || !$(evt.target).is(this.calculatorFormId)) {
            return;
        }
        evt.preventDefault();
        let saveValues = [];
        this.openingTypesContainer.find(this.tableBodySelector).find(this.rowSelector).each((index, row) => {
            let openingTypeName = $(row).find("td:nth-child(2)").find("select").val().toString();
            let saveOpeningType = {
                type: openingTypeName,
                width: parseInt($(row).find("td:nth-child(3)").find("input").val().toString()),
                height: parseInt($(row).find("td:nth-child(4)").find("input").val().toString()),
                amount: parseInt($(row).find("td:nth-child(5)").find("input").val().toString()),
                isWindow: openingTypeName === this.openingTypes[0].type
            };
            saveValues.push(saveOpeningType);
        });
        var jsonValues = JSON.stringify(saveValues);
        // save values into opening types field
        this.openingTypeSelect.val(jsonValues);
        this.areTypesCalculated = true;
        // continue the submit unbind preventDefault
        $(evt.target).unbind('submit').submit();
    }
    updateRow(evt) {
        let currentRow = $(evt.target).closest("tr");
        let openingTypeName = currentRow.find("td:nth-child(2)").find("select").val().toString();
        let openingTypeRow = openingTypeName === this.openingTypes[0].type
            ? this.openingTypes[0].row
            : this.openingTypes[1].row;
        currentRow.find(".type-name").text(openingTypeRow.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2)));
        this.updateErrorRows();
    }
    initInitialState() {
        this.checkRowsLength();
    }
    initOpeningTypesOptions() {
        $(this.openingTypesSelector).selectpicker(this.selectorOptions);
    }
    add(openingItem, isResulter = false, typeIndex = 0) {
        let row = this.openingTypesContainer.find(this.defaultRowSelector);
        let error = this.openingTypesContainer.find(this.defaultErrorSelector);
        let rowCopy = row.clone();
        let errorWidthCopy = error.clone();
        let errorHeightCopy = error.clone();
        if (isResulter && typeIndex === 0) {
            rowCopy = row;
        }
        // set default error values
        errorWidthCopy.find("span").remove();
        errorHeightCopy.find("span").remove();
        // set default row values
        rowCopy.find(this.removeTypeBtn).show();
        rowCopy.find("td:nth-child(2)").html(`<select class='opening-types-selector'>
                <option> ${this.openingTypes[0].type} </option>
                <option> ${this.openingTypes[1].type} </option>
            </select>`);
        rowCopy.find("td:nth-child(3)").find("input").val(openingItem.width);
        rowCopy.find("td:nth-child(4)").find("input").val(openingItem.height);
        rowCopy.find("td:nth-child(5)").find("input").val(openingItem.amount);
        if (!isResulter || typeIndex > 0) {
            // add new row
            this.openingTypesContainer.find(this.tableBodySelector).append(rowCopy);
            this.openingTypesContainer.find(this.openingTypeErrors).append(errorWidthCopy);
            this.openingTypesContainer.find(this.openingTypeErrors).append(errorHeightCopy);
        }
        // update new row
        let currentRow = this.openingTypesContainer.find(this.rowSelector).last();
        let rowName = openingItem.isWindow ? this.openingTypes[0].row : this.openingTypes[1].row;
        let rowNumber = isResulter && typeIndex === 0 ? 1 : parseInt(currentRow.prev().find(".type-number").text()) + 1;
        currentRow.find(".type-name").text(rowName);
        currentRow.find(".type-number").text(rowNumber);
        currentRow.find("td:nth-child(3)").find("input").attr("name", `${this.nameWidth}${rowNumber}`);
        currentRow.find("td:nth-child(3)").find("input").attr("data-val-required", `${rowName} ${rowNumber}: ${this.translateWidth} ${this.translateIsRequired}`);
        currentRow.find("td:nth-child(4)").find("input").attr("name", `${this.nameHeight}${rowNumber}`);
        currentRow.find("td:nth-child(4)").find("input").attr("data-val-required", `${rowName} ${rowNumber}: ${this.translateHeight} ${this.translateIsRequired}`);
        currentRow.find(this.openingTypesSelector).selectpicker(this.selectorOptions);
        let currentWidthError = this.openingTypesContainer.find(this.openingTypeErrors).find(this.errorSelector).last();
        let currentHeightError = currentWidthError.prev();
        currentWidthError.attr("data-valmsg-for", `${this.nameWidth}${rowNumber}`);
        currentHeightError.attr("data-valmsg-for", `${this.nameHeight}${rowNumber}`);
        if (isResulter) {
            currentRow.find(this.openingTypesSelector).selectpicker('val', openingItem.type);
        }
        this.checkRowsLength();
        // update client validation
        this.calculatorForm.removeData("validator");
        this.calculatorForm.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(this.calculatorForm);
    }
    remove(evt) {
        let rowsCount = this.openingTypesContainer.find(this.tableBodySelector).find(this.rowSelector).length;
        let currentRow = $(evt.target).closest(this.rowSelector);
        if (rowsCount > 1) {
            $(evt.target).closest(currentRow).remove();
        }
        this.updateTypeNumbers();
        this.checkRowsLength();
        this.updateErrorRows();
    }
    updateErrorRows() {
        let error = this.openingTypesContainer.find(this.defaultErrorSelector);
        // remove all error messages
        this.openingTypesContainer.find(this.openingTypeErrors).find(`> ${this.errorSelector}`).each((index, row) => {
            $(row).remove();
        });
        // update validation info form rows
        this.openingTypesContainer.find(this.tableBodySelector).find(this.rowSelector).each((index, row) => {
            let errorWidthCopy = error.clone();
            let errorHeightCopy = error.clone();
            this.openingTypesContainer.find(this.openingTypeErrors).append(errorWidthCopy);
            this.openingTypesContainer.find(this.openingTypeErrors).append(errorHeightCopy);
            let rowName = $.trim($(row).find(".type-name").text());
            let rowNumber = index + 1;
            $(row).find("td:nth-child(3)").find("input").attr("name", `${this.nameWidth}${rowNumber}`);
            $(row).find("td:nth-child(3)").find("input").attr("data-val-required", `${rowName} ${rowNumber}: ${this.translateWidth} ${this.translateIsRequired}`);
            $(row).find("td:nth-child(3)").find("input").attr("aria-describedby", "");
            $(row).find("td:nth-child(4)").find("input").attr("name", `${this.nameHeight}${rowNumber}`);
            $(row).find("td:nth-child(4)").find("input").attr("data-val-required", `${rowName} ${rowNumber}: ${this.translateHeight} ${this.translateIsRequired}`);
            $(row).find("td:nth-child(4)").find("input").attr("aria-describedby", "");
            let currentWidthError = this.openingTypesContainer.find(this.openingTypeErrors).find(`> ${this.errorSelector}`).last();
            let currentHeightError = currentWidthError.prev();
            currentWidthError.attr("data-valmsg-for", `${this.nameWidth}${rowNumber}`);
            currentHeightError.attr("data-valmsg-for", `${this.nameHeight}${rowNumber}`);
        });
        // show validation messages
        $(this.widthInput).valid();
        $(this.heightInput).valid();
        // update validation info after manual validation call
        this.openingTypesContainer.find(this.openingTypeErrors).find(`> ${this.errorSelector}`).each((index, row) => {
            if ($(row).hasClass(this.fieldValidationErrorClass)) {
                let valMsgAttr = $(row).attr("data-valmsg-for");
                let rowNumber = parseInt(valMsgAttr.match(/\d+/)[0]);
                let fieldName = valMsgAttr.indexOf("Width") !== -1 ? this.translateWidth : this.translateHeight;
                fieldName = fieldName.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2));
                let tableRow = this.openingTypesContainer.find(this.tableBodySelector).find(this.rowSelector).eq(rowNumber - 1);
                let typeName = $.trim(tableRow.find("td:first-child").text()).replace(/\n/g, '').replace(/  +/g, ' ');
                let errorReplacePattern = new RegExp(`: (\\S*\\s+)`, "g"); // match any word including special characters without spaces
                let errorText = $.trim($(row).find("span").text()).replace(/\n/g, '').replace(/  +/g, ' ');
                let errorTypeText = errorText.split(":")[0];
                let typeValue = parseInt(tableRow.find(`input[name='${valMsgAttr}']`).val().toString());
                errorText = errorText
                    .replace(errorTypeText, `${typeName}`)
                    .replace(errorReplacePattern, `: ${fieldName} `);
                if (typeValue < this.minValue) {
                    errorText = `${typeName}: ${fieldName} ${this.translateShouldBeGreater.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2))} ${this.minValue}`;
                }
                if (typeValue > this.maxValue) {
                    errorText = `${typeName}: ${fieldName} ${this.translateShouldBeLess.replace(/&#(\d+);/g, (match, match2) => String.fromCharCode(+match2))} ${this.maxValue}`;
                }
                $(row).find("span").text(errorText);
            }
        });
    }
    updateValidationMessage(evt) {
        let input = $(evt.target);
        let currentRow = input.closest("tr");
        let typeName = $.trim(currentRow.find("td:first-child").text());
        let valueName = input.hasClass("width") ? this.translateWidth : this.translateHeight;
        // update required messages
        this.updateErrorRows();
        $.extend($.validator.messages, {
            min: `${typeName}: ${valueName} ${this.translateShouldBeGreater} {0}`,
            max: `${typeName}: ${valueName} ${this.translateShouldBeLess} {0}`
        });
    }
    initOpeningTypesChanges() {
        this.openingTypesContainer.on("click", this.addTypeBtn, () => this.add(this.openingTypeDefaultValues));
        this.openingTypesContainer.on("click", this.removeTypeBtn, (evt) => this.remove(evt));
        this.openingTypesContainer.on("change", this.openingTypesSelector, (evt) => this.updateRow(evt));
        this.openingTypesContainer.on("input", `${this.widthInput}, ${this.heightInput}`, (evt) => this.updateValidationMessage(evt));
    }
    initFormSubmit() {
        // update opening types
        $(document).on("submit", this.calculatorForm, (evt) => this.getTypeValues(evt));
        this.calculatorBtn.on("click", () => this.updateErrorRows());
    }
    init() {
        this.initInitialState();
        this.initOpeningTypesOptions();
        this.initOpeningTypesChanges();
        this.initFormSubmit();
        this.showOnResulter();
    }
}
let openingTypes = new OpeningTypes();
$(document).ready(() => {
    openingTypes.init();
});
//# sourceMappingURL=opening-types.js.map