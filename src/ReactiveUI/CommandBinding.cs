using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Various helpers to bind View controls and ViewModel commands together
    /// </summary>
    public static class CommandBinder
    {
        static ICommandBinderImplementation binderImplementation;

        static CommandBinder()
        {
            RxApp.EnsureInitialized();

            binderImplementation = Locator.Current.GetService<ICommandBinderImplementation>() ??
                new CommandBinderImplementation();
        }

        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding</returns>
        /// <param name="view">The View</param>
        /// <param name="viewModel">The View model</param>
        /// <param name="controlName">The name of the control on the view</param>
        /// <param name="propertyName">The ViewModel command to bind.</param>
        /// <param name="withParameter">The ViewModel property to pass as the
        /// param of the ICommand</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.</param>
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                IObservable<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter, toEvent);
        }

        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding</returns>
        /// <param name="view">The View</param>
        /// <param name="viewModel">The View model</param>
        /// <param name="propertyName">The ViewModel command to bind</param>
        /// <param name="controlName">The name of the control on the view</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.</param>
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return binderImplementation.BindCommand(viewModel, view, propertyName, controlName, toEvent);
        }

        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding</returns>
        /// <param name="view">The View</param>
        /// <param name="viewModel">The View model</param>
        /// <param name="propertyName">The ViewModel command to bind</param>
        /// <param name="controlName">The name of the control on the view</param>
        /// <param name="withParameter">The ViewModel property to pass as the
        /// param of the ICommand</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.</param>
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Expression<Func<TViewModel, TParam>> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter, toEvent);
        }
    }

    interface ICommandBinderImplementation : IEnableLogger
    {
        IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Func<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;

        IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                IObservable<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;
    }

    /// <summary>
    /// Used by the CommandBinder extension methods to handle binding View controls and ViewModel commands
    /// </summary>
    public class CommandBinderImplementation : ICommandBinderImplementation
    {
        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding</returns>
        /// <param name="view">The View</param>
        /// <param name="viewModel">The View model</param>
        /// <param name="controlProperty">The name of the control on the view</param>
        /// <param name="vmProperty">The ViewModel command to bind</param>
        /// <param name="withParameter">The ViewModel property to pass as the
        /// param of the ICommand</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.</param>
        public IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TControl>> controlProperty,
                Func<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var controlExpression = Reflection.Rewrite(controlProperty.Body);
            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>();

            IDisposable bindingDisposable = bindCommandInternal(source, view, controlExpression, Observable.Defer(() => Observable.Return(withParameter())), toEvent, cmd => {
                var rc = cmd as ReactiveCommand;
                if (rc == null) {
                    return new RelayCommand(cmd.CanExecute, _ => cmd.Execute(withParameter()));
                }

                var ret = ReactiveCommand.Create(() => ((ICommand)rc).Execute(null), rc.CanExecute);
                return ret;
            });

            return new ReactiveBinding<TView, TViewModel, TProp>(view, viewModel, controlExpression, vmExpression,
                source, BindingDirection.OneWay, bindingDisposable);
        }

        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding</returns>
        /// <param name="view">The View</param>
        /// <param name="viewModel">The View model</param>
        /// <param name="controlProperty">The name of the control on the view</param>
        /// <param name="vmProperty">The ViewModel command to bind</param>
        /// <param name="withParameter">The ViewModel property to pass as the
        /// param of the ICommand</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.</param>
        public IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TControl>> controlProperty,
                IObservable<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var controlExpression = Reflection.Rewrite(controlProperty.Body);
            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>();

            IDisposable bindingDisposable = bindCommandInternal(source, view, controlExpression, withParameter, toEvent);

            return new ReactiveBinding<TView, TViewModel, TProp>(view, viewModel, controlExpression, vmExpression,
                source, BindingDirection.OneWay, bindingDisposable);
        }

        IDisposable bindCommandInternal<TView, TProp, TParam>(
                IObservable<TProp> This,
                TView view,
                Expression controlExpression,
                IObservable<TParam> withParameter,
                string toEvent,
                Func<ICommand, ICommand> commandFixuper = null)
            where TView : class, IViewFor
            where TProp : ICommand
        {
            IDisposable disp = Disposable.Empty;

            var bindInfo = Observable.CombineLatest(
                This, view.WhenAnyDynamic(controlExpression, x => x.Value),
                (val, host) => new { val, host });

            var propSub = bindInfo
                .Where(x => x.host != null)
                .Subscribe(x => {
                    disp.Dispose();
                    if (x == null) {
                        disp = Disposable.Empty;
                        return;
                    }

                    var cmd = commandFixuper != null ? commandFixuper(x.val) : x.val;
                    if (toEvent != null) {
                        disp = CreatesCommandBinding.BindCommandToObject(cmd, x.host, withParameter.Select(y => (object)y), toEvent);
                    } else {
                        disp = CreatesCommandBinding.BindCommandToObject(cmd, x.host, withParameter.Select(y => (object)y));
                    }
                });

            return Disposable.Create(() => {
                propSub.Dispose();
                disp.Dispose();
            });
        }
    }

    static class CommandBinderImplementationMixins
    {
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl>(
                this ICommandBinderImplementation This,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return This.BindCommand(viewModel, view, propertyName, controlName, Observable<object>.Empty, toEvent);
        }

        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this ICommandBinderImplementation This,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Expression<Func<TViewModel, TParam>> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return This.BindCommand(viewModel, view, propertyName, controlName, view.ViewModel.WhenAnyValue(withParameter), toEvent);
        }
    }

    class CreatesCommandBinding
    {
        static readonly MemoizingMRUCache<Type, ICreatesCommandBinding> bindCommandCache =
            new MemoizingMRUCache<Type, ICreatesCommandBinding>((t, _) => {
                return Locator.Current.GetServices<ICreatesCommandBinding>()
                    .Aggregate(Tuple.Create(0, (ICreatesCommandBinding)null), (acc, x) => {
                        int score = x.GetAffinityForObject(t, false);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, RxApp.SmallCacheLimit);

        static readonly MemoizingMRUCache<Type, ICreatesCommandBinding> bindCommandEventCache =
            new MemoizingMRUCache<Type, ICreatesCommandBinding>((t, _) => {
                return Locator.Current.GetServices<ICreatesCommandBinding>()
                    .Aggregate(Tuple.Create(0, (ICreatesCommandBinding)null), (acc, x) => {
                        int score = x.GetAffinityForObject(t, true);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, RxApp.SmallCacheLimit);

        public static IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var binder = default(ICreatesCommandBinding);
            var type = target.GetType();

            lock(bindCommandCache) {
                binder = bindCommandCache.Get(type);
            }

            if (binder == null) {
                throw new Exception(String.Format("Couldn't find a Command Binder for {0}", type.FullName));
            }

            var ret = binder.BindCommandToObject(command, target, commandParameter);
            if (ret == null) {
                throw new Exception(String.Format("Couldn't bind Command Binder for {0}", type.FullName));
            }

            return ret;
        }

        public static IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            var type = target.GetType();
            var binder = bindCommandEventCache.Get(type);
            if (binder == null) {
                throw new Exception(String.Format("Couldn't find a Command Binder for {0} and event {1}", type.FullName, eventName));
            }

            var eventArgsType = Reflection.GetEventArgsTypeForEvent(type, eventName);
            var mi = binder.GetType().GetTypeInfo().DeclaredMethods.First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
            mi = mi.MakeGenericMethod(new[] {eventArgsType});

            var ret = (IDisposable) mi.Invoke(binder, new[] {command, target, commandParameter, eventName});
            if (ret == null) {
                throw new Exception(String.Format("Couldn't bind Command Binder for {0} and event {1}", type.FullName, eventName));
            }

            return ret;
        }
    }

    internal class RelayCommand : ICommand
    {
        readonly Func<object, bool> canExecute;
        readonly Action<object> execute;

        public RelayCommand(Func<object, bool> canExecute = null, Action<object> execute = null)
        {
            this.canExecute = canExecute ?? (_ => true);
            this.execute = execute ?? (_ => {});
        }

        public event EventHandler CanExecuteChanged;

        bool? prevCanExecute = null;
        public bool CanExecute(object parameter)
        {
            var ce = canExecute(parameter);
            if (CanExecuteChanged != null && (!prevCanExecute.HasValue || ce != prevCanExecute)) {
                CanExecuteChanged(this, EventArgs.Empty);
                prevCanExecute = ce;
            }

            return ce;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
