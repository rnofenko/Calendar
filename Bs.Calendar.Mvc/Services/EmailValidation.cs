using System.Text.RegularExpressions;

namespace Bs.Calendar.Mvc.Services
{
    public static class EmailValidation
    {
        public static bool IsValidEmailAddress(this string s)
        {
            var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }
    }
}