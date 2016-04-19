using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Logging;

namespace UnitTest.Core.Logging {
    
    [TestClass]
    public class When_creating_a_logger {
        
        [TestMethod]
        public void it_should_return_a_valid_logger_instance() {

            ILogger logger = Logger.GetLogger();
            Assert.IsNotNull(logger);

        }

    }

    /// <summary>
    /// TODO: Need to figure out how to verify expectations for this class.
    /// </summary>
    [TestClass]
    public class When_logging {
        
        [TestMethod]
        public void logger_should_log_a_simple_message_with_no_parameters() {
            ILogger logger = Logger.GetLogger();
            logger.Log(LogLevel.Debug, "This is a message");
        }

        [TestMethod]
        public void logger_should_log_a_message_with_an_exception() {
            ILogger logger = Logger.GetLogger();

            Exception e = new Exception("foo");
            logger.Log(LogLevel.Debug, "This is a message", e);
        }

        [TestMethod]
        public void logger_should_log_a_formatted_message() {
            ILogger logger = Logger.GetLogger();

            logger.Log(LogLevel.Debug, "This is a message {0} and {1}", "Test", "Test2");
        }

        [TestMethod]
        public void logger_should_log_a_formatted_message_with_an_exception() {
            ILogger logger = Logger.GetLogger();

            Exception e = new Exception("foo");
            logger.Log(LogLevel.Debug, e, "This is a message {0} and {1}", "Test", "Test2");
        }

    }



}
