using System.Collections;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public struct RemoteHostInfo : IEqualityComparer
    {
        public string Host { get; }
        public string Protocol{ get; }
        public RemoteHostInfo(string host, string protocol)
        {
            Host = host;
            Protocol = protocol;
        }

        public new bool Equals(object x, object y)
        {
            var remX = (RemoteHostInfo) x;
            var remY = (RemoteHostInfo) y;
            return remX.Protocol == remY.Protocol && remX.Host == remY.Host;
        }

        public int GetHashCode(object obj)
        {
            return Protocol.GetHashCode();
        }
    }
}