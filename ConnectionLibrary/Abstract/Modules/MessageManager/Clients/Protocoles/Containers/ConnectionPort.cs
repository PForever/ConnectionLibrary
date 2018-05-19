using ConnectionLibrary.Abstract.Server;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers
{
    public class ConnectionPort : IConnectPoint<int>
    {
        public int Value { get; set; }
    }
}