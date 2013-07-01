using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ActionbarSherlock.App;

namespace ReactiveUI.Android
{
    public sealed class ActivityRoutedViewHost : IDisposable
    {
        IDisposable _inner;
        IScreen _hostScreen;

        public ActivityRoutedViewHost(Activity hostActivity, IViewLocator viewLocator = null)
        {
            viewLocator = viewLocator ?? ViewLocator.Current;
            var platform = RxApp.DependencyResolver.GetService<IPlatformOperations>();

            var keyUp = hostActivity.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(x => x.Name == "OnKeyUp");

            if (keyUp == null) {
                throw new Exception("You must override OnKeyUp and call theRoutedViewHost.OnKeyUp");
            }

            var viewFor = hostActivity as IViewFor;
            if (viewFor == null) {
                throw new Exception("You must implement IViewFor<TheViewModelClass>");
            }

            bool firstSet = false;

            _inner = _hostScreen.Router.CurrentViewModel
                .Where(x => x != null)
                .Subscribe(vm => {
                    if (!firstSet) {
                        viewFor.ViewModel = vm;
                        firstSet = true;
                        return;
                    }

                    var view = viewLocator.ResolveView(vm, platform.GetOrientation());
                    if (view.GetType() != typeof(Type)) {
                        throw new Exception("Views in Android must be the Type of an Activity");
                    }
                    
                    hostActivity.StartActivity((Type)view);
                });
        }

        public bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            if (keyCode != Keycode.Back) return false;

            if (!_hostScreen.Router.NavigateBack.CanExecute(null)) return false;

            if (_inner == null) return false;

            _hostScreen.Router.NavigateBack.Execute(null);
            return true;
        }

        public void Dispose()
        {
            var disp = Interlocked.Exchange(ref _inner, null);
            if (disp != null) disp.Dispose();
        }
    }
}

