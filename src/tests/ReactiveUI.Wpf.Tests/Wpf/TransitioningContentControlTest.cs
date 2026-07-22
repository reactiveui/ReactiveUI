// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for <see cref="TransitioningContentControl"/>.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class TransitioningContentControlTest
{
    /// <summary>The number of values in the <see cref="TransitioningContentControl.TransitionType"/> enumeration.</summary>
    private const int TransitionTypeCount = 5;

    /// <summary>The number of values in the transition direction enumeration.</summary>
    private const int TransitionDirectionCount = 4;

    /// <summary>The DPI scale used when overriding the control's DPI for tests.</summary>
    private const double OverriddenDpiScale = 1.25;

    /// <summary>Half a second, expressed in seconds.</summary>
    private const double HalfSecond = 0.5;

    /// <summary>One second, expressed in seconds.</summary>
    private const double OneSecond = 1.0;

    /// <summary>The default transition duration, in seconds.</summary>
    private const double DefaultDurationSeconds = 0.3;

    /// <summary>The multiplier used to negate a value.</summary>
    private const int NegativeSign = -1;

    /// <summary>The width and height applied to the control under test.</summary>
    private const double ControlSize = 100;

    /// <summary>The width applied to button content.</summary>
    private const double ButtonWidth = 100;

    /// <summary>The height applied to button content.</summary>
    private const double ButtonHeight = 50;

    /// <summary>The template part name of the container element.</summary>
    private const string PartContainerName = "PART_Container";

    /// <summary>The template part name of the current content presentation site.</summary>
    private const string PartCurrentContentName = "PART_CurrentContentPresentationSite";

    /// <summary>The template part name of the previous image site.</summary>
    private const string PartPreviousImageName = "PART_PreviousImageSite";

    /// <summary>The name of the visual state group containing presentation states.</summary>
    private const string PresentationStatesName = "PresentationStates";

    /// <summary>The name of the fade transition visual state.</summary>
    private const string TransitionFadeName = "Transition_Fade";

    /// <summary>The text used to represent newly presented content.</summary>
    private const string NewContentText = "New Content";

    /// <summary>An out-of-range value used to exercise the default arms of the transition switch expressions.</summary>
    private const int InvalidEnumValue = 999;

    /// <summary>The maximum number of dispatcher pump iterations while waiting for a storyboard to complete.</summary>
    private const int DispatcherPumpCount = 50;

    /// <summary>The delay, in milliseconds, between dispatcher pump iterations.</summary>
    private const int PumpDelayMs = 10;

    /// <summary>Tests that Transition property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Transition_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Fade
        };

        await Assert.That(control.Transition).IsEqualTo(TransitioningContentControl.TransitionType.Fade);

        control.Transition = TransitioningContentControl.TransitionType.Move;

        await Assert.That(control.Transition).IsEqualTo(TransitioningContentControl.TransitionType.Move);
    }

    /// <summary>Tests that Direction property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Direction_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl
        {
            Direction = TransitioningContentControl.TransitionDirection.Left
        };

        await Assert.That(control.Direction).IsEqualTo(TransitioningContentControl.TransitionDirection.Left);

        control.Direction = TransitioningContentControl.TransitionDirection.Right;

        await Assert.That(control.Direction).IsEqualTo(TransitioningContentControl.TransitionDirection.Right);
    }

    /// <summary>Tests that Duration property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Duration_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl
        {
            Duration = TimeSpan.FromSeconds(HalfSecond)
        };

        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));

        control.Duration = TimeSpan.FromSeconds(OneSecond);

        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(OneSecond));
    }

    /// <summary>Tests that all transition types are supported.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Transition_AllTypes_CanBeSet()
    {
        var control = new TransitioningContentControl();
        var types = new[]
        {
            TransitioningContentControl.TransitionType.Fade,
            TransitioningContentControl.TransitionType.Move,
            TransitioningContentControl.TransitionType.Slide,
            TransitioningContentControl.TransitionType.Drop,
            TransitioningContentControl.TransitionType.Bounce
        };

        foreach (var type in types)
        {
            control.Transition = type;
            await Assert.That(control.Transition).IsEqualTo(type);
        }
    }

    /// <summary>Tests that all transition directions are supported.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Direction_AllDirections_CanBeSet()
    {
        var control = new TransitioningContentControl();
        var directions = new[]
        {
            TransitioningContentControl.TransitionDirection.Up,
            TransitioningContentControl.TransitionDirection.Down,
            TransitioningContentControl.TransitionDirection.Left,
            TransitioningContentControl.TransitionDirection.Right
        };

        foreach (var direction in directions)
        {
            control.Direction = direction;
            await Assert.That(control.Direction).IsEqualTo(direction);
        }
    }

    /// <summary>Tests that TransitionProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TransitionProperty_IsRegistered() =>
        await Assert.That(TransitioningContentControl.TransitionProperty).IsNotNull();

    /// <summary>Tests that TransitionDirectionProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TransitionDirectionProperty_IsRegistered() =>
        await Assert.That(TransitioningContentControl.TransitionDirectionProperty).IsNotNull();

    /// <summary>Tests that TransitionDurationProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TransitionDurationProperty_IsRegistered() =>
        await Assert.That(TransitioningContentControl.TransitionDurationProperty).IsNotNull();

    /// <summary>Tests that OverrideDpi can be set to true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OverrideDpi_CanBeSet()
    {
        TransitioningContentControl.OverrideDpi = true;
        await Assert.That(TransitioningContentControl.OverrideDpi).IsTrue();

        TransitioningContentControl.OverrideDpi = false;
        await Assert.That(TransitioningContentControl.OverrideDpi).IsFalse();
    }

    /// <summary>Tests that Content property can be set.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Content_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl();
        var content = new TextBlock { Text = "Test Content" };

        control.Content = content;

        await Assert.That(control.Content).IsEqualTo(content);
    }

    /// <summary>Tests that control can be created with default values.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesControlWithDefaults()
    {
        var control = new TransitioningContentControl();

        await Assert.That(control).IsNotNull();
        await Assert.That(control.Transition).IsEqualTo(TransitioningContentControl.TransitionType.Fade);
        await Assert.That(control.Direction).IsEqualTo(TransitioningContentControl.TransitionDirection.Left);
        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(DefaultDurationSeconds));
    }

    /// <summary>Tests that GetDpiScaleForElement returns correct DPI scale without override.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetDpiScaleForElement_WithoutOverride_ReturnsActualDpi()
    {
        TransitioningContentControl.OverrideDpi = false;
        var button = new Button();

        var dpiScale = TransitioningContentControl.GetDpiScaleForElement(button);

        await Assert.That(dpiScale.DpiScaleX).IsGreaterThan(0);
        await Assert.That(dpiScale.DpiScaleY).IsGreaterThan(0);
    }

    /// <summary>Tests that GetDpiScaleForElement returns overridden DPI scale when enabled.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetDpiScaleForElement_WithOverride_ReturnsOverriddenDpi()
    {
        TransitioningContentControl.OverrideDpi = true;
        var button = new Button();

        var dpiScale = TransitioningContentControl.GetDpiScaleForElement(button);

        await Assert.That(dpiScale.DpiScaleX).IsEqualTo(OverriddenDpiScale);
        await Assert.That(dpiScale.DpiScaleY).IsEqualTo(OverriddenDpiScale);

        TransitioningContentControl.OverrideDpi = false;
    }

    /// <summary>Tests that SetFadeTransitionDefaults sets duration on animations.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetFadeTransitionDefaults_WithValidStoryboard_SetsDuration()
    {
        var control = new TransitioningContentControl
        {
            Duration = TimeSpan.FromSeconds(HalfSecond),
            Transition = TransitioningContentControl.TransitionType.Fade
        };

        var storyboard = new Storyboard();
        var animation1 = new DoubleAnimation();
        var animation2 = new DoubleAnimation();
        storyboard.Children.Add(animation1);
        storyboard.Children.Add(animation2);

        control.CompletingTransition = storyboard;

        control.SetFadeTransitionDefaults();

        await Assert.That(animation1.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));
        await Assert.That(animation2.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));
    }

    /// <summary>Tests that SetFadeTransitionDefaults handles null CompletingTransition gracefully.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetFadeTransitionDefaults_WithNullStoryboard_DoesNotThrow()
    {
        var control = new TransitioningContentControl();

        // Should not throw when CompletingTransition is null
        control.SetFadeTransitionDefaults();

        await Task.CompletedTask;
    }

    /// <summary>Tests that SetSlideTransitionDefaults sets correct values based on direction.</summary>
    /// <param name="direction">The transition direction.</param>
    /// <param name="expectedSign">The expected sign of the From value (positive or negative).</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionDirection.Down, NegativeSign)]
    [Arguments(TransitioningContentControl.TransitionDirection.Up, 1)]
    [Arguments(TransitioningContentControl.TransitionDirection.Right, NegativeSign)]
    [Arguments(TransitioningContentControl.TransitionDirection.Left, 1)]
    public async Task SetSlideTransitionDefaults_WithDirection_SetsCorrectFromValue(
        TransitioningContentControl.TransitionDirection direction,
        int expectedSign)
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Slide,
            Direction = direction,
            Duration = TimeSpan.FromSeconds(HalfSecond),
            Width = ControlSize,
            Height = ControlSize
        };

        // Force a measure and arrange to set ActualWidth/Height
        control.Measure(new(ControlSize, ControlSize));
        control.Arrange(new(0, 0, ControlSize, ControlSize));

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation();
        storyboard.Children.Add(animation);

        control.CompletingTransition = storyboard;

        control.SetSlideTransitionDefaults();

        await Assert.That(animation.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));

        var expectedValue = direction is TransitioningContentControl.TransitionDirection.Down or TransitioningContentControl.TransitionDirection.Up
            ? expectedSign * control.ActualHeight
            : expectedSign * control.ActualWidth;

        await Assert.That(animation.From).IsEqualTo(expectedValue);
    }

    /// <summary>Tests that SetMoveTransitionDefaults sets correct values based on direction.</summary>
    /// <param name="direction">The transition direction.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionDirection.Down)]
    [Arguments(TransitioningContentControl.TransitionDirection.Up)]
    [Arguments(TransitioningContentControl.TransitionDirection.Right)]
    [Arguments(TransitioningContentControl.TransitionDirection.Left)]
    public async Task SetMoveTransitionDefaults_WithDirection_SetsCorrectValues(
        TransitioningContentControl.TransitionDirection direction)
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Move,
            Direction = direction,
            Duration = TimeSpan.FromSeconds(HalfSecond),
            Width = ControlSize,
            Height = ControlSize
        };

        control.Measure(new(ControlSize, ControlSize));
        control.Arrange(new(0, 0, ControlSize, ControlSize));

        var storyboard = new Storyboard();
        var completingAnimation = new DoubleAnimation();
        var startingAnimation = new DoubleAnimation();
        storyboard.Children.Add(completingAnimation);
        storyboard.Children.Add(startingAnimation);

        control.CompletingTransition = storyboard;

        control.SetMoveTransitionDefaults();

        await Assert.That(completingAnimation.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));
        await Assert.That(startingAnimation.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));
        await Assert.That(startingAnimation.To).IsNotNull();
        await Assert.That(completingAnimation.From).IsNotNull();
    }

    /// <summary>Tests that SetBounceTransitionDefaults sets correct values for bounce transition.</summary>
    /// <param name="direction">The transition direction.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionDirection.Down)]
    [Arguments(TransitioningContentControl.TransitionDirection.Up)]
    [Arguments(TransitioningContentControl.TransitionDirection.Right)]
    [Arguments(TransitioningContentControl.TransitionDirection.Left)]
    public async Task SetBounceTransitionDefaults_WithDirection_SetsCorrectToValue(
        TransitioningContentControl.TransitionDirection direction)
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Bounce,
            Direction = direction,
            Width = ControlSize,
            Height = ControlSize
        };

        control.Measure(new(ControlSize, ControlSize));
        control.Arrange(new(0, 0, ControlSize, ControlSize));

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation();
        storyboard.Children.Add(animation);

        control.StartingTransition = storyboard;

        control.SetBounceTransitionDefaults();

        await Assert.That(animation.To.HasValue).IsTrue();

        var isVertical = direction is TransitioningContentControl.TransitionDirection.Down or TransitioningContentControl.TransitionDirection.Up;
        var expectedMagnitude = isVertical ? control.ActualHeight : control.ActualWidth;

        await Assert.That(Math.Abs(animation.To!.Value)).IsEqualTo(expectedMagnitude);
    }

    /// <summary>Tests that ConfigureStandardTransition returns correct transition name for Fade.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ConfigureStandardTransition_WithFadeType_ReturnsCorrectName()
    {
        var control = CreateControlWithTemplate();
        control.Transition = TransitioningContentControl.TransitionType.Fade;

        var transitionName = control.ConfigureStandardTransition();

        await Assert.That(transitionName).IsEqualTo(TransitionFadeName);
    }

    /// <summary>Tests that ConfigureStandardTransition returns correct transition name for non-Fade types.</summary>
    /// <param name="transitionType">The transition type.</param>
    /// <param name="direction">The transition direction.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionType.Move, TransitioningContentControl.TransitionDirection.Left)]
    [Arguments(TransitioningContentControl.TransitionType.Slide, TransitioningContentControl.TransitionDirection.Right)]
    [Arguments(TransitioningContentControl.TransitionType.Drop, TransitioningContentControl.TransitionDirection.Up)]
    public async Task ConfigureStandardTransition_WithNonFadeType_ReturnsCorrectName(
        TransitioningContentControl.TransitionType transitionType,
        TransitioningContentControl.TransitionDirection direction)
    {
        var control = CreateControlWithTemplate();
        control.Transition = transitionType;
        control.Direction = direction;

        var transitionName = control.ConfigureStandardTransition();

        var expectedName = $"Transition_{transitionType}{direction}";
        await Assert.That(transitionName).IsEqualTo(expectedName);
    }

    /// <summary>Tests that ConfigureBounceTransition returns correct transition names.</summary>
    /// <param name="direction">The transition direction.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionDirection.Left)]
    [Arguments(TransitioningContentControl.TransitionDirection.Right)]
    [Arguments(TransitioningContentControl.TransitionDirection.Up)]
    [Arguments(TransitioningContentControl.TransitionDirection.Down)]
    public async Task ConfigureBounceTransition_WithDirection_ReturnsCorrectNames(
        TransitioningContentControl.TransitionDirection direction)
    {
        var control = CreateControlWithTemplate();
        control.Transition = TransitioningContentControl.TransitionType.Bounce;
        control.Direction = direction;

        var (startingName, completingName) = control.ConfigureBounceTransition();

        var expectedStartingName = $"Transition_Bounce{direction}Out";
        var expectedCompletingName = $"Transition_Bounce{direction}In";

        await Assert.That(startingName).IsEqualTo(expectedStartingName);
        await Assert.That(completingName).IsEqualTo(expectedCompletingName);
    }

    /// <summary>Tests that PrepareTransitionImages sets content on the content presenter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task PrepareTransitionImages_WithNewContent_SetsContentPresenterContent()
    {
        var control = CreateControlWithTemplateParts();
        var newContent = new TextBlock { Text = NewContentText };

        control.PrepareTransitionImages(newContent);

        await Assert.That(control.CurrentContentPresentationSite?.Content).IsEqualTo(newContent);
    }

    /// <summary>Tests that GetRenderTargetBitmapFromUiElement returns default when element has zero size.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetRenderTargetBitmapFromUiElement_WithZeroSize_ReturnsDefault()
    {
        var button = new Button();

        var bitmap = TransitioningContentControl.GetRenderTargetBitmapFromUiElement(button);

        await Assert.That(bitmap).IsNull();
    }

    /// <summary>Tests that GetRenderTargetBitmapFromUiElement creates bitmap for rendered element.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetRenderTargetBitmapFromUiElement_WithRenderedElement_CreatesBitmap()
    {
        var button = new Button
        {
            Width = ButtonWidth,
            Height = ButtonHeight,
            Content = "Test"
        };

        button.Measure(new(ButtonWidth, ButtonHeight));
        button.Arrange(new(0, 0, ButtonWidth, ButtonHeight));

        TransitioningContentControl.OverrideDpi = true;
        var bitmap = TransitioningContentControl.GetRenderTargetBitmapFromUiElement(button);
        TransitioningContentControl.OverrideDpi = false;

        await Assert.That(bitmap).IsNotNull();
        await Assert.That(bitmap.PixelWidth).IsGreaterThan(0);
        await Assert.That(bitmap.PixelHeight).IsGreaterThan(0);
    }

    /// <summary>Tests that GetTransitionStoryboardByName throws when transition name is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetTransitionStoryboardByName_WithNullName_ThrowsArgumentNullException()
    {
        var control = CreateControlWithTemplate();

        await Assert.That(() => control.GetTransitionStoryboardByName(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that GetTransitionStoryboardByName throws when transition name is empty.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetTransitionStoryboardByName_WithEmptyName_ThrowsArgumentException()
    {
        var control = CreateControlWithTemplate();

        await Assert.That(() => control.GetTransitionStoryboardByName(string.Empty)).Throws<ArgumentException>();
    }

    /// <summary>Tests that GetTransitionStoryboardByName throws when visual state group is not initialized.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetTransitionStoryboardByName_WithoutVisualStateGroup_ThrowsInvalidOperationException()
    {
        var control = new TransitioningContentControl();

        await Assert.That(() => control.GetTransitionStoryboardByName(TransitionFadeName))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Tests that GetTransitionStoryboardByName throws when transition is not found.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetTransitionStoryboardByName_WithInvalidTransition_ThrowsInvalidOperationException()
    {
        var control = CreateControlWithTemplate();

        await Assert.That(() => control.GetTransitionStoryboardByName("NonExistentTransition"))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Tests that GetTransitionStoryboardByName returns storyboard for valid transition.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetTransitionStoryboardByName_WithValidTransition_ReturnsStoryboard()
    {
        var control = CreateControlWithTemplate();

        var storyboard = control.GetTransitionStoryboardByName(TransitionFadeName);

        await Assert.That(storyboard).IsNotNull();
    }

    /// <summary>Tests that SetTransitionDefaultValues calls correct method for each transition type.</summary>
    /// <param name="transitionType">The transition type.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionType.Fade)]
    [Arguments(TransitioningContentControl.TransitionType.Slide)]
    [Arguments(TransitioningContentControl.TransitionType.Move)]
    [Arguments(TransitioningContentControl.TransitionType.Bounce)]
    [Arguments(TransitioningContentControl.TransitionType.Drop)]
    public async Task SetTransitionDefaultValues_WithTransitionType_DoesNotThrow(
        TransitioningContentControl.TransitionType transitionType)
    {
        var control = new TransitioningContentControl
        {
            Transition = transitionType,
            Width = ControlSize,
            Height = ControlSize
        };

        control.Measure(new(ControlSize, ControlSize));
        control.Arrange(new(0, 0, ControlSize, ControlSize));

        // Should not throw even without storyboards set (methods check for null)
        control.SetTransitionDefaultValues();

        await Task.CompletedTask;
    }

    /// <summary>Tests that OnApplyTemplate throws when PART_Container is missing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnApplyTemplate_WithoutContainer_ThrowsInvalidOperationException()
    {
        var control = new TransitioningContentControl
        {
            Template = new(typeof(TransitioningContentControl))
            {
                VisualTree = new(typeof(Grid)) { Name = "WrongName" },
            },
        };

        await Assert.That(control.ApplyTemplate)
            .Throws<InvalidOperationException>();
    }

    /// <summary>Tests that OnApplyTemplate throws when PART_CurrentContentPresentationSite is missing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnApplyTemplate_WithoutContentPresenter_ThrowsInvalidOperationException()
    {
        var control = new TransitioningContentControl
        {
            Template = new(typeof(TransitioningContentControl))
            {
                VisualTree = new(typeof(Grid)) { Name = PartContainerName },
            },
        };

        await Assert.That(control.ApplyTemplate)
            .Throws<InvalidOperationException>();
    }

    /// <summary>Tests that control can apply template successfully with all required parts.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnApplyTemplate_WithAllParts_Succeeds()
    {
        var control = CreateControlWithTemplateParts();

        await Assert.That(control.CurrentContentPresentationSite).IsNotNull();
    }

    /// <summary>Tests that enum values have correct count.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TransitionType_Enum_HasExpectedValues()
    {
        var values = Enum.GetValues<TransitioningContentControl.TransitionType>();

        await Assert.That(values.Length).IsEqualTo(TransitionTypeCount);
    }

    /// <summary>Tests that direction enum has correct count.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TransitionDirection_Enum_HasExpectedValues()
    {
        var values = Enum.GetValues<TransitioningContentControl.TransitionDirection>();

        await Assert.That(values.Length).IsEqualTo(TransitionDirectionCount);
    }

    /// <summary>Tests that setting CompletingTransition triggers SetTransitionDefaultValues.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompletingTransition_Set_TriggersSetTransitionDefaultValues()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Fade,
            Duration = TimeSpan.FromSeconds(HalfSecond)
        };

        var storyboard = new Storyboard();
        var animation1 = new DoubleAnimation();
        var animation2 = new DoubleAnimation();
        storyboard.Children.Add(animation1);
        storyboard.Children.Add(animation2);

        control.CompletingTransition = storyboard;

        // SetTransitionDefaultValues should have been called, setting the durations
        await Assert.That(animation1.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));
        await Assert.That(animation2.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(HalfSecond));
    }

    /// <summary>Tests that setting StartingTransition triggers SetTransitionDefaultValues.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task StartingTransition_Set_TriggersSetTransitionDefaultValues()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Bounce,
            Direction = TransitioningContentControl.TransitionDirection.Down,
            Width = ControlSize,
            Height = ControlSize
        };

        control.Measure(new(ControlSize, ControlSize));
        control.Arrange(new(0, 0, ControlSize, ControlSize));

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation();
        storyboard.Children.Add(animation);

        control.StartingTransition = storyboard;

        // SetTransitionDefaultValues should have been called
        await Assert.That(animation.To).IsNotNull();
    }

    /// <summary>Tests that PrepareTransitionImages captures the current content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task PrepareTransitionImages_CapturesCurrentContent()
    {
        var control = CreateControlWithTemplate();
        control.Width = ControlSize;
        control.Height = ControlSize;
        control.Measure(new(ControlSize, ControlSize));
        control.Arrange(new(0, 0, ControlSize, ControlSize));

        // Set initial content
        if (control.CurrentContentPresentationSite is not null)
        {
            control.CurrentContentPresentationSite.Content = "Old Content";
        }

        // Prepare transition with new content
        control.PrepareTransitionImages(NewContentText);

        // Should update to new content
        await Assert.That(control.CurrentContentPresentationSite!.Content).IsEqualTo(NewContentText);
    }

    /// <summary>Tests that ConfigureBounceTransition sets both transitions.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ConfigureBounceTransition_SetsBothTransitions()
    {
        var control = CreateControlWithTemplate();
        control.Transition = TransitioningContentControl.TransitionType.Bounce;
        control.Direction = TransitioningContentControl.TransitionDirection.Down;

        var (startingName, completingName) = control.ConfigureBounceTransition();

        await Assert.That(startingName).IsEqualTo("Transition_BounceDownOut");
        await Assert.That(completingName).IsEqualTo("Transition_BounceDownIn");
        await Assert.That(control.StartingTransition).IsNotNull();
        await Assert.That(control.CompletingTransition).IsNotNull();
    }

    /// <summary>Tests that ConfigureStandardTransition returns correct name for different transition types.</summary>
    /// <param name="transitionType">The transition type.</param>
    /// <param name="direction">The transition direction.</param>
    /// <param name="expectedName">The expected transition name.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionType.Fade, TransitioningContentControl.TransitionDirection.Left, TransitionFadeName)]
    [Arguments(TransitioningContentControl.TransitionType.Move, TransitioningContentControl.TransitionDirection.Right, "Transition_MoveRight")]
    [Arguments(TransitioningContentControl.TransitionType.Slide, TransitioningContentControl.TransitionDirection.Up, "Transition_SlideUp")]
    [Arguments(TransitioningContentControl.TransitionType.Drop, TransitioningContentControl.TransitionDirection.Down, "Transition_DropDown")]
    public async Task ConfigureStandardTransition_ReturnsCorrectName(
        TransitioningContentControl.TransitionType transitionType,
        TransitioningContentControl.TransitionDirection direction,
        string expectedName)
    {
        var control = CreateControlWithTemplate();
        control.Transition = transitionType;
        control.Direction = direction;

        var transitionName = control.ConfigureStandardTransition();

        await Assert.That(transitionName).IsEqualTo(expectedName);
    }

    /// <summary>Tests PrepareTransitionImages with zero-size element.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task PrepareTransitionImages_WithZeroSizeElement_HandlesGracefully()
    {
        var control = CreateControlWithTemplate();

        // Don't set size - ActualWidth/Height will be 0
        // Should not throw even with zero size
        control.PrepareTransitionImages(NewContentText);

        await Assert.That(control.CurrentContentPresentationSite!.Content).IsEqualTo(NewContentText);
    }

    /// <summary>An invalid direction makes SetSlideTransitionDefaults throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetSlideTransitionDefaults_WithInvalidDirection_Throws()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Slide,
            Direction = TransitioningContentControl.TransitionDirection.Left,
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(new DoubleAnimation());

        // Assign with a valid direction (the setter eagerly applies defaults), then corrupt the direction and invoke
        // the method directly so the switch expression hits its default arm.
        control.CompletingTransition = storyboard;
        control.Direction = (TransitioningContentControl.TransitionDirection)InvalidEnumValue;

        await Assert.That(control.SetSlideTransitionDefaults).Throws<InvalidOperationException>();
    }

    /// <summary>An invalid direction makes SetMoveTransitionDefaults throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetMoveTransitionDefaults_WithInvalidDirection_Throws()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Move,
            Direction = TransitioningContentControl.TransitionDirection.Left,
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(new DoubleAnimation());
        storyboard.Children.Add(new DoubleAnimation());
        control.CompletingTransition = storyboard;
        control.Direction = (TransitioningContentControl.TransitionDirection)InvalidEnumValue;

        await Assert.That(control.SetMoveTransitionDefaults).Throws<InvalidOperationException>();
    }

    /// <summary>An invalid direction makes SetBounceTransitionDefaults throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetBounceTransitionDefaults_WithInvalidDirection_Throws()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Bounce,
            Direction = TransitioningContentControl.TransitionDirection.Left,
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(new DoubleAnimation());
        control.StartingTransition = storyboard;
        control.Direction = (TransitioningContentControl.TransitionDirection)InvalidEnumValue;

        await Assert.That(control.SetBounceTransitionDefaults).Throws<InvalidOperationException>();
    }

    /// <summary>An invalid transition type makes SetTransitionDefaultValues throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetTransitionDefaultValues_WithInvalidType_Throws()
    {
        var control = new TransitioningContentControl
        {
            Transition = (TransitioningContentControl.TransitionType)InvalidEnumValue,
        };

        await Assert.That(control.SetTransitionDefaultValues).Throws<InvalidOperationException>();
    }

    /// <summary>Setting CompletingTransition a second time unhooks the previous storyboard's completion handler.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompletingTransition_SetTwice_DecouplesPreviousTransition()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Fade,
            Duration = TimeSpan.FromSeconds(HalfSecond),
        };

        var first = new Storyboard();
        first.Children.Add(new DoubleAnimation());
        first.Children.Add(new DoubleAnimation());
        control.CompletingTransition = first;

        var second = new Storyboard();
        second.Children.Add(new DoubleAnimation());
        second.Children.Add(new DoubleAnimation());

        // Setting again triggers the decouple branch on the previously assigned transition.
        control.CompletingTransition = second;

        await Assert.That(control.CompletingTransition).IsSameReferenceAs(second);
    }

    /// <summary>Setting CompletingTransition back to null leaves it null after decoupling.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompletingTransition_SetToNull_Decouples()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Fade,
            Duration = TimeSpan.FromSeconds(HalfSecond),
        };

        var first = new Storyboard();
        first.Children.Add(new DoubleAnimation());
        first.Children.Add(new DoubleAnimation());
        control.CompletingTransition = first;

        control.CompletingTransition = null;

        await Assert.That(control.CompletingTransition).IsNull();
    }

    /// <summary>
    /// Changing content on a realized control configured for a standard (Fade) transition drives the full transition
    /// pipeline: it snapshots the old content, swaps in the new content, raises <c>TransitionStarted</c> and enters the
    /// transition visual state.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnContentChanged_FadeTransition_RunsStandardTransition()
    {
        var control = CreateRealizedControl();
        control.Transition = TransitioningContentControl.TransitionType.Fade;

        var started = false;
        control.TransitionStarted += (_, _) => started = true;

        // The synthetic visual-state storyboards have no animation targets, so WPF's VisualStateManager.GoToState (the
        // final step of QueueTransition) fails internally. By then the whole transition body has already run and
        // TransitionStarted has fired, which is what this test exercises.
        ChangeContentDrivingTransition(control, new TextBlock { Text = NewContentText });

        using (Assert.Multiple())
        {
            await Assert.That(started).IsTrue();
            await Assert.That(control.CurrentContentPresentationSite!.Content).IsTypeOf<TextBlock>();
        }
    }

    /// <summary>Changing content on a realized control configured for Bounce runs the two-phase bounce transition.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnContentChanged_BounceTransition_RunsBounceTransition()
    {
        var control = CreateRealizedControl();
        control.Transition = TransitioningContentControl.TransitionType.Bounce;
        control.Direction = TransitioningContentControl.TransitionDirection.Down;

        var started = false;
        control.TransitionStarted += (_, _) => started = true;

        ChangeContentDrivingTransition(control, new TextBlock { Text = NewContentText });

        using (Assert.Multiple())
        {
            await Assert.That(started).IsTrue();
            await Assert.That(control.StartingTransition).IsNotNull();
            await Assert.That(control.CompletingTransition).IsNotNull();
        }
    }

    /// <summary>
    /// A second content change while a transition is already in progress updates the current content immediately
    /// without queueing another animation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnContentChanged_WhileTransitioning_UpdatesContentImmediately()
    {
        var control = CreateRealizedControl();
        control.Transition = TransitioningContentControl.TransitionType.Fade;

        // First change starts a transition (sets _isTransitioning = true) before the synthetic storyboard fails inside
        // GoToState, so the control is left mid-transition.
        ChangeContentDrivingTransition(control, new TextBlock { Text = "first" });

        // Second change while transitioning falls into the immediate-update branch (no animation queued).
        var secondContent = new TextBlock { Text = "second" };
        control.Content = secondContent;

        await Assert.That(control.CurrentContentPresentationSite!.Content).IsSameReferenceAs(secondContent);
    }

    /// <summary>When the template has no previous-image part, a content change falls back to a plain content swap.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnContentChanged_WithoutPreviousImageSite_UpdatesContentImmediately()
    {
        var control = CreateRealizedControl(includePreviousImage: false);

        var newContent = new TextBlock { Text = NewContentText };
        control.Content = newContent;

        await Assert.That(control.CurrentContentPresentationSite!.Content).IsSameReferenceAs(newContent);
    }

    /// <summary>Before the template is applied, a content change is a no-op on the (absent) content presenter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnContentChanged_WithoutTemplate_DoesNotThrow()
    {
        var control = new TransitioningContentControl
        {
            Content = new TextBlock { Text = NewContentText },
        };

        await Assert.That(control.CurrentContentPresentationSite).IsNull();
    }

    /// <summary>
    /// When the completing storyboard runs to completion, the control aborts the transition (returns to Normal, clears
    /// the previous-image snapshot) and raises <c>TransitionCompleted</c>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompletingTransition_Completes_AbortsTransitionAndRaisesCompleted()
    {
        var control = CreateRealizedControl();
        control.Transition = TransitioningContentControl.TransitionType.Fade;

        // Seed a previous-image snapshot so AbortTransition exercises its render-target clearing branch.
        control.PrepareTransitionImages(new TextBlock { Text = "snapshot" });

        // A real, zero-duration storyboard targeting an existing element so it actually completes and fires Completed.
        // Two children are required because the Fade defaults read Children[0] and Children[1].
        var storyboard = new Storyboard();
        var opacityAnimation = new DoubleAnimation(1.0, 1.0, new Duration(TimeSpan.Zero));
        Storyboard.SetTarget(opacityAnimation, control);
        Storyboard.SetTargetProperty(opacityAnimation, new(UIElement.OpacityProperty));
        storyboard.Children.Add(opacityAnimation);

        var secondAnimation = new DoubleAnimation(1.0, 1.0, new Duration(TimeSpan.Zero));
        Storyboard.SetTarget(secondAnimation, control);
        Storyboard.SetTargetProperty(secondAnimation, new(UIElement.OpacityProperty));
        storyboard.Children.Add(secondAnimation);

        var completed = false;
        control.TransitionCompleted += (_, _) => completed = true;

        // Assigning CompletingTransition hooks OnTransitionCompleted onto the storyboard's Completed event.
        control.CompletingTransition = storyboard;

        storyboard.Begin(control, true);

        // Pump the dispatcher so the zero-length animation completes and fires its Completed callback.
        for (var i = 0; i < DispatcherPumpCount && !completed; i++)
        {
            Tests.Xaml.Utilities.DispatcherUtilities.DoEvents();
            await Task.Delay(PumpDelayMs);
        }

        await Assert.That(completed).IsTrue();
    }

    /// <summary>
    /// Assigns new content to a realized control to drive its transition pipeline, tolerating the WPF-internal failure
    /// raised by <c>VisualStateManager.GoToState</c> when the synthetic visual-state storyboards lack animation targets.
    /// The transition body (snapshot, content swap, state bookkeeping and the <c>TransitionStarted</c> event) runs
    /// before that failure.
    /// </summary>
    /// <param name="control">The realized control under test.</param>
    /// <param name="newContent">The new content to assign.</param>
    private static void ChangeContentDrivingTransition(TransitioningContentControl control, object newContent)
    {
        try
        {
            control.Content = newContent;
        }
        catch (SystemException)
        {
            // The exception originates inside WPF's VisualStateManager when flattening target-less storyboards; the
            // control's own transition logic has already executed by this point. VisualStateManager raises either a
            // NullReferenceException or an InvalidOperationException here depending on the WPF build, and both derive
            // from SystemException; catching the shared base tolerates the internal failure without singling one out.
        }
    }

    /// <summary>
    /// Creates a fully realized control: full template, presentation visual states, non-zero layout size and initial
    /// content, so that a subsequent content change drives the real transition pipeline.
    /// </summary>
    /// <param name="includePreviousImage">Whether to include the optional previous-image template part.</param>
    /// <returns>A realized <see cref="TransitioningContentControl"/>.</returns>
    private static TransitioningContentControl CreateRealizedControl(bool includePreviousImage = true)
    {
        var control = new TransitioningContentControl
        {
            Width = ControlSize,
            Height = ControlSize,
            Content = new TextBlock { Text = "initial" },
        };

        var template = new ControlTemplate(typeof(TransitioningContentControl));
        var grid = new FrameworkElementFactory(typeof(Grid)) { Name = PartContainerName };

        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter)) { Name = PartCurrentContentName };
        grid.AppendChild(contentPresenter);

        if (includePreviousImage)
        {
            var image = new FrameworkElementFactory(typeof(Image)) { Name = PartPreviousImageName };
            grid.AppendChild(image);
        }

        template.VisualTree = grid;
        control.Template = template;
        _ = control.ApplyTemplate();

        if (control.Template.FindName(PartContainerName, control) is Grid container)
        {
            var stateGroup = new VisualStateGroup { Name = PresentationStatesName };
            AddTransitionStates(stateGroup);
            var groups = VisualStateManager.GetVisualStateGroups(container);
            _ = groups.Add(stateGroup);
            control.PresentationStateGroup = groups.OfType<VisualStateGroup>().FirstOrDefault(static g => g.Name == PresentationStatesName);
        }

        control.Measure(new(ControlSize, ControlSize));
        control.Arrange(new(0, 0, ControlSize, ControlSize));
        control.UpdateLayout();

        return control;
    }

    /// <summary>Creates a control with a minimal template for testing.</summary>
    /// <returns>A configured TransitioningContentControl.</returns>
    private static TransitioningContentControl CreateControlWithTemplate()
    {
        var control = new TransitioningContentControl();

        // Create a minimal template with visual state groups
        var template = new ControlTemplate(typeof(TransitioningContentControl));
        var grid = new FrameworkElementFactory(typeof(Grid)) { Name = PartContainerName };

        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter)) { Name = PartCurrentContentName };
        grid.AppendChild(contentPresenter);

        var image = new FrameworkElementFactory(typeof(Image)) { Name = PartPreviousImageName };
        grid.AppendChild(image);

        template.VisualTree = grid;
        control.Template = template;
        _ = control.ApplyTemplate();

        // Set up visual state groups AFTER applying template
        if (control.Template.FindName(PartContainerName, control) is Grid container)
        {
            var stateGroup = new VisualStateGroup { Name = PresentationStatesName };

            // Add states for all transition types
            AddTransitionStates(stateGroup);

            var groups = VisualStateManager.GetVisualStateGroups(container);
            _ = groups.Add(stateGroup);

            // Manually update the PresentationStateGroup property since OnApplyTemplate already ran
            // Get the first group with name "PresentationStates"
            control.PresentationStateGroup = groups.OfType<VisualStateGroup>().FirstOrDefault(static g => g.Name == PresentationStatesName);
        }

        return control;
    }

    /// <summary>Creates a control with template parts set up for testing.</summary>
    /// <returns>A configured TransitioningContentControl.</returns>
    private static TransitioningContentControl CreateControlWithTemplateParts()
    {
        var control = new TransitioningContentControl();

        var template = new ControlTemplate(typeof(TransitioningContentControl));
        var grid = new FrameworkElementFactory(typeof(Grid)) { Name = PartContainerName };

        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter)) { Name = PartCurrentContentName };
        grid.AppendChild(contentPresenter);

        var image = new FrameworkElementFactory(typeof(Image)) { Name = PartPreviousImageName };
        grid.AppendChild(image);

        template.VisualTree = grid;
        control.Template = template;
        _ = control.ApplyTemplate();

        return control;
    }

    /// <summary>Adds all required transition states to a visual state group.</summary>
    /// <param name="stateGroup">The visual state group.</param>
    private static void AddTransitionStates(VisualStateGroup stateGroup)
    {
        // Normal state
        _ = stateGroup.States.Add(new VisualState { Name = "Normal", Storyboard = new() });

        // Fade transition
        var fadeStoryboard = new Storyboard();
        fadeStoryboard.Children.Add(new DoubleAnimation());
        fadeStoryboard.Children.Add(new DoubleAnimation());
        _ = stateGroup.States.Add(new VisualState { Name = TransitionFadeName, Storyboard = fadeStoryboard });

        // Add states for each direction and transition type
        var directions = new[] { "Left", "Right", "Up", "Down" };
        foreach (var transition in new[] { "Move", "Slide", "Drop" })
        {
            foreach (var direction in directions)
            {
                var storyboard = new Storyboard();
                storyboard.Children.Add(new DoubleAnimation());
                if (transition == "Move")
                {
                    storyboard.Children.Add(new DoubleAnimation());
                }

                _ = stateGroup.States.Add(new VisualState { Name = $"Transition_{transition}{direction}", Storyboard = storyboard });
            }
        }

        // Bounce transitions (need both In and Out)
        foreach (var direction in directions)
        {
            var bounceOutStoryboard = new Storyboard();
            bounceOutStoryboard.Children.Add(new DoubleAnimation());
            _ = stateGroup.States.Add(new VisualState { Name = $"Transition_Bounce{direction}Out", Storyboard = bounceOutStoryboard });

            var bounceInStoryboard = new Storyboard();
            var keyFrameAnimation = new DoubleAnimationUsingKeyFrames();
            _ = keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame());
            _ = keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame());
            bounceInStoryboard.Children.Add(keyFrameAnimation);
            _ = stateGroup.States.Add(new VisualState { Name = $"Transition_Bounce{direction}In", Storyboard = bounceInStoryboard });
        }
    }
}
