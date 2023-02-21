declare var successErrorMessagesDictionary: {};

class ErrorSuccessMessages {

    private projectSaveMsg: string;
    private projectPrintMsg: string;
    private projectEmailMsg: string;
    private projectErrorMsg: string;
    private projectDeleteMsg: string;
    private calculationErrorMsg: string;
    private calculationLinkErrorMsg: string;
    private projectPasswordRecoverMsg: string;

    private messageId: number;

    private errorMsgClass: string;
    private successMsgClass: string;
    private calcErrorMsgClass: string;

    private notificationWrapper: JQuery;

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

    public getMessage(msgText: string, msgType: string, msgId: number): string {
        return `<div class="alert ${msgType} id-${msgId} fade show" role="alert" style="z-index: 1; text-align: center;">${msgText}<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times</span></button></div>`;
    }

    public addNotification(config: string): void {
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
        window.setTimeout(() => { ($(`.id-${Id}`) as any).alert("close") }, time);
        this.messageId += 1;
        
    }

    public init(): void {
    }
}

class Header {
    private selectorOptions: ILanguageSelectorOptions;

    private languageSelector: JQuery;
    private languageOptionSelected: JQuery;

    private messages: ErrorSuccessMessages;

    constructor() {
        this.selectorOptions = {
            dropdownAlignRight: true,
            width: "fit"
        }
        this.languageSelector = $(".language-selector");
        this.languageSelector.val(window["culture"]);
        this.messages = new ErrorSuccessMessages();
    }

    public initLangOptions(): void {
        this.languageSelector.selectpicker(this.selectorOptions);
    }

    public initLanguageChanging(): void {
        this.languageSelector.on("change", (evt) => this.changeLanguage(evt));
    }

    public changeLanguage(evt: any): void {
        this.languageOptionSelected = $("option:selected", evt.target);
        let cultureUrl = this.languageOptionSelected.attr("data-value");
        location.href = cultureUrl;
    }

    public init(): void {
        this.messages.init();
        this.initLangOptions();
        this.initLanguageChanging();
    }
}

let header = new Header();

$(<any>document).ready(() => {
    header.init();
});