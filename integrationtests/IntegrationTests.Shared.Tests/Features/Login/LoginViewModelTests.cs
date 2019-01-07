﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
using Shouldly;
using Xunit;

namespace IntegrationTests.Shared.Tests.Features.Login
{
    /// <summary>
    /// Tests associated with the LoginViewModel class.
    /// </summary>
    public class LoginViewModelTests
    {
        /// <summary>
        /// Checks to make sure that the cancel command actually cancels a login attempt.
        /// </summary>
        [Fact]
        public void CancelButton_Cancels_Login()
        {
            var scheduler = new TestScheduler();

            LoginViewModel sut = new LoginViewModelBuilder()
                .WithScheduler(scheduler)
                    .WithUserName("coolusername")
                    .WithPassword("excellentpassword");

            scheduler.AdvanceByMs(TimeSpan.FromSeconds(1).Milliseconds);

            sut.Login.Subscribe(x => x.ShouldBe(true));

            Observable
                .Return(Unit.Default)
                .InvokeCommand(sut.Login);

            sut.Cancel.CanExecute.Subscribe(x => x.ShouldBe(true));

            scheduler.AdvanceByMs(1000);

            Observable
                .Return(Unit.Default)
                .InvokeCommand(sut.Cancel);
        }

        /// <summary>
        /// Checks to make sure that the cancel button is available within two seconds.
        /// </summary>
        [Fact]
        public void CancelButton_IsAvailableUntil_TwoSeconds()
        {
            var actual = false;
            var scheduler = new TestScheduler();

            LoginViewModel sut = new LoginViewModelBuilder()
                .WithScheduler(scheduler)
                .WithUserName("coolusername")
                .WithPassword("excellentpassword");

            sut.Cancel.CanExecute.Subscribe(x =>
            {
                actual = x;
            });

            Observable.Return(Unit.Default).InvokeCommand(sut.Login);

            actual.ShouldBe(false);

            // 50ms
            scheduler.AdvanceByMs(50);

            actual.ShouldBe(true);

            // 1sec 50ms
            scheduler.AdvanceByMs(TimeSpan.FromSeconds(1).TotalMilliseconds);

            actual.ShouldBe(true);

            // 2sec 50sms
            scheduler.AdvanceByMs(TimeSpan.FromSeconds(1).TotalMilliseconds);

            actual.ShouldBe(false);
        }

        /// <summary>
        /// Checks to make sure that the login button is disabled when not logging in.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task CancelButton_IsDisabled_WhenNot_LoggingIn()
        {
            LoginViewModel sut = new LoginViewModelBuilder();

            (await sut.Cancel.CanExecute.FirstAsync()).ShouldBe(false);
        }

        /// <summary>
        /// Checks to make sure that the login ticks correctly and the action is performed.
        /// </summary>
        [Fact]
        public void CanLogin_TicksCorrectly()
        {
            var scheduler = new TestScheduler();
            LoginViewModel sut = new LoginViewModelBuilder()
                .WithScheduler(scheduler)
                .WithUserName("coolusername")
                .WithPassword("Mr. Goodbytes");

            sut.Cancel.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var collection).Subscribe();

            Observable.Return(Unit.Default).InvokeCommand(sut.Login);

            scheduler.AdvanceByMs(TimeSpan.FromSeconds(5).TotalMilliseconds);

            collection.ToList().ShouldBe(new[] { false, true, false });
        }

        /// <summary>
        /// Checks to make sure that the login button is disabled with default values.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task LoginButton_IsDisabled_ByDefault()
        {
            LoginViewModel sut = new LoginViewModelBuilder();

            var result = await sut.Login.CanExecute.FirstAsync();
            result.ShouldBe(false);
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

            (await sut.Login.CanExecute.FirstAsync()).ShouldBe(false);
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

            (await sut.Login.CanExecute.FirstAsync()).ShouldBe(true);
        }

        /// <summary>
        /// Checks to make sure the user can login with a correct password.
        /// </summary>
        [Fact]
        public void User_CanLogin_WithCorrect_Password()
        {
            var scheduler = new TestScheduler();
            LoginViewModel sut = new LoginViewModelBuilder()
                .WithScheduler(scheduler)
                .WithUserName("coolusername")
                .WithPassword("Mr. Goodbytes");

            bool? value = null;
            sut.Login.Subscribe(x => value = x);

            Observable.Return(Unit.Default).InvokeCommand(sut.Login);

            scheduler.AdvanceByMs(TimeSpan.FromSeconds(3).TotalMilliseconds);

            value.ShouldBe(true);
        }

        /// <summary>
        /// Checks to make sure the user cannot login with a incorrect password.
        /// </summary>
        [Fact]
        public void User_CannotLogin_WithIncorrect_Password()
        {
            var scheduler = new TestScheduler();
            LoginViewModel sut = new LoginViewModelBuilder()
                .WithScheduler(scheduler)
                .WithUserName("coolusername")
                .WithPassword("incorrectpassword");

            bool? value = null;
            sut.Login.Subscribe(x => value = x);

            Observable.Return(Unit.Default).InvokeCommand(sut.Login);

            scheduler.AdvanceByMs(TimeSpan.FromSeconds(3).TotalMilliseconds);

            value.ShouldBe(false);
        }
    }
}
