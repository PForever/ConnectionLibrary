using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Abstract.Modules.MessageManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using LogSingleton;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLibrary.Modules.DbManager
{
    public class Actualizer : AActulizer, ILoggable
    {
        protected Actualizer()
        {
            Logger = Logging.Log;
            Logger.Debug("Actualizer Create");
        }

        private string _myCode;
        public Actualizer(AMessageManager messageManager, IDb dbController, string myCode)
        {
            Logger = Logging.Log;
            Logger.Debug("Actualizer Create");

            _myCode = myCode;

            ActualSpan = new TimeSpan(0, 0, 10);
            DbController = dbController;
            MessageSendler = messageManager;
        }
        protected override void UpdateOrder(List<Order> orders)
        {
            foreach (Order order in orders)
            {
                MessageSendler.OnOrder(this, new EventOrderArgs(order));
            }
        }

        protected override bool ActualCheck(Telemetry telemetry)
        {
            bool result = DateTime.Now.Subtract(telemetry.TimeMarker) < ActualSpan;
            Logger.Debug($"ActualCheck for (code: {telemetry.DeviceCode} timeMark: {telemetry.TimeMarker}). Result: {result}");
            return result;
        }

        protected List<Order> CreateOrders(IDictionary<string, IList<Telemetry>> telemetries, out IList<string> devices, out IDictionary<string, IList<Telemetry>> oldTelemetries)
        {
            List<Order> orders = new List<Order>();
            oldTelemetries = new Dictionary<string, IList<Telemetry>>();
            devices = new List<string>();
            foreach (var telemetryList in telemetries)
            {
                var order = CreateOrder(telemetryList, out var oldTelemetriesList);
                if(oldTelemetriesList.Count == 0) continue;
                oldTelemetries.Add(telemetryList.Key, oldTelemetriesList);
                orders.Add(order);
                devices.Add(order.DeviceCode);
            }
            return orders;
        }
        protected List<Order> CreateOrders(IDictionary<string, IList<Telemetry>> telemetries, out IList<string> devices, out IDictionary<string, IList<Telemetry>> oldTelemetries, IDictionary<string, PropertiesValues> setProperties)
        {
            List<Order> orders = new List<Order>();
            oldTelemetries = new Dictionary<string, IList<Telemetry>>();
            devices = new List<string>();
            List<string> setKeys = setProperties.Keys.ToList();
            if(telemetries != null && telemetries.Count != 0)
                foreach (var telemetryList in telemetries)
                {
                    PropertiesValues setProps = setProperties.ContainsKey(telemetryList.Key) ? setProperties[telemetryList.Key] : null;
                    Order order = CreateOrder(telemetryList, out var oldTelemetriesList, setProps);
                    if(oldTelemetriesList.Count == 0) continue;
                    oldTelemetries.Add(telemetryList.Key, oldTelemetriesList);
                    orders.Add(order);
                    devices.Add(order.TargetDeviceCode);
                    setKeys.Remove(telemetryList.Key);
                }
            foreach (string key in setKeys)
            {
                Order setOrder = new Order(_myCode, DateTime.Now, key, setPropertiesValues: new Dictionary<string, PropertiesValues>{{key, setProperties[key]}});
                orders.Add(setOrder);
            }
            return orders;
        }

        //TODO убедиться, что в базе данные групперуются перед выдачей
        /// <summary>
        /// Создаёт запрос на обновление устаревшей телеметри
        /// Вызывающая сторона гарантирует, что телеметрия относится к одному устройству
        /// Гарантирует уникальность имен в запрашиваемых свойств
        /// </summary>
        /// <param name="telemetries"></param>
        /// <param name="oldTelemetries"></param>
        /// <param name="setProps"></param>
        /// <returns></returns>
        protected Order CreateOrder(KeyValuePair<string, IList<Telemetry>> telemetries,
            out List<Telemetry> oldTelemetries, PropertiesValues setProps = null)
        {
            oldTelemetries = new List<Telemetry>();
            var getList = new HashSet<string>();
            for (var i = 0; i < telemetries.Value.Count;)
            {
                Telemetry telemetry = telemetries.Value[i];
                if (ActualCheck(telemetry))
                {
                    i++;
                    continue;
                }

                foreach (string s in telemetry.Values.Keys)
                {
                    getList.Add(s);
                }

                oldTelemetries.Add(telemetry);
                telemetries.Value.Remove(telemetry);
            }
            var setPropDict = setProps == null ? null : new Dictionary<string, PropertiesValues>{{telemetries.Key, setProps}};
            var getPropertiesValues = new Dictionary<string, List<string>> {{telemetries.Key, getList.ToList()}};
            Order order = new Order(_myCode, DateTime.Now, telemetries.Key, getPropertiesValues: getPropertiesValues, setPropertiesValues: setPropDict);
            return order;
        }

        //TODO гарантировать добавление в результат старых данных в случае ошибки апдейта
        public override ConnectionResult GetData(out IDictionary<string, IList<Telemetry>> telemetries,
            IList<string> properties, IDictionary<string, PropertiesValues> setProperties = null)
        {
            telemetries = DbController.GetData(properties: properties);
            IDictionary<string, IList<Telemetry>> oldTelemetries;
            IList<string> devices;
            List<Order> orders = setProperties == null ? 
                CreateOrders(telemetries, out devices, out oldTelemetries) : 
                CreateOrders(telemetries, out devices, out oldTelemetries, setProperties);

            ConnectionResult err = ConnectionResult.Successful;
            UpdateOrder(orders);
            if (devices.Any())
            {
                IList<Telemetry> updatedTelemetries = null;
                //var task = Task.Run(()=> );
                //task.Wait();
                //err = task.Result;
                err = WaitRecall(devices, ActualSpan, out updatedTelemetries);
                BuildTelemetries(telemetries, updatedTelemetries);
            }
            return err;
        }

        public override Devices GetDevices(IList<string> deviceCodes = null, IList<string> propNames = null)
        {
            //TODO проверять устройтва на активность
            return DbController.GetDevicesProperties(deviceCodes, propNames);
        }

        /// <summary>
        /// Добавляет обновлённые данные к существующим
        /// Требует наличия в словаре всех кодов устройств, данные по которым были обновлены
        /// </summary>
        /// <param name="oldTelemeties"></param>
        /// <param name="updatedTelemetries"></param>
        protected void BuildTelemetries(IDictionary<string, IList<Telemetry>> oldTelemeties, IList<Telemetry> updatedTelemetries)
        {
            foreach (var telemetry in updatedTelemetries)
            {
                oldTelemeties[telemetry.DeviceCode].Add(telemetry);
            }
        }
        protected ConnectionResult WaitRecall(IList<string> deviceCodes, TimeSpan timeOut, out IList<Telemetry> telemetries)
        {
            StringBuilder devices = new StringBuilder(deviceCodes.Count);
            foreach (string deviceCode in deviceCodes)
            {
                devices.Append(deviceCode).Append("; ");
            }
            Logger.Debug($"Waiting Telemetry from {devices}");

            var sTime = new SynchronizeData();
            var sData = new SynchronizeData();
            ConcurrentBag<Telemetry> concurrent = new ConcurrentBag<Telemetry>();
            void TimeElapsed()
            {
                Task.Delay(timeOut).Wait();
                sTime.IsEmty = false;
            }
            void OnTelemetry(object sender, Telemetry args)
            {
                if (!deviceCodes.Contains(args.DeviceCode)) return;
                concurrent.Add(args);
                lock (deviceCodes) {deviceCodes.Remove(args.DeviceCode);}
                if (deviceCodes.Count == 0) sData.IsEmty = false;
            }
            DbController.DataAdded += OnTelemetry;
            Task.Run(() => TimeElapsed());
            while (sData.IsEmty && sTime.IsEmty) { }
            DbController.DataAdded -= OnTelemetry;

            telemetries = concurrent.ToArray();
            if (sData.ResultInfo == SynchronizeResult.Empty)
            {
                StringBuilder devicesLost = new StringBuilder(deviceCodes.Count);
                foreach (string deviceCode in deviceCodes)
                {
                    devicesLost.Append(deviceCode).Append("; ");
                }
                Logger.Error($"Not Found Telemetry from {devicesLost}");
                return ConnectionResult.NotFound;
            }
            Logger.Debug($"Waiting Telemetry from {devices}");
            return ConnectionResult.Successful;
        }
        public ILogger Logger { get; }
    }
}