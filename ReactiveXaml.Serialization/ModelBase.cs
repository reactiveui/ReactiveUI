using System;
using System.Runtime.Serialization;

namespace ReactiveXaml.Serialization
{
#if WINDOWS_PHONE
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public abstract class ModelBase : ReactiveObject, ISerializableItem
#else
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public abstract class ModelBase : ReactiveValidatedObject, ISerializableItem
#endif
    {
        static Guid inProgressGuid = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        [IgnoreDataMember] Guid _ContentHash;
        [IgnoreDataMember]
        public Guid ContentHash {
            get {
                if (_ContentHash == Guid.Empty) {
                    // XXX: This is a blatant hack to make sure that the JSON serializer
                    // doesn't end up recursively calling CalculateHash
                    _ContentHash = inProgressGuid;
                    _ContentHash = CalculateHash();
                }
                return _ContentHash;
            }
            protected set { _ContentHash = value; }
        }

        public ModelBase()
        {
            setupModelBase();
        }

        [OnDeserialized]
        void setupModelBase(StreamingContext sc) { setupModelBase(); }
        void setupModelBase()
        {
            this.Log().InfoFormat("Deserialized ModelBase 0x{0:X}", this.GetHashCode());
            Changed.Subscribe(_ => invalidateHash());
            invalidateHash();
        }

        public virtual Guid CalculateHash()
        {
            return new Guid(this.ObjectContentsHash());
        }

        protected virtual void invalidateHash()
        {
            ContentHash = Guid.Empty;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :