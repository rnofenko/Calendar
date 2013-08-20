using System;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class BookHistory : BaseEntity
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DirectionEnums Action { get; set; }
    }
}