function PersonalEventVm(jsonCalendarEventVm, formId) {
    var self = this;

    var dateTimeControl = {
        fromTime: $("#" + formId + " #fromTime"),
        toTime: $("#" + formId + " #toTime"),
        date: $("#" + formId + " #date")
    }; //Setup time range control html elements

    var formatSettings = { date: "YYYY-MM-DD", time: "hh:mm a" };
    var defaultTimeDifference = { minutes: dateTimeControl.fromTime.timepicker('option', 'step') };

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

    self.fromDateTime = ko.observable(moment().startOf('day'));
    self.toDateTime = ko.observable(moment().startOf('day').add(defaultTimeDifference));

    self.isAllDay = ko.observable(false);//jsonCalendarEventVm.IsAllDay);

    self.dateInput = {
        value: ko.computed(function() {
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

            if (newDate > minDate) {
                setDate(currentDate, newDate);

                self.fromDateTime(currentDate);
                self.toDateTime(currentDate);
            }
            else {
                $(event.target).val(self.dateInput.value());
            }
        }
    };

    self.fromInput = {
        value: ko.computed(function () {
                return self.fromDateTime().format(formatSettings.time);
        }, self),
        timeChanged: function (context, event) {
            var fromTime = moment($(event.target).val(), formatSettings.time),
                toTime = moment(self.toDateTime().format(formatSettings.time), formatSettings.time),
                currentFromTime = self.fromDateTime();

            setTime(currentFromTime, fromTime);
            self.fromDateTime(currentFromTime);
            
            if (fromTime > toTime) {
                self.toDateTime(currentFromTime);
            }
        }
    };    

    self.toInput = {
        value: ko.computed(function () {
            return self.toDateTime().format(formatSettings.time);
        }, self),
        timeChanged: function (context, event) {
            //Todo: This bidlocodishe is because of time formats difference in case of using timepicker and moment.js
            var toTime = moment($(event.target).val(), formatSettings.time), //moment($(event.target).timepicker("getTime")).year(0).month(0).date(1),
                fromTime = moment(self.fromDateTime().format(formatSettings.time), formatSettings.time), //self.fromDateTime();
                currentToTime = self.toDateTime();

                setTime(currentToTime, toTime);
                self.toDateTime(currentToTime);
            
                if (toTime < fromTime) {
                    self.fromDateTime(currentToTime);
                }
        }
    };
};