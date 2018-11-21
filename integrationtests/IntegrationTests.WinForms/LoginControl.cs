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
    /// <summary>
    /// A control for logging in a user.
    /// </summary>
    public partial class LoginControl : UserControl, IViewFor<LoginViewModel>
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
                       .SelectMany(result =>
                       {
                           if (!result.HasValue)
                           {
                               return Observable.Empty<DialogResult>();
                           }

                           var dialogResult = Task.Run(() => result.Value ?
                               MessageBox.Show("Welcome!", "Login Successful") :
                               MessageBox.Show("Ah, ah, ah, you didn't say the magic word!", "Login Failed"));

                           return dialogResult.ToObservable();
                       })
                       .Subscribe()
                       .DisposeWith(disposables);
                   });
        }

        /// <summary>
        /// Gets or sets the login view model.
        /// </summary>
        public LoginViewModel ViewModel { get; set; }

        /// <inheritdoc />
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (LoginViewModel)value;
        }
    }
}
