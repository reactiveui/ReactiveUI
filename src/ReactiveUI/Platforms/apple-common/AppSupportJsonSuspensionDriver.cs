// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text.Json;
using Foundation;

namespace ReactiveUI;

/// <summary>
/// Loads and saves state to persistent storage.
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
[Preserve]
#endif
public class AppSupportJsonSuspensionDriver : ISuspensionDriver
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
    [Preserve]
#endif
    public IObservable<object?> LoadState()
    {
        try
        {
            var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.dat");

            var result = default(object);
            using (var st = File.OpenRead(target))
            {
                result = JsonSerializer.Deserialize<object>(st);
            }

            return Observable.Return(result);
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
    [Preserve]
#endif
    public IObservable<Unit> SaveState(object state)
    {
        try
        {
            var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.dat");

            using (var st = File.Open(target, FileMode.Create))
            {
                JsonSerializer.Serialize(st, state);
            }

            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <inheritdoc/>
    public IObservable<Unit> InvalidateState()
    {
        try
        {
            var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.dat");
            File.Delete(target);

            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    private static string CreateAppDirectory(NSSearchPathDirectory targetDir, string subDir = "Data")
    {
        var fm = new NSFileManager();
        var url = fm.GetUrl(targetDir, NSSearchPathDomain.All, null, true, out _);
        var ret = Path.Combine(url.RelativePath!, NSBundle.MainBundle.BundleIdentifier, subDir);
        if (!Directory.Exists(ret))
        {
            Directory.CreateDirectory(ret);
        }

        return ret;
    }
}
