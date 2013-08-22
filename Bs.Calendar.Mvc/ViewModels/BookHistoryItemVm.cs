using System;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookHistoryItemVm : BaseEntity
    {        
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int BookId { get; set; }
        public DateTime OrderDate { get; set; }
        public DirectionEnums Action { get; set; }

        public bool Deleted { get; set; }

        public BookHistoryItemVm()
        {            
        }

        public BookHistoryItemVm(BookHistoryItem model)
        {
            Id = model.Id;
            FullName = model.User.FullName;
            Action = model.Action;
            OrderDate = model.OrderDate;
            BookId = model.BookId;
            UserId = model.UserId;
        }
    }
}