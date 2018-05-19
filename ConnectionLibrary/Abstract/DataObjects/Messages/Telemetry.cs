using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.DataObjects.Messages
{
    public class Telemetry : IMessage
    {
        public DateTime TimeMarker { get; set; }
        public MessageType MessageType { get; set; }
        public string DeviceCode { get; set; }
        public string TargetDeviceCode { get; set; }
        public PropertiesValues Values { get; set; }
        public Telemetry(string deviceCode, PropertiesValues values, DateTime timeMarker, string targetDeviceCode)
        {
            TimeMarker = timeMarker;
            TargetDeviceCode = targetDeviceCode;
            DeviceCode = deviceCode;
            Values = values;
            MessageType = MessageType.Telemetry;
        }

        public Telemetry()
        {
            
        }
    }
}