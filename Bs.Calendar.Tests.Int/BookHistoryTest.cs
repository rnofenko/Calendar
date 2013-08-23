using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using FluentAssertions;
using NUnit.Framework;
using Bs.Calendar.Mvc.Services;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    public class BookHistoryTest
    {
        private BookHistoryService _bookHistoryService;
        private RepoUnit _unit;

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();
            Ioc.RegisterType<IBookRepository, BookRepository>();
            Ioc.RegisterType<IBookHistoryRepository, BookHistoryRepository>();
            Ioc.RegisterType<IUserRepository, UserRepository>();

            _unit = new RepoUnit();

            _unit.User.Save(new User { FirstName = "Dominick", LastName = "Cobb"});
            _unit.Book.Save(new Book { Author = "Christopher Nolan", Title = "Inception", Code = "0-1011101-1-0" });
        
            _bookHistoryService = new BookHistoryService(_unit);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var firstBookToDelete = _unit.Book.Get(book => book.Code == "0-1011101-1-0");
            var secondBookToDelete = _unit.Book.Get(book => book.Code == "9379992");
            _unit.Book.Delete(firstBookToDelete);
            _unit.Book.Delete(secondBookToDelete);

            var userToDelete = _unit.User.Get(user => user.FullName == "Dominick Cobb");
            _unit.User.Delete(userToDelete);            
            
            var firstBookHistory = _unit.BookHistory.Load(h => h.BookId == firstBookToDelete.Id).ToList();
            var secondBookHistory = _unit.BookHistory.Load(h => h.BookId == secondBookToDelete.Id).ToList();
            foreach (var bookHistory in firstBookHistory)
            {
                _unit.BookHistory.Delete(bookHistory);
            }
            foreach (var bookHistory in secondBookHistory)
            {
                _unit.BookHistory.Delete(bookHistory);
            }
        }

        [Test]
        public void ShouldAddNewRecordToTheBook()
        {
            // arrange
            var book = _unit.Book.Get(b => b.Code == "0-1011101-1-0");
            var user = _unit.User.Get(u => u.FullName == "Dominick Cobb");
            var bookHistoryVm = new BookHistoryVm(book);
            var bookHistoryList = new List<BookHistory>
            {
                new BookHistory
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    OrderDirection = DirectionEnums.Take,
                    TakeDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(1)
                }
            };
            bookHistoryVm.BookHistoryList = new List<BookHistory>(bookHistoryList);

            // act
            _bookHistoryService.AddRecord(bookHistoryVm);

            // assert
            _bookHistoryService.GetBookHistories(book.Id).BookHistoryList.Count.Should().Be(1);
        }

        [Test]
        public void RecordsShouldBeOrderedByTakeDateDeskAndGroupedByOrderDirection()
        {
            // arrange
            var book = new Book {Author = "author", Title = "title", Code = "9379992"};
            _unit.Book.Save(book);
            book = _unit.Book.Get(b => b.Code == "9379992");
            var user = _unit.User.Get(u => u.FullName == "Dominick Cobb");
            var firstRecord = new BookHistory
            {
                BookId = book.Id, UserId = user.Id,
                OrderDirection = DirectionEnums.Take,
                TakeDate = DateTime.Now, ReturnDate = DateTime.Now.AddDays(1)
            };
            var secondRecord = new BookHistory
            {
                BookId = book.Id, UserId = user.Id,
                OrderDirection = DirectionEnums.Return,
                TakeDate = DateTime.Now, ReturnDate = DateTime.Now.AddDays(1)
            };
            _unit.BookHistory.Save(firstRecord);
            _unit.BookHistory.Save(secondRecord);

            // act
            var result = _bookHistoryService.GetBookHistories(book.Id);

            // assert
            result.BookHistoryList.Count.Should().Be(2, "because we added only two records to the book history");
            result.BookHistoryList.First().OrderDirection.Should().Be(DirectionEnums.Return);
            result.BookHistoryList.Last().OrderDirection.Should().Be(DirectionEnums.Take);
        }    
    }
}