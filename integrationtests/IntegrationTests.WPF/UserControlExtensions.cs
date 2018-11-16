using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace IntegrationTests.WPF
{
    /// <summary>
    /// Extension methods associated with the UserControl class.
    /// </summary>
    public static class UserControlExtensions
    {
        /// <summary>
        /// Shows a message to the user, and have the results wrapped in a observable.
        /// </summary>
        /// <param name="this">The user control that hosts the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to show to the user.</param>
        /// <param name="style">The style settings of the message box.</param>
        /// <param name="settings">General settings of the message box.</param>
        /// <returns>An observable of the result from the message box.</returns>
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
