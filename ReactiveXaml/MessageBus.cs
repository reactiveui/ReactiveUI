using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    public class MessageBus : IMessageBus 
    {
        Dictionary<Tuple<Type, string>, WeakReference> messageBus = 
            new Dictionary<Tuple<Type,string>,WeakReference>();

        public IObservable<T> Listen<T>(string Contract = null)
        {
            IObservable<T> ret = null;
            withMessageBus(typeof(T), Contract, (mb, tuple) => {
                ret = (IObservable<T>)mb[tuple].Target;
            });

            return ret;
        }

        public bool IsRegistered(Type Type, string Contract = null)
        {
            bool ret = false;
            withMessageBus(Type, Contract, (mb, tuple) => {
                ret = mb.ContainsKey(tuple) && mb[tuple].IsAlive;
            });

            return ret;
        }

        public void RegisterMessageSource<T>(IObservable<T> Source, string Contract = null)
        {
            withMessageBus(typeof(T), Contract, (mb, tuple) => {
                mb[tuple] = new WeakReference(Source);
            });
        }

        public void SendMessage<T>(T Message, string Contract = null)
        {
            withMessageBus(typeof(T), Contract, (mb, tuple) => {
                var subj = mb[tuple].Target as Subject<T>;

                if (subj == null) {
                    subj = new Subject<T>();
                    IObservable<T> prev = mb[tuple].Target as IObservable<T>;
                    if (prev != null) {
                        prev.Subscribe(subj.OnNext, subj.OnError, subj.OnCompleted);
                    }
                    
                    mb[tuple] = new WeakReference(subj);
                }

                subj.OnNext(Message);
            });
        }

        public static IMessageBus Current {
            get { return RxApp.MessageBus; }
        }

        void withMessageBus(Type Type, string Contract, Action<Dictionary<Tuple<Type, string>, WeakReference>, Tuple<Type, string>> block)
        {
            lock(messageBus) {
                var tuple = new Tuple<Type, String>(Type, Contract);
                block(messageBus, tuple);
                if (messageBus.ContainsKey(tuple) && !messageBus[tuple].IsAlive) {
                    messageBus.Remove(tuple);
                }
            }
        }
    }

    public static class MessageBusMixins
    {
        public static void RegisterViewModel<T>(this IMessageBus This, T Source, string Contract = null)
            where T : IReactiveNotifyPropertyChanged
        {
            string contractName = viewModelContractName(typeof(T), Contract);
            if (This.IsRegistered(typeof(ObservedChange<T, Unit>), contractName)) {
                throw new Exception(typeof(T).FullName + " must be a singleton class or have a unique Contract name");
            }

            This.RegisterMessageSource(
                Source.Select(x => new ObservedChange<T, Unit>() { Sender = Source, PropertyName = x.PropertyName }), 
                contractName);

            This.RegisterMessageSource(
                 Observable.Defer(() => Observable.Return(Source)), viewModelCurrentValueContractName(typeof(T), Contract));
        }

        public static IObservable<ObservedChange<T, Unit>> ListenToViewModel<T>(this IMessageBus This, string Contract = null)
        {
            return This.Listen<ObservedChange<T, Unit>>(viewModelContractName(typeof(T), Contract));
        }

        public static T ViewModelForType<T>(this IMessageBus This, string Contract = null)
        {
            return This.Listen<T>(viewModelCurrentValueContractName(typeof(T), Contract)).First();
        }

        static string viewModelContractName(Type Type, string Contract)
        {
            return Type.FullName + "_" + (Contract ?? "");
        }

        static string viewModelCurrentValueContractName(Type Type, string Contract)
        {
            return viewModelContractName(Type, Contract) + "__current";
        }
    }
}
