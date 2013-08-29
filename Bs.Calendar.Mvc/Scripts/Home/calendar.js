function CalendarVm() {
    var self = this;

    self.title = ko.observable();

    self.nextButton = function() {
        mediator.trigger("Calendar:nextButton");
    };

    self.prevButton = function() {
        mediator.trigger("Calendar:prevButton");
    };

    self.showWeek = function () { mediator.trigger("Calendar:setMode", "week"); };
    self.showMonth = function () { mediator.trigger("Calendar:setMode", "month"); };
    self.showDay = function () { mediator.trigger("Calendar:setMode", "day"); };

    self.setTitle = function(title) {
        self.title(title);
    };
    
    //Setup bindings
    mediator.bind("Calendar:setPanelTitle", self.setTitle);
}