using System;

namespace ConnectionLibrary.Abstract.Server
{
    public interface IListener<T, S> : IDisposable
    {
        IConnectPoint<S> LocalHost { get; set; }
        event Action<object, EventDataArg<string>> OnMessage;
        bool IsListening { get; }
        void Start();
        void Stop();
    }
}