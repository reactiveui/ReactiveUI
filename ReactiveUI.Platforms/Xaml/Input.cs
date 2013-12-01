using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINRT
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;

using Key = Windows.System.VirtualKey;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
using System.Diagnostics;
#else
using System.Windows;
using System.Windows.Input;
#endif

namespace ReactiveUI.Xaml
{
    public static class InputMixins
    {
        public static IObservable<IInputCommand> BindInputCommand<TViewModel>(this IViewFor<TViewModel> This, Expression<Func<IViewFor<TViewModel>, IReactiveCommand>> command, ModifierKeys modifiers, Key key, string description = null)
            where TViewModel : class
        {
            var keyEvent = Observable.Never<Unit>();
            var element = This as UIElement;

            if (element == null) {
                This.Log().Warn("Attempted to bind command '{0}' to a non-UIControl, command will never invoke!", description ?? "(none)");
            } else {
#if WINRT
                // NB: This code probably doesn't work. WinRT is deeply,
                // thoroughly broken, in every possible way.
                keyEvent = Observable.FromEventPattern<KeyEventHandler, KeyRoutedEventArgs>(x => element.KeyUp += x, x => element.KeyUp -= x)
                    .Where(x => x.EventArgs.Key == key && getCurrentModifiers() == modifiers)
                    .Select(_ => Unit.Default);
#else
                keyEvent = Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => element.KeyUp += x, x => element.KeyUp -= x)
                    .Where(x => x.EventArgs.Key == key && x.EventArgs.KeyboardDevice.Modifiers == modifiers)
                    .Select(_ => Unit.Default);
#endif
            }

            var ret = Observable.Create<IInputCommand>(subj => {
                var shortcut = keysToShortcutName(modifiers, key);

                return new CompositeDisposable(
                    This.WhenAnyValue(command)
                        .Select(x => new InputCommand(x, shortcut, description))
                        .Subscribe(subj),
                    keyEvent.Subscribe(_ => KeyboardManager.Current.InvokeShortcut(shortcut)));
            });

            return ret.Publish(null).RefCount();
        }

        static string keysToShortcutName(ModifierKeys modifiers, Key key)
        {
            var mods = new string[] {
                modifiers.HasFlag(ModifierKeys.Control) ? "Ctrl" : null,
#if !WINRT
                modifiers.HasFlag(ModifierKeys.Alt) ? "Alt" : null,
#endif
                modifiers.HasFlag(ModifierKeys.Shift) ? "Shift" : null,
                modifiers.HasFlag(ModifierKeys.Windows) ? "Meta" : null,
            }.Where(x => x != null);

            var modString = String.Join("+", mods);
            return (modString.Length > 1) ?
                String.Format("{0}-{1}", modString, key.ToString()) :
                key.ToString();
        }

#if WINRT
        static ModifierKeys getCurrentModifiers()
        {
            var wnd = Window.Current.CoreWindow;

            return 
                (wnd.GetKeyState(Key.Shift) == CoreVirtualKeyStates.Down ? ModifierKeys.Shift : ModifierKeys.None) |
                (wnd.GetKeyState(Key.Control) == CoreVirtualKeyStates.Down ? ModifierKeys.Control : ModifierKeys.None) |
                (wnd.GetKeyState(Key.LeftWindows) == CoreVirtualKeyStates.Down ? ModifierKeys.Windows : ModifierKeys.None) |
                (wnd.GetKeyState(Key.RightWindows) == CoreVirtualKeyStates.Down ? ModifierKeys.Windows : ModifierKeys.None);
        }
#endif
    }
}