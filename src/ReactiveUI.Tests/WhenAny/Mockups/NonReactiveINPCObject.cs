﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Tests
{
    public class NonReactiveINPCObject : INotifyPropertyChanged
    {
        private TestFixture _inpcProperty = new TestFixture();

        public event PropertyChangedEventHandler? PropertyChanged;

        public TestFixture InpcProperty
        {
            get => _inpcProperty;
            set
            {
                if (_inpcProperty == value)
                {
                    return;
                }

                _inpcProperty = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InpcProperty)));
            }
        }
    }
}
