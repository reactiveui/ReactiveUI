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
    public class WinRTAppDataDriver : ISuspensionDriver
    {
        public IObservable<object> LoadState()
        {
            return ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").ToObservable()
                .SelectMany(x => FileIO.ReadTextAsync(x, UnicodeEncoding.Utf8))
                .SelectMany(x => {
                    var line = x.IndexOf('\n');
                    var typeName = x.Substring(0, line-1); // -1 for CR
                    var serializer = new DataContractSerializer(Type.GetType(typeName));

                    // NB: WinRT is terrible
                    var obj = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(x.Substring(line+1))));
                    return Observable.Return(obj);
                });
        }

        public IObservable<Unit> SaveState(object state)
        {
            try {
                var ms = new MemoryStream();
                var writer = new StreamWriter(ms, Encoding.UTF8);
                var serializer = new DataContractSerializer(state.GetType());
                writer.WriteLine(state.GetType().AssemblyQualifiedName);
                writer.Flush();

                serializer.WriteObject(ms, state);

                return ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.xmlish", CreationCollisionOption.ReplaceExisting).ToObservable()
                    .SelectMany(x => FileIO.WriteBytesAsync(x, ms.ToArray()).ToObservable());
            } catch (Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        public IObservable<Unit> InvalidateState()
        {
            return ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").ToObservable()
                .SelectMany(x => x.DeleteAsync().ToObservable());
        }
    }
}
