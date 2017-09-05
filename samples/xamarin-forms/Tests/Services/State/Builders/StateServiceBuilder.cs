// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using Akavache;
using Services.Connected.State;
using NSubstitute;

namespace UnitTests.Services.State.Builders
{

    internal sealed class StateServiceBuilder : IBuilder
    {
        private IBlobCache _blobCache;

        public StateServiceBuilder()
        {
            _blobCache = Substitute.For<IBlobCache>();
        }

        public StateServiceBuilder WithBlobCache(IBlobCache blobCache) =>
            this.With(ref _blobCache, blobCache);

        public StateService Build() =>
            new StateService(_blobCache);

        public static implicit operator StateService(StateServiceBuilder builder) => builder.Build();
    }
}
