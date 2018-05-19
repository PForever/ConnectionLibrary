using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;

namespace ConnectionLibrary.Abstract.DataObjects.Messages
{
    public class Request : IMessage
    {
        public DateTime TimeMarker { get; set; }
        public MessageType MessageType { get; set; }
        public string DeviceCode { get; set; }
        //TODO Request code дял ответа на запросы
        public string TargetDeviceCode { get; set; }

        public IDictionary<string, IList<Telemetry>> Telemetries { get; set; }
        public Devices Devices { get; set; }
        public Request(string deviceCode, string requestMessage, DateTime timeMarker, string targetDeviceCode, IDictionary<string, IList<Telemetry>> telemetries = null, Devices devices = null)
        {
            TimeMarker = timeMarker;
            Telemetries = telemetries;
            TargetDeviceCode = targetDeviceCode;
            Devices = devices;
            DeviceCode = deviceCode;
            MessageType = MessageType.Request;
        }

        public Request()
        {
        }
    }
}