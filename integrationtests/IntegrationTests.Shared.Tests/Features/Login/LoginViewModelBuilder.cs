using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;

namespace IntegrationTests.Shared.Tests.Features.Login
{
    public sealed class LoginViewModelBuilder : IBuilder
    {
        private string _userName;
        private string _password;
        private IScheduler _mainScheduler;

        public LoginViewModelBuilder()
        {
            _mainScheduler = CurrentThreadScheduler.Instance;
        }

        public LoginViewModelBuilder WithUserName(string username) => this.With(ref _userName, username);
        public LoginViewModelBuilder WithPassword(string password) => this.With(ref _password, password);

        public LoginViewModelBuilder WithScheduler(IScheduler mainScheduler) => this.With(ref _mainScheduler, mainScheduler);

        public LoginViewModel Build()
        {
            var result = new LoginViewModel(_mainScheduler)
            {
                UserName = _userName,
                Password = _password,
            };

            return result;
        }

        public static implicit operator LoginViewModel(LoginViewModelBuilder builder) =>
            builder.Build();
    }
}
