using ReactiveUI;
using ReactiveUI.Xaml;
using Xunit;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ReactiveUI.Tests
{
    public class ValidatedTestFixture : ReactiveValidatedObject
    {
        public ValidatedTestFixture()
        {
            this.ValidateViaAttributes();
        }

        public string _IsNotNullString;
        [Required]
        public string IsNotNullString {
            get { return _IsNotNullString; }
            set { this.RaiseAndSetIfChanged(x => x.IsNotNullString, value); }
        }
        
        public string _IsOnlyOneWord;
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string IsOnlyOneWord {
            get { return _IsOnlyOneWord; }
            set { this.RaiseAndSetIfChanged(x => x.IsOnlyOneWord, value); }
        }

        public string _UsesExprRaiseSet;
        public string UsesExprRaiseSet {
            get { return _UsesExprRaiseSet; }
            set { _UsesExprRaiseSet = this.RaiseAndSetIfChanged(x => x.UsesExprRaiseSet, value); }
        }
    }

    public class ValidatedIgnoresStaticPropertyTestFixture : ReactiveValidatedObject
    {
        public static int StaticProperty
        {
            get { return 5; }
        }

        public string NonStaticProperty
        {
            get { return "A string"; }
        }
    }

    public class ReactiveValidatedObjectTest
    {
        [Fact]
        public void IsObjectValidTest()
        {
            var output = new List<bool>();
            var fixture = new ValidatedTestFixture();
            //fixture.IsValidObservable.Subscribe(output.Add);

            Assert.True(fixture.HasErrors);

            fixture.IsNotNullString = "foo";
            Assert.True(fixture.HasErrors);

            fixture.IsOnlyOneWord = "Foo Bar";
            Assert.True(fixture.HasErrors);

            fixture.IsOnlyOneWord = "Foo";
            Assert.False(fixture.HasErrors);

            fixture.IsOnlyOneWord = "";
            Assert.True(fixture.HasErrors);

            /*
            new[] { false, false, false, true, false }.Zip(output, (expected, actual) => new { expected, actual })
                .Do(Console.WriteLine)
                .ForEach(x => Assert.Equal(x.expected, x.actual));
             */
        }

        [Fact]
        public void IgnoresStaticPropertiesTest()
        {
            var fixture = new ValidatedIgnoresStaticPropertyTestFixture();

            Assert.DoesNotThrow(delegate
            {
                var error = fixture.GetErrors("NonStaticProperty");
            });
        }
    }
}
