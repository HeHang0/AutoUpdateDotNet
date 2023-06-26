using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdate.Core
{
    static class Logger
    {
        public static ILogger logger = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter(null, LogLevel.Debug)
                .AddConsole();
        }).CreateLogger("AutoUpdate.Core");
        public static ILogger Log => logger;
    }
}
