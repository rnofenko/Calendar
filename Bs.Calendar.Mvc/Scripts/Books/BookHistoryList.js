function BookHistory(source)
{
    var _self = this;

    _self.userId = ko.observable(source.UserId);
    _self.userFullName = ko.observable();

    ko.computed(function ()
    {
        if (_self.userId() == 0)
        {
            $.getJSON("/Users/GetCurrentUserId", {}, _self.userId);
        }
        else
        {
            $.getJSON("/Users/GetUserFullName", { id: _self.userId }, _self.userFullName);
        }
    }, this);

    _self.takeDate = new Date(parseInt(source.TakeDate.substr(6))).toISOString().substr(0, 10);
    _self.returnDate = new Date(parseInt(source.ReturnDate.substr(6))).toISOString().substr(0, 10);
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
            
    self.addBookHistory = function (data)
    {
        if (typeof data === "undefined" || data == self)
        {
            data =
            {
                UserId: 0,
                TakeDate: (new Date()).toJSON(),
                ReturnDate: (new Date()).toJSON()
            };
        }

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
}
