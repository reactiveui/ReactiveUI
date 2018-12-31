// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    /// <inheritdoc />
    /// <summary>
    /// The Bespoke platform.
    /// </summary>
    /// <seealso cref="BasePlatform" />
    public class Bespoke : BasePlatform
    {
        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.None;

        /// <inheritdoc />
        public override Task Extract()
        {
            return Task.CompletedTask;
        }
    }
}
