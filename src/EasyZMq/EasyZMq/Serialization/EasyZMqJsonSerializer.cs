using System.Text;
using Newtonsoft.Json;

namespace EasyZMq.Serialization
{
    public class EasyZMqJsonSerializer : ISerializer
    {
        private readonly Encoding _textEncoding = Encoding.UTF8;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public object Deserialize(byte[] bytes)
        {
            var str = _textEncoding.GetString(bytes);

            return JsonConvert.DeserializeObject(str, _jsonSerializerSettings);
        }

        public byte[] Serialize<T>(T value)
        {
            var str = JsonConvert.SerializeObject(value, Formatting.None, _jsonSerializerSettings);

            return _textEncoding.GetBytes(str);
        }
    }
}