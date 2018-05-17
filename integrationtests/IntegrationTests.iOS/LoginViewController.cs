using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using IntegrationTests.Shared;
using ReactiveUI;
using UIKit;

namespace IntegrationTests.iOS
{
    public partial class LoginViewController : ReactiveViewController<LoginViewModel>
    {
        public LoginViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.UserName, v => v.UsernameField.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Password, v => v.PasswordField.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Login, v => v.LoginButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.CancelButton)
                    .DisposeWith(disposables);

                ViewModel.Login
                         .SelectMany(result =>
                         {
                             if (!result.HasValue)
                             {
                                 return Observable.Empty<Unit>();
                             }

                             UIAlertController alert;

                             if (result.Value)
                             {
                                 alert = UIAlertController.Create("Login Successful", "Welcome!", UIAlertControllerStyle.Alert);
                             }
                             else
                             {
                                 alert = UIAlertController.Create("Login Failed", "Ah, ah, ah, you didn't say the magic word!", UIAlertControllerStyle.Alert);
                             }

                             alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                             PresentViewController(alert, true, null);

                             return Observable.Return(Unit.Default);
                         })
                         .Subscribe()
                         .DisposeWith(disposables);
            });
        }
    }
}
