using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.XmlTv;

namespace GuideHoundEPG.Tests.XMLTV {
    [TestClass, Ignore]
    public class FormatDateTests {
        
        [TestMethod]
        public void CanFormatAPropertyFormedLocalTimeWithOffset() {

            DateTime date = DateTools.FormatDate("20100510002000 +1200");
            Assert.AreEqual(new DateTime(2010, 05, 10, 12, 20, 00), date);

        }

        [TestMethod]
        public void CanFormatAPropertyFormedLocalTimeWithOffset2() {

            DateTime date = DateTools.FormatDate("20100510002000 +0600");
            Assert.AreEqual(new DateTime(2010, 05, 10, 6, 20, 00), date);

        }

        [TestMethod]
        public void CanFormatAPropertyFormedLocalTimeWithOffset3() {

            DateTime date = DateTools.FormatDate("20100510002000 -0600");
            Assert.AreEqual(new DateTime(2010, 05, 9, 18, 20, 00), date);

        }

        [TestMethod]
        public void CanFormatAPropertyFormedLocalTimeWithOffset4() {

            DateTime date = DateTools.FormatDate("20100510002000 -0630");
            Assert.AreEqual(new DateTime(2010, 05, 9, 17, 50, 00), date);

        }

        [TestMethod]
        public void CanFormatATimeWithoutOffset() {

            DateTime date = DateTools.FormatDate("20100510002000");
            Assert.AreEqual(new DateTime(2010, 05, 10, 00, 20, 00), date);

        }
    }
}
