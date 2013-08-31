var eventTypes = {
    personal: 1,
    meeting: 2
};

var formatSettings = {
    timeFormat: "h:mma",
    dateFormat: "DD MMM YYYY"
};

function CalendarCellEventVm() {
    var self = this;

    self.Id = 0;

    self.Title = "";

    self.DateStart = moment();
    self.DateEnd = moment();
    self.IsAllDay = false;

    self.Room = null;
    self.EventType = eventTypes.personal;

    self.map = function(event) {
        self.Id = event.Id;

        self.Title = event.Title;
        self.DateStart = moment(event.DateStart);
        self.DateEnd = moment(event.DateEnd);
        self.IsAllDay = event.IsAllDay;
        self.Room = event.Room;
        self.EventType = event.EventType;
    };

    self.getTitle = function () {
        if (self.EventType == eventTypes.personal) {
            return self.DateStart.format(formatSettings.timeFormat) + " " + self.Title;
        } else {
            return self.DateStart.format(formatSettings.timeFormat) + "-" + self.DateEnd.format(formatSettings.timeFormat) + " "+ self.Title;
        }
    };
    
    self.setAttributes = function () {

        console.log(self.Room);
        return {
            "class": (function () {
                var mainClass = "clickable bc-event button-like";
                return self.Room == null ? mainClass + " bc-event-calendar " : mainClass + " roomColor_" + (self.Room.Color);
            })()
        };
    };

    self.clickHandle = function (context, event) {
        $.getJSON("/Home/Edit", { id: self.Id }, function(data) {
            window.location.href = data.redirectToUrl;
        });
    };
};

function BirthdayEventVm() {
    var self = this;

    self.Date = moment();
    self.Text = "";
    
    self.map = function(event) {
        self.Date = moment(event.Date);
        self.Text = event.Text;
    };
    
    self.getTitle = function () {
        return self.Text;
    };
};

function DayVm(date, events) {
    var self = this;
    
    self.day = date.date();
    self.date = moment(date);

    self.events =
        {
            CalendarEvents: ko.observableArray(events.CalendarEvents || []),
            BirthdayEvents: ko.observableArray(events.BirthdayEvents || [])
        };
    
    self.clickHandle = function() {
        //create event

        $("#month-dialog-form").
        $("#month-dialog-form").className = $("#month-dialog-form").className.replace(/\bui-.*?\b/g, '');
    };
};

function CreateEventDialogVm() {
    var self = this;

    self.title = ko.observable("");
    self.text = ko.observable("");
    
    self.acceptHandle
};

function WeekVm(days) {
    var self = this;
    self.days = days;
};

function MonthVm() {
    var self = this;

    self.weeks = ko.observableArray([]);
    self.monthStart = ko.observable(moment().startOf('month'));
    self.title = ko.observable();
    self.isChosen = ko.observable(true);

    self.monthInit = function () {
        var month = moment(self.monthStart());
        self.title(month.format("MMMM") + " " + month.format("YYYY"));
        mediator.trigger("Calendar:setPanelTitle", self.title());

        var date = month.day("Sunday");
        var events = self.getEvents();

        for (var row = 0; row < 6; row++) {
            var days = [];
            for (var col = 0; col < 7; col++) {
                days.push(new DayVm(date, self.getDayEvents(events, date)));
                date = date.add("days", 1);
            }
            
            self.weeks.push(new WeekVm(days));
        }
    };

    self.defineClass = function (date) {
        if (date < self.monthStart() || date > moment(self.monthStart()).endOf('month'))
            return "bc-month-day other-month button-like-colorful";
        if (date.isSame(moment().startOf('day')))
            return "bc-month-day current-day button-like-colorful";
        return "bc-month-day button-like-colorful";
    };

    //Event handlers
    self.getEvents = function () {
        var events = {
            BirthdayEvents: [],
            CalendarEvents: []
        };
        var from = moment(self.monthStart()).day("Sunday");
        var timeRange = { from: from.toJSON(), to: from.add('weeks', 6).toJSON() };
        
        $.ajax({
            url: "/Home/List",
            async: false,
            type: "GET",
            data: timeRange,
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (data) {
            $.each(data.BirthdayEvents, function (key, value) {
                var event = new BirthdayEventVm();
                event.map(value);
                
                events.BirthdayEvents.push(event);
            });
            $.each(data.CalendarEvents, function (key, value) {
                var event = new CalendarCellEventVm();
                event.map(value);

                events.CalendarEvents.push(event);
            });
        });
        
        return events;
    };
    
    self.getDayEvents = function (events, date) {
        var filteredEvents = {
            CalendarEvents: $.grep(events.CalendarEvents, function (event) {
                return event.DateStart.clone().startOf('day').isSame(date.clone().startOf('day'));
            }),
            BirthdayEvents: $.grep(events.BirthdayEvents, function (event) {
                return moment(date).year(event.Date.year()).isSame(event.Date);
            })
        };
        
        return filteredEvents;
    };

    //Calendar month manipulator
    self.updateMonth = function () {
        self.weeks.removeAll();
        self.monthInit();
    };

    self.nextMonth = function () {
        if (!self.isChosen()) return;
        self.monthStart(self.monthStart().add("month", 1));
        self.updateMonth();
    };
    self.prevMonth = function () {
        if (!self.isChosen()) return;
        self.monthStart(self.monthStart().subtract("month", 1));
        self.updateMonth();
    };

    self.today = function () {
        self.monthStart(moment().startOf('month'));
        self.updateMonth();
    };

    self.setMode = function(mode) {
        self.isChosen(mode == 'month');
        if (self.isChosen()) self.updateMonth();
    };
    
    //Initialize
    self.monthInit();
    
    //Setup bindings
    mediator.bind("Calendar:nextButton", self.nextMonth);
    mediator.bind("Calendar:prevButton", self.prevMonth);
    mediator.bind("Calendar:setMode", self.setMode);
};

