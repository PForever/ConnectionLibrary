using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;

namespace ConnectionLibrary.Abstract.Modules.MessageManager
{
    public abstract class AConnecter
    {
        protected IObjectParser Sender;
        protected IMessageParser Server;
    }
}