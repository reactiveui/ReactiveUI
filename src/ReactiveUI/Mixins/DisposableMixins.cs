// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reactive.Disposables
{
    public static class DisposableMixins
    {
        /// <summary>
        /// Ensures the provided disposable is disposed with the specified <see cref="CompositeDisposable"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the disposable.
        /// </typeparam>
        /// <param name="this">
        /// The disposable.
        /// </param>
        /// <param name="compositeDisposable">
        /// The <see cref="CompositeDisposable"/> to which <paramref name="this"/> will be added.
        /// </param>
        /// <returns>
        /// The disposable.
        /// </returns>
        public static T DisposeWith<T>(this T @this, CompositeDisposable compositeDisposable)
            where T : IDisposable
        {
            compositeDisposable.Add(@this);
            return @this;
        }
    }
}