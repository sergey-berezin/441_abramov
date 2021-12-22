// Categories selector

class CategorySelector {
    constructor() {
        this.selector = document.querySelector(".selector");
        this.selectorList = this.selector.querySelector(".selector-list");
        this.selectorCategory = this.selectorList.querySelector(".selector-list-element");
    }

    LoadData(data) {
        for (let i = 0; i < data.length; i++) {
            let category = data[i];
            let option = document.createElement("option");
            option.className = "selector-list-element";
            option.value = category;
            option.append(category);
            this.selectorList.append(option);
        }
    }

    OnCategoryChanged(handler) {
        this.selectorList.addEventListener("change", handler);
    }
}