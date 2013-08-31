using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class Tag : BaseEntity
    {
        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        public string Name { get; set; }
    }
}
