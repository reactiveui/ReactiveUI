// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Mock Forms.
    /// </summary>
    public static class MockForms
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Init()
        {
            Device.Info = new MockDeviceInfo();
            Device.PlatformServices = new MockPlatformServices();

            DependencyService.Register<MockResourcesProvider>();
            DependencyService.Register<MockDeserializer>();
        }

        internal class MockPlatformServices : IPlatformServices
        {
            private Action<Action>? _invokeOnMainThread;
            private Action<Uri>? _openUriAction;
            private Func<Uri, CancellationToken, Task<Stream>>? _getStreamAsync;

            public MockPlatformServices(Action<Action>? invokeOnMainThread = null, Action<Uri>? openUriAction = null, Func<Uri, CancellationToken, Task<Stream>>? getStreamAsync = null)
            {
                _invokeOnMainThread = invokeOnMainThread;
                _openUriAction = openUriAction;
                _getStreamAsync = getStreamAsync;
            }

            public bool IsInvokeRequired
            {
                get { return false; }
            }

            public string? RuntimePlatform { get; set; }

            public OSAppTheme RequestedTheme => OSAppTheme.Dark;

            public static int Hex(int v) => v < 10 ? '0' + v : 'a' + v - 10;

            public void OpenUriAction(Uri uri)
            {
                if (_openUriAction != null)
                {
                    _openUriAction(uri);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            public void BeginInvokeOnMainThread(Action action)
            {
                if (_invokeOnMainThread == null)
                {
                    action();
                }
                else
                {
                    _invokeOnMainThread(action);
                }
            }

            public string GetMD5Hash(string input)
            {
                // Step 1, calculate MD5 hash from input
                var md5 = MD5.Create();
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Step 2, convert byte array to hex string
                var sb = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }

            public double GetNamedSize(NamedSize size, Type targetElement, bool useOldSizes)
            {
                switch (size)
                {
                    case NamedSize.Default:
                        return 10;

                    case NamedSize.Micro:
                        return 4;

                    case NamedSize.Small:
                        return 8;

                    case NamedSize.Medium:
                        return 12;

                    case NamedSize.Large:
                        return 16;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(size));
                }
            }

            public Ticker CreateTicker() => new MockTicker();

            public void StartTimer(TimeSpan interval, Func<bool> callback)
            {
                Timer? timer = null;
                TimerCallback onTimeout = o => BeginInvokeOnMainThread(() =>
                {
                    if (callback())
                    {
                        return;
                    }

                    timer?.Dispose();
                });
                timer = new Timer(onTimeout, null, interval, interval);
            }

            public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
            {
                if (_getStreamAsync == null)
                {
                    throw new NotImplementedException();
                }

                return _getStreamAsync(uri, cancellationToken);
            }

            public Assembly[] GetAssemblies() => new Assembly[0];

            public IIsolatedStorageFile GetUserStoreForApplication()
            {
                throw new NotImplementedException();
            }

            public string GetHash(string input) => throw new NotImplementedException();

            public Color GetNamedColor(string name) => Color.DeepSkyBlue;

            public void QuitApplication()
            {
            }

            public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint) => throw new NotImplementedException();
        }

        internal class MockDeserializer : IDeserializer
        {
            public Task<IDictionary<string, object>> DeserializePropertiesAsync() =>
                Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object>());

            public Task SerializePropertiesAsync(IDictionary<string, object> properties) =>
                Task.FromResult(false);
        }

        internal class MockResourcesProvider : ISystemResourcesProvider
        {
            public IResourceDictionary GetSystemResources()
            {
                var dictionary = new ResourceDictionary();
                Style style;
                style = new Style(typeof(Label));
                dictionary[Device.Styles.BodyStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 50);
                dictionary[Device.Styles.TitleStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 40);
                dictionary[Device.Styles.SubtitleStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 30);
                dictionary[Device.Styles.CaptionStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 20);
                dictionary[Device.Styles.ListItemTextStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 10);
                dictionary[Device.Styles.ListItemDetailTextStyleKey] = style;

                return dictionary;
            }
        }

        internal class MockTicker : Ticker
        {
            private bool _enabled;

            protected override void EnableTimer()
            {
                _enabled = true;

                while (_enabled)
                {
                    SendSignals(16);
                }
            }

            protected override void DisableTimer()
            {
                _enabled = false;
            }
        }

        internal class MockDeviceInfo : DeviceInfo
        {
            public override Size PixelScreenSize => throw new NotImplementedException();

            public override Size ScaledScreenSize => throw new NotImplementedException();

            public override double ScalingFactor => throw new NotImplementedException();
        }
    }
}
