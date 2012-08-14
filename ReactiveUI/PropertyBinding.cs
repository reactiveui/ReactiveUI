using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;

namespace ReactiveUI
{
    interface IPropertyBinderImplementation
    {
        IDisposable Bind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>;

        IDisposable OneWayBind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>;

        IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>;

        IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>;
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
                foreach(var propName in propNames.SkipLast(1)) {
                    if (current == null) {
                        return;
                    }

                    pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propName);
                    current = pi.GetValue(current, null);
                }
                if (current == null) {
                    return;
                }

                pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propNames.Last());
                sourceSub.Disposable = This.Subscribe(x => pi.SetValue(current, x, null));
            });

            var toDispose = new IDisposable[] {sourceSub, null};
            var propertyNames = RxApp.expressionToPropertyNames(property);
            toDispose[1] = target.WhenAny(property, _ => Unit.Default).Subscribe(_ => subscribify(target, propertyNames));

            return Disposable.Create(() => { toDispose[0].Dispose(); toDispose[1].Dispose(); });
        }
    }
}