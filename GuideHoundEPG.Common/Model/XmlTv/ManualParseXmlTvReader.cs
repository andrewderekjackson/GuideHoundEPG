using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GuideHoundEPG.Common.Model.XmlTv {
    public class ManualParseXmlTvReader : IXmlTvReader {
        
        /// <summary>
        /// Parse XMLTV file for programs
        /// </summary>
        public List<XmlTvProgram> LoadProgramList(string file) {
            
            // USING XMLTEXTREADER RATHER THAN DESERIALIZATION WITH DTD BECAUSE NOT ALL XMLTV FILES
            // IN NEW ZEALAND HAVE VALID XML AND THEN DON'T DESERIALIZE PROPERLY.

            XmlTextReader xmlReader = new XmlTextReader(file);
            xmlReader.XmlResolver = null;

            List<XmlTvProgram> programList = new List<XmlTvProgram>();
            


            xmlReader.Read(); 

            while (xmlReader.Read()) {
                if (xmlReader.Name == "programme")
                {
                    XmlTvProgram newProg = new XmlTvProgram();
                    newProg.StartTime = DateTools.FormatDate(xmlReader.GetAttribute("start").Trim());
                    newProg.StopTime = DateTools.FormatDate(xmlReader.GetAttribute("stop").Trim());
                    newProg.Channel = xmlReader.GetAttribute("channel").Trim();

                    int depth = xmlReader.Depth;

                    xmlReader.Read();
                    while (xmlReader.Depth > depth) {
                        if (xmlReader.NodeType == XmlNodeType.Element) {
                            switch (xmlReader.Name) {
                                case "title":
                                    newProg.Title = xmlReader.ReadString().Trim();
                                    break;
                                case "desc":
                                    newProg.Description = xmlReader.ReadString().Trim().Replace('"', ' ');
                                    break;
                                case "sub-title":
                                    newProg.SubTitle = xmlReader.ReadString().Trim();
                                    break;
                                case "rating":
                                    newProg.Rating = xmlReader.ReadString().Trim();
                                    break;
                            }
                        }
                        xmlReader.Read(); 
                    }


                    programList.Add(newProg); 
                }
            }
            xmlReader.Close();
            return programList.OrderBy(e => e.StartTime).ToList();

        }

        /// <summary>
        /// Parse XMLTV file for channels
        /// </summary>
        public List<XmlTvChannel> LoadChannelList(string file) {

            // USING XMLTEXTREADER RATHER THAN DESERIALIZATION WITH DTD BECAUSE NOT ALL XMLTV FILES
            // IN NEW ZEALAND HAVE VALID XML AND THEN DON'T DESERIALIZE PROPERLY.

            XmlTextReader xmlReader = new XmlTextReader(file) {XmlResolver = null};
            List<XmlTvChannel> channelList = new List<XmlTvChannel>();

            xmlReader.Read(); 
            while (xmlReader.Read()) {
                
                if ((xmlReader.Name == "channel") && (xmlReader.NodeType==XmlNodeType.Element)) {
                    XmlTvChannel channel = new XmlTvChannel();
                    channel.Id = xmlReader.GetAttribute("id");
                    
                    int depth = xmlReader.Depth;
                    xmlReader.Read();
                    while (xmlReader.Depth > depth) {
                        if (xmlReader.NodeType == XmlNodeType.Element) {
                            switch (xmlReader.Name.ToString()) {
                                case "display-name":
                                    if (String.IsNullOrEmpty(channel.DisplayName)) {
                                        channel.DisplayName = xmlReader.ReadString().Trim();
                                    }
                                    break;
                                case "icon":
                                    try {
                                        channel.Icon = xmlReader.GetAttribute("src");
                                    } catch {
                                        // ignore this 
                                    }
                                    break;
                            }
                        }
                        xmlReader.Read(); // get the next element within the program
                    }

                    channelList.Add(channel);
                }
            }
            xmlReader.Close();

            return channelList;
        }

        public string Name {
            get { return "Basic manual parse xml reader";  }
        }
    }
}