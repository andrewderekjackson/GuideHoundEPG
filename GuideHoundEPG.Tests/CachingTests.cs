using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests {
    [TestClass]
    public class CachingTests {
        
        [TestMethod]
        public void BasicCacheTest()  {

            CacheProvider<String, SeriesInfo> seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);

            string expectedSeriesKey = Guid.NewGuid().ToString();
            SeriesInfo expectedSeries = new SeriesInfo();
            expectedSeries.CacheKey = expectedSeriesKey;
            expectedSeries.Id = "foo";
            expectedSeries.Description = "foo";

            SeriesInfo tempSeries = seriesCache.FindKey(expectedSeriesKey);
            Assert.IsNull(tempSeries);

            seriesCache.Add(expectedSeries);
            SeriesInfo actualSeries = seriesCache.FindKey(expectedSeriesKey);

            Assert.IsNotNull(actualSeries);
            Assert.AreEqual(expectedSeries.Uid, actualSeries.Uid);
            Assert.AreEqual("foo", actualSeries.Description);
            Assert.AreSame(expectedSeries, actualSeries);

        }
    }
}
