// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows;
using Android;
using Android.App;
using Android.Widget;
using Android.Content;
using Xunit;
using ReactiveUI;

namespace ReactiveUI.Tests
{
    public class PropertyBindView : ReactiveFragment<PropertyBindViewModel>
    {
        public PropertyBindView()
        {
            SomeTextBox = new TextView(Application.Context);
            Property2 = new TextView(Application.Context);
            FakeControl = new PropertyBindFakeControl();
        }

        public TextView SomeTextBox { get; set; }

        public TextView Property2 { get; set; }

        public PropertyBindFakeControl FakeControl { get; set; }
    }

    public class PropertyBindFakeControl : ReactiveFragment, INotifyPropertyChanged
    {
        private double? _NullableDouble;
        private double _JustADouble;
        private string _NullHatingString = string.Empty;

        public double? NullableDouble
        {
            get { return _NullableDouble; }
            set { this.RaiseAndSetIfChanged(ref _NullableDouble, value); }
        }

        public double JustADouble
        {
            get { return _JustADouble; }
            set { this.RaiseAndSetIfChanged(ref _JustADouble, value); }
        }

        public string NullHatingString
        {
            get
            {
                return _NullHatingString;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.RaiseAndSetIfChanged(ref _NullHatingString, value);
            }
        }
    }
}
