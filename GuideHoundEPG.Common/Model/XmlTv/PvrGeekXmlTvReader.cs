using System;
using System.Collections.Generic;
using System.Xml;

namespace GuideHoundEPG.Common.Model.XmlTv {
    public class PvrGeekXmlTvReader : IXmlTvReader {

        public string Name {
            get { return "Pvr.geek.nz manual parse xml reader"; }
        }

        /// <summary>
        /// Parse XMLTV file for programs
        /// </summary>
        public List<XmlTvProgram> LoadProgramList(string file) {

            // USING XMLTEXTREADER RATHER THAN DESERIALIZATION WITH DTD BECAUSE NOT ALL XMLTV FILES
            // IN NEW ZEALAND HAVE VALID XML AND THEN DON'T DESERIALIZE PROPERLY.

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            List<XmlTvProgram> programList = new List<XmlTvProgram>();

            XmlNodeList list = xmldoc.GetElementsByTagName("programme");
            foreach (XmlNode prog in list) {
                XmlTvProgram newProg = new XmlTvProgram();
                newProg.StartTime = DateTools.FormatDate(prog.Attributes["start"].Value);
                newProg.StopTime = DateTools.FormatDate(prog.Attributes["stop"].Value);
                newProg.Channel = prog.Attributes["channel"].Value;

                // title
                XmlNode title = prog.SelectSingleNode("title");
                if (title != null) {
                    newProg.Title = title.FirstChild.Value;
                }

                // description
                XmlNode desc = prog.SelectSingleNode("desc");
                if (desc != null) {
                    if (desc.FirstChild != null) {
                        newProg.Description = desc.FirstChild.Value;
                    }
                }

                // sub-title
                XmlNode subtitle = prog.SelectSingleNode("sub-title");
                if (subtitle != null) {
                    if (subtitle.FirstChild != null) {
                        newProg.SubTitle = subtitle.FirstChild.Value;
                    }
                }

                // rating
                XmlNode rating = prog.SelectSingleNode("rating");
                if (rating != null) {
                    XmlNode ratingValue = rating.SelectSingleNode("value");
                    if (ratingValue != null) {
                        if (ratingValue.FirstChild != null) {
                            newProg.Rating = ratingValue.FirstChild.Value;
                        }
                    }
                }

                // categories
                foreach (XmlNode category in prog.SelectNodes("category")) {
                    if (category.FirstChild != null) {
                        newProg.CategoryList.Add(category.FirstChild.Value);
                    }
                }

                // video
                XmlNode video = prog.SelectSingleNode("video");
                if (video != null) {
                    foreach (XmlNode videoQuality in video.SelectNodes("quality")) {
                        if (videoQuality.FirstChild != null) {
                            newProg.VideoQuality = videoQuality.FirstChild.Value;
                        }
                    }
                    foreach (XmlNode videoPresent in video.SelectNodes("present")) {
                        if (videoPresent.FirstChild != null) {
                            newProg.VideoPresent = videoPresent.FirstChild.Value;
                        }
                    }
                    foreach (XmlNode videoAspect in video.SelectNodes("aspect")) {
                        if (videoAspect.FirstChild != null) {
                            newProg.VideoAspect = videoAspect.FirstChild.Value;
                        }
                    }
                }

                // star-rating
                XmlNode starRating = prog.SelectSingleNode("star-rating");
                if (starRating != null) {
                    XmlNode starRatingValue = starRating.SelectSingleNode("value");
                    if (starRatingValue != null) {
                        if (starRatingValue.FirstChild != null) {
                            newProg.StarRating = starRatingValue.FirstChild.Value;
                        }
                    }
                }

                // credits
                XmlNode credits = prog.SelectSingleNode("credits");
                if (credits != null) {
                    foreach (XmlNode role in credits) {
                        XmlTvCredit newCredit = new XmlTvCredit();
                        newCredit.CreditType = role.Name;
                        if (role.Attributes["role"] != null) {
                            newCredit.Role = role.Attributes["role"].Value;
                        }
                        if (role.FirstChild != null) {
                            newCredit.Person = role.FirstChild.Value;
                        }
                        newProg.Credits.Add(newCredit);
                    }
                }

                programList.Add(newProg);

            }
  
            return programList;

        }

        /// <summary>
        /// Parse XMLTV file for channels
        /// </summary>
        public List<XmlTvChannel> LoadChannelList(string file) {

            //// USING XMLTEXTREADER RATHER THAN DESERIALIZATION WITH DTD BECAUSE NOT ALL XMLTV FILES
            //// IN NEW ZEALAND HAVE VALID XML AND THEN DON'T DESERIALIZE PROPERLY.

            XmlTextReader xmlReader = new XmlTextReader(file) { XmlResolver = null };

            List<XmlTvChannel> channelList = new List<XmlTvChannel>();

            xmlReader.Read();
            while (xmlReader.Read()) {

                if ((xmlReader.Name == "channel") && (xmlReader.NodeType == XmlNodeType.Element)) {
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
    }
}