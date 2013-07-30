using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class MvcConfig : IConfig 
    {
        public bool SendEmail 
        {
            get 
            {
                return ConfigurationManager.AppSettings["SendEmail"].Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}