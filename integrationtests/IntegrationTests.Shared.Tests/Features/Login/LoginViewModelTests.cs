using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace IntegrationTests.Shared.Tests.Features.Login
{
    /// <summary>
    /// Tests associated with the LoginViewModel class.
    /// </summary>
    public class LoginViewModelTests
    {
        /// <summary>
        /// Checks to make sure that the login button is disabled with default values.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task LoginButton_IsDisabled_ByDefault()
        {
            LoginViewModel sut = new LoginViewModelBuilder();

            var result = await sut.Login.CanExecute.FirstAsync();
            result.Should().BeFalse();
        }

        /// <summary>
        /// Checks to make sure that the login button is disabled with empty password or username values.
        /// </summary>
        /// <param name="userName">The current user name being tested.</param>
        /// <param name="password">The current password being tested.</param>
        /// <returns>A task to monitor the progress.</returns>
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData(" ", "")]
        [InlineData("", " ")]
        [InlineData(" ", " ")]
        public async Task LoginButton_IsDisabled_WhenUserNameOrPassword_IsEmpty(string userName, string password)
        {
            LoginViewModel sut = new LoginViewModelBuilder()
                .WithUserName(userName)
                .WithPassword(password);

            var result = await sut.Login.CanExecute.FirstAsync();
            result.Should().BeFalse();
        }

        /// <summary>
        /// Checks to make sure that the login button is enabled if both the username and password aren't empty.
        /// </summary>
        /// <param name="userName">The current user name being tested.</param>
        /// <param name="password">The current password being tested.</param>
        /// <returns>A task to monitor the progress.</returns>
        [Theory]
        [InlineData("coolusername", "excellentpassword")]
        public async Task LoginButton_IsEnabled_WhenUserNameAndPassword_IsNotEmptyAsync(string userName, string password)
        {
            LoginViewModel sut = new LoginViewModelBuilder()
                .WithUserName(userName)
                .WithPassword(password);

            var result = await sut.Login.CanExecute.FirstAsync();
            result.Should().BeTrue();
        }

        /// <summary>
        /// Checks to make sure that the login button is disabled when not logging in.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task CancelButton_IsDisabled_WhenNot_LoggingIn()
        {
            LoginViewModel sut = new LoginViewModelBuilder();

            var result = await sut.Cancel.CanExecute.FirstAsync();
            result.Should().BeFalse();
        }

        /// <summary>
        /// Checks to make sure that the cancel command actually cancels a login attempt.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task CancelButton_Cancels_Login()
        {
            await new TestScheduler().With(async sched =>
            {
                LoginViewModel sut = new LoginViewModelBuilder()
                    .WithScheduler(sched)
                    .WithUserName("coolusername")
                    .WithPassword("excellentpassword");

                sut.Login.Subscribe(x => x.Should().BeTrue());

                sched.AdvanceByMs(TimeSpan.FromSeconds(1).Milliseconds);

                var result = await sut.Cancel.CanExecute.FirstAsync();
                result.Should().BeTrue();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks to make sure that the cancel button is available within two seconds.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task CancelButton_IsAvailableUntil_TwoSeconds()
        {
            new TestScheduler().With(sched =>
            {
                LoginViewModel sut = new LoginViewModelBuilder()
                    .WithScheduler(sched)
                    .WithUserName("coolusername")
                    .WithPassword("excellentpassword");

                sut.Login.Execute().Subscribe();

                sut.Cancel.CanExecute
                    .FirstAsync().Wait()
                    .Should().BeFalse();

                // 50ms
                sched.AdvanceByMs(50);

                sut.Cancel.CanExecute
                    .FirstAsync().Wait()
                    .Should().BeTrue();

                // 1sec 50ms
                sched.AdvanceByMs(TimeSpan.FromSeconds(1).Milliseconds);

                sut.Cancel.CanExecute
                    .FirstAsync().Wait()
                    .Should().BeTrue();

                // 2sec 50sms
                sched.AdvanceByMs(TimeSpan.FromSeconds(1).Milliseconds);

                sut.Cancel.CanExecute
                    .FirstAsync().Wait()
                    .Should().BeFalse();
            });
        }

        /// <summary>
        /// Checks to make sure the user cannot login with a incorrect password.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task User_CannotLogin_WithIncorrect_Password()
        {
            LoginViewModel sut = new LoginViewModelBuilder()
                .WithUserName("coolusername")
                .WithPassword("incorrectpassword");

            Assert.False(true);
        }

        /// <summary>
        /// Checks to make sure the user can login with a correct password.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task User_CanLogin_WithCorrect_Password()
        {
            LoginViewModel sut = new LoginViewModelBuilder()
                .WithUserName("coolusername")
                .WithPassword("Mr. Goodbytes");

            Assert.False(true);
        }
    }
}
