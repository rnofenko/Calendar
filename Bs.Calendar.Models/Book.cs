using System.ComponentModel.DataAnnotations;

namespace Bs.Calendar.Models
{
    public class Book : Bases.BaseEntity
    {
        [StringLength(LENGTH_NAME)]
        public string Code { get; set; }

        [StringLength(LENGTH_NAME)]
        public string Title { get; set; }

        [StringLength(LENGTH_NAME)]
        public string Author { get; set; }

        public string Description { get; set; }
    }
}
