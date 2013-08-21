using System;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookHistoryItemVm
    {

        public int UserId { get; set; }
        public string FullName { get; set; }
        public int BookId { get; set; }
        public DateTime OrderDate { get; set; }
        public DirectionEnums Action { get; set; }

        public BookHistoryItemVm()
        {            
        }

        public BookHistoryItemVm(BookHistory model)
        {
            FullName = model.User.FullName;
            Action = model.Action;
            OrderDate = model.OrderDate;
            BookId = model.BookId;
            UserId = model.UserId;
        }
    }
}