// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    /// <summary>
    /// A view model which shows information about the application.
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        /// <param name="mainThreadScheduler">The scheduler to use for processing on the main UI thread.</param>
        /// <param name="taskPoolScheduler">The scheduler to use for scheduling on a background thread.</param>
        /// <param name="hostScreen">The main screen for routing.</param>
        public AboutViewModel(
                IScheduler mainThreadScheduler = null,
                IScheduler taskPoolScheduler = null,
                IScreen hostScreen = null)
            : base("About", mainThreadScheduler, taskPoolScheduler, hostScreen)
        {
            ShowIconCredits = ReactiveCommand.CreateFromObservable<string, Unit>(url => OpenBrowser.Handle(url));
            ShowIconCredits.Subscribe();
        }

        /// <summary>
        /// Gets a command which will show the icon credits.
        /// </summary>
        public ReactiveCommand<string, Unit> ShowIconCredits
        {
            get;
        }
    }
}
