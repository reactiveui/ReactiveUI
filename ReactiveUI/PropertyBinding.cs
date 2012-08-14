using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;

namespace ReactiveUI
{
    public static class BindingMixins
    {
        static IPropertyBinderImplementation binderImplementation;

        static BindingMixins()
        {
            binderImplementation = new PropertyBinderImplementation();
        }

        public static IDisposable Bind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, fallbackValue);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, selector, fallbackValue);
        }

        public static IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, viewProperty, selector, fallbackValue);
        }
    }

    public interface IPropertyBinderImplementation
    {
        IDisposable Bind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>;

        IDisposable OneWayBind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>;

        IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>;

        IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>;
    }

    class PropertyBinderImplementation : IPropertyBinderImplementation 
    {
        public IDisposable Bind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            var ret = new CompositeDisposable();
            var somethingChanged = Observable.Merge(
                viewModel.WhenAny(vmProperty, x => x.Value),
                view.WhenAny(viewProperty, x => x.Value)
            ).Multicast(new Subject<TProp>());

            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            ret.Add(somethingChanged.Where(x => {
                TProp result;
                if (!Reflection.TryGetValueForPropertyChain(out result, viewModel, vmPropChain))
                    return false;
                return EqualityComparer<TProp>.Default.Equals(result, x) != true;
            }).Subscribe(x => Reflection.SetValueToPropertyChain(viewModel, vmPropChain, x, false)));

            var viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
            ret.Add(somethingChanged.Where(x => {
                TProp result;
                if (!Reflection.TryGetValueForPropertyChain(out result, view, viewPropChain))
                    return false;
                return EqualityComparer<TProp>.Default.Equals(result, x) != true;
            }).Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false)));

            // NB: Even though it's technically a two-way bind, most people 
            // want the ViewModel to win at first.
            TProp initialVal;
            bool shouldSet = Reflection.TryGetValueForPropertyChain(out initialVal, viewModel, vmPropChain);

            ret.Add(somethingChanged.Connect());

            if (shouldSet) Reflection.SetValueToPropertyChain(view, viewPropChain, initialVal);
            return ret;
        }

        public IDisposable OneWayBind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            return viewModel
                .WhenAny(vmProperty, x => x.Value)
                .OneWayBind(view, viewProperty, fallbackValue);
        }

        public IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            return viewModel
                .WhenAny(vmProperty, x => x.Value)
                .Select(selector)
                .OneWayBind(view, viewProperty, fallbackValue);
        }

        public IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : class, IViewForViewModel<TViewModel>
        {
            return viewModel
                .WhenAny(vmProperty, x => x.Value)
                .SelectMany(selector)
                .OneWayBind(view, viewProperty, fallbackValue);
        }
    }

    public static class ObservableBindingMixins
    {
        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to "Subscribe(x =&gt;
        /// target.property = x)", but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">An expression representing the target
        /// property to set. This can be a child property (i.e. x.Foo.Bar.Baz).</param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        public static IDisposable OneWayBind<TTarget, TValue>(
            this IObservable<TValue> This, 
            TTarget target,
            Expression<Func<TTarget, TValue>> property,
            Func<TValue> fallbackValue = null)
            where TTarget : class
        {
            var sourceSub = new MultipleAssignmentDisposable();
            if (fallbackValue != null) {
                This = This.StartWith(fallbackValue())
                    .Select(x => (EqualityComparer<TValue>.Default.Equals(x, default(TValue)) ? fallbackValue() : x));
            }

            var subscribify = new Action<TTarget, string[]>((tgt, propNames) => {
                if (sourceSub.Disposable != null) {
                    sourceSub.Disposable.Dispose();
                }

                object current = tgt;
                PropertyInfo pi = null;
                FieldInfo fi = null;
                Type type;
                foreach(var propName in propNames.SkipLast(1)) {
                    if (current == null) {
                        return;
                    }

                    type = current.GetType();
                    fi = Reflection.GetFieldInfoForField(type, propName);
                    if (fi != null) {
                        current = fi.GetValue(current);
                        continue;
                    }

                    pi = Reflection.GetPropertyInfoOrThrow(current.GetType(), propName);
                    current = pi.GetValue(current, null);
                }
                if (current == null) {
                    return;
                }

                type = current.GetType();
                fi = Reflection.GetFieldInfoForField(type, propNames.Last());
                if (fi != null) {
                    sourceSub.Disposable = This.Subscribe(x => fi.SetValue(current, x));
                    return;
                }

                pi = Reflection.GetPropertyInfoOrThrow(type, propNames.Last());
                sourceSub.Disposable = This.Subscribe(x => pi.SetValue(current, x, null));
            });

            var toDispose = new IDisposable[] {sourceSub, null};
            var propertyNames = Reflection.ExpressionToPropertyNames(property);
            toDispose[1] = target.WhenAny(property, _ => Unit.Default)
                .Subscribe(_ => subscribify(target, propertyNames));

            return Disposable.Create(() => { toDispose[0].Dispose(); toDispose[1].Dispose(); });
        }
    }
}