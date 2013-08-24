function DateTimeHandler(eventModel) {
    var self = this;

    self.isAllDay = eventModel.IsAllDay;

    var formatSettings = { date: "YYYY-MM-DD", time: "hh:mm a" };
    var timeRangeSettings = {
        minTime: moment().setTime(moment("8:00 am", formatSettings.time)),
        maxTime: moment().setTime(moment("6:00 pm", formatSettings.time)),
        step: 60
    };

    var dateTimeControl = {
        fromTime: $("#fromTime"),
        date: $("#date"),
    }; //Setup time range control html elements

    $(dateTimeControl.fromTime).timepicker({
        timeFormat: "g:i a",
        step: timeRangeSettings.step,
        minTime: timeRangeSettings.minTime.format(formatSettings.time),
        maxTime: timeRangeSettings.maxTime.format(formatSettings.time)
    });

    var dateDefaults = { initialValue: timeRangeSettings.minTime.clone()};
    self.fromDateTime = ko.observable(dateDefaults.initialValue);

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

            if (newDate < timeRangeSettings.minTime.clone().startOf('day')) {
                $(event.target).val(self.dateInput.value());
            }
            else {
                self.fromDateTime(newDate.clone().setTime(self.fromDateTime()));
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

            console.log(timeRangeSettings.minTime);
            timeRangeSettings.minTime.clone().setDate(fromTime);
            console.log(timeRangeSettings.minTime);

            if (!moment.isMoment(fromTime) ||
                !fromTime.isValid() ||
                fromTime < timeRangeSettings.minTime.clone().setDate(fromTime) ||
                fromTime > timeRangeSettings.maxTime.clone().setDate(fromTime) ||
                event.type === "timeFormatError") {
                self.fromDateTime(currentFromTime);
                return;
            }

            self.fromDateTime(currentFromTime.setTime(fromTime));
        }
    };
}