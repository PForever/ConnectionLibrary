using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Delegates;

namespace ConnectionLibrary.Abstract.Modules.MessageManager
{
    public abstract class AConnectionWorker : AAddressManager
    {
        public abstract ConnectionResult OpenDeviceConnection(string deviceCode, out RemoteHostInfo hostInfo);
        public abstract void CloseDeviceConnection(string deviceCode, RemoteHostInfo remoteHostInfo);
        protected abstract ConnectionResult UpdateAddressBook(string deviceCode, AddressBook addressBook);
        protected abstract ConnectionResult GetIp(string deviceCode, TimeSpan timeOut, out string ip);
        protected abstract ConnectionResult WakeUp(string ip, string deviceCode);
    }
}