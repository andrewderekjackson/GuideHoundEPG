using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests.MxfModel {
    [TestClass]
    public class ProgramTests {

        [TestMethod]
        public void ProgramUidFormatMustBeCorrect() {

            ProgramInfo p1 = new ProgramInfo();
            p1.Id = "program1";

            string actual = p1.Uid;

            Assert.IsTrue(actual == "!Program!program1");

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ProgramMustHaveADuration() {

            Provider p = new Provider();
            Assert.IsTrue(p.Programs.Count == 0);

            ProgramInfo p1 = new ProgramInfo();
            p1.Id = "program1";

            p.AddProgram(p1);

            Assert.IsTrue(p.Programs.Count==0);

        }

        [TestMethod]
        public void ProgramShouldBeAddedToProvider() {
            Provider p = new Provider();
            Assert.IsTrue(p.Programs.Count == 0);

            ProgramInfo pr = new ProgramInfo();
            pr.Id = "foo";
            pr.DurationInMinutes = 10;
            p.AddProgram(pr);
            Assert.IsTrue(p.Programs.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ProgramMustHaveAnId() {
            Provider p = new Provider();
            p.Id = String.Empty;
            Assert.IsTrue(p.Programs.Count == 0);

            ProgramInfo pr = new ProgramInfo();
            p.AddProgram(pr);

            Assert.IsTrue(p.Programs.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ProgramMustHaveAnUniqueId() {
            Provider p = new Provider();
            Assert.IsTrue(p.Programs.Count == 0);

            ProgramInfo pr = new ProgramInfo();
            pr.Id = "foo";
            p.AddProgram(pr);
            Assert.IsTrue(p.Programs.Count == 1);

            ProgramInfo pr2 = new ProgramInfo();
            pr2.Id = "foo";
            p.AddProgram(pr2);

        }

        [TestMethod]
        public void IfAProgramHasASeriesItShouldBeAddedToProvider() {

            SeriesInfo s1 = new SeriesInfo();
            s1.Description = "series";

            ProgramInfo p1 = new ProgramInfo();
            p1.Id = "program1";
            p1.Series = s1;
            p1.DurationInMinutes = 10;

            Provider p = new Provider();
            
            Assert.IsTrue(p.Programs.Count == 0);
            Assert.IsTrue(p.Series.Count == 0);

            p.AddProgram(p1);

            Assert.IsTrue(p.Programs.Count == 1);
            Assert.IsTrue(p.Series.Count == 1);

            Assert.IsTrue(p.Series.Contains(s1));

        }
    }
}
