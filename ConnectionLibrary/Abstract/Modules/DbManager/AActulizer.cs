using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager;

namespace ConnectionLibrary.Abstract.Modules.DbManager
{
    public abstract class AActulizer
    {
        public TimeSpan ActualSpan { get; set; }
        protected IFullMessageSender MessageSendler;
        protected IDb DbController;
        protected abstract void UpdateOrder(List<Order> orders);
        protected IDictionary<string, IList<string>> UpdateCollection;
        protected abstract bool ActualCheck(Telemetry telemetry);
        public abstract ConnectionResult GetData(out IDictionary<string, IList<Telemetry>> telemetries, IList<string> getProperties, IDictionary<string, PropertiesValues> setProperties = null);
        public abstract Devices GetDevices(IList<string> deviceCodes = null, IList<string> propNames = null);
    }
}