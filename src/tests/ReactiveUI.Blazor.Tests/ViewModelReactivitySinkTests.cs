// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Blazor.Internal;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for <see cref="ViewModelReactivitySink{T}"/>, the sink that re-renders a component when its view model is
/// reassigned and whenever the currently observed view model raises a property change.
/// </summary>
public class ViewModelReactivitySinkTests
{
    /// <summary>The component property name the sink watches for view model reassignment.</summary>
    private const string ViewModelPropertyName = nameof(ComponentStub.ViewModel);

    /// <summary>An arbitrary view model property name, used to raise a change on the observed view model.</summary>
    private const string ObservedPropertyName = "Value";

    /// <summary>
    /// Verifies that the sink observes the view model the component already holds when it is constructed, without
    /// forcing a render for that initial value.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Constructor_ObservesTheViewModelTheComponentAlreadyHolds()
    {
        var viewModel = new ViewModelStub();
        var component = new ComponentStub { ViewModel = viewModel };
        var renderCount = 0;

        using var sink = CreateSink(component, () => renderCount++);

        await Assert.That(viewModel.AttachCount).IsEqualTo(1);
        await Assert.That(renderCount).IsEqualTo(0);

        viewModel.RaisePropertyChanged(ObservedPropertyName);

        await Assert.That(renderCount).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that disposal detaches both the component handler and the observed view model handler, so neither the
    /// component nor a retained view model can drive a render afterwards.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Dispose_DetachesFromTheComponentAndTheObservedViewModel()
    {
        var viewModel = new ViewModelStub();
        var component = new ComponentStub { ViewModel = viewModel };
        var renderCount = 0;
        var sink = CreateSink(component, () => renderCount++);

        sink.Dispose();

        await Assert.That(component.DetachCount).IsEqualTo(1);
        await Assert.That(viewModel.DetachCount).IsEqualTo(1);

        viewModel.RaisePropertyChanged(ObservedPropertyName);
        component.RaisePropertyChanged(ViewModelPropertyName);

        await Assert.That(renderCount).IsEqualTo(0);
    }

    /// <summary>
    /// Verifies that disposal is idempotent: a second call neither throws nor detaches the handlers again, so it cannot
    /// remove a handler another sink attached in the meantime.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Dispose_CalledTwice_DetachesTheHandlersOnce()
    {
        var viewModel = new ViewModelStub();
        var component = new ComponentStub { ViewModel = viewModel };
        var sink = CreateSink(component, static () => { });
        sink.Dispose();

        await Assert.That(sink.Dispose).ThrowsNothing();

        await Assert.That(component.DetachCount).IsEqualTo(1);
        await Assert.That(viewModel.DetachCount).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that a sink constructed against a component with no view model still releases the component handler on
    /// disposal.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Dispose_WithNoViewModelObserved_DetachesFromTheComponent()
    {
        var component = new ComponentStub();
        var renderCount = 0;
        using var sink = CreateSink(component, () => renderCount++);

        await Assert.That(sink.Dispose).ThrowsNothing();

        await Assert.That(component.DetachCount).IsEqualTo(1);

        component.RaisePropertyChanged(ViewModelPropertyName);

        await Assert.That(renderCount).IsEqualTo(0);
    }

    /// <summary>
    /// Verifies that a change to some other component property is ignored: it neither renders nor disturbs the observed
    /// view model.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ComponentPropertyChanged_ForAnUnrelatedProperty_IsIgnored()
    {
        var viewModel = new ViewModelStub();
        var component = new ComponentStub { ViewModel = viewModel };
        var renderCount = 0;
        using var sink = CreateSink(component, () => renderCount++);

        component.RaisePropertyChanged("SomeOtherProperty");

        await Assert.That(renderCount).IsEqualTo(0);
        await Assert.That(viewModel.AttachCount).IsEqualTo(1);
        await Assert.That(viewModel.DetachCount).IsEqualTo(0);
    }

    /// <summary>Verifies that clearing the component's view model does not render, as there is nothing new to show.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ComponentPropertyChanged_WhenTheViewModelIsCleared_DoesNotRender()
    {
        var viewModel = new ViewModelStub();
        var component = new ComponentStub { ViewModel = viewModel };
        var renderCount = 0;
        using var sink = CreateSink(component, () => renderCount++);

        component.ViewModel = null;

        await Assert.That(renderCount).IsEqualTo(0);
    }

    /// <summary>
    /// Verifies that re-announcing the same view model instance still renders but leaves the existing subscription in
    /// place, so the sink does not churn handlers on a redundant reassignment.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ComponentPropertyChanged_WithTheSameViewModelInstance_RendersWithoutResubscribing()
    {
        var viewModel = new ViewModelStub();
        var component = new ComponentStub { ViewModel = viewModel };
        var renderCount = 0;
        using var sink = CreateSink(component, () => renderCount++);

        component.RaisePropertyChanged(ViewModelPropertyName);

        await Assert.That(renderCount).IsEqualTo(1);
        await Assert.That(viewModel.AttachCount).IsEqualTo(1);
        await Assert.That(viewModel.DetachCount).IsEqualTo(0);
    }

    /// <summary>
    /// Verifies that the sink drops a view model it picked up while attaching if the component has already cleared it
    /// again by the time the initial swap runs, so no stale view model is left driving renders.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Constructor_WhenTheComponentClearsItsViewModelWhileAttaching_DropsTheStaleViewModel()
    {
        var viewModel = new ViewModelStub();
        var component = new ComponentStub();
        var renderCount = 0;

        using var sink = new ViewModelReactivitySink<ViewModelStub>(
            () => component.ViewModel,
            handler =>
            {
                component.PropertyChanged += handler;

                // The component publishes a view model and clears it again while the sink is still attaching, so the
                // sink's initial swap runs with the published view model subscribed but nothing current.
                component.ViewModel = viewModel;
                component.ViewModel = null;
            },
            handler => component.PropertyChanged -= handler,
            ViewModelPropertyName,
            () => renderCount++);

        await Assert.That(viewModel.AttachCount).IsEqualTo(1);
        await Assert.That(viewModel.DetachCount).IsEqualTo(1);

        renderCount = 0;
        viewModel.RaisePropertyChanged(ObservedPropertyName);

        await Assert.That(renderCount).IsEqualTo(0);
    }

    /// <summary>Creates a sink wired to <paramref name="component"/> in the same shape the component base classes use.</summary>
    /// <param name="component">The component whose view model the sink observes.</param>
    /// <param name="stateHasChanged">The render callback the sink invokes.</param>
    /// <returns>The sink under test.</returns>
    private static ViewModelReactivitySink<ViewModelStub> CreateSink(ComponentStub component, Action stateHasChanged) =>
        new(
            () => component.ViewModel,
            handler => component.PropertyChanged += handler,
            handler => component.PropertyChanged -= handler,
            ViewModelPropertyName,
            stateHasChanged);

    /// <summary>
    /// A property-change source that records how many handlers have been attached and detached, so a test can tell
    /// whether the sink moved a subscription or left it alone.
    /// </summary>
    private class NotifyingStub : INotifyPropertyChanged
    {
        /// <summary>The attached handlers.</summary>
        private PropertyChangedEventHandler? _propertyChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                AttachCount++;
                _propertyChanged += value;
            }

            remove
            {
                DetachCount++;
                _propertyChanged -= value;
            }
        }

        /// <summary>Gets the number of handlers that have been attached.</summary>
        public int AttachCount { get; private set; }

        /// <summary>Gets the number of handlers that have been detached.</summary>
        public int DetachCount { get; private set; }

        /// <summary>Raises <see cref="PropertyChanged"/> for the supplied property.</summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        public void RaisePropertyChanged(string propertyName) => _propertyChanged?.Invoke(this, new(propertyName));
    }

    /// <summary>A stand-in for a reactive component, raising a property change whenever its view model is assigned.</summary>
    private sealed class ComponentStub : NotifyingStub
    {
        /// <summary>Gets or sets the current view model. Assignment always announces the change, as a reassignment does.</summary>
        public ViewModelStub? ViewModel
        {
            get;
            set
            {
                field = value;
                RaisePropertyChanged(nameof(ViewModel));
            }
        }
    }

    /// <summary>A stand-in for a view model, used to observe how the sink attaches to and detaches from it.</summary>
    private sealed class ViewModelStub : NotifyingStub;
}
