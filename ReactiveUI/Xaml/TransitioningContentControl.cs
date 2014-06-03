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
    [TemplatePart(Name = "PART_Container", Type = typeof (FrameworkElement))]
    [TemplatePart(Name = "PART_PreviousContentPresentationSite", Type = typeof (ContentPresenter))]
    [TemplatePart(Name = "PART_CurrentContentPresentationSite", Type = typeof (ContentPresenter))]
    [TemplateVisualState(Name = NormalState, GroupName = PresentationGroup)]
    public class TransitioningContentControl : ContentControl
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Transition"/> property.
        /// </summary>
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.RegisterAttached(
            "Transition", typeof (TransitionType), typeof (TransitioningContentControl),
            new PropertyMetadata(TransitionType.Fade, OnTransitionChanged));

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="TransitionPart"/> property.
        /// </summary>
        public static readonly DependencyProperty TransitionPartProperty =
            DependencyProperty.RegisterAttached("TransitionPart", typeof (TransitionPartType),
                typeof (TransitioningContentControl),
                new PropertyMetadata(TransitionPartType.OutIn, OnTransitionPartChanged));

        const string PresentationGroup = "PresentationStates";
        const string NormalState = "Normal";

        bool isTransitioning;
        bool canSplitTransition;
        Storyboard startingTransition;
        Storyboard completingTransition;

        Grid container;
        ContentPresenter previousContentPresentationSite;
        ContentPresenter currentContentPresentationSite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitioningContentControl"/> class.
        /// </summary>
        public TransitioningContentControl()
        {
            this.DefaultStyleKey = typeof (TransitioningContentControl);
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
        /// Represents the part of the transition that the developer would like the TransitioningContentControl to perform
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
        public TransitionType Transition { get { return (TransitionType) this.GetValue(TransitionProperty); } set { this.SetValue(TransitionProperty, value); } }

        /// <summary>
        /// Gets or sets the transition part.
        /// </summary>
        /// <value>The transition part.</value>
        public TransitionPartType TransitionPart { get { return (TransitionPartType) this.GetValue(TransitionPartProperty); } set { this.SetValue(TransitionPartProperty, value); } }

        Storyboard StartingTransition
        {
            get { return this.startingTransition; }

            set
            {
                this.startingTransition = value;
                if (this.startingTransition != null) {
                    this.SetTransitionDefaultValues();
                }
            }
        }

        Storyboard CompletingTransition
        {
            get { return this.completingTransition; }

            set
            {
                // Decouple transition.
                if (this.completingTransition != null) {
                    this.completingTransition.Completed -= this.OnTransitionCompleted;
                }

                this.completingTransition = value;

                if (this.completingTransition != null) {
                    this.completingTransition.Completed += this.OnTransitionCompleted;
                    this.SetTransitionDefaultValues();
                }
            }
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            this.QueueTransition(oldContent, newContent);
            base.OnContentChanged(oldContent, newContent);
        }

        static void OnTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = (TransitioningContentControl) d;
            var transition = (TransitionType) e.NewValue;

            transitioningContentControl.canSplitTransition = VerifyCanSplitTransition(transition,
                transitioningContentControl.TransitionPart);
        }

        static void OnTransitionPartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = (TransitioningContentControl) d;
            var transitionPart = (TransitionPartType) e.NewValue;

            transitioningContentControl.canSplitTransition =
                VerifyCanSplitTransition(transitioningContentControl.Transition, transitionPart);
        }

        static bool VerifyCanSplitTransition(TransitionType transition, TransitionPartType transitionPart)
        {
            // Check whether the TransitionPart is compatible with the current transition.
            var canSplitTransition = true;
            if (transition == TransitionType.Fade || transition == TransitionType.FadeDown) {
                if (transitionPart != TransitionPartType.OutIn) {
                    throw new InvalidOperationException("Cannot split this transition.");
                }

                canSplitTransition = false;
            }

            return canSplitTransition;
        }

        /// <summary>
        /// Aborts the transition.
        /// </summary>
        void AbortTransition()
        {
            // Go to a normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, NormalState, false);
            this.isTransitioning = false;
            if (this.previousContentPresentationSite != null) {
                this.previousContentPresentationSite.Content = null;
            }
        }

        void OnTransitionCompleted(object sender, EventArgs e)
        {
            this.AbortTransition();

            var handler = this.TransitionCompleted;
            if (handler != null) {
                handler(this, new RoutedEventArgs());
            }
        }

        void RaiseTransitionStarted()
        {
            var handler = this.TransitionStarted;
            if (handler != null) {
                handler(this, new RoutedEventArgs());
            }
        }

        void QueueTransition(object oldContent, object newContent)
        {
            // Both ContentPresenters must be available, otherwise a transition is useless.
            if (this.currentContentPresentationSite != null && this.previousContentPresentationSite != null) {
                this.currentContentPresentationSite.Content = newContent;
                this.previousContentPresentationSite.Content = oldContent;

                if (!this.isTransitioning) {
                    // Determine the TransitionPart that is associated with this transition and either set up a single part transition, or a queued transition.
                    string startingTransitionName;
                    if (this.TransitionPart == TransitionPartType.OutIn && this.canSplitTransition) {
                        // Wire up the completion transition.
                        var transitionInName = this.Transition + "Transition_" + TransitionPartType.In;
                        var transitionIn = this.GetTransitionStoryboardByName(transitionInName);
                        this.CompletingTransition = transitionIn;

                        // Wire up the first transition to start the second transition when it's complete.
                        startingTransitionName = this.Transition + "Transition_" + TransitionPartType.Out;
                        var transitionOut = this.GetTransitionStoryboardByName(startingTransitionName);
                        transitionOut.Completed +=
                            delegate { VisualStateManager.GoToState(this, transitionInName, false); };
                        this.StartingTransition = transitionOut;
                    } else {
                        startingTransitionName = this.Transition + "Transition_" + this.TransitionPart;
                        var transitionIn = this.GetTransitionStoryboardByName(startingTransitionName);
                        this.CompletingTransition = transitionIn;
                    }

                    // Start the transition.
                    this.isTransitioning = true;
                    this.RaiseTransitionStarted();
                    VisualStateManager.GoToState(this, startingTransitionName, false);
                }
            } else {
                if (this.currentContentPresentationSite != null)
                    this.currentContentPresentationSite.Content = newContent;
            }
        }

        Storyboard GetTransitionStoryboardByName(string transitionName)
        {
            // Hook up the CurrentTransition.
            var presentationGroup =
                ((IEnumerable<VisualStateGroup>) VisualStateManager.GetVisualStateGroups(this.container)).Where(
                    o => o.Name == PresentationGroup).FirstOrDefault();
            if (presentationGroup == null) {
                throw new ArgumentException("Invalid VisualStateGroup.");
            }

            var transition =
                ((IEnumerable<VisualState>) presentationGroup.States).Where(o => o.Name == transitionName).Select(
                    o => o.Storyboard).FirstOrDefault();
            if (transition == null) {
                throw new ArgumentException("Invalid transition");
            }

            return transition;
        }

        public override void OnApplyTemplate()
        {
            // Wire up all of the various control parts.
            this.container = (Grid) GetTemplateChild("PART_Container");
            if (this.container == null) {
                throw new ArgumentException("PART_Container not found.");
            }

            this.currentContentPresentationSite =
                (ContentPresenter) GetTemplateChild("PART_CurrentContentPresentationSite");
            if (this.currentContentPresentationSite == null) {
                throw new ArgumentException("PART_CurrentContentPresentationSite not found.");
            }

            this.previousContentPresentationSite =
                (ContentPresenter) GetTemplateChild("PART_PreviousContentPresentationSite");

            // Set the current content site to the first piece of content.
            this.currentContentPresentationSite.Content = Content;
            VisualStateManager.GoToState(this, NormalState, false);
        }

        /// <summary>
        /// Sets default values for certain transition types.
        /// </summary>
        void SetTransitionDefaultValues()
        {
            // Do some special handling of particular transitions so that we get nice smooth transitions that utilise the size of the content.
            if (this.Transition == TransitionType.FadeDown) {
                var completingDoubleAnimation = (DoubleAnimation) this.CompletingTransition.Children[0];
                completingDoubleAnimation.From = -this.ActualHeight;

                var startingDoubleAnimation = (DoubleAnimation) this.CompletingTransition.Children[1];
                startingDoubleAnimation.To = this.ActualHeight;

                return;
            }

            if (this.Transition == TransitionType.SlideLeft) {
                if (this.CompletingTransition != null) {
                    var completingDoubleAnimation =
                        (DoubleAnimationUsingKeyFrames) this.CompletingTransition.Children[0];
                    completingDoubleAnimation.KeyFrames[1].Value = -this.ActualWidth;
                }

                if (this.StartingTransition != null) {
                    var startingDoubleAnimation = (DoubleAnimation) this.StartingTransition.Children[0];
                    startingDoubleAnimation.To = -this.ActualWidth;
                }

                return;
            }
        }
    }
}