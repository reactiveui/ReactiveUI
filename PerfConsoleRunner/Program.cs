using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerfConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Dig through to find all test classes
            var classes =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from m in a.GetModules(false)
                from t in m.SafeGetTypes()
                where t.GetCustomAttributes(true).Any(x => x.GetType().FullName.Contains("TestClass")) 
                select Activator.CreateInstance(t);

            var actions = classes.SelectMany(klass => {
                var emptyArr = new object[0];
                return klass.GetType().GetMethods()
                    .Where(x => x.GetCustomAttributes(true).Any(y => y.GetType().FullName.Contains("TestMethod")))
                    .Select(x => new Action(() => x.Invoke(klass, emptyArr)));
            }).ToArray();

            Console.WriteLine("Preparing to run {0} tests", actions.Length);
            Console.WriteLine("Unpause the Perf / Memory profiler now, and hit Enter");
            Console.ReadLine();

            foreach(var v in actions) {
                try {
                    v();
                } catch(Exception ex) {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
