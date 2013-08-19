function ColumnModel(title) {
    var self = this;
    self.title = ko.observable(title);
    self.allUsers = ko.observableArray();
    
    self.shortName = function (fullName) {
        var strings = ko.unwrap(fullName).split(" ");
        if (strings.length == 1) return strings[0];
        return strings[0] + ' ' + strings[1].charAt(0) + '.';
    };
}

function UserColumnVm(usersArray) {
    var self = this;

    self.userColumns = ko.observableArray();
    self.removedUsers = usersArray;
    self.showColumnUserList = ko.observable(false);
    self.columnUsersCount = ko.observable(0);

    self.getTitles = function () {
        jQuery.ajaxSetup({ async: false });
        var str = "";
        $.getJSON("/Users/GetHeaderPattern", null, function(data) {
            str = data;
        });
        jQuery.ajaxSetup({ async: true });
        return str.split(',');
    };

    self.titles = self.getTitles();

    $.each(self.titles, function (key, title) {
        self.userColumns.push(new ColumnModel(title));
    });

    self.getColumnsUsers = function (users, column) {
        return $.grep(users, function (element) {
            return new RegExp('^[' + column + ']', 'i').test(ko.unwrap(element.FullName));
        });
    };

    self.pushToColumns = function (users) {
        $.each(self.titles, function (key, title) {
            $.each(self.getColumnsUsers(users, title), function (index, value) {
                self.userColumns()[key].allUsers.push(value);
            });
        });
    };
    
    self.removeUser = function (columnModel, user) {
        columnModel.allUsers.remove(user);
        self.removedUsers.push(user);
        self.columnUsersCount(self.columnUsersCount() - 1);
    };
    
    self.addUser = function (user) {
        self.removedUsers.remove(user);
        self.pushToColumns([user]);
        self.columnUsersCount(self.columnUsersCount() + 1);
    };
}