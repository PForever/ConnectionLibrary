using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;

namespace ConnectionLibrary.Abstract.Modules.MessageManager
{
    public abstract class AAddressManager : AConnecter
    {
        protected IDb DbManager;

        protected AddressBook AddressBook;
        protected abstract AddressBook GetAddressBookDb();
        public abstract void AddAddress(string code, Addresses addresses);
    }
}