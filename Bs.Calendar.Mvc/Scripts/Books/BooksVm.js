window.BooksVm = function ()
{
    var self = this;
    
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
        for (var i = 0; i < data.length; ++i)
        {
            self.books.push(new self.BookVm(data[i]));
        }
    };

    self.LoadData = function ()
    {
        $.ajax(
            {
                url: "/Book/List",
                type: "GET",
                dataType: "json",
                success: self.recieveData,
            });
    };
    self.LoadData();
};