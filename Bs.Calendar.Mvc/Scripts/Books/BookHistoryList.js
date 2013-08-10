function BookHistory(source)
{
    var _self = this;
    
    _self.bookId = source.BookId;
    _self.bookTitle = source.BookTitle;
    _self.bookAuthor = source.BookAuthor;
    
    _self.userId = source.UserId;
    _self.userFullName = source.UserFullName;
                                            
    _self.takeDate = new Date(parseInt(source.TakeDate.substr(6))).toISOString().substr(0, 10);
    _self.returnDate = new Date(parseInt(source.ReturnDate.substr(6))).toISOString().substr(0, 10);
}

function BookHistoryVm(id)
{
    var self = this;
    
    self.bookHistory = ko.observableArray();
    
    self.addBookHistory = function (data)
    {
        if (typeof data === "undefined" || data == self)
        {
            data =
            {
                BookId: 0,
                BookTitle: "",
                BookAuthor: "",
                UserId: 0,
                UserFullName: "",
                TakeDate: (new Date()).toJSON(),
                ReturnDate: (new Date()).toJSON()
            };
        }
        self.bookHistory.push(new BookHistory(data));
    };

    self.recieveData = function (data)
    {
        self.bookHistory.removeAll();
        for (var i = 0; i < data.length; ++i)
        {
            self.bookHistory.push(new BookHistory(data[i]));
        }
    };

    self.LoadData = function ()
    {
        $.ajax(
            {
                url: "/Book/BookHistoryList/".concat(id),
                type: "GET",
                dataType: "json",
                success: self.recieveData,
            });
    };
    self.LoadData();
}