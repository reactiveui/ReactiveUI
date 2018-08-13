using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace IntegrationTests.Shared.Tests.Features.Login
{
    public class LoginViewModelTests
    {
        [Fact]
        public void LoginButton_IsDisabled_ByDefault()
        {
            var sut = new LoginViewModelBuilder()
                .Build();

            sut.Login.CanExecute
                .FirstAsync().Wait()
                .Should().BeFalse();;
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData(" ", "")]
        [InlineData("", " ")]
        [InlineData(" ", " ")]
        public void LoginButton_IsDisabled_WhenUserNameOrPassword_IsEmpty(string userName, string password)
        {
            var sut = new LoginViewModelBuilder()
                .WithUserName(userName)
                .WithPassword(password)
                .Build();

            sut.Login.CanExecute
                .FirstAsync().Wait()
                .Should().BeFalse();;
        }

        [Theory]
        [InlineData("coolusername", "excellentpassword")]
        public void LoginButton_IsEnabled_WhenUserNameAndPassword_IsNotEmptyAsync(string userName, string password)
        {
            var sut = new LoginViewModelBuilder()
                .WithUserName(userName)
                .WithPassword(password)
                .Build();

            sut.Login.CanExecute
                .FirstAsync().Wait()
                .Should().BeTrue();
        }

        [Fact]
        public void CancelButton_IsDisabled_WhenNot_LoggingIn()
        {
            var sut = new LoginViewModelBuilder()
                .Build();

            sut.Cancel.CanExecute
                .FirstAsync().Wait()
                .Should().BeFalse();;
        }

        [Fact]
        public void CancelButton_Cancels_Login()
        {
            (new TestScheduler()).With(sched => {

                var sut = new LoginViewModelBuilder()
                    .WithScheduler(sched)
                    .WithUserName("coolusername")
                    .WithPassword("excellentpassword")
                    .Build();

                sut.Login.Subscribe(x => {
                    x.Should().BeTrue();
                });

                //sut.Login.Execute();

                sched.AdvanceByMs(TimeSpan.FromSeconds(1).Milliseconds);

                sut.Cancel.CanExecute
                    .FirstAsync().Wait()
                    .Should().BeTrue();

                //sut.Cancel.Execute();
            });
        }

        [Fact]
        public void CancelButton_IsAvailableUntil_TwoSeconds()
        {
            (new TestScheduler()).With(sched => {

                var sut = new LoginViewModelBuilder()
                    .WithScheduler(sched)
                    .WithUserName("coolusername")
                    .WithPassword("excellentpassword")
                    .Build();

                sut.Login.Execute().Subscribe();

                sut.Cancel.CanExecute
                    .FirstAsync().Wait()
                    .Should().BeFalse();;

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
                    .Should().BeFalse();;
            });
        }

        [Fact]
        public void User_CannotLogin_WithIncorrect_Password()
        {
            var sut = new LoginViewModelBuilder()
                .WithUserName("coolusername")
                .WithPassword("incorrectpassword")
                .Build();


            Assert.False(true);

        }

        [Fact]
        public void User_CanLogin_WithCorrect_Password()
        {
            var sut = new LoginViewModelBuilder()
                .WithUserName("coolusername")
                .WithPassword("Mr. Goodbytes")
                .Build();

            Assert.False(true);
            
        }

    }
}
