// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Legacy;

#pragma warning disable CS0618 // Item has been marked as Obsolete

namespace ReactiveUI.Tests.Xaml
{
    public class LegacyPropertyBindViewModel : ReactiveObject
    {
        private decimal _JustADecimal;

        private double _JustADouble;

        private int _JustAInt32;

        private PropertyBindModel _Model;

        private double? _NullableDouble;

        private string _Property1;

        private int _Property2;

        public LegacyPropertyBindViewModel(PropertyBindModel model = null)
        {
            Model = model ?? new PropertyBindModel
            {
                AThing = 42,
                AnotherThing = "Baz"
            };
            SomeCollectionOfStrings = new ReactiveList<string>(new[] { "Foo", "Bar" });
        }

        public decimal JustADecimal
        {
            get => _JustADecimal;
            set => this.RaiseAndSetIfChanged(ref _JustADecimal, value);
        }

        public double JustADouble
        {
            get => _JustADouble;
            set => this.RaiseAndSetIfChanged(ref _JustADouble, value);
        }

        public int JustAInt32
        {
            get => _JustAInt32;
            set => this.RaiseAndSetIfChanged(ref _JustAInt32, value);
        }

        public PropertyBindModel Model
        {
            get => _Model;
            set => this.RaiseAndSetIfChanged(ref _Model, value);
        }

        public double? NullableDouble
        {
            get => _NullableDouble;
            set => this.RaiseAndSetIfChanged(ref _NullableDouble, value);
        }

        public string Property1
        {
            get => _Property1;
            set => this.RaiseAndSetIfChanged(ref _Property1, value);
        }

        public int Property2
        {
            get => _Property2;
            set => this.RaiseAndSetIfChanged(ref _Property2, value);
        }

        public ReactiveList<string> SomeCollectionOfStrings { get; }
    }
}
