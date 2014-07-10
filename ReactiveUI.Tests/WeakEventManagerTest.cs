using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using Xunit;

namespace ReactiveUI.Tests
{
    public class WeakEventManagerTest
    {
        [Fact]        
        public void ButtonDoesNotLeakTest()
        {
            Button button = new Button();
            ReactiveCommand<object> command = ReactiveCommand.Create();
            button.Command = command;

            WeakReference buttonRef = new WeakReference(button);
            GC.Collect();

            Assert.True(buttonRef.IsAlive);

            //remove ref
            button = null;
            GC.Collect();

            Assert.False(buttonRef.IsAlive);
        }

        [Fact]
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

        [Fact]
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
