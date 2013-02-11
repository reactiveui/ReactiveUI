using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;

namespace ReactiveUI.Mobile
{
    public class PhoneServiceStateDriver : ISuspensionDriver
    {
        public JsonSerializerSettings SerializerSettings { get; set; }

        public PhoneServiceStateDriver() : this(null) { }
        public PhoneServiceStateDriver(JsonSerializerSettings settings)
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
            try {
                var state = (string) PhoneApplicationService.Current.State["state"];
                var reader = new JsonTextReader(new StringReader(state));
                return Observable.Return(serializer.Deserialize<T>(reader));
            } catch (Exception ex) {
                return Observable.Throw<T>(ex);
            }
        }

        public IObservable<Unit> SaveState<T>(T state) where T : class, IApplicationRootState
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            try {
                var writer = new StringWriter();
                serializer.Serialize(writer, state);
                PhoneApplicationService.Current.State["state"] = writer.GetStringBuilder().ToString();
                return Observable.Return(Unit.Default);
            } catch (Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        public IObservable<Unit> InvalidateState()
        {
            PhoneApplicationService.Current.State["state"] = null;
            return Observable.Return(Unit.Default);
        }
    }
}