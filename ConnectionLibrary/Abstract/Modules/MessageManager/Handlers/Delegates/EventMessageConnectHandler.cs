using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Delegates
{
    public delegate void EventMessageConnectHandler(RemoteHostInfo remoteHost, EventMessageConnectArgs args);
}