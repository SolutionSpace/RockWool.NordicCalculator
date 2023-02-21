class ErrorSuccessMessages {
    constructor() {
        this.projectSaveMsg = successErrorMessagesDictionary["projectSaveMsg"];
        this.projectPrintMsg = successErrorMessagesDictionary["projectPrintMsg"];
        this.projectEmailMsg = successErrorMessagesDictionary["projectEmailMsg"];
        this.projectErrorMsg = successErrorMessagesDictionary["projectErrorMsg"];
        this.projectDeleteMsg = successErrorMessagesDictionary["projectDeleteMsg"];
        this.calculationErrorMsg = successErrorMessagesDictionary["calculationErrorMsg"];
        this.calculationLinkErrorMsg = successErrorMessagesDictionary["calculationLinkErrorMsg"];
        this.projectPasswordRecoverMsg = successErrorMessagesDictionary["memberRecoveryPasswordMsg"];
        this.errorMsgClass = "alert-danger";
        this.calcErrorMsgClass = "alert-danger calc-error-alert";
        this.successMsgClass = "alert-success";
        this.messageId = 0;
        this.notificationWrapper = $(".notification-wrapper-main");
    }
    getMessage(msgText, msgType, msgId) {
        return `<div class="alert ${msgType} id-${msgId} fade show" role="alert" style="z-index: 1; text-align: center;">${msgText}<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times</span></button></div>`;
    }
    addNotification(config) {
        var colorType = this.successMsgClass;
        switch (config.split(":")[0]) {
            case "success":
                colorType = this.successMsgClass;
                break;
            case "error":
                colorType = this.errorMsgClass;
                break;
            case "calc-error":
                colorType = this.calcErrorMsgClass;
                break;
            case "calc-link-error":
                colorType = this.calcErrorMsgClass;
                break;
        }
        var msg = this.projectErrorMsg;
        switch (config.split(":")[1]) {
            case "email":
                msg = this.projectEmailMsg;
                break;
            case "save":
                msg = this.projectSaveMsg;
                break;
            case "error":
                msg = this.projectErrorMsg;
                break;
            case "delete":
                msg = this.projectDeleteMsg;
                break;
            case "calc-error":
                msg = this.calculationErrorMsg;
                break;
            case "calc-link-error":
                msg = this.calculationLinkErrorMsg;
                break;
            case "print":
                msg = this.projectPrintMsg;
                break;
            case "pass-recovery":
                msg = this.projectPasswordRecoverMsg;
                break;
        }
        var time = +(config.split(":")[2]); // 1000 = 1 sec
        const Id = this.messageId;
        this.notificationWrapper.append(this.getMessage(msg, colorType, Id));
        window.setTimeout(() => { $(`.id-${Id}`).alert("close"); }, time);
        this.messageId += 1;
    }
    init() {
    }
}
class Header {
    constructor() {
        this.selectorOptions = {
            dropdownAlignRight: true,
            width: "fit"
        };
        this.languageSelector = $(".language-selector");
        this.languageSelector.val(window["culture"]);
        this.messages = new ErrorSuccessMessages();
    }
    initLangOptions() {
        this.languageSelector.selectpicker(this.selectorOptions);
    }
    initLanguageChanging() {
        this.languageSelector.on("change", (evt) => this.changeLanguage(evt));
    }
    changeLanguage(evt) {
        this.languageOptionSelected = $("option:selected", evt.target);
        let cultureUrl = this.languageOptionSelected.attr("data-value");
        location.href = cultureUrl;
    }
    init() {
        this.messages.init();
        this.initLangOptions();
        this.initLanguageChanging();
    }
}
let header = new Header();
$(document).ready(() => {
    header.init();
});
//# sourceMappingURL=header.js.map