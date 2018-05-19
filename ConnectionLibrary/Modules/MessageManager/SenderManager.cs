using ConnectionLibrary.Abstract.Modules.MessageManager.Clients;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.MessageControllers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;
using ConnectionLibrary.Abstract.Server;
using ConnectionLibrary.Modules.MessageManager.Serialize;
using LogSingleton;

namespace ConnectionLibrary.Modules.MessageManager
{
    public class SenderManager : ASenderController, IObjectParser, ILoggable
    {
        protected override TcpSender TcpSender { get; }
        protected override UdpSender UdpSender { get; }
        public override void Dispose()
        {
            Logger.Debug("Dispose SenderManager");

            TcpSender?.Dispose();
            UdpSender?.Dispose();
        }

        public SenderManager(string multicastHostint, int tcpRemotePort, int udpRemotePort)
        {
            //Logging.Logger.InitLogger();
            Logger = Logging.Log;
            Logger.Debug("Creating SenderManager");

            TcpSender = new TcpSender(tcpRemotePort);
            UdpSender = new UdpSender(multicastHostint, udpRemotePort);

            Logger.Debug("Created SenderManager");
        }

        ~SenderManager()
        {
            Logger.Debug("Close SenderManager");
        }
        public void OnRequest(RemoteHostInfo remoteHost, EventRequestArgs args)
        {
            Logger.Debug($"Invoked OnRequest for {remoteHost.Host} via {remoteHost.Protocol} file {args.RequestInfo}");
            string message = Serializing.GetString(args.RequestInfo);
            OnSend(this, new EventDataArg<string>(remoteHost, message));
        }
        public void OnConnectMessage(RemoteHostInfo remoteHost, EventMessageConnectArgs args)
        {
            Logger.Debug($"Invoked OnConnectMessage for {remoteHost.Host} via {remoteHost.Protocol} file {args.ConnectMessage}");
            string message = Serializing.GetString(args.ConnectMessage);
            OnSend(this, new EventDataArg<string>(remoteHost, message));
        }
        //TODO попробовать всё свести к передачи IEventIMessageArgs
        public void OnCommandMessage(RemoteHostInfo remoteHost, EventCommandMessageArgs args)
        {
            Logger.Debug($"Invoked OnCommandMessage for {remoteHost.Host} via {remoteHost.Protocol} file {args.CommandMessage}");
            string message = Serializing.GetString(args.CommandMessage);
            OnSend(this, new EventDataArg<string>(remoteHost, message));
        }
        public void OnTelemetry(RemoteHostInfo remoteHost, EventTelemetryArgs args)
        {
            Logger.Debug($"Invoked OnTelemetry for {remoteHost.Host} via {remoteHost.Protocol} file {args.TelemetryInfo}");
            string message = Serializing.GetString(args.TelemetryInfo);
            OnSend(this, new EventDataArg<string>(remoteHost, message));
        }

        public void OnEventErrorMessage(RemoteHostInfo remoteHost, EventErrArgs args)
        {
            Logger.Debug($"Invoked OnEventErrorMessage for {remoteHost.Host} via {remoteHost.Protocol} file {args.ErrorMessage}");
            string message = Serializing.GetString(args.ErrorMessage);
            OnSend(this, new EventDataArg<string>(remoteHost, message));
        }

        public void OnOrder(RemoteHostInfo remoteHost, EventOrderArgs args)
        {
            Logger.Debug($"Invoked OnOrder for {remoteHost.Host} via {remoteHost.Protocol} file {args.Order}");
            string message = Serializing.GetString(args.Order);
            OnSend(this, new EventDataArg<string>(remoteHost, message));
        }

        public void OnCall(RemoteHostInfo remoteHost, EventCallArgs args)
        {
            Logger.Debug($"Invoked OnCall for {remoteHost.Host} via {remoteHost.Protocol} file {args.CallInfo}");
            string message = Serializing.GetString(args.CallInfo);
            OnSend(this, new EventDataArg<string>(remoteHost, message));
        }

        public void OnSend(object sender, EventDataArg<string> e)
        {
            Logger.Info($"Send to {e.HostInfo.Host} via {e.HostInfo.Protocol} message {e.Data}");
            switch (e.HostInfo.Protocol)
            {
                case UdpSender.Name:
                    UdpSender.SendAsync(e.HostInfo.Host, e.Data);
                    break;
                case TcpSender.Name:
                    TcpSender.SendAsync(e.HostInfo.Host, e.Data);
                    break;
                default:
                    Logger.Error($"Unknow protocol {e.HostInfo.Protocol} for {e.HostInfo}, message {e.Data}");
                    break;
            }
        }
        public ILogger Logger { get; }
    }
}