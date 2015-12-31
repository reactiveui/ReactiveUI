using Xunit;

namespace ReactiveUI.Tests
{
    public class BindingTypeConvertersTest
    {
        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertNullableValues()
        {
            double? nullDouble = null;
            double? expected = 0.0;
            var result = EqualityTypeConverter.DoReferenceCast(nullDouble, typeof(double));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertValueTypes()
        {
            double doubleValue = 0.0;
            var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double));
            Assert.Equal(doubleValue, result);
        }
    }
}