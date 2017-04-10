using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Foundation;

namespace ReactiveUI
{
    public class AppSupportJsonSuspensionDriver : ISuspensionDriver
    {
        public IObservable<object> LoadState()
        {
            try {
                var serializer = new BinaryFormatter();
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.dat");

                var result = default(object);
                using (var st = File.OpenRead(target)) {
                    result = serializer.Deserialize(st);
                }

                return Observable.Return(result);
            } catch (Exception ex) {
                return Observable.Throw<object>(ex);
            }
        }

        public IObservable<Unit> SaveState(object state)
        {
            try {
                var serializer = new BinaryFormatter();
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.dat");

                using (var st = File.Open(target, FileMode.Create)) {
                    serializer.Serialize(st, state);
                }

                return Observables.Unit;

            } catch (Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        public IObservable<Unit> InvalidateState()
        {
            try {
                var target = Path.Combine(CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), "state.dat");
                File.Delete(target);

                return Observables.Unit;

            } catch (Exception ex) {
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