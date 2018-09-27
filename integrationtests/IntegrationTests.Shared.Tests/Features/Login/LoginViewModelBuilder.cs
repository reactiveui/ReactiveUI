using System.Reactive.Concurrency;

namespace IntegrationTests.Shared.Tests.Features.Login
{
    /// <summary>
    /// A builder which will build a login view.
    /// </summary>
    public sealed class LoginViewModelBuilder : IBuilder
    {
        private string _userName;
        private string _password;
        private IScheduler _mainScheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModelBuilder"/> class.
        /// </summary>
        public LoginViewModelBuilder()
        {
            _mainScheduler = CurrentThreadScheduler.Instance;
        }

        /// <summary>
        /// Converts the builder into a LoginViewModel using automatic casting.
        /// </summary>
        /// <param name="builder">The builder instance to convert into a LoginViewModel.</param>
        public static implicit operator LoginViewModel(LoginViewModelBuilder builder) => builder.ToLoginViewModel();

        /// <summary>
        /// Logs the user in with the specified user name.
        /// </summary>
        /// <param name="username">The username to log the user in with.</param>
        /// <returns>The current builder instance.</returns>
        public LoginViewModelBuilder WithUserName(string username) => this.With(ref _userName, username);

        /// <summary>
        /// Logs the user in with the specified password.
        /// </summary>
        /// <param name="password">The password to log the user in with.</param>
        /// <returns>The current builder instance.</returns>
        public LoginViewModelBuilder WithPassword(string password) => this.With(ref _password, password);

        /// <summary>
        /// Performs the login interactions on the specified scheduler.
        /// </summary>
        /// <param name="mainScheduler">The scheduler to perform the login actions with.</param>
        /// <returns>The current builder instance.</returns>
        public LoginViewModelBuilder WithScheduler(IScheduler mainScheduler) => this.With(ref _mainScheduler, mainScheduler);

        /// <summary>
        /// Builds a LoginViewModel based on the specified builder parameters.
        /// </summary>
        /// <returns>A LoginViewModel instance.</returns>
        public LoginViewModel ToLoginViewModel()
        {
            return new LoginViewModel(_mainScheduler)
            {
                UserName = _userName,
                Password = _password,
            };
        }
    }
}
