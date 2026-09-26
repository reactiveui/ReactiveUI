// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using Splat;
using TUnit.Core.Executors;
using DesignerProperties = System.ComponentModel.DesignerProperties;
using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;
using PropertyChangedEventHandler = System.ComponentModel.PropertyChangedEventHandler;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests the split between the default WPF hosts, which ask the view locator's ahead-of-time safe lookup (the
/// generated view lookup and the <c>Map</c> registrations), and their Unsafe twins, which also ask the service locator.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class UnsafeViewResolutionTests
{
    /// <summary>The number of lookups a host performs when the contract lookup falls back to the default view.</summary>
    private const int LookupsWithFallback = 2;

    /// <summary>Verifies the default host asks the locator's safe lookup and never its reflective one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHost_ResolvesThroughTheSafeLookup()
    {
        var locator = new RecordingViewLocator(static () => new MappedOnlyView());
        var host = new ViewModelViewHost { ViewLocator = locator, ViewModel = new TestViewModel() };

        Activate(host);

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<MappedOnlyView>();
            await Assert.That(locator.SafeLookups).IsGreaterThan(0);
            await Assert.That(locator.UnsafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the Unsafe host asks the locator's reflective lookup and never its safe one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHostUnsafe_ResolvesThroughTheUnsafeLookup()
    {
        var locator = new RecordingViewLocator(static () => new MappedOnlyView());
        var host = new ViewModelViewHostUnsafe { ViewLocator = locator, ViewModel = new TestViewModel() };

        Activate(host);

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<MappedOnlyView>();
            await Assert.That(locator.UnsafeLookups).IsGreaterThan(0);
            await Assert.That(locator.SafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the default host shows a view the app added to the view locator with <c>Map</c>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHost_ShowsAMappedView()
    {
        var locator = new DefaultViewLocator();
        locator.Map<MappedOnlyViewModel>(static () => new MappedOnlyView());
        var viewModel = new MappedOnlyViewModel();
        var host = new ViewModelViewHost { ViewLocator = locator, ViewModel = viewModel };

        Activate(host);

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<MappedOnlyView>();
            await Assert.That(((MappedOnlyView)host.Content).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the default host falls back to the default view, then shows its default content when neither lookup finds one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHost_WithNoView_FallsBackAndShowsTheDefaultContent()
    {
        var defaultContent = new Label();
        var locator = new RecordingViewLocator(static () => null);
        var host = new ViewModelViewHost { DefaultContent = defaultContent, ViewLocator = locator, ViewModel = new TestViewModel() };

        Activate(host);

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(defaultContent);
            await Assert.That(locator.SafeLookups).IsEqualTo(LookupsWithFallback);
        }
    }

    /// <summary>Verifies the default routed host asks the locator's safe lookup and never its reflective one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_ResolvesThroughTheSafeLookup()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new RecordingViewLocator(static () => new MappedOnlyView());
        var host = new RoutedViewHost { ViewLocator = locator, Router = router };
        Activate(host);

        using var navigation = router.Navigate.Execute(new TestViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<MappedOnlyView>();
            await Assert.That(locator.SafeLookups).IsGreaterThan(0);
            await Assert.That(locator.UnsafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the Unsafe routed host asks the locator's reflective lookup and never its safe one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHostUnsafe_ResolvesThroughTheUnsafeLookup()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new RecordingViewLocator(static () => new MappedOnlyView());
        var host = new RoutedViewHostUnsafe { ViewLocator = locator, Router = router };
        Activate(host);

        using var navigation = router.Navigate.Execute(new TestViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<MappedOnlyView>();
            await Assert.That(locator.UnsafeLookups).IsGreaterThan(0);
            await Assert.That(locator.SafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the default routed host shows a view the app added to the view locator with <c>Map</c>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_ShowsAMappedView()
    {
        var locator = new DefaultViewLocator();
        locator.Map<MappedOnlyViewModel>(static () => new MappedOnlyView());
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHost { ViewLocator = locator, Router = router };
        var viewModel = new MappedOnlyViewModel();
        Activate(host);

        using var navigation = router.Navigate.Execute(viewModel).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<MappedOnlyView>();
            await Assert.That(((MappedOnlyView)host.Content).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the default routed host reports a view model neither lookup finds a view for.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_WithNoView_Throws()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHost { ViewLocator = new RecordingViewLocator(static () => null), Router = router };
        Activate(host);

        await Assert.That(() => router.Navigate.Execute(new TestViewModel()).Subscribe())
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies the Unsafe hook replaces the default template the default hook assigned.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoDataTemplateBindingHookUnsafe_ReplacesTheDefaultTemplate()
    {
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        _ = new AutoDataTemplateBindingHook().ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);
        var proceed = new AutoDataTemplateBindingHookUnsafe().ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(AutoDataTemplateBindingHookUnsafe.DefaultItemTemplate.Value);
        }
    }

    /// <summary>Verifies the Unsafe hook leaves a template the app set itself alone.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoDataTemplateBindingHookUnsafe_KeepsAnAppTemplate()
    {
        var template = new DataTemplate();
        var itemsControl = new ListBox { ItemTemplate = template };
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        var proceed = new AutoDataTemplateBindingHookUnsafe().ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(template);
        }
    }

    /// <summary>Verifies the Unsafe template hosts each item in the Unsafe host, bound to the item.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// The template is built the way <see cref="AutoDataTemplateBindingHookUnsafe.DefaultItemTemplate"/> builds it. The
    /// shared template belongs to the thread that first read it, and each test runs on a dispatcher thread of its own.
    /// </remarks>
    [Test]
    public async Task AutoDataTemplateBindingHookUnsafe_HostsTheItemInAViewModelViewHostUnsafe()
    {
        var host = AutoDataTemplateBindingHook.CreateItemTemplate(nameof(ViewModelViewHostUnsafe)).LoadContent() as ViewModelViewHostUnsafe;

        using (Assert.Multiple())
        {
            await Assert.That(host).IsNotNull();
            await Assert.That(BindingOperations.IsDataBound(host!, ViewModelViewHost.ViewModelProperty)).IsTrue();
            await Assert.That(host!.IsTabStop).IsFalse();
        }
    }

    /// <summary>Verifies the default host does not find a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHost_ViewOnlyInTheServiceLocator_ShowsTheDefaultContent()
    {
        RegisterSplatOnlyView();
        var defaultContent = new Label();
        var host = new ViewModelViewHost { DefaultContent = defaultContent, ViewLocator = new DefaultViewLocator(), ViewModel = new SplatOnlyViewModel() };

        Activate(host);

        await Assert.That(host.Content).IsSameReferenceAs(defaultContent);
    }

    /// <summary>Verifies the Unsafe host finds a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHostUnsafe_ViewOnlyInTheServiceLocator_IsFound()
    {
        RegisterSplatOnlyView();
        var viewModel = new SplatOnlyViewModel();
        var host = new ViewModelViewHostUnsafe { ViewLocator = new DefaultViewLocator(), ViewModel = viewModel };

        Activate(host);

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<SplatOnlyView>();
            await Assert.That(((SplatOnlyView)host.Content).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the default routed host does not find a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_ViewOnlyInTheServiceLocator_Throws()
    {
        RegisterSplatOnlyView();
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHost { ViewLocator = new DefaultViewLocator(), Router = router };
        Activate(host);

        await Assert.That(() => router.Navigate.Execute(new SplatOnlyViewModel()).Subscribe())
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies the Unsafe routed host finds a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHostUnsafe_ViewOnlyInTheServiceLocator_IsFound()
    {
        RegisterSplatOnlyView();
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHostUnsafe { ViewLocator = new DefaultViewLocator(), Router = router };
        var viewModel = new SplatOnlyViewModel();
        Activate(host);

        using var navigation = router.Navigate.Execute(viewModel).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsTypeOf<SplatOnlyView>();
            await Assert.That(((SplatOnlyView)host.Content).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the default host activates the view model it hosts while the host is active.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHost_ActivatesItsViewModel()
    {
        using var viewModel = new ActivatableRoutableViewModel();
        var host = new ViewModelViewHost { ViewLocator = new RecordingViewLocator(static () => new MappedOnlyView()), ViewModel = viewModel };

        Activate(host);

        await Assert.That(viewModel.IsActive).IsTrue();
    }

    /// <summary>Verifies a routed host a subclass makes an <see cref="IViewFor"/> activates its view model, and follows a new one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_SubclassThatIsAViewWithChangeNotification_ActivatesItsViewModel()
    {
        using var first = new ActivatableRoutableViewModel();
        using var second = new ActivatableRoutableViewModel();
        var host = new NotifyingViewForRoutedViewHost { ViewModel = first, Router = new(Sequencer.Immediate) };

        Activate(host);
        var firstActiveOnActivation = first.IsActive;
        host.RaiseUnrelatedPropertyChanged();
        host.ViewModel = second;

        using (Assert.Multiple())
        {
            await Assert.That(firstActiveOnActivation).IsTrue();
            await Assert.That(first.IsActive).IsFalse();
            await Assert.That(second.IsActive).IsTrue();
        }
    }

    /// <summary>Verifies a routed host a subclass makes an <see cref="IViewFor"/> without change notification activates its view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_SubclassThatIsAView_ActivatesItsViewModel()
    {
        using var viewModel = new ActivatableRoutableViewModel();
        var host = new ViewForRoutedViewHost { ViewModel = viewModel, Router = new(Sequencer.Immediate) };

        Activate(host);

        await Assert.That(viewModel.IsActive).IsTrue();
    }

    /// <summary>Verifies the default host wires nothing while a designer loads it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHost_InDesignMode_ResolvesNothing()
    {
        var locator = new RecordingViewLocator(static () => new MappedOnlyView());
        var host = new DesignModeViewModelViewHost { ViewLocator = locator, ViewModel = new TestViewModel() };

        Activate(host);

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsNull();
            await Assert.That(locator.SafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the default routed host wires nothing while a designer loads it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_InDesignMode_ResolvesNothing()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new RecordingViewLocator(static () => new MappedOnlyView());
        var host = new DesignModeRoutedViewHost { ViewLocator = locator, Router = router };
        Activate(host);

        using var navigation = router.Navigate.Execute(new TestViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsNull();
            await Assert.That(locator.SafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Registers <see cref="SplatOnlyView"/> for <see cref="SplatOnlyViewModel"/> with the service locator only.</summary>
    private static void RegisterSplatOnlyView() =>
        AppLocator.CurrentMutable.Register<IViewFor<SplatOnlyViewModel>>(static () => new SplatOnlyView());

    /// <summary>Raises the Loaded event so the host's activation wires its view resolution.</summary>
    /// <param name="element">The host to activate.</param>
    private static void Activate(FrameworkElement element) =>
        element.RaiseEvent(new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent });

    /// <summary>Builds the observed-change chain a property binding would hand a hook.</summary>
    /// <param name="sender">The bound target.</param>
    /// <param name="expression">The bound property expression.</param>
    /// <returns>The observed-change chain ending at the bound property.</returns>
    private static IObservedChange<object, object>[] ViewProperties(object sender, System.Linq.Expressions.Expression expression) =>
        [new ObservedChange<object, object>(sender, Reflection.Rewrite(expression), null!)];

    /// <summary>A routed host a subclass makes an <see cref="IViewFor"/>, with no change notification.</summary>
    private sealed class ViewForRoutedViewHost : RoutedViewHost, IViewFor
    {
        /// <inheritdoc/>
        public object? ViewModel { get; set; }
    }

    /// <summary>A routed host a subclass makes an <see cref="IViewFor"/> that raises a change for its view model.</summary>
    private sealed class NotifyingViewForRoutedViewHost : RoutedViewHost, IViewFor, INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public object? ViewModel
        {
            get;
            set
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
        }

        /// <summary>Raises a change for a property other than <see cref="ViewModel"/>.</summary>
        public void RaiseUnrelatedPropertyChanged() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Router)));
    }

    /// <summary>A view model host that reports it is loaded by a designer.</summary>
    private sealed class DesignModeViewModelViewHost : ViewModelViewHost
    {
        /// <summary>Initializes static members of the <see cref="DesignModeViewModelViewHost"/> class.</summary>
        static DesignModeViewModelViewHost() =>
            DesignerProperties.IsInDesignModeProperty.OverrideMetadata(
                typeof(DesignModeViewModelViewHost),
                new FrameworkPropertyMetadata(true));
    }

    /// <summary>A routed host that reports it is loaded by a designer.</summary>
    private sealed class DesignModeRoutedViewHost : RoutedViewHost
    {
        /// <summary>Initializes static members of the <see cref="DesignModeRoutedViewHost"/> class.</summary>
        static DesignModeRoutedViewHost() =>
            DesignerProperties.IsInDesignModeProperty.OverrideMetadata(
                typeof(DesignModeRoutedViewHost),
                new FrameworkPropertyMetadata(true));
    }
}
