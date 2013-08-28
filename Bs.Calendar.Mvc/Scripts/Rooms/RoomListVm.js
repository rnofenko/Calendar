function RoomFilterVm() {
    var self = this;

    self.searchString = ko.observable("");
    self.sortByField = ko.observable("");

    self.page = ko.observable();

    self.getFilter = function () {
        return {
            SearchString: self.searchString(),
            SortByField: self.sortByField(),
            Page: self.page()
        };
    };
    
    self.setFilter = function (filter) {
        self.searchString(filter.SearchString);
        self.page(filter.Page);
        self.sortByField(filter.SortByField);
    };
}

function RoomListVm() {
    var self = this;

    self.filter = new RoomFilterVm();
    self.totalPages = ko.observable();

    self.arrowUp = false;
    self.currColumnId = "";

    self.updateList = function() {
        $.get("/Room/List", self.filter.getFilter(), function (html) {
            //Update list content and paging controls
            $("#rooms-table").html(html);
            
            self.filter.page($(html).filter("#listPage").val());
            self.totalPages($(html).filter("#listTotalPages").val());
        });
    };
    
    self.previousPage = function () {
        var prevPage = Number(self.filter.page()) - 1;

        if (prevPage >= 1) {
            self.filter.page(prevPage);
            self.updateList();
        }
    };

    self.nextPage = function() {
        var nextPage = Number(self.filter.page()) + 1;

        if (nextPage <= self.totalPages()) {
            self.filter.page(nextPage);
            self.updateList();
        }
    };

    self.changeHandle = function (event) {
        self.updateList();
    };
    
    self.headerClickHandle = function (data, event) {
        showArrow('#' + event.currentTarget.id);
        self.filter.sortByField($(event.currentTarget).data("field").concat(self.arrowUp ? "" : " Desc"));
        self.updateList();
    };
    
    function showArrow(columnId) {
        $(self.currColumnId + " > i").remove();

        if (self.currColumnId != columnId) {
            self.arrowUp = false;
            self.currColumnId = columnId;
        }

        self.arrowUp = !self.arrowUp;
        $(columnId).append($('<i/>', { "class": self.arrowUp ? "icon-up" : "icon-down" }));
    };
};