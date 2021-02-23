// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Xunit;
using ReactiveUI.Cocoa;

namespace ReactiveUI.Tests
{
    public class FooController : ReactiveViewController, IViewFor<PropertyBindViewModel>
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
    }

    public class KVOBindingTests
    {
        [Fact]
        public void MakeSureKVOBindingsBindToKVOThings()
        {
            var input = new FooController();
            var fixture = new KVOObservableForProperty();

            Assert.NotEqual(0, fixture.GetAffinityForObject(typeof(FooController), "View"));
            Assert.Equal(0, fixture.GetAffinityForObject(typeof(FooController), "ViewModel"));
        }
    }
}
