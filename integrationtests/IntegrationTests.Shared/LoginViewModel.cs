using Genesis.Ensure;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace IntegrationTests.Shared
{
    /// <summary>
    /// View model for login functionality.
    /// </summary>
    /// <seealso cref="ReactiveUI.ReactiveObject" />
    public class LoginViewModel : ReactiveObject
    {
        private string _userName;
        private string _password;
        private IScheduler _mainScheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="mainScheduler">The main scheduler.</param>
        public LoginViewModel(IScheduler mainScheduler)
        {
            Ensure.ArgumentNotNull(mainScheduler, nameof(mainScheduler));

            _mainScheduler = mainScheduler;

            var canLogin = this
                .WhenAnyValue(
                    vm => vm.UserName,
                    vm => vm.Password,
                    (user, password) => !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(password));

            Login = ReactiveCommand.CreateFromObservable(
                () =>
                    LoginInternal().TakeUntil(Cancel),
                canLogin, _mainScheduler);

            Cancel = ReactiveCommand.Create(() => { }, Login.IsExecuting, _mainScheduler);
        }

        public ReactiveCommand<Unit, bool> Login { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private IObservable<bool> LoginInternal()
        {
            return Observable.Return(Password == "Mr. Goodbytes").Delay(TimeSpan.FromSeconds(2), _mainScheduler);
        }
    }
}
