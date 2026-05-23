// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Executors;

namespace ReactiveUI.AOT.Tests;

/// <summary>
/// Observable string based observation tests.
/// </summary>
[TestExecutor<AppBuilderTestExecutor>]
public class StringBasedObservationTests
{
    /// <summary>
    /// The initial integer value assigned before observation begins.
    /// </summary>
    private const int InitialIntValue = 5;

    /// <summary>
    /// The first changed integer value, repeated to verify distinct filtering.
    /// </summary>
    private const int FirstChangedIntValue = 7;

    /// <summary>
    /// The second changed integer value.
    /// </summary>
    private const int SecondChangedIntValue = 9;

    /// <summary>
    /// The first integer value used by the before-change test.
    /// </summary>
    private const int BeforeChangeFirstValue = 2;

    /// <summary>
    /// The second integer value used by the before-change test.
    /// </summary>
    private const int BeforeChangeSecondValue = 3;

    /// <summary>
    /// Observables for property string name emits initial then changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task ObservableForProperty_StringName_EmitsInitialThenChanges()
    {
        var s = new Sample { IntValue = InitialIntValue };
        var values = new List<int>();

        s.ObservableForProperty<Sample, int>(nameof(Sample.IntValue), false, false, true)
            .Select(static x => x.Value)
            .Subscribe(values.Add);

        s.IntValue = FirstChangedIntValue;
        s.IntValue = FirstChangedIntValue; // distinct should suppress duplicate
        s.IntValue = SecondChangedIntValue;

        await Assert.That(values).IsEquivalentTo([InitialIntValue, FirstChangedIntValue, SecondChangedIntValue]);
    }

    /// <summary>
    /// Observables for property before change emits before setter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task ObservableForProperty_BeforeChange_EmitsBeforeSetter()
    {
        var s = new Sample { IntValue = 1 };
        var before = new List<int>();

        s.ObservableForProperty<Sample, int>(nameof(Sample.IntValue), true, true, false)
            .Select(static x => x.Value)
            .Subscribe(before.Add);

        s.IntValue = BeforeChangeFirstValue; // should emit previous value (1) before change
        s.IntValue = BeforeChangeSecondValue; // should emit 2

        await Assert.That(before).IsEquivalentTo([1, BeforeChangeFirstValue]);
    }

    /// <summary>
    /// Whens any value string name works and is distinct.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task WhenAnyValue_StringName_WorksAndIsDistinct()
    {
        var s = new Sample { Name = "a" };
        var values = new List<string>();

        s.WhenAnyValue<Sample, string?>(nameof(Sample.Name))
            .Subscribe(v => values.Add(v!));

        s.Name = "b";
        s.Name = "b"; // duplicate should be filtered by default overload
        s.Name = "c";

        await Assert.That(values).IsEquivalentTo(["a", "b", "c"]);
    }

    /// <summary>
    /// Whens any value string name not distinct when requested.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task WhenAnyValue_StringName_NotDistinctWhenRequested()
    {
        var s = new Sample { Name = "x" };
        var values = new List<string>();

        s.WhenAnyValue<Sample, string?>(nameof(Sample.Name), false)
            .Subscribe(v => values.Add(v!));

        s.Name = "y";
        s.Name = "y"; // should be included

        await Assert.That(values).IsEquivalentTo(["x", "y", "y"]);
    }

    /// <summary>
    /// Sample reactive object used to exercise string-based property observation.
    /// </summary>
    private sealed class Sample : ReactiveObject
    {
        /// <summary>
        /// Backing field for <see cref="IntValue"/>.
        /// </summary>
        private int _intValue;

        /// <summary>
        /// Backing field for <see cref="Name"/>.
        /// </summary>
        private string? _name;

        /// <summary>
        /// Gets or sets the integer value.
        /// </summary>
        public int IntValue
        {
            get => _intValue;
            set => this.RaiseAndSetIfChanged(ref _intValue, value);
        }

        /// <summary>
        /// Gets or sets the name value.
        /// </summary>
        public string? Name
        {
            get => _name;
            set
            {
                // Using RaisePropertyChanged to ensure property change notification
                _name = value;
                this.RaisePropertyChanged(nameof(Name));
            }
        }
    }
}
