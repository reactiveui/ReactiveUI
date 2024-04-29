// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ReactiveUI;

/// <summary>
/// Loads and saves state to persistent storage.
/// </summary>
public class BundleSuspensionDriver : ISuspensionDriver
{
    /// <inheritdoc/>
    public IObservable<object> LoadState() // TODO: Create Test
    {
        try
        {
            // NB: Sometimes OnCreate gives us a null bundle
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return Observable.Throw<object>(new Exception("New bundle, start from scratch"));
            }

            var serializer = new BinaryFormatter();
            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray("__state");

            if (buffer is null)
            {
                return Observable.Throw<object>(new InvalidOperationException("The buffer __state could not be found."));
            }

            var st = new MemoryStream(buffer);

            return Observable.Return(serializer.Deserialize(st));
        }
        catch (Exception ex)
        {
            return Observable.Throw<object>(ex);
        }
    }

    /// <inheritdoc/>
    public IObservable<Unit> SaveState(object state) // TODO: Create Test
    {
        try
        {
            var serializer = new BinaryFormatter();
            var st = new MemoryStream();

            AutoSuspendHelper.LatestBundle?.PutByteArray("__state", st.ToArray());
            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <inheritdoc/>
    public IObservable<Unit> InvalidateState() // TODO: Create Test
    {
        try
        {
            AutoSuspendHelper.LatestBundle?.PutByteArray("__state", []);
            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }
}
