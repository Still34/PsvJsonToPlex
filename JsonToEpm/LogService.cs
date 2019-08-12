using Microsoft.Extensions.Logging;

namespace PsvJsonToPlex
{
    public class LogService
    {
        private readonly ILogger _logger;

        public LogService(ILoggerFactory factory)
        {
            _logger = factory
                .CreateLogger("Main");
        }

        public void Log(LogLevel logLevel, string info)
        {
            _logger.Log(logLevel, 0, info, null, (s, _) => s);
        }

        public void Log(Result result)
        {
            var logLevel = result.IsSuccess ? LogLevel.Information : LogLevel.Error;
            _logger.Log(logLevel, 0, result.Details, result.Exception, (s, ex) => ex?.ToString() ?? s);
        }
    }
}