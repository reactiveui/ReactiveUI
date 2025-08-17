// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace ReactiveUI.Builder.WpfApp.Views;

/// <summary>
/// Lobby (rooms listing) view.
/// </summary>
public partial class LobbyView : IViewFor<ViewModels.LobbyViewModel>
{
    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(ViewModels.LobbyViewModel), typeof(LobbyView), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="LobbyView"/> class.
    /// </summary>
    public LobbyView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.DisplayName, v => v.DisplayNameBox.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.RoomName, v => v.RoomNameBox.Text).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.CreateRoom, v => v.CreateRoomButton).DisposeWith(d);

            RoomsList.MouseDoubleClick += Dbl;
            RoomsList.KeyDown += Enter;
            Disposable.Create(() =>
            {
                RoomsList.MouseDoubleClick -= Dbl;
                RoomsList.KeyDown -= Enter;
            }).DisposeWith(d);

            // Delete selected room via Delete button only
            var selectedRoomStream = this.WhenAnyValue(x => x.RoomsList.SelectedItem)
                .Select(x => x as ViewModels.ChatRoom)
                .WhereNotNull();
            this.BindCommand(ViewModel, vm => vm.DeleteRoom, v => v.DeleteRoomButton, selectedRoomStream)
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.Rooms, v => v.RoomsList.ItemsSource).DisposeWith(d);
        });

        // Enter key to join
        void Enter(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && RoomsList.SelectedItem is ViewModels.ChatRoom room)
            {
                ViewModel?.JoinRoom.Execute(room).Subscribe();
            }
        }

        // Double-click to join room
        void Dbl(object s, MouseButtonEventArgs e)
        {
            if (RoomsList.SelectedItem is ViewModels.ChatRoom room)
            {
                ViewModel?.JoinRoom.Execute(room).Subscribe();
            }
        }
    }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public ViewModels.LobbyViewModel? ViewModel
    {
        get => (ViewModels.LobbyViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the ViewModel corresponding to this specific View. This should be
    /// a DependencyProperty if you're using XAML.
    /// </summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ViewModels.LobbyViewModel?)value;
    }
}
