// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Windows;

using ReactiveUI.Builder.WpfApp.ViewModels;

namespace ReactiveUI.Builder.WpfApp.Views;

/// <summary>
/// Chat room view.
/// </summary>
public partial class ChatRoomView : IViewFor<ChatRoomViewModel>
{
    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(ChatRoomViewModel), typeof(ChatRoomView), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatRoomView"/> class.
    /// </summary>
    public ChatRoomView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            // Map ViewModel to DataContext for XAML bindings like {Binding RoomName}
            this.WhenAnyValue<ChatRoomView, ChatRoomViewModel>(nameof(ViewModel)).BindTo(this, v => v.DataContext).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.MessageText, v => v.MessageBox.Text).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.SendMessage, v => v.SendButton).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.Messages, v => v.MessagesList.ItemsSource).DisposeWith(d);

            // Back navigation
            this.BindCommand(ViewModel, s => s!.HostScreen.Router.NavigateBack, v => v.BackButton).DisposeWith(d);
        });
    }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public ChatRoomViewModel? ViewModel
    {
        get => (ChatRoomViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the ViewModel corresponding to this specific View. This should be
    /// a DependencyProperty if you're using XAML.
    /// </summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ChatRoomViewModel?)value;
    }
}
