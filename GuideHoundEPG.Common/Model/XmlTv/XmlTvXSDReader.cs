using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Core.Logging;

namespace GuideHoundEPG.Common.Model.XmlTv {
    public class XmlTvXSDReader : IXmlTvReader {

        private static ILogger log = Logger.GetLogger();

        public string Name {
            get { return "Xsd based xmltv reader"; }
        }

        public List<XmlTvProgram> LoadProgramList(string file) {

            StreamReader str = new System.IO.StreamReader(file);
            XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(tv));
            tv tv = (tv)xSerializer.Deserialize(str);

            List<XmlTvProgram> programList = new List<XmlTvProgram>();

            if (tv.programme != null) {

                foreach (var xmlProgram in tv.programme) {

                    try {

                        XmlTvProgram newProg = new XmlTvProgram();
                        newProg.StartTime = DateTools.FormatDate(xmlProgram.start.Trim());
                        newProg.StopTime = DateTools.FormatDate(xmlProgram.stop.Trim());
                        newProg.Channel = xmlProgram.channel.Trim();

                        if (xmlProgram.title != null && xmlProgram.title.Count() > 0 && xmlProgram.title.FirstOrDefault() != null && xmlProgram.title.FirstOrDefault().Text != null) {
                            newProg.Title = xmlProgram.title.FirstOrDefault().Text.FirstOrDefault();
                        }

                        if (xmlProgram.desc != null && xmlProgram.desc.Count() > 0 && xmlProgram.desc.FirstOrDefault() != null && xmlProgram.desc.FirstOrDefault().Text != null) {
                            newProg.Description = xmlProgram.desc.FirstOrDefault().Text.FirstOrDefault();
                        }

                        if (xmlProgram.subtitle != null && xmlProgram.subtitle.Count() > 0 && xmlProgram.subtitle.FirstOrDefault() != null && xmlProgram.subtitle.FirstOrDefault().Text != null) {
                            newProg.SubTitle = xmlProgram.subtitle.First().Text.First();
                        }

                        if (xmlProgram.category != null) {
                            foreach (var category in xmlProgram.category) {
                                if (category != null && category.Text != null && category.Text.FirstOrDefault() != null) {
                                    newProg.CategoryList.Add(category.Text.FirstOrDefault());
                                }
                            }
                        }

                        if (xmlProgram.video != null && xmlProgram.video.present != null && xmlProgram.video.present.Text != null) {
                            newProg.VideoPresent = xmlProgram.video.present.Text.FirstOrDefault();
                        }

                        if (xmlProgram.video != null && xmlProgram.video.aspect != null && xmlProgram.video.aspect.Text != null) {
                            newProg.VideoAspect = xmlProgram.video.aspect.Text.FirstOrDefault();
                        }

                        if (xmlProgram.video != null && xmlProgram.video.quality != null && xmlProgram.video.quality.Text != null) {
                            newProg.VideoQuality = xmlProgram.video.quality.Text.FirstOrDefault();
                        }

                        if (xmlProgram.credits != null) {

                            if (xmlProgram.credits.actor != null) {
                                foreach (var actor in xmlProgram.credits.actor) {
                                    XmlTvCredit newCredit = new XmlTvCredit {
                                        Person =
                                            actor.Text != null
                                                ? actor.Text.FirstOrDefault()
                                                : string.Empty,
                                        CreditType = "actor"
                                    };
                                    newProg.Credits.Add(newCredit);
                                }
                            }

                            if (xmlProgram.credits.director != null) {

                                foreach (var director in xmlProgram.credits.director) {
                                    XmlTvCredit newCredit = new XmlTvCredit {
                                        Person =
                                            director.Text != null
                                                ? director.Text.FirstOrDefault()
                                                : string.Empty,
                                        CreditType = "director"
                                    };
                                    newProg.Credits.Add(newCredit);
                                }
                            }

                            if (xmlProgram.credits.producer != null) {
                                foreach (var producer in xmlProgram.credits.producer) {
                                    XmlTvCredit newCredit = new XmlTvCredit {
                                        Person =
                                            producer.Text != null
                                                ? producer.Text.FirstOrDefault()
                                                : string.Empty,
                                        CreditType = "producer"
                                    };
                                    newProg.Credits.Add(newCredit);
                                }
                            }

                            if (xmlProgram.credits.writer != null) {
                                foreach (var writer in xmlProgram.credits.writer) {
                                    XmlTvCredit newCredit = new XmlTvCredit {
                                        Person = writer.Text != null ? writer.Text.FirstOrDefault() : string.Empty,
                                        CreditType = "writer"
                                    };
                                    newProg.Credits.Add(newCredit);
                                }
                            }

                        }

                        //// credits
                        //XmlNode credits = prog.SelectSingleNode("credits");
                        //if (credits != null)
                        //{
                        //    foreach (XmlNode role in credits)
                        //    {
                        //        XmlTvCredit newCredit = new XmlTvCredit();
                        //        newCredit.CreditType = role.Name;
                        //        if (role.Attributes["role"] != null)
                        //        {
                        //            newCredit.Role = role.Attributes["role"].Value;
                        //        }
                        //        if (role.FirstChild != null)
                        //        {
                        //            newCredit.Person = role.FirstChild.Value;
                        //        }
                        //        newProg.Credits.Add(newCredit);
                        //    }
                        //}

                        // bse episode number scheme
                        if (xmlProgram.episodenum != null) {
                            episodenum bseEpisodeNum = xmlProgram.episodenum.Where(e => e.system == "bsepg-epid").FirstOrDefault();
                            if (bseEpisodeNum != null && bseEpisodeNum.Text != null && bseEpisodeNum.Text.FirstOrDefault() != null && !string.IsNullOrWhiteSpace(bseEpisodeNum.Text.FirstOrDefault())) {
                                newProg.BseEpisodeInfo = new BseEpisodeInfo(bseEpisodeNum.Text.FirstOrDefault());
                            }
                        }

                        programList.Add(newProg);

                    } catch (Exception e) {
                        log.Log(LogLevel.Error, e, "Error parsing a program from the xmltv file - skipping this program.");
                    }
                }
            }

            return programList.OrderBy(e => e.StartTime).ToList();
        }

        public List<XmlTvChannel> LoadChannelList(string file) {

            StreamReader str = new System.IO.StreamReader(file);
            XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(tv));
            tv tv = (tv)xSerializer.Deserialize(str);


            List<XmlTvChannel> channelList = new List<XmlTvChannel>();

            foreach (var xmlChannel in tv.channel) {
                XmlTvChannel channel = new XmlTvChannel();
                channel.Id = xmlChannel.id;

                channel.DisplayName = xmlChannel.displayname.First().Text.First().Trim();

                if (xmlChannel.icon != null) {
                    channel.Icon = xmlChannel.icon.First().src;
                }

                channelList.Add(channel);
            }

            return channelList;

        }
    }
}