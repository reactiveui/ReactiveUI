using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI.Xaml;
using Xunit;

namespace ReactiveUI.Tests
{
    class MyAwesomeUserError : UserError
    {
        public MyAwesomeUserError() : base("Blargh") {}
    }

    public class ErrorsTest
    { 
        [Fact]
        public void UnhandledUserErrorsShouldDie()
        {
            // Since we haven't registered any user error handler
            Assert.Throws<UnhandledUserErrorException>(() => UserError.Throw("Something Bad Has Happened"));
        }

        [Fact]
        public void HandledUserErrorsShouldNotThrow() 
        {
            using (UserError.RegisterHandler(x => Observable.Return(RecoveryOptionResult.RetryOperation))) {
                var result = UserError.Throw("This should catch!").First();
                Assert.Equal(RecoveryOptionResult.RetryOperation, result);
            }

            Assert.Throws<UnhandledUserErrorException>(() => UserError.Throw("This should throw!"));
        }

        [Fact]
        public void NestedHandlersShouldFireInANestedWay()
        {
            RecoveryOptionResult result;
            using (UserError.RegisterHandler(x => Observable.Return(RecoveryOptionResult.CancelOperation))) {

                using (UserError.RegisterHandler(x => Observable.Return(RecoveryOptionResult.RetryOperation))) {
                    result = UserError.Throw("This should catch!").First();
                    Assert.Equal(RecoveryOptionResult.RetryOperation, result);
                }

                result = UserError.Throw("This should catch!").First();
                Assert.Equal(RecoveryOptionResult.CancelOperation, result);
            }

            Assert.Throws<UnhandledUserErrorException>(() => UserError.Throw("This should throw!"));
        }

        [Fact]
        public void TypeSpecificFiltersShouldntFireOnOtherExceptions()
        {
            using (UserError.RegisterHandler<MyAwesomeUserError>(x => Observable.Return(RecoveryOptionResult.CancelOperation))) {
                var result = UserError.Throw(new MyAwesomeUserError()).First();
                Assert.Equal(RecoveryOptionResult.RetryOperation, result);

                Assert.Throws<UnhandledUserErrorException>(() => UserError.Throw("This should throw!"));
            }

            Assert.Throws<UnhandledUserErrorException>(() => UserError.Throw(new MyAwesomeUserError()));
        }
    }
}
