namespace Bs.Calendar.Models.Bases
{
    public class BaseEntity : IEntityId
    {
        public const int LENGTH_NAME = 200;

        public int Id { get; set; }
    }
}
