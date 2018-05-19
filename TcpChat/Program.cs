using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConnectionLibrary;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Devices;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Delegates;
using ConnectionLibrary.Modules.DbManager;
using ConnectionLibrary.Modules.MessageManager;
using Newtonsoft.Json;
using Log4NetProj;
using LogSingleton;

namespace TcpChat
{
    class Program
    {
        private const string MulticastHostint = "244.011.061.973";
        private const string Host = "192.168.1.33";
        private const int UdpPort = 8083;
        private const int TcpPort = 8082;


        private static string _protocol = "UDP";
        private static string _myCode = "krakadile";
                
        private static ServerManager _server;
        private static SenderManager _sender;
        private static DbManager _dbManager;
        static void Main(string[] args)
        {
            var log = new Logger();
            Logging.Initialize(log);

            _server = new ServerManager(MulticastHostint, UdpPort, TcpPort);
            _sender = new SenderManager(MulticastHostint, TcpPort, UdpPort);
            
        }
    }
}
