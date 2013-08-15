<<<<<<< Updated upstream
﻿function UserFrame() {
=======
﻿function PagingVm() {
    var self = this;
    self.TotalPages = ko.observable();
    self.Page = ko.observable();
    self.SortByStr = ko.observable();
    self.SearchStr = ko.observable();

    self.ShowAdmins = ko.observable();
    self.ShowNotApproved = ko.observable();
    self.ShowDeleted = ko.observable();
}

function UserFrame() {
>>>>>>> Stashed changes
    var self = this;

    self.TotalPages = ko.observable();
    self.filter = new UserFrameFilter(self);

    self.arrowUp = false;
    self.currColumnId = "";

    //Methods
    self.headerClick = function(data, event) {
        self.showArrow('#' + event.currentTarget.id);
        self.filter.SortByField($(event.currentTarget).data("field").concat(self.arrowUp ? "" : " Desc"));
        self.updateList();
    };

    self.previousPage = function() {
        var page = Number(self.filter.Page()) - 1;
        if (page >= 1) {
            self.filter.Page(page);
            self.updateList();
        }
    };

    self.nextPage = function () {
        var page = Number(self.filter.Page()) + 1;
        if (page <= self.TotalPages()) {
            self.model.Page(page);
            self.updateList();
        }
    };

    self.showArrow = function(columnId) {
        $(self.currColumnId + " > i").remove();

        if (self.currColumnId != columnId) {
            self.arrowUp = false;
            self.currColumnId = columnId;
        }

        self.arrowUp = !self.arrowUp;
        $(columnId).append($('<i/>', { "class": self.arrowUp ? "icon-up" : "icon-down" }));
    };

    self.updateList = function () {
        $.get("Users/List", self.filter.getCurrentValues(), function (html) {
            $("#list-body").html(html);
            self.filter.Page($(html).filter("#listPage").val());
            self.TotalPages($(html).filter("#listTotalPages").val());
        }, "html");
    };

    self.changeHandler = function () {
<<<<<<< Updated upstream
        self.updateList();
=======
        self.updateList(self.model);
    };
    
    self.exitAccept = function () {
        var showAdmins = $("#adminsPrompt").hasClass("checked");
        var showNotApproved = $("#notApprovedPrompt").hasClass("checked");
        var showDeleted = $("#deletedPrompt").hasClass("checked");
        
        if (showAdmins != self.model.ShowAdmins() ||
            showNotApproved != self.model.ShowNotApproved() ||
            showDeleted != self.model.ShowDeleted()) {
            
            self.model.ShowAdmins(showAdmins);
            self.model.ShowNotApproved(showNotApproved);
            self.model.ShowDeleted(showDeleted);
            console.log(self.model);
            self.updateList(self.model);
        }

        self.resetFlags();
    };

    self.resetFlags = function () {
        if ($("#adminsPrompt").hasClass("checked") != self.model.ShowAdmins())
            $("#adminsPrompt").click();

        if ($("#notApprovedPrompt").hasClass("checked") != self.model.ShowNotApproved())
            $("#notApprovedPrompt").click();
        
        if ($("#deletedPrompt").hasClass("checked") != self.model.ShowDeleted())
            $("#deletedPrompt").click();

        self.toggleModal();
    };

    self.toggleModal = function() {
        $("#filter_settings").toggleClass("active");
    };
    
    self.chekedFlagsToString = ko.computed(function () {
        
        var noteMessage = [];

        if (self.model.ShowAdmins())
            noteMessage.push("Only Admins");
        if (self.model.ShowNotApproved())
            noteMessage.push("Not approved");
        if (self.model.ShowDeleted())
            noteMessage.push("Deleted");
        
        if (noteMessage.length == 0)
            noteMessage.push("Default");

        return noteMessage.join(", ");
        
    }, self);

    self.updateModal = function (model) {

        console.log(self.model.ShowAdmins(), self.model.ShowNotApproved(), self.model.ShowDeleted());

        self.model.ShowAdmins(model.ShowAdmins);
        self.model.ShowNotApproved(model.ShowNotApproved);
        self.model.ShowDeleted(model.ShowDeleted);

        console.log(self.model.ShowAdmins(), self.model.ShowNotApproved(), self.model.ShowDeleted());
>>>>>>> Stashed changes
    };
}
