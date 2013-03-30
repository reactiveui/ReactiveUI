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
        public IObservable<T> LoadState<T>() where T : class, IApplicationRootState
        {
            try {
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.json");
                return Observable.Return(JsonConvert.DeserializeObject<T>(File.ReadAllText(target, Encoding.UTF8)));
            } catch (Exception ex) {
                return Observable.Throw<T>(ex);
            }
        }
        
        public IObservable<Unit> SaveState<T>(T state) where T : class, IApplicationRootState
        {
            try {
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.json");
                File.WriteAllText(target, JsonConvert.SerializeObject(state));

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

