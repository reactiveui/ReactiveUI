// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using Android.Content;
using Android.Hardware.Usb;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Extension methods for the usb manager.
/// </summary>
public static class UsbManagerExtensions
{
    /// <summary>
    /// The intent action used when requesting USB permission.
    /// </summary>
    private const string ActionUsbPermission = "com.reactiveui.USB_PERMISSION";

    /// <summary>
    /// Requests temporary permission for the given package to access the device.
    /// This may result in a system dialog being displayed to the user if permission had not already been granted.
    /// </summary>
    /// <returns>The observable sequence of permission values.</returns>
    /// <param name="manager">The UsbManager system service.</param>
    /// <param name="context">The Context to request the permission from.</param>
    /// <param name="device">The UsbDevice to request permission for.</param>
    public static IObservable<bool>
        PermissionRequested(this UsbManager manager, Context context, UsbDevice device) =>
        new DevicePermissionObservable(manager, context, device);

    /// <summary>
    /// Requests temporary permission for the given package to access the accessory.
    /// This may result in a system dialog being displayed to the user if permission had not already been granted.
    /// </summary>
    /// <returns>The observable sequence of permission values.</returns>
    /// <param name="manager">The UsbManager system service.</param>
    /// <param name="context">The Context to request the permission from.</param>
    /// <param name="accessory">The UsbAccessory to request permission for.</param>
    public static IObservable<bool> PermissionRequested(
        this UsbManager manager,
        Context context,
        UsbAccessory accessory) =>
        new AccessoryPermissionObservable(manager, context, accessory);

    /// <summary>
    /// Requests USB device permission on subscribe and surfaces the granted result — replacing <c>Observable.Create</c>.
    /// The broadcast receiver is unregistered when the subscription is disposed.
    /// </summary>
    /// <param name="manager">The USB manager system service.</param>
    /// <param name="context">The context to request the permission from.</param>
    /// <param name="device">The USB device to request permission for.</param>
    private sealed class DevicePermissionObservable(UsbManager manager, Context context, UsbDevice device)
        : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            UsbDevicePermissionReceiver usbPermissionReceiver = new(observer, device);
            context.RegisterReceiver(usbPermissionReceiver, new(ActionUsbPermission));

            var intent = PendingIntent.GetBroadcast(context, 0, new(ActionUsbPermission), 0);
            manager.RequestPermission(device, intent);

            return new ActionDisposable(() => context.UnregisterReceiver(usbPermissionReceiver));
        }
    }

    /// <summary>
    /// Requests USB accessory permission on subscribe and surfaces the granted result — replacing <c>Observable.Create</c>.
    /// The broadcast receiver is unregistered when the subscription is disposed.
    /// </summary>
    /// <param name="manager">The USB manager system service.</param>
    /// <param name="context">The context to request the permission from.</param>
    /// <param name="accessory">The USB accessory to request permission for.</param>
    private sealed class AccessoryPermissionObservable(UsbManager manager, Context context, UsbAccessory accessory)
        : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            UsbAccessoryPermissionReceiver usbPermissionReceiver = new(observer, accessory);
            context.RegisterReceiver(usbPermissionReceiver, new(ActionUsbPermission));

            var intent = PendingIntent.GetBroadcast(context, 0, new(ActionUsbPermission), 0);
            manager.RequestPermission(accessory, intent);

            return new ActionDisposable(() => context.UnregisterReceiver(usbPermissionReceiver));
        }
    }

    /// <summary>
    /// Private implementation of BroadcastReceiver to handle device permission requests.
    /// </summary>
    private sealed class UsbDevicePermissionReceiver(IObserver<bool> observer, UsbDevice device)
        : BroadcastReceiver
    {
        /// <summary>
        /// Handles the broadcast for a USB device permission result.
        /// </summary>
        /// <param name="context">The context in which the receiver is running.</param>
        /// <param name="intent">The intent containing the permission result.</param>
        [ObsoletedOSPlatform("android33.0")]
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent is null)
            {
                return;
            }

            var extraDevice = intent.GetParcelableExtra(UsbManager.ExtraDevice) as UsbDevice;
            if (device.DeviceName != extraDevice?.DeviceName)
            {
                return;
            }

            var permissionGranted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
            observer.OnNext(permissionGranted);
            observer.OnCompleted();
        }
    }

    /// <summary>
    /// Private implementation of BroadcastReceiver to handle accessory permission requests.
    /// </summary>
    private sealed class UsbAccessoryPermissionReceiver(IObserver<bool> observer, UsbAccessory accessory)
        : BroadcastReceiver
    {
        /// <summary>
        /// Handles the broadcast for a USB accessory permission result.
        /// </summary>
        /// <param name="context">The context in which the receiver is running.</param>
        /// <param name="intent">The intent containing the permission result.</param>
        [ObsoletedOSPlatform("android33.0")]
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.GetParcelableExtra(UsbManager.ExtraAccessory) is not UsbAccessory extraAccessory)
            {
                return;
            }

            if (accessory.Manufacturer != extraAccessory.Manufacturer || accessory.Model != extraAccessory.Model)
            {
                return;
            }

            var permissionGranted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
            observer.OnNext(permissionGranted);
            observer.OnCompleted();
        }
    }
}
