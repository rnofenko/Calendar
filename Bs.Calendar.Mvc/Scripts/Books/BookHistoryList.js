function BookHistory(source)
{
    var _self = this;

    _self.userId = source.UserId;
    _self.userFullName = ko.observable();

    _self.bookId = ko.observable(source.BookId);

    ko.computed(function ()
    {
        $.getJSON("/Users/GetUserFullName", { id: _self.userId }, _self.userFullName);
    }, this);

    _self.takeDate = new Date(parseInt(source.TakeDate.substr(6))).toISOString().substr(0, 10);
    _self.returnDate = new Date(parseInt(source.ReturnDate.substr(6))).toISOString().substr(0, 10);

    _self.orderDirection = source.OrderDirection;

    ko.computed(function ()
    {
        if (_self.orderDirection == 1)
        {
            _self.orderDirection = "Take";
        } else if (_self.orderDirection == 2)
        {
            _self.orderDirection = "Return";
        }
    }, this);
}

function People(id, name)
{
    var self = this;

    self.peopleId = id;
    self.peopleName = name;
}

function BookHistoryVm(param)
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
                    self.peoples.push(new People(index, item));
                });
            });
    }, this);

    self.showAdd = ko.observable(false);
    self.showList = ko.observable(true);

    self.show = function ()
    {
        self.showAdd(true);
        self.showList(false);
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
    self.bookId = ko.observable();

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
                success: function (result)
                {
                    //alert(result);
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

    //self.indexedName = function (index, parameter)
    //{
    //    return "BookHistoryList[" + index + "]." + parameter;
    //};
}
