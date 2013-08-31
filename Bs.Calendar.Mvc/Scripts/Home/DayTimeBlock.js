function BlockTimeHandler() {
    var self = this;
    self.gridStep = 30;
    
    self.timeFromDate = function(dateFrom, dateTo) {
        var from = moment(dateFrom).format("LT");
        var to = moment(dateTo).format("LT");
        return from + ' - ' + to;
    };
        
    self.pixelToTime = function (block) {
        var startTime = self.getMomentStart(block);
        var endTime = self.getMomentEnd(block);

        return startTime.format("LT") + ' - ' + endTime.format("LT");
    };

    self.timeToPixel = function (dateTime) {
        var time = moment(dateTime);
        var totalMinutes = time.hours() * 60 + time.minutes();
        return Math.floor(totalMinutes / 30) * self.gridStep;
    };

    self.getMomentStart = function(block) {
        var hoursStart = Math.floor(block.blockBegin() / (self.gridStep * 2));
        var minutesStart = block.blockBegin() % (self.gridStep * 2) == 0 ? "00" : "30";
        return moment().hours(hoursStart).minutes(minutesStart);
    };
    
    self.getMomentEnd = function (block) {
        var end = block.blockEnd() + 2;
        var hoursStart = Math.floor(end / (self.gridStep * 2));
        var minutesStart = end % (self.gridStep * 2) == 0 ? "00" : "30";
        return moment().hours(hoursStart).minutes(minutesStart);
    };
}

function DayTimeBlock(parent, blocks, parentModel) {
    var self = this;

    self.parent = parent;
    self.gridStep = 30;
    self.block = null;
    self.allBlocks = blocks;
    self.timeHandler = new BlockTimeHandler();
    self.clickPrevPosition = 0;
    self.topBound = 0;
    self.bottomBound = 0;
    self.title = "";
    self.isCreated = false;
    self.parentModel = parentModel;

    //Block Creating
    self.createBlock = function(event) {
        self.block = $('<div class="day-time-block"><div></div><div></div></div>');
        self.block.css('top', self.relativeRow(event.pageY));
        self.block.css('background', '#4197c2');
        self.block.css('border', '1px solid #2f6f8e');
        self.block.mousemove(self.showEdgeCursor);
        self.block.mousedown(self.mouseDown);

        self.parent.append(self.block);
        self.setTopBottomBounds();
        self.parent.mouseup(self.mouseUp);
        self.parent.mousemove(self.mouseEdgeDown);
        self.updateTime();
    };

    self.addBlock = function(eventModel) {
        self.block = $('<div class="day-time-block"></div>');
        self.block.css('top', self.timeHandler.timeToPixel(eventModel.DateStart));
        self.block.height(self.timeHandler.timeToPixel(eventModel.DateEnd) - self.timeHandler.timeToPixel(eventModel.DateStart));
        
        if (eventModel.EventType == 1 || (eventModel.EventType != 1 && eventModel.Room == null)) {
            self.block.css('background', '#4197c2');
            self.block.css('border', '1px solid #2f6f8e');
        }
        if (eventModel.EventType != 1 && eventModel.Room != null) {
            self.block.addClass("roomColor_" + eventModel.Room.Color);
            self.block.addClass("roomColorBorder_" + eventModel.Room.Color);
        }
        
        self.block.append(self.timeHandler.timeFromDate(eventModel.DateStart, eventModel.DateEnd));
        //self.block.append(' ' + eventModel.Title);
        self.title = eventModel.Title;
        self.block.attr('title', eventModel.Title + " : " + eventModel.Text);

        self.parent.append(self.block);
        self.setTopBottomBounds();
    };

    self.addBirthdayBlock = function(eventModel) {

    };

    //Block moving
    self.setTopBottomBounds = function () {
        self.topBound = 0;
        for (var i = 0; i < self.allBlocks.length; i++) {
            if (self.allBlocks[i].block == self.block) continue;   
            if (self.allBlocks[i].blockEnd() <= self.blockBegin())
                self.topBound = Math.max(self.topBound, self.allBlocks[i].blockEnd());
        }

        self.bottomBound = self.parent.height();
        for (var j = 0; j < self.allBlocks.length; j++) {
            if (self.allBlocks[j].block == self.block) continue;
            if (self.allBlocks[j].blockBegin() > self.blockEnd())
                self.bottomBound = Math.min(self.bottomBound, self.allBlocks[j].blockBegin());
        }
    };
    
    self.isInBounds = function (coordinate) {
        return coordinate >= self.topBound && coordinate <= self.bottomBound;
    };
    
    self.mouseUp = function(event) {
        self.parent.unbind("mousemove");
        self.parent.unbind("mouseup");
        mediator.trigger("WeekTimeBlock:setBounds");
        if (self.isCreated == false) {
            self.isCreated = true;
            self.parentModel.onEventCreate(self);
        }
    };

    self.totalUnbind = function() {
        self.block.unbind("mousemove");
        self.block.unbind("mousedown");
    };

    self.mouseEdgeDown = function(event) {
        var mousePosition = self.relativeRow(event.pageY);
        var blockBegin = self.blockBegin();
        if (!self.isInBounds(mousePosition)) return;

        if (blockBegin >= mousePosition) return;
        self.setBlockHeight(mousePosition - blockBegin);
        self.updateTime();
    };

    self.mouseEdgeUp = function(event) {
        var mousePosition = self.relativeRow(event.pageY);
        if (!self.isInBounds(mousePosition)) return;

        var shift = self.clickPrevPosition - mousePosition;
        if ((self.block.height() + shift) < (self.gridStep - 2)) return;
        
        self.clickPrevPosition = mousePosition;
        self.block.css('top', mousePosition);
        self.block.height(self.block.height() + shift);
        self.updateTime();
    };

    self.blockMove = function(event) {
        var mousePosition = self.relativeRow(event.pageY);
        var shift = self.clickPrevPosition - mousePosition;
        self.clickPrevPosition = mousePosition;

        var leftIndent = self.blockBegin() - shift;
        if (!self.isInBounds(leftIndent) || (!self.isInBounds(leftIndent + self.block.height()))) return;
        self.block.css('top', leftIndent);
        self.updateTime();
    };
    
    self.mouseDown = function (event) {
        self.clickPrevPosition = self.relativeRow(event.pageY);
        $(parent).mouseup(self.mouseUp);

        if (self.mouseOnEdge(event) != 0) self.mouseEdgeClick(event);
        else self.mouseNonEdgeClick();
    };

    self.mouseNonEdgeClick = function () { $(parent).mousemove(self.blockMove); };

    self.mouseEdgeClick = function (event) {
        if (self.mouseOnEdge(event) == 1)
            $(parent).mousemove(self.mouseEdgeDown);
        else
            $(parent).mousemove(self.mouseEdgeUp);
    };
    
    //Other methods
    self.showEdgeCursor = function (event) {
        event.target.style.cursor = self.mouseOnEdge(event) != 0 ? 's-resize' : 'default';
    };

    self.mouseOnEdge = function (event) {
        var block = self.block.get(0);
        var top = event.offsetY, bottom = block.offsetHeight - top;
        var min = Math.min(Math.min(top, bottom), 10);
        return min == top ? -1 : min == bottom ? 1 : 0;     //bottom edge = 1; top edge = -1; none = 0;
    };

    self.setBlockHeight = function(height) {
        self.block.height(height - 2);
    };
    
    self.blockBegin = function () {
        return self.relativePosition(self.block.offset().top);
    };

    self.blockEnd = function() {
        return self.blockBegin() + self.block.height();
    };
    
    self.relativeRow = function (coordinateY) {
        return self.getRow(self.relativePosition(coordinateY));
    };

    self.getRow = function (x) {
        return x - (x % self.gridStep);
    };

    self.relativePosition = function (coordinateY) {
        return coordinateY - self.parent.offset().top;
    };

    self.updateTime = function () {
        self.block.contents().filter(function() { return this.nodeType === 3; }).remove();
        self.block.append(self.timeHandler.pixelToTime(self) + ' ' + self.title);
    };
    
    //Setup Bindings
    mediator.bind("WeekTimeBlock:setBounds", self.setTopBottomBounds);
}