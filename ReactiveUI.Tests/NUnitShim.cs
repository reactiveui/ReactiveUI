using System;
using System.Linq;
using N = NUnit.Framework;
using System.Collections.Generic;

namespace Xunit
{
    public class FactsAttribute : N.TestFixtureAttribute {}
    public class FactAttribute : N.TestAttribute {}

    public static class Assert
    {
        public static void False(bool condition)
        {
            N.Assert.IsFalse(condition);
        }
                
        public static void True(bool condition)
        {
            N.Assert.IsTrue(condition);
        }

        public static void Equal<T>(T lhs, T rhs)
        {
            N.Assert.AreEqual(lhs, rhs);
        }

        public static void NotEqual<T>(T lhs, T rhs)
        {
            N.Assert.AreNotEqual(lhs, rhs);
        }

        public static void Throws<T>(Action block) where T : Exception
        {
            bool didntThrow = true;

            try {
                block();
            } catch (T ex) {
                didntThrow = false;
            }

            Assert.False(didntThrow);
        }

        public static void Null(object o)
        {
            N.Assert.Null(o);
        }
        
        public static void NotNull(object o)
        {
            N.Assert.NotNull(o);
        }

        public static void Contains(string needle, string haystack)
        {
            N.Assert.IsTrue(haystack.Contains(needle));
        }

        public static void Empty<T>(IEnumerable<T> coll)
        {
            N.Assert.False(coll.Any());
        }
    }
}