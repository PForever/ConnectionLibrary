using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Server;

namespace ConnectionLibrary.Abstract.Modules.MessageManager
{
    public abstract class AFullMessageReceiver : AMessageManager
    {
        protected abstract void EventRequestHandler(RemoteHostInfo remoteHost, EventRequestArgs args);
        protected abstract void EventConnectMessageHandler(RemoteHostInfo remoteHost, EventMessageConnectArgs args);
        protected abstract void EventCommandMessageHandler(RemoteHostInfo remoteHost, EventCommandMessageArgs args);
        protected abstract void EventTelemetryHandler(RemoteHostInfo remoteHost, EventTelemetryArgs args);
        protected abstract void EventErrHandler(RemoteHostInfo remoteHost, EventErrArgs args);
        protected abstract void EventOrderHandler(RemoteHostInfo remoteHost, EventOrderArgs args);
    }
}