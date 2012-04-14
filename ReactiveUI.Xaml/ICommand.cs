using System;

namespace ReactiveUI.Xaml
{
	public interface ICommand
	{
		bool CanExecute(object parameter);
		void Execute(object parameter);
		event EventHandler CanExecuteChanged;
	}
}