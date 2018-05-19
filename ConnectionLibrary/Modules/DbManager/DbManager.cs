using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.DbManager;
using DbSingleton;
using DbSingleton.Controller;
using LogSingleton;
using static SqliteDb.SqlHelper;

namespace ConnectionLibrary.Modules.DbManager
{
    public class DbManager : IDb, IReadeble, IWriteble, ICodeble, ILoggable
    {
        public ILogger Logger { get; }
        public IDataBaseReader DbReader { get; set; }
        public IDataBaseWriter DbWriter { get; set; }

        public DbManager(string myCode)
        {
            Logger = Logging.Log;
            DbReader = DbControlling.DbReader;
            DbWriter = DbControlling.DbWriter;
            MyCode = myCode;
        }

        public void AddDevice(IDevice device)
        {
            Logger.Debug($"Db Manager AddDevice Invoked (device code: {device.Code})");
            int err = DbWriter.AddDevice(device.Code, device.Name, device.MacAddress);
            if (err != 0)Logger.Error($"Db error on add {device.Code} code: {err}");
            //TODO на сторону бд!
            var props = device.Info;
            foreach (var prop in props)
            {
                err = DbWriter.AddProperties(device.Code, prop.Key, prop.Value.Type.ToString(), (prop.Value.IsSetter? 1 : 0).ToString(), prop.Value.Description);
                if (err != 0) Logger.Error($"Db error on add {prop.Key} code: {err}");
            }
        }

        public void AddData(Telemetry telemetry)
        {
            Logger.Debug(
                $"Db Manager AddData Invoked for telemetry (code: {telemetry.DeviceCode}, values {telemetry.Values}, timeMark {telemetry.TimeMarker})");
            foreach (KeyValuePair<string, string> propertiesValue in telemetry.Values)
            {
                //TODO не забуть сверять корректность времени устройств с сервером
                //TODO реализовать добавление списком на уровне базы
                DbWriter.AddTelemetry(telemetry.DeviceCode, propertiesValue.Key, telemetry.TimeMarker.TimeFormater(),
                    propertiesValue.Value);
            }

            DataAdded?.Invoke(this, telemetry);
        }

        public event DataAddHandler DataAdded;

        public IDictionary<string, DateValues> GetData(IList<string> deviceCodes = null, IList<string> properties = null)
        {
            Logger.Debug($"Db Manager GetData Invoked for (code: {deviceCodes} properties {properties})");
            IDictionary<string, DateValues> telemetiesData = new Dictionary<string, DateValues>();

            if (properties == null) return telemetiesData;
            int err = DbReader.GetTelemetries(out IDictionary<string, IList<(string propCode, string timeMarker, string propValue)>> propertiesData, deviceCodes, properties);
            if (err != 0 || propertiesData.Count == 0) return null;
            var dataValues = new DateValues(propertiesData.Count);
            foreach (var propertyData in propertiesData)
            {
                foreach (var value in propertyData.Value)
                {
                    dataValues.Add((value.propValue, value.timeMarker.TimeFormater()));
                }
                telemetiesData.Add(propertyData.Key, dataValues);
            }

            return telemetiesData;
        }

        protected IDictionary<string, IList<Telemetry>> CreateTelemetries(IDictionary<string, IList<(string propName, string timeMarker, string propValue)>> dictionary)
        {
            var telemetriesDictionary = new Dictionary<string, IList<Telemetry>>();
            foreach (var keyValuePair in dictionary)
            {
                var telemetriesList = CreateTelemetry(keyValuePair);
                telemetriesDictionary.Add(keyValuePair.Key, telemetriesList);
            }
            return telemetriesDictionary;
        }

        protected IList<Telemetry> CreateTelemetry(KeyValuePair<string, IList<(string propName, string timeMarker, string propValue)>> data)
        {
            IList<Telemetry> telemetries = new List<Telemetry>();
            foreach (var dataAtDate in data.Value.GroupBy(d => d.timeMarker))
            {
                var propValues = new PropertiesValues();
                foreach (var tuple in dataAtDate)
                {
                    propValues.Add(tuple.propName, tuple.propValue);
                }
                Telemetry telemetry = new Telemetry(MyCode, propValues, dataAtDate.First().timeMarker.TimeFormater(), data.Key);
                telemetries.Add(telemetry);
            }
            return telemetries;
        }

        public IDictionary<string, IList<Telemetry>> GetData(IList<string> deviceCodes = null, IList<string> properties = null, DateTime? dateTime = null)
        {
            //TODO проверка на пустоту. Где её делать?
            Logger.Debug($"Db Manager GetDataLast Invoked for (code: {deviceCodes?.ToSb().ToString() ?? "Any"}, properies: {properties}");

            int err = DbReader.GetTelemetries(out var dictionary, deviceCodes, properties, dateTime?.TimeFormater());
            if (err != 0)
            {
                Logger.Error($"Db error code: {err}");
                return null;
            }

            return CreateTelemetries(dictionary);
        }

        //TODO обращение только к таблице девайсов
        public IList<string> GetDevices()
        {
            Logger.Debug($"Db Manager GetDevices Invoked");
            int err = DbReader.GetDevicesPropertyNames(out var devices);
            if (err != 0)
            {
                Logger.Error($"Db error code: {err}");
                return null;
            }
            return devices.Keys.ToList();
        }

        public Devices GetDevicesProperties(IList<string> deviceCodes = null, IList<string> propNames = null)
        {
            int err = DbReader.GetDevicesProperties(out var propertiesData, deviceCodes, propNames);
            if (err != 0)
            {
                Logger.Error($"Db error code: {err}");
                return null;
            }

            IList<(string deviceCode, string deviceName, string macAddress)> devicesData;
            if (propNames == null || propNames.Count == 0)
                err = DbReader.GetDevices(out devicesData, deviceCodes);
            else
                err = DbReader.GetDevicesByPropNames(out devicesData, propNames, deviceCodes);
            if (err != 0)
            {
                Logger.Error($"Db error code: {err}");
                return null;
            }

            var devices = new Devices();
            foreach (var deviceTube in devicesData)
            {
                var data = propertiesData[deviceTube.deviceCode];
                var propInfos = new Properties(data.Count);
                foreach (var props in data)
                {
                    ProperyType.TryParse(props.propType, out ProperyType propType);
                    var propInfo = new PropertyInfo(props.description, props.isSetter.BoolFormater(), propType);
                    propInfos.Add(props.propCode, propInfo);
                }
                Device device = new Device(deviceTube.deviceCode, deviceTube.macAddress, deviceTube.deviceName, propInfos);
                devices.Add(device.Code, device);
            }
            return devices;
        }

        public Properties GetDeviceInfo(string code)
        {
            throw new NotImplementedException();
        }

        public string MyCode { get; set; }
    }
}