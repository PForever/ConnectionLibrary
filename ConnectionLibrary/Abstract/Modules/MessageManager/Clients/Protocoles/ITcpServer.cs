using System;
using System.Net.Sockets;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles
{
    public interface ITcpServer : IDisposable
    {
        TcpListener TcpListener { get; }    
    }
}