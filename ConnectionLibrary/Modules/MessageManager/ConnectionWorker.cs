using System;
using System.Threading.Tasks;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Abstract.Modules.MessageManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;
using LogSingleton;

namespace ConnectionLibrary.Modules.MessageManager
{
    public class ConnectionWorker : AConnectionWorker, ICodeble
    {
        public string MyCode { get; set; }

        private readonly TimeSpan _timeOut;
        private const string EmptyMacAddress = "";
        //TODO сделать нормально
        private const string TcpProlocol = "TCP";
        private const string UdpProtocol = "UDP";
        private readonly string _multicastHost;
        protected ConnectionWorker() { }
        public ConnectionWorker(string multicastHost, TimeSpan timeOut, IObjectParser sender, IMessageParser server, IDb dbManager, string myCode)
        {
            _multicastHost = multicastHost;
            _timeOut = timeOut;
            Sender = sender;
            Server = server;
            MyCode = myCode;
            DbManager = dbManager;
            AddressBook = GetAddressBookDb();

            Server.CallReceived += ServerOnCallReceived;
        }
        public override ConnectionResult OpenDeviceConnection(string deviceCode, out RemoteHostInfo hostInfo)
        {
            ConnectionResult err;
            string ip;

            hostInfo = default(RemoteHostInfo);

            if (!AddressBook.ContainsKey(deviceCode))
            {
                err = UpdateAddressBook(deviceCode, AddressBook);
                if (err != 0) return err;
                err = GetIp(deviceCode, _timeOut, out ip);
                if (err != 0) return err;
                err = WakeUp(ip, deviceCode);
                if (err != 0) return err;
                AddressBook.AddOrUpdate(deviceCode, new Addresses(EmptyMacAddress, ip), (s, addresses) => addresses);
            }
            ip = AddressBook[deviceCode].IpAddress;
            if (String.IsNullOrEmpty(ip))
            {
                err = GetIp(deviceCode, _timeOut, out ip);
                if (err != 0) return err;
                AddressBook[deviceCode] = new Addresses(EmptyMacAddress, ip);
            }
            err = WakeUp(ip, deviceCode);
            if (err != 0) return err;
            hostInfo = new RemoteHostInfo(ip, TcpProlocol);
            return err;
        }

        private void ServerOnCallReceived(RemoteHostInfo remoteHostInfo, EventCallArgs eventCallArgs)
        {
            var callInfo = eventCallArgs.CallInfo;
            if (callInfo.TargetDeviceCode != MyCode) return;
            CallType type = callInfo.CallType;

            switch (type)
            {
                case CallType.Call:
                    Call recall = new Call(DateTime.Now, MyCode, CallType.Recall, callInfo.DeviceCode);
                    Sender.OnCall(remoteHostInfo, new EventCallArgs(recall));
                    break;
                case CallType.Recall:
                    break;
                case CallType.WakeUp:
                    break;
                case CallType.Ready:
                    break;
                case CallType.Sleep:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void CloseDeviceConnection(string deviceCode, RemoteHostInfo hostInfo)
        {
            Call callMessage = new Call(DateTime.Now, MyCode, CallType.Sleep, deviceCode);
            Sender.OnCall(hostInfo, new EventCallArgs(callMessage));
            //TODO ЗАЛОГИРОВАТЬ ВСЁ НАФИГ
        }

        protected override ConnectionResult UpdateAddressBook(string deviceCode, AddressBook addressBook)
        {
            var err = GetIp(deviceCode, _timeOut, out string ip);
            addressBook.AddOrUpdate(deviceCode, new Addresses(EmptyMacAddress, ip), (s, addresses) => addresses);
            return err;
        }

        protected override ConnectionResult GetIp(string deviceCode, TimeSpan timeOut, out string ip)
        {
            var remoteInfo = new RemoteHostInfo(_multicastHost, UdpProtocol);
            var wakeUpMessage = new Call(DateTime.Now, MyCode, CallType.Call, deviceCode);
            var arg = new EventCallArgs(wakeUpMessage);
            Sender.OnCall(remoteInfo, arg);
            return WaitRecall(deviceCode, _timeOut, out ip);
        }

        //TODO заменить протокол на энам
        protected override ConnectionResult WakeUp(string ip, string deviceCode)
        {
            var remoteInfo = new RemoteHostInfo(ip, UdpProtocol);
            var wakeUpMessage = new Call(DateTime.Now, MyCode, CallType.WakeUp, deviceCode);
            var arg = new EventCallArgs(wakeUpMessage);
            Sender.OnCall(remoteInfo, arg);
            return WaitReady(deviceCode, _timeOut);
        }

        #region Waiters

        protected ConnectionResult WaitReady(string deviceCode, TimeSpan timeOut)
        {
            var sData = new SynchronizeData();
            var sTime = new SynchronizeData();

            void TimeElapsed()
            {
                Task.Delay(timeOut).Wait();
                sTime.IsEmty = false;
            }
            void OnCallReady(RemoteHostInfo hostInfo, EventCallArgs args)
            {
                if(args.Message.DeviceCode != deviceCode) return;
                if(args.CallInfo.CallType != CallType.Ready) return;
                sData.IsEmty = false;
            }

            Server.CallReceived += OnCallReady;
            Task.Run(() => TimeElapsed());
            while (sData.IsEmty && sTime.IsEmty) {}
            Server.CallReceived -= OnCallReady;

            if (sData.ResultInfo == SynchronizeResult.Empty)
                return ConnectionResult.NotFound;
            return ConnectionResult.Successful;
        }

        protected ConnectionResult WaitRecall(string deviceCode, TimeSpan timeOut, out string ip)
        {
            var sTime = new SynchronizeData();
            var sData = new SynchronizeData<string>();

            void TimeElapsed()
            {
                Task.Delay(timeOut).Wait();
                sTime.IsEmty = false;
            }
            void OnCallReady(RemoteHostInfo hostInfo, EventCallArgs args)
            {
                if(args.Message.DeviceCode != deviceCode) return;
                if(args.CallInfo.CallType != CallType.Recall) return;
                sData.IsEmty = false;
                sData.Data = hostInfo.Host;
            }

            Server.CallReceived += OnCallReady;
            Task.Run(() => TimeElapsed());
            while (sData.IsEmty && sTime.IsEmty) {}
            Server.CallReceived -= OnCallReady;

            ip = sData.Data;
            if (sData.ResultInfo == SynchronizeResult.Empty)
                return ConnectionResult.NotFound;
            return ConnectionResult.Successful;
        }


        #endregion

        protected sealed override AddressBook GetAddressBookDb()
        {
            var devices = DbManager.GetDevices();
            AddressBook address = new AddressBook();

            foreach (string device in devices)
            {
                address.TryAdd(device, new Addresses());
            }

            return address;
        }

        public override void AddAddress(string code, Addresses addresses)
        {
            AddressBook.AddOrUpdate(code, addresses, (s, oldAddress) => addresses);
        }
    }
}