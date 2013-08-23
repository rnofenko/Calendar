function BookHistoryItem(source)
{
    var self_ = this;
    self_.id = source.Id;
    self_.userId = source.UserId;
    self_.actionId = source.Action;
    self_.bookId = window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length);

    self_.userFullName = ko.observable(source.FullName);
    self_.orderDate = moment(source.OrderDate).format("YYYY-MM-DD");
    self_.action = source.Action == 1 ? "Take" : "Return";

    self_.deleted = source.Deleted;
}

function User(id, name)
{
    var self_ = this;

    self_.peopleId = id;
    self_.peopleName = name;
}

function BookItem()
{
    var self_ = this;

    self_.BookCode = ko.observable();
    self_.BookTitle = ko.observable();
    self_.BookAuthor = ko.observable();
    self_.BookDescription = ko.observable();
}

function BookHistoryList(param)
{
    var self_ = this;
    
    self_.bookItem = new BookItem();
    self_.oldBookHistory = ko.observableArray();
    self_.newBookHistory = ko.observableArray();
    self_.historyToDelete = Array();
    self_.reader = self_.oldBookHistory()[self_.oldBookHistory().length - 1].action == "Take" ? self_.oldBookHistory()[self_.oldBookHistory().length - 1].userFullName : "";

    self_.peoples = ko.observableArray();
    $.getJSON("/Users/GetAllUsers", {},
            function (data)
            {
                $.each(data, function (index, item)
                {
                    self_.peoples.push(new User(index, item));
                });
            });

    self_.showAdd = ko.observable(false);

    self_.show = function ()
    {
        self_.addNewRecord();
        self_.showAdd(true);
    };

    self_.saveRecords = function ()
    {
        $.ajax("/Book/Save",
        {
            data: ko.toJSON(
            {
                BookHistoryList: $.merge(self_.historyToDelete, self_.newBookHistory()),
                BookId: window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length),
                BookCode: self_.bookItem.BookCode,
                BookTitle: self_.bookItem.BookTitle,
                BookAuthor: self_.bookItem.BookAuthor,
                BookDescription: self_.bookItem.BookDescription,
            }),
            type: "post",
            contentType: "application/json",
            success: function (data)
            {
                window.location.href = data.redirectToUrl + "/" + data.id;
            }
        });
    };

    self_.removeOldRecord = function (history)
    {
        self_.oldBookHistory.remove(history);
        history.deleted = true;
        self_.historyToDelete.push(history);
    };

    self_.removeNewRecord = function (history)
    {
        self_.newBookHistory.remove(history);
    };

    self_.addOldRecord = function (data)
    {
        self_.oldBookHistory.push(new BookHistoryItem(data));
    };

    self_.addNewRecord = function (data)
    {
        if (typeof data === "undefined")
        {
            data =
            {
                UserId: 0,
                FullName: "",
                OrderDate: new Date(),
                Action: self_.newBookHistory().length == 0 ?
                        self_.oldBookHistory().length == 0 ? 1 :
                        (self_.oldBookHistory()[0].action == "Take" ? 2 : 1) :
                        (self_.newBookHistory()[0].action == "Take" ? 2 : 1),
                Deleted: false
            };
        }
        self_.newBookHistory.unshift(new BookHistoryItem(data));
    };

    $.each(param, function (key, value)
    {
        self_.bookItem = {
            BookCode: param.BookCode,
            BookTitle: param.BookTitle,
            BookAuthor: param.BookAuthor,
            BookDescription: param.BookDescription
        };

        if (key == "BookHistoryList")
        {
            $.each(value, function (k, v)
            {
                self_.addOldRecord(v);
            });
        }
    });

    //ko.computed(function ()
    //{
    //$.each(self_.oldBookHistory(), function (key, value)
    //{
        
        //var now = new Date(moment().format("YYYY-MM-DD"));
        //if (now > new Date(value.orderDate))
        //{
        //    if (value.action == "Take") {
        //        self._reader = value.userFullName;
        //    } else if (value.action == "Return") {
        //        if (now == value.orderDate) {
        //            self_.reader = value.userFullName;
        //        } else {
        //            self_.reader = "";
        //        }
        //    }
        //}
    //});
    //});
}
