using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Microsoft.Phone.Shell;

namespace ReactiveUI
{
    public class PhoneServiceStateDriver : ISuspensionDriver
    {
        public IObservable<object> LoadState()
        {
            try {
                var state = (byte[]) PhoneApplicationService.Current.State["state"];
                var serializer = new DataContractSerializer(Type.GetType((string)PhoneApplicationService.Current.State["state_type"]));

                return Observable.Return(serializer.ReadObject(new MemoryStream(state)));
            } catch (Exception ex) {
                return Observable.Throw<object>(ex);
            }
        }

        public IObservable<Unit> SaveState(object state)
        {
            var serializer = new DataContractSerializer(state.GetType());

            try {
                var writer = new MemoryStream();
                serializer.WriteObject(writer, state);
                PhoneApplicationService.Current.State["state"] = writer.ToArray();
                PhoneApplicationService.Current.State["state_type"] = state.GetType().AssemblyQualifiedName;

                return Observable.Return(Unit.Default);
            } catch (Exception ex) {
                return Observable.Throw<Unit>(ex);
            }
        }

        public IObservable<Unit> InvalidateState()
        {
            PhoneApplicationService.Current.State["state"] = null;
            PhoneApplicationService.Current.State["state_type"] = null;
            return Observable.Return(Unit.Default);
        }
    }
}