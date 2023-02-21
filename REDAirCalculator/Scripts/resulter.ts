class Resulter {
    private wrapper: JQuery;
    private resultContainer: JQuery;
    private resulterForm: JQuery;
    private linkResultTable: JQuery;

    private browserWidth: number;
    private mobileRange: number;
    private projectsApiPath: number;
    private projectsType: string;

    constructor() {
        this.wrapper = $(".wrapper");
        this.resultContainer = $(".result-container");
        this.resulterForm = $(".umbraco-forms-Calculator").find("form");
        this.linkResultTable = $(".link-results");
        this.projectsType = (<any>window).projectsType;
        this.projectsApiPath = (<any>window).projectsApiPath;
    }

    public renderResulter(): void {
        this.browserWidth = $((<any>window)).width();
        this.mobileRange = (<any>window).mobileRange;

        if (window.screen.width >= 900) {
            this.linkResultTable.removeClass("table-responsive");
        }

        if (this.browserWidth > this.mobileRange) {
            this.wrapper.addClass("desktop-resulter-wrapper");           
        } else {
            this.resultContainer.show();
        }

        this.resulterForm.attr("action", this.projectsApiPath + "/update?t=" + this.projectsType);
    }

    public stateResizeEvent(): void {
        this.browserWidth = $(<any>window).width();
        this.mobileRange = (<any>window).mobileRange;

        if (this.browserWidth > this.mobileRange) {
            this.wrapper.addClass("desktop-resulter-wrapper");
        } else {
            this.wrapper.removeClass("desktop-resulter-wrapper");
            this.resultContainer.show();
        }
    }

    public initStates(): void {
        this.renderResulter();
        $(<any>window).on("resize", () => this.stateResizeEvent());
    }

    public init(): void {
        this.initStates();
    }
}

let resulter = new Resulter();

$(<any>document).ready(() => {
    resulter.init();
});