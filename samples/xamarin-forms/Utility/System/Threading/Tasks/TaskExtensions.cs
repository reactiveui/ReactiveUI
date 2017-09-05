// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Genesis.Ensure;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable ContinueOnAnyContext(this Task @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.ConfigureAwait(continueOnCapturedContext: false);
        }

        public static ConfiguredTaskAwaitable<T> ContinueOnAnyContext<T>(this Task<T> @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
