class Resulter {
    constructor() {
        this.wrapper = $(".wrapper");
        this.resultContainer = $(".result-container");
        this.resulterForm = $(".umbraco-forms-Calculator").find("form");
        this.linkResultTable = $(".link-results");
        this.projectsType = window.projectsType;
        this.projectsApiPath = window.projectsApiPath;
    }
    renderResulter() {
        this.browserWidth = $(window).width();
        this.mobileRange = window.mobileRange;
        if (window.screen.width >= 900) {
            this.linkResultTable.removeClass("table-responsive");
        }
        if (this.browserWidth > this.mobileRange) {
            this.wrapper.addClass("desktop-resulter-wrapper");
        }
        else {
            this.resultContainer.show();
        }
        this.resulterForm.attr("action", this.projectsApiPath + "/update?t=" + this.projectsType);
    }
    stateResizeEvent() {
        this.browserWidth = $(window).width();
        this.mobileRange = window.mobileRange;
        if (this.browserWidth > this.mobileRange) {
            this.wrapper.addClass("desktop-resulter-wrapper");
        }
        else {
            this.wrapper.removeClass("desktop-resulter-wrapper");
            this.resultContainer.show();
        }
    }
    initStates() {
        this.renderResulter();
        $(window).on("resize", () => this.stateResizeEvent());
    }
    init() {
        this.initStates();
    }
}
let resulter = new Resulter();
$(document).ready(() => {
    resulter.init();
});
//# sourceMappingURL=resulter.js.map