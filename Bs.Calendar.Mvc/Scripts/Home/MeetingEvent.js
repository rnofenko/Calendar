function CalendarEventVm() {
    var self = this;

    self.teamList = ko.observableArray();
    self.userList = ko.observableArray();
}

function TeamVm() {
    var self = this;
    self.Id = 0;
    self.Name = ko.observable();
}

function MeetingEventVm() {
    var self = this;

    self.teamList = ko.observableArray();
    self.eventVm = new CalendarEventVm();
    self.userColumnVm = new UserColumnVm();

    self.isUsersLoaded = false;

    self.clickTeamList = function () {
        if (self.teamList().length > 0 || self.eventVm.teamList().length > 0) return;
        $.getJSON("/Home/GetTeams", null, function(data) {
            ko.mapping.fromJS(data, {}, self.teamList);
        });
    };

    self.getAllUsers = function () {
        $.getJSON("/Home/GetAllUsers", null, function (users) {
            self.userColumnVm.pushToColumns(users);
            self.userColumnVm.columnUsersCount(self.userColumnVm.columnUsersCount() + users.length);
        });
    };

    self.addTeam = function(team) {
        self.teamList.remove(team);
        self.eventVm.teamList.push(team);
    };
    self.removeTeam = function (team) {
        self.eventVm.teamList.remove(team);
        self.teamList.push(team);
    };
    
    self.addUser = function (team) {
        self.userList.remove(team);
        self.eventVm.userList.push(team);
    };
    self.removeUser = function (team) {
        self.eventVm.userList.remove(team);
        self.userList.push(team);
    };

    self.showUserList = function () {
        if (self.isUsersLoaded == false) {
            self.isUsersLoaded = true;
            self.getAllUsers();
        }
        self.userColumnVm.showColumnUserList(!self.userColumnVm.showColumnUserList());
    };
}