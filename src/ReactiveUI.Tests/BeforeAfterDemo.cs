// Demonstration of the problem and solution for ReactiveUI scheduler consumption

namespace ReactiveUI.Demo
{
    // BEFORE: Using RxApp schedulers requires RequiresUnreferencedCode attributes
    public class BeforeViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _status;

#if NET6_0_OR_GREATER
        [RequiresDynamicCode("Uses RxApp schedulers which require dynamic code generation")]
        [RequiresUnreferencedCode("Uses RxApp schedulers which may require unreferenced code")]
#endif
        public BeforeViewModel()
        {
            // Using RxApp.MainThreadScheduler triggers RequiresUnreferencedCode requirement
            _status = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => loading ? "Loading..." : "Ready")
                .ObserveOn(RxApp.MainThreadScheduler)  // ❌ Requires attributes
                .ToProperty(this, nameof(Status), scheduler: RxApp.MainThreadScheduler);
        }

        public bool IsLoading { get; set; }
        public string Status => _status.Value;
    }

    // Repository that forces consumers to use RequiresUnreferencedCode
    public class BeforeRepository
    {
#if NET6_0_OR_GREATER
        [RequiresDynamicCode("Uses RxApp schedulers which require dynamic code generation")]
        [RequiresUnreferencedCode("Uses RxApp schedulers which may require unreferenced code")]
#endif
        public IObservable<string> GetDataAsync()
        {
            return Observable.Return("data")
                .ObserveOn(RxApp.TaskpoolScheduler)  // ❌ Requires attributes
                .Select(ProcessData);
        }

        private static string ProcessData(string data) => $"Processed: {data}";
    }

    // AFTER: Using RxSchedulers requires NO attributes
    public class AfterViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _status;

        // ✅ No RequiresUnreferencedCode attributes needed!
        public AfterViewModel()
        {
            // Using RxSchedulers instead of RxApp schedulers avoids attribute requirements
            _status = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => loading ? "Loading..." : "Ready")
                .ObserveOn(RxSchedulers.MainThreadScheduler)  // ✅ No attributes needed
                .ToProperty(this, nameof(Status), scheduler: RxSchedulers.MainThreadScheduler);
        }

        public bool IsLoading { get; set; }
        public string Status => _status.Value;
    }

    // Repository that doesn't force consumers to use RequiresUnreferencedCode
    public class AfterRepository
    {
        // ✅ No RequiresUnreferencedCode attributes needed!
        public IObservable<string> GetDataAsync()
        {
            return Observable.Return("data")
                .ObserveOn(RxSchedulers.TaskpoolScheduler)  // ✅ No attributes needed
                .Select(ProcessData);
        }

        private static string ProcessData(string data) => $"Processed: {data}";
    }

    // ReactiveProperty usage comparison
    public class ReactivePropertyDemo
    {
        // BEFORE: Constructor approach requires attributes
#if NET6_0_OR_GREATER
        [RequiresDynamicCode("ReactiveProperty initialization uses RxApp which requires dynamic code generation")]
        [RequiresUnreferencedCode("ReactiveProperty initialization uses RxApp which may require unreferenced code")]
#endif
        public ReactiveProperty<string> CreatePropertyOldWay()
        {
            return new ReactiveProperty<string>("initial");  // ❌ Requires attributes
        }

        // AFTER: Factory method approach requires no attributes
        public ReactiveProperty<string> CreatePropertyNewWay()
        {
            return ReactiveProperty<string>.Create("initial");  // ✅ No attributes needed
        }
    }

    // Consumer classes demonstrate the impact
    public class ConsumerService
    {
        // This would require RequiresUnreferencedCode if using BeforeRepository
        // But works fine with AfterRepository - no attributes needed!
        public void UseRepository()
        {
            var repo = new AfterRepository();
            repo.GetDataAsync().Subscribe(data => Console.WriteLine(data));
        }

        // Similarly, no attributes needed when using AfterViewModel
        public void UseViewModel()
        {
            var vm = new AfterViewModel();
            vm.IsLoading = true;
            Console.WriteLine(vm.Status);
        }
    }
}