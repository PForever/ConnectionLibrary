using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.DataObjects.Messages
{
    public class ErrorMessage : IMessage
    {
        public ErrorMessage(string deviceCode, string errInfo, string targetDeviceCode)
        {
            DeviceCode = deviceCode;
            ErrInfo = errInfo;
            TargetDeviceCode = targetDeviceCode;
            MessageType = MessageType.Err;
        }

        public ErrorMessage()
        {
            
        }
        public string TargetDeviceCode { get; set; }

        public DateTime TimeMarker { get; set; }
        public MessageType MessageType { get; set; }
        public string DeviceCode { get; set; }
        public string ErrInfo { get; set; }
    }
}