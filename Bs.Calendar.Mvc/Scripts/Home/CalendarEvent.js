function CalendarEvent() {
    var self = this;

    self.Id = 0;
    self.Title = ko.observable();
    self.Text = ko.observable();
    self.Room = ko.observable();
    self.Teams = ko.observableArray();
    self.Users = ko.observableArray();
    self.EventType = ko.observable();

    self.DateStart = ko.observable();
    self.DateEnd = ko.observable();
    self.IsAllDay = ko.observable();
}

function EventRoomOptionHandler(selectedRoom) {
    var self = this;

    self.roomList = ko.observableArray();
    self.selectedRoom = selectedRoom;

    $.getJSON("/Room/GetAllRooms", null, function (rooms) {
        ko.mapping.fromJS(rooms, {}, self.roomList);
    });

    self.optionText = function (room) {
        return $.validator.format("{0} [{1}]", room.Name(), room.NumberOfPlaces());
    };

    self.roomOptionColor = ko.computed(function () {
        if (self.selectedRoom() == undefined) return "";
        return $.validator.format("roomColor roomColor_{0}", self.selectedRoom().Color());
    });
}

function EventSubscribersHandler(eventModel) {
    var self = this;

    self.simpleTeamListVm = new SimpleTeamListVm(eventModel.Teams);
    self.userColumnVm = new UserColumnVm(eventModel.Users);

    self.isUsersLoaded = false;
    self.isTeamsLoaded = false;

    self.getAllTeams = function () {
        $.getJSON("/Home/GetTeams", null, function (teams) {
            ko.mapping.fromJS(teams, {}, self.simpleTeamListVm.teamList);
        });
    };

    self.getAllUsers = function () {
        $.getJSON("/Home/GetAllUsers", null, function (users) {
            self.userColumnVm.pushToColumns(users);
            self.userColumnVm.columnUsersCount(self.userColumnVm.columnUsersCount() + users.length);
        });
    };

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

    self.removeUser = function (user) {
        self.userColumnVm.addUser(user);
    };

    self.removeTeam = function (team) {
        self.simpleTeamListVm.addTeam(team);
    };
}

function DateTimeHandler(eventModel) {
    var self = this;

    var dateTimeControl = {
        fromTime: $("#fromTime"),
        toTime: $("#toTime"),
        date: $("#date")
    }; //Setup time range control html elements

    var formatSettings = { date: "YYYY-MM-DD", time: "hh:mm a" };
    var dateDefaults = { initialValue: moment().startOf('day'), initialDifference: { minutes: dateTimeControl.fromTime.timepicker('option', 'step') } };

    var setTime = function (updateMoment, withMoment) {

        return updateMoment
            .hour(withMoment.hour())
            .minute(withMoment.minute())
            .second(withMoment.second());
    };
    var setDate = function (updateMoment, withMoment) {

        return updateMoment
            .year(withMoment.year())
            .month(withMoment.month())
            .date(withMoment.date());
    };

    self.fromDateTime = ko.observable(dateDefaults.initialValue);
    self.toDateTime = ko.observable(dateDefaults.initialValue.clone().add(dateDefaults.initialDifference));

    self.isAllDay = eventModel.IsAllDay;

    self.dateInput = {
        value: ko.computed(function () {
            return self.fromDateTime().format(formatSettings.date);
        }, self),
        min: moment().format(formatSettings.date),
        dateChanged: function (context, event) {

            var dateString = $(event.target).val();

            if (dateString == "") {
                return;
            }

            var newDate = moment(dateString).startOf('day'),
                currentDate = self.fromDateTime(),
                minDate = moment(self.dateInput.min).startOf('day');

            if (newDate < minDate) {
                $(event.target).val(self.dateInput.value());
            }
            else {
                setDate(currentDate, newDate);

                self.fromDateTime(currentDate);
                self.toDateTime(currentDate.clone());
            }
        }
    };

    self.fromInput = {
        value: ko.computed(function () {
            return self.fromDateTime().format(formatSettings.time);
        }, self),
        timeChanged: function (context, event) {
            var fromTime = moment($(event.target).val(), formatSettings.time),
                currentFromTime = self.fromDateTime();
            
            if (!moment.isMoment(fromTime) ||
                !fromTime.isValid() ||
                event.type === "timeFormatError") {
                self.fromDateTime(currentFromTime);
                return;
            }

            setTime(currentFromTime, fromTime);
            self.fromDateTime(currentFromTime);

            //Update "from" time and "to" time if needed
            var toTime = moment(self.toDateTime().format(formatSettings.time), formatSettings.time);

            if (fromTime > toTime) {
                self.toDateTime(currentFromTime.clone());
            }
        }
    };

    self.toInput = {
        value: ko.computed(function () {
            return self.toDateTime().format(formatSettings.time);
        }, self),
        timeChanged: function (context, event) {
            var toTime = moment($(event.target).val(), formatSettings.time),
                currentToTime = self.toDateTime();

            if (!moment.isMoment(toTime) ||
                !toTime.isValid() ||
                event.type === "timeFormatError") {
                self.toDateTime(currentToTime);
                return;
            }
            
            //Update "to" time and "from" time if needed
            var fromTime = moment(self.fromDateTime().format(formatSettings.time), formatSettings.time);

            setTime(currentToTime, toTime);
            self.toDateTime(currentToTime);

            if (toTime < fromTime) {
                self.fromDateTime(currentToTime.clone());
            }
        }
    };
}

function CalendarEventVm(eventModel) {
    var self = this;

    self.eventModel = ko.mapping.fromJS(eventModel, {}, new CalendarEvent());
    if (self.eventModel.Users() == null) self.eventModel.Users([]);
    if (self.eventModel.Teams() == null) self.eventModel.Teams([]);

    self.dateTime = new DateTimeHandler(self.eventModel);
    self.subscribers = new EventSubscribersHandler(self.eventModel);
    self.roomOptions = new EventRoomOptionHandler(self.eventModel.Room);
    self.isError = ko.observable(false);

    self.setEventType = function (event) {
        self.isError(false);
        $('[class*=event-btn]').removeClass('info').addClass('default');
        $(event).toggleClass('default info');
        if ($(event).hasClass('event-btn1')) self.eventModel.EventType(1);
        if ($(event).hasClass('event-btn2')) self.eventModel.EventType(2);
        if ($(event).hasClass('event-btn3')) self.eventModel.EventType(3);
    };

    self.sendModel = function () {
        self.eventModel.DateStart(self.dateTime.fromDateTime().toJSON());
        self.eventModel.DateEnd(self.dateTime.toDateTime().toJSON());

        $.ajax({
            url: self.eventModel.Id != 0 ? "/Home/Edit" : "/Home/CreateEvent",
            data: JSON.stringify(ko.toJS(self.eventModel)),
            success: function (data) { window.location.href = data.redirectToUrl; },
            error: self.onError,
            type: 'POST',
            contentType: 'application/json, charset=utf-8',
            dataType: 'json'
        });
    };

    $("form:first").on("submit", function (e) {
        e.preventDefault();
        if (!self.validateModel()) return;
        self.sendModel();
    });

    self.validateModel = function () {
        $("div[class*='validation-summary'] > ul").empty();
        self.isError(true);

        if (!$("form:first").valid()) return false;
        if (self.eventModel.EventType() == 2 && self.eventModel.Users().length == 0 && self.eventModel.Teams().length == 0) {
            $("div[class*='validation-summary'] > ul").append("<li>At least one Team or User should be specified!</li>");
            return false;
        }

        self.isError(false);
        return true;
    };

    self.onError = function (data) {
        $("div[class|='validation-summary'] > ul").empty();
        $.each(data.responseJSON, function (key, object) {
            $.each(object.errors, function (index, value) {
                $("div[class|='validation-summary'] > ul").append("<li class='danger alert'>" + value + "</li>");
            });
        });
    };
}