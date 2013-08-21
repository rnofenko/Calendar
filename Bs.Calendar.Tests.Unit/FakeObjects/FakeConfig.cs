using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    public class FakeConfig : IConfig
    {
        private int? _pageSize;
        private bool? _sendMail;
        private string _teamHeaderPattern;

        public bool SendEmail
        {
            get { return _sendMail ?? false; }
            set { _sendMail = value; }
        }

        public string TeamHeaderPattern
        {
            get { return _teamHeaderPattern ?? "ABC,DEF,GHIJ,KLMN,OPQR,STUV,WXYZ"; }
            set { _teamHeaderPattern = value; }
        }

        public int PageSize
        {
            get { return _pageSize ?? 7; }
            set { _pageSize = value; }
        }

        public DateTime Now { get { return DateTime.Now; } }
    }
}
