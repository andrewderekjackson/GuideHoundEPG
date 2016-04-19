using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Logging {
    
    public interface ILogger {
        void Log(LogLevel level, string message, params object[] args);
        void Log(LogLevel level, Exception exception, String message, params object[] args);
    }
}
