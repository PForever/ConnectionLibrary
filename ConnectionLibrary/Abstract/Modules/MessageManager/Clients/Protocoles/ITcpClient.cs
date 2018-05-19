using System.Net.Sockets;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles
{
    public interface ITcpClient
    {
        TcpClient TcpClient { get; }
    }
}