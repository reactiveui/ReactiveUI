// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Winforms
{
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

        public FakeWinformViewModel(IScreen? screen = null) => HostScreen = screen ?? new TestScreen();

        public string UrlPathSegment => "fake";

        public IScreen HostScreen { get; }

        public int SomeInteger
        {
            get => _someInteger;
            set => this.RaiseAndSetIfChanged(ref _someInteger, value);
        }

        public string? SomeText
        {
            get => _someText;
            set => this.RaiseAndSetIfChanged(ref _someText, value);
        }

        public double SomeDouble
        {
            get => _someDouble;
            set => this.RaiseAndSetIfChanged(ref _someDouble, value);
        }

        public string? Property1
        {
            get => _property1;
            set => this.RaiseAndSetIfChanged(ref _property1, value);
        }

        public string? Property2
        {
            get => _property2;
            set => this.RaiseAndSetIfChanged(ref _property2, value);
        }

        public string? Property3
        {
            get => _property3;
            set => this.RaiseAndSetIfChanged(ref _property3, value);
        }

        public string? Property4
        {
            get => _property4;
            set => this.RaiseAndSetIfChanged(ref _property4, value);
        }

        public bool BooleanProperty
        {
            get => _someBooleanProperty;
            set => this.RaiseAndSetIfChanged(ref _someBooleanProperty, value);
        }
    }
}
