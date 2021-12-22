// JavaScript source code

class TableContent {
    constructor(documentElement) {
        this.table = documentElement;
        this.data = [];
    }

    clear() {
        for (let row = this.table.rows.length - 1; row > 0; row--) {
            this.table.deleteRow(row);
        }
    }

    LoadData(data) {
        this.data = [];
        for (let i = 0; i < data.length; i++) {
            this.data.push(data[i].value);
        }
        this.Show(0);
    }

    Show(index) {
        this.clear();
        for (let ind = 0; ind < this.data[index].length; ind++) {
            let tr = document.createElement("tr");
            let td1 = document.createElement("td");
            let td2 = document.createElement("td");
            td1.textContent = this.data[index][ind].categoryName;
            td2.textContent = this.data[index][ind].confidence.toFixed(2);
            tr.append(td1);
            tr.append(td2);
            this.table.append(tr);
        }
    }
}