
function EventVm(eventContent, date) {
    var self = this;
    self.eventContent = eventContent;
    self.eventDate = date;
    //self.event_type = event_type;
};

function DayVm(date, events) {
    var self = this;
    self.day = date.date();
    self.date = moment(date);
    self.events = ko.observableArray(events || []);
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

    self.monthInit = function () {
        var month = moment(self.monthStart);
        self.title(month.format("MMMM") + " " + month.format("YYYY"));
        
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
        if (date < self.monthStart || date > moment(self.monthStart).endOf('month'))
            return "bc-month-day other-month";
        if (date.isSame(moment().startOf('day')))
            return "bc-month-day current-day";
        return "bc-month-day";
    };

    //Event handlers
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
        }).done(function (data) {
            $.each(data, function (key, value) {
                events.push(new EventVm(value.Text, moment(value.Date)));
            });
        });
        return events;
    };

    self.getDayEvents = function (events, date) {       //Only birthday events for now
        return $.grep(events, function (element) {
            return moment(date).year(element.eventDate.year()).isSame(element.eventDate);
        });
    };

    //Calendar month manipulator
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

