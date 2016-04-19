using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Tests.Properties;
using System.Reflection;
using System.IO;
using GuideHoundEPG.Common.Model.XmlTv;

namespace GuideHoundEPG.Tests.XMLTV {
    
    // [TestClass]
    public class ManualParseBasedXmlTvReaderTests : XmlTvBaseTests {

        public override IXmlTvReader XmlTvReader {
            get { return new ManualParseXmlTvReader(); }
        }

    }
}
