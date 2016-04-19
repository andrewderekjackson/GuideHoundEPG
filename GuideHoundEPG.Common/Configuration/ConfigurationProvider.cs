using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Xml;
using Core.Logging;
using log4net;
using GuideHoundEPG.Common.Model.Configuration;
using System.Xml.Serialization;
using System.Reflection;

namespace GuideHoundEPG.Common.Configuration {
    public class ConfigurationProvider {
        private static readonly ILogger logger = Logger.GetLogger();
        private string defaultFilename;
        
        public ConfigurationProvider() {
            defaultFilename = EnvironmentInfo.ConfigFileName;
        }

        public EPGDownloaderConfig LoadConfigurationFile(string configFilename) {
            EPGDownloaderConfig obj = null;
            try {
                using (FileStream reader = new FileStream(configFilename, FileMode.Open)) {
                    XmlSerializer ser = new XmlSerializer(typeof(EPGDownloaderConfig));
                    obj = (EPGDownloaderConfig)ser.Deserialize(reader);
                    reader.Close();
                }
            } catch (Exception) {
                // log.Log(LogLevel.Error, "Cannot read configuration file: "+fileName, ex);
            }

            return obj;
        }

        public EPGDownloaderConfig LoadConfigurationFile() {
            return LoadConfigurationFile(defaultFilename);
        }
        
        public EPGDownloaderConfig CreateDefault() {
            logger.Log(LogLevel.Info, "Loading default configuration file.");
            EPGDownloaderConfig defaultConfig = null;
            if (File.Exists(EnvironmentInfo.DefaultConfigFileName)) {
                try {
                    defaultConfig = LoadConfigurationFile(EnvironmentInfo.DefaultConfigFileName);
                } catch (Exception ex) {
                    logger.Log(LogLevel.Debug, "Unable to load default configuration file - creating an empty file.", ex);
                }
            }
            if (defaultConfig != null) {
                return defaultConfig;
            } else {
                return new EPGDownloaderConfig();
            }
        }

        public void SaveConfigurationFile(EPGDownloaderConfig configSection) {

            FileStream writer = new FileStream(defaultFilename, FileMode.Create);
            XmlSerializer ser = new XmlSerializer(typeof(EPGDownloaderConfig));
            ser.Serialize(writer, configSection);
            writer.Close();

        }
    }
}