function ColumnModel(users, title) {
    var self = this;

    self.title = ko.observable(title);
    self.teamUsers = ko.observableArray(users);
    self.allUsers = ko.observableArray();
}


function TeamUsersVm(editModel, titles) {
    var self = this;

    self.teamId = editModel.TeamId;

    self.showTeamUsers = ko.observable(true);
    self.userColumns = ko.observableArray();
    self.teamUserIds = ko.observableArray();

    self.getColumnsUsers = function(users, column) {
        if (users == null)
            return [];
        return $.grep(users, function(element) {
            return new RegExp('^[' + column + ']').test(element.Name);
        });
    };

    $.each(titles, function(key, title) {
        var columnUsers = self.getColumnsUsers(editModel.Users, title);
        self.userColumns.push(new ColumnModel(columnUsers, title));
    });

    $.getJSON("/Team/GetAllUsers", { teamId: self.teamId }, function(users) {
        $.each(self.userColumns(), function(key, model) {
            var columnUsers = self.getColumnsUsers(users, model.title());
            model.allUsers(columnUsers);
        });
    });

    if (editModel.Users != null) {
        $.each(editModel.Users, function(index, user) {
            self.teamUserIds.push(user.UserId);
        });
    }

    self.moveUser = function (columnModel, user) {
        if (self.showTeamUsers()) self.removeUser(columnModel, user);
        else self.addUser(columnModel, user);
    };

    self.addUser = function (columnModel, user) {
        columnModel.allUsers.remove(user);
        columnModel.teamUsers.push(user);
        self.teamUserIds.push(user.UserId);
    };

    self.removeUser = function (columnModel, user) {
        columnModel.teamUsers.remove(user);
        columnModel.allUsers.push(user);
        self.teamUserIds.remove(user.UserId);
    };

    self.iconClass = ko.computed(function () {
        var classText = self.showTeamUsers() ? 'icon-cancel-squared pull_right'
            : 'icon-plus-squared pull_right';
        return classText;
    });

    self.clickAllUsers = function () {
        self.showTeamUsers(false);
    };
    self.clickTeamUsers = function () {
        self.showTeamUsers(true);
    };
}