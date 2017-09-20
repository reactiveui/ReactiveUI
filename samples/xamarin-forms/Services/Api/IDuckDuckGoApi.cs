// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Services.Api
{
    public interface IDuckDuckGoApi
    {
        [Get("/?q={query}&format=json")]
        IObservable<DuckDuckGoSearchResult> Search(string query);
    }
}
