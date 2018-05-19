using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Delegates
{
    public delegate void EventCommandHandler(RemoteHostInfo remoteHost, EventCommandMessageArgs args);
}