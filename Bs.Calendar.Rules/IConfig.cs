namespace Bs.Calendar.Rules
{
    public interface IConfig
    {
        bool SendEmail { get; set; }

        string TeamHeaderPattern { get; }

        int PageSize { get; }
    }
}
