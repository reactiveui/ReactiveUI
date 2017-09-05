using System;
using System.Diagnostics;
using System.Reflection;
using Xunit.Sdk;

namespace UnitTests.Utility
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DebugAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest) =>
            Debug.WriteLine("BEFORE: " + methodUnderTest.Name);

        public override void After(MethodInfo methodUnderTest) =>
            Debug.WriteLine("AFTER: " + methodUnderTest.Name);
    }
}
