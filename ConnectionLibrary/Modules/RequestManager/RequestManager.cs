using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Abstract.Modules.MessageManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.Requester;

namespace ConnectionLibrary.Modules.RequestManager
{
    public class RequestManager : ARequestManager, ICodeble
    {
        private readonly AActulizer _actulizer;
        private readonly AMessageManager _messageManager;
        public RequestManager(AActulizer actulizer, AMessageManager messageManager, string myCode)
        {
            _actulizer = actulizer;
            _messageManager = messageManager;
            MyCode = myCode;
            _messageManager.OrderReceived += (o, args) => OnOrder(args);
        }
        public override void OnRequest(EventRequestArgs eventRequest)
        {
        }

        public override void OnOrder(EventOrderArgs eventOrder)
        {
            var devices = _actulizer.GetDevices(propNames: eventOrder.Order.GetPropertiesValues);
            //TODO нужно грегировать получения и выдачу данных

            _actulizer.GetData(eventOrder.Order.GetPropertiesValues, out IDictionary<string, IList<Telemetry>> telemetries);
            Request request = new Request(MyCode, "", DateTime.Now, eventOrder.Order.DeviceCode, telemetries, devices);
            _messageManager.OnRequest(this, new EventRequestArgs(request));
        }

        protected override Dictionary<string, bool> ContainsDb(List<string> properties)
        {
            throw new System.NotImplementedException();
        }

        protected override bool ContainsDb(string property)
        {
            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, bool> ContainsWeb(List<string> properies)
        {
            throw new System.NotImplementedException();
        }

        protected override bool ContainsWeb(string property)
        {
            throw new System.NotImplementedException();
        }

        protected override PropertiesValues GetDataDb(IList<string> properties)
        {
            throw new System.NotImplementedException();
        }

        protected override PropertiesValues GetDataWeb(IList<string> properties)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdataRequest(IList<Request> requests)
        {
            throw new System.NotImplementedException();
        }

        public string MyCode { get; set; }
    }
}