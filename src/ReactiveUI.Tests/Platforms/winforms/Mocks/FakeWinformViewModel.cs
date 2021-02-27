// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Winforms
{
    /// <summary>
    /// A fake view model.
    /// </summary>
    public class FakeWinformViewModel : ReactiveObject, IRoutableViewModel
    {
        private bool _someBooleanProperty;
        private int _someInteger;
        private string? _someText;
        private double _someDouble;
        private string? _property1;
        private string? _property2;
        private string? _property3;
        private string? _property4;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeWinformViewModel"/> class.
        /// </summary>
        /// <param name="screen">The screen.</param>
        public FakeWinformViewModel(IScreen? screen = null) => HostScreen = screen ?? new TestScreen();

        /// <inheritdoc/>
        public string UrlPathSegment => "fake";

        /// <inheritdoc/>
        public IScreen HostScreen { get; }

        /// <summary>
        /// Gets or sets some integer.
        /// </summary>
        public int SomeInteger
        {
            get => _someInteger;
            set => this.RaiseAndSetIfChanged(ref _someInteger, value);
        }

        /// <summary>
        /// Gets or sets some text.
        /// </summary>
        public string? SomeText
        {
            get => _someText;
            set => this.RaiseAndSetIfChanged(ref _someText, value);
        }

        /// <summary>
        /// Gets or sets some double.
        /// </summary>
        public double SomeDouble
        {
            get => _someDouble;
            set => this.RaiseAndSetIfChanged(ref _someDouble, value);
        }

        /// <summary>
        /// Gets or sets the property1.
        /// </summary>
        public string? Property1
        {
            get => _property1;
            set => this.RaiseAndSetIfChanged(ref _property1, value);
        }

        /// <summary>
        /// Gets or sets the property2.
        /// </summary>
        public string? Property2
        {
            get => _property2;
            set => this.RaiseAndSetIfChanged(ref _property2, value);
        }

        /// <summary>
        /// Gets or sets the property3.
        /// </summary>
        public string? Property3
        {
            get => _property3;
            set => this.RaiseAndSetIfChanged(ref _property3, value);
        }

        /// <summary>
        /// Gets or sets the property4.
        /// </summary>
        public string? Property4
        {
            get => _property4;
            set => this.RaiseAndSetIfChanged(ref _property4, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [boolean property].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boolean property]; otherwise, <c>false</c>.
        /// </value>
        public bool BooleanProperty
        {
            get => _someBooleanProperty;
            set => this.RaiseAndSetIfChanged(ref _someBooleanProperty, value);
        }
    }
}
