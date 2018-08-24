using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace ReactiveUI.Benchmarks
{
    [ClrJob]
    [CoreJob]
    [MonoJob]
    [RPlotExporter]
    [RankColumn]
    [MarkdownExporterAttribute.GitHub]
    public class INPCObservableForPropertyBenchmarks
    {
        private Expression exp;
        private readonly Expression<Func<TestClassChanged, string>> expr = x => x.Property1;
        private readonly INPCObservableForProperty instance = new INPCObservableForProperty();
        private string propertyName;


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

        private class TestClassChanged : INotifyPropertyChanged
        {
            private string property;

            private string property2;

            public string Property1
            {
                get => property;
                set
                {
                    property = value;
                    OnPropertyChanged();
                }
            }

            public string Property2
            {
                get => property2;
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
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
