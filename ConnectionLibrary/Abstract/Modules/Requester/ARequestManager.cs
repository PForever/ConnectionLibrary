using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibrary.Abstract.Modules.Requester
{
    public abstract class ARequestManager
    {
        public abstract void OnRequest(EventRequestArgs eventRequest);
        public abstract void OnOrder(EventOrderArgs eventRequest);
        protected abstract Dictionary<string, bool> ContainsDb(List<string> properties);
        protected abstract bool ContainsDb(string property);
        protected abstract Dictionary<string, bool> ContainsWeb(List<string> properies);
        protected abstract bool ContainsWeb(string property);
        protected abstract PropertiesValues GetDataDb(IList<string> properties);
        protected abstract PropertiesValues GetDataWeb(IList<string> properties);
        protected abstract void UpdataRequest(IList<Request> requests);
    }
}