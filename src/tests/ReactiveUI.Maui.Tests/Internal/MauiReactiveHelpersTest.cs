// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI.Maui.Internal;

namespace ReactiveUI.Maui.Tests.Internal;

/// <summary>
/// Tests for MauiReactiveHelpers.
/// </summary>
public class MauiReactiveHelpersTest
{
    /// <summary>
    /// Tests that CreatePropertyChangedPulse emits when the property changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_EmitsWhenPropertyChanges()
    {
        var vm = new TestViewModel();
        var changes = 0;
        using var sub = MauiReactiveHelpers.CreatePropertyChangedPulse(vm, nameof(TestViewModel.Name))
            .Subscribe(_ => changes++);

        vm.Name = "New Name";

        await Assert.That(changes).IsEqualTo(1);
    }

    /// <summary>
    /// Tests that CreatePropertyChangedPulse does not emit for other properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_DoesNotEmitForOtherProperties()
    {
        var vm = new TestViewModel();
        var changes = 0;
        using var sub = MauiReactiveHelpers.CreatePropertyChangedPulse(vm, "OtherProperty")
            .Subscribe(_ => changes++);

        vm.Name = "New Name";

        await Assert.That(changes).IsEqualTo(0);
    }

    /// <summary>
    /// Tests that CreatePropertyValueObservable emits initial and subsequent values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_EmitsInitialAndSubsequentValues()
    {
        var vm = new TestViewModel { Name = "Initial" };
        var values = new List<string?>();

        using var sub = MauiReactiveHelpers.CreatePropertyValueObservable(
            vm,
            nameof(TestViewModel.Name),
            () => vm.Name)
            .Subscribe(values.Add);

        vm.Name = "Updated";

        await Assert.That(values.Count).IsEqualTo(2);
        await Assert.That(values[0]).IsEqualTo("Initial");
        await Assert.That(values[1]).IsEqualTo("Updated");
    }

    private class TestViewModel : INotifyPropertyChanged
    {
        private string? _name;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}