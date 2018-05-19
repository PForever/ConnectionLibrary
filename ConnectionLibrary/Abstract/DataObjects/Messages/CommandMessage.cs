using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.DataObjects.Messages
{
    public class CommandMessage : IMessage
    {
        //TODO парс и валидация команды
        //TODO добавить информацию о способе связи в описание устройств
        public string Message { get; set; }
        public CommandMessage(string message, string deviceCode, DateTime timeMarker, string targetDeviceCode)
        {
            TimeMarker = timeMarker;
            TargetDeviceCode = targetDeviceCode;
            Message = message;
            DeviceCode = deviceCode;
            MessageType = MessageType.Command;
        }
        public string TargetDeviceCode { get; set; }
        public DateTime TimeMarker { get; set; }
        public MessageType MessageType { get; set; }
        public string DeviceCode { get; set; }
    }
}