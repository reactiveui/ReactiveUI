using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Forms;
using IntegrationTests.Shared;
using ReactiveUI;

namespace IntegrationTests.WinForms
{
    public partial class LoginControl : UserControl, IViewFor<LoginViewModel>
    {
        public LoginControl()
        {
            InitializeComponent();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);
            this
               .WhenActivated(
                   disposables => {

                       this
                           .Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                           .DisposeWith(disposables);
                       this
                           .Bind(ViewModel, vm => vm.Password, v => v.Password.Text)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                           .DisposeWith(disposables);


                       ViewModel
                       .Login
                       .SelectMany(result => {
                           if (!result.HasValue) {
                               return Observable.Empty<DialogResult>();
                           }

                           if (result.Value) {
                               var dialogResult = Task.Run(() => MessageBox.Show("Welcome!", "Login Successful"));
                               return dialogResult.ToObservable();
                           } else {
                               var dialogResult = Task.Run(() => MessageBox.Show("Ah, ah, ah, you didn't say the magic word!", "Login Failed"));
                               return dialogResult.ToObservable();
                           }
                       })
                       .Subscribe()
                       .DisposeWith(disposables);
                   });

        }

        public LoginViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (LoginViewModel)value;
        }
    }
}
