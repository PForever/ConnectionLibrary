using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Delegates
{
    public delegate void EventErrHandler(RemoteHostInfo remoteHost, EventErrArgs args);
}