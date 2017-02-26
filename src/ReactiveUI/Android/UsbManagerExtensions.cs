using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;

namespace ReactiveUI
{
    /// <summary>
    /// Usb Manager Extensions
    /// </summary>
    public static class UsbManagerExtensions
    {
        private const string ACTION_USB_PERMISSION = "com.reactiveui.USB_PERMISSION";

        /// <summary>
        /// Requests temporary permission for the given package to access the device. This may result
        /// in a system dialog being displayed to the user if permission had not already been granted.
        /// </summary>
        /// <returns>The observable sequence of permission values.</returns>
        /// <param name="manager">The UsbManager system service.</param>
        /// <param name="context">The Context to request the permission from.</param>
        /// <param name="device">The UsbDevice to request permission for.</param>
        public static IObservable<bool> PermissionRequested(this UsbManager manager, Context context, UsbDevice device)
        {
            return Observable.Create<bool>(observer => {
                var usbPermissionReceiver = new UsbDevicePermissionReceiver(observer, device);
                context.RegisterReceiver(usbPermissionReceiver, new IntentFilter(ACTION_USB_PERMISSION));

                var intent = PendingIntent.GetBroadcast(context, 0, new Intent(ACTION_USB_PERMISSION), 0);
                manager.RequestPermission(device, intent);

                return Disposable.Create(() => context.UnregisterReceiver(usbPermissionReceiver));
            });
        }

        /// <summary>
        /// Requests temporary permission for the given package to access the accessory. This may
        /// result in a system dialog being displayed to the user if permission had not already been granted.
        /// </summary>
        /// <returns>The observable sequence of permission values.</returns>
        /// <param name="manager">The UsbManager system service.</param>
        /// <param name="context">The Context to request the permission from.</param>
        /// <param name="accessory">The UsbAccessory to request permission for.</param>
        public static IObservable<bool> PermissionRequested(this UsbManager manager, Context context, UsbAccessory accessory)
        {
            return Observable.Create<bool>(observer => {
                var usbPermissionReceiver = new UsbAccessoryPermissionReceiver(observer, accessory);
                context.RegisterReceiver(usbPermissionReceiver, new IntentFilter(ACTION_USB_PERMISSION));

                var intent = PendingIntent.GetBroadcast(context, 0, new Intent(ACTION_USB_PERMISSION), 0);
                manager.RequestPermission(accessory, intent);

                return Disposable.Create(() => context.UnregisterReceiver(usbPermissionReceiver));
            });
        }

        /// <summary>
        /// Private implementation of BroadcastReceiver to handle accessory permission requests.
        /// </summary>
        private class UsbAccessoryPermissionReceiver
            : BroadcastReceiver
        {
            private readonly UsbAccessory accessory;
            private readonly IObserver<bool> observer;

            public UsbAccessoryPermissionReceiver(IObserver<bool> observer, UsbAccessory accessory)
            {
                this.observer = observer;
                this.accessory = accessory;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                var extraAccessory = intent.GetParcelableExtra(UsbManager.ExtraAccessory) as UsbAccessory;
                if (this.accessory.Manufacturer != extraAccessory.Manufacturer || this.accessory.Model != extraAccessory.Model)
                    return;

                var permissionGranted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
                this.observer.OnNext(permissionGranted);
                this.observer.OnCompleted();
            }
        }

        /// <summary>
        /// Private implementation of BroadcastReceiver to handle device permission requests.
        /// </summary>
        private class UsbDevicePermissionReceiver
            : BroadcastReceiver
        {
            private readonly UsbDevice device;
            private readonly IObserver<bool> observer;

            public UsbDevicePermissionReceiver(IObserver<bool> observer, UsbDevice device)
            {
                this.observer = observer;
                this.device = device;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                var extraDevice = intent.GetParcelableExtra(UsbManager.ExtraDevice) as UsbDevice;
                if (this.device.DeviceName != extraDevice.DeviceName)
                    return;

                var permissionGranted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
                this.observer.OnNext(permissionGranted);
                this.observer.OnCompleted();
            }
        }
    }
}