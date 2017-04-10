using System;
using System.Reactive;
using System.Reactive.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ReactiveUI
{
    /// <summary>
    ///
    /// </summary>
    public class BundleSuspensionDriver : ISuspensionDriver
    {
        public IObservable<object> LoadState()
        {
            try {
                // NB: Sometimes OnCreate gives us a null bundle
                if (AutoSuspendHelper.LatestBundle == null) {
                    return Observable.Throw<object>(new Exception("New bundle, start from scratch"));
                }

                var serializer = new BinaryFormatter();
                var st = new MemoryStream(AutoSuspendHelper.LatestBundle.GetByteArray("__state"));

                return Observable.Return(serializer.Deserialize(st));
            } catch (Exception ex) {
                return Observable.Throw<object>(ex);
            }
        }

        public IObservable<Unit> SaveState(object state)
        {
            try {
                var serializer = new BinaryFormatter();
                var st = new MemoryStream();

                AutoSuspendHelper.LatestBundle.PutByteArray("__state", st.ToArray());
                return Observables.Unit;
            
            } catch(Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        public IObservable<Unit> InvalidateState()
        {
            try {
                AutoSuspendHelper.LatestBundle.PutByteArray("__state", new byte[0]);
                return Observables.Unit;
            
            } catch(Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }
    }
}
