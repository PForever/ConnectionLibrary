using System.Net;
using ConnectionLibrary.Abstract.Server;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers
{
    public class ConnectionIpHost : IConnectPoint<IPEndPoint>
    {
        public IPEndPoint Value { get; set; }
    }
}