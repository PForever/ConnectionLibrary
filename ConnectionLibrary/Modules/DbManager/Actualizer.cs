using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Abstract.Modules.MessageManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using LogSingleton;

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

        //TODO убедиться, что в базе данные групперуются перед выдачей
        /// <summary>
        /// Создаёт запрос на обновление устаревшей телеметри
        /// Вызывающая сторона гарантирует, что телеметрия относится к одному устройству
        /// Гарантирует уникальность имен в запрашиваемых свойств
        /// </summary>
        /// <param name="telemetries"></param>
        /// <param name="oldTelemetries"></param>
        /// <returns></returns>
        protected Order CreateOrder(KeyValuePair<string, IList<Telemetry>> telemetries, out List<Telemetry> oldTelemetries)
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

            Order order = new Order(_myCode, DateTime.Now, telemetries.Key, getPropertiesValues: getList.ToList());
            return order;
        }
        //TODO гарантировать добавление в результат старых данных в случае ошибки апдейта
        public override ConnectionResult GetData(List<string> properties, out IDictionary<string, IList<Telemetry>> telemetries)
        {
            telemetries = DbController.GetData(properties: properties);
            List<Order> orders = CreateOrders(telemetries, out IList<string> devices, out IDictionary<string, IList<Telemetry>> oldTelemetries);
            UpdateOrder(orders);
            var err = WaitRecall(devices, ActualSpan, out IList<Telemetry> updatedTelemetries);
            BuildTelemetries(telemetries, updatedTelemetries);
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
                deviceCodes.Remove(args.DeviceCode);
                if (deviceCodes.Count == 0) sData.IsEmty = false;
            }
            DbController.DataAdded += OnTelemetry;
            Task.Run(() => TimeElapsed());
            while (sData.IsEmty && sTime.IsEmty) { }
            DbController.DataAdded -= OnTelemetry;

            telemetries = concurrent.ToArray();
            if (sData.ResultInfo == SynchronizeResult.Empty)
                return ConnectionResult.NotFound;
            return ConnectionResult.Successful;
        }
        public ILogger Logger { get; }
    }
}