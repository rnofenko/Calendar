function CalendarCellEventVm() {
    var self = this;

    self.Id = ko.observable(0);
    self.Title = ko.observable("");
    self.DateStart = ko.observable();
    self.DateEnd = ko.observable();
    self.IsAllDay = ko.observable();
    self.Room = ko.observable();
    self.EventType = ko.observable();
};

function BirthdayEventVm() {
    var self = this;

    self.Date = ko.observable();
    self.Text = ko.observable();
};

function EventFilterVm() {
    var self = this;

    self.FromDate = ko.observable();
    self.ToDate = ko.observable();
};

function CalendVm() {
    var self = this;

    self.filter = ko.observable();

    self.birthdayEvents = ko.observableArray();
    self.calendarEvents = ko.observableArray();
};

function CalendarVm() {
    var self = this;

    self.title = ko.observable();

    self.nextButton = function() {
        mediator.trigger("Calendar:nextButton");
    };

    self.prevButton = function() {
        mediator.trigger("Calendar:prevButton");
    };

    self.showWeek = function () { mediator.trigger("Calendar:setMode", "week"); };
    self.showMonth = function () { mediator.trigger("Calendar:setMode", "month"); };
    self.showDay = function () { mediator.trigger("Calendar:setMode", "day"); };

    self.setTitle = function(title) {
        self.title(title);
    };

    //Setup bindings
    mediator.bind("Calendar:setPanelTitle", self.setTitle);
}