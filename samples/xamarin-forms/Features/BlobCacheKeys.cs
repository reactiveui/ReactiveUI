// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class BlobCacheKeys
    {
        public static string GetKeyForSearch(string query) => string.Format("searchQuery-{0}", query);
    }
}
