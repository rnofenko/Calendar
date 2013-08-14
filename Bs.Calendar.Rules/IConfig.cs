namespace Bs.Calendar.Rules
{
    public interface IConfig
    {
        int PageSize { get; set; }
        bool SendEmail { get; set; }

        string TeamHeaderPattern { get; }
    }
}
