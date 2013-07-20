namespace Bs.Calendar.Models.Bases
{
    public class BaseEntity : IEntityId
    {
        public const int LENGTH_NAME = 200;

        public const int MIN_COLOR_VALUE = 0;
        public const int MAX_COLOR_VALUE = 7;

        public int Id { get; set; }
    }
}
