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

        public static void Throws<T>(Action block) where T : Exception
        {
            N.Assert.Throws<T>(() => block());
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
       
        /*
        public static void Contains<T>(IEnumerable<T> list, T item)
        {
            N.Assert.True(list.Any(x => x.Equals(item)));
        }
         */
    }
}