using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace ReactiveUI.Xaml
{
    public static class CommandBinder
    {
        static ICommandBinderImplementation binderImplementation;

        static CommandBinder()
        {
            binderImplementation = new CommandBinderImplementation();
        }

        public static IDisposable BindCommand<TView, TViewModel, TProp>(
                this TView view, 
                TViewModel viewModel, 
                Expression<Func<TViewModel, TProp>> propertyName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return binderImplementation.BindCommand(viewModel, view, propertyName, toEvent);
        }

        public static IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this TView view, 
                TViewModel viewModel, 
                Expression<Func<TViewModel, TProp>> propertyName, 
                Expression<Func<TView, TControl>> controlName,
                Func<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter, toEvent);
        }

        public static IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
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

        public static IDisposable BindCommand<TView, TViewModel, TProp, TControl>(
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

        public static IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
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
        IDisposable BindCommand<TView, TViewModel, TProp>(
                TViewModel viewModel, 
                TView view, 
                Expression<Func<TViewModel, TProp>> propertyName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;

        IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel, 
                TView view, 
                Expression<Func<TViewModel, TProp>> propertyName, 
                Expression<Func<TView, TControl>> controlName,
                Func<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;

        IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
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

    public class CommandBinderImplementation : ICommandBinderImplementation 
    {
        public IDisposable BindCommand<TView, TViewModel, TProp>(
                TViewModel viewModel, 
                TView view, 
                Expression<Func<TViewModel, TProp>> propertyName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            var ctlName = Reflection.SimpleExpressionToPropertyName(propertyName);
            var viewPropGetter = Reflection.GetValueFetcherForProperty(typeof (TView), ctlName);

            return bindCommandInternal(viewModel, view, propertyName, viewPropGetter, Observable.Empty<object>(), toEvent);
        }

        public IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel, 
                TView view, 
                Expression<Func<TViewModel, TProp>> propertyName, 
                Expression<Func<TView, TControl>> controlName,
                Func<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            var ctlName = Reflection.SimpleExpressionToPropertyName(controlName);
            var viewPropGetter = Reflection.GetValueFetcherForProperty(typeof (TView), ctlName);

            return bindCommandInternal(viewModel, view, propertyName, viewPropGetter, Observable.Empty<object>(), toEvent, cmd => {
                var rc = cmd as IReactiveCommand;
                if (rc == null) {
                    return ReactiveCommand.Create(x => cmd.CanExecute(x), _ => cmd.Execute(withParameter()));
                } 

                var ret = new ReactiveCommand(rc.CanExecuteObservable);
                ret.Subscribe(_ => rc.Execute(withParameter()));
                return ret;
            });
        }

        public IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel, 
                TView view, 
                Expression<Func<TViewModel, TProp>> propertyName, 
                Expression<Func<TView, TControl>> controlName,
                IObservable<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            var ctlName = Reflection.SimpleExpressionToPropertyName(controlName);
            var viewPropGetter = Reflection.GetValueFetcherForProperty(typeof (TView), ctlName);
            return bindCommandInternal(viewModel, view, propertyName, viewPropGetter, withParameter, toEvent);
        }

        IDisposable bindCommandInternal<TView, TViewModel, TProp, TParam>(
                TViewModel viewModel, 
                TView view, 
                Expression<Func<TViewModel, TProp>> propertyName, 
                Func<object, object> viewPropGetter,
                IObservable<TParam> withParameter,
                string toEvent,
                Func<ICommand, ICommand> commandFixuper = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            var propName = Reflection.SimpleExpressionToPropertyName(propertyName);

            IDisposable disp = Disposable.Empty;

            var propSub = Reflection.ViewModelWhenAnyValue(viewModel, view, propertyName).Subscribe(x => {
                disp.Dispose();
                if (x == null) {
                    disp = Disposable.Empty;
                    return;
                }

                var target = viewPropGetter(view);
                if (target == null) {
                    this.Log().Error("Binding {0}.{1} => {2}.{1} failed because target is null",
                        typeof(TViewModel).FullName, propName, view.GetType().FullName);
                    disp = Disposable.Empty;
                }

                var cmd = commandFixuper != null ? commandFixuper(x) : x;
                if (toEvent != null) {
                    disp = CreatesCommandBinding.BindCommandToObject(cmd, target, withParameter.Select(y => (object)y), toEvent);
                } else {
                    disp = CreatesCommandBinding.BindCommandToObject(cmd, target, withParameter.Select(y => (object)y));
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
        public static IDisposable BindCommand<TView, TViewModel, TProp, TControl>(
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
            return This.BindCommand(viewModel, view, propertyName, controlName, Observable.Empty<object>(), toEvent);
        }

        public static IDisposable BindCommand<TView, TViewModel, TProp, TControl, TParam>(
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
            return This.BindCommand(viewModel, view, propertyName, controlName, view.ViewModel.WhenAny(withParameter, x => x.Value), toEvent);
        }
    }

    class CreatesCommandBinding
    {
        static readonly MemoizingMRUCache<Type, ICreatesCommandBinding> bindCommandCache = 
            new MemoizingMRUCache<Type, ICreatesCommandBinding>((t, _) => {
                return RxApp.GetAllServices<ICreatesCommandBinding>()
                    .Aggregate(Tuple.Create(0, (ICreatesCommandBinding)null), (acc, x) => {
                        int score = x.GetAffinityForObject(t, false);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, 50);

        static readonly MemoizingMRUCache<Type, ICreatesCommandBinding> bindCommandEventCache = 
            new MemoizingMRUCache<Type, ICreatesCommandBinding>((t, _) => {
                return RxApp.GetAllServices<ICreatesCommandBinding>()
                    .Aggregate(Tuple.Create(0, (ICreatesCommandBinding)null), (acc, x) => {
                        int score = x.GetAffinityForObject(t, true);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, 50);

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
            var mi = binder.GetType().GetMethods().First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
            mi = mi.MakeGenericMethod(new[] {eventArgsType});

            //var ret = binder.BindCommandToObject<TEventArgs>(command, target, commandParameter, eventName);
            var ret = (IDisposable) mi.Invoke(binder, new[] {command, target, commandParameter, eventName});
            if (ret == null) {
                throw new Exception(String.Format("Couldn't bind Command Binder for {0} and event {1}", type.FullName, eventName));
            }

            return ret;
        }
    }
}