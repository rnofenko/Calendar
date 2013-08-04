using System;
using System.Configuration;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class MvcConfig : IConfig
    {
        private bool? _sendEmail;

        public bool SendEmail
        {
            get
            {
                if (_sendEmail == null)
                {
                    var parameter = ConfigurationManager.AppSettings["SendEmail"];
                    _sendEmail = parameter != null && parameter.Equals("true", StringComparison.OrdinalIgnoreCase);
                }

                return _sendEmail.Value;
            }
            set { _sendEmail = value; }
        }
    }
}