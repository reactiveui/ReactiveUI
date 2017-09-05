using Conventional.Roslyn;
using Xunit;

namespace Conventions.Security
{
    public class SecurityConventions : IClassFixture<BaseFixture>
    {
        /// <summary>
        /// https://nakedsecurity.sophos.com/2014/02/24/anatomy-of-a-goto-fail-apples-ssl-bug-explained-plus-an-unofficial-patch/
        /// </summary>
        public void IfAndElseMustHaveBracesToReduceRiskOfGotoFail()
        {
            ThisCodebase
                .MustConformTo(RoslynConvention.IfAndElseMustHaveBraces());
        }
    }
}
