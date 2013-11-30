using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReactiveUI.Xaml
{
    public static class InputMixins
    {
        public static IObservable<IInputCommand> BindInputCommand<TViewModel>(this IViewFor<TViewModel> This, Expression<Func<IViewFor<TViewModel>, IReactiveCommand>> command, ModifierKeys modifiers, Key key, string description = null)
            where TViewModel : class
        {
            var keyEvent = Observable.Never<Unit>();
            var element = command as UIElement;

            if (element == null) {
                This.Log().Warn("Attempted to bind command '{0}' to a non-UIControl, command will never invoke!", description ?? "(none)");
            } else {
                keyEvent = Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => element.KeyUp += x, x => element.KeyUp -= x)
                    .Where(x => x.EventArgs.Key == key && x.EventArgs.KeyboardDevice.Modifiers == modifiers)
                    .Select(_ => Unit.Default);
            }

            var ret = Observable.Create<IInputCommand>(subj => {
                var shortcut = keysToShortcutName(modifiers, key);

                return new CompositeDisposable(
                    This.WhenAnyValue(command)
                        .Select(x => new InputCommand(x, shortcut, description))
                        .Subscribe(subj),
                    keyEvent.Subscribe(_ => InputScope.Current.InvokeShortcut(shortcut)));
            });

            return ret.Publish(null).RefCount();
        }

        static string keysToShortcutName(ModifierKeys modifiers, Key key)
        {
            var mods = new string[] {
                modifiers.HasFlag(ModifierKeys.Control) ? "Ctrl" : null,
                modifiers.HasFlag(ModifierKeys.Alt) ? "Alt" : null,
                modifiers.HasFlag(ModifierKeys.Shift) ? "Shift" : null,
                modifiers.HasFlag(ModifierKeys.Windows) ? "Meta" : null,
            }.Where(x => x != null);

            var modString = String.Join("+", mods);
            return (modString.Length > 1) ?
                String.Format("{0}-{1}", modString, key.ToString()) :
                key.ToString();
        }
    }
}