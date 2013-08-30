function TimeBlock() {
    var self = this;
    
    self.minMinute = 5;
    self.hoursCount = 11;
    self.startHour = 8;
    self.gridStep = 6;
    
    self.isCreated = false;
    self.clickPrevPosition = 0;

    self.block = null;
    self.parent = {};
    self.rightEnd = 0;
    self.leftEnd = 0;

    self.createBlock = function (event, roomEvents) {
        self.parent = $(event.currentTarget);
        if (self.eventOverlap(roomEvents)) return;

        $("#timeblock").remove();
        self.block = $('<div id="timeblock"><div></div><div></div></div>');
        self.block.addClass("roomColor_" + roomEvents.room.Color());
        self.parent.append(self.block);

        self.block.css('left', self.relativeColumn(self.parent, event.pageX));
        self.setRightLeftBounds(roomEvents);
        self.block.mousemove(self.showEdgeCursor);
        self.block.mousedown(self.mouseDown);
        
        $(document).mouseup(self.mouseUp);
        $(document).mousemove(self.blockShiftRight);
    };

    self.eventOverlap = function (roomEvents) {
        var click = self.relativeColumn(self.parent, event.pageX);
        for (var i = 0; i < roomEvents.pixelTimeBegin.length; i++)
            if (click >= roomEvents.pixelTimeBegin[i] && click < roomEvents.pixelTimeEnd[i])
                return true;
        return false;
    };

    self.setRightLeftBounds = function (roomEvents) {
        self.leftEnd = 0;
        for (var i = 0; i < roomEvents.pixelTimeEnd.length; i++) {
            if (roomEvents.pixelTimeEnd[i] <= self.block[0].offsetLeft)
                self.leftEnd = Math.max(self.leftEnd, roomEvents.pixelTimeEnd[i]);
        }
        
        self.rightEnd = self.parent.width();
        for (var j = 0; j < roomEvents.pixelTimeBegin.length; j++) {
            if (roomEvents.pixelTimeBegin[j] > self.block[0].offsetLeft)
                self.rightEnd = Math.min(self.rightEnd, roomEvents.pixelTimeBegin[j]);
        }
    };

    self.inBounds = function(coordinate) {
        return coordinate >= self.leftEnd && coordinate <= self.rightEnd;
    };

    self.blockShiftRight = function (event) {
        var mousePosition = self.relativeColumn(self.parent, event.pageX);
        if (!self.inBounds(mousePosition)) return;

        var blockBegin = self.blockBegin();
        self.block.width(mousePosition - blockBegin);
        self.updateTimePicker();
    };

    self.blockShiftLeft = function(event) {
        var mousePosition = self.relativeColumn(self.parent, event.pageX);
        if (!self.inBounds(mousePosition)) return;

        var shift = self.clickPrevPosition - mousePosition;
        self.clickPrevPosition = mousePosition;
        self.block.css('left', mousePosition);
        self.block.width(self.block.width() + shift);
        self.updateTimePicker();
    };

    self.blockMove = function(event) {
        var mousePosition = self.relativeColumn(self.parent, event.pageX);
        var shift = self.clickPrevPosition - mousePosition;
        self.clickPrevPosition = mousePosition;

        var leftIndent = self.blockBegin() - shift;
        if (!self.inBounds(leftIndent) || (!self.inBounds(leftIndent + self.block.width()))) return;
        self.block.css('left', leftIndent);
        self.updateTimePicker();
    };

    self.mouseUp = function () {
        $(document).unbind("mousemove");
        $(document).unbind("mouseup");
        if (self.block.width() < self.gridStep) {
            self.block.remove();
            mediator.trigger("EventRoomHandler:clearRoom");
        } else
            self.isCreated = true;
    };

    self.mouseDown = function (event) {
        self.clickPrevPosition = self.relativeColumn(self.parent, event.pageX);

        if (self.mouseOnEdge(event) != 0) self.mouseEdgeDown(event);
        else self.mouseNonEdgeDown(event);
    };

    self.mouseNonEdgeDown = function(event) {
        $(document).mousemove(self.blockMove);
        $(document).mouseup(self.mouseUp);
    };

    self.mouseEdgeDown = function (event) {
        if (self.mouseOnEdge(event) == 1)
            $(document).mousemove(self.blockShiftRight);
        else
            $(document).mousemove(self.blockShiftLeft);
        
        $(document).mouseup(self.mouseUp);
    };

    self.showEdgeCursor = function(event) {
        if (self.isCreated == false) return;
        event.target.style.cursor = self.mouseOnEdge(event) != 0 ? 'w-resize': 'default';
    };

    self.mouseOnEdge = function(event) {
        var block = self.block.get(0);
        var left = event.offsetX, right = block.offsetWidth - left;
        var min = Math.min(Math.min(left, right), self.gridStep - 0.1);
        return min == left ? -1 : min == right ? 1 : 0;     //right edge = 1; left edge = -1; none = 0;
    };

    self.updateTimePicker = function () {
        var blockBegin = self.blockBegin();
        mediator.trigger("DateTimeHandler:setFromTime", self.getMomentTime(blockBegin));
        mediator.trigger("DateTimeHandler:setToTime", self.getMomentTime(blockBegin + self.block.width()));
    };

    self.getMomentTime = function(coordinate) {
        var dateTime = moment();
        var totalMinutes = (coordinate / self.gridStep) * self.minMinute;
        var hours = self.startHour + Math.floor(totalMinutes / 60);
        var minutes = totalMinutes % 60;
        return dateTime.hours(hours).minutes(minutes);
    };

    self.blockBegin = function() {
        return self.relativePosition(self.parent, self.block.offset().left);
    };  

    self.relativeColumn = function(parent, coordinateX) {
        return self.getColumn(self.relativePosition(parent, coordinateX));
    };

    self.getColumn = function(x) {
        return x - (x % self.gridStep);
    };
    
    self.relativePosition = function (parent, coordinateX) {
        return coordinateX - parent.offset().left;
    };
    
    self.setInputTime = function (start, end) {
        if (self.block == null || self.block.width() < 6) return;
        var endPixel = Math.min(self.timeToPixel(end), self.rightEnd);
        var beginPixel = Math.max(self.timeToPixel(start), self.leftEnd);
        
        self.block.css('left', beginPixel);
        self.block.width(endPixel - beginPixel);
        self.updateTimePicker();
    };
    
    self.timeToPixel = function (dateTime) {
        var time = moment(dateTime);
        var totalMinutes = (time.hours() - 8) * 60 + time.minutes();
        return Math.floor(totalMinutes / 5) * 6;
    };

    //Setup Bindings
    mediator.bind("RoomTimeSlider:setInputTime", self.setInputTime);
}

function RoomOrderElement(roomEventVm) {
    var self = this;

    self.room = ko.mapping.fromJS(roomEventVm.Room);
    self.events = roomEventVm.Events;
    self.pixelTimeBegin = [];
    self.pixelTimeEnd = [];
    self.divContainer = {};

    self.update = function (divContainer) {
        self.divContainer = divContainer;
        if (self.events == null || self.events == undefined) return;
        $.each(self.events, function (key, value) {
            self.pixelTimeBegin[key] = self.timeToPixel(value.DateStart);
            self.pixelTimeEnd[key] = self.timeToPixel(value.DateEnd);

            self.drawEventBlock(key);
        });
    };

    self.drawEventBlock = function (key) {
        var block = $('<div class="event-timeblock"></div>');
        block.css('left', self.pixelTimeBegin[key]);
        block.addClass("roomColor_" + self.room.Color());
        block.text(self.events[key].Title);
        block.attr("title", block.text());
        block.width(self.pixelTimeEnd[key] - self.pixelTimeBegin[key]);
        self.divContainer.append(block);
    };
    
    self.timeToPixel = function (dateTime) {
        var time = moment(dateTime);
        var totalMinutes = (time.hours() - 8) * 60 + time.minutes();
        return Math.floor(totalMinutes / 5) * 6;
    };
}

function RoomOrderListVm() {
    var self = this;

    self.timeBlock = new TimeBlock();
    self.showRoomOrderList = ko.observable(false);
    self.dateTime = ko.observable(moment());
    self.roomList = ko.observableArray();
    self.chosenRoom = ko.observable();

    self.getRooms = ko.computed(function() {
        $.getJSON("/Event/GetRooms", { dateTime: self.dateTime().toJSON() }, function (rooms) {
            self.roomList.removeAll();
            $.each(rooms, function (key, value) {
                var orderElement = new RoomOrderElement(value);
                self.roomList.push(orderElement);
                orderElement.update($(".room-order-time").last());
            });
            self.initialize();
        });
    });

    self.initialize = function () {
        $(".room-order-time").mousedown(self.mouseDown);
        $(".room-order-time > div").addClass("room-order-separator");
        $(".room-order-name:last").addClass("room-last-name");
    };

    self.mouseDown = function (event) {
        if (event.which != 1) return;       //not left mouse button
        if (self.timeBlock.isCreated && event.target == self.timeBlock.block.get(0)) return;
        $("#help-wrap").remove();

        var roomName = $(event.currentTarget).closest("div").prev().text();
        var roomListElement = $.grep(self.roomList(), function(element) { return element.room.Name() == roomName; })[0];
        self.chosenRoom(roomListElement.room);
        self.timeBlock.createBlock(event, roomListElement);
    };

    self.onRommChange = ko.computed(function() {
        if (self.chosenRoom() != undefined) mediator.trigger("EventRoomHandler:setRoom", self.chosenRoom());
    });

    self.hideRoomOrderList = function () { self.showRoomOrderList(false); };
    
    self.invertRoomOrderListView = function () { self.showRoomOrderList(!self.showRoomOrderList()); };
    
    self.updateDateTime = function (dateTime) { self.dateTime(dateTime.clone()); };
    

    //Setup bindings
    mediator.bind("RoomOrderListVm:hideRoomOrderList", self.hideRoomOrderList);
    mediator.bind("RoomOrderListVm:invertRoomOrderListView", self.invertRoomOrderListView);
    mediator.bind("EventDateTime:dateUpdate", self.updateDateTime);
}