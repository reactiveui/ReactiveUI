using IntegrationTests.Shared;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using Windows.UI.Popups;

namespace IntegrationTests.UWP
{
    public class LoginControlBase : ReactiveUserControl<LoginViewModel> { }

    public partial class LoginControl : LoginControlBase
    {
        public LoginControl()
        {
            this.InitializeComponent();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler)
            {
                UserName = "",
                Password = ""
            };

            this
               .WhenActivated(
                   disposables =>
                   {
                       this
                           .Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                           .DisposeWith(disposables);
                       this
                           .Bind(ViewModel, vm => vm.Password, v => v.Password.Password)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                           .DisposeWith(disposables);
                       
                       this
                           .ViewModel
                           .Login
                           .SelectMany(
                               result =>
                               {
                                   if (!result.HasValue)
                                   {
                                       return Observable.Empty<IUICommand>();
                                   }

                                   if (result.Value)
                                   {
                                       var dialog = new MessageDialog("Login Successful", "Welcome!");
                                       return dialog.ShowAsync().ToObservable();
                                   }
                                   else
                                   {
                                       var dialog = new MessageDialog("Login Failed", "Ah, ah, ah, you didn't say the magic word!");
                                       return dialog.ShowAsync().ToObservable();
                                   }
                               })
                           .Subscribe()
                           .DisposeWith(disposables);
                   });
        }
    }
}
