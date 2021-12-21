// SliderArrow

class SliderArrow {
    constructor(documentElement) {
        this.documentElement = documentElement;
        this.makeSelectionStyle();
        this.clickHandlers = [];
    }

    makeSelectionStyle() {
        let selectArrow = function () {
            this.style.opacity = '1';
        };

        let removeSelection = function () {
            this.style.opacity = '0.6';
        };

        this.documentElement.addEventListener("mouseover", selectArrow);
        this.documentElement.addEventListener("mouseout", removeSelection);
    }

    OnClickEventAdd(handler) {
        this.documentElement.addEventListener("click", handler);
        this.clickHandlers.push(handler);
    }

    EventHandlersClear() {
        for (var handler in this.clickHandlers) {
            this.documentElement.removeEventListener("click", handler);
        }
    }
}

