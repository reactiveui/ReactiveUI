using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Windows;
using ReactiveUI.Samples.Routing.Interactions;
using ReactiveUI.Samples.Routing.ViewModels;

namespace ReactiveUI.Samples.Routing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MessageInteractions.ShowMessage.RegisterHandler(context =>
            {
                MessageBox.Show(context.Input);
                context.SetOutput(Unit.Default);
            });
        }
    }
}
