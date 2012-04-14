using System;

namespace System.Windows.Input
{
	public interface ICommand
	{
		bool CanExecute(object parameter);
		void Execute(object parameter);
		event EventHandler CanExecuteChanged;
	}
}