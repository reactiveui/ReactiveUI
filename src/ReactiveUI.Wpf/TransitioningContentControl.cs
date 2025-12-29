// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using ReactiveUI.Helpers;

// This control is gratefully borrowed from http://blog.landdolphin.net/?p=17
// Thanks guys!
namespace ReactiveUI;

/// <summary>
/// A <see cref="ContentControl"/> that animates the visual transition whenever its <see cref="ContentControl.Content"/>
/// changes.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TransitioningContentControl"/> works by taking a bitmap “snapshot” of the previously displayed content
/// (rendered to a <see cref="RenderTargetBitmap"/>) and then running a WPF <see cref="VisualStateManager"/> transition
/// between that image and the new content.
/// </para>
/// <para>
/// The default template is expected to define the following named parts:
/// </para>
/// <list type="bullet">
/// <item>
/// <description><c>PART_Container</c>: a <see cref="FrameworkElement"/> (typically a <see cref="Grid"/>) that hosts visual states.</description>
/// </item>
/// <item>
/// <description><c>PART_PreviousImageSite</c>: an <see cref="Image"/> used to display a snapshot of the outgoing content.</description>
/// </item>
/// <item>
/// <description><c>PART_CurrentContentPresentationSite</c>: a <see cref="ContentPresenter"/> used to display the incoming content.</description>
/// </item>
/// </list>
/// <para>
/// The template must also provide a <see cref="VisualStateGroup"/> named <c>PresentationStates</c> containing a
/// <c>Normal</c> state, plus the transition states referenced below (see <see cref="Transition"/> and
/// <see cref="Direction"/>).
/// </para>
/// <para>
/// <strong>How it transitions</strong>
/// </para>
/// <list type="number">
/// <item>
/// <description>
/// When <see cref="ContentControl.Content"/> changes, the control captures a bitmap snapshot of the current visual from
/// <c>PART_CurrentContentPresentationSite</c> into <c>PART_PreviousImageSite</c>.
/// </description>
/// </item>
/// <item>
/// <description>
/// The new content is assigned to <c>PART_CurrentContentPresentationSite</c>.
/// </description>
/// </item>
/// <item>
/// <description>
/// A <see cref="Storyboard"/> associated with the configured visual state is started using
/// <see cref="VisualStateManager.GoToState(FrameworkElement,string,bool)"/>.
/// </description>
/// </item>
/// <item>
/// <description>
/// When the transition completes, the control returns to the <c>Normal</c> state and clears the outgoing snapshot image.
/// </description>
/// </item>
/// </list>
/// <para>
/// <strong>Visual state naming</strong>
/// </para>
/// <para>
/// Most transitions use the visual state name <c>Transition_{Transition}{Direction}</c>. For example:
/// <c>Transition_SlideLeft</c>.
/// </para>
/// <para>
/// <see cref="TransitionType.Fade"/> uses <c>Transition_Fade</c> (no direction suffix).
/// </para>
/// <para>
/// <see cref="TransitionType.Bounce"/> uses a two-phase animation: an “out” phase followed by an “in” phase:
/// <c>Transition_Bounce{Direction}Out</c> then <c>Transition_Bounce{Direction}In</c>.
/// </para>
/// <para>
/// <strong>Events</strong>
/// </para>
/// <para>
/// The <see cref="TransitionStarted"/> event fires after a transition has been queued and just before the first visual
/// state is entered. The <see cref="TransitionCompleted"/> event fires after the storyboard completes and the control
/// has returned to <c>Normal</c> (and released the outgoing snapshot image).
/// </para>
/// <para>
/// <strong>Notes</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// If the template has not been applied yet (for example, prior to <see cref="OnApplyTemplate"/>), the control falls back
/// to simply updating the content with no animation.
/// </description>
/// </item>
/// <item>
/// <description>
/// If a transition is already in progress, additional content changes update the current content immediately (no queued
/// animations).
/// </description>
/// </item>
/// <item>
/// <description>
/// Snapshot creation depends on the element’s current layout size. If the content presenter has a zero width or height,
/// the snapshot is not meaningful and the control will effectively behave like a normal <see cref="ContentControl"/>.
/// </description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <para>
/// Basic usage in XAML. The control animates when <c>Content</c> changes.
/// </para>
/// <code language="xaml">
/// &lt;rxui:TransitioningContentControl
///     Transition="Slide"
///     Direction="Left"
///     Duration="0:0:0.25"
///     Content="{Binding CurrentView}" /&gt;
/// </code>
/// <para>
/// Example with event handlers.
/// </para>
/// <code language="xaml">
/// &lt;rxui:TransitioningContentControl
///     Transition="Fade"
///     TransitionStarted="OnTransitionStarted"
///     TransitionCompleted="OnTransitionCompleted"
///     Content="{Binding CurrentView}" /&gt;
/// </code>
/// </example>
[TemplatePart(Name = "PART_Container", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_PreviousImageSite", Type = typeof(Image))]
[TemplatePart(Name = "PART_CurrentContentPresentationSite", Type = typeof(ContentPresenter))]
[TemplateVisualState(Name = NormalState, GroupName = PresentationGroup)]
public class TransitioningContentControl : ContentControl
{
    /// <summary>
    /// Identifies the <see cref="Transition"/> dependency property.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="TransitionType.Fade"/>.
    /// </remarks>
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.RegisterAttached(
     nameof(Transition),
     typeof(TransitionType),
     typeof(TransitioningContentControl),
     new PropertyMetadata(TransitionType.Fade));

    /// <summary>
    /// Identifies the <see cref="Direction"/> dependency property.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="TransitionDirection.Left"/>.
    /// </remarks>
    public static readonly DependencyProperty TransitionDirectionProperty = DependencyProperty.RegisterAttached(
     nameof(TransitionDirection),
     typeof(TransitionDirection),
     typeof(TransitioningContentControl),
     new PropertyMetadata(TransitionDirection.Left));

    /// <summary>
    /// Identifies the <see cref="Duration"/> dependency property.
    /// </summary>
    /// <remarks>
    /// The default value is 0.3 seconds.
    /// </remarks>
    public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.RegisterAttached(
     nameof(Duration),
     typeof(TimeSpan),
     typeof(TransitioningContentControl),
     new PropertyMetadata(TimeSpan.FromSeconds(0.3)));

    private const string PresentationGroup = "PresentationStates";
    private const string NormalState = "Normal";
    private bool _isTransitioning;
    private Storyboard? _startingTransition;
    private Storyboard? _completingTransition;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransitioningContentControl"/> class.
    /// </summary>
    /// <remarks>
    /// The control’s default style key is set to <see cref="TransitioningContentControl"/> so that it can locate its
    /// default template.
    /// </remarks>
    public TransitioningContentControl() => DefaultStyleKey = typeof(TransitioningContentControl);

    /// <summary>
    /// Occurs when a transition has completed.
    /// </summary>
    /// <remarks>
    /// The event is raised after the control returns to the <c>Normal</c> visual state and after the previous content
    /// snapshot has been released.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1159:Use EventHandler<T>", Justification = "Using WPF's RoutedEventHandler pattern.")]
    public event RoutedEventHandler? TransitionCompleted;

    /// <summary>
    /// Occurs when a transition has started.
    /// </summary>
    /// <remarks>
    /// The event is raised when a transition is about to begin (after the control has prepared the outgoing snapshot and
    /// set the new content), but before the first visual state is entered.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1159:Use EventHandler<T>", Justification = "Using WPF's RoutedEventHandler pattern.")]
    public event RoutedEventHandler? TransitionStarted;

    /// <summary>
    /// Specifies the animation behavior to use when the content changes.
    /// </summary>
    /// <remarks>
    /// The configured value determines which visual state(s) the control attempts to enter when transitioning.
    /// See <see cref="TransitioningContentControl"/> remarks for the required state names.
    /// </remarks>
    public enum TransitionType
    {
        /// <summary>
        /// A simple fading transition (no directional variant).
        /// </summary>
        Fade,

        /// <summary>
        /// A transition that slides old content out of view, and slides new content back in from the same direction.
        /// </summary>
        Move,

        /// <summary>
        /// A transition that keeps old content in view, and slides new content over it.
        /// </summary>
        Slide,

        /// <summary>
        /// A transition that slides old content in view, and slides new content over it a short distance while changing opacity.
        /// </summary>
        Drop,

        /// <summary>
        /// A transition that slides old content out of view, then slides new content back in from the opposite direction.
        /// </summary>
        Bounce
    }

    /// <summary>
    /// Specifies the directional variant of a transition (where applicable).
    /// </summary>
    /// <remarks>
    /// Direction is used by transition types that have direction-specific visual states, such as <see cref="TransitionType.Slide"/>,
    /// <see cref="TransitionType.Move"/>, and <see cref="TransitionType.Bounce"/>.
    /// </remarks>
    public enum TransitionDirection
    {
        /// <summary>
        /// Up direction.
        /// </summary>
        Up,

        /// <summary>
        /// Down direction.
        /// </summary>
        Down,

        /// <summary>
        /// Left direction.
        /// </summary>
        Left,

        /// <summary>
        /// Right direction.
        /// </summary>
        Right
    }

    /// <summary>
    /// Gets or sets the transition type used when the content changes.
    /// </summary>
    /// <value>
    /// The transition type. The default is <see cref="TransitionType.Fade"/>.
    /// </value>
    /// <remarks>
    /// This property selects which visual state(s) the control will attempt to enter during a transition.
    /// </remarks>
    public TransitionType Transition
    {
        get => (TransitionType)GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction used by directional transitions.
    /// </summary>
    /// <value>
    /// The transition direction. The default is <see cref="TransitionDirection.Left"/>.
    /// </value>
    /// <remarks>
    /// This property is ignored by <see cref="TransitionType.Fade"/>.
    /// </remarks>
    public TransitionDirection Direction
    {
        get => (TransitionDirection)GetValue(TransitionDirectionProperty);
        set => SetValue(TransitionDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the transition duration.
    /// </summary>
    /// <value>The duration.</value>
    public TimeSpan Duration { get => (TimeSpan)GetValue(TransitionDurationProperty); set => SetValue(TransitionDurationProperty, value); }

    /// <summary>
    /// Gets or sets a value indicating whether to override DPI scaling for testing.
    /// </summary>
    internal static bool OverrideDpi { get; set; }

    /// <summary>
    /// Gets or sets the starting transition storyboard for testing.
    /// </summary>
    internal Storyboard? StartingTransition
    {
        get => _startingTransition;
        set
        {
            _startingTransition = value;
            if (_startingTransition is not null)
            {
                SetTransitionDefaultValues();
            }
        }
    }

    /// <summary>
    /// Gets or sets the completing transition storyboard for testing.
    /// </summary>
    internal Storyboard? CompletingTransition
    {
        get => _completingTransition;
        set
        {
            // Decouple transition.
            if (_completingTransition is not null)
            {
                CompletingTransition!.Completed -= OnTransitionCompleted;
            }

            _completingTransition = value;

            if (_completingTransition is not null)
            {
                CompletingTransition!.Completed += OnTransitionCompleted;
                SetTransitionDefaultValues();
            }
        }
    }

    /// <summary>
    /// Gets the current content presentation site for testing.
    /// </summary>
    internal ContentPresenter? CurrentContentPresentationSite { get; private set; }

    /// <summary>
    /// Gets or sets the presentation state group for testing.
    /// </summary>
    internal VisualStateGroup? PresentationStateGroup { get; set; }

    /// <summary>
    /// Gets or sets the grid container associated with this instance.
    /// </summary>
    private Grid? Container { get; set; }

    /// <summary>
    /// Gets or sets the previous image associated with the site.
    /// </summary>
    private Image? PreviousImageSite { get; set; }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// When the template is applied, the control locates and caches the required template parts and the visual state
    /// group used for transitions.
    /// </para>
    /// <para>
    /// The following parts are required:
    /// <c>PART_Container</c> and <c>PART_CurrentContentPresentationSite</c>. If either is missing, an
    /// <see cref="InvalidOperationException"/> is thrown.
    /// </para>
    /// <para>
    /// The <c>PART_PreviousImageSite</c> part is optional; if it is missing, transitions will not run and the control
    /// behaves like a normal <see cref="ContentControl"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <c>PART_Container</c> or <c>PART_CurrentContentPresentationSite</c> cannot be found in the applied template.
    /// </exception>
    public override void OnApplyTemplate()
    {
        // Wire up all of the various control parts.
        if (GetTemplateChild("PART_Container") is not Grid container)
        {
            throw new InvalidOperationException("PART_Container not found.");
        }

        Container = container;

        if (GetTemplateChild("PART_CurrentContentPresentationSite") is not ContentPresenter contentPresenter)
        {
            throw new InvalidOperationException("PART_CurrentContentPresentationSite not found.");
        }

        CurrentContentPresentationSite = contentPresenter;
        PreviousImageSite = GetTemplateChild("PART_PreviousImageSite") as Image;

        // Set the current content site to the first piece of content.
        CurrentContentPresentationSite.Content = Content;

        if (VisualStateManager.GetVisualStateGroups(Container) is IEnumerable<VisualStateGroup> groups)
        {
            PresentationStateGroup = groups.FirstOrDefault(static o => o.Name == PresentationGroup);
        }

        VisualStateManager.GoToState(this, NormalState, false);
    }

    /// <summary>
    /// Gets the DPI scale for the specified UI element.
    /// </summary>
    /// <param name="uiElement">The UI element.</param>
    /// <returns>The DPI scale.</returns>
    internal static DpiScale GetDpiScaleForElement(UIElement uiElement)
    {
        var dpiScale = VisualTreeHelper.GetDpi(uiElement);

        if (OverrideDpi)
        {
            dpiScale = new DpiScale(1.25, 1.25);
        }

        return dpiScale;
    }

    /// <summary>
    /// Creates a render target bitmap from the specified UI element.
    /// </summary>
    /// <param name="uiElement">The UI element to render into a bitmap.</param>
    /// <returns>A frozen <see cref="RenderTargetBitmap"/> containing the rendered UI element, or <c>null</c> if the element has zero size.</returns>
    /// <remarks>
    /// This method captures the visual appearance of the UI element at its current DPI scale.
    /// The returned bitmap is frozen for thread-safety and performance.
    /// </remarks>
    internal static RenderTargetBitmap GetRenderTargetBitmapFromUiElement(UIElement uiElement)
    {
        if (uiElement.RenderSize.Height == 0 || uiElement.RenderSize.Width == 0)
        {
            return default!;
        }

        var dpiScale = GetDpiScaleForElement(uiElement);

        var pixelWidth = Math.Max(1.0, uiElement.RenderSize.Width * dpiScale.DpiScaleX);
        var pixelHeight = Math.Max(1.0, uiElement.RenderSize.Height * dpiScale.DpiScaleY);
        var renderTargetBitmap = new RenderTargetBitmap(
                                                        Convert.ToInt32(pixelWidth),
                                                        Convert.ToInt32(pixelHeight),
                                                        dpiScale.PixelsPerInchX,
                                                        dpiScale.PixelsPerInchY,
                                                        PixelFormats.Pbgra32);

        renderTargetBitmap.Render(uiElement);
        renderTargetBitmap.Freeze();

        return renderTargetBitmap;
    }

    /// <summary>
    /// Sets default values for fade transitions.
    /// </summary>
    internal void SetFadeTransitionDefaults()
    {
        if (CompletingTransition is null)
        {
            return;
        }

        var completingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[0];
        completingDoubleAnimation.Duration = Duration;
        var startingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[1];
        startingDoubleAnimation.Duration = Duration;
    }

    /// <summary>
    /// Sets default values for slide transitions.
    /// </summary>
    internal void SetSlideTransitionDefaults()
    {
        if (CompletingTransition is null)
        {
            return;
        }

        var startingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[0];
        startingDoubleAnimation.Duration = Duration;

        startingDoubleAnimation.From = Direction switch
        {
            TransitionDirection.Down => -ActualHeight,
            TransitionDirection.Up => ActualHeight,
            TransitionDirection.Right => -ActualWidth,
            TransitionDirection.Left => ActualWidth,
            _ => throw new InvalidOperationException($"Unsupported transition direction: {Direction}")
        };
    }

    /// <summary>
    /// Sets default values for move transitions.
    /// </summary>
    internal void SetMoveTransitionDefaults()
    {
        if (CompletingTransition is null)
        {
            return;
        }

        var completingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[0];
        var startingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[1];
        startingDoubleAnimation.Duration = Duration;
        completingDoubleAnimation.Duration = Duration;

        var (startingTo, completingFrom) = Direction switch
        {
            TransitionDirection.Down => (ActualHeight, -ActualHeight),
            TransitionDirection.Up => (-ActualHeight, ActualHeight),
            TransitionDirection.Right => (ActualWidth, -ActualWidth),
            TransitionDirection.Left => (-ActualWidth, ActualWidth),
            _ => throw new InvalidOperationException($"Unsupported transition direction: {Direction}")
        };

        startingDoubleAnimation.To = startingTo;
        completingDoubleAnimation.From = completingFrom;
    }

    /// <summary>
    /// Sets default values for bounce transitions.
    /// </summary>
    internal void SetBounceTransitionDefaults()
    {
        if (CompletingTransition is not null)
        {
            var completingDoubleAnimation = (DoubleAnimationUsingKeyFrames)CompletingTransition.Children[0];
            completingDoubleAnimation.KeyFrames[1].Value = ActualHeight;
        }

        if (StartingTransition is null)
        {
            return;
        }

        var startingDoubleAnimation = (DoubleAnimation)StartingTransition.Children[0];

        startingDoubleAnimation.To = Direction switch
        {
            TransitionDirection.Down => ActualHeight,
            TransitionDirection.Up => -ActualHeight,
            TransitionDirection.Right => ActualWidth,
            TransitionDirection.Left => -ActualWidth,
            _ => throw new InvalidOperationException($"Unsupported transition direction: {Direction}")
        };
    }

    /// <summary>
    /// Prepares the transition by capturing the current content as an image and setting the new content.
    /// </summary>
    /// <param name="newContent">The new content to display.</param>
    internal void PrepareTransitionImages(object newContent)
    {
        PreviousImageSite!.Source = GetRenderTargetBitmapFromUiElement(CurrentContentPresentationSite!);
        CurrentContentPresentationSite!.Content = newContent;
    }

    /// <summary>
    /// Configures the bounce transition by setting up both the outgoing and incoming storyboards.
    /// </summary>
    /// <returns>A tuple containing the starting transition name and the completing transition name.</returns>
    internal (string StartingName, string CompletingName) ConfigureBounceTransition()
    {
        var transitionInName = $"Transition_{Transition}{Direction}In";
        CompletingTransition = GetTransitionStoryboardByName(transitionInName);

        var startingTransitionName = $"Transition_{Transition}{Direction}Out";
        StartingTransition = GetTransitionStoryboardByName(startingTransitionName);

        return (startingTransitionName, transitionInName);
    }

    /// <summary>
    /// Configures a standard (non-bounce) transition by setting up the appropriate storyboard.
    /// </summary>
    /// <returns>The name of the transition to start.</returns>
    internal string ConfigureStandardTransition()
    {
        StartingTransition = null;
        var startingTransitionName = Transition switch
        {
            TransitionType.Fade => "Transition_Fade",
            _ => $"Transition_{Transition}{Direction}"
        };

        CompletingTransition = GetTransitionStoryboardByName(startingTransitionName);
        return startingTransitionName;
    }

    /// <summary>
    /// Retrieves the storyboard associated with the specified transition name from the visual state group.
    /// </summary>
    /// <param name="transitionName">The name of the transition whose storyboard is to be retrieved. Cannot be null, empty, or consist only of
    /// white-space characters.</param>
    /// <returns>The storyboard corresponding to the specified transition name.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the visual state group is not initialized, the states collection is invalid, or if a transition with
    /// the specified name is not found.</exception>
    internal Storyboard GetTransitionStoryboardByName(string transitionName)
    {
        ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(transitionName);

        if (PresentationStateGroup?.States is not IEnumerable<VisualState> states)
        {
            throw new InvalidOperationException("Visual state group is not initialized or states collection is invalid.");
        }

        var transition = states
            .Where(o => o.Name == transitionName)
            .Select(o => o.Storyboard)
            .FirstOrDefault();

        return transition ?? throw new InvalidOperationException($"Transition '{transitionName}' not found in visual state group.");
    }

    /// <summary>
    /// Sets default values for certain transition types.
    /// </summary>
    internal void SetTransitionDefaultValues()
    {
        // Do some special handling of particular transitions so that we get nice smooth transitions that utilize the size of the content.
        switch (Transition)
        {
            case TransitionType.Fade:
                SetFadeTransitionDefaults();
                break;

            case TransitionType.Slide:
                SetSlideTransitionDefaults();
                break;

            case TransitionType.Move:
                SetMoveTransitionDefaults();
                break;

            case TransitionType.Bounce:
                SetBounceTransitionDefaults();
                break;

            case TransitionType.Drop:
                break;

            default:
                throw new InvalidOperationException($"Unsupported transition type: {Transition}");
        }
    }

    /// <summary>
    /// Called when the value of the <see cref="ContentControl.Content"/> property changes.
    /// </summary>
    /// <param name="oldContent">The previous content value.</param>
    /// <param name="newContent">The new content value.</param>
    /// <remarks>
    /// This override queues and begins a transition (when possible) before invoking the base implementation.
    /// If the required template parts are not available, or the control is already transitioning, the new content is
    /// applied immediately without animation.
    /// </remarks>
    protected override void OnContentChanged(object oldContent, object newContent)
    {
        QueueTransition(newContent);
        base.OnContentChanged(oldContent, newContent);
    }

    /// <summary>
    /// Aborts the transition.
    /// </summary>
    private void AbortTransition()
    {
        // Go to a normal state and release our hold on the old content.
        VisualStateManager.GoToState(this, NormalState, false);
        _isTransitioning = false;

        if (PreviousImageSite is not null)
        {
            if (PreviousImageSite.Source is RenderTargetBitmap renderTargetBitmap)
            {
                renderTargetBitmap.Clear();
            }

            // https://github.com/dotnet/wpf/issues/2397
            PreviousImageSite.Source = null;
            PreviousImageSite.UpdateLayout();
        }
    }

    /// <summary>
    /// Handles the completion of a transition and raises the TransitionCompleted event.
    /// </summary>
    /// <param name="sender">The source of the event. This is typically the object that initiated the transition.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private void OnTransitionCompleted(object? sender, EventArgs e)
    {
        AbortTransition();

        TransitionCompleted?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Raises the TransitionStarted event to signal that a transition has begun.
    /// </summary>
    private void RaiseTransitionStarted() => TransitionStarted?.Invoke(this, new RoutedEventArgs());

    /// <summary>
    /// Queues a visual transition to display the specified content, applying the configured transition effect if
    /// possible.
    /// </summary>
    /// <remarks>If a transition is already in progress or required visual elements are unavailable, the
    /// content is updated immediately without animation. Otherwise, the method prepares and initiates the appropriate
    /// transition effect based on the current configuration.</remarks>
    /// <param name="newContent">The new content object to be displayed during the transition.</param>
    private void QueueTransition(object newContent)
    {
        // Both ContentPresenters must be available, otherwise a transition is useless.
        if (CurrentContentPresentationSite is null)
        {
            return;
        }

        if (_isTransitioning || PreviousImageSite is null)
        {
            CurrentContentPresentationSite.Content = newContent;
            return;
        }

        PrepareTransitionImages(newContent);

        string startingTransitionName;
        var transitionInName = string.Empty;
        var statesRemaining = 0;

        if (Transition == TransitionType.Bounce)
        {
            (startingTransitionName, transitionInName) = ConfigureBounceTransition();
            statesRemaining = 2;
            StartingTransition!.Completed += NextState;
        }
        else
        {
            if (StartingTransition is not null)
            {
                StartingTransition.Completed -= NextState;
            }

            startingTransitionName = ConfigureStandardTransition();
            statesRemaining = 1;
        }

        // Start the transition.
        _isTransitioning = true;
        RaiseTransitionStarted();

        statesRemaining--;
        VisualStateManager.GoToState(this, startingTransitionName, false);

        void NextState(object? o, EventArgs e)
        {
            StartingTransition!.Completed -= NextState;
            if (statesRemaining == 1)
            {
                statesRemaining--;
                VisualStateManager.GoToState(this, transitionInName, false);
            }
        }
    }
}
