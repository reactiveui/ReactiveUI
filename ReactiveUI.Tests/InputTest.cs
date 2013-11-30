using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveUI.Tests
{
    public class InputScopeTests
    {
        [Fact]
        public void InputScopeShouldInvokeCommandsInReverseOrder()
        {
            var view = new FakeView() { ViewModel = new FakeViewModel(), };
            var fixture = new KeyboardManager();

            fixture.Register(
                new InputSection("Foo",
                    view.GetInputCommand(x => x.ViewModel.Cmd, "Ctrl-C")));

            fixture.Register(
                new InputSection("Bar",
                    view.GetInputCommand(x => x.ViewModel.Cmd2, "Ctrl-C")));

            var output = Observable.Merge(
                    view.ViewModel.Cmd.Select(_ => "Foo"),
                    view.ViewModel.Cmd2.Select(_ => "Bar"))
                .CreateCollection();

            fixture.InvokeShortcut("Ctrl-C");
            Assert.Equal(1, output.Count);
            Assert.Equal("Bar", output[0]);
        }

        [Fact]
        public void InputScopeShouldntBlowUpOnMissingShortcut()
        {
            var view = new FakeView() { ViewModel = new FakeViewModel(), };
            var fixture = new KeyboardManager();

            fixture.Register(
                new InputSection("Foo",
                    view.GetInputCommand(x => x.ViewModel.Cmd, "Ctrl-C")));

            fixture.Register(
                new InputSection("Bar",
                    view.GetInputCommand(x => x.ViewModel.Cmd2, "Ctrl-C")));

            var output = Observable.Merge(
                    view.ViewModel.Cmd.Select(_ => "Foo"),
                    view.ViewModel.Cmd2.Select(_ => "Bar"))
                .CreateCollection();

            fixture.InvokeShortcut("Ctrl-R");
            Assert.Equal(0, output.Count);
        }
    }
}
