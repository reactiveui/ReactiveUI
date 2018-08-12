using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IntegrationTests.WPF
{
    public static class UserControlExtensions
    {
        public static IObservable<MessageDialogResult> ShowMessage(
            this UserControl @this,
            string title,
            string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative,
            MetroDialogSettings settings = null)
        {
            var window = (MetroWindow)Window.GetWindow(@this);
            return window
                .ShowMessageAsync(title, message, style, settings)
                .ToObservable();
        }
    }
}
