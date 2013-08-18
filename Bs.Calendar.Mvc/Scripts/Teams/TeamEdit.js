function ColumnModel(title) {
    var self = this;
    self.title = ko.observable(title);
    self.allUsers = ko.observableArray();
}

function TeamModel() {
    var self = this;
    self.TeamId = 0;
    self.Name = ko.observable();
    self.Users = ko.observableArray();
    self.IsDeleted = false;
}

function TeamUsersVm(editModel) {
    var self = this;

    self.nonTeamUsersCount = ko.observable(0);
    self.showTeamUsers = ko.observable(true);
    self.isOtherUsersAdded = false;
    self.userColumns = ko.observableArray();
    self.titles = editModel.HeaderPattern.split(',');

    self.teamModel = ko.mapping.fromJS(editModel, TeamModel);
    if (self.teamModel.Users() == null) self.teamModel.Users = ko.observableArray();

    $.each(self.titles, function (key, title) {
        self.userColumns.push(new ColumnModel(title));
    });

    self.addUser = function (columnModel, user) {
        columnModel.allUsers.remove(user);
        self.teamModel.Users.push(user);
        self.nonTeamUsersCount(self.nonTeamUsersCount() - 1);
    };

    self.removeUser = function (user) {
        self.teamModel.Users.remove(user);
        self.pushToColumns([user]);
        self.nonTeamUsersCount(self.nonTeamUsersCount() + 1);
    };
    
    self.getColumnsUsers = function (users, column) {
        return $.grep(users, function (element) {
            return new RegExp('^[' + column + ']', 'i').test(ko.unwrap(element.FullName));
        });
    };

    self.pushToColumns = function(users) {
        $.each(self.titles, function (key, title) {
            $.each(self.getColumnsUsers(users, title), function (index, value) {
                self.userColumns()[key].allUsers.push(value);
            });
        });
    };

    self.getOtherUsers = function () {
        $.getJSON("/Team/GetAllUsers", { teamId: self.teamModel.TeamId }, function (users) {
            self.pushToColumns(users);
            self.nonTeamUsersCount(self.nonTeamUsersCount() + users.length);
        });
    };

    self.clickAllUsers = function () {
        if (!self.isOtherUsersAdded) {
            self.isOtherUsersAdded = true;
            self.getOtherUsers();
        }
        self.showTeamUsers(false);
    };
    self.clickTeamUsers = function () {
        self.showTeamUsers(true);
    };

    self.shortName = function(fullName) {
        var strings = ko.unwrap(fullName).split(" ");
        if (strings.length == 1) return strings[0];
        return strings[0] + ' ' + strings[1].charAt(0) + '.';
    };

    self.actionUrl = function() {
        if (self.teamModel.TeamId() != 0) return "/Team/Edit";
        return "/Team/Create";
    };

    self.onSubmit = function(formElement) {
        if ($(formElement).valid()) {
            $.ajax({
                url: self.actionUrl(),
                data: JSON.stringify(ko.toJS(self.teamModel)),
                success: function (data) { window.location.href = data.redirectToUrl; },
                error: self.onError,
                type: 'POST',
                contentType: 'application/json, charset=utf-8',
                dataType: 'json'
            });
        }
    };

    self.onDelete = function() {
        self.teamModel.IsDeleted = true;
        $("#team-edit-form").submit();
    };

    self.onError = function (data) {
        $("div[class|='validation-summary'] > ul").empty();
        $.each(data.responseJSON, function(key, object) {
            $.each(object.errors, function(index, value) {
                $("div[class|='validation-summary'] > ul").append("<li class='danger alert'>" + value + "</li>");
            });
        });
    };
}