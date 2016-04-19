using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Logging;
using GuideHoundEPG.Common.Model.Configuration;

namespace GuideHoundEPG.Common.Model.XmlTv {
    
    public class XmlTvData {

        public List<XmlTvProgram> Programs { get; set; }
        public List<XmlTvChannel> Channels { get; set; }
        
        public List<Channel> ChannelConfiguration { get; private set; }

        public XmlTvData() {
            Programs = new List<XmlTvProgram>();
            Channels = new List<XmlTvChannel>();
            ChannelConfiguration = new List<Channel>();
        }

        
        
        public List<String> GetCategories() {
            return null;
        }
    }
}
