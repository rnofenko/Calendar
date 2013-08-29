function CalendarWeekVm() {
    var self = this;

    self.isChosen = ko.observable(false);
    
    self.setMode = function (mode) {
        self.isChosen(mode == 'week');
    };

    self.nextWeek = function() {
        if (!self.isChosen()) return;
    };

    self.prevWeek = function() {
        if (!self.isChosen()) return;
    };

    //Setup bindings
    mediator.bind("Calendar:nextButton", self.nextWeek);
    mediator.bind("Calendar:prevButton", self.prevWeek);
    mediator.bind("Calendar:setMode", self.setMode);
}