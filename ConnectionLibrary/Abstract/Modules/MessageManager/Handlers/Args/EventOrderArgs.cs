using System;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public class EventOrderArgs : EventArgs, IEventIMessageArgs
    {
        public Order Order { get; }
        public EventOrderArgs(Order order)
        {
            Order = order;
        }

        public IMessage Message => Order;
    }
}