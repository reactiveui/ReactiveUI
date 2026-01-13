// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using ReactiveUI.Tests.Xaml.Utilities;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for XAML and commands.
/// </summary>
[NotInParallel]
public class XamlViewCommandTests
{
    private WpfAppBuilderScope? _appBuilderScope;

    /// <summary>
    /// Sets up the WPF app builder scope for each test.
    /// </summary>
    [Before(Test)]
    public void Setup()
    {
        _appBuilderScope = new WpfAppBuilderScope();
    }

    /// <summary>
    /// Tears down the WPF app builder scope after each test.
    /// </summary>
    [After(Test)]
    public void TearDown()
    {
        _appBuilderScope?.Dispose();
    }

    /// <summary>
    /// Test that event binder binds to explicit inherited event.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void EventBinderBindsToExplicitInheritedEvent()
    {
        var fixture = new FakeView();
        fixture.BindCommand(fixture!.ViewModel, static x => x!.Cmd, static x => x.TheTextBox, "MouseDown");
    }

    /// <summary>
    /// Test that event binder binds to implicit event.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task EventBinderBindsToImplicitEvent()
    {
        var input = new Button();
        var fixture = new CreatesCommandBindingViaEvent();
        var cmd = ReactiveCommand.Create<int>(_ => { }, outputScheduler: ImmediateScheduler.Instance);

        await Assert.That(fixture.GetAffinityForObject<Button>(false)).IsGreaterThan(0);

        var invokeCount = 0;
        cmd.Subscribe(_ => ++invokeCount);

        var disp = fixture.BindCommandToObject(cmd, input, Observable.Return((object)5));
        using (Assert.Multiple())
        {
            await Assert.That(disp).IsNotNull();
            await Assert.That(invokeCount).IsEqualTo(0);
        }

        var automationPeer = new ButtonAutomationPeer(input);
        var invoker = (IInvokeProvider)automationPeer.GetPattern(PatternInterface.Invoke);

        invoker.Invoke();
        DispatcherUtilities.DoEvents();
        await Assert.That(invokeCount).IsEqualTo(1);

        disp?.Dispose();
        invoker.Invoke();
        await Assert.That(invokeCount).IsEqualTo(1);
    }
}
