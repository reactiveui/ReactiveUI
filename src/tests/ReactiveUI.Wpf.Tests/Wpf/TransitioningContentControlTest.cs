// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="TransitioningContentControl"/>.
/// </summary>
[NotInParallel]
public class TransitioningContentControlTest
{
    private WpfAppBuilderScope? _appBuilderScope;

    /// <summary>
    /// Sets up the WPF app builder scope for each test.
    /// </summary>
    [Before(Test)]
    public void Setup()
    {
        _appBuilderScope = new WpfAppBuilderScope();
    }

    /// <summary>
    /// Tears down the WPF app builder scope after each test.
    /// </summary>
    [After(Test)]
    public void TearDown()
    {
        _appBuilderScope?.Dispose();
    }

    /// <summary>
    /// Tests that Transition property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    /// <summary>
    /// Tests that Direction property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    /// <summary>
    /// Tests that Duration property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Duration_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl
        {
            Duration = TimeSpan.FromSeconds(0.5)
        };

        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(0.5));

        control.Duration = TimeSpan.FromSeconds(1.0);

        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(1.0));
    }

    /// <summary>
    /// Tests that all transition types are supported.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    /// <summary>
    /// Tests that all transition directions are supported.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    /// <summary>
    /// Tests that TransitionProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionProperty_IsRegistered()
    {
        await Assert.That(TransitioningContentControl.TransitionProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that TransitionDirectionProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionDirectionProperty_IsRegistered()
    {
        await Assert.That(TransitioningContentControl.TransitionDirectionProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that TransitionDurationProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionDurationProperty_IsRegistered()
    {
        await Assert.That(TransitioningContentControl.TransitionDurationProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that OverrideDpi can be set to true.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task OverrideDpi_CanBeSet()
    {
        TransitioningContentControl.OverrideDpi = true;
        await Assert.That(TransitioningContentControl.OverrideDpi).IsTrue();

        TransitioningContentControl.OverrideDpi = false;
        await Assert.That(TransitioningContentControl.OverrideDpi).IsFalse();
    }

    /// <summary>
    /// Tests that Content property can be set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Content_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl();
        var content = new TextBlock { Text = "Test Content" };

        control.Content = content;

        await Assert.That(control.Content).IsEqualTo(content);
    }

    /// <summary>
    /// Tests that control can be created with default values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Constructor_CreatesControlWithDefaults()
    {
        var control = new TransitioningContentControl();

        await Assert.That(control).IsNotNull();
        await Assert.That(control.Transition).IsEqualTo(TransitioningContentControl.TransitionType.Fade);
        await Assert.That(control.Direction).IsEqualTo(TransitioningContentControl.TransitionDirection.Left);
        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(0.3));
    }

    /// <summary>
    /// Tests that GetDpiScaleForElement returns correct DPI scale without override.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetDpiScaleForElement_WithoutOverride_ReturnsActualDpi()
    {
        TransitioningContentControl.OverrideDpi = false;
        var button = new Button();

        var dpiScale = TransitioningContentControl.GetDpiScaleForElement(button);

        await Assert.That(dpiScale.DpiScaleX).IsGreaterThan(0);
        await Assert.That(dpiScale.DpiScaleY).IsGreaterThan(0);
    }

    /// <summary>
    /// Tests that GetDpiScaleForElement returns overridden DPI scale when enabled.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetDpiScaleForElement_WithOverride_ReturnsOverriddenDpi()
    {
        TransitioningContentControl.OverrideDpi = true;
        var button = new Button();

        var dpiScale = TransitioningContentControl.GetDpiScaleForElement(button);

        await Assert.That(dpiScale.DpiScaleX).IsEqualTo(1.25);
        await Assert.That(dpiScale.DpiScaleY).IsEqualTo(1.25);

        TransitioningContentControl.OverrideDpi = false;
    }

    /// <summary>
    /// Tests that SetFadeTransitionDefaults sets duration on animations.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task SetFadeTransitionDefaults_WithValidStoryboard_SetsDuration()
    {
        var control = new TransitioningContentControl
        {
            Duration = TimeSpan.FromSeconds(0.5),
            Transition = TransitioningContentControl.TransitionType.Fade
        };

        var storyboard = new Storyboard();
        var animation1 = new DoubleAnimation();
        var animation2 = new DoubleAnimation();
        storyboard.Children.Add(animation1);
        storyboard.Children.Add(animation2);

        control.CompletingTransition = storyboard;

        control.SetFadeTransitionDefaults();

        await Assert.That(animation1.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(0.5));
        await Assert.That(animation2.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(0.5));
    }

    /// <summary>
    /// Tests that SetFadeTransitionDefaults handles null CompletingTransition gracefully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task SetFadeTransitionDefaults_WithNullStoryboard_DoesNotThrow()
    {
        var control = new TransitioningContentControl();

        // Should not throw when CompletingTransition is null
        control.SetFadeTransitionDefaults();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Tests that SetSlideTransitionDefaults sets correct values based on direction.
    /// </summary>
    /// <param name="direction">The transition direction.</param>
    /// <param name="expectedSign">The expected sign of the From value (positive or negative).</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionDirection.Down, -1)]
    [Arguments(TransitioningContentControl.TransitionDirection.Up, 1)]
    [Arguments(TransitioningContentControl.TransitionDirection.Right, -1)]
    [Arguments(TransitioningContentControl.TransitionDirection.Left, 1)]
    public async Task SetSlideTransitionDefaults_WithDirection_SetsCorrectFromValue(
        TransitioningContentControl.TransitionDirection direction,
        int expectedSign)
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Slide,
            Direction = direction,
            Duration = TimeSpan.FromSeconds(0.5),
            Width = 100,
            Height = 100
        };

        // Force a measure and arrange to set ActualWidth/Height
        control.Measure(new Size(100, 100));
        control.Arrange(new Rect(0, 0, 100, 100));

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation();
        storyboard.Children.Add(animation);

        control.CompletingTransition = storyboard;

        control.SetSlideTransitionDefaults();

        await Assert.That(animation.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(0.5));

        var expectedValue = direction == TransitioningContentControl.TransitionDirection.Down || direction == TransitioningContentControl.TransitionDirection.Up
            ? expectedSign * control.ActualHeight
            : expectedSign * control.ActualWidth;

        await Assert.That(animation.From).IsEqualTo(expectedValue);
    }

    /// <summary>
    /// Tests that SetMoveTransitionDefaults sets correct values based on direction.
    /// </summary>
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
            Duration = TimeSpan.FromSeconds(0.5),
            Width = 100,
            Height = 100
        };

        control.Measure(new Size(100, 100));
        control.Arrange(new Rect(0, 0, 100, 100));

        var storyboard = new Storyboard();
        var completingAnimation = new DoubleAnimation();
        var startingAnimation = new DoubleAnimation();
        storyboard.Children.Add(completingAnimation);
        storyboard.Children.Add(startingAnimation);

        control.CompletingTransition = storyboard;

        control.SetMoveTransitionDefaults();

        await Assert.That(completingAnimation.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(0.5));
        await Assert.That(startingAnimation.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(0.5));
        await Assert.That(startingAnimation.To).IsNotNull();
        await Assert.That(completingAnimation.From).IsNotNull();
    }

    /// <summary>
    /// Tests that SetBounceTransitionDefaults sets correct values for bounce transition.
    /// </summary>
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
            Width = 100,
            Height = 100
        };

        control.Measure(new Size(100, 100));
        control.Arrange(new Rect(0, 0, 100, 100));

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation();
        storyboard.Children.Add(animation);

        control.StartingTransition = storyboard;

        control.SetBounceTransitionDefaults();

        await Assert.That(animation.To.HasValue).IsTrue();

        var isVertical = direction == TransitioningContentControl.TransitionDirection.Down || direction == TransitioningContentControl.TransitionDirection.Up;
        var expectedMagnitude = isVertical ? control.ActualHeight : control.ActualWidth;

        await Assert.That(Math.Abs(animation.To!.Value)).IsEqualTo(expectedMagnitude);
    }

    /// <summary>
    /// Tests that ConfigureStandardTransition returns correct transition name for Fade.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ConfigureStandardTransition_WithFadeType_ReturnsCorrectName()
    {
        var control = CreateControlWithTemplate();
        control.Transition = TransitioningContentControl.TransitionType.Fade;

        var transitionName = control.ConfigureStandardTransition();

        await Assert.That(transitionName).IsEqualTo("Transition_Fade");
    }

    /// <summary>
    /// Tests that ConfigureStandardTransition returns correct transition name for non-Fade types.
    /// </summary>
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

    /// <summary>
    /// Tests that ConfigureBounceTransition returns correct transition names.
    /// </summary>
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

    /// <summary>
    /// Tests that PrepareTransitionImages sets content on the content presenter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task PrepareTransitionImages_WithNewContent_SetsContentPresenterContent()
    {
        var control = CreateControlWithTemplateParts();
        var newContent = new TextBlock { Text = "New Content" };

        control.PrepareTransitionImages(newContent);

        await Assert.That(control.CurrentContentPresentationSite?.Content).IsEqualTo(newContent);
    }

    /// <summary>
    /// Tests that GetRenderTargetBitmapFromUiElement returns default when element has zero size.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetRenderTargetBitmapFromUiElement_WithZeroSize_ReturnsDefault()
    {
        var button = new Button();

        var bitmap = TransitioningContentControl.GetRenderTargetBitmapFromUiElement(button);

        await Assert.That(bitmap).IsNull();
    }

    /// <summary>
    /// Tests that GetRenderTargetBitmapFromUiElement creates bitmap for rendered element.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetRenderTargetBitmapFromUiElement_WithRenderedElement_CreatesBitmap()
    {
        var button = new Button
        {
            Width = 100,
            Height = 50,
            Content = "Test"
        };

        button.Measure(new Size(100, 50));
        button.Arrange(new Rect(0, 0, 100, 50));

        TransitioningContentControl.OverrideDpi = true;
        var bitmap = TransitioningContentControl.GetRenderTargetBitmapFromUiElement(button);
        TransitioningContentControl.OverrideDpi = false;

        await Assert.That(bitmap).IsNotNull();
        await Assert.That(bitmap.PixelWidth).IsGreaterThan(0);
        await Assert.That(bitmap.PixelHeight).IsGreaterThan(0);
    }

    /// <summary>
    /// Tests that GetTransitionStoryboardByName throws when transition name is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetTransitionStoryboardByName_WithNullName_ThrowsArgumentNullException()
    {
        var control = CreateControlWithTemplate();

        await Assert.That(() => control.GetTransitionStoryboardByName(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that GetTransitionStoryboardByName throws when transition name is empty.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetTransitionStoryboardByName_WithEmptyName_ThrowsArgumentException()
    {
        var control = CreateControlWithTemplate();

        await Assert.That(() => control.GetTransitionStoryboardByName(string.Empty)).Throws<ArgumentException>();
    }

    /// <summary>
    /// Tests that GetTransitionStoryboardByName throws when visual state group is not initialized.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetTransitionStoryboardByName_WithoutVisualStateGroup_ThrowsInvalidOperationException()
    {
        var control = new TransitioningContentControl();

        await Assert.That(() => control.GetTransitionStoryboardByName("Transition_Fade"))
            .Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that GetTransitionStoryboardByName throws when transition is not found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetTransitionStoryboardByName_WithInvalidTransition_ThrowsInvalidOperationException()
    {
        var control = CreateControlWithTemplate();

        await Assert.That(() => control.GetTransitionStoryboardByName("NonExistentTransition"))
            .Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that GetTransitionStoryboardByName returns storyboard for valid transition.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetTransitionStoryboardByName_WithValidTransition_ReturnsStoryboard()
    {
        var control = CreateControlWithTemplate();

        var storyboard = control.GetTransitionStoryboardByName("Transition_Fade");

        await Assert.That(storyboard).IsNotNull();
    }

    /// <summary>
    /// Tests that SetTransitionDefaultValues calls correct method for each transition type.
    /// </summary>
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
            Width = 100,
            Height = 100
        };

        control.Measure(new Size(100, 100));
        control.Arrange(new Rect(0, 0, 100, 100));

        // Should not throw even without storyboards set (methods check for null)
        control.SetTransitionDefaultValues();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Tests that OnApplyTemplate throws when PART_Container is missing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task OnApplyTemplate_WithoutContainer_ThrowsInvalidOperationException()
    {
        var control = new TransitioningContentControl();
        var template = new ControlTemplate(typeof(TransitioningContentControl));
        var grid = new FrameworkElementFactory(typeof(Grid));
        grid.Name = "WrongName";
        template.VisualTree = grid;
        control.Template = template;

        await Assert.That(() => control.ApplyTemplate())
            .Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that OnApplyTemplate throws when PART_CurrentContentPresentationSite is missing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task OnApplyTemplate_WithoutContentPresenter_ThrowsInvalidOperationException()
    {
        var control = new TransitioningContentControl();
        var template = new ControlTemplate(typeof(TransitioningContentControl));
        var grid = new FrameworkElementFactory(typeof(Grid));
        grid.Name = "PART_Container";
        template.VisualTree = grid;
        control.Template = template;

        await Assert.That(() => control.ApplyTemplate())
            .Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that control can apply template successfully with all required parts.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task OnApplyTemplate_WithAllParts_Succeeds()
    {
        var control = CreateControlWithTemplateParts();

        await Assert.That(control.CurrentContentPresentationSite).IsNotNull();
    }

    /// <summary>
    /// Tests that enum values have correct count.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionType_Enum_HasExpectedValues()
    {
        var values = Enum.GetValues(typeof(TransitioningContentControl.TransitionType));

        await Assert.That(values.Length).IsEqualTo(5);
    }

    /// <summary>
    /// Tests that direction enum has correct count.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionDirection_Enum_HasExpectedValues()
    {
        var values = Enum.GetValues(typeof(TransitioningContentControl.TransitionDirection));

        await Assert.That(values.Length).IsEqualTo(4);
    }

    /// <summary>
    /// Tests that setting CompletingTransition triggers SetTransitionDefaultValues.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CompletingTransition_Set_TriggersSetTransitionDefaultValues()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Fade,
            Duration = TimeSpan.FromSeconds(0.5)
        };

        var storyboard = new Storyboard();
        var animation1 = new DoubleAnimation();
        var animation2 = new DoubleAnimation();
        storyboard.Children.Add(animation1);
        storyboard.Children.Add(animation2);

        control.CompletingTransition = storyboard;

        // SetTransitionDefaultValues should have been called, setting the durations
        await Assert.That(animation1.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(0.5));
        await Assert.That(animation2.Duration.TimeSpan).IsEqualTo(TimeSpan.FromSeconds(0.5));
    }

    /// <summary>
    /// Tests that setting StartingTransition triggers SetTransitionDefaultValues.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task StartingTransition_Set_TriggersSetTransitionDefaultValues()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Bounce,
            Direction = TransitioningContentControl.TransitionDirection.Down,
            Width = 100,
            Height = 100
        };

        control.Measure(new Size(100, 100));
        control.Arrange(new Rect(0, 0, 100, 100));

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation();
        storyboard.Children.Add(animation);

        control.StartingTransition = storyboard;

        // SetTransitionDefaultValues should have been called
        await Assert.That(animation.To).IsNotNull();
    }

    /// <summary>
    /// Tests that PrepareTransitionImages captures the current content.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task PrepareTransitionImages_CapturesCurrentContent()
    {
        var control = CreateControlWithTemplate();
        control.Width = 100;
        control.Height = 100;
        control.Measure(new Size(100, 100));
        control.Arrange(new Rect(0, 0, 100, 100));

        // Set initial content
        if (control.CurrentContentPresentationSite != null)
        {
            control.CurrentContentPresentationSite.Content = "Old Content";
        }

        // Prepare transition with new content
        control.PrepareTransitionImages("New Content");

        // Should update to new content
        await Assert.That(control.CurrentContentPresentationSite!.Content).IsEqualTo("New Content");
    }

    /// <summary>
    /// Tests that ConfigureBounceTransition sets both transitions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    /// <summary>
    /// Tests that ConfigureStandardTransition returns correct name for different transition types.
    /// </summary>
    /// <param name="transitionType">The transition type.</param>
    /// <param name="direction">The transition direction.</param>
    /// <param name="expectedName">The expected transition name.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Arguments(TransitioningContentControl.TransitionType.Fade, TransitioningContentControl.TransitionDirection.Left, "Transition_Fade")]
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

    /// <summary>
    /// Tests PrepareTransitionImages with zero-size element.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task PrepareTransitionImages_WithZeroSizeElement_HandlesGracefully()
    {
        var control = CreateControlWithTemplate();

        // Don't set size - ActualWidth/Height will be 0

        // Should not throw even with zero size
        control.PrepareTransitionImages("New Content");

        await Assert.That(control.CurrentContentPresentationSite!.Content).IsEqualTo("New Content");
    }

    /// <summary>
    /// Creates a control with a minimal template for testing.
    /// </summary>
    /// <returns>A configured TransitioningContentControl.</returns>
    private static TransitioningContentControl CreateControlWithTemplate()
    {
        var control = new TransitioningContentControl();

        // Create a minimal template with visual state groups
        var template = new ControlTemplate(typeof(TransitioningContentControl));
        var grid = new FrameworkElementFactory(typeof(Grid));
        grid.Name = "PART_Container";

        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenter.Name = "PART_CurrentContentPresentationSite";
        grid.AppendChild(contentPresenter);

        var image = new FrameworkElementFactory(typeof(Image));
        image.Name = "PART_PreviousImageSite";
        grid.AppendChild(image);

        template.VisualTree = grid;
        control.Template = template;
        control.ApplyTemplate();

        // Set up visual state groups AFTER applying template
        var container = control.Template.FindName("PART_Container", control) as Grid;
        if (container != null)
        {
            var stateGroup = new VisualStateGroup { Name = "PresentationStates" };

            // Add states for all transition types
            AddTransitionStates(stateGroup);

            var groups = VisualStateManager.GetVisualStateGroups(container);
            groups.Add(stateGroup);

            // Manually update the PresentationStateGroup property since OnApplyTemplate already ran
            // Get the first group with name "PresentationStates"
            var presentationGroup = groups.OfType<VisualStateGroup>().FirstOrDefault(g => g.Name == "PresentationStates");
            control.PresentationStateGroup = presentationGroup;
        }

        return control;
    }

    /// <summary>
    /// Creates a control with template parts set up for testing.
    /// </summary>
    /// <returns>A configured TransitioningContentControl.</returns>
    private static TransitioningContentControl CreateControlWithTemplateParts()
    {
        var control = new TransitioningContentControl();

        var template = new ControlTemplate(typeof(TransitioningContentControl));
        var grid = new FrameworkElementFactory(typeof(Grid));
        grid.Name = "PART_Container";

        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenter.Name = "PART_CurrentContentPresentationSite";
        grid.AppendChild(contentPresenter);

        var image = new FrameworkElementFactory(typeof(Image));
        image.Name = "PART_PreviousImageSite";
        grid.AppendChild(image);

        template.VisualTree = grid;
        control.Template = template;
        control.ApplyTemplate();

        return control;
    }

    /// <summary>
    /// Adds all required transition states to a visual state group.
    /// </summary>
    /// <param name="stateGroup">The visual state group.</param>
    private static void AddTransitionStates(VisualStateGroup stateGroup)
    {
        // Normal state
        stateGroup.States.Add(new VisualState { Name = "Normal", Storyboard = new Storyboard() });

        // Fade transition
        var fadeStoryboard = new Storyboard();
        fadeStoryboard.Children.Add(new DoubleAnimation());
        fadeStoryboard.Children.Add(new DoubleAnimation());
        stateGroup.States.Add(new VisualState { Name = "Transition_Fade", Storyboard = fadeStoryboard });

        // Add states for each direction and transition type
        var directions = new[] { "Left", "Right", "Up", "Down" };
        var transitions = new[] { "Move", "Slide", "Drop" };

        foreach (var transition in transitions)
        {
            foreach (var direction in directions)
            {
                var storyboard = new Storyboard();
                storyboard.Children.Add(new DoubleAnimation());
                if (transition == "Move")
                {
                    storyboard.Children.Add(new DoubleAnimation());
                }

                stateGroup.States.Add(new VisualState { Name = $"Transition_{transition}{direction}", Storyboard = storyboard });
            }
        }

        // Bounce transitions (need both In and Out)
        foreach (var direction in directions)
        {
            var bounceOutStoryboard = new Storyboard();
            bounceOutStoryboard.Children.Add(new DoubleAnimation());
            stateGroup.States.Add(new VisualState { Name = $"Transition_Bounce{direction}Out", Storyboard = bounceOutStoryboard });

            var bounceInStoryboard = new Storyboard();
            var keyFrameAnimation = new DoubleAnimationUsingKeyFrames();
            keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame());
            keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame());
            bounceInStoryboard.Children.Add(keyFrameAnimation);
            stateGroup.States.Add(new VisualState { Name = $"Transition_Bounce{direction}In", Storyboard = bounceInStoryboard });
        }
    }
}
