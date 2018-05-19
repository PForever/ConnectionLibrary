using System;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Server;

namespace ConnectionLibrary.Abstract.Modules.MessageManager
{
    public interface IFullMessageReceiver
    {
        event Action<object, EventRequestArgs> RequestReceived;
        event Action<object, EventTelemetryArgs> TelemetryReceived;
        event Action<object, EventMessageConnectArgs> ConnectMessageReceived;
        event Action<object, EventErrArgs> ErrorMessageReceived;
        event Action<object, EventCommandMessageArgs> CommandMessageReceived;
        event Action<object, EventOrderArgs> OrderReceived;
    }
}