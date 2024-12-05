// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;

namespace ReactiveUI.Avalonia
{
    /// <summary>
    /// A ReactiveUI <see cref="Window"/> that implements the <see cref="IViewFor{TViewModel}"/> interface and will
    /// activate your ViewModel automatically if the view model implements <see cref="IActivatableViewModel"/>. When
    /// the DataContext property changes, this class will update the ViewModel property with the new DataContext value,
    /// and vice versa.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    public class ReactiveWindow<TViewModel> : Window, IViewFor<TViewModel> where TViewModel : class
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1002", Justification = "Generic avalonia property is expected here.")]
        public static readonly StyledProperty<TViewModel?> ViewModelProperty = AvaloniaProperty
            .Register<ReactiveWindow<TViewModel>, TViewModel?>(nameof(ViewModel));

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindow{TViewModel}"/> class.
        /// </summary>
        public ReactiveWindow()
        {
            // This WhenActivated block calls ViewModel's WhenActivated
            // block if the ViewModel implements IActivatableViewModel.
            this.WhenActivated(disposables => { });
        }
            
        /// <summary>
        /// The ViewModel.
        /// </summary>
        public TViewModel? ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel?)value;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataContextProperty)
            {
                if (ReferenceEquals(change.OldValue, ViewModel)
                    && change.NewValue is null or TViewModel)
                {
                    SetCurrentValue(ViewModelProperty, change.NewValue);
                }
            }
            else if (change.Property == ViewModelProperty)
            {
                if (ReferenceEquals(change.OldValue, DataContext))
                {
                    SetCurrentValue(DataContextProperty, change.NewValue);
                }
            }
        }
    }
}
