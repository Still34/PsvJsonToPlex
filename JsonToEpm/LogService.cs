using Microsoft.Extensions.Logging;
using PsvJsonToPlex.Model;

namespace PsvJsonToPlex
{
    public class LogService
    {
        private readonly ILogger _logger;
        private readonly Options _options;

        public LogService(ILoggerFactory factory, Options options)
        {
            _options = options;
            _logger = factory.AddConsole().CreateLogger("Main");
        }

        public void Log(LogLevel logLevel, string info)
        {
            if (!_options.IsDebug && logLevel == LogLevel.Debug) return;
            _logger.Log(logLevel, 0, info, null, (s, exception) => s?.ToString());
        }

        public void Log(Result result)
        {
            var logLevel = result.IsSuccess ? LogLevel.Information : LogLevel.Error;
            _logger.Log(logLevel, 0, result.Details, result.Exception, (s, exception) => $"{s} {exception}");
        }
    }
}