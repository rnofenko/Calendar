function DayTimeBlock(parent, blocks) {
    var self = this;

    self.parent = parent;
    self.gridStep = 30;
    self.block = null;
    self.allBlocks = blocks;

    self.clickPrevPosition = 0;
    self.topBound = 0;
    self.bottomBound = 0;

    self.createBlock = function(event) {
        self.block = $('<div class="day-time-block"><div></div><div></div></div>');
        self.block.css('top', self.relativeRow(event.pageY));
        self.block.mousemove(self.showEdgeCursor);
        self.block.mousedown(self.mouseDown);

        self.parent.append(self.block);
        self.setTopBottomBounds();
        self.parent.mouseup(self.mouseUp);
        self.parent.mousemove(self.mouseEdgeDown);
    };

    self.addBlock = function(eventModel) {

    };

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
    };

    self.mouseEdgeDown = function(event) {
        var mousePosition = self.relativeRow(event.pageY);
        var blockBegin = self.blockBegin();
        if (!self.isInBounds(mousePosition)) return;

        if (blockBegin >= mousePosition) return;
        self.setBlockHeight(mousePosition - blockBegin);
    };

    self.mouseEdgeUp = function(event) {
        var mousePosition = self.relativeRow(event.pageY);
        if (!self.isInBounds(mousePosition)) return;

        var shift = self.clickPrevPosition - mousePosition;
        if ((self.block.height() + shift) < (self.gridStep - 2)) return;
        
        self.clickPrevPosition = mousePosition;
        self.block.css('top', mousePosition);
        self.block.height(self.block.height() + shift);
    };

    self.blockMove = function(event) {
        var mousePosition = self.relativeRow(event.pageY);
        var shift = self.clickPrevPosition - mousePosition;
        self.clickPrevPosition = mousePosition;

        var leftIndent = self.blockBegin() - shift;
        if (!self.isInBounds(leftIndent) || (!self.isInBounds(leftIndent + self.block.height()))) return;
        self.block.css('top', leftIndent);
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
    
    self.pixelToTime = function (coordinate) {
        var hours = Math.floor(coordinate / self.gridStep * 2);
        var minutes = coordinate % (self.gridStep * 2) == 0 ? "00" : "30";
        return hours + ":" + minutes;
    };
    
    //Setup Bindings
    mediator.bind("WeekTimeBlock:setBounds", self.setTopBottomBounds);
}