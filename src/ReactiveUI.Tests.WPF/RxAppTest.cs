using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Xunit;

namespace ReactiveUI.Tests.WPF
{
    public class RxAppTest
    {
        [Fact]
        public void DepPropNotifierShouldBeFound()
        {
            Assert.True(Locator.Current.GetServices<ICreatesObservableForProperty>()
                               .Any(x => x is DependencyObjectObservableForProperty));
        }
    }
}
