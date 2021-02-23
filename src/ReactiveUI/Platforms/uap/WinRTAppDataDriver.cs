// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace ReactiveUI
{
    /// <summary>
    /// Loads and saves state to persistent storage.
    /// </summary>
    public class WinRTAppDataDriver : ISuspensionDriver
    {
        /// <inheritdoc/>
        public IObservable<object> LoadState() =>
            ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").ToObservable()
                           .SelectMany(x => FileIO.ReadTextAsync(x, UnicodeEncoding.Utf8))
                           .SelectMany(x =>
                           {
                               var line = x.IndexOf('\n');
                               var typeName = x.Substring(0, line - 1); // -1 for CR
                               var serializer = new DataContractSerializer(Type.GetType(typeName));

                               // NB: WinRT is terrible
                               var obj = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(x.Substring(line + 1))));
                               return Observable.Return(obj);
                           });

        /// <inheritdoc/>
        public IObservable<Unit> SaveState(object state)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            try
            {
                var ms = new MemoryStream();
                var writer = new StreamWriter(ms, Encoding.UTF8);
                var serializer = new DataContractSerializer(state.GetType());
                writer.WriteLine(state.GetType().AssemblyQualifiedName);
                writer.Flush();

                serializer.WriteObject(ms, state);

                return ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.xmlish", CreationCollisionOption.ReplaceExisting).ToObservable()
                    .SelectMany(x => FileIO.WriteBytesAsync(x, ms.ToArray()).ToObservable());
            }
            catch (Exception ex)
            {
                return Observable.Throw<Unit>(ex);
            }
        }

        /// <inheritdoc/>
        public IObservable<Unit> InvalidateState() =>
            ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").ToObservable()
                           .SelectMany(x => x.DeleteAsync().ToObservable());
    }
}
