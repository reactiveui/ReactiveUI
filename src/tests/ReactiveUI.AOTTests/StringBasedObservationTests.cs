// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.AOTTests;

/// <summary>
/// StringBasedObservationTests.
/// </summary>
[TestFixture]
public class StringBasedObservationTests
{
    /// <summary>
    /// Observables for property string name emits initial then changes.
    /// </summary>
    [Test]
    public void ObservableForProperty_StringName_EmitsInitialThenChanges()
    {
        var s = new Sample { IntValue = 5 };
        var values = new List<int>();

        s.ObservableForProperty<Sample, int>(nameof(Sample.IntValue), beforeChange: false, skipInitial: false, isDistinct: true)
         .Select(static x => x.Value)
         .Subscribe(values.Add);

        s.IntValue = 7;
        s.IntValue = 7; // distinct should suppress duplicate
        s.IntValue = 9;

        Assert.That(values, Is.EqualTo([5, 7, 9]));
    }

    /// <summary>
    /// Observables for property before change emits before setter.
    /// </summary>
    [Test]
    public void ObservableForProperty_BeforeChange_EmitsBeforeSetter()
    {
        var s = new Sample { IntValue = 1 };
        var before = new List<int>();

        s.ObservableForProperty<Sample, int>(nameof(Sample.IntValue), beforeChange: true, skipInitial: true, isDistinct: false)
         .Select(static x => x.Value)
         .Subscribe(before.Add);

        s.IntValue = 2; // should emit previous value (1) before change
        s.IntValue = 3; // should emit 2

        Assert.That(before, Is.EqualTo([1, 2]));
    }

    /// <summary>
    /// Whens any value string name works and is distinct.
    /// </summary>
    [Test]
    public void WhenAnyValue_StringName_WorksAndIsDistinct()
    {
        var s = new Sample { Name = "a" };
        var values = new List<string?>();

        s.WhenAnyValue<Sample, string?>(nameof(Sample.Name))
         .Subscribe(values.Add);

        s.Name = "b";
        s.Name = "b"; // duplicate should be filtered by default overload
        s.Name = "c";

        Assert.That(values, Is.EqualTo(["a", "b", "c"]));
    }

    /// <summary>
    /// Whens any value string name not distinct when requested.
    /// </summary>
    [Test]
    public void WhenAnyValue_StringName_NotDistinctWhenRequested()
    {
        var s = new Sample { Name = "x" };
        var values = new List<string?>();

        s.WhenAnyValue<Sample, string?>(nameof(Sample.Name), isDistinct: false)
         .Subscribe(values.Add);

        s.Name = "y";
        s.Name = "y"; // should be included

        Assert.That(values, Is.EqualTo(["x", "y", "y"]));
    }

    private sealed class Sample : ReactiveObject
    {
        private int _intValue;
        private string? _name;

        public int IntValue
        {
            get => _intValue;
            set => this.RaiseAndSetIfChanged(ref _intValue, value);
        }

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
