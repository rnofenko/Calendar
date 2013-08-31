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
            self.LoadSearch();
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
    self.serverBooks = ko.observableArray();
    self.allTags = ko.observableArray();
    self._allTags = [];
    self.BookVm = function (source)
    {
        var _self = this;
        _self.id = source.Id;
        _self.code = source.Code;
        _self.author = source.Author;
        _self.title = source.Title;
        _self.reader = source.ReaderName;
        _self.status = source.ReaderName == "None" ? "In stock" : "Unavailable";
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
        
        _self.detailsEnabled = ko.observable(false);
        _self.enableDetails = function ()
        {
            _self.detailsEnabled(true);
        };
        _self.disableDetails = function ()
        {
            _self.detailsEnabled(false);
        };
    };

    self.BookTagVm = function (source)
    {
        var _self = this;
        _self.id = source.Id;
        _self.code = source.Code;
        _self.author = source.Author;
        _self.title = source.Title;
        _self.reader = source.ReaderName;
        _self.status = source.ReaderName == "None" ? "In stock" : "Unavailable";
        _self.tags = ko.observableArray(source.BookTags);
        console.log(_self.tags());
        console.log("_self.tags length = " + _self.tags().length);
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

    self.showTags = ko.observable(false);

    self.showTagsList = function ()
    {
        if (self.showTags() == false) {
            self.showTags(true);
        } else {
            self.showTags(false);
        }
    };

    self.getAllBooks = function (data, options) {
        if (options && options.reset){
            data = self.serverBooksRow;
            self.filters.removeAll();
        }
        self.allBooks.removeAll();
        self.shelfRows.removeAll();
        $("#bookShelfBottom1").empty();
        for (var i = 0; i < data.length; ++i)
        {
            if (data[i].HasCover == true) {
                self.booksWithCover(self.booksWithCover() + 1);
            }
            self.allBooks.push(new self.BookTagVm(data[i]));
            if (options && options.isFromServer){
                self.serverBooksRow = data;
                self.serverBooks.push(new self.BookTagVm(data[i]));
            }
        }
        
            

        console.log("all books");
        console.log(self.allBooks);

        if (self.allBooks().length != 0)
        {
            console.log(self.allBooks().length);
            for (var i = 0; i < self.allBooks().length; ++i)
            {
                $.each(self.allBooks()[i].tags(), function (key, value)
                {
                    if ($.inArray(value, self._allTags) === -1) {

                        self.allTags.push({title: value, isSelected: ko.observable(false)});
                        self._allTags.push(value);
                    }
                });
            }
            console.log("ALL TAGS = " + self.allTags());
            for (var i = 0; i < parseInt(self.allBooks().length / self.bookQuantaty) + 1; ++i)
            {
                var shelfRow = new self.BookShelfRow();
                for (var j = 0; j < self.bookQuantaty; ++j)
                {
                    if (j + self.bookQuantaty * i < self.allBooks().length)
                    {
                        shelfRow.booksRow.push(self.allBooks()[j + self.bookQuantaty * i]);
                    }
                }
                if (shelfRow.booksRow().length != 0)
                {
                    self.shelfRows.push(shelfRow);
                }
            }
        }

        if (self.shelfRows().length < 3)
        {
            for (var k = 0; k < 3 - self.shelfRows().length; k++)
            {
                $("#bookShelfBottom1").append('<div class="row" style="background: url(/Images/bs4.jpg); background-repeat: no-repeat"><div class="column" style="width: 90px; height: 120px;"></div></div>');
            }
        }
    };

    self.filters = ko.observableArray();

    self.filterClick = function (param)
    {
        console.log("filterClick: param =  " + param);
        console.log("filterClick: filters() =  " + self.filters());
        param.isSelected(!param.isSelected());
        if ($.inArray(param, self.filters()) === -1)
        {
            self.filters.push(param);
        } else {
            self.filters.remove(param);
        }

        console.log(self.filters());

        var filteredBooks = new Array();

        for (var i = 0; i < self.filters().length; i++) {
            for (var j = 0; j < self.serverBooks().length; j++) {
                for (var k = 0; k < self.serverBooks()[j].tags().length; k++) {
                    if (self.filters()[i].title == self.serverBooks()[j].tags()[k]) {
                        var exists = false;
                        $.each(filteredBooks, function (key, v) {
                            if (v.code == self.serverBooks()[j].code) {
                                exists = true;
                                return false;
                            }
                        });
                        if (!exists) {
                            filteredBooks.push(self.serverBooks()[j]);
                        }
                        break;
                    }                    
                }
            }                
        }

        console.log(filteredBooks);
        //self.allBooks.removeAll();        
        filteredBooks.map(function(it){
            it.Author = it.author;
            it.Code = it.code;
            it.BookTags = it.tags();
            it.Title = it.title;
            it.HasCover = it.hasCover;
            it.Id = it.id;
            it.ReaderName = it.reader;
        });
        self.getAllBooks(filteredBooks);
    };

    self.clearFiltering = function(){
        self.getAllBooks({}, {reset: true});
        self.allTags().forEach(function(it){
            it.isSelected(false);
        });
    }

    self.loadAllBooks = function ()
    {
        $.ajax(
            {
                url: "/Book/ListAllBooks",
                type: "GET",
                dataType: "json",
                success: function(data) {
                    self.getAllBooks(data, {isFromServer: true});
                }
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
            $("#tagBtn").hide();
        } else
        {
            self.booksView("List");
            $("#bookShelf").show();
            $("#bookList").hide();
            $("#booksSearch").hide();
            $("#coverCheck").show();
            $("#tagBtn").show();
        }
    });
    
    self.LoadSearch = function ()
    {
        var data = {
            orderby: self._orderby,
            //page: self.page()
        };
        console.log(data);
        if (self._searchStr != "")
        {
            data['search'] = self._searchStr;
            $.ajax(
            {
                url: "/Book/ListSearch",
                type: "GET",
                data: data,
                dataType: "json",
                success: self.recieveData,
            });
        } else {
            data = {
                orderby: self._orderby,
                page: self.page()
            };
            $.ajax(
           {
               url: "/Book/List",
               type: "GET",
               data: data,
               dataType: "json",
               success: self.recieveData,
           });
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