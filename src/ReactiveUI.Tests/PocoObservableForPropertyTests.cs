using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    public class PocoObservableForPropertyTests
    {
        [Fact]
        public void CheckGetAffinityForObjectValues()
        {
            var instance = new POCOObservableForProperty();

            Assert.Equal(1, instance.GetAffinityForObject(typeof(PocoType), null, false));
            Assert.Equal(1, instance.GetAffinityForObject(typeof(INPCClass), null, false));
        }

        [Fact]
        public void NotificationPocoErrorOnBind()
        {
            var instance = new POCOObservableForProperty();

            var testLogger = new TestLogger();
            Locator.CurrentMutable.RegisterConstant<ILogger>(testLogger);

            var testClass = new PocoType();

            Expression<Func<PocoType, string>> expr = x => x.Property1;
            var exp = Reflection.Rewrite(expr.Body);

            instance.GetNotificationForProperty(testClass, exp, exp.GetMemberInfo().Name, false).Subscribe(_ => { });

            Assert.True(testLogger.LastMessages.Count > 0);
            Assert.Equal(testLogger.LastMessages[0], $"{nameof(POCOObservableForProperty)}: The class {typeof(PocoType).FullName} property {nameof(PocoType.Property1)} is a POCO type and won't send change notifications, WhenAny will only return a single value!");
        }

        private class PocoType
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }
        }

        private class INPCClass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
        }

        private class TestLogger : ILogger
        {
            public List<string> LastMessages { get; } = new List<string>();

            public LogLevel Level { get; set; }

            public void Write(string message, LogLevel logLevel)
            {
                LastMessages.Add(message);
            }
        }
    }
}
