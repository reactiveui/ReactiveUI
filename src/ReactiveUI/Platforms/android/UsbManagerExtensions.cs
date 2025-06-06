﻿// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;

using Android.App;
using Android.Content;
using Android.Hardware.Usb;

namespace ReactiveUI;

/// <summary>
/// Extension methods for the usb manager.
/// </summary>
public static class UsbManagerExtensions
{
    private const string ActionUsbPermission = "com.reactiveui.USB_PERMISSION";

    /// <summary>
    /// Requests temporary permission for the given package to access the device.
    /// This may result in a system dialog being displayed to the user if permission had not already been granted.
    /// </summary>
    /// <returns>The observable sequence of permission values.</returns>
    /// <param name="manager">The UsbManager system service.</param>
    /// <param name="context">The Context to request the permission from.</param>
    /// <param name="device">The UsbDevice to request permission for.</param>
    public static IObservable<bool> PermissionRequested(this UsbManager manager, Context context, UsbDevice device) => // TODO: Create Test
        Observable.Create<bool>(observer =>
        {
            var usbPermissionReceiver = new UsbDevicePermissionReceiver(observer, device);
            context.RegisterReceiver(usbPermissionReceiver, new IntentFilter(ActionUsbPermission));

            var intent = PendingIntent.GetBroadcast(context, 0, new Intent(ActionUsbPermission), 0);
            manager.RequestPermission(device, intent);

            return Disposable.Create(() => context.UnregisterReceiver(usbPermissionReceiver));
        });

    /// <summary>
    /// Requests temporary permission for the given package to access the accessory.
    /// This may result in a system dialog being displayed to the user if permission had not already been granted.
    /// </summary>
    /// <returns>The observable sequence of permission values.</returns>
    /// <param name="manager">The UsbManager system service.</param>
    /// <param name="context">The Context to request the permission from.</param>
    /// <param name="accessory">The UsbAccessory to request permission for.</param>
    public static IObservable<bool> PermissionRequested(this UsbManager manager, Context context, UsbAccessory accessory) => // TODO: Create Test
        Observable.Create<bool>(observer =>
        {
            var usbPermissionReceiver = new UsbAccessoryPermissionReceiver(observer, accessory);
            context.RegisterReceiver(usbPermissionReceiver, new IntentFilter(ActionUsbPermission));

            var intent = PendingIntent.GetBroadcast(context, 0, new Intent(ActionUsbPermission), 0);
            manager.RequestPermission(accessory, intent);

            return Disposable.Create(() => context.UnregisterReceiver(usbPermissionReceiver));
        });

    /// <summary>
    /// Private implementation of BroadcastReceiver to handle device permission requests.
    /// </summary>
    private class UsbDevicePermissionReceiver(IObserver<bool> observer, UsbDevice device)
                : BroadcastReceiver
    {
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
    private class UsbAccessoryPermissionReceiver(IObserver<bool> observer, UsbAccessory accessory)
                : BroadcastReceiver
    {
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
