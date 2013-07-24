using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class HomeBirthdayListVm
    {
        public IEnumerable<User> GuysWhoHaveBirthdays { get; set; }
    }
}