
window.EventVm = function(event_content, event_type){
    self = this;
    self.event_content = event_content;
    self.event_type = event_type;
};
window.DayVm = function (day_title, events) {
    var self = this;
    self.day_title = day_title;
    self.events = ko.observableArray(events || []);
};
window.WeekVm = function(days) {
    var self = this;
    days.unshift(new DayVm(""));
    self.days = days;
};
window.MonthVm = function() {
    var self = this;
    self.weeks = ko.observableArray([
        new WeekVm([
            new DayVm("31"),
            new DayVm("1"),
            new DayVm("2"),
            new DayVm("3"),
            new DayVm("4", [
                    new EventVm("Event 1", "alert"),
                    new EventVm("Event 2", "alert")
                ]),
            new DayVm("5"),
            new DayVm("6")
        ]),
        new WeekVm([
            new DayVm("7"),
            new DayVm("8", [
                    new EventVm("Event 1", "alert"),
                    new EventVm("Event 2", "alert"),
                    new EventVm("Event 3", "alert"),
                    new EventVm("Event 4", "alert")
                ]),
            new DayVm("9"),
            new DayVm("10"),
            new DayVm("11"),
            new DayVm("12"),
            new DayVm("13")
        ])
    ]);
};
 