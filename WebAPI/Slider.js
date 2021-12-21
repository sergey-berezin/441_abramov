// Images slider

class Slider {
    constructor() {
        this.slider = document.querySelector(".slider");
        this.sliderList = document.querySelector(".slider-list");
        this.leftArrow = new SliderArrow(document.querySelector("div.slider-arrow-left"));
        this.rightArrow = new SliderArrow(document.querySelector("div.slider-arrow-right"));
        this.table = new TableContent(document.querySelector("#table-body"));
        this.sliderElemsCount = 0;
    }

    moveRight() {
        if (this.sliderElemsCount > 1) {
            let prevElemInd = this.currentElemInd;
            this.currentElemInd = (this.currentElemInd + 1) % this.sliderElemsCount;
            this.sliderElements[prevElemInd].style.opacity = '0';
            this.sliderElements[this.currentElemInd].style.opacity = '1';

            this.table.Show(this.currentElemInd);
        }
    }

    moveLeft() {
        if (this.sliderElemsCount > 1) {
            let nextElemInd = this.currentElemInd;
            this.currentElemInd -= 1;
            if (this.currentElemInd < 0)
                this.currentElemInd = this.sliderElemsCount - 1;
            this.sliderElements[nextElemInd].style.opacity = '0';
            this.sliderElements[this.currentElemInd].style.opacity = '1';

            this.table.Show(this.currentElemInd);
        }
    }

    clear() {
        for (let i = this.sliderElemsCount; i > 0; i--) {
            this.sliderList.removeChild(this.sliderElements[i - 1]);
        }
        this.currentElemInd = 0;
        this.sliderElemsCount = this.sliderList.length;
    }

    LoadData(data) {
        this.clear();
        for (let i = 0; i < data.length; i++) {
            let img = document.createElement("img");
            img.src = 'data:image/png;base64,' + data[i].key;
            let sliderElem = document.createElement("li");
            sliderElem.className = "slider-list-element";
            sliderElem.append(img);
            this.sliderList.append(sliderElem);
        }
        this.sliderElements = this.sliderList.querySelectorAll(".slider-list-element");
        this.sliderElemsCount = this.sliderElements.length;
        this.currentElemInd = 0;
        this.sliderElements[this.currentElemInd].style.opacity = '1';

        this.table.LoadData(data);

        this.MakeMoves();
    }

    MakeMoves() {
        let that = this;
        this.rightArrow.EventHandlersClear();
        this.rightArrow.OnClickEventAdd(function () { that.moveRight(); });
        this.leftArrow.EventHandlersClear();
        this.leftArrow.OnClickEventAdd(function () { that.moveLeft(); });
    }

    Interactions(handler) {
        this.rightArrow.OnClickEventAdd(handler);
        this.leftArrow.OnClickEventAdd(handler);
    }
}