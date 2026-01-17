// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests for WinformsCreatesObservableForProperty.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]

public class WinformsCreatesObservableForPropertyTests
{

    /// <summary>
    /// Tests that GetAffinityForObject returns correct affinity for Component types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_ComponentType_ReturnsCorrectAffinity()
    {
        var creator = new WinformsCreatesObservableForProperty();

        // Should return 8 for property with Changed event
        var affinity = creator.GetAffinityForObject(typeof(TestComponent), nameof(TestComponent.TestProperty));
        await Assert.That(affinity).IsEqualTo(8);
    }

    /// <summary>
    /// Tests that GetAffinityForObject returns 0 for non-Component types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_NonComponentType_ReturnsZero()
    {
        var creator = new WinformsCreatesObservableForProperty();

        var affinity = creator.GetAffinityForObject(typeof(string), "Length");
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    /// Tests that GetNotificationForProperty sends notifications when property changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_PropertyChanged_SendsNotification()
    {
        var creator = new WinformsCreatesObservableForProperty();
        var testComponent = new TestComponent();
        IObservedChange<object, object?>? receivedChange = null;

        Expression<Func<string?>> expression = () => testComponent.TestProperty;
        var observable = creator.GetNotificationForProperty(
            testComponent,
            expression,
            nameof(TestComponent.TestProperty))
            .ObserveOn(ImmediateScheduler.Instance);

        observable.Subscribe(change => receivedChange = change);

        // Trigger the property changed event
        testComponent.TestProperty = "new value";

        await Assert.That(receivedChange).IsNotNull();
        await Assert.That(receivedChange!.Sender).IsEqualTo(testComponent);
    }

    /// <summary>
    /// Tests that GetNotificationForProperty throws when event not found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_NoEvent_ThrowsInvalidOperationException()
    {
        var creator = new WinformsCreatesObservableForProperty();
        var testComponent = new TestComponentWithoutEvent();
        Exception? caughtException = null;

        Expression<Func<string?>> expression = () => testComponent.PropertyWithoutEvent;

        try
        {
            var observable = creator.GetNotificationForProperty(
                testComponent,
                expression,
                nameof(TestComponentWithoutEvent.PropertyWithoutEvent))
                .ObserveOn(ImmediateScheduler.Instance);

            // Subscribe to execute the observable - this is where the exception will be thrown
            observable.Subscribe();
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        await Assert.That(caughtException).IsNotNull();
        await Assert.That(caughtException).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>
    /// Test component with a property that has a Changed event.
    /// </summary>
    private class TestComponent : Component
    {
        private string? _testProperty;

        /// <summary>
        /// Occurs when test property changed.
        /// </summary>
        public event EventHandler? TestPropertyChanged;

        /// <summary>
        /// Gets or sets the test property.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? TestProperty
        {
            get => _testProperty;
            set
            {
                if (_testProperty != value)
                {
                    _testProperty = value;
                    TestPropertyChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    /// Test component with a property that does NOT have a Changed event.
    /// </summary>
    private class TestComponentWithoutEvent : Component
    {
        /// <summary>
        /// Gets or sets a property without a corresponding Changed event.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? PropertyWithoutEvent { get; set; }
    }
}
