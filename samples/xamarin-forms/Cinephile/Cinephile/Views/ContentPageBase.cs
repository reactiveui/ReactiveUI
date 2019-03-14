// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using System.Reactive.Disposables;
using Cinephile.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Cinephile.Views
{
    public class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel> where TViewModel : ViewModelBase
    {
        public ContentPageBase()
        {
            this.WhenActivated(disposables =>
            {
                this
                    .ViewModel
                    .ShowAlert
                    .RegisterHandler(interaction =>
                        {
                            AlertViewModel input = interaction.Input;
                            DisplayAlert(input.Title, input.Description, input.ButtonText);
                            interaction.SetOutput(new Unit());
                        })
                    .DisposeWith(disposables);

                this
                    .ViewModel
                    .OpenBrowser
                    .RegisterHandler(interaction =>
                    {
                        Device.OpenUri(new Uri(interaction));
                        interaction.SetOutput(new Unit());
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}

