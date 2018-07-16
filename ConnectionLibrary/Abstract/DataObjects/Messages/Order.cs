using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.DataObjects.Messages
{
    public class Order : IMessage
    {
        public DateTime TimeMarker { get; set; }
        public MessageType MessageType { get; set; }
        public string DeviceCode { get; set; }
        public Dictionary<string, PropertiesValues> SetPropertiesValues { get; set; }
        public Dictionary<string, List<string>>  GetPropertiesValues { get; set; }
        public string TargetDeviceCode { get; set; }

        public Order()
        {
            
        }
        public Order(string deviceCode, DateTime timeMarker, string targetDeviceCode, Dictionary<string, PropertiesValues> setPropertiesValues = null, Dictionary<string, List<string>> getPropertiesValues = null)
        {
            TimeMarker = timeMarker;
            TargetDeviceCode = targetDeviceCode;
            DeviceCode = deviceCode;
            SetPropertiesValues = setPropertiesValues;
            GetPropertiesValues = getPropertiesValues;
            MessageType = MessageType.Order;
        }
    }
}