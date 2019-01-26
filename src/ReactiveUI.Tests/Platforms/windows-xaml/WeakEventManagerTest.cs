// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using DynamicData.Binding;
using Xunit;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
#else
using System.Windows.Controls;
using FactAttribute = Xunit.WpfFactAttribute;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public class WeakEventManagerTest
    {
        [Fact(Skip = "you can blame @shiftkey")]
        public void ButtonDoesNotLeakTest()
        {
            var button = new Button();
            var command = ReactiveCommand.Create(() => { });
            button.Command = command;

            var buttonRef = new WeakReference(button);
            GC.Collect();

            Assert.True(buttonRef.IsAlive);

            // remove ref
            button = null;
            GC.Collect();

            Assert.False(buttonRef.IsAlive);
        }

        [Fact(Skip="you can blame @shiftkey")]
        public void ListBoxDoesNotLeakTest()
        {
            var listBox = new ListBox();
            var list = new ObservableCollectionExtended<object>();
            listBox.ItemsSource = list;

            var listBoxRef = new WeakReference(listBox);
            GC.Collect();

            Assert.True(listBoxRef.IsAlive);

            // remove ref
            listBox = null;
            GC.Collect();

            Assert.False(listBoxRef.IsAlive);
        }

        [Fact(Skip = "you can blame @shiftkey")]
        public void DataContextDoesNotLeakTest()
        {
            var listBox = new ListBox();
            var vm = new TestFixture();
            listBox.DataContext = vm;

            var listBoxRef = new WeakReference(listBox);
            GC.Collect();

            Assert.True(listBoxRef.IsAlive);

            // remove ref
            listBox = null;
            GC.Collect();

            Assert.False(listBoxRef.IsAlive);
        }
    }
}
