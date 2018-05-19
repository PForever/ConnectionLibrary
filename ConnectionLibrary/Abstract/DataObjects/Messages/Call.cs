using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.DataObjects.Messages
{
    public class Call : IMessage
    {
        public DateTime TimeMarker { get; set; }
        public MessageType MessageType { get; set; }
        public string DeviceCode { get; set; }
        public string TargetDeviceCode { get; set; }
        public CallType CallType { get; set; }
        public Call() { }
        public Call(DateTime timeMarker, string deviceCode, CallType callType, string targetDeviceCode)
        {
            TimeMarker = timeMarker;
            DeviceCode = deviceCode;
            CallType = callType;
            TargetDeviceCode = targetDeviceCode;
            MessageType = MessageType.Call;
        }
    }
}