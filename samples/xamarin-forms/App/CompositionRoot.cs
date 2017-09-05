using System;
using System.Reactive.Concurrency;

using Akavache;
using Services.Api;
using Services.State;

using Splat;

namespace App
{
    public abstract partial class CompositionRoot
    {
        protected CompositionRoot()
        {
            _mainScheduler = new Lazy<IScheduler>(CreateMainScheduler);
            _taskPoolScheduler = new Lazy<IScheduler>(CreateTaskPoolScheduler);

            _loggingService = new Lazy<ILogger>(CreateLoggingService);
            _blobCache = new Lazy<IBlobCache>(CreateBlobCache);
            _stateService = new Lazy<IStateService>(CreateStateService);
            _duckDuckGoApiService = new Lazy<IDuckDuckGoApiService>(CreateDuckDuckGoApiService);
        }
    }
}
