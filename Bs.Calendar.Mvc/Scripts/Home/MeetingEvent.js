function DateTimeHandler() {
    var self = this;

    var formatSettings = { date: "YYYY-MM-DD", time: "hh:mm a" };
    var timeRangeSettings = {
        minTime: moment().setTime(moment("8:00 am", formatSettings.time)),
        maxTime: moment().setTime(moment("6:00 pm", formatSettings.time)),
        step: 60
    };

    var dateTimeControl = {
        fromTime: $("#fromTime"),
        toTime: $("#toTime"),
        date: $("#date"),
    }; //Setup time range control html elements

    $(dateTimeControl.fromTime).add(dateTimeControl.toTime).timepicker({
        timeFormat: "g:i a",
        step: timeRangeSettings.step,
        minTime: timeRangeSettings.minTime.format(formatSettings.time),
        maxTime: timeRangeSettings.maxTime.format(formatSettings.time)
    });

    var dateDefaults = { initialValue: timeRangeSettings.minTime.clone(), initialDifference: { minutes: timeRangeSettings.step } };

    self.fromDateTime = ko.observable(dateDefaults.initialValue);
    self.toDateTime = ko.observable(dateDefaults.initialValue.clone().add(dateDefaults.initialDifference));

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
}