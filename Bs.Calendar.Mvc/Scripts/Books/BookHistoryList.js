function BookHistory(source)
{
    var _self = this;

    _self.userId = source.UserId;    
    _self.userFullName = ko.observable();
    ko.computed(function ()
    {
        $.getJSON("/Users/GetUserFullName", { id: _self.userId }, _self.userFullName);
    }, this);

    _self.takeDate = new Date(parseInt(source.TakeDate.substr(6))).toISOString().substr(0, 10);
    _self.returnDate = new Date(parseInt(source.ReturnDate.substr(6))).toISOString().substr(0, 10);

    _self.orderDirection = source.OrderDirection == 1 ? "Take" : "Return";
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

    self.bookHistory = ko.observableArray();
    self.peoples = ko.observableArray();
    self.orderDirections = ko.observableArray(['Take', 'Return']);

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
        self.showAdd(true);
    };
    $("#SaveHistoryButton").on("click", function ()
    {
        self.showAdd(false);
        self.showList(true);
    });

    self.takeDate = ko.observable(new Date().toJSON());
    self.returnDate = ko.observable(new Date().toJSON());
    self.orderDirection = ko.observable();
    self.userId = ko.observable();    
    self.bookId = window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length);

    self.save = function ()
    {
        $.ajax("/Book/Save",
            {
                data: ko.toJSON(
                    {
                        TakeDate: self.takeDate,
                        ReturnDate: self.returnDate,
                        OrderDirection: self.orderDirection,
                        UserId: self.userId,
                        BookId: self.bookId
                    }),
                type: "post",
                contentType: "application/json",
                success: function (data)
                {
                    window.location.href = data.redirectToUrl + "/" + self.bookId;
                }
            });
    };

    self.addBookHistory = function (data)
    {
        self.bookHistory.push(new BookHistory(data));
    };

    $.each(param, function (key, value)
    {
        self.addBookHistory(value);
    });
}
