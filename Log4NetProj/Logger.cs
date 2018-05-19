using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using log4net;
using log4net.Config;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using LogSingleton;

namespace Log4NetProj
{
    public class Logger : ILogger
    {
        public ILog Log { get; }

        public Logger()
        {
            var ass = Assembly.GetCallingAssembly();
            ILoggerRepository rep = LogManager.GetRepository(ass);

            var r = new FileInfo(@"C:\Users\ASUS\source\repos\RaspbIoT\RaspbIoT\Config\Log4Net.config");
            var rt = r.Exists;

            var c = Directory.GetCurrentDirectory();
            var path = c + @"\..\Config\Log4Net.config";

            var t1 = File.Exists(@"C:\Users\ASUS\source\repos\RaspbIoT\RaspbIoT\bin\x64\Debug\Config\Log4Net.config");
            var t2 = File.Exists(@".\Config\Log4Net.config");
            XmlConfigurator.Configure(rep, new FileInfo(/*path*/@".\Config\Log4Net.config"));

            Log = LogManager.GetLogger(ass, "LOGGER");
            //var t2 = File.Exists(@".\Log4Net.config");
        }
        //public static void InitLogger()
        //{

        //}
        public void Debug(object message) => Log.Debug(message);
        public void Debug(object message, Exception exception) => Log.Debug(message, exception);
        public void DebugFormat(string format, params object[] args) => Log.DebugFormat(format, args);
        public void Error(object message) => Log.Error(message);
        public void Error(object message, Exception exception) => Log.Error(message, exception);
        public void ErrorFormat(string format, params object[] args) => Log.ErrorFormat(format, args);
        public void Fatal(object message) => Log.Fatal(message);
        public void Fatal(object message, Exception exception) => Log.Fatal(message, exception);
        public void FatalFormat(string format, params object[] args) => Log.FatalFormat(format, args);
        public void Info(object message) => Log.Info(message);
        public void Info(object message, Exception exception) => Log.Info(message, exception);
        public void InfoFormat(string format, params object[] args) => Log.InfoFormat(format, args);
        public void Warn(object message) => Log.Warn(message);
        public void Warn(object message, Exception exception) => Log.Warn(message, exception);
        public void WarnFormat(string format, params object[] args) => Log.WarnFormat(format, args);
    }
}