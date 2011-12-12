using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ReactiveUI.Tests;

namespace PerfConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            // HACK: Make sure the test libraries are loaded
            var foo = typeof (ReactiveUI.Tests.EnumerableTestMixin);
            var foo2 = "";
            var dontoptimizeme = foo.ToString() + foo2.ToString();

            // Dig through to find all test classes
            var classes =
               (from a in AppDomain.CurrentDomain.GetAssemblies()
                from m in a.GetModules(false)
                from t in m.SafeGetTypes()
                where t.GetMethods().Any(x => x.GetCustomAttributes(typeof (FactAttribute), true).Length > 0)
                select Activator.CreateInstance(t)).ToArray();

            var actions = classes.SelectMany(klass => {
                var emptyArr = new object[0];
                return klass.GetType().GetMethods()
                    .Where(x => x.GetCustomAttributes(typeof(FactAttribute), true).Length > 0)
                    .Select(x => new Action(() => x.Invoke(klass, emptyArr)));
            }).ToArray();

            Console.WriteLine("Preparing to run {0} tests from {1} classes", actions.Length, classes.Length);
            Console.WriteLine("Unpause the Perf / Memory profiler now, and hit Enter");
            Console.ReadLine();

            foreach(var v in actions) {
                try {
                    v();
                } catch(Exception ex) {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("*** Made it to the end! ***");
        }
    }
}
