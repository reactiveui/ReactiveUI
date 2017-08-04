// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using System.Windows.Controls;
using Xunit;

namespace ReactiveUI.Tests
{
    public class WeakEventManagerTest
    {
        [Fact(Skip = "you can blame @shiftkey")]
        public void ButtonDoesNotLeakTest()
        {
            Button button = new Button();
            ReactiveCommand command = ReactiveCommand.Create(() => { });
            button.Command = command;

            WeakReference buttonRef = new WeakReference(button);
            GC.Collect();

            Assert.True(buttonRef.IsAlive);

            //remove ref
            button = null;
            GC.Collect();

            Assert.False(buttonRef.IsAlive);
        }

        [Fact(Skip="you can blame @shiftkey")]
        public void ListBoxDoesNotLeakTest()
        {
            ListBox listBox = new ListBox();
            ReactiveList<object> list = new ReactiveList<object>();
            listBox.ItemsSource = list;

            WeakReference listBoxRef = new WeakReference(listBox);
            GC.Collect();

            Assert.True(listBoxRef.IsAlive);

            //remove ref
            listBox = null;
            GC.Collect();

            Assert.False(listBoxRef.IsAlive);
        }

        [Fact(Skip = "you can blame @shiftkey")]
        public void DataContextDoesNotLeakTest()
        {
            ListBox listBox = new ListBox();
            TestFixture vm = new TestFixture();
            listBox.DataContext = vm;

            WeakReference listBoxRef = new WeakReference(listBox);
            GC.Collect();

            Assert.True(listBoxRef.IsAlive);

            //remove ref
            listBox = null;
            GC.Collect();

            Assert.False(listBoxRef.IsAlive);
        }
    }
}
