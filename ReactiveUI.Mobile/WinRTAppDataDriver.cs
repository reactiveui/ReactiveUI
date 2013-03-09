using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace ReactiveUI.Mobile
{
    public class WinRTAppDataDriver : ISuspensionDriver
    {
        public JsonSerializerSettings SerializerSettings { get; set; }

        public WinRTAppDataDriver() : this(null) { }
        public WinRTAppDataDriver(JsonSerializerSettings settings)
        {
            SerializerSettings = settings ?? new JsonSerializerSettings() {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
            };
        }


        public IObservable<T> LoadState<T>() where T : class, IApplicationRootState
        {
            var serializer = JsonSerializer.Create(SerializerSettings);

            return ApplicationData.Current.RoamingFolder.GetFileAsync("appData.json").ToObservable()
                .SelectMany(x => FileIO.ReadTextAsync(x, UnicodeEncoding.Utf8))
                .SelectMany(x => {
                    try {
                        var reader = new JsonTextReader(new StringReader(x));
                        var ret = serializer.Deserialize<T>(reader);
                        return Observable.Return(ret);
                    } catch (Exception ex) {
                        return Observable.Throw<T>(ex);
                    }
                });
        }

        public IObservable<Unit> SaveState<T>(T state) where T : class, IApplicationRootState
        {

            var serializer = JsonSerializer.Create(SerializerSettings);
            try {
                var writer = new StringWriter();
                serializer.Serialize(writer, state);

                return ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.json", CreationCollisionOption.ReplaceExisting).ToObservable()
                    .SelectMany(x => FileIO.WriteTextAsync(x, writer.GetStringBuilder().ToString(), UnicodeEncoding.Utf8).ToObservable());
            } catch (Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        public IObservable<Unit> InvalidateState()
        {
            return ApplicationData.Current.RoamingFolder.GetFileAsync("appData.json").ToObservable()
                .SelectMany(x => x.DeleteAsync().ToObservable());
        }
    }
}
