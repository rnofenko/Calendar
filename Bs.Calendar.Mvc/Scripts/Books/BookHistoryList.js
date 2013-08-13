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

function BookHistoryVm(param)
{
    var self = this;
    self.bookHistory = ko.observableArray();
    
    self.showOrder = ko.observable(true);
    self.showOk = ko.observable(false);
    self.show = function ()
    {
        self.showOrder(false);
        self.showOk(false);
    };
    $("#OrderButton").on("click", function() {
        self.showOk(true);
    });

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