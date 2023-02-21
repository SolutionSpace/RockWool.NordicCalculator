class Main {
    constructor() {
        this.preloader = $(".page-preloader");
        this.calculatorFormSelector = ".umbraco-forms-Calculator form";
        this.projectOpenCell = ".projects-table-row td:not(td.projects-table-btn-td)";
        this.projectOpenIcon = "td.projects-table-btn-td .open-btn";
    }
    showPreloader() {
        this.preloader.fadeIn();
    }
    initPreloader() {
        $(window).on("load", () => {
            this.preloader.fadeOut("slow");
        });
        $(this.projectOpenCell + ", " + this.projectOpenIcon).on("click", () => {
            this.showPreloader();
        });
        $(this.calculatorFormSelector).on("submit", () => {
            this.showPreloader();
        });
    }
    initRemoveHash() {
        $(window).on("load", (evt) => {
            this.removeHash(evt);
        });
    }
    removeHash(evt) {
        if (window.location.hash == '#_=_') {
            window.location.hash = ''; // for older browsers, leaves a # behind
            history.pushState('', document.title, window.location.pathname); // nice and clean
            evt.preventDefault(); // no page reload
        }
    }
    init() {
        this.initPreloader();
        this.initRemoveHash();
    }
}
let main = new Main();
$(document).ready(() => {
    main.init();
});
//# sourceMappingURL=main.js.map