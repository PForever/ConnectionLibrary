namespace LogSingleton
{
    public static class Logging
    {
        public static bool IsEmpty { get; private set; }
        private const string Line = "===================================================";
        public static ILogger Log { get; private set; }
        public static void Initialize(ILogger logger)
        {
            if (!IsEmpty) return;
            Log = logger;
            IsEmpty = false;
            Log.Info(Line + "Logger Started" + Line);
        }

        static Logging()
        {
            IsEmpty = true;
            Log = new EmptyLogger();
        }
    }
}