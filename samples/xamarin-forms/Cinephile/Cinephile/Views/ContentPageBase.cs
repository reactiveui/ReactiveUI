// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Disposables;
using Cinephile.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Cinephile.Views
{
    /// <summary>
    /// A base page used for all our content pages. It is mainly used for interaction registrations.
    /// </summary>
    /// <typeparam name="TViewModel">The view model which the page contains.</typeparam>
    public class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel>
        where TViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPageBase{TViewModel}"/> class.
        /// </summary>
        public ContentPageBase()
        {
            this.WhenActivated(disposables =>
            {
                ViewModel
                    .ShowAlert
                    .RegisterHandler(interaction =>
                        {
                            AlertViewModel input = interaction.Input;
                            DisplayAlert(input.Title, input.Description, input.ButtonText);
                            interaction.SetOutput(Unit.Default);
                        })
                    .DisposeWith(disposables);

                ViewModel
                    .OpenBrowser
                    .RegisterHandler(interaction =>
                    {
                        Device.OpenUri(new Uri(interaction.Input));
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}
