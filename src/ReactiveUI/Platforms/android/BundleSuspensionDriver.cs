// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text.Json;

namespace ReactiveUI;

/// <summary>
/// Loads and saves state to persistent storage.
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public class BundleSuspensionDriver : ISuspensionDriver
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public IObservable<object?> LoadState() // TODO: Create Test
    {
        try
        {
            // NB: Sometimes OnCreate gives us a null bundle
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return Observable.Throw<object>(new Exception("New bundle, start from scratch"));
            }

            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray("__state");

            if (buffer is null)
            {
                return Observable.Throw<object>(new InvalidOperationException("The buffer __state could not be found."));
            }

            var st = new MemoryStream(buffer);

            return Observable.Return(JsonSerializer.Deserialize<object>(st));
        }
        catch (Exception ex)
        {
            return Observable.Throw<object>(ex);
        }
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif

    public IObservable<Unit> SaveState(object state) // TODO: Create Test
    {
        try
        {
            var st = new MemoryStream();
            JsonSerializer.Serialize(st, state);
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
