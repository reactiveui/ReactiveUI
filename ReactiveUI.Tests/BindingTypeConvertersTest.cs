using Xunit;

namespace ReactiveUI.Tests
{
    public class BindingTypeConvertersTest
    {
        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertNullableValues()
        {
            double? nullableDouble = 0.0;
            var result = EqualityTypeConverter.DoReferenceCast<double?>(nullableDouble);
            Assert.Equal(nullableDouble, result);
        }

        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertValueTypes()
        {
            double doubleValue = 0.0;
            var result = EqualityTypeConverter.DoReferenceCast<double>(doubleValue);
            Assert.Equal(doubleValue, result);
        }
    }
}