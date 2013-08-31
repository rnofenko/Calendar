function CalendarDayModeHandler(currDate) {
    var self = this;

    self.date = currDate;
    self.blocks = [];
    self.domElement = {};
    self.blockModel = null;

    self.mouseDown = function(event) {
        if (event.which != 1) return;
        if ($.grep(self.blocks, function (element) { return event.target === element.block[0]; }).length != 0) return;

        self.removeElement(self.blockModel);
        var block = new DayTimeBlock(self.domElement, self.blocks, self);
        self.blocks.push(block);
        block.createBlock(event);
    };

    self.initialize = function() {
        $('.day-container-day').mousedown(self.mouseDown);
        self.domElement = $('.day-container-day');
        self.loadEvents();
    };

    self.uninitialize = function() {
        $('.day-container-day').unbind("mousedown");
        $(".day-time-block.day-time-block").remove();
        $(".day-allday-block").remove();
        $(".calendar-day-allday").height(0);
    };
    
    self.getEvents = function () {
        var events;
        var timeRange = { from: self.date.toJSON(), to: self.date.clone().add('minutes', 1435).toJSON() };

        $.ajax({
            url: "/Home/List",
            async: false,
            type: "GET",
            data: timeRange,
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (data) { events = data; });
        return events;
    };

    self.loadEvents = function () {
        var events = self.getEvents();
        if (events == null) return;

        $.each(events.CalendarEvents, function (key, eventModel) {
            var block = new DayTimeBlock(self.domElement, self.blocks);
            self.blocks.push(block);
            block.addBlock(eventModel);
            block.block.text(block.block.text() + ' | ' + eventModel.Title + " | Text: " + eventModel.Text);
            block.block.dblclick(function (event) {
                window.location.href = window.location.pathname + "Event/Edit/" + eventModel.Id;
            });
        });

        self.showBirthdays(events.BirthdayEvents);
    };
    
    self.onEventCreate = function (blockModel) {
        if (blockModel.parent != self.domElement) return;
        blockModel.block.addClass("block-created");
        self.blockModel = blockModel;

        $("#week-dialog-form .btn").unbind("mouseup");
        $("#week-dialog-form .btn").mouseup(self.dialogClick);
        $("#week-dialog-form").show();
    };
    
    self.dialogClick = function (event) {
        if ($(event.target).text() == "Cancel") {
            self.removeElement(self.blockModel);
            self.blockModel = null;
            $("#week-dialog-form").hide();
            return;
        }
        if (self.blockModel.block[0] != $(".block-created")[0]) return;

        var eventModel = new CalendarEvent();
        eventModel.EventType(1);
        eventModel.Title($(".week-dialog-title-input").val());
        eventModel.Text($(".week-dialog-text").val());
        
        if (eventModel.Title() == "" || eventModel.Text() == "") {
            self.removeElement(self.blockModel);
            self.blockModel = null;
            $("#week-dialog-form").hide();
            return;
        }

        $(".week-dialog-title-input").val("");
        $(".week-dialog-text").val("");

        var start = self.blockModel.timeHandler.getMomentStart(self.blockModel);
        eventModel.DateStart(self.date.clone().hours(start.hours()).minutes(start.minutes()));
        var end = self.blockModel.timeHandler.getMomentEnd(self.blockModel);
        eventModel.DateEnd(self.date.clone().hours(end.hours()).minutes(end.minutes()));

        $.ajax({
            url: "/Event/Create",
            data: JSON.stringify(ko.toJS(eventModel)),
            type: 'POST',
            contentType: 'application/json, charset=utf-8',
            dataType: 'json'
        });

        self.blocks.push(self.blockModel);
        self.blockModel.totalUnbind();
        self.blockModel = null;
        $("#week-dialog-form").hide();
        $(".block-created > div").remove();
        $(".block-created").removeClass("block-created");
        mediator.trigger("CalendarDay:update");
    };
    
    self.removeElement = function (blockModel) {
        $(".block-created").remove();
        if (blockModel == null) return;
        self.blocks.splice(self.blocks.indexOf(blockModel));
    };
    
    //Birthday loader
    self.showBirthdays = function (birthdays) {
        var count = birthdays.length;
        if (count * 26 > $(".calendar-day-allday").height()) $(".calendar-day-allday").height(count * 26);

        $.each(birthdays, function (id, value) {
            var block = $('<div class="day-allday-block"></div>');
            block.css('left', 61);
            block.css('top', id * 26);
            block.append(value.Text);
            block.attr("title", value.Text);
            $(".calendar-day-allday").append(block);
        });
    };
    
    //Initialize
    self.initialize();
}


function CalendarDayVm() {
    var self = this;
    
    self.isChosen = ko.observable(false);
    self.currDate = moment().startOf('day');
    self.dayTitle = ko.observable();
    self.handler = null;

    self.update = function() {
        self.dayTitle(self.currDate.format('dddd') + ' ' + self.currDate.date() + '/' + self.currDate.month());
        self.setTitle();
        var todayColor = moment().startOf('day').isSame(self.currDate) ? "#fcf8e3" : "none";
        $(".day-container-day").css("background", todayColor);
        $(".calendar-week-container")[1].scrollTop = 470;


        if (self.handler != null) self.handler.uninitialize();
        self.handler = new CalendarDayModeHandler(self.currDate);

        $(".calendar-day-allday > div").addClass('calendar-day-allday-cell');
        $(".calendar-day-allday .middle-allday").css('background', todayColor);
    };

    self.setMode = function (mode) {
        self.isChosen(mode == 'day');
        if (self.isChosen()) {
            self.currDate = moment().startOf('day');
            self.update();
        }
    };

    self.nextDay = function () {
        if (!self.isChosen()) return;
        self.currDate.add('day', 1);
        self.update();
    };

    self.prevDay = function () {
        if (!self.isChosen()) return;
        self.currDate.subtract('day', 1);
        self.update();
    };
    
    self.initialize = function () {
        $(".day-container-day > div").addClass("week-time-line");
        $(".week-time-panel > div").addClass("week-time-line");
        $("#week-dialog-form").hide();
    };

    self.setTitle = function (date) {
        var title = self.currDate.format('ddd') + ', ' + self.currDate.format("MMM") + ' ' + self.currDate.date() + ', ' + self.currDate.format("YYYY");
        mediator.trigger("Calendar:setPanelTitle", title);
    };

    //Initialize
    self.initialize();

    //SetUp Bindings
    mediator.bind("CalendarDay:update", self.update);
    mediator.bind("Calendar:nextButton", self.nextDay);
    mediator.bind("Calendar:prevButton", self.prevDay);
    mediator.bind("Calendar:setMode", self.setMode);
}