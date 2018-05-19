using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.MessageControllers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;
using ConnectionLibrary.Abstract.Server;
using ConnectionLibrary.Modules.MessageManager.Serialize;
using LogSingleton;

namespace ConnectionLibrary.Modules.MessageManager
{
    public sealed class ServerManager : AServerController, IMessageParser, ILoggable
    {
        protected override TcpServer TcpServer { get; }
        protected override UdpServer UdpServer { get; }
        public event Action<RemoteHostInfo, EventRequestArgs> RequestReceived;
        public event Action<RemoteHostInfo, EventTelemetryArgs> TelemetryReceived;
        public event Action<RemoteHostInfo, EventMessageConnectArgs> ConnectMessageReceived;
        public event Action<RemoteHostInfo, EventErrArgs> ErrorMessageReceived;
        public event Action<RemoteHostInfo, EventCommandMessageArgs> CommandMessageReceived;
        public event Action<RemoteHostInfo, EventCallArgs> CallReceived;
        public event Action<RemoteHostInfo, EventOrderArgs> OrderReceived;

        public ServerManager(string multicastHostint, int udpPort, int tcpPort)
        {
            Logger = Logging.Log;
            Logger.Debug("Creating ServerManager");

            TcpServer = new TcpServer(tcpPort);
            UdpServer = new UdpServer(multicastHostint, udpPort);

            TcpServer.OnMessage += (o, arg) => EventDataHandler(o, arg);
            UdpServer.OnMessage += (o, arg) => EventDataHandler(o, arg);

            Logger.Debug("Created ServerManager");
        }

        public void EventDataHandler(object sender, EventDataArg<string> e)
        {
            Logger.Info($"Invoked EventDataHandler for {e.HostInfo.Host} via {e.HostInfo.Protocol}, message {e.Data}");
            IMessage message = Deserializing.GetMessage(e.Data, out MessageType type);
            Logger.Debug($"File {message} deserialized to {type} for {e.HostInfo.Host} via {e.HostInfo.Protocol}, message {e.Data}");
            if (message == default(IMessage))
            {
                Logger.Error("Desirializing error");
                return;
            }
            switch (type)
            {
                case MessageType.Command:
                    {
                        CommandMessageReceived?.Invoke(e.HostInfo, new EventCommandMessageArgs(message as CommandMessage));
                        break;
                    }
                case MessageType.Connect:
                    {
                        ConnectMessageReceived?.Invoke(e.HostInfo, new EventMessageConnectArgs(message as ConnectMessage));
                        break;
                    }
                case MessageType.Request:
                    {
                        RequestReceived?.Invoke(e.HostInfo, new EventRequestArgs(message as Request));
                        break;
                    }
                case MessageType.Telemetry:
                    {
                        TelemetryReceived?.Invoke(e.HostInfo, new EventTelemetryArgs(message as Telemetry));
                        break;
                    }
                case MessageType.Call:
                    {
                        CallReceived?.Invoke(e.HostInfo, new EventCallArgs(message as Call));
                        break;
                    }
                case MessageType.Order:
                    {
                        OrderReceived?.Invoke(e.HostInfo, new EventOrderArgs(message as Order));
                        break;
                    }
                //TODO ВСЕХ НЕВЕРНЫХ СЖЕЧЬ!.. Всмысле, в error
                case MessageType.Err:
                    {
                        ErrorMessageReceived?.Invoke(e.HostInfo, new EventErrArgs(message as ErrorMessage));
                        break;
                    }

                default:
                    Logger.Error("Desirializing error unknow MessageType");
                    return;
            }
        }

        public void ServerStart()
        {
            Logger.Debug("Starting ServerManager");

            TcpServer?.Start();
            UdpServer?.Start();

            Logger.Debug("Started ServerManager");
        }
        public override void Dispose()
        {
            Logger.Debug("Disposing ServerManager");

            TcpServer?.Dispose();
            UdpServer?.Dispose();

            Logger.Debug("Disposed ServerManager");
        }

        public ILogger Logger { get; }
    }
}