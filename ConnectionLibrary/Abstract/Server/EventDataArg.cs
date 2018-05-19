using System;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibrary.Abstract.Server
{
    public class EventDataArg<T> : EventArgs
    {
        public RemoteHostInfo HostInfo { get; set; }
        public T Data { get; set; }

        public EventDataArg(RemoteHostInfo hostInfo, T data)
        {
            HostInfo = hostInfo;
            Data = data;
        }
    }
}