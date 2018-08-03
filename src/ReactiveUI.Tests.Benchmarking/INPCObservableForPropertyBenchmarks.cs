using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ReactiveUI.Tests.Benchmarking
{
    [ClrJob, CoreJob, MonoJob, CoreRtJob]
    [RPlotExporter, RankColumn]
    public class INPCObservableForPropertyBenchmarks
    {
        private string propertyName;
        private Expression<Func<TestClassChanged, string>> expr = x => x.Property1;
        private Expression exp;
        private INPCObservableForProperty instance = new INPCObservableForProperty();


        [GlobalSetup]
        public void Setup()
        {
            exp = Reflection.Rewrite(expr.Body);
            propertyName = exp.GetMemberInfo().Name;
        }

        [Benchmark]
        public void PropertyBinding()
        {
            var testClass = new TestClassChanged();

            var changes = new List<IObservedChange<object, object>>();
            instance.GetNotificationForProperty(testClass, exp, propertyName, false).Subscribe(c => changes.Add(c));
        }

        class TestClassChanged : INotifyPropertyChanged
        {
            string property;

            string property2;

            public string Property1
            {
                get { return property; }
                set
                {
                    property = value;
                    OnPropertyChanged();
                }
            }

            public string Property2
            {
                get { return property2; }
                set
                {
                    property2 = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}