// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Xunit;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
#else
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using FactAttribute = Xunit.WpfFactAttribute;
#endif

namespace ReactiveUI.Tests.Xaml
{
    /// <summary>
    /// Tests for XAML and commands.
    /// </summary>
    public class XamlViewCommandTests
    {
        /// <summary>
        /// Test that event binder binds to explicit inherited event.
        /// </summary>
        [Fact]
        public void EventBinderBindsToExplicitInheritedEvent()
        {
            var fixture = new FakeView();
            fixture.BindCommand(fixture!.ViewModel, x => x!.Cmd, x => x.TheTextBox, "MouseDown");
        }

        /// <summary>
        /// Test that event binder binds to implicit event.
        /// </summary>
        [Fact]
        public void EventBinderBindsToImplicitEvent()
        {
            var input = new Button();
            var fixture = new CreatesCommandBindingViaEvent();
            var cmd = ReactiveCommand.Create<int>(_ => { });

            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            var invokeCount = 0;
            cmd.Subscribe(_ => ++invokeCount);

            var disp = fixture.BindCommandToObject(cmd, input, Observable.Return((object)5));
            Assert.NotNull(disp);
            Assert.Equal(0, invokeCount);

            var automationPeer = new ButtonAutomationPeer(input);
            var invoker = (IInvokeProvider)automationPeer.GetPattern(PatternInterface.Invoke);

            invoker.Invoke();
            DispatcherUtilities.DoEvents();
            Assert.Equal(1, invokeCount);

            disp?.Dispose();
            invoker.Invoke();
            Assert.Equal(1, invokeCount);
        }
    }
}
