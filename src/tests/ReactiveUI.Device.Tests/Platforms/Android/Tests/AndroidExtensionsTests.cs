// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests the Android service-binding and shared-preference streams against the real platform services.</summary>
public class AndroidExtensionsTests
{
    /// <summary>The shared preferences file the tests write to.</summary>
    private const string PreferencesName = "rxui-device-tests";

    /// <summary>Committing a preference emits the changed key.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task PreferenceChanged_EmitsTheChangedKey()
    {
        var preferences = ActivityLauncher.TargetContext.GetSharedPreferences(PreferencesName, FileCreationMode.Private)!;
        var key = $"key-{Guid.NewGuid():N}";

        var changed = preferences.PreferenceChanged().FirstValueAsync(out var subscription);
        using (subscription)
        {
            _ = preferences.Edit()!.PutString(key, "value")!.Commit();

            await Assert.That(await changed.WithTimeout("the preference change")).IsEqualTo(key);
        }
    }

    /// <summary>Disposing the subscription unregisters the listener, so later changes are not seen.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task PreferenceChanged_AfterDispose_StopsEmitting()
    {
        var preferences = ActivityLauncher.TargetContext.GetSharedPreferences(PreferencesName, FileCreationMode.Private)!;
        List<string?> keys = [];

        var subscription = preferences.PreferenceChanged().Subscribe(new CollectingWitness(keys));
        _ = preferences.Edit()!.PutString("before", Guid.NewGuid().ToString())!.Commit();
        await Eventually.TrueAsync(() => keys.Contains("before"), "the change before dispose");

        subscription.Dispose();
        _ = preferences.Edit()!.PutString("after", Guid.NewGuid().ToString())!.Commit();
        await Task.Run(TestInstrumentation.Current.WaitForIdleSync);

        await Assert.That(keys).DoesNotContain("after");
    }

    /// <summary>Binding a local service emits the service's own binder.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ServiceBound_EmitsTheServiceBinder()
    {
        // The connection disposes the context it binds through, so give it a wrapper rather than the app context.
        var context = new ContextWrapper(ActivityLauncher.TargetContext);
        var intent = new Intent(ActivityLauncher.TargetContext, typeof(EchoService));

        var bound = context.ServiceBound<EchoBinder>(intent, Bind.AutoCreate).FirstValueAsync(out var subscription);
        using (subscription)
        {
            var binder = await bound.WithTimeout("the service connection");

            await Assert.That(binder).IsNotNull();
            await Assert.That(binder!.Echo("ping")).IsEqualTo("ping");
        }
    }

    /// <summary>Binding an intent that matches no service fails the stream.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ServiceBound_WithNoMatchingService_Fails()
    {
        var context = new ContextWrapper(ActivityLauncher.TargetContext);
        var intent = new Intent("net.reactiveui.devicetests.NO_SUCH_SERVICE");
        _ = intent.SetPackage(ActivityLauncher.TargetContext.PackageName);

        var bound = context.ServiceBound(intent, Bind.AutoCreate).FirstValueAsync(out var subscription);
        using (subscription)
        {
            await Assert.That(async () => await bound.WithTimeout("the bind failure")).Throws<InvalidOperationException>();
        }
    }

    /// <summary>Collects every value it sees.</summary>
    /// <param name="values">The list the values go into.</param>
    private sealed class CollectingWitness(List<string?> values) : IObserver<string?>
    {
        /// <inheritdoc/>
        public void OnNext(string? value)
        {
            lock (values)
            {
                values.Add(value);
            }
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }
    }
}
