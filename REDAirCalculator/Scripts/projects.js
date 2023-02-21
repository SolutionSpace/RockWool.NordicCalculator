class Projects {
    constructor() {
        this.isProjectDeleted = false;
        this.projectTypesSelector = ".projects-types-selector";
        $(this.projectTypesSelector).val(window["projectsTypeValue"]);
        this.selectorOptions = {
            showTick: false,
            dropdownAlignRight: false
        };
    }
    deleteProject(evt, deleteUrl) {
        if (!this.isProjectDeleted) {
            this.isProjectDeleted = true;
            location.href = deleteUrl;
        }
        else {
            $(evt.target).prop("disabled", true);
        }
    }
    initProjectTypesOptions() {
        $(this.projectTypesSelector).selectpicker(this.selectorOptions);
    }
    initProjectTypesChanging() {
        $(this.projectTypesSelector).on("change", (evt) => this.changeProjectType(evt));
    }
    changeProjectType(evt) {
        this.projectTypeOptionSelected = $("option:selected", evt.target);
        let typeUrl = this.projectTypeOptionSelected.attr("data-value");
        location.href = typeUrl;
    }
    init() {
        this.initProjectTypesOptions();
        this.initProjectTypesChanging();
    }
}
let projects = new Projects();
$(document).ready(() => {
    projects.init();
});
//# sourceMappingURL=projects.js.map