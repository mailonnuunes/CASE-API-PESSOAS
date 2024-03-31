
namespace PeopleApi.Infrastructure.Logging
{
    public class CustomLogger : ILogger
    {
        private readonly string loggerName;
        private readonly CustomLoggerProviderConfiguration _loggerConfiguration;
        public static bool FileLog { get; set; } = true;
        public CustomLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfiguration)
        {
            this.loggerName = loggerName;
            _loggerConfiguration = loggerConfiguration;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string mesagge = $"Log de inicializacao {logLevel}: {eventId} - {formatter(state, exception)}";

            if (FileLog)
            {
                WriteTextInFile(mesagge);
            }
            else
            {
                Console.WriteLine(mesagge);
            }
        }

        private void WriteTextInFile(string mesagge)
        {
            string filePath = Environment.CurrentDirectory + $@"\LOG-{DateTime.Now:yyyy-MM-dd}.txt";

            if (!File.Exists(filePath)){
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.Create(filePath).Dispose();
            }
            using StreamWriter sw = new(filePath,true);
            sw.WriteLine(mesagge);
            sw.Close();
        }
    }
}
