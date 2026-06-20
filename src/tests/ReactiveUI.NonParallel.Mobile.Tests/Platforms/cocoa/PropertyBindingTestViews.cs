// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using UIKit;
using ReactiveUI.Cocoa;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A test view.
    /// </summary>
    public class PropertyBindView : ReactiveViewController, IViewFor<PropertyBindViewModel>
    {
        /// <summary>
        /// The backing field for the view model.
        /// </summary>
        PropertyBindViewModel _ViewModel;

        /// <summary>
        /// Some text box.
        /// </summary>
        public UITextView SomeTextBox;

        /// <summary>
        /// The second property text box.
        /// </summary>
        public UITextView Property2;

        /// <summary>
        /// The fake control.
        /// </summary>
        public PropertyBindFakeControl FakeControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBindView"/> class.
        /// </summary>
        public PropertyBindView()
        {
            SomeTextBox = new UITextView();
            Property2 = new UITextView();
            FakeControl = new PropertyBindFakeControl();
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public PropertyBindViewModel ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }

        /// <inheritdoc/>
        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (PropertyBindViewModel)value; }
        }
    }

    /// <summary>
    /// A fake control.
    /// </summary>
    public class PropertyBindFakeControl : ReactiveView
    {
        /// <summary>
        /// The backing field for the nullable double.
        /// </summary>
        double? _NullableDouble;

        /// <summary>
        /// The backing field for the just a double value.
        /// </summary>
        double _JustADouble;

        /// <summary>
        /// The backing field for the null hating string.
        /// </summary>
        string _NullHatingString = "";

        /// <summary>
        /// Gets or sets the nullable double.
        /// </summary>
        public double? NullableDouble {
            get { return _NullableDouble; }
            set { this.RaiseAndSetIfChanged(ref _NullableDouble, value); }
        }

        /// <summary>
        /// Gets or sets the just a double value.
        /// </summary>
        public double JustADouble {
            get { return _JustADouble; }
            set { this.RaiseAndSetIfChanged(ref _JustADouble, value); }
        }

        /// <summary>
        /// Gets or sets the null hating string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        public string NullHatingString {
            get { return _NullHatingString; }
            set {
                if (value is null) throw new ArgumentNullException("No nulls! I get confused!");
                this.RaiseAndSetIfChanged(ref _NullHatingString, value);
            }
        }
    }
}
