// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI
{
    /// <summary>
    /// This base class is mostly used by the Framework. Implement <see cref="IViewFor{T}"/>
    /// instead.
    /// </summary>
    public interface IViewFor : IActivatable
    {
        /// <summary>
        ///
        /// </summary>
        object ViewModel { get; set; }
    }

#pragma warning disable SA1402
    /// File may only contain a single type
    /// <summary>
    /// Implement this interface on your Views to support Routing and Binding.
    /// </summary>
    public interface IViewFor<T> : IViewFor
#pragma warning restore SA1402 // File may only contain a single type
        where T : class
    {
        /// <summary>
        /// The ViewModel corresponding to this specific View. This should be
        /// a DependencyProperty if you're using XAML.
        /// </summary>

        new T ViewModel { get; set; }
    }
}
