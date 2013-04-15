using RxUIViewModelGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ViewModelRendererTests : IEnableLogger
    {
        const string testTemplate = "";

        [Fact]
        public void ShouldThrowOnEmptyStringOrGarbage()
        {
            var inputs = new[] {
                "###woefowaefjawioefj",
                "",
            };

            inputs.ForEach(x => {
                Assert.Throws<ArgumentException>(() => {
                    var fixture = new ScaffoldRenderer();
                    fixture.RenderGeneratedViewModel(x);
                });
            });
        }

        [Fact]
        public void ParseInterfacesSmokeTest()
        {
            var fixture = new ScaffoldRenderer();

            var f = new StackTrace(true).GetFrame(0);
            var dir = Path.GetDirectoryName(f.GetFileName());
            string result = fixture.RenderGeneratedViewModel(File.ReadAllText(Path.Combine(dir, "TestInterface.cs.txt")));
            this.Log().Info(result);

            Assert.Contains("IRoutableViewModel", result);
            Assert.Contains("ObservableAsPropertyHelper", result);
            Assert.Contains("HostScreen", result);
        }

        [Fact]
        public void RenderXamlControlsSmokeTest()
        {
            var fixture = new ScaffoldRenderer();

            var f = new StackTrace(true).GetFrame(0);
            var dir = Path.GetDirectoryName(f.GetFileName());
            var results = fixture.RenderUserControlXaml(File.ReadAllText(Path.Combine(dir, "TestInterface.cs.txt")));

            Assert.Equal(2, results.Count());
        }
    }
}
