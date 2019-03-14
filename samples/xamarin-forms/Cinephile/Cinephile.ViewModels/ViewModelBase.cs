// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;
using Splat;

namespace Cinephile.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        public string UrlPathSegment
        {
            get;
            private set;
        }

        public IScreen HostScreen
        {
            get;
            private set;
        }

        protected readonly ViewModelActivator viewModelActivator = new ViewModelActivator();
        public ViewModelActivator Activator => viewModelActivator;

        protected readonly Interaction<AlertViewModel, Unit> _showAlert;
        public Interaction<AlertViewModel, Unit> ShowAlert => _showAlert;

        protected readonly Interaction<string, Unit> _openBrowser;
        public Interaction<string, Unit> OpenBrowser => _openBrowser;


        protected readonly IScheduler _mainThreadScheduler;
        protected readonly IScheduler _taskPoolScheduler;

        protected ViewModelBase(string title, IScheduler mainThreadScheduler = null, IScheduler taskPoolScheduler = null, IScreen hostScreen = null)
        {
            UrlPathSegment = title;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            // Set the schedulers like this so we can inject the test scheduler later on when doing VM unit tests
            _mainThreadScheduler = mainThreadScheduler ?? RxApp.MainThreadScheduler;
            _taskPoolScheduler = taskPoolScheduler ?? RxApp.TaskpoolScheduler;

            _showAlert = new Interaction<AlertViewModel, Unit>(_mainThreadScheduler);
            _openBrowser = new Interaction<string, Unit>(_mainThreadScheduler);
        }
    }
}

