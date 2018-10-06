using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IntegrationTests.Shared;
using System.Reactive.Linq;
using System;
using ReactiveUI;
using System.Reactive.Disposables;

namespace IntegrationTests.Avalonia
{
    public class LoginControl : UserControl, IViewFor<LoginViewModel>
    {
        private static readonly AvaloniaProperty<LoginViewModel> ViewModelProperty = 
            AvaloniaProperty.Register<LoginControl, LoginViewModel>(
                nameof(ViewModel), inherits: true);

        public LoginControl()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = ViewModel = new LoginViewModel(AvaloniaScheduler.Instance);
            this.WhenActivated(disposableRegistration => 
            {
                ViewModel.Login.Subscribe(result => 
                {
                    if (!result.HasValue) return;
                    if (result.Value) {
                        // Show success dialog
                    } else {
                        // Show failure dialog
                    }
                })
                .DisposeWith(disposableRegistration);
            });
        }

        public LoginViewModel ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (LoginViewModel)value;
        }
    }
}
