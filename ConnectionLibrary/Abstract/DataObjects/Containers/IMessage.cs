using System;

namespace ConnectionLibrary.Abstract.DataObjects.Containers
{
    public interface IMessage
    {
        DateTime TimeMarker { get; }
        MessageType MessageType { get; }
        string DeviceCode { get; }
        string TargetDeviceCode { get; }
    }
}