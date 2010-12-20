using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReactiveXaml;
using System.Reflection;
using System.Runtime.Serialization;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Disposables;
using System.ComponentModel;
using System.Globalization;

namespace ReactiveXaml.Serialization
{
    [TypeConverter(typeof(ExplicitReferenceTypeConverter))]
    public class ExplicitReference<T> : IExplicitReferenceBase
        where T : class, ISerializableItem
    {
        T valueCache;
        IDisposable watcher;
        Guid objRef;
        readonly IStorageEngine storage;

        public ExplicitReference(T obj = null, IStorageEngine engine = null)
        {
            this.storage = engine;
            this.Value = obj;
        }

        public T Value {
            get {
                if (valueCache != null)
                    return valueCache;
                if (objRef == Guid.Empty)
                    return null;

                return (valueCache = loadObjectFromStorage());
            }
            set { 
                if (isInUpdate) {
                    throw new Exception("Object is already being updated");
                }

                invalidateCache();
                this.objRef = (value != null ? value.ContentHash : Guid.Empty);
            }
        }

        bool isInUpdate;
        public IDisposable Update()
        {
            if (isInUpdate) {
                throw new Exception("Object is already being updated");
            }

            var to_set = Value;
            isInUpdate = true;
            return Disposable.Create(() => {
                isInUpdate = false;
                Value = to_set;
            });
        }

        public void Update(Action<T> block)
        {
            using(Update()) { block(this.Value); }
        }

        public Guid ValueHash {
            get { return objRef; }
            set {
                if (isInUpdate) {
                    throw new Exception("Object is already being updated");
                }

                invalidateCache();
                objRef = value;
            }
        }

        T loadObjectFromStorage()
        {
            invalidateCache();
            T ret = storage.Load<T>(ValueHash);

            if (ret != null) {
                watcher = ret.Changing.Subscribe(_ => {
                    if (isInUpdate)
                        return;
                    valueCache = null;
                });
            }

            return ret;
        }

        void invalidateCache()
        {
            if (watcher != null) {
                watcher.Dispose();
                watcher = null;
            }
            valueCache = null;
        }
    }

    public class ExplicitReferenceSurrogate : IDataContractSurrogate
    {
        public Type GetDataContractType(Type type)
        {
            if(typeof(IExplicitReferenceBase).IsAssignableFrom(type)) {
                return typeof(Guid);
            }

            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if(obj is Guid && typeof(IExplicitReferenceBase).IsAssignableFrom(targetType)) {
                var type = typeof(ExplicitReference<>).MakeGenericType(targetType);
                var ret = (IExplicitReferenceBase)Activator.CreateInstance(type, new object[]{null, null});
                ret.ValueHash = (Guid)obj;
                return ret;
            }
            return obj;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            var sib = obj as ISerializableItem;
            if (sib != null) {
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

    public class ExplicitReferenceTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(IExplicitReferenceBase).IsAssignableFrom(sourceType)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var val = value as IExplicitReferenceBase;

            if (val != null) {
                return RxStorage.Engine.Load(val.ValueHash);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :