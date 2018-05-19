using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using Newtonsoft.Json;

namespace ConnectionLibrary.Modules.MessageManager.Serialize 
{
    public static class Deserializing
    {
        private static readonly JsonSerializerSettings JsonSet = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        private static IMessage Deserialize<T>(string data) where T : IMessage => JsonConvert.DeserializeObject<T>(data/*, JsonSet*/);
        public static IMessage GetMessage(string data, out MessageType type)
        {
            type = JsonHelp.MessageTypeSeacher(ref data);
            switch (type)
            {
                case MessageType.Command:
                    return Deserialize<CommandMessage>(data);
                case MessageType.Telemetry:
                    return Deserialize<Telemetry>(data);
                case MessageType.Connect:
                    return Deserialize<ConnectMessage>(data);
                case MessageType.Err:
                    return Deserialize<ErrorMessage>(data);
                case MessageType.Order:
                    return Deserialize<Order>(data);
                case MessageType.Request:
                    return Deserialize<Request>(data);
                case MessageType.Call:
                    return Deserialize<Call>(data);
                default:
                    return default(IMessage);
            }
        }
    }
}