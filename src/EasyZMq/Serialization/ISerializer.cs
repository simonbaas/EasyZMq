namespace EasyZMq.Serialization
{
    public interface ISerializer
    {
        object Deserialize(byte[] bytes);
        byte[] Serialize<T>(T obj);
    }
}