var eventTypes = {
    personal: 1,
    meeting: 2
};

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

function EventRoomOptionHandler(room) {
    var self = this;

    self.selectedRoom = room;

    self.isRoomDefined = ko.computed(function() {
        return self.selectedRoom() != undefined && self.selectedRoom() != null;
    });

    self.roomText = ko.computed(function () {
        if (self.isRoomDefined())
            return $.validator.format("{0} [{1}]", self.selectedRoom().Name(), self.selectedRoom().NumberOfPlaces());
        else
            return "None";
    });

    self.setRoom = function(roomToSet) {
        self.selectedRoom(roomToSet);
    };

    self.clearRoom = function() {
        self.selectedRoom(null);
    };
    
    //Setup bindings
    mediator.bind("EventRoomHandler:setRoom", self.setRoom);
    mediator.bind("EventRoomHandler:clearRoom", self.clearRoom);
}

function EventSubscribersHandler(eventModel) {
    var self = this;

    self.simpleTeamListVm = new SimpleTeamListVm(eventModel.Teams);
    self.userColumnVm = new UserColumnVm(eventModel.Users);

    self.isUsersLoaded = false;
    self.isTeamsLoaded = false;

    self.getAllTeams = function () {
        $.getJSON("/Event/GetTeams", null, function (teams) {
            ko.mapping.fromJS(teams, {}, self.simpleTeamListVm.teamList);
        });
    };

    self.getAllUsers = function () {
        $.getJSON("/Event/GetAllUsers", null, function (users) {
            self.userColumnVm.pushToColumns(users);
            self.userColumnVm.columnUsersCount(self.userColumnVm.columnUsersCount() + users.length);
        });
    };

    self.showTeamList = function () {
        if (self.isTeamsLoaded == false) {
            self.isTeamsLoaded = true;
            self.getAllTeams();
        }
        mediator.trigger("RoomOrderListVm:hideRoomOrderList");
        self.userColumnVm.showColumnUserList(false);
        self.simpleTeamListVm.showTeamList(!self.simpleTeamListVm.showTeamList());
    };

    self.showUserList = function () {
        if (self.isUsersLoaded == false) {
            self.isUsersLoaded = true;
            self.getAllUsers();
        }
        mediator.trigger("RoomOrderListVm:hideRoomOrderList");
        self.simpleTeamListVm.showTeamList(false);
        self.userColumnVm.showColumnUserList(!self.userColumnVm.showColumnUserList());
    };

    self.displayRoomOrderList = function() {
        self.userColumnVm.showColumnUserList(false);
        self.simpleTeamListVm.showTeamList(false);
        mediator.trigger("RoomOrderListVm:invertRoomOrderListView");
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
    console.log(eventModel);
    var formatSettings = { date: "YYYY-MM-DD", time: "H:mm" };
    var timeRangeSettings = {
        minTime: moment().setTime(moment("8:00", formatSettings.time)),
        maxTime: moment().setTime(moment("18:00", formatSettings.time)),
        step: 60
    };
    
    var dateTimeControl = {
        fromTime: $("#fromTime"),
        toTime: $("#toTime"),
        date: $("#date"),
        isAllDay: $("#isAllDay")
    }; //Setup time range control html elements

    $(dateTimeControl.fromTime).add(dateTimeControl.toTime).timepicker({
        timeFormat: "H:i",
        step: timeRangeSettings.step,
        minTime: timeRangeSettings.minTime.format(formatSettings.time),
        maxTime: timeRangeSettings.maxTime.format(formatSettings.time)
    });
    
    var dateDefaults = { initialValue: timeRangeSettings.minTime.clone(), initialDifference: { minutes: timeRangeSettings.step } };
    
    self.IsAllDay = ko.observable(eventModel.IsAllDay());
    dateTimeControl.isAllDay.trigger("gumby." + (self.IsAllDay() ? "check" : "uncheck"));

    self.fromDateTime = ko.observable(eventModel.Id != 0 ? moment(eventModel.DateStart()) : dateDefaults.initialValue);
    self.toDateTime = ko.observable(eventModel.Id != 0 ? moment(eventModel.DateEnd()) : dateDefaults.initialValue.clone().add(dateDefaults.initialDifference));

    self.dateInput = {
        value: ko.computed(function () {
            return self.fromDateTime().format(formatSettings.date);
        }, self),
        min: timeRangeSettings.minTime.format(formatSettings.date),
        dateChanged: function (context, event) {

            var dateString = $(event.target).val();

            if (dateString == "") {
                return;
            }

            var newDate = moment(dateString).startOf('day');
            
            mediator.trigger("EventRoomHandler:clearRoom", self.clearRoom);
            mediator.trigger("EventDateTime:dateUpdate", newDate);

            if (newDate < timeRangeSettings.minTime.clone().startOf('day')) {
                $(event.target).val(self.dateInput.value());
            }
            else {
                self.fromDateTime(newDate.clone().setTime(self.fromDateTime()));
                self.toDateTime(newDate.clone().setTime(self.toDateTime()));
            }
        }
    };

    self.fromInput = {
        value: ko.computed(function () {
            return self.fromDateTime().format(formatSettings.time);
        }, self),
        timeChanged: function (context, event) {
            var currentFromTime = self.fromDateTime(),
                fromTime = moment($(event.target).val(), formatSettings.time);

            timeRangeSettings.minTime.clone().setDate(fromTime);
            
            if (!moment.isMoment(fromTime) ||
                !fromTime.isValid() ||
                fromTime < timeRangeSettings.minTime.clone().setDate(fromTime) ||
                fromTime > timeRangeSettings.maxTime.clone().setDate(fromTime) ||
                event.type === "timeFormatError") {
                self.fromDateTime(currentFromTime);
                return;
            }

            self.fromDateTime(currentFromTime.setTime(fromTime));

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
            var currentToTime = self.toDateTime(),
                toTime = moment($(event.target).val(), formatSettings.time);

            if (!moment.isMoment(toTime) ||
                !toTime.isValid() ||
                toTime < timeRangeSettings.minTime.clone().setDate(toTime) ||
                toTime > timeRangeSettings.maxTime.clone().setDate(toTime) ||
                event.type === "timeFormatError") {
                self.toDateTime(currentToTime);
                return;
            }
            
            //Update "to" time and "from" time if needed
            var fromTime = moment(self.fromDateTime().format(formatSettings.time), formatSettings.time);

            self.toDateTime(currentToTime.setTime(toTime));

            if (toTime < fromTime) {
                self.fromDateTime(currentToTime.clone());
            }
        }
    };

    self.setFromDateTime = function(time) {
        self.fromDateTime().hours(time.hours()).minutes(time.minutes());
        self.fromDateTime(self.fromDateTime());
    };
    self.setToDateTime = function(time) {
        self.toDateTime().hours(time.hours()).minutes(time.minutes());
        self.toDateTime(self.toDateTime());
    };
    
    //Setup bindings
    mediator.bind("DateTimeHandler:setFromTime", self.setFromDateTime);
    mediator.bind("DateTimeHandler:setToTime", self.setToDateTime);
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
    
    if (eventModel.EventType == eventTypes.personal) {
        self.setEventType($(".event-btn1"));
    } else {
        self.setEventType($(".event-btn2"));
    }

    self.sendModel = function () {
        self.eventModel.DateStart(self.dateTime.fromDateTime().toJSON());
        self.eventModel.DateEnd(self.dateTime.toDateTime().toJSON());
        self.eventModel.IsAllDay(self.dateTime.IsAllDay());

        $.ajax({
            url: self.eventModel.Id != 0 ? "/Event/Edit" : "/Event/Create",
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