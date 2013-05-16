using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    public class TestLogger: IRxUILogger
    {
        public List<Tuple<string, LogLevel>> Messages { get; private set; }
        public LogLevel Level { get; set; }

        public TestLogger()
        {
            Messages = new List<Tuple<string, LogLevel>>();
            Level = LogLevel.Debug;
        }

        public void Write(string message, LogLevel logLevel)
        {
            this.Messages.Add(Tuple.Create(message, logLevel));
        }

        public void With(Action<TestLogger> action)
        {
            var originalFactory = RxApp.LoggerFactory;
            RxApp.LoggerFactory = x => this;

            try
            {
                action(this);
            }
            finally
            {
                RxApp.LoggerFactory = originalFactory;
            }
        }
    }
}
