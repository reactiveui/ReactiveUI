// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ReactiveUI.Avalonia
{
    /// <summary>
    /// Determines when Avalonia IVisuals get activated.
    /// </summary>
    public class AvaloniaActivationForViewFetcher : IActivationForViewFetcher
    {
        /// <inheritdoc/>
        public int GetAffinityForView(Type view) => typeof(Visual).IsAssignableFrom(view) ? 10 : 0;

        /// <inheritdoc/>
        public IObservable<bool> GetActivationForView(IActivatableView view)
        {
            if (view is not Visual visual)
            {
                return Observable.Return(false);
            }

            if (view is Control control)
            {
                return GetActivationForControl(control);
            }

            return GetActivationForVisual(visual);
        }

        /// <summary>
        /// Listens to Loaded and Unloaded
        /// events for Avalonia Control.
        /// </summary>
        private static IObservable<bool> GetActivationForControl(Control control)
        {
            var controlLoaded = Observable
                                .FromEventPattern<RoutedEventArgs>(
                                                                   x => control.Loaded += x,
                                                                   x => control.Loaded -= x)
                                .Select(args => true);
            var controlUnloaded = Observable
                                  .FromEventPattern<RoutedEventArgs>(
                                                                     x => control.Unloaded += x,
                                                                     x => control.Unloaded -= x)
                                  .Select(args => false);
            return controlLoaded
                   .Merge(controlUnloaded)
                   .DistinctUntilChanged();
        }

        /// <summary>
        /// Listens to AttachedToVisualTree and DetachedFromVisualTree
        /// events for Avalonia IVisuals.
        /// </summary>
        private static IObservable<bool> GetActivationForVisual(Visual visual)
        {
            var visualLoaded = Observable
                               .FromEventPattern<VisualTreeAttachmentEventArgs>(
                                                                                x => visual.AttachedToVisualTree += x,
                                                                                x => visual.AttachedToVisualTree -= x)
                               .Select(args => true);
            var visualUnloaded = Observable
                                 .FromEventPattern<VisualTreeAttachmentEventArgs>(
                                      x => visual.DetachedFromVisualTree += x,
                                      x => visual.DetachedFromVisualTree -= x)
                                 .Select(args => false);
            return visualLoaded
                   .Merge(visualUnloaded)
                   .DistinctUntilChanged();
        }
    }
}
