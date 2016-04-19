using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using GuideHoundEPG.Common;
using GuideHoundEPG.Common.Configuration;
using GuideHoundEPG.Common.Model.Configuration;
using GuideHoundEPG.Common.Model.MXF;
using GuideHoundEPG.Common.Model.XmlTv;
using Channel = GuideHoundEPG.Common.Model.MXF.Channel;
using Provider = GuideHoundEPG.Common.Model.MXF.Provider;
using GuideHoundEPG.Common.Model;
using Core.Logging;
using GuideHoundEPG.Properties;

namespace GuideHoundEPG
{
    class Program {

        private static readonly ILogger logger = Logger.GetLogger();
        
        static void Main(string[] args) {

            PrintHeader();

            logger.Log(LogLevel.Info, "Loading configuration...");

            EPGDownloaderConfig config = new ConfigurationProvider().LoadConfigurationFile();
            if (config == null) {
                logger.Log(LogLevel.Error, "Unable to read configuration file. Please use the configuration program to create a configuration file.");
                ExitGracefully();
            }

            logger.Log(LogLevel.Info, "Starting import...");
            ImportRunner importRunner = new ImportRunner(config, logger);
            try {
                importRunner.Import();
            } catch (Exception gex) {
                logger.Log(LogLevel.Error, "An unknown exception was encountered while running the importer. Please have a look at the log file for more information.", gex);
                ExitGracefully();
            }

            ExitGracefully();
        }

        private static void PrintHeader() {
            string appVersion = AppVersion.ShortAppVersion;
            string buildDate = AppVersion.BuildDate.ToString("MMM yyyy");

            logger.Log(LogLevel.Info, "------------------------------------------------------------------");
            logger.Log(LogLevel.Info, String.Format(" GuideHound EPG                                    Version {0}", appVersion));
            logger.Log(LogLevel.Info, String.Format(" Copyright (c) 2010 Andrew Jackson                      {0}", buildDate));
            logger.Log(LogLevel.Info, "------------------------------------------------------------------");
            logger.Log(LogLevel.Info, "");

        }
        
        private static void ExitGracefully() {

            // wait 5 seconds (with message)
            for (int i = 5; i > 0; i--) {
                Console.Write(String.Format("\rClosing in {0} seconds...", i));
                Thread.Sleep(1000);
            }

#if DEBUG
            Console.ReadLine();
#endif

            // close application
            Environment.Exit(0);
        }
         
    }

    
}

