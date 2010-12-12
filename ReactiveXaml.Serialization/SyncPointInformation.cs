using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ReactiveXaml.Serialization
{
    public class SyncPointInformation : ISyncPointInformation
    {
        Guid _RootObjectHash;
        public Guid RootObjectHash { 
            get { return _RootObjectHash; }
        }

        string _RootObjectTypeName;
        public string RootObjectTypeName { 
            get { return _RootObjectTypeName; }
        }

        string _Qualifier;
        public string Qualifier {
            get { return _Qualifier; }
        }

        DateTimeOffset _CreatedOn;
        public DateTimeOffset CreatedOn {
            get { return _CreatedOn; }
        }

        public SyncPointInformation(Guid rootObjectHash, Type rootObjectType, string qualifier, DateTimeOffset createdOn)
        {
            _RootObjectHash = rootObjectHash;
            _RootObjectTypeName = rootObjectType.FullName;
            _Qualifier = qualifier;
            _CreatedOn = createdOn;
            _ContentHash = new Guid(String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", 
                _RootObjectHash, _RootObjectTypeName, _Qualifier, _CreatedOn).MD5Hash());
        }

        Guid _ContentHash;
        public Guid ContentHash {
            get { return _ContentHash; }
        }

        public IObservable<object> ItemChanging {
            get { return Observable.Never<object>(); }
        }

        public IObservable<object> ItemChanged {
            get { return Observable.Never<object>(); }
        }

        public Guid CalculateHash() 
        {
            return _ContentHash;
        }
    }
}
