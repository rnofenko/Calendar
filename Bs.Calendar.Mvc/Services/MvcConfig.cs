using System;
using System.Configuration;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class MvcConfig : IConfig
    {
        private int? _pageSize;
        private bool? _sendEmail;
        private string _teamHeaderPattern;

        public int PageSize
        {
            get
            {
                if (!_pageSize.HasValue)
                {
                    const int pageSizeByDefault = 7;

                    int pageSize;
                    _pageSize = int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageSize) ? pageSize : pageSizeByDefault;
                }

                return _pageSize.Value;
            }
            set
            {
                _pageSize = value;
            }
        }

        public bool SendEmail
        {
            get
            {
                if (_sendEmail == null)
                {
                    var parameter = ConfigurationManager.AppSettings["SendEmail"];
                    _sendEmail = parameter != null && parameter.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
                }

                return _sendEmail.Value;
            }
            set { _sendEmail = value; }
        }

        public string TeamHeaderPattern
        {
            get
            {
                if (_teamHeaderPattern == null)
                {
                    _teamHeaderPattern = ConfigurationManager.AppSettings["TeamHeaderPattern"];
                }
                return _teamHeaderPattern;
            }
        }
    }
}