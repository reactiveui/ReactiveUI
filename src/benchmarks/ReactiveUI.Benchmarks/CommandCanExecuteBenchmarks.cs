// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks the command <c>CanExecute</c> pipeline on the immediate scheduler: a <c>WhenAnyValue</c>-driven
/// canExecute observable that re-evaluates and broadcasts as the source property changes.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class CommandCanExecuteBenchmarks
{
    /// <summary>The number of source-property changes pushed per benchmark invocation.</summary>
    private const int ToggleCount = 10_000;

    /// <summary>Sink for the command's canExecute stream.</summary>
    private readonly NoopObserver<bool> _sink = new();

    /// <summary>The view model whose property drives canExecute.</summary>
    private BenchmarkViewModel _viewModel = null!;

    /// <summary>The command whose canExecute is driven by the view model.</summary>
    private ReactiveCommand<RxVoid, RxVoid> _command = null!;

    /// <summary>The standing canExecute subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates a command whose canExecute is a <c>WhenAnyValue</c> projection, with a standing subscription.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new();
        _command = ReactiveCommand.Create(
            static () => { },
            _viewModel.WhenAnyValue(x => x.Count, static count => (count & 1) == 0),
            Sequencer.Immediate);
        _subscription = _command.CanExecute.Subscribe(_sink);
    }

    /// <summary>Disposes the subscription and the command.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _subscription.Dispose();
        _command.Dispose();
    }

    /// <summary>Measures canExecute re-evaluation + broadcast as the source property changes.</summary>
    [Benchmark]
    public void ToggleCanExecute()
    {
        for (var i = 0; i < ToggleCount; i++)
        {
            _viewModel.Count = i;
        }
    }
}
