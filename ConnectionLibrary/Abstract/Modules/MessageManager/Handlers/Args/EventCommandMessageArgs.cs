using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public class EventCommandMessageArgs : EventArgs, IEventIMessageArgs
    {
        public CommandMessage CommandMessage { get; }
        public EventCommandMessageArgs(CommandMessage commandMessage)
        {
            CommandMessage = commandMessage;
        }

        public IMessage Message => CommandMessage;
    }
}