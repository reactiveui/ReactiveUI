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
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty);
        }

        public static IDisposable Bind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, null);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, fallbackValue);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, null, fallbackValue);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, selector, fallbackValue);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, null, selector, fallbackValue);
        }

        public static IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, viewProperty, selector, fallbackValue);
        }

        public static IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, null, selector, fallbackValue);
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
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>;

        IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>;
    }

    class PropertyBinderImplementation : IPropertyBinderImplementation 
    {
        public IDisposable Bind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            var ret = new CompositeDisposable();

            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            string[] viewPropChain;

            if (viewProperty == null) {
                viewPropChain = getDefaultViewPropChain(view, vmPropChain);
            } else {
                viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
            }

            var somethingChanged = Observable.Merge(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty),
                view.WhenAnyDynamic(viewPropChain, x => (TProp)x.Value)
            ).Multicast(new Subject<TProp>());

            ret.Add(somethingChanged.Where(x => {
                TProp result;
                if (!Reflection.TryGetValueForPropertyChain(out result, view.ViewModel, vmPropChain))
                    return false;
                return EqualityComparer<TProp>.Default.Equals(result, x) != true;
            }).Subscribe(x => Reflection.SetValueToPropertyChain(view.ViewModel, vmPropChain, x, false)));

            ret.Add(somethingChanged.Where(x => {
                TProp result;
                if (!Reflection.TryGetValueForPropertyChain(out result, view, viewPropChain))
                    return false;
                return EqualityComparer<TProp>.Default.Equals(result, x) != true;
            }).Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false)));

            // NB: Even though it's technically a two-way bind, most people 
            // want the ViewModel to win at first.
            TProp initialVal;
            bool shouldSet = Reflection.TryGetValueForPropertyChain(out initialVal, view.ViewModel, vmPropChain);

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
            where TView : IViewForViewModel<TViewModel>
        {
            if (viewProperty == null) {
                var viewPropChain = getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            }

            return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                .BindTo(view, viewProperty, fallbackValue);
        }

        public IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            if (viewProperty == null) {
                var viewPropChain = getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .Select(selector)
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            }

            return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                .Select(selector)
                .BindTo(view, viewProperty, fallbackValue);
        }

        public IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewForViewModel<TViewModel>
        {
            if (viewProperty == null) {
                var viewPropChain = getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .SelectMany(selector)
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            }

            return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                .SelectMany(selector)
                .BindTo(view, viewProperty, fallbackValue);
        }

        static string[] getDefaultViewPropChain(object view, string[] vmPropChain)
        {
            var vmPropertyName = vmPropChain.First();
            var control = Reflection.GetValueFetcherForProperty(view.GetType(), vmPropertyName)(view);

            if (control == null) {
                throw new Exception(String.Format("Tried to bind to control but it was null: {0}.{1}", view.GetType().FullName,
                    vmPropertyName));
            }

            var defaultProperty = DefaultPropertyBinding.GetPropertyForControl(control);
            if (defaultProperty == null) {
                throw new Exception(String.Format("Couldn't find a default property for type {0}", control.GetType()));
            }
            return new[] {vmPropertyName, defaultProperty};
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
        public static IDisposable BindTo<TTarget, TValue>(
            this IObservable<TValue> This, 
            TTarget target,
            Expression<Func<TTarget, TValue>> property,
            Func<TValue> fallbackValue = null)
        {
            var pn = Reflection.ExpressionToPropertyNames(property);

            var lastValue = default(TValue);
            return Observable.Merge(target.WhenAny(property, _ => lastValue).Skip(1), This)
                .Subscribe(x => {
                    lastValue = x;
                    Reflection.SetValueToPropertyChain(target, pn, x);
                });
        }
    }
}