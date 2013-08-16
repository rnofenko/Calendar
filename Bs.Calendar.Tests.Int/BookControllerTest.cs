using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class BookControllerTest
    {
        private BookController _bookController;
        private BookService _bookService;
        private BookHistoryService _bookHistoryService;
        private RepoUnit _unit;

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();
            Ioc.RegisterType<IBookRepository, BookRepository>();
            Ioc.RegisterType<IBookHistoryRepository, BookHistoryRepository>();
            
            _unit = new RepoUnit();

            _unit.Book.Save(new Book{Author = "Andrew Hunt", Title = "Pragmatic unit testing", Code = "0-9745140-2-0"});
            
            _bookService = new BookService(_unit);
            _bookHistoryService = new BookHistoryService(_unit);            
            _bookController = new BookController(_bookService, _bookHistoryService);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var firstBookToDelete = _unit.Book.Get(book => book.Code == "0-9745140-2-0");
            var secondBookToDelete = _unit.Book.Get(book => book.Code == "5-8046-0051-6");
            _unit.Book.Delete(firstBookToDelete);
            _unit.Book.Delete(secondBookToDelete);
        }

        [Test]
        public void ShouldAddNewBookToTheDatabase()
        {
            // arrange
            var quantaty = _unit.Book.Load().Count();
            var newBook = new Book {Author = "Ken Bent", Title = "Extreme programming", Code = "5-8046-0051-6"};

            // act
            _bookService.Save(new BookHistoryVm(newBook));

            // assert
            _unit.Book.Load().Count().Should().Be(++quantaty);
        }

        [Test]
        public void ShouldThrowAnExceptionWhenTryingToAddNewBookWithExistingInTheDatabaseCode()
        {
            // arrange
            var sameBook = new Book
            {
                Author = "David Thomas",
                Title = "Pragmatic unit testing in C# with NUnit",
                Code = "0-9745140-2-0"
            };

            // act
            Action action = () => _bookService.Save(new BookHistoryVm(sameBook));

            // assert
            action.ShouldThrow<WarningException>().WithMessage("Code should be unique");
        }

        [Test]
        public void ShouldEditBookData()
        {
            // arrange
            var bookToModify = _unit.Book.Get(book => book.Code == "0-9745140-2-0");
            bookToModify.Title = "Pragmatic unit testing in C# with NUnit";

            // act 
            _bookController.Edit(new BookHistoryVm(bookToModify));
            var changedBook = _bookService.Get(bookToModify.Id);

            // assert
            changedBook.Id.Should().Be(bookToModify.Id);
            changedBook.Title.Should().Be(bookToModify.Title);
        }

        [Test]
        public void ShouldDeleteBook()
        {
            // arrange
            var bookToDelete = new Book
            {
                Author = "James W. Newkirk, Alexei A. Vorontsov",
                Title = "Test-Driven Development in Microsoft .NET",
                Code = "978-0735619487"
            };
            _bookService.Save(new BookHistoryVm(bookToDelete));
            bookToDelete = _unit.Book.Get(book => book.Code == "978-0735619487");

            // act
            _bookController.Delete(bookToDelete.Id);

            // assert
            _unit.Book.Get(bookToDelete.Id).Should().BeNull();
        }
    }
}