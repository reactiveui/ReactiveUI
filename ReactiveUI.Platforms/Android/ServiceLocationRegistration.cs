using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace ReactiveUI.Android
{
    /// <summary>
    /// Ignore me. This class is a secret handshake between RxUI and RxUI.Xaml
    /// in order to register certain classes on startup that would be difficult
    /// to register otherwise.
    /// </summary>
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register()
        {
        }
    }
}
