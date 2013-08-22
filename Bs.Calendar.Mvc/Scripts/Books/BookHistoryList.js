function BookHistoryItem(source)
{
    var _self = this;
    _self.id = source.Id;
    _self.userId = source.UserId;
    _self.actionId = source.Action;
    _self.bookId = window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length);

    _self.userFullName = ko.observable(source.FullName);
    _self.orderDate = new Date(moment(source.OrderDate).format('L'));
    _self.action = source.Action == 1 ? "Take" : "Return";

    _self.deleted = source.Deleted;
}

function User(id, name)
{
    var self = this;

    self.peopleId = id;
    self.peopleName = name;
}

function BookItem()              
{
    var self = this;

    self.BookCode = ko.observable();
    self.bookTitle = ko.observable();
    self.bookAuthor = ko.observable();
    self.bookDescription = ko.observable();    
}

function BookHistoryList(param)
{
    var self = this;
    
    self.bookItem = {
        BookCode: "",
        BookTitle: "",
        BookAuthor: "",
        BookDescription: ""
    };

    self.oldBookHistory = ko.observableArray();
    self.newBookHistory = ko.observableArray();
    self.historyToDelete = Array();

    self.peoples = ko.observableArray();

    ko.computed(function ()
    {
        $.getJSON("/Users/GetAllUsers", {},
            function (data)
            {
                $.each(data, function (index, item)
                {
                    self.peoples.push(new User(index, item));
                });
            });
    }, this);

    self.showAdd = ko.observable(false);

    self.show = function ()
    {
        self.addNewRecord();
        self.showAdd(true);
    };

    self.saveRecords = function ()
    {
        $.ajax("/Book/Save",
        {
            data: ko.toJSON(
            {
                BookHistoryList: $.merge(self.historyToDelete, self.newBookHistory()),
                BookId: window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length),                
                BookCode: self.bookItem.BookCode,
                BookTitle: self.bookItem.BookTitle,
                BookAuthor: self.bookItem.BookAuthor,
                BookDescription: self.bookItem.BookDescription,
            }),
            type: "post",
            contentType: "application/json",
            success: function (data)
            {
                window.location.href = data.redirectToUrl + "/" + window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length);
            }
        });
    };

    self.removeOldRecord = function (history)
    {
        self.oldBookHistory.remove(history);        
        history.deleted = true;
        self.historyToDelete.push(history);
    };

    self.removeNewRecord = function (history)
    {
        self.newBookHistory.remove(history);
    };

    self.addOldRecord = function (data)
    {
        self.oldBookHistory.push(new BookHistoryItem(data));
    };

    self.addNewRecord = function (data)
    {
        if (typeof data === "undefined")
        {
            data =
            {
                UserId: 0,
                FullName: "",
                OrderDate: new Date(),
                Action: self.newBookHistory().length == 0 ?
                        self.oldBookHistory().length == 0 ? 1 :
                        (self.oldBookHistory()[0].action == "Take" ? 2 : 1) :
                        (self.newBookHistory()[0].action == "Take" ? 2 : 1),
                Deleted: false
        };
        }
        self.newBookHistory.unshift(new BookHistoryItem(data));
    };

    $.each(param, function (key, value)
    {

        self.bookItem = {
            BookCode: param.BookCode,
            BookTitle: param.BookTitle,
            BookAuthor: param.BookAuthor,
            BookDescription: param.BookDescription
        };

        if (key == "BookHistoryList")
        {
            $.each(value, function (k, v)
            {
                self.addOldRecord(v);
            });
        }
    });
}