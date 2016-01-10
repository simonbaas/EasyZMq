namespace EasyZMq.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(string strObj);
        string Serialize<T>(T obj);
    }
}