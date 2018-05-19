using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers
{
    public class AddressBook : ConcurrentDictionary<string, Addresses>
    {
        public AddressBook() : base()
        {
        }
        public AddressBook(IDictionary<string, Addresses> dictionary) : base(dictionary)
        {
        }
    }
}