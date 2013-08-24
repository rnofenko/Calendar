moment.fn.setTime = function (fromMoment) {
    if (moment.isMoment(fromMoment)) {
        return this
            .hour(fromMoment.hour())
            .minute(fromMoment.minute())
            .second(fromMoment.second())
            .millisecond(fromMoment.millisecond());
    }

    //TODO: extend with additional checks for setting by string format
    return this;
};

moment.fn.setDate = function (fromMoment) {
    if (moment.isMoment(fromMoment)) {
        return this
                .year(fromMoment.year())
                .month(fromMoment.month())
                .date(fromMoment.date());
    }

    //TODO: extend with additional checks if needed
    return this;
};