using Genesis.Ensure;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTests.Shared
{
    public class LoginViewModel : ReactiveObject
    {
        private string _userName;
        private string _password;
        private IScheduler _mainScheduler;

        public LoginViewModel(IScheduler mainScheduler)
        {
            Ensure.ArgumentNotNull(mainScheduler, nameof(mainScheduler));

            _mainScheduler = mainScheduler;

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
                canLogin, _mainScheduler);

            Cancel = ReactiveCommand.Create(() => { }, Login.IsExecuting, _mainScheduler);
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
            await Task.Delay(TimeSpan.FromSeconds(2), ct);

            return result;
        }
    }
}
