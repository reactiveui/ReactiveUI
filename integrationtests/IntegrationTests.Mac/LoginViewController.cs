using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AppKit;
using Foundation;
using IntegrationTests.Shared;
using ReactiveUI;

namespace IntegrationTests.Mac
{
    public partial class LoginViewController : ReactiveViewController<LoginViewModel>
    {
        public LoginViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new LoginViewModel();

            this.WhenActivated(disposables =>
            {
                /*
                 * Using Bind is unfortunately not possible on NSTextFields, due to KVO not working out of the box:
                 * https://stackoverflow.com/a/13190202/2782141
                 */

                this.OneWayBind(ViewModel, vm => vm.UserName, v => v.UsernameField.StringValue, username => username ?? string.Empty)
                    .DisposeWith(disposables);

                UsernameField.Events().Changed
                             .Select(_ => UsernameField.StringValue)
                             .BindTo(this, v => v.ViewModel.UserName)
                             .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Password, v => v.PasswordField.StringValue, password => password ?? string.Empty)
                    .DisposeWith(disposables);

                PasswordField.Events().Changed
                             .Select(_ => PasswordField.StringValue)
                             .BindTo(this, v => v.ViewModel.Password)
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

                             var alert = new NSAlert();

                             if (result.Value)
                             {
                                 alert.AlertStyle = NSAlertStyle.Informational;
                                 alert.MessageText = "Login Successful";
                                 alert.InformativeText = "Welcome!";
                             }
                             else
                             {
                                 alert.AlertStyle = NSAlertStyle.Critical;
                                 alert.MessageText = "Login Failed";
                                 alert.InformativeText = "Ah, ah, ah, you didn't say the magic word!";
                             }

                             alert.RunModal();

                             return Observable.Return(Unit.Default);
                         })
                         .Subscribe()
                         .DisposeWith(disposables);
            });
        }

        public override NSObject RepresentedObject
        {
            get { return base.RepresentedObject; }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
