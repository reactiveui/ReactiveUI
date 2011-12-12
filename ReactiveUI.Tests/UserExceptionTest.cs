using ReactiveUI;
using Xunit;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace ReactiveUI.Tests
{
    public class UserExceptionTest : IEnableLogger
    {
        [Fact]
        public void UserExceptionConstructorTest()
        {
            return;
            UserException target = new UserException("Foo");
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
