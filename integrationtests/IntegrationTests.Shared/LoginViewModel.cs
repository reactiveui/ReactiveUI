using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTests.Shared
{
    public class LoginViewModel : ReactiveObject
    {
        private string _userName;
        private string _password;

        public LoginViewModel()
        {
            var canLogin = this
                .WhenAnyValue(
                    vm => vm.UserName,
                    vm => vm.Password,
                    (user, password) => !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(password)
                );

            Login = ReactiveCommand.CreateFromObservable(
                () =>
                    Observable
                        .StartAsync(LoginAsync)
                        .TakeUntil(Cancel),
                canLogin);

            Cancel = ReactiveCommand.Create(() => { }, Login.IsExecuting);
        }

        public ReactiveCommand<Unit, bool?> Login { get; }

        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private async Task<bool?> LoginAsync(CancellationToken ct)
        {
            var result = Password == "Mr. Goodbytes";
            await Task.Delay(TimeSpan.FromSeconds(1.5), ct);

            return result;
        }
    }
}