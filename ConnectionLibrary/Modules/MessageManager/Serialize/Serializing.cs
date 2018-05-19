using ConnectionLibrary.Abstract.DataObjects.Messages;
using Newtonsoft.Json;

namespace ConnectionLibrary.Modules.MessageManager.Serialize
{
    public static class Serializing
    {
        #region Serialize Implementation
        private static readonly JsonSerializerSettings JsonSet = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        private static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value/*, JsonSet*/);
        }

        #endregion

        public static string GetString(CommandMessage commandMessage) => Serialize(commandMessage);
        public static string GetString(ErrorMessage err) => Serialize(err);
        public static string GetString(Order order) => Serialize(order);
        public static string GetString(ConnectMessage connectMessage) => Serialize(connectMessage);
        public static string GetString(Request request) => Serialize(request);
        public static string GetString(Telemetry telemetry) => Serialize(telemetry);
        public static string GetString(Call call) => Serialize(call);
    }
}