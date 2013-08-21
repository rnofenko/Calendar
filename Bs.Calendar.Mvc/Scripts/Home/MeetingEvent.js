function MeetingEvent() {
    var self = this;

    self.Title = ko.observable();
    self.Text = ko.observable();
    self.Room = ko.observable();
    self.Teams = ko.observableArray();
    self.Users = ko.observableArray();
    
    self.DateStart = ko.observableArray();
    self.DateEnd = ko.observableArray();

    self.EventType = 2;
}

function MeetingEventVm() {
    var self = this;

    self.isUsersLoaded = false;
    self.isTeamsLoaded = false;
    
    self.eventVm = new MeetingEvent();
    self.simpleTeamListVm = new SimpleTeamListVm(self.eventVm.Teams);
    self.userColumnVm = new UserColumnVm(self.eventVm.Users);

    self.roomList = ko.observableArray();
    $.getJSON("/Room/GetAllRooms", null, function (rooms) {
        ko.mapping.fromJS(rooms, {}, self.roomList);
    });
    
    self.getAllTeams = function() {
        $.getJSON("/Home/GetTeams", null, function(teams) {
            ko.mapping.fromJS(teams, {}, self.simpleTeamListVm.teamList);
        });
    };

    self.getAllUsers = function() {
        $.getJSON("/Home/GetAllUsers", null, function(users) {
            self.userColumnVm.pushToColumns(users);
            self.userColumnVm.columnUsersCount(self.userColumnVm.columnUsersCount() + users.length);
        });
    };

    self.roomOptionText = function(room) {
        return $.validator.format("{0} [{1}]", room.Name(), room.NumberOfPlaces());
    };

    self.roomOptionColor = ko.computed(function () {
        if (self.eventVm.Room() == undefined) return "";
        return $.validator.format("roomColor roomColor_{0}", self.eventVm.Room().Color());
    });

    self.showTeamList = function () {
        if (self.isTeamsLoaded == false) {
            self.isTeamsLoaded = true;
            self.getAllTeams();
        }
        self.userColumnVm.showColumnUserList(false);
        self.simpleTeamListVm.showTeamList(!self.simpleTeamListVm.showTeamList());
    };

    self.showUserList = function () {
        if (self.isUsersLoaded == false) {
            self.isUsersLoaded = true;
            self.getAllUsers();
        }
        self.simpleTeamListVm.showTeamList(false);
        self.userColumnVm.showColumnUserList(!self.userColumnVm.showColumnUserList());
    };
}