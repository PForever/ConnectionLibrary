using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager;

namespace ConnectionLibrary.Abstract.Modules.DbManager
{
    public interface IDb
    {
        //TODO переделать в int out для проброса ошибок из базы
        void AddDevice(IDevice device);
        void AddData(Telemetry telemetry);
        event DataAddHandler DataAdded;
        //группировка телеметрии
        IDictionary<string, IList<Telemetry>> GetData(IList<string> deviceCodes = null,
            IList<string> properties = null, DateTime? dateTime = null);
        IList<string> GetDevices();
        Devices GetDevicesProperties(IList<string> deviceCodes = null, IList<string> propNames = null);
        Properties GetDeviceInfo(string code);
    }
}