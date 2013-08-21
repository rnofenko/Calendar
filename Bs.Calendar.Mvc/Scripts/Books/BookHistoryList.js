function BookHistory(source)
{
    var _self = this;

    _self.userId = source.UserId;
    _self.userFullName = ko.observable();

    ko.computed(function ()
    {
        if (_self.userId != 0) {
            $.getJSON("/Users/GetUserFullName", { id: _self.userId }, _self.userFullName);
        }
    }, this);

    _self.orderDate = moment(source.OrderDate).format("Do MMMM, YYYY");

    _self.actionId = source.Action;
    _self.action = source.Action == 1 ? "Take" : "Return";
}

function User(id, name)
{
    var self = this;

    self.peopleId = id;
    self.peopleName = name;
}

function BookHistoryList(param)
{
    var self = this;

    self.bookId = window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length);
    self.bookHistory = ko.observableArray();
    self.newBookHistory = ko.observableArray();
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
    self.showList = ko.observable(true);
  
    self.show = function ()
    {
        self.addNewBookHistory();
        self.showAdd(true);
    };

    //$("#SaveHistoryButton").on("click", function ()
    //{
    //    self.showAdd(false);
    //    self.showList(true);
    //});

    self.addNewBookHistory = function (data)
    {
        if (typeof data === "undefined")
        {
            data =
            {
                UserId: 0,
                OrderDate: ko.observable(new Date().toJSON()),
                Action: (self.bookHistory().length + self.newBookHistory().length) % 2 + 1
            };
        }
        self.newBookHistory.push(new BookHistory(data));
    };
    
    self.addBookHistory = function (data)
    {
        self.bookHistory.push(new BookHistory(data));
    };

    $.each(param, function (key, value)
    {
        self.addBookHistory(value);
    });
    
    self.indexedName = function (index, parameter)
    {
        return "BookHistoryList[" + index + "]." + parameter;
    };

    self.removeRecord = function (history)
    {
        self.newBookHistory.remove(history);
    };
}