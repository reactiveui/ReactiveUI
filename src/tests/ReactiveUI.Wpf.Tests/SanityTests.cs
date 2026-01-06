// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Basic sanity tests that run on all platforms including Linux/Mac.
/// These tests ensure the test project builds and runs even on non-Windows platforms.
/// </summary>
public class SanityTests
{
    /// <summary>
    /// Verifies that basic ReactiveUI functionality works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveObject_RaisesPropertyChanged()
    {
        // Arrange
        var testObject = new TestReactiveObject();
        var propertyChanged = false;
        testObject.PropertyChanged += (_, _) => propertyChanged = true;

        // Act
        testObject.TestProperty = "new value";

        // Assert
        await Assert.That(propertyChanged).IsTrue();
        await Assert.That(testObject.TestProperty).IsEqualTo("new value");
    }

    /// <summary>
    /// Verifies that WhenAnyValue works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_ObservesPropertyChanges()
    {
        // Arrange
        var testObject = new TestReactiveObject();
        string? observedValue = null;

        testObject.WhenAnyValue(x => x.TestProperty)
            .Subscribe(value => observedValue = value);

        // Act
        testObject.TestProperty = "test value";

        // Assert
        await Assert.That(observedValue).IsEqualTo("test value");
    }

    /// <summary>
    /// Simple test reactive object for sanity tests.
    /// </summary>
    private sealed class TestReactiveObject : ReactiveObject
    {
        private string? _testProperty;

        /// <summary>Gets or sets the test property.</summary>
        public string? TestProperty
        {
            get => _testProperty;
            set => this.RaiseAndSetIfChanged(ref _testProperty, value);
        }
    }
}
