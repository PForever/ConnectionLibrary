using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public class EventMessageConnectArgs : EventArgs, IEventIMessageArgs
    {
        public EventMessageConnectArgs(ConnectMessage connectMessage)
        {
            ConnectMessage = connectMessage;
        }

        public ConnectMessage ConnectMessage { get; }
        public IMessage Message => ConnectMessage;
    }
}