class Projects {
    private isProjectDeleted: boolean;
    private projectTypesSelector: string;

    private projectTypeOptionSelected: JQuery;

    private selectorOptions: IProjectTypesSelectorOptions;

    constructor() {
        this.isProjectDeleted = false;
        this.projectTypesSelector = ".projects-types-selector";
        $(this.projectTypesSelector).val(window["projectsTypeValue"]);

        this.selectorOptions = {
            showTick: false,
            dropdownAlignRight: false
        };
    }

    public deleteProject(evt: any, deleteUrl: string): void {
        if (!this.isProjectDeleted) {
            this.isProjectDeleted = true;
            location.href = deleteUrl;
        } else {
            $(evt.target).prop("disabled", true);
        }
    }

    public initProjectTypesOptions(): void {
        $(this.projectTypesSelector).selectpicker(this.selectorOptions);
    }

    public initProjectTypesChanging(): void {
        $(this.projectTypesSelector).on("change", (evt) => this.changeProjectType(evt));
    }

    public changeProjectType(evt: any): void {
        this.projectTypeOptionSelected = $("option:selected", evt.target);
        let typeUrl = this.projectTypeOptionSelected.attr("data-value");
        location.href = typeUrl;
    }

    public init(): void {
        this.initProjectTypesOptions();
        this.initProjectTypesChanging();
    }
}

let projects = new Projects();

$(<any>document).ready(() => {
    projects.init();
});