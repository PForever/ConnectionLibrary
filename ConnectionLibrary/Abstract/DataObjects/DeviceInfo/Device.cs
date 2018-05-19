using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.DataObjects.DeviceInfo
{
    public class Device : IDevice
    {
        public object Clone()
        {
            //TODO Code не должен копироваться... наверное
            return new Device(Code, MacAddress, Name, Info.Clone() as Properties);
        }

        public string Code { get; set; }
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public Properties Info { get; set; }
        public Device() { }
        public Device(IDevice device) : this(device.Code, device.MacAddress, device.Name, device.Info.Clone() as Properties) {}
        public Device(string code, string macAddress, string name, Properties info)
        {
            Code = code;
            MacAddress = macAddress;
            Name = name;
            Info = info;
        }
    }
}