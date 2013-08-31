function CalendarRoomDay(date) {
    var self = this;

    self.date = ko.observable(date);
    self.blocks = [];
    self.domElement = {};
    self.blockModel = null;
    self.roomList = ko.observableArray();

    self.dayTitle = ko.computed(function() {
        if (date == null) return " ";
        return date.format('ddd') + ' ' + date.date() + '/' + date.month();
    }, this);
    
    self.getRooms = ko.computed(function () {
        $.getJSON("/Event/GetRooms", { dateTime: self.date().toJSON() }, function (rooms) {
            self.roomList.removeAll();
            $.each(rooms, function (key, value) {
                var orderElement = new RoomOrderElement(value);
                self.roomList.push(orderElement);
                orderElement.update($(".room-order-time").last());
            });
            self.initialize();
            if (self.date().day() == 6)
                mediator.trigger("CalendarRoom:update");
        });
    });

    self.initialize = function() {
        $(".room-order-time > div").addClass("room-order-separator");
    };
}

function CalendarRoomVm() {
    var self = this;
    self.isChosen = ko.observable(false);
    
    self.currDate = moment();
    self.days = ko.observableArray();

    self.update = function () {
        self.isChosen(false);
        var week = self.currDate.clone().startOf('week');
        self.setTitle(week.clone());
        var todayColor = moment().week() == self.currDate.week() ? '#fcf8e3' : 'white';
        $('#week-' + self.currDate.format('ddd')).css('background', todayColor);
        
        //$(".calendar-week-container")[0].scrollTop = 470;

        //$.each(self.days(), function (id, value) { value.uninitialize(); });
        self.days.removeAll();
        //$(".week-allday-block").remove();
        //$(".day-time-block").remove();
        //$(".week-allday-block").remove();
        //$(".calendar-week-allday").height(0);

        for (var i = 0; i < 7; i++, week.add('days', 1)) {
            self.days.push(new CalendarRoomDay(week.clone()));
        }
        //self.days.unshift(new CalendarWeekDay(null));
        //self.days.push(new CalendarWeekDay(null));

        //$.each($(".calendar-week-allday > div"), function (id, value) {
        //    if (id == 0) $(value).text("All Day");
        //    if (id == (self.currDate.day() + 1)) $(value).css('background', todayColor);
        //});
        //$(".calendar-week-allday > div").addClass("calendar-week-allday-cell");
        //$($(".calendar-week-allday > div")[8]).css("width", "17px");
    };

    self.initialize = function () {
        //$(".calendar-room-time-container .room-order-name:last").addClass("room-last-name");
        //$(".week-container-day > div").addClass("week-time-line");
        //$(".week-time-panel > div").addClass("week-time-line");

        //$("#week-dialog-form").hide();
    };

    self.setTitle = function (date) {
        var title = date.format("MMM") + ' ' + date.date() + ' \u2014 ';
        var startMonth = date.month();
        var dayEnd = date.add('days', 6).date();
        var monthEnd = startMonth != date.month() ? date.format('MMM') + ' ' : '';
        title += monthEnd + dayEnd + ' ' + date.format('YYYY');
        mediator.trigger("Calendar:setPanelTitle", title);
    };

    self.setMode = function (mode) {
        self.isChosen(mode == 'room');
        if (self.isChosen()) {
            self.currDate = moment();
            self.update();
        }
    };

    self.nextWeek = function () {
        if (!self.isChosen()) return;
        self.currDate.add('week', 1);
        self.update();
    };

    self.prevWeek = function () {
        if (!self.isChosen()) return;
        self.currDate.subtract('week', 1);
        self.update();
    };

    self.testMy = function() {
        self.isChosen(true);
    };

    //Initialize
    self.initialize();

    //Setup bindings
    mediator.bind("CalendarRoom:update", self.testMy);
    mediator.bind("Calendar:nextButton", self.nextWeek);
    mediator.bind("Calendar:prevButton", self.prevWeek);
    mediator.bind("Calendar:setMode", self.setMode);
};