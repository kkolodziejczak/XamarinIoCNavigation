using System;
using NUnit.Framework;
using TestDI.Services;

namespace TestDI.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test()
        {
            var service = new NavigationService(null, null);
            Assert.Pass("Test");
        }
    }
}
