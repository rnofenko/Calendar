var isNumber = function(x) {
    return !isNaN(x - 0);
};

window.BooksVm = function ()
{
    var self = this;

    self.TotalPages = ko.observable(1);
    self._page = ko.observable(1);
    self.isCorrectPage = function (page) {
        return page == 1 || (isNumber(page) && page > 0 && page <= self.TotalPages());
    };
    self.page = ko.computed({
        write: function (value) {
            if (value == self._page()) {
                self._page("---");
                self.page(value);
                return;
            }
            if (!self.isCorrectPage(value)) {
                if (!self.isCorrectPage(self._page())) {
                    self._page(1);
                }
                self.page(self._page());
                return;
            }
            self._page(value);
            self.LoadData();
        },
        read: function () {
            return self._page();
        },
        owner: this
    });
    self.nextPage = function() {
        self.page(self.page() - 0 + 1);
    };
    self.prevPage = function() {
        self.page(self.page() - 1);
    };
    self.shouldShowNextPage = ko.computed(function () {
        console.log(self);
        return self.page() < self.TotalPages();
    });
    self.shouldShowPrevPage = ko.computed(function () {
        console.log(self);
        return self.page() > 1;
    });
    self.shouldShowPager = ko.computed(function () {
        console.log(self);
        return self.TotalPages() > 1;
    });

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
        _self.id = source.Id;
        _self.code = source.Code;
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
        self.TotalPages(data['TotalPages']);
        data = data['Data'];
        for (var i = 0; i < data.length; ++i)
        {
            self.books.push(new self.BookVm(data[i]));
        }
    };

    self.LoadData = function ()
    {
        var data = {
            orderby: self._orderby,
            page: self.page()
        };
        console.log(data);
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