using System;
using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;

namespace ConnectionLibrary.Abstract.Modules.MessageManager
{
    public abstract class AMessageManager : AConnecter, IFullMessageReceiver, IFullMessageSender
    {
        protected AConnectionWorker Worker;
        protected IDb DbManager;

        public abstract event Action<object, EventRequestArgs> RequestReceived;
        public abstract event Action<object, EventTelemetryArgs> TelemetryReceived;
        public abstract event Action<object, EventMessageConnectArgs> ConnectMessageReceived;
        public abstract event Action<object, EventErrArgs> ErrorMessageReceived;
        public abstract event Action<object, EventCommandMessageArgs> CommandMessageReceived;
        public abstract event Action<object, EventOrderArgs> OrderReceived;

        public abstract void OnRequest(object sender, EventRequestArgs args);
        public abstract void OnConnectMessage(object sender, EventMessageConnectArgs args);
        public abstract void OnCommandMessage(object sender, EventCommandMessageArgs args);
        public abstract void OnTelemetry(object sender, EventTelemetryArgs args);
        public abstract void OnEventErrorMessage(object sender, EventErrArgs args);
        public abstract void OnOrder(object sender, EventOrderArgs args);
    }
}