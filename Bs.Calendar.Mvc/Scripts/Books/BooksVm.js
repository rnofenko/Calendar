window.BooksVm = function ()
{
    var self = this;

    self._searchStr = "";
    self.searchStr = ko.computed({
        write: function (value) {
            self._searchStr = value;
            self.LoadData();
        },
        read: function () {
            return self._searchStr;
        },
        owner: this
    });

    self._orderby = "Id";
    self.toggle_orderby = function (k, event)
    {
        var key = event.target.id.substring(3);
        if (self._orderby == key)
        {
            key = "-" + key;
        }
        $("#order_" + self._orderby).toggleClass('hide');
        self._orderby = key;
        $("#order_" + self._orderby).toggleClass('hide');
        self.LoadData();
    };

    self.books = ko.observableArray();

    self.BookVm = function(source)
    {
        var _self = this;
        _self.id= source.Id;
        _self.author = source.Author;
        _self.title = source.Title;
        _self.getEditLink = function()
        {
            return "/Book/Edit/" + _self.id;
        };
    };

    self.recieveData = function (data)
    {
        self.books.removeAll();
        for (var i = 0; i < data.length; ++i)
        {
            self.books.push(new self.BookVm(data[i]));
        }
    };

    self.LoadData = function ()
    {
        var data = {
            orderby: self._orderby
        };
        if (self._searchStr != "")
        {
            data['search'] = self._searchStr;
        }
        $.ajax(
            {
                url: "/Book/List",
                type: "GET",
                data: data,
                dataType: "json",
                success: self.recieveData,
            });
    };
    self.LoadData();
};