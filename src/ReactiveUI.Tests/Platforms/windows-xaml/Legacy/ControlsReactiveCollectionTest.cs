// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using FactAttribute = Xunit.WpfFactAttribute;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public class ControlsReactiveCollectionTest
    {
        [Fact]
        public void DataboundReactiveListDoesNotThrowForAddRange()
        {
            var vm = new LegacyPropertyBindViewModel();
            var view = new LegacyPropertyBindView
            {
                ViewModel = vm
            };
            var fixture = new PropertyBinderImplementation();
            fixture.OneWayBind(vm, view, m => m.SomeCollectionOfStrings, v => v.FakeItemsControl.ItemsSource);

            // eliminate the ResetChangeThreshold from the equation
            vm.SomeCollectionOfStrings.ResetChangeThreshold = int.MinValue;

            // Within the reset threshold
            vm.SomeCollectionOfStrings.AddRange(Create(5));
            vm.SomeCollectionOfStrings.AddRange(Create(20));

            IEnumerable<string> Create(int numElements) => Enumerable.Range(1, numElements).Select(i => $"item_{i}");
        }

        [Fact]
        public void DataboundReactiveListDoesNotThrowForInsertRange()
        {
            var vm = new LegacyPropertyBindViewModel();
            var view = new LegacyPropertyBindView
            {
                ViewModel = vm
            };
            var fixture = new PropertyBinderImplementation();
            fixture.OneWayBind(vm, view, m => m.SomeCollectionOfStrings, v => v.FakeItemsControl.ItemsSource);
            vm.SomeCollectionOfStrings.ResetChangeThreshold = int.MinValue;

            foreach (var item in Create(5))
            {
                vm.SomeCollectionOfStrings.Add(item);
            }

            // within reset threshold
            vm.SomeCollectionOfStrings.InsertRange(2, Create(5));

            // outside reset threshold
            vm.SomeCollectionOfStrings.InsertRange(2, Create(20));

            IEnumerable<string> Create(int numElements) => Enumerable.Range(1, numElements).Select(i => $"item_{i}");
        }

        [Fact]
        public void DataboundReactiveListDoesNotThrowForRemoveRange()
        {
            var vm = new LegacyPropertyBindViewModel();
            var view = new LegacyPropertyBindView
            {
                ViewModel = vm
            };
            var fixture = new PropertyBinderImplementation();
            fixture.OneWayBind(vm, view, m => m.SomeCollectionOfStrings, v => v.FakeItemsControl.ItemsSource);
            vm.SomeCollectionOfStrings.ResetChangeThreshold = int.MinValue;

            foreach (var item in Enumerable.Range(1, 40).Select(i => $"item_{i}"))
            {
                vm.SomeCollectionOfStrings.Add(item);
            }

            // within reset threshold
            vm.SomeCollectionOfStrings.RemoveRange(2, 5);

            // outside reset threshold
            vm.SomeCollectionOfStrings.RemoveRange(2, 20);
        }
    }
}
