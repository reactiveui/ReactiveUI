// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Services.Api;

namespace Services.Disconnected.Api
{
    public class DuckDuckGoApiDisconnected : IDuckDuckGoApi
    {
        private readonly bool _enableRandomDelays;
        private readonly bool _enableRandomErrors;
        private readonly DuckDuckGoSearchResult _searchResult;

        public DuckDuckGoApiDisconnected(bool enableRandomDelays, bool enableRandomErrors)
        {
            _enableRandomDelays = enableRandomDelays;
            _enableRandomErrors = enableRandomErrors;
            _searchResult = JsonConvert.DeserializeObject<DuckDuckGoSearchResult>(DuckDuckGoApiDisconnectedResponses.Search);
        }

        public IObservable<DuckDuckGoSearchResult> Search(string query)
        {
            return Observable.Return(_searchResult)
                .ErrorWithProbabilityIf(_enableRandomErrors, 5)
                .DelayIf(_enableRandomDelays, 500, 1000);
        }
    }
}
