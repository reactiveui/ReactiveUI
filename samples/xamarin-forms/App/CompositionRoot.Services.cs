// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using Akavache;
using Services.Api;
using Services.Connected.Api;
using Services.Connected.State;
using Services.State;
using Splat;

namespace App
{
    public abstract partial class CompositionRoot
    {
        protected readonly Lazy<IBlobCache> _blobCache;
        protected readonly Lazy<IDuckDuckGoApiService> _duckDuckGoApiService;
        protected readonly Lazy<ILogger> _loggingService;
        protected readonly Lazy<IStateService> _stateService;

        public IDuckDuckGoApiService ResolveDuckDuckGoApiService() => _duckDuckGoApiService.Value;

        public ILogger ResolveLoggingService() => _loggingService.Value;

        public IStateService ResolveStateService() => _stateService.Value;

        protected abstract ILogger CreateLoggingService();

        private IBlobCache CreateBlobCache() => BlobCache.LocalMachine;

#if DISCONNECTED || DISCONNECTED_ERRORS || DISCONNECTED_FAST
        private IDuckDuckGoApiService CreateDuckDuckGoApiService() => new DuckDuckGoApiServiceDisconnected(
#if DISCONNECTED_FAST
            enableRandomDelays: false,
#else
            enableRandomDelays: true,
#endif
#if DISCONNECTED_ERRORS
            enableRandomErrors: true
#else
            enableRandomErrors: false
#endif
        );
#else

        private IDuckDuckGoApiService CreateDuckDuckGoApiService() => new DuckDuckGoApiService();

#endif

        private IStateService CreateStateService() => new StateService(ResolveBlobCache());

        private IBlobCache ResolveBlobCache() => _blobCache.Value;
    }
}
