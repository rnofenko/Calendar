function ColumnModel(users, title) {
    var self = this;
    self.title = ko.observable(title);
    self.allUsers = ko.observableArray(users);
}

function TeamModel() {
    var self = this;
    self.TeamId = 0;
    self.Name = ko.observable();
    self.Users = ko.observableArray();
}

function TeamUsersVm(editModel) {
    var self = this;

    self.showTeamUsers = ko.observable(true);
    self.userColumns = ko.observableArray();
    self.titles = editModel.HeaderPattern.split(',');

    self.teamModel = ko.mapping.fromJS(editModel, TeamModel);
    if (self.teamModel.Users() == null) self.teamModel.Users = ko.observableArray();

    self.addUser = function (columnModel, user) {
        columnModel.allUsers.remove(user);
        if (self.teamModel.Users() == null) self.teamModel.Users = ko.observableArray();
        self.teamModel.Users.push(user);
    };

    self.removeUser = function (user) {
        self.teamModel.Users.remove(user);
    };
    
    self.getColumnsUsers = function (users, column) {
        return $.grep(users, function (element) {
            return new RegExp('^[' + column + ']').test(element.Name);
        });
    };

    self.getOtherUsers = function () {
        $.getJSON("/Team/GetAllUsers", { teamId: self.teamModel.TeamId }, function (users) {
            $.each(self.titles, function (key, title) {
                var columnUsers = self.getColumnsUsers(users, title);
                self.userColumns.push(new ColumnModel(columnUsers, title));
            });
        });
    };

    self.clickAllUsers = function () {
        if (self.userColumns()[0] == undefined) {
            self.getOtherUsers();
        }
        self.showTeamUsers(false);
    };
    self.clickTeamUsers = function () {
        self.showTeamUsers(true);
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

    self.onError = function (data) {
        $("div[class|='validation-summary'] > ul").empty();
        $.each(data.responseJSON, function(key, object) {
            $.each(object.errors, function(index, value) {
                $("div[class|='validation-summary'] > ul").append("<li class='danger alert'>" + value + "</li>");
            });
        });
    };
}