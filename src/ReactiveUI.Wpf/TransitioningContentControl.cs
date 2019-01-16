// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

// This control is gratefully borrowed from http://blog.landdolphin.net/?p=17
// Thanks guys!
namespace ReactiveUI
{
    /// <summary>
    /// A ContentControl that animates the transition when its content is changed.
    /// </summary>
    [TemplatePart(Name = "PART_Container", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_PreviousContentPresentationSite", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_CurrentContentPresentationSite", Type = typeof(ContentPresenter))]
    [TemplateVisualState(Name = NormalState, GroupName = PresentationGroup)]
    public class TransitioningContentControl : ContentControl
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Transition"/> property.
        /// </summary>
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.RegisterAttached(
            "Transition",
            typeof(TransitionType),
            typeof(TransitioningContentControl),
            new PropertyMetadata(TransitionType.Fade, OnTransitionChanged));

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="TransitionPart"/> property.
        /// </summary>
        public static readonly DependencyProperty TransitionPartProperty =
            DependencyProperty.RegisterAttached(
                "TransitionPart",
                typeof(TransitionPartType),
                typeof(TransitioningContentControl),
                new PropertyMetadata(TransitionPartType.OutIn, OnTransitionPartChanged));

        private const string PresentationGroup = "PresentationStates";
        private const string NormalState = "Normal";
        private bool _isTransitioning;
        private bool _canSplitTransition;
        private Storyboard _startingTransition;
        private Storyboard _completingTransition;
        private Grid _container;
        private ContentPresenter _previousContentPresentationSite;
        private ContentPresenter _currentContentPresentationSite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitioningContentControl"/> class.
        /// </summary>
        public TransitioningContentControl()
        {
            DefaultStyleKey = typeof(TransitioningContentControl);
        }

        /// <summary>
        /// Occurs when a transition has completed.
        /// </summary>
        public event RoutedEventHandler TransitionCompleted;

        /// <summary>
        /// Occurs when a transition has started.
        /// </summary>
        public event RoutedEventHandler TransitionStarted;

        /// <summary>
        /// Represents the type of transition that a TransitioningContentControl will perform.
        /// </summary>
        public enum TransitionType
        {
            /// <summary>
            /// A simple fading transition.
            /// </summary>
            Fade,

            /// <summary>
            /// A transition that fades the new element in from the top.
            /// </summary>
            FadeDown,

            /// <summary>
            /// A transition that slides old content left and out of view, then slides new content back in from the same direction.
            /// </summary>
            SlideLeft
        }

        /// <summary>
        /// Represents the part of the transition that the developer would like the TransitioningContentControl to perform.
        /// </summary>
        /// <remarks>This only applies to certain TransitionTypes. An InvalidOperationException will be thrown if the TransitionType does not support the TransitionPartType. Default is OutIn.</remarks>
        public enum TransitionPartType
        {
            /// <summary>
            /// Transitions out only.
            /// </summary>
            Out,

            /// <summary>
            /// Transitions in only.
            /// </summary>
            In,

            /// <summary>
            /// Transitions in and out.
            /// </summary>
            OutIn
        }

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>The transition.</value>
        public TransitionType Transition { get => (TransitionType)GetValue(TransitionProperty); set => SetValue(TransitionProperty, value); }

        /// <summary>
        /// Gets or sets the transition part.
        /// </summary>
        /// <value>The transition part.</value>
        public TransitionPartType TransitionPart { get => (TransitionPartType)GetValue(TransitionPartProperty); set => SetValue(TransitionPartProperty, value); }

        private Storyboard StartingTransition
        {
            get => _startingTransition;
            set
            {
                _startingTransition = value;
                if (_startingTransition != null)
                {
                    SetTransitionDefaultValues();
                }
            }
        }

        private Storyboard CompletingTransition
        {
            get => _completingTransition;
            set
            {
                // Decouple transition.
                if (_completingTransition != null)
                {
                    _completingTransition.Completed -= OnTransitionCompleted;
                }

                _completingTransition = value;

                if (_completingTransition != null)
                {
                    _completingTransition.Completed += OnTransitionCompleted;
                    SetTransitionDefaultValues();
                }
            }
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            // Wire up all of the various control parts.
            _container = (Grid)GetTemplateChild("PART_Container");
            if (_container == null)
            {
                throw new ArgumentException("PART_Container not found.");
            }

            _currentContentPresentationSite =
                (ContentPresenter)GetTemplateChild("PART_CurrentContentPresentationSite");
            if (_currentContentPresentationSite == null)
            {
                throw new ArgumentException("PART_CurrentContentPresentationSite not found.");
            }

            _previousContentPresentationSite =
                (ContentPresenter)GetTemplateChild("PART_PreviousContentPresentationSite");

            // Set the current content site to the first piece of content.
            _currentContentPresentationSite.Content = Content;
            VisualStateManager.GoToState(this, NormalState, false);
        }

        /// <summary>
        /// Called when the value of the <see cref="ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            QueueTransition(oldContent, newContent);
            base.OnContentChanged(oldContent, newContent);
        }

        private static void OnTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = (TransitioningContentControl)d;
            var transition = (TransitionType)e.NewValue;

            transitioningContentControl._canSplitTransition = VerifyCanSplitTransition(
                transition,
                transitioningContentControl.TransitionPart);
        }

        private static void OnTransitionPartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = (TransitioningContentControl)d;
            var transitionPart = (TransitionPartType)e.NewValue;

            transitioningContentControl._canSplitTransition =
                VerifyCanSplitTransition(transitioningContentControl.Transition, transitionPart);
        }

        private static bool VerifyCanSplitTransition(TransitionType transition, TransitionPartType transitionPart)
        {
            // Check whether the TransitionPart is compatible with the current transition.
            var canSplitTransition = true;
            if (transition == TransitionType.Fade || transition == TransitionType.FadeDown)
            {
                if (transitionPart != TransitionPartType.OutIn)
                {
                    throw new InvalidOperationException("Cannot split this transition.");
                }

                canSplitTransition = false;
            }

            return canSplitTransition;
        }

        /// <summary>
        /// Aborts the transition.
        /// </summary>
        private void AbortTransition()
        {
            // Go to a normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, NormalState, false);
            _isTransitioning = false;
            if (_previousContentPresentationSite != null)
            {
                _previousContentPresentationSite.Content = null;
            }
        }

        private void OnTransitionCompleted(object sender, EventArgs e)
        {
            AbortTransition();

            TransitionCompleted?.Invoke(this, new RoutedEventArgs());
        }

        private void RaiseTransitionStarted()
        {
            TransitionStarted?.Invoke(this, new RoutedEventArgs());
        }

        private void QueueTransition(object oldContent, object newContent)
        {
            // Both ContentPresenters must be available, otherwise a transition is useless.
            if (_currentContentPresentationSite != null && _previousContentPresentationSite != null)
            {
                _currentContentPresentationSite.Content = newContent;
                _previousContentPresentationSite.Content = oldContent;

                if (!_isTransitioning)
                {
                    // Determine the TransitionPart that is associated with this transition and either set up a single part transition, or a queued transition.
                    string startingTransitionName;
                    if (TransitionPart == TransitionPartType.OutIn && _canSplitTransition)
                    {
                        // Wire up the completion transition.
                        var transitionInName = Transition + "Transition_" + TransitionPartType.In;
                        CompletingTransition = GetTransitionStoryboardByName(transitionInName);

                        // Wire up the first transition to start the second transition when it's complete.
                        startingTransitionName = Transition + "Transition_" + TransitionPartType.Out;
                        var transitionOut = GetTransitionStoryboardByName(startingTransitionName);
                        transitionOut.Completed += (sender, args) => VisualStateManager.GoToState(this, transitionInName, false);
                        StartingTransition = transitionOut;
                    }
                    else
                    {
                        startingTransitionName = Transition + "Transition_" + TransitionPart;
                        CompletingTransition = GetTransitionStoryboardByName(startingTransitionName);
                    }

                    // Start the transition.
                    _isTransitioning = true;
                    RaiseTransitionStarted();
                    VisualStateManager.GoToState(this, startingTransitionName, false);
                }
            }
            else
            {
                if (_currentContentPresentationSite != null)
                {
                    _currentContentPresentationSite.Content = newContent;
                }
            }
        }

        private Storyboard GetTransitionStoryboardByName(string transitionName)
        {
            // Hook up the CurrentTransition.
            var presentationGroup =
                ((IEnumerable<VisualStateGroup>)VisualStateManager.GetVisualStateGroups(_container)).FirstOrDefault(o => o.Name == PresentationGroup);
            if (presentationGroup == null)
            {
                throw new ArgumentException("Invalid VisualStateGroup.");
            }

            var transition =
                ((IEnumerable<VisualState>)presentationGroup.States).Where(o => o.Name == transitionName).Select(
                    o => o.Storyboard).FirstOrDefault();
            if (transition == null)
            {
                throw new ArgumentException("Invalid transition");
            }

            return transition;
        }

        /// <summary>
        /// Sets default values for certain transition types.
        /// </summary>
        private void SetTransitionDefaultValues()
        {
            // Do some special handling of particular transitions so that we get nice smooth transitions that utilise the size of the content.
            if (Transition == TransitionType.FadeDown)
            {
                var completingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[0];
                completingDoubleAnimation.From = -ActualHeight;

                var startingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[1];
                startingDoubleAnimation.To = ActualHeight;

                return;
            }

            if (Transition == TransitionType.SlideLeft)
            {
                if (CompletingTransition != null)
                {
                    var completingDoubleAnimation =
                        (DoubleAnimationUsingKeyFrames)CompletingTransition.Children[0];
                    completingDoubleAnimation.KeyFrames[1].Value = -ActualWidth;
                }

                if (StartingTransition != null)
                {
                    var startingDoubleAnimation = (DoubleAnimation)StartingTransition.Children[0];
                    startingDoubleAnimation.To = -ActualWidth;
                }

                return;
            }
        }
    }
}
