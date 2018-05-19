using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;

namespace ConnectionLibrary.Abstract.Modules.Register
{
    public abstract class ARegister
    {
        public abstract bool Contains(string code);
        public abstract bool Contains(IDevice code);
        public abstract int AddDevice(IDevice device);
        public abstract int RemoveDevice(string code);
        public abstract int RemoveDevice(IDevice device);
        public abstract IDevice GetDeviceByCode(string code);
    }
}