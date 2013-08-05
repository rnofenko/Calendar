function PagingVm() {
    var self = this;
    self.TotalPages = ko.observable();
    self.Page = ko.observable();
    self.SortByStr = ko.observable();
    self.SearchStr = ko.observable();
}

function ViewModel() {
    var self = this;

    //Variables
    self.model = {SearchStr : ko.observable(""), SortByStr : ko.observable(""), Page : ko.observable(), TotalPages : ko.observable()};

    self.arrowUp = false;
    self.currColumnId = "";
    self.listBodyId = "#list-body";

    //Methods
    self.headerClick = function(data, event) {
        self.showArrow('#' + event.currentTarget.id);

        self.model.SortByStr(event.currentTarget.innerText.concat(self.arrowUp ? "" : "Desc"));
        self.updateList(self.model);
    };

    self.previousPage = function() {
        var page = Number(self.model.Page()) - 1;
        if (page >= 1) {
            self.model.Page(page);
            self.updateList(self.model);
        }
    };

    self.nextPage = function () {
        var page = Number(self.model.Page()) + 1;
        if (page <= self.model.TotalPages()) {
            self.model.Page(page);
            self.updateList(self.model);
        }
    };

    self.showArrow = function(columnId) {
        $(self.currColumnId + " > i").remove();

        if (self.currColumnId != columnId) {
            self.arrowUp = false;
            self.currColumnId = columnId;
        }

        self.arrowUp = !self.arrowUp;
        $(columnId).append($('<i/>', { "class": self.arrowUp ? "icon-up" : "icon-down" }));
    };

    self.updateList = function(model) {
        $.get("Users/List", model, function(htmlData) {
            $(self.listBodyId).html(htmlData);
            self.model.Page($(htmlData).filter("#listPage").val());
            self.model.TotalPages($(htmlData).filter("#listTotalPages").val());
            self.model.SearchStr($(htmlData).filter("#listSearchStr").val());
            self.model.SortByStr($(htmlData).filter("#listSortByStr").val());
        }, "html");
    };

    self.changeHandler = function () {
        self.updateList(self.model);
    };
}