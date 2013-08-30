var isNumber = function (x)
{
    return !isNaN(x - 0);
};

window.BooksVm = function ()
{
    var self = this;

    self.TotalPages = ko.observable(1);
    self._page = ko.observable(1);
    self.isCorrectPage = function (page)
    {
        return page == 1 || (isNumber(page) && page > 0 && page <= self.TotalPages());
    };
    self.page = ko.computed({
        write: function (value)
        {
            if (value == self._page())
            {
                self._page("---");
                self.page(value);
                return;
            }
            if (!self.isCorrectPage(value))
            {
                if (!self.isCorrectPage(self._page()))
                {
                    self._page(1);
                }
                self.page(self._page());
                return;
            }
            self._page(value);
            self.LoadData();
        },
        read: function ()
        {
            return self._page();
        },
        owner: this
    });
    self.nextPage = function ()
    {
        self.page(self.page() - 0 + 1);
    };
    self.prevPage = function ()
    {
        self.page(self.page() - 1);
    };
    self.shouldShowNextPage = ko.computed(function ()
    {
        console.log(self);
        return self.page() < self.TotalPages();
    });
    self.shouldShowPrevPage = ko.computed(function ()
    {
        console.log(self);
        return self.page() > 1;
    });
    self.shouldShowPager = ko.computed(function ()
    {
        console.log(self);
        return self.TotalPages() > 1;
    });

    self._searchStr = "";
    self.searchStr = ko.computed({
        write: function (value)
        {
            self._searchStr = value;
            self.LoadData();
        },
        read: function ()
        {
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
    self.allBooks = ko.observableArray();

    self.BookVm = function (source)
    {
        var _self = this;
        _self.id = source.Id;
        _self.code = source.Code;
        _self.author = source.Author;
        _self.title = source.Title;
        _self.reader = source.ReaderName;
        _self.status = source.ReaderName == "None" ? "In stock" : "Unavailable";
        _self.tags = ko.observableArray();
        _self.getEditLink = function ()
        {
            return "/Book/Edit/" + _self.id;
        };
        _self.hasCover = source.HasCover;
        _self.imageUrl = function ()
        {
            if (_self.hasCover)
            {
                return "/Images/Books/" + source.Code + ".jpg";
            } else
            {
                return "/Images/binaryLogo.png";
            }
        };
    };

    self.BookShelfRow = function ()
    {
        var self_ = this;
        self_.booksRow = ko.observableArray();
    };

    self.shelfRows = ko.observableArray();
    self.bookQuantaty = 5;
    self.withCover = ko.observable(true);
    self.booksWithCover = ko.observable(0);

    self.detailsEnabled = ko.observable(false);

    self.enableDetails = function () {
        self.detailsEnabled(true);
    };
    
    self.disableDetails = function() {
        self.detailsEnabled(false);
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

    self.addTagsToBook = function (data) {
        console.log(data.length);
        //alert("data.length = " + data.length);
        //for (var i = 0; i < data.length; ++i)
        //{            
            //alert(data[i].BookId);
            //alert(data[i].Name);
            //self.allBooks()[data[i].BookId].push(data[i]);
        //}
    };

    self.getAllBooks = function (data)
    {
        self.allBooks.removeAll();
        for (var i = 0; i < data.length; ++i) {
            if (data[i].HasCover == true) {
                self.booksWithCover(self.booksWithCover() + 1);
            }
            self.allBooks.push(new self.BookVm(data[i]));
        }


        ///
        
        $.each(self.allBooks(), function (key) {
            var bookId = self.allBooks()[key].id;
            var data = { bookId: bookId };
            $.ajax(
            {
                url: "Book/GetBookTags",
                    //async: false,
                    data: data,
                    type: "GET",
                    dataType: "json",
                    success: self.addTagsToBook
                });
        });
        
        ///

        if (self.allBooks().length != 0) {
            for (var i = 0; i < parseInt(self.allBooks().length / self.bookQuantaty) + 1; ++i)
            {
                var shelfRow = new self.BookShelfRow();
                for (var j = 0; j < self.bookQuantaty; ++j)
                {
                    if (j + self.bookQuantaty * i < self.allBooks().length) {
                        shelfRow.booksRow.push(self.allBooks()[j + self.bookQuantaty * i]);
                    } else {
                    }
                }
                if (shelfRow.booksRow().length != 0) {
                    self.shelfRows.push(shelfRow);
                }
            }
        }

        if (self.shelfRows().length < 3) {
            for (var k = 0; k < 3 - self.shelfRows().length; k++) {
                $("#bookShelfBottom1").append('<div class="row" style="background: url(/Images/bs4.jpg); background-repeat: no-repeat"><div class="column" style="width: 90px; height: 120px;"></div></div>');
            }
        }
    };

    self.loadAllBooks = function ()
    {
        $.ajax(
            {
                url: "Book/ListAllBooks",
                type: "GET",
                dataType: "json",
                success: self.getAllBooks
            });
    };

    self.loadAllBooks();

    self.booksView = ko.observable("List");

    $("#booksViewBtn").on("click", function ()
    {
        if (self.booksView() == "List")
        {
            self.booksView("Shelf");
            $("#bookShelf").hide();
            $("#coverCheck").hide();
            $("#bookList").show();
            $("#booksSearch").show();
        } else
        {
            self.booksView("List");
            $("#bookShelf").show();
            $("#bookList").hide();
            $("#booksSearch").hide();
            $("#coverCheck").show();
        }
    });

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
        console.log(data);
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