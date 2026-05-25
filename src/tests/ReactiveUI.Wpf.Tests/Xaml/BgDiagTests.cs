// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using ReactiveUI.Tests.Xaml.Utilities;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>Temporary diagnostic for the background-thread WPF binding failure.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class BgDiagTests
{
    /// <summary>Diagnostic.</summary>
    /// <returns>A task.</returns>
    [Test]
    public async Task BgDiag()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        using var binding = view.Bind(view.ViewModel, static x => x.Property1, static x => x.SomeTextBox.Text);

        var emissions = new System.Collections.Generic.List<string>();
        using var sub = binding.Changed.Subscribe(c => emissions.Add($"view='{c.view}' isVm={c.isViewModel} tid={System.Environment.CurrentManagedThreadId}"));

        var setupTid = System.Environment.CurrentManagedThreadId;
        var bgTid = 0;
        Exception? thrown = null;
        await Task.Run(() =>
        {
            bgTid = System.Environment.CurrentManagedThreadId;
            try
            {
                vm.Property1 = "background update";
            }
            catch (Exception ex)
            {
                thrown = ex;
            }
        });

        DispatcherUtilities.DoEvents();

        var diag = $"changed={emissions.Count} text='{view.SomeTextBox.Text}' vmProp='{vm.Property1}' "
                   + $"thrown={thrown?.GetType().Name ?? "null"} tids(setup={setupTid},bg={bgTid},pump={System.Environment.CurrentManagedThreadId}) "
                   + $"[{string.Join(";", emissions)}]";

        // Surface the diagnostic by asserting it (the failure message shows the actual state).
        await Assert.That(diag).IsEqualTo("SHOW_DIAG");
    }
}
