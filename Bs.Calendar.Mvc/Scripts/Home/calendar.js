
window.DayVm = function (day_title) {
    var self = this;
    self.day_title = ko.observable(day_title);
};
window.WeekVm = function(days) {
    var self = this;
    self.days = ko.observableArray(days);
};
window.MonthVm = function() {
    var self = this;
    self.weeks = ko.observableArray([
        new WeekVm([
            new DayVm("31"),
            new DayVm("1"),
            new DayVm("2"),
            new DayVm("3"),
            new DayVm("4"),
            new DayVm("5"),
            new DayVm("6")
        ]),
        new WeekVm([
            new DayVm("7"),
            new DayVm("8"),
            new DayVm("9"),
            new DayVm("10"),
            new DayVm("11"),
            new DayVm("12"),
            new DayVm("13")
        ])
    ]);
};
 