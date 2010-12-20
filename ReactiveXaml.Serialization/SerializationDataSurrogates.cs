using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace ReactiveXaml.Serialization
{
    public class SerializationItemDataSurrogate : IDataContractSurrogate
    {
        IStorageEngine _engine;
        object _toSkip;

        public SerializationItemDataSurrogate(IStorageEngine engine = null, object rootObject = null)
        {
            _engine = engine ?? RxStorage.Engine;
            _toSkip = rootObject;
        }

        public Type GetDataContractType(Type type)
        {
            if (typeof(ISerializableItem).IsAssignableFrom(type) && _toSkip == null) {
                return typeof(Guid);
            }

            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (obj == _toSkip) {
                _toSkip = null;
                return obj;
            }

            if (obj is Guid && typeof(ISerializableItem).IsAssignableFrom(targetType)) {
                return _engine.Load((Guid) obj);
            }

            return obj;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (obj == _toSkip) {
                _toSkip = null;
                return obj;
            }

            var sib = obj as ISerializableItem;
            if (sib != null) {
                _engine.Save(sib);
                return sib.ContentHash;
            }
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType) { return null; }
        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType) { return null; }
        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes) { }
        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit) { return typeDeclaration; }
    }

    public class SerializedListData {
        public string ItemsRootTypeFullName { get; set; }
        public Dictionary<Guid, DateTimeOffset> CreatedOn { get; set; }
        public Dictionary<Guid, DateTimeOffset> UpdatedOn { get; set; }
        public Guid[] Items { get; set; }
    }

    public class SerializedListDataSurrogate : IDataContractSurrogate
    {
        IStorageEngine _engine;
        object _toUse;

        public SerializedListDataSurrogate(IStorageEngine engine = null, object rootObject = null)
        {
            _engine = engine ?? RxStorage.Engine;
            this._toUse = rootObject;
        }

        public Type GetDataContractType(Type type)
        {
            if (typeof(ISerializableList).IsAssignableFrom(type) && this._toUse != null) {
                return typeof (SerializedListData);
            }

            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            var listData = obj as SerializedListData;
            if (listData != null && typeof(ISerializableList).IsAssignableFrom(targetType) && obj == _toUse) {
                var newType = typeof (SerializedCollection<>).MakeGenericType(new[] {Type.GetType(listData.ItemsRootTypeFullName)});
                var items = listData.Items.Select(x => _engine.Load(x));
                _toUse = null;
                return Activator.CreateInstance(newType, new object[] {items, null, listData.CreatedOn, listData.UpdatedOn});
            }

            return obj;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            var slist = obj as ISerializableList;
            if (slist != null && obj == _toUse) {
                this._toUse = null;
                return new SerializedListData() {
                    CreatedOn = slist.CreatedOn.ToConcreteDictionary(),
                    UpdatedOn = slist.UpdatedOn.ToConcreteDictionary(),
                    ItemsRootTypeFullName = slist.GetBaseListType().FullName,
                    Items = slist.OfType<ISerializableItem>().Select(x => x.ContentHash).ToArray(),
                };
            }

            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType) { return null; }
        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType) { return null; }
        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes) { }
        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit) { return typeDeclaration; }
    }
}
