using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveXaml;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ReactiveXaml.Serialization
{
    [DataContract]
    public abstract class ModelBase : ReactiveValidatedObject, ISerializableItem
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
            get { return Changing.Select(_ => this); }
        }

        [IgnoreDataMember]
        public IObservable<object> ItemChanged {
            get { return Changed.Select(_ => this); }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
