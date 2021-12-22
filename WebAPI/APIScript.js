// JavaScript source code

serverSource = {
    url: "http://localhost:5000/api/datastorage",
    catUrl: "http://localhost:5000/api/datastorage/categories", 
    category: "None",
    URI: function () { return this.url + "/" + this.category; },
}

pageComponents = {
    slider: new Slider(),
    selector: new CategorySelector(),
}

function GetImagesInfo() {
    var imagesReq = new XMLHttpRequest();
    imagesReq.open("GET", serverSource.URI(), true);
    imagesReq.addEventListener("load", function () {
        let data = JSON.parse(imagesReq.responseText);
        pageComponents.slider.LoadData(data);
    });
    imagesReq.send(null);
}

function GetCategories() {
    var categoriesReq = new XMLHttpRequest();
    categoriesReq.open("GET", serverSource.catUrl, true);
    categoriesReq.addEventListener("load", function () {
        pageComponents.selector.LoadData(JSON.parse(categoriesReq.responseText));
        pageComponents.selector.OnCategoryChanged(OnCategoryChanged);
    });
    categoriesReq.send(null);
}

function OnCategoryChanged() {
    serverSource.category = this.options[this.selectedIndex].text;
    GetImagesInfo();
}

GetImagesInfo();
GetCategories();
