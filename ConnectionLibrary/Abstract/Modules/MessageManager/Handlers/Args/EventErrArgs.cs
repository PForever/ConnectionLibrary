using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public class EventErrArgs : EventArgs, IEventIMessageArgs
    {
        public ErrorMessage ErrorMessage { get; }

        public IMessage Message => ErrorMessage;

        public EventErrArgs(ErrorMessage errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}