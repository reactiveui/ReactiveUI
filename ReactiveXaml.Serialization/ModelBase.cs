using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Concurrency;
#endif

namespace ReactiveXaml.Serialization
{
#if WINDOWS_PHONE
    [DataContract]
    public abstract class ModelBase : ReactiveObject, ISerializableItem
#else
    [DataContract]
    public abstract class ModelBase : ReactiveValidatedObject, ISerializableItem
#endif
    {
        [IgnoreDataMember]
        public Guid ContentHash { get; protected set; }

        public ModelBase()
        {
            setupModelBase();
        }

        [OnDeserialized]
        void setupModelBase(StreamingContext sc) { setupModelBase(); }
        void setupModelBase()
        {
            this.Log().InfoFormat("Deserialized ModelBase 0x{0:X}", this.GetHashCode());
            Changed.Subscribe(_ => {
                ContentHash = CalculateHash();
                RxStorage.Engine.Save(this);
            });
            ContentHash = CalculateHash();
        }

        public virtual Guid CalculateHash()
        {
            return new Guid(this.ObjectContentsHash());
        }

        [IgnoreDataMember]
        public IObservable<object> ItemChanging {
            // XXX: We need the explicit type on SL4 :-/
            get { return Changing.Select<IObservedChange<object, object>, object>(_ => this); }
        }

        [IgnoreDataMember]
        public IObservable<object> ItemChanged {
            get { return Changed.Select<IObservedChange<object, object>, object>(_ => this); }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
