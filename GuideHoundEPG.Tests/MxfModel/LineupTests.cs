using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests.MxfModel {
    [TestClass]
    public class LineupTests {
        
        [TestMethod]
        public void PrimaryLineupMustHaveSpecificMxfUId() {

            Lineup l1 = Lineup.Primary;

            Assert.IsTrue(l1.IsPrimaryLineup);
            Assert.IsTrue(l1.Id == "l1");
            Assert.IsTrue(l1.Uid=="!Lineup!l1");
            Assert.IsTrue(l1.PrimaryProvider == "!MCLineup!MainLineup");

        }

        [TestMethod]
        public void LineupUidFormatMustBeCorrect() {

            Lineup a1 = new Lineup();
            a1.Id = "foo";

            Assert.IsTrue(a1.Uid == "!Lineup!foo");

        }
    }
}
