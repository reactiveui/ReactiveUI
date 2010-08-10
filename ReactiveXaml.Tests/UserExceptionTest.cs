using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class UserExceptionTest
    {
        [TestMethod()]
        public void UserExceptionConstructorTest()
        {
            UserException target = new UserException("Foo", "Bar");
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
