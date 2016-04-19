using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.TvdbLookup.Model;

namespace Services.TvdbLookup.Tests
{
    /// <summary>
    /// Summary description for TvdbServiceTests
    /// </summary>
    [TestClass]
    public class TvdbServiceTests
    {

        public TvdbService CreateAndConfigureService() {
            TvdbServiceConfiguration expectedConfiguration = GetConfiguration();
            TvdbService service = new TvdbService(expectedConfiguration);
            Assert.IsNotNull(service, "Service cannot be null");
            Assert.IsTrue(service.Configuration.ApiKey==expectedConfiguration.ApiKey);
            Assert.IsTrue(service.Configuration.CacheFolder==expectedConfiguration.CacheFolder);
            return service;
        }

        

        private TvdbServiceConfiguration GetConfiguration() {
            return new TvdbServiceConfiguration() {ApiKey = "CE72338B4ECE6EDA", CacheFolder = "cache"};
        }


        [TestMethod]
        public void CanDownloadBasicSeriesInfo() {
            
            TvdbService service = CreateAndConfigureService();
            List<TvdbSeriesBase> results = service.Search("Chuck");
            Assert.IsTrue(results!=null, "results is null");

        }
    }
}
