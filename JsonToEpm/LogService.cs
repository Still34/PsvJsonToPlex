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
            _logger = factory
                .AddConsole(options.IsDebug ? LogLevel.Debug : LogLevel.Information)
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