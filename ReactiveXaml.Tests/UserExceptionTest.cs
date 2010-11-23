using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class UserExceptionTest : IEnableLogger
    {
        [TestMethod()]
        public void UserExceptionConstructorTest()
        {
            return;
            UserException target = new UserException("Foo");
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :