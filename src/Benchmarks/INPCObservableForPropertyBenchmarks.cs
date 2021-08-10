// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks
{
    /// <summary>
    /// Bench marks checking the performance of the INotifyPropertyChanged related classes.
    /// </summary>
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class INPCObservableForPropertyBenchmarks
    {
        private readonly Expression<Func<TestClassChanged, string?>> _expr = x => x.Property1;
        private readonly INPCObservableForProperty _instance = new INPCObservableForProperty();
        private Expression? _exp;
        private string? _propertyName;

        /// <summary>
        /// Setup the benchmarks. This will be run once per set of benchmarks.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _exp = Reflection.Rewrite(_expr.Body);
            _propertyName = _exp!.GetMemberInfo()?.Name;
        }

        /// <summary>
        /// Check the performance of the property binding system.
        /// </summary>
        [Benchmark]
        public void PropertyBinding()
        {
            var testClass = new TestClassChanged();

            var changes = new List<IObservedChange<object?, object?>>();
            var dispose = _instance.GetNotificationForProperty(testClass, _exp!, _propertyName!, false).Subscribe(c => changes.Add(c));
            dispose.Dispose();
        }

        private class TestClassChanged : INotifyPropertyChanged
        {
            private string? _property;

            private string? _property2;

            public event PropertyChangedEventHandler? PropertyChanged;

            public string? Property1
            {
                get => _property;
                set
                {
                    _property = value;
                    OnPropertyChanged();
                }
            }

            public string? Property2
            {
                get => _property2;
                set
                {
                    _property2 = value;
                    OnPropertyChanged();
                }
            }

            public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
