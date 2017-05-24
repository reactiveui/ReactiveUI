using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new UIKitObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new UIKitCommandBinders(), typeof(ICreatesCommandBinding));
            registerFunction(() => new DateTimeNSDateConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler());
            registerFunction(() => new AppSupportJsonSuspensionDriver(), typeof(ISuspensionDriver));
        }
    }
}
