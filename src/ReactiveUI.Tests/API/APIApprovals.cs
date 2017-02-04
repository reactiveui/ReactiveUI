using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using PublicApiGenerator;
using Xunit;

namespace ReactiveUI.Tests
{
    [ExcludeFromCodeCoverage]
    public class APIApprovals
    {

        [Fact]
        public void Blend()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(ReactiveUI.Blend.ObservableTrigger).Assembly));

            Approvals.Verify(publicApi);
        }

        [Fact]
        public void Testing()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(ReactiveUI.Testing.TestUtils).Assembly));
            Approvals.Verify(publicApi);
        }

        [Fact]
        public void NET45()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(ReactiveUI.RxApp).Assembly));
            Approvals.Verify(publicApi);
        }

        [Fact]
        public void Winforms()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(ReactiveUI.Winforms.WinformsCreatesObservableForProperty).Assembly));

            Approvals.Verify(publicApi);
        }

        string Filter(string text)
        {
            return string.Join(Environment.NewLine, text.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.StartsWith("[assembly: AssemblyVersion("))
                .Where(l => !l.StartsWith("[assembly: AssemblyFileVersion("))
                .Where(l => !l.StartsWith("[assembly: AssemblyInformationalVersion("))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                );
        }

    }
}
