using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI.Xaml;
using Xunit;

namespace ReactiveUI.Tests
{
    public class CommandBindingTests
    {
        [Fact]
        public void CommandBinderBindsToButton()
        {
            var fixture = new CreatesCommandBindingViaCommandParameter();
            var origCmd = new RoutedCommand();
            var cmd = new ReactiveCommand();
            var input = new Button { Command = origCmd, CommandParameter = 42 };

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) <= 0);
            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            var disp = fixture.BindCommandToObject(cmd, input, Observable.Return((object)5));

            Assert.Equal(cmd, input.Command);
            Assert.Equal(5, input.CommandParameter);

            disp.Dispose();

            Assert.Equal(origCmd, input.Command);
            Assert.Equal(42, input.CommandParameter);
        }

        [Fact]
        public void EventBinderBindsToExplicitEvent()
        {
            var input = new NonReactiveINPCObjectMadeReactive();
            var fixture = new CreatesCommandBindingViaEvent();
            var cmd = new ReactiveCommand();

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
            Assert.False(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            bool wasCalled = false;
            cmd.Subscribe(_ => wasCalled = true);

            var disp = fixture.BindCommandToObject<PropertyChangedEventArgs>(cmd, input, Observable.Return((object) 5), "PropertyChanged");
            input.InpcProperty = new TestFixture();
            Assert.True(wasCalled);

            wasCalled = false;
            disp.Dispose();
            input.InpcProperty = new TestFixture();
            Assert.False(wasCalled);
        }

        [Fact]
        public void EventBinderBindsToImplicitEvent()
        {
            var input = new Button();
            var fixture = new CreatesCommandBindingViaEvent();
            var cmd = new ReactiveCommand();

            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            int invokeCount = 0;
            cmd.Subscribe(_ => invokeCount += 1);

            var disp = fixture.BindCommandToObject(cmd, input, Observable.Return((object) 5));
            Assert.NotNull(disp);
            Assert.Equal(0, invokeCount);

            input.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(1, invokeCount);

            disp.Dispose();
            input.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(1, invokeCount);
        }
    }

}
