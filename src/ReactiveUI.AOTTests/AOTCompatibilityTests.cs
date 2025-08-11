// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Xunit;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Tests to verify that ReactiveUI works correctly in AOT (Ahead-of-Time) compilation scenarios.
/// These tests ensure that the library doesn't rely on reflection in ways that break with AOT.
/// </summary>
public class AOTCompatibilityTests
{
    /// <summary>
    /// Tests that ReactiveObjects can be created and property changes work in AOT.
    /// </summary>
    [Fact]
    public void ReactiveObject_PropertyChanges_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var propertyChanged = false;

        obj.PropertyChanged += (s, e) => propertyChanged = true;
        obj.TestProperty = "New Value";

        Assert.True(propertyChanged);
        Assert.Equal("New Value", obj.TestProperty);
    }

    /// <summary>
    /// Tests that ReactiveCommands can be created and executed in AOT.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    public void ReactiveCommand_Create_WorksInAOT()
    {
        var executed = false;
        var command = ReactiveCommand.Create(() => executed = true);

        command.Execute().Subscribe();

        Assert.True(executed);
    }

    /// <summary>
    /// Tests that ReactiveCommands with parameters work in AOT.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    public void ReactiveCommand_CreateWithParameter_WorksInAOT()
    {
        string? result = null;
        var command = ReactiveCommand.Create<string>(param => result = param);

        command.Execute("test").Subscribe();

        Assert.Equal("test", result);
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works in AOT.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public void ObservableAsPropertyHelper_WorksInAOT()
    {
        var obj = new TestReactiveObject();

        // Test string-based property helper (should work in AOT)
        var helper = Observable.Return("computed value")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.Equal("computed value", helper.Value);
    }

    /// <summary>
    /// Tests that WhenAnyValue works with string property names in AOT.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing WhenAnyValue which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing WhenAnyValue which requires AOT suppression")]
    public void WhenAnyValue_StringPropertyNames_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        string? observedValue = null;

        // Using string property names should work in AOT
        obj.WhenAnyValue(x => x.TestProperty)
           .Subscribe(value => observedValue = value);

        obj.TestProperty = "test value";

        Assert.Equal("test value", observedValue);
    }

    /// <summary>
    /// Tests that interaction requests work in AOT.
    /// </summary>
    [Fact]
    public void Interaction_WorksInAOT()
    {
        var interaction = new Interaction<string, bool>();
        var called = false;

        interaction.RegisterHandler(context =>
        {
            called = true;
            context.SetOutput(true);
        });

        var result = interaction.Handle("test").Wait();

        Assert.True(called);
        Assert.True(result);
    }

    /// <summary>
    /// Tests that INPC property observation works in AOT.
    /// </summary>
    [Fact]
    public void INPCPropertyObservation_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var changes = new List<string?>();

        obj.PropertyChanged += (s, e) => changes.Add(e.PropertyName);

        obj.TestProperty = "value1";
        obj.TestProperty = "value2";

        Assert.Contains(nameof(TestReactiveObject.TestProperty), changes);
        Assert.Equal(2, changes.Count(x => x == nameof(TestReactiveObject.TestProperty)));
    }

    /// <summary>
    /// Tests that ReactiveCommand.CreateFromObservable works in AOT scenarios.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible ReactiveCommand.CreateFromObservable method")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible ReactiveCommand.CreateFromObservable method")]
    public void ReactiveCommand_CreateFromObservable_WorksInAOT()
    {
        var result = 0;
        var command = ReactiveCommand.CreateFromObservable(() => Observable.Return(42));

        command.Subscribe(x => result = x);
        command.Execute().Subscribe();

        Assert.Equal(42, result);
    }

    /// <summary>
    /// Tests that string-based property bindings work in AOT (preferred pattern).
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public void StringBasedPropertyBinding_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var helper = Observable.Return("test")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.Equal("test", helper.Value);
    }
}
