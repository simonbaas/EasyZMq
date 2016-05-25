using System;
using System.Text;
using Newtonsoft.Json;

namespace EasyZMq.Serialization
{
    internal class TypeAwareJsonSerializer : ISerializer
    {
        private readonly Encoding _textEncoding = Encoding.UTF8;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public object Deserialize(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var str = _textEncoding.GetString(bytes);

            return JsonConvert.DeserializeObject(str, _jsonSerializerSettings);
        }

        public byte[] Serialize<T>(T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var str = JsonConvert.SerializeObject(value, Formatting.None, _jsonSerializerSettings);

            return _textEncoding.GetBytes(str);
        }
    }
}