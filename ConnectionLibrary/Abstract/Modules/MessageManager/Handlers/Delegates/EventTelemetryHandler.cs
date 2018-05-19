using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Delegates
{
    public delegate void EventTelemetryHandler(RemoteHostInfo remoteHost, EventTelemetryArgs args);
}