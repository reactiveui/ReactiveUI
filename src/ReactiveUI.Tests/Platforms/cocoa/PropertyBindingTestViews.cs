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
using UIKit;
using ReactiveUI.Cocoa;
using Xunit;

namespace ReactiveUI.Tests
{
    public class PropertyBindView : ReactiveViewController, IViewFor<PropertyBindViewModel>
    {
        PropertyBindViewModel _ViewModel;
        public PropertyBindViewModel ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }

        object IViewFor.ViewModel { 
            get { return ViewModel; }
            set { ViewModel = (PropertyBindViewModel)value; } 
        }
        
        public UITextView SomeTextBox;
        public UITextView Property2;
        public PropertyBindFakeControl FakeControl;

        public PropertyBindView()
        {
            SomeTextBox = new UITextView();
            Property2 = new UITextView();
            FakeControl = new PropertyBindFakeControl();
        }
    }

    public class PropertyBindFakeControl : ReactiveView
    {
        double? _NullableDouble;
        public double? NullableDouble {
            get { return _NullableDouble; }
            set { this.RaiseAndSetIfChanged(ref _NullableDouble, value); }
        }

        double _JustADouble;
        public double JustADouble {
            get { return _JustADouble; }
            set { this.RaiseAndSetIfChanged(ref _JustADouble, value); }
        }

        string _NullHatingString = "";
        public string NullHatingString {
            get { return _NullHatingString; }
            set {
                if (value is null) throw new ArgumentNullException("No nulls! I get confused!");
                this.RaiseAndSetIfChanged(ref _NullHatingString, value);
            }
        }
    }
}
