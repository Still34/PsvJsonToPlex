using Microsoft.Extensions.Logging;

namespace JsonToEpm
{
    public class LogService
    {
        private readonly ILogger _logger;

        public LogService(ILoggerFactory factory)
        {
            _logger = factory.AddConsole().CreateLogger("Main");
        }

        public void Log(LogLevel logLevel, string info)
        {
            _logger.Log(logLevel, 0, info, null, (s, exception) => s?.ToString());
        }

        public void Log(Result result)
        {
            var logLevel = result.IsSuccess ? LogLevel.Information : LogLevel.Error;
            _logger.Log(logLevel, 0, result.Details, result.Exception, (s, exception) => $"{s} {exception}");
        }
    }
}