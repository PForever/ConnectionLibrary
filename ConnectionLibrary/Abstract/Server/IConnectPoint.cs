namespace ConnectionLibrary.Abstract.Server
{
    public interface IConnectPoint<T>
    {
        T Value { get; set; }
    }
}