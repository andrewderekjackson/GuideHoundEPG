using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests.MxfModel {
    [TestClass]
    public class ServiceTests {
        
        [TestMethod]
        public void ServiceShouldBeAddedToProvider() {

            Provider p = new Provider();

            Service s1 = new Service();
            s1.Name = "tesT";
            s1.Affiliate = new Affiliate() { Name = "affiliate"};

            p.AddService(s1);
            Assert.IsTrue(p.Services.Count == 1);
            Assert.IsTrue(p.Affiliates.Count == 1);
            Assert.IsFalse(String.IsNullOrEmpty(s1.Id));
     
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AServiceRequiresAnAffiliate() {

            Provider p = new Provider();

            Service s = new Service();

            p.AddService(s);

        }

    }
}
