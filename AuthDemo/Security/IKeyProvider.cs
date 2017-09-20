namespace AuthDemo.Security
{
    public interface IKeyProvider
    {
        byte[] GetKey();

        byte[] GetKey(int byteCount);
    }
}