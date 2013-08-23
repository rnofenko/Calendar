function UserFrameFilter(frame) {
    var self = this;

    self.SearchString = ko.observable("");
    self.SortByField = ko.observable("");
    self.Page = ko.observable();

    self.OnlyAdmins = ko.observable(false);
    self.NotApproved = ko.observable(false);
    self.Deleted = ko.observable(false);

    self.OnlyAdminsGetCheck = function () { return self.getCheck("#FilterOnlyAdmins"); };
    self.OnlyAdminsSetCheck = function (val) { self.setCheck('#FilterOnlyAdmins', val); };

    self.NotApprovedGetCheck = function () { return self.getCheck("#FilterNotApproved"); };
    self.NotApprovedSetCheck = function (val) { self.setCheck('#FilterNotApproved', val); };

    self.DeletedGetCheck = function () { return self.getCheck("#FilterDeleted"); };
    self.DeletedSetCheck = function (val) { self.setCheck('#FilterDeleted', val); };

    self.getCheck = function (name) {
        return $(name).hasClass("checked");
    };
    self.setCheck = function (name, val) {
        if (val) {
            $(name).trigger('gumby.check');
        } else {
            $(name).trigger('gumby.uncheck');
        }
    };

    self.acceptFilter = function () {
        self.OnlyAdmins(self.OnlyAdminsGetCheck());
        self.NotApproved(self.NotApprovedGetCheck());
        self.Deleted(self.DeletedGetCheck());
        $("#filter_settings").toggleClass("active");

        frame.updateList();
    };

    self.closeWindow = function () {
        $("#filter_settings").toggleClass("active");
    };

    self.showFilterWindow = function () {
        self.OnlyAdminsSetCheck(self.OnlyAdmins());
        self.NotApprovedSetCheck(self.NotApproved());
        self.DeletedSetCheck(self.Deleted());
        $("#filter_settings").toggleClass("active");
    };

    self.displayFilter = ko.computed(function () {
        var noteMessage = [];

        if (self.OnlyAdmins())
            noteMessage.push("Only Admins");
        if (self.NotApproved())
            noteMessage.push("Not approved");
        if (self.Deleted())
            noteMessage.push("Deleted");

        if (noteMessage.length == 0)
            noteMessage.push("Default");

        return noteMessage.join(", ");

    }, self);

    self.setFilter = function (newFilter) {
        self.OnlyAdmins(newFilter.OnlyAdmins);
        self.NotApproved(newFilter.NotApproved);
        self.Deleted(newFilter.Deleted);
        self.Page(newFilter.Page);

        frame.updateList();
    };

    self.getCurrentValues = function () {
        return {
            OnlyAdmins: self.OnlyAdmins(),
            NotApproved: self.NotApproved(),
            Deleted: self.Deleted(),
            SearchString: self.SearchString(),
            SortByField: self.SortByField(),
            Page: self.Page()
        };
    };
}