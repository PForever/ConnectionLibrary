using System;
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
    
    public class TcpSender : ITcpClient, ISender<string, IPEndPoint>, IDisposable, ILoggable
    {
        public const string Name = "TCP";
        public int Port { get; set; }
        public void Dispose()
        {
            TcpClient?.Close();
            //TcpClient?.Dispose();
        }

        public TcpSender(int remotePort)
        {
            Logger = Logging.Log;
            Logger.Debug($"Create TcpSender to {remotePort}");
            Port = remotePort;
        }
        public TcpClient TcpClient { get; private set; }
        //public async void SendAsync(string data)
        //{
        //    using (TcpClient = new TcpClient(/*RemoteHost.Value*/))
        //    {
        //        try
        //        {
        //            Logger.Debug($"Connect to {RemoteHost.Value.Address}:{RemoteHost.Value.Port} via {Name}");
        //            await TcpClient.ConnectAsync(RemoteHost.Value.Address, RemoteHost.Value.Port);
        //            using (var network = TcpClient.GetStream())
        //            {
        //                byte[] bytes = Encoding.UTF8.GetBytes(data);
        //                Logger.Info($"Send to {RemoteHost.Value.Address}:{RemoteHost.Value.Port} via {Name} message {data}");
        //                network.Write(bytes, 0, bytes.Length);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Error($"Connect to {RemoteHost.Value.Address}:{RemoteHost.Value.Port} via {Name} filed with err {e.Message}");
        //        }
        //    }
        //}
        public async void SendAsync(string host, string data)
        {
            using (TcpClient = new TcpClient(/*RemoteHost.Value*/))
            {
                try

                {
                    Logger.Debug($"Connect to {host}:{Port} via {Name}");
                    await TcpClient.ConnectAsync(host, Port);
                    using (var network = TcpClient.GetStream())
                    {
                        Logger.Info($"Send to {host}:{Port} via {Name} message {data}");
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        network.Write(bytes, 0, bytes.Length);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Connect to {host}:{Port} via {Name} filed with err {e.Message}");
                }
            }
        }

        public ILogger Logger { get; }
    }
}