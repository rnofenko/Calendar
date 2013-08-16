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

        public bool SendEmail
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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
