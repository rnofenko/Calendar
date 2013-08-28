function TimeBlock() {
    var self = this;
    
    self.minMinute = 5;
    self.hoursCount = 11;
    self.startHour = 8;
    self.gridStep = 6;
    
    self.isCreated = false;
    self.clickPrevPosition = 0;

    self.block = {};
    self.parent = {};
    self.rightEnd = 0;
    self.leftEnd = 0;

    self.createBlock = function (event) {
        $("#timeblock").remove();
        self.block = $('<div id="timeblock"><div></div><div></div></div>');
        self.parent = $(event.currentTarget);
        self.parent.append(self.block);

        self.block.css('left', self.relativeColumn(self.parent, event.pageX));
        self.setRightLeftEnds();
        self.block.mousemove(self.showEdgeCursor);
        self.block.mousedown(self.mouseDown);
        
        $(document).mouseup(self.mouseUp);
        $(document).mousemove(self.blockShiftRight);
    };

    self.setRightLeftEnds = function() {
        self.leftEnd = 0;
        self.rightEnd = self.parent.width();
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
        if (self.block.width() < self.gridStep) self.block.remove();
        else self.isCreated = true;
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
}

function RoomOrderElementVm() {
    var self = this;

    var model = ko.observable();
    

}

function RoomOrderListVm() {
    var self = this;

    self.timeBlock = new TimeBlock();
    self.showRoomOrderList = ko.observable(false);
    self.dateTime = ko.observable();
    self.roomList = ko.observableArray();

    $(".room-order-time").mousedown(function (event) {
        if (event.which != 1) return;       //not left mouse button
        if (self.timeBlock.isCreated && event.target == self.timeBlock.block.get(0)) return;
        self.timeBlock.createBlock(event);
    });
    
    $(".room-order-time > div").addClass("room-order-separator");
    $(".room-order-name:last").addClass("room-last-name");

    self.hideRoomOrderList = function () { self.showRoomOrderList(false); };
    self.invertRoomOrderListView = function () { self.showRoomOrderList(!self.showRoomOrderList()); };

    self.getRooms = ko.computed(function() {

    });

    self.updateDateTime = function (dateTime) { self.dateTime(dateTime); };
    

    //Setup bindings
    mediator.bind("RoomOrderListVm:hideRoomOrderList", self.hideRoomOrderList);
    mediator.bind("RoomOrderListVm:invertRoomOrderListView", self.invertRoomOrderListView);
}