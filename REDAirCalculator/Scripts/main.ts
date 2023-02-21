class Main {
    private preloader: JQuery;
    private calculatorFormSelector: string;
    private projectOpenCell: string;
    private projectOpenIcon: string;

    constructor() {
        this.preloader = $(".page-preloader");
        this.calculatorFormSelector = ".umbraco-forms-Calculator form";
        this.projectOpenCell = ".projects-table-row td:not(td.projects-table-btn-td)";
        this.projectOpenIcon = "td.projects-table-btn-td .open-btn";
    }

    public showPreloader(): void {
        this.preloader.fadeIn();
    }

    public initPreloader(): void {
        $(<any>window).on("load", () => {
            this.preloader.fadeOut("slow");
        });

        $(this.projectOpenCell + ", " + this.projectOpenIcon).on("click", () => {
            this.showPreloader();
        });

        $(this.calculatorFormSelector).on("submit", () => {
            this.showPreloader();
        });
    }

    public initRemoveHash(): void {
        $(<any>window).on("load", (evt) => {
            this.removeHash(evt);
        });
    }

    public removeHash(evt: any): void {
        if ((<any>window).location.hash == '#_=_') {
            (<any>window).location.hash = ''; // for older browsers, leaves a # behind
            history.pushState('', <any>document.title, (<any>window).location.pathname); // nice and clean
            evt.preventDefault(); // no page reload
        }
    }

    public init(): void {
        this.initPreloader();
        this.initRemoveHash();
    }
}

let main = new Main();

$(<any>document).ready(() => {
    main.init();
});