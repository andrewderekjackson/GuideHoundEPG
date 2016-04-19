using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests.MxfModel {
    [TestClass]
    public class ChannelTests {

        [TestMethod]
        public void ChannelUidFormatMustBeCorrect() {

            Channel c1 = new Channel();
            c1.Lineup = new Lineup() { Id = "l1", Name = "lineup1" };
            c1.Id = 1;

            string actual = c1.Uid;

            Assert.IsTrue(actual == "!Channel!lineup1!1");

        }

        [TestMethod]
        public void ChannelShouldBeAddedToProvider() {

            Provider p = new Provider();
            Assert.IsTrue(p.Channels.Count == 0);
            Assert.IsTrue(p.Affiliates.Count == 0);
            Assert.IsTrue(p.Services.Count == 0);

            Affiliate a = new Affiliate();
            Service s = new Service();
            s.Affiliate = a;

            Channel c1 = new Channel();
            c1.Id = 0;
            c1.Service = s;
            c1.Lineup = Lineup.Primary;
            c1.Number = 1;

            Channel c2 = new Channel();
            c2.Id = 1;
            c2.Service = s;
            c2.Lineup = Lineup.Primary;
            c2.Number = 2;

            p.AddChannel(c1);
            
            Assert.IsTrue(p.Channels.Count == 1);
            Assert.IsTrue(p.Affiliates.Count == 1);
            Assert.IsTrue(p.Services.Count == 1);

            p.AddChannel(c2);

            Assert.IsTrue(p.Channels.Count == 2);
            Assert.IsTrue(p.Affiliates.Count == 1);
            Assert.IsTrue(p.Services.Count == 1);

        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AChannelRequiresAService() {

            Provider p = new Provider();

            Channel c = new Channel();
            c.Lineup = Lineup.Primary;

            p.AddChannel(c);

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AChannelRequiresALineup() {

            Provider p = new Provider();
            
            Affiliate a = new Affiliate();
            Service s = new Service();
            s.Affiliate = a;

            Channel c = new Channel();

            p.AddChannel(c);

        }
    }
}
