using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace ReactiveUI.Mobile
{
    public class WinRTAppDataDriver : ISuspensionDriver
    {
        public IObservable<object> LoadState()
        {
            return ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").ToObservable()
                .SelectMany(x => FileIO.ReadTextAsync(x, UnicodeEncoding.Utf8))
                .SelectMany(x => {
                    var line = x.IndexOf('\n');
                    var typeName = x.Substring(0, line);
                    var serializer = new XmlSerializer(Type.GetType(typeName));
                    return Observable.Return(serializer.Deserialize(new StringReader(x.Substring(line + 1))));
                });
        }

        public IObservable<Unit> SaveState(object state)
        {
            try {
                var writer = new StringWriter();
                var serializer = new XmlSerializer(state.GetType());
                writer.WriteLine(state.GetType().FullName);
                serializer.Serialize(writer, state);

                return ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.xmlish", CreationCollisionOption.ReplaceExisting).ToObservable()
                    .SelectMany(x => FileIO.WriteTextAsync(x, writer.GetStringBuilder().ToString(), UnicodeEncoding.Utf8).ToObservable());
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
