﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Rules;
using FluentAssertions;
using NUnit.Framework;
using Bs.Calendar.Core;

namespace Bs.Calendar.Tests.Unit
{

    [TestFixture]
    class PageCalculatorTest
    {
        [Test]
        public void Can_Return_Valid_Page_Counter()
        {
            //act
            var pageCount = PageCounter.GetTotalPages(10, 3);

            //assert
            pageCount.ShouldBeEquivalentTo(4);
        }

        [Test]
        public void Can_Return_Ranged_Page() 
        {
            //arrange
            var totalPages = 3;

            //act
            var validPage = PageCounter.GetRangedPage(2, totalPages);
            var greaterPage = PageCounter.GetRangedPage(4, totalPages);
            var smallerPage = PageCounter.GetRangedPage(0, totalPages);

            //assert
            validPage.ShouldBeEquivalentTo(2);
            greaterPage.ShouldBeEquivalentTo(totalPages);
            smallerPage.ShouldBeEquivalentTo(1);
        }
    }
}
