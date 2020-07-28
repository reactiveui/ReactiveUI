using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ReactiveUI.Tests.Wpf
{
    public class FakeXamlCommandBindingView : IViewFor<CommandBindingViewModel>
    {
        private readonly Button _buttonDeclaredInXaml;

        public FakeXamlCommandBindingView()
        {
            _buttonDeclaredInXaml = new Button();

            this.BindCommand(ViewModel!, vm => vm!.Command2!, v => v._buttonDeclaredInXaml);
        }

        public string NameOfButtonDeclaredInXaml => nameof(_buttonDeclaredInXaml);

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CommandBindingViewModel?)value;
        }

        public CommandBindingViewModel? ViewModel { get; set; }
    }
}
