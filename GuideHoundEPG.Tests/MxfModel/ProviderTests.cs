using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests.MxfModel {
    [TestClass]
    public class ProviderTests {

        [TestMethod]
        public void MxfFileMustRegisterProvider() {

            MxfFile mxfFile = new MxfFile();

            Provider p1 = new Provider();
            p1.AddLineup(Lineup.Primary);

            Assert.AreEqual(p1.Id, null);

            mxfFile.AddProvider(p1);

            Assert.IsFalse(String.IsNullOrEmpty(p1.Id));

        }

        [TestMethod]
        public void ProviderRequiresAPrimaryLineup() {

            MxfFile mxfFile = new MxfFile();

            Provider p1 = new Provider();
            Assert.AreEqual(p1.Id, null);

            Assert.IsFalse(p1.Validate());

        }

        [TestMethod]
        public void RegisteredProvidersMustHaveUniqueIds() {

            MxfFile mxfFile = new MxfFile();

            Provider p1 = new Provider();
            p1.AddLineup(Lineup.Primary);

            Assert.AreEqual(p1.Id, null);

            Provider p2 = new Provider();
            p2.AddLineup(Lineup.Primary);
            Assert.AreEqual(p2.Id, null);

            mxfFile.AddProvider(p1);
            mxfFile.AddProvider(p2);

            Assert.IsFalse(String.IsNullOrEmpty(p1.Id));
            Assert.IsFalse(String.IsNullOrEmpty(p2.Id));

            Assert.IsTrue(p1.Id!=p2.Id);
            Assert.IsTrue(mxfFile.Providers.Count == 2);
            Assert.IsTrue(mxfFile.Providers.Where(e=> e.Id == p1.Id).FirstOrDefault() != null);
            Assert.IsTrue(mxfFile.Providers.Where(e=> e.Id == p2.Id).FirstOrDefault() != null);

        }

        [TestMethod]
        [ExpectedException(typeof(MxfValidationException))]
        public void ProvidersCannotBeRegisteredMoreThanOnce() {

            MxfFile mxfFile = new MxfFile();

            Provider p1 = new Provider();
            p1.AddLineup(Lineup.Primary);
            Assert.AreEqual(p1.Id, null);

            mxfFile.AddProvider(p1);
            mxfFile.AddProvider(p1);

        }

        [TestMethod]
        [ExpectedException(typeof(MxfValidationException))]
        public void InvalidProviderThrowsAnExceptionWhenRegistered() {

            MxfFile mxfFile = new MxfFile();

            Provider p1 = new Provider();
            mxfFile.AddProvider(p1);

        }

        [TestMethod]
        public void SettingAChannelLogo() {

            Provider p1 = new Provider();

            Service s1 = new Service();
            s1.Affiliate = new Affiliate() { Name = "a1"};
            p1.AddService(s1);
            
            Assert.IsTrue(p1.GuideImages.Count == 0);
            Assert.IsTrue(s1.LogoImage == null);

            GuideImage g1 = new GuideImage("bla");

            s1.LogoImage = g1;
            p1.BuildProvider();

            Assert.AreEqual(g1, s1.LogoImage);
            Assert.IsTrue(p1.GuideImages.Count == 1);
            Assert.IsTrue(p1.GuideImages[0].ImageUrl == "bla");

            g1.ImageUrl = "foo";
            Assert.IsTrue(p1.GuideImages[0].ImageUrl == "foo");

            g1.ImageUrl = "bar";
            p1.BuildProvider();

            Assert.IsTrue(p1.GuideImages[0].ImageUrl == "bar");
            Assert.AreEqual(g1, s1.LogoImage);

            GuideImage g2 = new GuideImage("fuzz");
            s1.LogoImage = g2;
            p1.BuildProvider();

            Assert.AreEqual(g2, s1.LogoImage);

        }



    }
}

