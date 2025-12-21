// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings;

[TestFixture]
public class BindToNestedPropertyTests
{
    [Test]
    public void BindToSetsNestedPropertyOncePerValueOnSameHost()
    {
        TrackingNestedValue.SetCallCount = 0;
        TextTrackingView.SetCallCount = 0;
        var view = new TextTrackingView();

        using var subscription1 = view.WhenAnyValue(x => x.ViewModel!.Nested.SomeText).BindTo(view, static x => x.TextField);
        using var subscription2 = view.WhenAnyValue(x => x.TextField).BindTo(view, static x => x.ViewModel!.Nested.SomeText);

        view.ViewModel.Nested = new() { SomeText = "Alpha" };
        view.ViewModel.Nested = new() { SomeText = "Alpha" };
        view.ViewModel.Nested = new() { SomeText = "Alpha" };
        view.ViewModel.Nested = new() { SomeText = "Beta" };
        view.ViewModel.Nested = new() { SomeText = "Beta" };
        view.ViewModel.Nested = new() { SomeText = "Beta" };
        view.ViewModel.Nested = new() { SomeText = "Gamma" };
        view.ViewModel.Nested = new() { SomeText = "Gamma" };
        view.ViewModel.Nested = new() { SomeText = "Gamma" };

        var nested = view.ViewModel!.Nested;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TrackingNestedValue.SetCallCount, Is.EqualTo(9));
            Assert.That(TextTrackingView.SetCallCount, Is.EqualTo(3));
            Assert.That(nested.SomeText, Is.EqualTo("Gamma"));
        }
    }

    [Test]
    public void BindToSetsNestedPropertyOncePerValueAfterHostReplacement()
    {
        TrackingNestedValue.SetCallCount = 0;
        TextTrackingView.SetCallCount = 0;
        var view = new TextTrackingView();

        using var source = new Subject<string>();
        using var subscription = source.BindTo(view, static x => x.ViewModel.Nested.SomeText);

        var values = new[] { "Delta", "Epsilon", "Zeta" };
        var count = 0;
        foreach (var value in values)
        {
            var replacement = new TrackingNestedValue();
            view.ViewModel.Nested = replacement;

            source.OnNext(value);
            count++;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(TrackingNestedValue.SetCallCount, Is.EqualTo(count), $"Value '{value}' invoked the setter more than once.");
                Assert.That(replacement.SomeText, Is.EqualTo(value));
            }
        }
    }

    [Test]
    public void BindToFromViewPropertyKeepsSingleSetterAfterHostReplacement()
    {
        TrackingNestedValue.SetCallCount = 0;
        TextTrackingView.SetCallCount = 0;
        var view = new TextTrackingView { TextField = "Initial" }; // 1

        using var subscription = view.WhenAnyValue(x => x.TextField)
            .BindTo(view, static x => x.ViewModel!.Nested.SomeText);

        var first = view.ViewModel.Nested;

        view.TextField = "First"; // 2

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TrackingNestedValue.SetCallCount, Is.EqualTo(2));
            Assert.That(first.SomeText, Is.EqualTo("First"));
        }

        var replacement = new TrackingNestedValue();
        view.ViewModel.Nested = replacement;

        view.TextField = "Second"; // 3

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TrackingNestedValue.SetCallCount, Is.EqualTo(3), "Setter invoked multiple times after host swap.");
            Assert.That(replacement.SomeText, Is.EqualTo("Second"));
        }
    }

    private sealed class TrackingHostViewModel : ReactiveObject
    {
        public TrackingHostViewModel() => Nested = new();

        public TrackingNestedValue Nested
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    private sealed class TrackingNestedValue : ReactiveObject
    {
        public static int SetCallCount { get; set; }

        public string? SomeText
        {
            get => field;
            set
            {
                if (value != field)
                {
                    this.RaisePropertyChanging();
                    field = value;
                    this.RaisePropertyChanged();
                    SetCallCount++;
                }
            }
        }
    }

    private sealed class TextTrackingView : ReactiveObject
    {
        public static int SetCallCount { get; set; }

        public TrackingHostViewModel ViewModel { get; set; } = new();

        public string? TextField
        {
            get => field;
            set
            {
                if (value != field)
                {
                    this.RaisePropertyChanging();
                    field = value;
                    this.RaisePropertyChanged();
                    SetCallCount++;
                }
            }
        }
    }
}
