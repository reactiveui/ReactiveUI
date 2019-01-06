using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using IntegrationTests.Shared;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace IntegrationTests.WPF
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml.
    /// </summary>
    public partial class LoginControl : ReactiveUserControl<LoginViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginControl"/> class.
        /// </summary>
        public LoginControl()
        {
            InitializeComponent();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);

            this
               .WhenActivated(
                   disposables =>
                   {
                       this
                           .Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                           .DisposeWith(disposables);

                       // we marshal changes to password manually because WPF's
                       // PasswordBox.Password property doesn't support change notifications
                       Password
                          .Events()
                          .PasswordChanged
                          .Select(_ => Password.Password)
                          .Subscribe(x => ViewModel.Password = x)
                          .DisposeWith(disposables);

                       ViewModel
                        .Login
                        .SelectMany(
                            result =>
                            {
                                if (result)
                                {
                                    return this.ShowMessage("Login Successful", "Welcome!");
                                }

                                return this.ShowMessage("Login Failed", "Ah, ah, ah, you didn't say the magic word!");
                            })
                        .Subscribe()
                        .DisposeWith(disposables);
                   });
        }
    }
}
