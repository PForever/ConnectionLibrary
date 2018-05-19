using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Server;
using LogSingleton;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients
{
    public class TcpServer : ITcpServer, IListener<string, int>, ILoggable
    {
        private const int MaxLen = 256;
        public const string Name = "TCP";
        public void Dispose()
        {
            Stop();
        }

        public TcpServer(int localHost)
        {
            Logger = Logging.Log;
            Logger.Debug("Create TcpServer");
            LocalHost = new ConnectionPort {Value = localHost};
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, LocalHost.Value);
            TcpListener = new TcpListener(endpoint);
        }

        public TcpListener TcpListener { get; }
        public IConnectPoint<int> LocalHost { get; set; }

        #region OnMessage
        private event Action<object, EventDataArg<string>> _OnMessage;
        public event Action<object, EventDataArg<string>> OnMessage
        {
            add => _OnMessage += value;
            remove => _OnMessage -= value;
        }
        #endregion
        public bool IsListening { get; private set; }

        public void Start()
        {
            Logger.Debug("Start TcpServer");
            IsListening = true;
            //TODO обработка закрытия tcp... твою мать
            TcpListener.Start();
            _openConnection = ConnectionAsync();
        }
        //TODO найти бы способ дисконектиться поизященее
        private Task _openConnection;
        private async Task ConnectionAsync()
        {
            while (IsListening)
            {
                Logger.Debug($"Reciving message from localPort via {Name}");
                try
                {
                    using (var tcpClient = await TcpListener.AcceptTcpClientAsync())
                    using (var network = tcpClient.GetStream())
                    {
                        byte[] buffer = new byte[MaxLen];
                        StringBuilder messageBldr = new StringBuilder();
                        do
                        {
                            int partCount = network.Read(buffer, 0, buffer.Length);
                            messageBldr.Append(Encoding.UTF8.GetString(buffer, 0, partCount));
                        } while (network.DataAvailable);
                        string message = messageBldr.ToString();
                        string host = tcpClient.Client.RemoteEndPoint.GetIp();
                        RemoteHostInfo hostInfo = new RemoteHostInfo(host, Name);
                        Logger.Info($"Recived from {host} via {Name} message {message}");
                        _OnMessage?.Invoke(this, new EventDataArg<string>(hostInfo, message));
                    }
                }
                catch (Exception)
                {
                    Logger.Debug($"Reciving message from localPort via {Name} stoped");
                }
            }
        }

        public void Stop()
        {
            Logger.Debug("Stop TcpServer");
            IsListening = false;
            TcpListener?.Stop();
            _openConnection.Wait();
        }

        public ILogger Logger { get; }
    }
}
