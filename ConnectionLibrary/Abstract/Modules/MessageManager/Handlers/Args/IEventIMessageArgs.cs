using ConnectionLibrary.Abstract.DataObjects.Containers;

namespace ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args
{
    public interface IEventIMessageArgs
    {
        IMessage Message { get; }
    }
}