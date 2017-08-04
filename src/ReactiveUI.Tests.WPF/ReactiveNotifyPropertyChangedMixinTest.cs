using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xunit;

namespace ReactiveUI.Tests.WPF
{
    public class HostTestView : Control, IViewFor<HostTestFixture>
    {
        public HostTestFixture ViewModel
        {
            get { return (HostTestFixture)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(HostTestFixture), typeof(HostTestView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (HostTestFixture)value; }
        }
    }

    public class WhenAnyThroughDependencyObjectTests
    {
        [Fact]
        public void WhenAnyThroughAViewShouldntGiveNullValues()
        {
            var vm = new HostTestFixture()
            {
                Child = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" },
            };

            var fixture = new HostTestView();

            var output = new List<string>();

            Assert.Equal(0, output.Count);
            Assert.Null(fixture.ViewModel);

            fixture.WhenAnyValue(x => x.ViewModel.Child.IsNotNullString).Subscribe(output.Add);

            fixture.ViewModel = vm;
            Assert.Equal(1, output.Count);

            fixture.ViewModel.Child.IsNotNullString = "Bar";
            Assert.Equal(2, output.Count);
            new[] { "Foo", "Bar" }.AssertAreEqual(output);
        }
    }
}
