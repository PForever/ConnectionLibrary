using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;
using ConnectionLibrary.Abstract.Server;
using LogSingleton;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients
{
    public class UdpSender : IUdp, ISender<string, IPEndPoint>, ILoggable
    {
        //TODO переделать атрибутами
        public const string Name = "UDP";
        private string _multicastHostint;
        public int Port { get; set; }
        public void Dispose()
        {
            UdpClient.Close();
        }

        public UdpSender(string multicastHostint, int remotePort)
        {
            _multicastHostint = multicastHostint;
            Logger = Logging.Log;
            Logger.Debug($"Create UdpSender to {remotePort}");
            Port = remotePort;
            UdpClient = new UdpClient();
            UdpClient.JoinMulticastGroup(IPAddress.Parse(multicastHostint));
        }
        public UdpClient UdpClient { get; }
        public IConnectPoint<IPEndPoint> RemoteHost { get; set; }
        //public void SendAsync(string data)
        //{
        //    Logger.Info($"Send to {RemoteHost.Value.Address}:{RemoteHost.Value.Port} via {Name} message {data}");
        //    byte[] buffer = Encoding.UTF8.GetBytes(data);
        //    UdpClient.SendAsync(buffer, buffer.Length, RemoteHost.Value);
        //}

        public void SendAsync(string host, string data)
        {
            Logger.Info($"Send to {host}:{Port} via {Name} message {data}");
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            UdpClient.Send(buffer, buffer.Length, host, Port); //TODO мб тоже будем порт держать
        }

        public ILogger Logger { get; }
    }
}