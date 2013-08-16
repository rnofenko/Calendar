function TeamFrameFilter(frame) {
    var self = this;

    self.SearchString = ko.observable("");
    self.SortByField = ko.observable("");
    self.Page = ko.observable();

    self.getCurrentValues = function () {
        return {
            SearchString: self.SearchString(),
            SortByField: self.SortByField(),
            Page: self.Page()
        };
    };
    
    
}