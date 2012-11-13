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
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, (IObservable<Unit>)null, null);
        }

        public static IDisposable Bind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, null, (IObservable<Unit>)null, null);
        }

        public static IDisposable Bind<TViewModel, TView, TProp, TDontCare>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, signalViewUpdate, null);
        }

        public static IDisposable Bind<TViewModel, TView, TProp, TDontCare>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                IObservable<TDontCare> signalViewUpdate)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, null, signalViewUpdate, null);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, fallbackValue);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
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
            where TView : IViewFor
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
            where TView : IViewFor
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
            where TView : IViewFor
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
            where TView : IViewFor
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, null, selector, fallbackValue);
        }
    }

    public interface IPropertyBinderImplementation : IEnableLogger
    {
        IDisposable Bind<TViewModel, TView, TProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint)
            where TViewModel : class
            where TView : IViewFor;

        IDisposable OneWayBind<TViewModel, TView, TProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                Func<TProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor;

        IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor;

        IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor;
    }

    class PropertyBinderImplementation : IPropertyBinderImplementation 
    {
        public IDisposable Bind<TViewModel, TView, TProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint)
            where TViewModel : class
            where TView : IViewFor
        {
            var ret = new CompositeDisposable();

            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            string[] viewPropChain;

            if (viewProperty == null) {
                viewPropChain = Reflection.getDefaultViewPropChain(view, vmPropChain);
            } else {
                viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
            }

            // NB: If an explicit signalViewUpdate is specified, we're not going
            // to set up a WhenAny on the View Property.
            var somethingChanged = Observable.Merge(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty),
                signalViewUpdate != null ? Observable.Never<TProp>() : view.WhenAnyDynamic(viewPropChain, x => (TProp)x.Value),
                signalViewUpdate == null ? Observable.Never<TProp>() : signalViewUpdate.SelectMany(_ => {
                    TProp val = default(TProp);
                    return Reflection.TryGetValueForPropertyChain(out val, view, viewPropChain) ? 
                        Observable.Return(val) :
                        Observable.Empty<TProp>();
                })
            ).Multicast(new Subject<TProp>());

            string vmChangedString = String.Format("Setting {0}.{1} => {2}.{3}: ",
                typeof (TViewModel).Name, String.Join(".", vmPropChain),
                typeof (TView).Name, String.Join(".", viewPropChain));

            ret.Add(somethingChanged.Where(x => {
                TProp result;
                if (!Reflection.TryGetValueForPropertyChain(out result, view.ViewModel, vmPropChain))
                    return false;
                var vmChanged = EqualityComparer<TProp>.Default.Equals(result, x) != true;
                if (vmChanged)  this.Log().Info(vmChangedString + (x != null ? x.ToString() : "(null)"));
                return vmChanged;
            }).Subscribe(x => Reflection.SetValueToPropertyChain(view.ViewModel, vmPropChain, x, false)));

            string viewChangedString = String.Format("Setting {0}.{1} => {2}.{3}: ",
                typeof (TView).Name, String.Join(".", viewPropChain),
                typeof (TViewModel).Name, String.Join(".", vmPropChain));

            ret.Add(somethingChanged.Where(x => {
                TProp result;
                if (!Reflection.TryGetValueForPropertyChain(out result, view, viewPropChain))
                    return false;
                var viewChanged = EqualityComparer<TProp>.Default.Equals(result, x) != true;
                if (viewChanged)  this.Log().Info(viewChangedString + (x != null ? x.ToString() : "(null)"));
                return viewChanged;
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
                Func<TProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor
        {
            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

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
            where TView : IViewFor
        {
            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

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
            where TView : IViewFor
        {
            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .SelectMany(selector)
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            }

            return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                .SelectMany(selector)
                .BindTo(view, viewProperty, fallbackValue);
        }

        MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter>(
            (types, _) =>
                RxApp.GetAllServices<IBindingTypeConverter>()
                    .Aggregate(Tuple.Create(-1, default(IBindingTypeConverter)), (acc, x) => {
                        var score = x.GetAffinityForObjects(types.Item1, types.Item2);
                        return score > acc.Item1 ? Tuple.Create(score, x) : acc;
                    }).Item2
            , 25);

        IBindingTypeConverter getConverterForTypes(Type lhs, Type rhs)
        {
            return typeConverterCache.Get(Tuple.Create(lhs, rhs));
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