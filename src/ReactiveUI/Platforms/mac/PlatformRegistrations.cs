using System;

namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));
            RxApp.TaskpoolScheduler = System.Reactive.Concurrency.TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler());
        }
    }
}
