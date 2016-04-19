using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;

namespace GuideHoundEPG.UI {
    
    public class LogAppender : AppenderSkeleton {

        public event LogAppenderEventHandler OnLoggingEvent;

        protected override void Append(log4net.Core.LoggingEvent loggingEvent) {
            if (OnLoggingEvent != null) {
                OnLoggingEvent(this, new LogAppenderEventArgs() { LoggingEvent = loggingEvent });
            }
        }
    }

    public delegate void LogAppenderEventHandler(object sender, LogAppenderEventArgs e);

    public class LogAppenderEventArgs : EventArgs {
        public log4net.Core.LoggingEvent LoggingEvent { get; set; }
    }
}
