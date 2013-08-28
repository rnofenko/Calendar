function RoomFilterVm() {
    var self = this;

    self.searchString = ko.observable("");
    self.sortByField = ko.observable("");

    self.page = ko.observable();

    self.getCurrentValues = function () {
        return {
            SearchString: self.SearchString(),
            SortByField: self.SortByField(),
            Page: self.Page()
        };
    };
}

function RoomListVm() {
    var self = this;

    self.filter = new RoomFilterVm();

    self.updateList = function() {
        if (/^[\s\t]*$/.test(self.filter.searchString())) {
            return;
        }

        $.get("/Room/List", self.filter, function(data, status, jqXHR) {
            console.log(data);
            console.log(status);
            console.log(jqXHR);
        });

        self.changeHandle = function () {
            self.updateList();
        };

    };

};