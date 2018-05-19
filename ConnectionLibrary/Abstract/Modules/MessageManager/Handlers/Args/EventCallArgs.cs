using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public class EventCallArgs : EventArgs, IEventIMessageArgs
    {
        public Call CallInfo { get; }
        public IMessage Message => CallInfo;

        public EventCallArgs(Call callInfo)
        {
            CallInfo = callInfo;
        }
    }
}