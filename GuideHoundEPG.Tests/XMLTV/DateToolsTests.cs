using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.XmlTv;

namespace GuideHoundEPG.Tests.XMLTV {
    [TestClass]
    public class DateToolsTests {
        
        [TestMethod, Ignore]
        public void CanReadLocalTime() {

            DateTime actualDate = DateTools.FormatDate("20100426060000 +1200");
            Assert.AreEqual(DateTimeKind.Utc, actualDate.Kind);                                            
            Assert.AreEqual(18, actualDate.Hour);
            Assert.AreEqual(0, actualDate.Minute);
            Assert.AreEqual(2010, actualDate.Year);
            Assert.AreEqual(4, actualDate.Month);
            Assert.AreEqual(25, actualDate.Day);
            

        }
    }
}
