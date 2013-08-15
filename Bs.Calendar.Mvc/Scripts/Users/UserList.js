function PagingVm() {
    var self = this;
    self.TotalPages = ko.observable();
    self.Page = ko.observable();
    self.SortByStr = ko.observable();
    self.SearchStr = ko.observable();

    self.ShowAdmins = ko.observable();
    self.ShowNotApproved = ko.observable();
    self.ShowDeleted = ko.observable();
}

function ViewModel() {
    var self = this;

    //Variables
    self.model = {
        SearchStr: ko.observable(""),
        SortByStr: ko.observable(""),
        Page: ko.observable(),
        TotalPages: ko.observable(),
        
        ShowAdmins: ko.observable(false),
        ShowNotApproved: ko.observable(false),
        ShowDeleted: ko.observable(false)
    };

    self.arrowUp = false;
    self.currColumnId = "";
    self.listBodyId = "#list-body";

    //Methods
    self.headerClick = function(data, event) {
        self.showArrow('#' + event.currentTarget.id);

        self.model.SortByStr(event.currentTarget.innerText.concat(self.arrowUp ? "" : "Desc"));
        self.updateList(self.model);
    };

    self.previousPage = function() {
        var page = Number(self.model.Page()) - 1;
        if (page >= 1) {
            self.model.Page(page);
            self.updateList(self.model);
        }
    };

    self.nextPage = function () {
        var page = Number(self.model.Page()) + 1;
        if (page <= self.model.TotalPages()) {
            self.model.Page(page);
            self.updateList(self.model);
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

    self.updateList = function (model) {

        $.get("Users/List", model, function(htmlData) {
            $(self.listBodyId).html(htmlData);
            self.model.Page($(htmlData).filter("#listPage").val());
            self.model.TotalPages($(htmlData).filter("#listTotalPages").val());
            self.model.SearchStr($(htmlData).filter("#listSearchStr").val());
            self.model.SortByStr($(htmlData).filter("#listSortByStr").val());
        }, "html");
    };

    self.changeHandler = function () {
        self.updateList(self.model);
    };
    
    self.exitAccept = function () {

        /* Save new settings and update list of users */

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

        /* Revert checkboxes to the last state */

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
    };
}
