using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public class EventRequestArgs : EventArgs, IEventIMessageArgs
    {
        public Request RequestInfo { get; }
        public EventRequestArgs(Request requestInfo)
        {
            RequestInfo = requestInfo;
        }

        public IMessage Message => RequestInfo;
    }
}