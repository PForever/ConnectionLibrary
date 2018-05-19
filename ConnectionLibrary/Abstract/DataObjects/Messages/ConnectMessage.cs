using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;

namespace ConnectionLibrary.Abstract.DataObjects.Messages
{
    public class ConnectMessage : IMessage
    {
        public Device Device { get; set; }
        public ConnectMessage() { }
        public ConnectMessage(Device device, string deviceCode, DateTime timeMarker)
        {
            TimeMarker = timeMarker;
            Device = device;
            DeviceCode = deviceCode;
            MessageType = MessageType.Connect;
        }
        public ConnectMessage(IDevice device, string deviceCode, DateTime timeMarker) : this(new Device(device), deviceCode, timeMarker) {}
        public string TargetDeviceCode { get; set; }
        public DateTime TimeMarker { get; set; }
        public MessageType MessageType { get; set; }
        public string DeviceCode { get; set; }
    }
}