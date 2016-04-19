using System.IO;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;

namespace GuideHoundEPG.Common.Model.XmlTv {
    public static class StreamTools {

        public static Stream GetStreamFromString(string stringValue) {
            MemoryStream memStream = new MemoryStream();
            byte[] data = Encoding.Unicode.GetBytes(stringValue);
            memStream.Write(data, 0, data.Length);
            memStream.Position = 0;
            return memStream;
        }

    }
}