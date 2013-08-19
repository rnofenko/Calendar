function TeamModel() {
    var self = this;
    self.TeamId = 0;
    self.Name = ko.observable();
    self.Users = ko.observableArray();
    self.IsDeleted = false;
}

function TeamUsersVm(editModel) {
    var self = this;

    self.teamModel = ko.mapping.fromJS(editModel, TeamModel);
    if (self.teamModel.Users() == null) self.teamModel.Users = ko.observableArray();

    self.userColumnVm = new UserColumnVm(self.teamModel.Users);
    self.isOtherUsersAdded = false;

    self.getOtherUsers = function () {
        $.getJSON("/Team/GetAllUsers", { teamId: self.teamModel.TeamId }, function (users) {
            self.userColumnVm.pushToColumns(users);
            self.userColumnVm.columnUsersCount(self.userColumnVm.columnUsersCount() + users.length);
        });
    };

    self.clickAllUsers = function () {
        if (!self.isOtherUsersAdded) {
            self.isOtherUsersAdded = true;
            self.getOtherUsers();
        }
        self.userColumnVm.showColumnUserList(true);
    };
    self.clickTeamUsers = function () {
        self.userColumnVm.showColumnUserList(false);
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