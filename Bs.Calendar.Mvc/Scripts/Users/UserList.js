function UserFrame() {
    var self = this;

    self.TotalPages = ko.observable();
    self.filter = new UserFrameFilter(self);

    self.arrowUp = false;
    self.currColumnId = "";

    //Methods
    self.headerClick = function(data, event) {
        self.showArrow('#' + event.currentTarget.id);
        self.filter.SortByField($(event.currentTarget).data("field").concat(self.arrowUp ? "" : " Desc"));
        self.updateList();
    };

    self.previousPage = function() {
        var page = Number(self.filter.Page()) - 1;
        if (page >= 1) {
            self.filter.Page(page);
            self.updateList();
        }
    };

    self.nextPage = function () {
        var page = Number(self.filter.Page()) + 1;
        if (page <= self.TotalPages()) {
            self.model.Page(page);
            self.updateList();
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

    self.updateList = function () {
        $.get("Users/List", self.filter.getCurrentValues(), function (html) {
            $("#list-body").html(html);
            self.filter.Page($(html).filter("#listPage").val());
            self.TotalPages($(html).filter("#listTotalPages").val());
        }, "html");
    };

    self.changeHandler = function () {
        self.updateList();
    };
}
