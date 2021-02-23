// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests
{
    public class HostTestFixture : ReactiveObject
    {
        private TestFixture? _Child;

        private NonObservableTestFixture? _PocoChild;

        private int _SomeOtherParam;

        public TestFixture? Child
        {
            get => _Child;
            set => this.RaiseAndSetIfChanged(ref _Child, value);
        }

        public NonObservableTestFixture? PocoChild
        {
            get => _PocoChild;
            set => this.RaiseAndSetIfChanged(ref _PocoChild, value);
        }

        public int SomeOtherParam
        {
            get => _SomeOtherParam;
            set => this.RaiseAndSetIfChanged(ref _SomeOtherParam, value);
        }
    }
}
