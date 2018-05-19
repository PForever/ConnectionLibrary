using System.Threading.Tasks;

namespace ConnectionLibrary.Abstract.Server
{
    public interface ISender<in T, S>
    {
        int Port { get; set; }
        void SendAsync(string host, T data);
    }
}