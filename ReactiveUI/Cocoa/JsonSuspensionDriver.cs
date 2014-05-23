using System;
using System.Reactive.Linq;
using Newtonsoft.Json;
using MonoTouch.Foundation;
using System.Reactive;
using System.IO;
using System.Text;

namespace ReactiveUI.Mobile
{
    public class AppSupportJsonSuspensionDriver : ISuspensionDriver
    {
        public JsonSerializerSettings SerializerSettings { get; set; }

        public AppSupportJsonSuspensionDriver()
        {
            SerializerSettings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All,
            };
        }

        public IObservable<object> LoadState()
        {
            try {
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.json");
                return Observable.Return(JsonConvert.DeserializeObject(File.ReadAllText(target, Encoding.UTF8), SerializerSettings));
            } catch (Exception ex) {
                return Observable.Throw<object>(ex);
            }
        }
        
        public IObservable<Unit> SaveState(object state)
        {
            try {
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.json");
                File.WriteAllText(target, JsonConvert.SerializeObject(state, SerializerSettings));

                return Observable.Return(Unit.Default);
                
            } catch(Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }
        
        public IObservable<Unit> InvalidateState()
        {
            try {
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.json");
                File.Delete(target);

                return Observable.Return(Unit.Default);
                
            } catch(Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        string CreateAppDirectory(NSSearchPathDirectory targetDir, string subDir = "Data")
        {
            NSError err;
            
            var fm = new NSFileManager();
            var url = fm.GetUrl(targetDir, NSSearchPathDomain.All, null, true, out err);
            var ret = Path.Combine(url.RelativePath, NSBundle.MainBundle.BundleIdentifier, subDir);
            if (!Directory.Exists(ret)) Directory.CreateDirectory(ret);
            
            return ret;
        }
    }
}