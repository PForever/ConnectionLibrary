using System;
using System.Net.Sockets;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles
{
    public interface IUdp : IDisposable
    {
        UdpClient UdpClient { get; }
    }
}