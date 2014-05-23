using System;
using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json;

namespace ReactiveUI.Mobile
{
    public class BundleSuspensionDriver : ISuspensionDriver
    {
        public JsonSerializerSettings SerializerSettings { get; set; }

        public BundleSuspensionDriver()
        {
            SerializerSettings = new JsonSerializerSettings() {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
            };
        }

        public IObservable<object> LoadState()
        {
            try {
                return Observable.Return(JsonConvert.DeserializeObject(
                    AutoSuspendActivityHelper.LatestBundle.GetString("__state"),
                    SerializerSettings));
            } catch (Exception ex) {
                return Observable.Throw<object>(ex);
            }
        }

        public IObservable<Unit> SaveState(object state)
        {
            try {
                AutoSuspendActivityHelper.LatestBundle.PutString("__state", JsonConvert.SerializeObject(state, SerializerSettings));
                return Observable.Return(Unit.Default);
            
            } catch(Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        public IObservable<Unit> InvalidateState()
        {
            try {
                AutoSuspendActivityHelper.LatestBundle.PutString("__state", "");
                return Observable.Return(Unit.Default);
            
            } catch(Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }
    }
}
