using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace Core.Logging {

    /// <summary>
    /// A logger used to log diagnostic messages.
    /// </summary>
    public class Logger : ILogger {

        internal ILog log;

        /// <summary>
        /// Sets up logging configuration
        /// </summary>
        static Logger() {
            //log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            log4net.Config.XmlConfigurator.Configure();
        }

        internal Logger(ILog internalLogger) {
            this.log = internalLogger;
        }

        /// <summary>
        /// Creates a log writer.
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLogger() {
            MethodBase callingMethod = GetPreviousMethodBase(MethodBase.GetCurrentMethod());
            Type loggerType = typeof (Logger);
            if (callingMethod!=null) {
                loggerType = callingMethod.ReflectedType;
            }
            ILog internalLogger = LogManager.GetLogger(loggerType);

            return new Logger(internalLogger);
        }

        internal ILog InternalLog {
            get {
                return log;
            }
        }

        internal static MethodBase GetPreviousMethodBase(MethodBase currentMethod) {
            try {
                StackTrace sTrace = new System.Diagnostics.StackTrace(true);

                //loop through all the stack frames
                for (Int32 frameCount = 0; frameCount < sTrace.FrameCount; frameCount++)
                {
                    StackFrame sFrame = sTrace.GetFrame(frameCount);
                    MethodBase thisMethod = sFrame.GetMethod();
                    //If the Type in the frame is the type that is being searched
                    if (thisMethod == currentMethod) {
                        if (frameCount + 1 <= sTrace.FrameCount) {
                            StackFrame prevFrame = sTrace.GetFrame(frameCount + 1);
                            MethodBase prevMethod = prevFrame.GetMethod();
                            return prevMethod;
                        }
                        break;
                    }
                }
            } catch {  
                // we tried - just ignore
            }
            return null;
        }

        /// <summary>
        /// Writes a log entry to the current log.
        /// </summary>
        /// <param name="level">The level at which to log.</param>
        /// <param name="message">The message to log.</param>
        public void Log(LogLevel level, string message, params object[] args) {
            Log(level, null, message, null);
        }
       
        /// <summary>
        /// Writes a log entry to the current log.
        /// </summary>
        /// <param name="level">The level at which to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Log(LogLevel level, Exception exception, string message, params object[] args) {

            string formattedMessage;
            if (args == null) {
                formattedMessage = message;
            } else {
                formattedMessage = String.Format(message, args);
            }
            
            switch (level) {
                case LogLevel.Error:
                    log.Error(formattedMessage, exception);
                    break;
                case LogLevel.Info:
                    log.Info(formattedMessage, exception);
                    break;
                case LogLevel.Debug:
                    log.Debug(formattedMessage, exception);
                    break;
                default:
                    log.Debug(formattedMessage, exception);
                    break;
            }
        }
    }
}
