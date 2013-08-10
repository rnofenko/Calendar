using System;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class BookHistory : BaseEntity
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime TakeDate { get; set; }
        public DateTime ReturnDate { get; set; }
        //public DirectionEnum DirectionEnum { get; set; }
    }
}