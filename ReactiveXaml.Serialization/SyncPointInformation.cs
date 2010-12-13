using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ReactiveXaml.Serialization
{
    [DataContract]
    public class SyncPointInformation : ISyncPointInformation
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
    }
}
