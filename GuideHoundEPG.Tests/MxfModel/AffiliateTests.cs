using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests.MxfModel {
    [TestClass]
    public class AffiliateTests {

        [TestMethod]
        public void AffiliateShouldBeAddedToProvider() {

            Provider p = new Provider();

            Affiliate a1 = new Affiliate();
            a1.Name = "foo";

            p.AddAffiliate(a1);
            Assert.IsTrue(p.Affiliates.Count == 1);

        }

        [TestMethod]
        public void AffiliateUidFormatMustBeCorrect() {

            Affiliate a1 = new Affiliate();
            a1.Name = "foo";

            Assert.IsTrue(a1.Uid == "!Affiliate!foo");
        
        }

        [TestMethod]
        public void AffiliateGuideImageShouldBeAddedToProviderCollection() {

            Provider p = new Provider();

            // adding without a guide image
            Affiliate a1 = new Affiliate();
            a1.Name = "foo";

            p.AddAffiliate(a1);
            Assert.IsTrue(p.GuideImages.Count==0);
            Assert.IsTrue(p.Affiliates.Count == 1);

            // adding with a guide image
            Affiliate a2 = new Affiliate();
            a2.Name = "foo2";
            a2.LogoImage = new GuideImage() {
                ImageUrl = "url"
            };
            Assert.IsTrue(p.GuideImages.Count == 0);
            Assert.IsTrue(p.Affiliates.Count == 1);
            p.AddAffiliate(a2);
            Assert.IsTrue(p.GuideImages.Count == 1);
            Assert.IsTrue(p.Affiliates.Count == 2);
            

            // updating guide image
            a2.LogoImage = new GuideImage() {
                ImageUrl = "url2"
            };
            p.AddAffiliate(a1);
            Assert.IsTrue(p.GuideImages.Count == 2);
            Assert.IsTrue(p.Affiliates.Count == 2);

        }

    }
}
