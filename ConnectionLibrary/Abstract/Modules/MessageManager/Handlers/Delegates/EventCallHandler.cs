using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Delegates
{
    public delegate void EventCallHandler(RemoteHostInfo remoteHost, EventCallArgs args);
}