using System;
using N = NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
	public class TestClassAttribute : N.TestFixtureAttribute { }
	public class TestMethodAttribute : N.TestAttribute { }
	
	public static class Assert
	{
		public static void IsTrue(bool b, string message = "") { N.Assert.IsTrue(b, message); }
		public static void IsFalse(bool b) { N.Assert.IsFalse(b); }
		public static void AreEqual<T>(T expected, T actual) { N.Assert.AreEqual(expected, actual); }
		public static void Fail(string message = "") { N.Assert.Fail(message); }
		public static void IsNull(object o) { N.Assert.IsNull(o); }
		public static void IsNotNull(object o) { N.Assert.IsNotNull(o); }
		public static void Inconclusive(string message = "") { N.Assert.Fail(message); }
	}
}