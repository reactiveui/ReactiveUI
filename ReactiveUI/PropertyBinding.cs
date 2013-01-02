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

        public static IDisposable Bind<TViewModel, TView, TVMProp, TVProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty)
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
            return binderImplementation.Bind<TViewModel, TView, TProp, TProp, Unit>(viewModel, view, vmProperty, null, null, null);
        }

        public static IDisposable Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
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
            return binderImplementation.Bind<TViewModel, TView, TProp, TDontCare, TDontCare>(viewModel, view, vmProperty, null, signalViewUpdate, null);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                Func<TVMProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, fallbackValue);
        }

        public static IDisposable OneWayBind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind<TViewModel, TView, TProp, Unit>(viewModel, view, vmProperty, null, fallbackValue, null);
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
        IDisposable Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint)
            where TViewModel : class
            where TView : IViewFor;

        IDisposable OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                Func<TVMProp> fallbackValue = null,
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
        public IDisposable Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint)
            where TViewModel : class
            where TView : IViewFor
        {
            var signalInitialUpdate = new Subject<bool>();
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            string[] viewPropChain;

            if (viewProperty == null) {
                // NB: In this case, TVProp is possibly wrong due to type 
                // conversion. Figure out if this is the case, then re-call Bind
                // with the right TVProp
                viewPropChain = Reflection.getDefaultViewPropChain(view, vmPropChain);
                var tvProp = Reflection.GetTypesForPropChain(typeof (TView), viewPropChain).Last();
                if (tvProp != typeof (TVProp)) {
                    var mi = this.GetType().GetMethod("Bind").MakeGenericMethod(typeof (TViewModel), typeof (TView), typeof (TVMProp), tvProp, typeof (TDontCare));
                    return (IDisposable) mi.Invoke(this, new[] {viewModel, view, vmProperty, null, signalViewUpdate, conversionHint});
                }
            } else {
                viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
            }

            var vmToViewConverter = getConverterForTypes(typeof (TVMProp), typeof (TVProp));
            var viewToVMConverter = getConverterForTypes(typeof (TVProp), typeof (TVMProp));

            if (vmToViewConverter == null || viewToVMConverter == null) {
                throw new ArgumentException(
                    String.Format("Can't two-way convert between {0} and {1}. To fix this, register a IBindingTypeConverter", typeof (TVMProp), typeof(TVProp)));
            }

            var somethingChanged = Observable.Merge(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty).Select(_ => true),
                signalInitialUpdate,
                signalViewUpdate != null ? 
                    signalViewUpdate.Select(_ => false) : 
                    view.WhenAnyDynamic(viewPropChain, x => (TVProp) x.Value).Select(_ => false));

            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));
            var vString = String.Format("{0}.{1}", typeof (TView).Name, String.Join(".", viewPropChain));
            var vmChangedString = String.Format("Setting {0} => {1}", vmString, vString);
            var viewChangedString = String.Format("Setting {0} => {1}", vString, vmString);

            var changeWithValues = somethingChanged.Select(isVm => {
                TVMProp vmValue; TVProp vValue;
                if (!Reflection.TryGetValueForPropertyChain(out vmValue, view.ViewModel, vmPropChain) ||
                    !Reflection.TryGetValueForPropertyChain(out vValue, view, viewPropChain)) {
                    return null;
                }

                if (isVm) {
                    object tmp;
                    if (!vmToViewConverter.TryConvert(vmValue, typeof (TVProp), conversionHint, out tmp)) {
                        return null;
                    }

                    var vmAsView = (tmp == null ? default(TVProp) : (TVProp) tmp);
                    var changed = EqualityComparer<TVProp>.Default.Equals(vValue, vmAsView) != true;
                    if (!changed) return null;

                    this.Log().Info(vmChangedString + (vmAsView != null ? vmAsView.ToString() : "(null)"));
                    return Tuple.Create((object)vmAsView, isVm);
                } else {
                    object tmp;
                    if (!viewToVMConverter.TryConvert(vValue, typeof (TVMProp), conversionHint, out tmp)) {
                        return null;
                    }

                    var vAsViewModel = (tmp == null ? default(TVMProp) : (TVMProp) tmp);
                    var changed = EqualityComparer<TVMProp>.Default.Equals(vmValue, vAsViewModel) != true;
                    if (!changed) return null;

                    this.Log().Info(viewChangedString + (vAsViewModel != null ? vAsViewModel.ToString() : "(null)"));
                    return Tuple.Create((object)vAsViewModel, isVm);
                }
            });

            var hooks = RxApp.GetAllServices<IPropertyBindingHook>();
            var shouldBail = hooks.Aggregate(true, (acc, x) => 
                acc && x.ExecuteHook(viewModel, view, vmString, vString, BindingDirection.TwoWay));

            if (shouldBail) {
                this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
                return Disposable.Empty;
            }

            var ret = changeWithValues.Subscribe(isVmWithLatestValue => {
                if (isVmWithLatestValue == null) return;

                if (isVmWithLatestValue.Item2) {
                    Reflection.SetValueToPropertyChain(view, viewPropChain, isVmWithLatestValue.Item1, false);
                } else {
                    Reflection.SetValueToPropertyChain(view.ViewModel, vmPropChain, isVmWithLatestValue.Item1, false);
                }
            });

            // NB: Even though it's technically a two-way bind, most people 
            // want the ViewModel to win at first.
            signalInitialUpdate.OnNext(true);

            return ret;
        }

        public IDisposable OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                Func<TVMProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor
        {
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));

            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                var viewType = Reflection.GetTypesForPropChain(typeof (TView), viewPropChain).Last();
                var converter = getConverterForTypes(typeof (TVMProp), viewType);

                if (converter == null) {
                    throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", typeof (TVMProp), viewType));
                }

                var vString = String.Format("{0}.{1}", viewType.Name, String.Join(".", viewPropChain));

                var hooks = RxApp.GetAllServices<IPropertyBindingHook>();
                var shouldBail = hooks.Aggregate(true, (acc, x) => 
                    acc && x.ExecuteHook(viewModel, view, vmString, vString, BindingDirection.OneWay));

                if (shouldBail) {
                    this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
                    return Disposable.Empty;
                }

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .SelectMany(x => {
                        object tmp;
                        if (!converter.TryConvert(x, viewType, conversionHint, out tmp)) return Observable.Empty<object>();
                        return Observable.Return(tmp);
                    })
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            } else {
                var converter = getConverterForTypes(typeof (TVMProp), typeof (TVProp));

                if (converter == null) {
                    throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", typeof (TVMProp), typeof(TVProp)));
                }

                var viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
                var vString = String.Format("{0}.{1}", typeof(TView), String.Join(".", viewPropChain));

                var hooks = RxApp.GetAllServices<IPropertyBindingHook>();
                var shouldBail = hooks.Aggregate(true, (acc, x) => 
                    acc && x.ExecuteHook(viewModel, view, vmString, vString, BindingDirection.OneWay));

                if (shouldBail) {
                    this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
                    return Disposable.Empty;
                }

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .SelectMany(x => {
                        object tmp;
                        if (!converter.TryConvert(x, typeof(TVProp), conversionHint, out tmp)) return Observable.Empty<TVProp>();
                        return Observable.Return(tmp == null ? default(TVProp) : (TVProp) tmp);
                    })
                    .BindTo(view, viewProperty, () => {
                        object tmp;
                        return converter.TryConvert(fallbackValue(), typeof(TVProp), conversionHint, out tmp) ?  (TVProp)tmp : default(TVProp);
                    });
            }
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
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));

            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));
                var vString = String.Format("{0}.{1}", typeof (TView), String.Join(".", viewPropChain));

                var hooks = RxApp.GetAllServices<IPropertyBindingHook>();
                var shouldBail = hooks.Aggregate(true, (acc, x) =>
                    acc && x.ExecuteHook(viewModel, view, vmString, vString, BindingDirection.OneWay));

                if (shouldBail) {
                    this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
                    return Disposable.Empty;
                }

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                                 .Select(selector)
                                 .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            } else {
                var viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
                var vString = String.Format("{0}.{1}", typeof(TView), String.Join(".", viewPropChain));

                var hooks = RxApp.GetAllServices<IPropertyBindingHook>();
                var shouldBail = hooks.Aggregate(true, (acc, x) =>
                    acc && x.ExecuteHook(viewModel, view, vmString, vString, BindingDirection.OneWay));

                if (shouldBail) {
                    this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
                    return Disposable.Empty;
                }

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .Select(selector)
                    .BindTo(view, viewProperty, fallbackValue);
            }
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
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));

            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view,
                    Reflection.ExpressionToPropertyNames(vmProperty));
                var vString = String.Format("{0}.{1}", typeof (TView), String.Join(".", viewPropChain));

                var hooks = RxApp.GetAllServices<IPropertyBindingHook>();
                var shouldBail = hooks.Aggregate(true, (acc, x) =>
                    acc && x.ExecuteHook(viewModel, view, vmString, vString, BindingDirection.AsyncOneWay));

                if (shouldBail) {
                    this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
                    return Disposable.Empty;
                }

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                                 .SelectMany(selector)
                                 .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            } else {
                var viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
                var vString = String.Format("{0}.{1}", typeof(TView), String.Join(".", viewPropChain));

                var hooks = RxApp.GetAllServices<IPropertyBindingHook>();
                var shouldBail = hooks.Aggregate(true, (acc, x) =>
                    acc && x.ExecuteHook(viewModel, view, vmString, vString, BindingDirection.OneWay));

                if (shouldBail) {
                    this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
                    return Disposable.Empty;
                }

                return Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .SelectMany(selector)
                    .BindTo(view, viewProperty, fallbackValue);
            }
        }

        MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter>(
            (types, _) =>
                RxApp.GetAllServices<IBindingTypeConverter>()
                    .Aggregate(Tuple.Create(-1, default(IBindingTypeConverter)), (acc, x) => {
                        var score = x.GetAffinityForObjects(types.Item1, types.Item2);
                        return score > acc.Item1 && score > 0 ? 
                            Tuple.Create(score, x) : acc;
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