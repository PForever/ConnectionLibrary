using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Parse
{
    public interface IParsing
    {
        Request Request { get; }
        Telemetry Telemetry { get; }
        MessageType MessageType { get; }
        string ErrInfo { get; }
    }
}