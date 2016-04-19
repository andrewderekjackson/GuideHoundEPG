using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Configuration;
using GuideHoundEPG.Common.Model.Configuration;

namespace GuideHoundEPG.Tests {
    [TestClass]
    public class ConfigurationTests {
        
        [TestMethod, Ignore]
        public void SerialisationTest() {

            ConfigurationProvider configProvider = new ConfigurationProvider();
            EPGDownloaderConfig config = configProvider.LoadConfigurationFile();

            int t = config.Sources.Count();


        }

        [TestMethod, Ignore]
        public void SerialisationTest2() {

            EPGDownloaderConfig config = new EPGDownloaderConfig();
            config.Sources.Add(new XmlSource() { SourceName = "Andrew" });

            ConfigurationProvider configProvider = new ConfigurationProvider();
            configProvider.SaveConfigurationFile(config);

        }

        [TestMethod, Ignore]
        public void CanLoadConfigurationFile() {

            EPGDownloaderConfig config = new EPGDownloaderConfig();
            config.Sources.Add(new XmlSource() { SourceName = "Andrew" });

            

        }
    }
}
