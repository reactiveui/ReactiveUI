// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ReactiveUI.Fody.Helpers
{
    /// <summary>
    /// Attribute that marks property for INotifyPropergyChanged weaving.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class ReactiveAttribute : Attribute
    {
    }
}
