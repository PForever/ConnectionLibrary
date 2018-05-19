using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.DataObjects.DeviceInfo
{
    public interface IDevice : ICloneable
    {
        string Code { get; }
        string MacAddress { get; set; }
        string Name { get; }
        Properties Info { get; }
    }
}