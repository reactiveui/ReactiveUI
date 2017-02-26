using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ReactiveUI
{
    /// <summary>
    /// Bundle Suspension Driver
    /// </summary>
    public class BundleSuspensionDriver : ISuspensionDriver
    {
        /// <summary>
        /// Invalidates the application state (i.e. deletes it from disk)
        /// </summary>
        /// <returns></returns>
        public IObservable<Unit> InvalidateState()
        {
            try {
                AutoSuspendHelper.LatestBundle.PutByteArray("__state", new byte[0]);
                return Observables.Unit;
            } catch (Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        /// <summary>
        /// Loads the application state from persistent storage
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Saves the application state to disk.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IObservable<Unit> SaveState(object state)
        {
            try {
                var serializer = new BinaryFormatter();
                var st = new MemoryStream();

                AutoSuspendHelper.LatestBundle.PutByteArray("__state", st.ToArray());
                return Observables.Unit;
            } catch (Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }
    }
}