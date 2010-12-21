using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;

namespace ReactiveXaml.Serialization
{
    public class JsonNetObjectSerializationProvider : IObjectSerializationProvider
    {
        ThreadLocal<SerializedItemsToGuidResolver> _guidResolver;
        ThreadLocal<JsonSerializer> _serializer;

        public JsonNetObjectSerializationProvider(IStorageEngine engine = null)
        {
            _guidResolver = new ThreadLocal<SerializedItemsToGuidResolver>(() =>
                new SerializedItemsToGuidResolver(engine));

            _serializer = new ThreadLocal<JsonSerializer>(() =>
                JsonSerializer.Create(new JsonSerializerSettings() {ContractResolver = _guidResolver.Value }));
        }

        public byte[] Serialize(object obj)
        {
            var ms = new MemoryStream();
            using (var writer = createWriterFromMemoryStream(ms)) {
                _guidResolver.Value.InitializeWithRootObject(obj);
                _serializer.Value.Serialize(writer, obj);
            }
            var ret = ms.ToArray();
            return ret;
        }

        public object Deserialize(byte[] data, Type type)
        {
            using (var reader = createReaderFromBytes(data)) {
                _guidResolver.Value.InitializeWithRootObject(null);
                return _serializer.Value.Deserialize(reader, type);
            }
        }

        public string SerializedDataToString(byte[] data)
        {
#if DEBUG
            return (new StreamReader(new MemoryStream(data))).ReadToEnd();
#else
            throw new NotImplementedException();
#endif
        }

        static JsonReader createReaderFromBytes(byte[] data)
        {
#if DEBUG
            return new JsonTextReader(new StreamReader(new MemoryStream(data)));
#else
            return new BsonReader(new MemoryStream(data));
#endif
        }

        static JsonWriter createWriterFromMemoryStream(MemoryStream ms)
        {
#if DEBUG
            return new JsonTextWriter(new StreamWriter(ms));
#else
            return new BsonWriter(ms);
#endif
        }
    }

    class SerializedItemsToGuidResolver : DefaultContractResolver
    {
        SerializableItemConverter _itemConverter;
        object _rootObject;

        public SerializedItemsToGuidResolver(IStorageEngine engine = null)
        {
            _itemConverter = new SerializableItemConverter(engine);
        }

        protected override JsonContract CreateContract(Type objectType) 
        {
            var ret = base.CreateContract(objectType);
            if ((typeof(ISerializableItem).IsAssignableFrom(objectType)) &&
                (_rootObject == null || _rootObject.GetType() != objectType)) {
                _rootObject = null;
                ret.Converter = _itemConverter;
            }

            return ret;
        }

        protected override JsonArrayContract CreateArrayContract(Type objectType) 
        {
            var ret = base.CreateArrayContract(objectType);
            if (typeof(ISerializableList).IsAssignableFrom(objectType)) {
            }

            return ret;
        }

        protected override IList<JsonProperty> CreateProperties(JsonObjectContract contract)
        {
            return base.CreateProperties(contract).Where(x => {
                // XXX: This is massively slow and dumb
                var attrs = contract.UnderlyingType.GetProperty(x.PropertyName).GetCustomAttributes(typeof (IgnoreDataMemberAttribute), true);
                return attrs.Length == 0;
            }).ToList();
        }

        public void InitializeWithRootObject(object rootObject)
        {
            _rootObject = rootObject;
        }
    }

    class SerializableItemConverter : JsonConverter
    {
        IStorageEngine _engine;
        public SerializableItemConverter(IStorageEngine engine = null)
        {
            _engine = engine ?? RxStorage.Engine;
        }

        public override bool CanConvert(Type objectType)
        {
            return (typeof (ISerializableItem).IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String) {
                throw new Exception(String.Format("Expected string, got {0}", reader.Value));
            }

            Guid contentHash;
            if (!Guid.TryParse((string)reader.Value, out contentHash)) {
                throw new Exception(String.Format("Expected Guid, got {0}", reader.Value));
            }

            return _engine.Load(contentHash);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var si = (ISerializableItem)value;
            _engine.Save(si);
            writer.WriteValue(si.ContentHash.ToString());
        }
    }

    class SerializableListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) 
        {
            return (typeof (ISerializableList).IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) 
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) 
        {
            ISer
        }
    }
}