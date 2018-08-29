// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;

#if PORTABLE || NETFX_CORE
namespace ReactiveUI
{
    /// <summary>
    /// A attribute to indicate if the target is localizable or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class LocalizableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the LocalizableAttribute class.
        /// </summary>
        /// <param name="isLocalizable">If the target is localizable or not.</param>
        public LocalizableAttribute(bool isLocalizable)
        {
        }
    }
}
#endif

// vim: tw=120 ts=4 sw=4 et :
