
function EventVm(eventContent, date) {
    var self = this;
    self.eventContent = eventContent;
    self.eventDate = date;
    //self.event_type = event_type;
};

function DayVm(date, events) {
    var self = this;
    self.day = date.date();
    self.date = date;
    
    self.events = ko.observableArray(events || []);

    self.defineClass = ko.computed(function() {
        if (self.date < moment().startOf('month') || self.date > moment().endOf('month')) {
            return "bc-month-day other-month";
        }
        if (self.date.unix() == moment().startOf('day').unix())
            return "bc-month-day current-day";
        return "bc-month-day";
    });
};

function WeekVm(days) {
    var self = this;
    self.days = days;
};

function MonthVm() {
    var self = this;

    self.weeks = ko.observableArray([]);
    self.monthStart = moment().startOf('month');
    self.title = ko.observable();


    self.getEvents = function () {
        var events = [];
        var from = moment(self.monthStart).day("Sunday");
        var timeRange = { from: from.toJSON(), to: from.add('weeks', 6).toJSON() };
        
        $.ajax({
            url: "/Home/GetEvents",
            async: false,
            type: "GET",
            data: timeRange,
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function(data) {
            $.each(data, function(key, value) {
                events.push(new EventVm(value.Text, moment(value.Date)));
            });
        });
        return events;
    };

    self.getDayEvents = function (events, date) {
        return $.grep(events, function (element) {
            return date.unix() == element.eventDate.unix();
        });
    };

    self.monthInit = function () {
        var month = moment(self.monthStart);
        self.title(month.format("MMMM") + " " + month.format("YYYY"));
        
        var date = month.day("Sunday");
        var days = [];
        var events = self.getEvents();
        
        for (var row = 0; row < 6; row++) {
            for (var col = 0; col < 7; col++) {
                var ev = self.getDayEvents(events, date);
                days.push(new DayVm(date, ev));
                date = date.add("days", 1);
            }
            self.weeks.push(new WeekVm(days));
            days = [];
        }
    };

    self.updateMonth = function () {
        self.weeks.removeAll();
        self.monthInit();
    };
    
    self.nextMonth = function() {
        self.monthStart.add("month", 1);
        self.updateMonth();
    };
    
    self.prevMonth = function () {
        self.monthStart.subtract("month", 1);
        self.updateMonth();
    };

    self.today = function() {
        self.monthStart = moment().startOf('month');
        self.updateMonth();
    };
};

