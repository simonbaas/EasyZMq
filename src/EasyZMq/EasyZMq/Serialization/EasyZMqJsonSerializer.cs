using Newtonsoft.Json;

namespace EasyZMq.Serialization
{
    public class EasyZMqJsonSerializer : ISerializer
    {
        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}