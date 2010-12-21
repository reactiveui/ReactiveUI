using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ReactiveXaml.Serialization
{
    public class DataContractSerializationProvider : IEnableLogger
    {
        IDataContractSurrogate _dataSurrogate;
        IEnumerable<Type> _knownTypes;
        Func<XmlObjectSerializer> _serializerFactory;

        public DataContractSerializationProvider(IEnumerable<Type> knownTypes = null, IEnumerable<IDataContractSurrogate> dataSurrogates = null, Func<XmlObjectSerializer> serializerFactory = null)
        {
            _knownTypes = knownTypes ?? Enumerable.Empty<Type>();
            _dataSurrogate = (dataSurrogates != null ? new AggregateDataContractSurrogate(dataSurrogates) : null);
            _serializerFactory = serializerFactory;
        }

        public byte[] Serialize(object obj)
        {
            var serializer = createSerializer(obj.GetType());
            using (var ms = new MemoryStream()) {
                serializer.WriteObject(ms, obj);
                return ms.ToArray();
            }
        }

        public object Deserialize(byte[] data, Type type)
        {
            var serializer = createSerializer(type);
#if DEBUG
            string json = Encoding.Default.GetString(data);
            this.Log().Info(json);
#endif
            using (var ms = new MemoryStream(data)) {
                return serializer.ReadObject(ms);
            }
        }

        public static string SerializedDataToString(byte[] data)
        {
            return Encoding.Default.GetString(data);
        }

        XmlObjectSerializer createSerializer(Type type)
        {
            if (_serializerFactory != null) {
                return _serializerFactory();
            }

            DataContractJsonSerializer serializer;
            var knownTypes = _knownTypes.Concat(new[] {type});

            if (this._dataSurrogate != null) {
                serializer = new DataContractJsonSerializer(type, knownTypes, Int32.MaxValue, false, this._dataSurrogate, false);
            } else {
                serializer = new DataContractJsonSerializer(type, knownTypes);
            }
            return serializer;
        }
    }
}