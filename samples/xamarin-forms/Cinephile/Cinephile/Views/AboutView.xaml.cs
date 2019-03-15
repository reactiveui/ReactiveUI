// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Input;
using Cinephile.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace Cinephile.Views
{
    /// <summary>
    /// A page which contains information about a movie.
    /// </summary>
    public partial class AboutView : ContentPageBase<AboutViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutView"/> class.
        /// </summary>
        public AboutView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, x => x.ShowIconCredits, x => x.OpenBrowser.Command).DisposeWith(disposables);
            });
        }

        private void OpenBrowserWithUrl(string url)
        {
            Device.OpenUri(new Uri(url));
        }
    }
}
