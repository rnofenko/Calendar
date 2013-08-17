using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    class FakeConfig : IConfig
    {
        private int? _pageSize;
        private bool? _sendMail;

        public bool SendEmail
        {
            get { return _sendMail ?? false; }
            set { _sendMail = value; }
        }

        public string TeamHeaderPattern
        {
            get { throw new NotImplementedException(); }
        }

        public int PageSize
        {
            get { return _pageSize ?? 7; }
            set { _pageSize = value; }
        }
    }
}
