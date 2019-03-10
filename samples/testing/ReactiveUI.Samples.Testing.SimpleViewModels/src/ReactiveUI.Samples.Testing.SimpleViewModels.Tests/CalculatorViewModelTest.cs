using ReactiveUI.Samples.Testing.SimpleViewModels;
using Xunit;

namespace ReactiveUI.Samples.Testing.SimpleViewModelsUnitTests
{
    /// <summary>
    /// A few sample unit tests. Note how simple they are: since the concept of time
    /// is not involved we do not have to model it in our unit tests. We test these almost
    /// as if they were a non ReactiveUI object.
    /// </summary>
    public class CalculatorViewModelTest
    {
        [Fact]
        public void TestTypingStringGetsError()
        {
            var fixture = new CalculatorViewModel();
            fixture.InputText = "hi";
            Assert.Equal("Error", fixture.ErrorText);
            Assert.Equal("", fixture.ResultText);
        }

        [Fact]
        public void TestTypingInteger()
        {
            var fixture = new CalculatorViewModel();
            fixture.InputText = "50";
            Assert.Equal("", fixture.ErrorText);
            Assert.Equal("100", fixture.ResultText);
        }
    }
}
