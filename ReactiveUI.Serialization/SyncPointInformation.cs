using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;

namespace ReactiveUI.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SyncPointInformation : ReactiveObject, ISyncPointInformation, IComparable<ISyncPointInformation>, IEquatable<ISyncPointInformation>, IComparable
    {
        [DataMember]
        public Guid RootObjectHash { get; protected set; }
        [DataMember]
        public Guid ParentSyncPoint { get; protected set; }
        [DataMember]
        public string RootObjectTypeName { get; protected set; }
        [DataMember]
        public string Qualifier { get; protected set; }
        [DataMember]
        public DateTimeOffset CreatedOn { get; protected set; }

        [IgnoreDataMember]
        public Guid ContentHash { get; protected set; }

        /// <summary>
        /// DONT USE THIS. This only exists to make the serializer not throw a fit
        /// </summary>
        public SyncPointInformation()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootObjectHash"></param>
        /// <param name="parentSyncPoint"></param>
        /// <param name="rootObjectType"></param>
        /// <param name="qualifier"></param>
        /// <param name="createdOn"></param>
        public SyncPointInformation(Guid rootObjectHash, Guid parentSyncPoint, Type rootObjectType, string qualifier, DateTimeOffset createdOn)
        {
            RootObjectHash = rootObjectHash;
            ParentSyncPoint = parentSyncPoint;
            RootObjectTypeName = rootObjectType.FullName;
            Qualifier = qualifier;
            CreatedOn = createdOn;
            ContentHash = new Guid(String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", 
                RootObjectHash, parentSyncPoint, RootObjectTypeName, Qualifier, CreatedOn).MD5Hash());
        }

        public IObservable<object> ItemChanging {
            get { return Observable.Never<object>(); }
        }

        public IObservable<object> ItemChanged {
            get { return Observable.Never<object>(); }
        }

        public Guid CalculateHash() 
        {
            return ContentHash;
        }

        public int CompareTo(ISyncPointInformation other)
        {
            int ret = 0;

            if ((ret = this.CreatedOn.CompareTo(other.CreatedOn)) != 0) {
                return ret;
            }

            return this.ContentHash.CompareTo(other.ContentHash);
        }

        public bool Equals(ISyncPointInformation other)
        {
            return (this.ContentHash == other.ContentHash);
        }

        public int CompareTo(object obj) 
        {
            if (!(obj is ISyncPointInformation)) {
                throw new ArgumentException();
            }
            return CompareTo((ISyncPointInformation) obj);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
