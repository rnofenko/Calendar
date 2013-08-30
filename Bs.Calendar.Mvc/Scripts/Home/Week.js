function CalendarWeekDay(date) {
    var self = this;

    self.date = date;
    self.blocks = [];

    self.dayTitle = ko.computed(function () {
        if (date == null) return " ";
        return date.format('ddd') + ' ' + date.date() + '/' + date.month();
    }, this);

    self.mouseDown = function (event) {
        if (event.which != 1) return;
        if ($.grep(self.blocks, function(element) { return event.target == element.block[0]; }).length != 0) return;

        var block = new DayTimeBlock($(event.currentTarget), self.blocks);
        self.blocks.push(block);
        block.initialize(event);
    };

    self.initialize = function () {
        if (self.date == null) return;
        $('#week-' + self.date.format('ddd')).mousedown(self.mouseDown);
    };
    
    //Initialize
    self.initialize();
}

function CalendarWeekVm() {
    var self = this;

    self.isChosen = ko.observable(false);
    self.currDate = moment();
    self.days = ko.observableArray();

    self.update = function () {
        var week = self.currDate.clone().startOf('week');
        self.setTitle(week.clone());
        var todayColor = moment().week() == self.currDate.week() ? '#fcf8e3' : 'white';
        $('#week-' + self.currDate.format('ddd')).css('background', todayColor);
        self.days.removeAll();
        
        for (var i = 0; i < 7; i++, week.add('days', 1)) {
            self.days.push(new CalendarWeekDay(week.clone()));
        }
        self.days.unshift(new CalendarWeekDay(null));
        self.days.push(new CalendarWeekDay(null));
    };

    self.initialize = function() {
        $(".week-container-day > div").addClass("week-time-line");
        $(".week-time-panel > div").addClass("week-time-line");
    };

    self.setTitle = function (date) {
        var title = date.format("MMM") + ' ' + date.date() + ' - ';
        var startMonth = date.month();
        var dayEnd = date.add('days', 6).date();
        var monthEnd = startMonth != date.month() ? date.format('MMM') + ' ' : '';
        title += monthEnd + dayEnd + ' ' + date.format('YYYY');
        mediator.trigger("Calendar:setPanelTitle", title);
    };
    
    self.setMode = function (mode) {
        self.isChosen(mode == 'week');
        if (self.isChosen()) self.update();
    };

    self.nextWeek = function() {
        if (!self.isChosen()) return;
        self.currDate.add('week', 1);
        self.update();
    };

    self.prevWeek = function() {
        if (!self.isChosen()) return;
        self.currDate.subtract('week', 1);
        self.update();
    };
    
    //Initialize
    //self.update();
    self.initialize();

    //Setup bindings
    mediator.bind("Calendar:nextButton", self.nextWeek);
    mediator.bind("Calendar:prevButton", self.prevWeek);
    mediator.bind("Calendar:setMode", self.setMode);
}