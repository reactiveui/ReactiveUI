using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI.Xaml;
using Xunit;

namespace ReactiveUI.Tests
{
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
            using (UserError.RegisterHandler(x => RecoveryOptionResult.RetryOperation)) 
            {
                var result = UserError.Throw("This should catch!");
                Assert.Equal(RecoveryOptionResult.RetryOperation, result);
            }

            Assert.Throws<UnhandledUserErrorException>(() => UserError.Throw("This should throw!"));
        }

        [Fact]
        public void NestedHandlersShouldFireInANestedWay()
        {
            RecoveryOptionResult result;
            using (UserError.RegisterHandler(x => RecoveryOptionResult.CancelOperation)) {

                using (UserError.RegisterHandler(x => RecoveryOptionResult.RetryOperation)) {
                    result = UserError.Throw("This should catch!");
                    Assert.Equal(RecoveryOptionResult.RetryOperation, result);
                }

                result = UserError.Throw("This should catch!");
                Assert.Equal(RecoveryOptionResult.CancelOperation, result);
            }

            Assert.Throws<UnhandledUserErrorException>(() => UserError.Throw("This should throw!"));
        }
    }
}
