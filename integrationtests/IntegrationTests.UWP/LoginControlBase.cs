using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using IntegrationTests.Shared;
using ReactiveUI;
using Windows.UI.Popups;

namespace IntegrationTests.UWP
{
    /// <summary>
    /// A base level control for logging the user in.
    /// </summary>
    public class LoginControlBase : ReactiveUserControl<LoginViewModel>
    {
    }
}
