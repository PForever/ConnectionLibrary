using System;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.MessageControllers
{
    public abstract class ASenderController : IDisposable
    {
        protected abstract TcpSender TcpSender { get; }
        protected abstract UdpSender UdpSender { get; }
        public abstract void Dispose();
    }
}