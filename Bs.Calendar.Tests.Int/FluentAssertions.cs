using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class WhyFluentAssertionIsReallyCoolThing
    {
        [Test]
        public void FirstExample()
        {
            "1234567890".Should().Be("1234098765");
            /*
                Expected string to be 
                "1234098765", but 
                "1234567890" differs near "567" (index 4).
            */

            // Assert.AreEqual("1234567890", "1234098765");

            /*
                String lengths are both 10. Strings differ at index 4.
                Expected: "1234567890"
                But was:  "1234098765"
                ---------------^
            */
        }

        [Test]
        public void SecondExample()
        {
            var arr = new[] { 1, 2, 3 };
            arr.Should().Contain(element => element > 3, "at least {0} element should be larger than 3", 1);

            // Collection {1, 2, 3} should have an item matching (element > 3) 
            // because at least 1 element should be larger than 3.

            // I HAVE NO IDEA HOW TO MAKE IT USING NUNIT.. 
        }

        [Test]
        public void ThirdExample()
        {
            const string actual = "ABCDEFGHI";
            actual.Should().StartWith("AB").And.EndWith("HI").And.Contain("EF").And.HaveLength(9);

            // I HAVE NO IDEA HOW TO MAKE IT USING NUNIT..             
        }
    }
}
