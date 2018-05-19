using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public class EventTelemetryArgs : EventArgs, IEventIMessageArgs
    {
        public Telemetry TelemetryInfo { get; }
        public EventTelemetryArgs(Telemetry telemetryInfo)
        {
            TelemetryInfo = telemetryInfo;
        }

        public IMessage Message => TelemetryInfo;
    }
}