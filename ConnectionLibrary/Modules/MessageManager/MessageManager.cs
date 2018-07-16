using System;
using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Abstract.Modules.MessageManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;
using LogSingleton;

namespace ConnectionLibrary.Modules.MessageManager
{
    public class MessageManager : AFullMessageReceiver, ILoggable
    {
        private readonly TimeSpan _timeOut = new TimeSpan(0, 0, 10);
        //TODO создать сохранение и чтение связки код-макадрес в/из бд. Читать через ConnectionWorker
        public MessageManager(string multicastHost, string myCode, IObjectParser sender, IMessageParser server, IDb dbManager)
        {
            Logger = Logging.Log;
            Logger.Debug("MessageManager Create");


            Sender = sender;
            Server = server;

            DbManager = dbManager;

            AddHandlers();
            Worker = new ConnectionWorker(multicastHost, _timeOut, sender, server, dbManager, myCode);
        }

        private void AddHandlers()
        {
            Server.CommandMessageReceived += EventCommandMessageHandler;
            Server.ConnectMessageReceived += EventConnectMessageHandler;
            Server.RequestReceived += EventRequestHandler;
            Server.TelemetryReceived += EventTelemetryHandler;
            Server.ErrorMessageReceived += EventErrHandler;
            Server.OrderReceived += EventOrderHandler;
            //TODO валидатор
            ConnectMessageReceived += (o, args) => DbManager.AddDevice(args.ConnectMessage.Device);
            TelemetryReceived += (o, args) => DbManager.AddData(args.TelemetryInfo);
        }

        #region ReceiveHandler

        //TODO ввести тип сообщения Эхо для установки канала соединения 
        protected override void EventRequestHandler(RemoteHostInfo remoteHost, EventRequestArgs args)
        {
            Logger.Debug($"MessageManager.EventRequestHandler Invoked for ({remoteHost.Host} via {remoteHost.Protocol} code: {args.Message.DeviceCode})");
            _RequestReceived?.Invoke(this, args);
        }

        protected override void EventConnectMessageHandler(RemoteHostInfo remoteHost, EventMessageConnectArgs args)
        {
            Logger.Debug($"MessageManager.EventConnectMessageHandler Invoked for ({remoteHost.Host} via {remoteHost.Protocol} code: {args.Message.DeviceCode})");
            //TODO валидация?
            var device = args.ConnectMessage.Device;
            Worker.AddAddress(device.Code, new Addresses(device.MacAddress, remoteHost.Host));
            _ConnectMessageReceived?.Invoke(this, args);
        }

        protected override void EventCommandMessageHandler(RemoteHostInfo remoteHost, EventCommandMessageArgs args)
        {
            Logger.Debug($"MessageManager.EventCommandMessageHandler Invoked for ({remoteHost.Host} via {remoteHost.Protocol} code: {args.Message.DeviceCode})");
            _CommandMessageReceived?.Invoke(this, args);
        }

        protected override void EventTelemetryHandler(RemoteHostInfo remoteHost, EventTelemetryArgs args)
        {
            Logger.Debug($"MessageManager.EventTelemetryHandler Invoked for ({remoteHost.Host} via {remoteHost.Protocol} code: {args.Message.DeviceCode})");
            //DbManager.AddData(args.TelemetryInfo);
            _TelemetryReceived?.Invoke(this, args);
        }

        protected override void EventErrHandler(RemoteHostInfo remoteHost, EventErrArgs args)
        {
            Logger.Debug($"MessageManager.EventErrHandler Invoked for ({remoteHost.Host} via {remoteHost.Protocol} code: {args.Message.DeviceCode})");
            _ErrorMessageReceived?.Invoke(this, args);
        }

        protected override void EventOrderHandler(RemoteHostInfo remoteHost, EventOrderArgs args)
        {
            Logger.Debug($"MessageManager.EventOrderHandler Invoked for ({remoteHost.Host} via {remoteHost.Protocol} code: {args.Message.DeviceCode})");
            _OrderReceived?.Invoke(this, args);
        }

        #endregion

        #region Events
        private event Action<object, EventOrderArgs> _OrderReceived;
        public override event Action<object, EventOrderArgs> OrderReceived
        {
            add => _OrderReceived += value;
            remove => _OrderReceived -= value;
        }

        private event Action<object, EventRequestArgs> _RequestReceived;
        public override event Action<object, EventRequestArgs> RequestReceived
        {
            add => _RequestReceived += value;
            remove => _RequestReceived -= value;
        }

        private event Action<object, EventTelemetryArgs> _TelemetryReceived;
        public override event Action<object, EventTelemetryArgs> TelemetryReceived
        {
            add => _TelemetryReceived += value;
            remove => _TelemetryReceived -= value;
        }

        private event Action<object, EventMessageConnectArgs> _ConnectMessageReceived;
        public override event Action<object, EventMessageConnectArgs> ConnectMessageReceived
        {
            add => _ConnectMessageReceived += value;
            remove => _ConnectMessageReceived -= value;
        }

        private event Action<object, EventErrArgs> _ErrorMessageReceived;
        public override event Action<object, EventErrArgs> ErrorMessageReceived
        {
            add => _ErrorMessageReceived += value;
            remove => _ErrorMessageReceived -= value;
        }

        private event Action<object, EventCommandMessageArgs> _CommandMessageReceived;
        public override event Action<object, EventCommandMessageArgs> CommandMessageReceived
        {
            add => _CommandMessageReceived += value;
            remove => _CommandMessageReceived -= value;
        }

        #endregion

        #region OnReceived

        public ILogger Logger { get; }
        //TODO все войды заменить на проброс ошибок. Шоб было
        public override void OnRequest(object sender, EventRequestArgs args)
        {
            //TODO подумать как можно блокировать только для конкретного устройства
            lock (this)
            {
                Logger.Debug($"MessageManager.OnRequest Invoked from {sender} code: {args.Message.TargetDeviceCode}");
                var err = Worker.OpenDeviceConnection(args.Message.TargetDeviceCode, out RemoteHostInfo connectInfo);
                if (err != 0) return;
                Sender.OnRequest(connectInfo, args);
                Worker.CloseDeviceConnection(args.Message.TargetDeviceCode, connectInfo);
            }
        }

        public override void OnConnectMessage(object sender, EventMessageConnectArgs args)
        {
            lock (this)
            {
                Logger.Debug($"MessageManager.OnRequest Invoked from {sender} code: {args.Message.TargetDeviceCode}");
                var err = Worker.OpenDeviceConnection(args.Message.TargetDeviceCode, out RemoteHostInfo connectInfo);
                if (err != 0) return;
                Sender.OnConnectMessage(connectInfo, args);
                Worker.CloseDeviceConnection(args.Message.TargetDeviceCode, connectInfo);
            }
        }

        public override void OnCommandMessage(object sender, EventCommandMessageArgs args)
        {
            lock (this)
            {
                Logger.Debug($"MessageManager.OnRequest Invoked from {sender} code: {args.Message.TargetDeviceCode}");
                var err = Worker.OpenDeviceConnection(args.Message.TargetDeviceCode, out RemoteHostInfo connectInfo);
                if (err != 0) return;
                Sender.OnCommandMessage(connectInfo, args);
                Worker.CloseDeviceConnection(args.Message.TargetDeviceCode, connectInfo);
            }
        }

        public override void OnTelemetry(object sender, EventTelemetryArgs args)
        {
            lock (this)
            {
                Logger.Debug($"MessageManager.OnRequest Invoked from {sender} code: {args.Message.TargetDeviceCode}");
                var err = Worker.OpenDeviceConnection(args.Message.TargetDeviceCode, out RemoteHostInfo connectInfo);
                if (err != 0) return;
                Sender.OnTelemetry(connectInfo, args);
                Worker.CloseDeviceConnection(args.Message.TargetDeviceCode, connectInfo);
            }
        }

        public override void OnEventErrorMessage(object sender, EventErrArgs args)
        {
            lock (this)
            {
                Logger.Debug($"MessageManager.OnRequest Invoked from {sender} code: {args.Message.TargetDeviceCode}");
                var err = Worker.OpenDeviceConnection(args.Message.TargetDeviceCode, out RemoteHostInfo connectInfo);
                if (err != 0) return;
                Sender.OnEventErrorMessage(connectInfo, args);
                Worker.CloseDeviceConnection(args.Message.TargetDeviceCode, connectInfo);
            }
        }

        public override void OnOrder(object sender, EventOrderArgs args)
        {
            lock (this)
            {
                Logger.Debug($"MessageManager.OnRequest Invoked from {sender} code: {args.Message.TargetDeviceCode}");
                var err = Worker.OpenDeviceConnection(args.Message.TargetDeviceCode, out RemoteHostInfo connectInfo);
                if (err != 0) return;
                Sender.OnOrder(connectInfo, args);
                Worker.CloseDeviceConnection(args.Message.TargetDeviceCode, connectInfo);
            }
        }

        #endregion
    }
}