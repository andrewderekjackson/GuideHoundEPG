using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Core.Logging;
using log4net;

namespace GuideHoundEPG.Common.Model.MXF {
    public class MxfWriter {

        private static readonly ILogger log = Logger.GetLogger();

        private XmlTextWriter xmlWriter;
        private MxfFile mxfFile;
        private string outputFolder;
        private string outputFileName;

        private string mxfOutputFile;

        public MxfWriter(MxfFile mxfFile, string outputFolder, string outputFileName) {
            this.mxfFile = mxfFile;
            this.outputFolder = outputFolder;
            this.outputFileName = outputFileName;

            mxfOutputFile = Path.Combine(outputFolder, outputFileName);
        }

        public void Save() {
            ProcessImages();
            SaveToFile(mxfOutputFile);
        }

        public void ProcessImages() {
            // process images and write to output folder
            log.Log(LogLevel.Info, String.Format("Processing images ..."));

            try {

                string channelLogoFolder = Path.Combine(outputFolder, "ChannelLogos");
                log.Log(LogLevel.Debug, String.Format("Making sure that channel logo folder '{0}' exists.", channelLogoFolder));
                if (!Directory.Exists(channelLogoFolder)) {
                    log.Log(LogLevel.Debug, String.Format("Does not exist - creating."));
                    Directory.CreateDirectory(channelLogoFolder);
                }
                string seriesPosterFolder = Path.Combine(outputFolder, "SeriesPosters");
                log.Log(LogLevel.Debug, String.Format("Making sure that series poster folder '{0}' exists.", seriesPosterFolder));
                if (!Directory.Exists(seriesPosterFolder)) {
                    log.Log(LogLevel.Debug, String.Format("Does not exist - creating."));
                    Directory.CreateDirectory(seriesPosterFolder);
                }
                string moviePosterFolder = Path.Combine(outputFolder, "MoviePosters");
                log.Log(LogLevel.Debug, String.Format("Making sure that movie poster folder '{0}' exists.", moviePosterFolder));
                if (!Directory.Exists(moviePosterFolder)) {
                    log.Log(LogLevel.Debug, String.Format("Does not exist - creating."));
                    Directory.CreateDirectory(moviePosterFolder);
                }

                foreach (var providerList in mxfFile.Providers) {
                    foreach (var guideImage in providerList.GuideImages) {
                        log.Log(LogLevel.Debug, String.Format("Processing image '{0}'...", guideImage.SourceUrl));

                        bool isUrl = false;
                        if (!String.IsNullOrEmpty(guideImage.SourceUrl)) {
                            Uri imageUri = new Uri(guideImage.SourceUrl ?? String.Empty);
                            if (imageUri.IsFile) {

                                // copy file to output folder
                                string imageOutoutFolder = outputFolder;
                                if (guideImage.OutputImageType == GuideImageType.ChannelLogo) {
                                    imageOutoutFolder = channelLogoFolder;
                                } else if (guideImage.OutputImageType == GuideImageType.MoviePoster) {
                                    imageOutoutFolder = moviePosterFolder;
                                } else if (guideImage.OutputImageType == GuideImageType.SeriesPoster) {
                                    imageOutoutFolder = seriesPosterFolder;
                                }

                                log.Log(LogLevel.Debug, String.Format("This is a local file URL - copying to output image folder '{0}'...", imageOutoutFolder));
                                string dest = Path.Combine(imageOutoutFolder, Path.GetFileName(guideImage.SourceUrl));
                                try {
                                    File.Copy(guideImage.SourceUrl, dest, true);
                                } catch (IOException) {
                                    log.Log(LogLevel.Debug, "Cannot copy image file to output folder - skipping.");
                                }

                                // make a media center compatable url
                                guideImage.ImageUrl = @"file://" + dest;

                            } else {
                                isUrl = true;
                            }
                        }

                        if (isUrl) {
                            log.Log(LogLevel.Debug, String.Format("This is a internet URL - no action required."));
                            guideImage.ImageUrl = guideImage.SourceUrl;
                        }
                    }
                }
            } catch (Exception e) {
                log.Log(LogLevel.Debug, "An general error occured processing images.", e);
            }

        }

        public bool SaveToFile(string fileName) {
            
            // TODO: Refactor and put unit testing around this.


            // write out the mxf file
            log.Log(LogLevel.Info, String.Format("Writing output file '{0}'...", mxfOutputFile));


            xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;

            // start of document
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("MXF");
            xmlWriter.WriteAttributeString("xmlns:sql", "urn:schemas-microsoft-com:xml-sql");
            xmlWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            WriteMxfHeader();
            
            // list of providers
            xmlWriter.WriteStartElement("Providers");

            foreach (var provider in mxfFile.Providers) {
                xmlWriter.WriteStartElement("Provider");
                xmlWriter.WriteAttributeString("id", provider.Id);
                xmlWriter.WriteAttributeString("name", provider.Name);
                xmlWriter.WriteAttributeString("displayName", provider.DisplayName);
                xmlWriter.WriteAttributeString("copyright", provider.Copyright);
                xmlWriter.WriteEndElement(); 
            }
            
            xmlWriter.WriteEndElement();

            foreach (var provider in mxfFile.Providers) {
                // provider
                xmlWriter.WriteStartElement("With");
                xmlWriter.WriteAttributeString("provider", provider.Id);

                // keywords
                xmlWriter.WriteStartElement("Keywords");
                foreach (var keyword in provider.Keywords) {
                    xmlWriter.WriteStartElement("Keyword");
                    xmlWriter.WriteAttributeString("id", keyword.Id);
                    xmlWriter.WriteAttributeString("word", keyword.Word);
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndElement();                 

                // keyword groups    
                xmlWriter.WriteStartElement("KeywordGroups");
                foreach (var keywordGroup in provider.KeywordGroups) {
                    xmlWriter.WriteStartElement("KeywordGroup");
                    xmlWriter.WriteAttributeString("uid", keywordGroup.Uid);
                    xmlWriter.WriteAttributeString("groupName", keywordGroup.GroupName);
                    xmlWriter.WriteAttributeString("keywords", keywordGroup.Keywords.GetDelimitedString("Id"));
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndElement();   
  
                // guide images   
                xmlWriter.WriteStartElement("GuideImages");
                foreach (var guideImage in provider.GuideImages) {
                    xmlWriter.WriteStartElement("GuideImage");
                    xmlWriter.WriteAttributeString("id", guideImage.Id);
                    xmlWriter.WriteAttributeString("uid", guideImage.Uid);
                    xmlWriter.WriteAttributeString("imageUrl", guideImage.ImageUrl);
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndElement();  

                // people
                xmlWriter.WriteStartElement("People");
                foreach (var person in provider.People) {
                    xmlWriter.WriteStartElement("Person");
                    xmlWriter.WriteAttributeString("id", person.Id);
                    xmlWriter.WriteAttributeString("uid", person.Uid);
                    xmlWriter.WriteAttributeString("name", person.Name);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();  

                // series info 
                xmlWriter.WriteStartElement("SeriesInfos");
                foreach (var series in provider.Series) {
                    xmlWriter.WriteStartElement("SeriesInfo");
                    xmlWriter.WriteAttributeString("id", series.Id);
                    xmlWriter.WriteAttributeString("uid", series.Uid);
                    xmlWriter.WriteAttributeString("title", series.Title);
                    xmlWriter.WriteAttributeString("description", series.Description);
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndElement(); 

                // seasons - not supported yet
                xmlWriter.WriteStartElement("Seasons");
                xmlWriter.WriteEndElement(); 

                // programs 
                xmlWriter.WriteStartElement("Programs");
                foreach (var program in provider.Programs) {
                    xmlWriter.WriteStartElement("Program");
                    xmlWriter.WriteAttributeString("id", program.Id);
                    xmlWriter.WriteAttributeString("uid", program.Uid);
                    xmlWriter.WriteAttributeString("title", program.Title);
                    xmlWriter.WriteAttributeString("description", program.Description);
                    xmlWriter.WriteAttributeString("keywords", program.Keywords.GetDelimitedString("Id"));
                    if (program.Series != null) {
                        xmlWriter.WriteAttributeString("series", program.Series.Id);
                        xmlWriter.WriteAttributeString("isSeries", program.IsSeries.ToMxfBoolean());
                    }
                    xmlWriter.WriteAttributeString("isMovie", program.IsMovie.ToMxfBoolean());
                    xmlWriter.WriteAttributeString("isNews", program.IsNews.ToMxfBoolean());
                    xmlWriter.WriteAttributeString("isSports", program.IsSports.ToMxfBoolean());
                    xmlWriter.WriteAttributeString("isSpecial", program.IsSpecial.ToMxfBoolean());
                    xmlWriter.WriteAttributeString("isSerial", program.IsSerial.ToMxfBoolean());
                    xmlWriter.WriteAttributeString("isReality", program.IsReality.ToMxfBoolean());
                    if (program.GuideImage!=null) {
                        xmlWriter.WriteAttributeString("guideImage", program.GuideImage.Uid);
                    }

                    foreach (var roleGroup in program.Roles.GroupBy(g=>g.GetType().Name)) {
                        int rank = 0;
                        foreach (var role in roleGroup) {
                            xmlWriter.WriteStartElement(role.GetType().Name);
                            xmlWriter.WriteAttributeString("person", role.Person.Uid);
                            xmlWriter.WriteAttributeString("rank", rank.ToString());
                            xmlWriter.WriteEndElement();
                            rank++;
                        }
                    }
                    xmlWriter.WriteEndElement(); 

                }
                xmlWriter.WriteEndElement(); 

                // affiliates
                xmlWriter.WriteStartElement("Affiliates");
                foreach (var affiliate in provider.Affiliates) {
                    xmlWriter.WriteStartElement("Affiliate");
                    xmlWriter.WriteAttributeString("name", affiliate.Name);
                    xmlWriter.WriteAttributeString("uid", affiliate.Uid);
                    if (affiliate.LogoImage != null) {
                        xmlWriter.WriteAttributeString("logoImage", affiliate.LogoImage.Id);
                    }   
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndElement();

                // services
                xmlWriter.WriteStartElement("Services");
                foreach (var service in provider.Services) {
                    xmlWriter.WriteStartElement("Service");
                    xmlWriter.WriteAttributeString("id", service.Id);
                    xmlWriter.WriteAttributeString("uid", service.Uid);
                    xmlWriter.WriteAttributeString("name", service.Name);
                    xmlWriter.WriteAttributeString("callsign", service.CallSign);
                    xmlWriter.WriteAttributeString("affiliate", service.Affiliate.Uid);
                    if (service.LogoImage!=null) {
                        xmlWriter.WriteAttributeString("logoImage", service.LogoImage.Id);
                    }
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndElement();

                // schedule entry
                foreach (var channel in provider.Channels) {
                    Channel currentChannel = channel;

                    List<ScheduleEntry> entiresForChannel = (from c in provider.ScheduleEntries
                                                             where c.Channel == currentChannel
                                                             orderby c.StartTime
                                                             select c).ToList();
                    
                    // create groups if there are gaps in the times
                    Dictionary<int, List<ScheduleEntry>> scheduleGroups = new Dictionary<int, List<ScheduleEntry>>();
                    int currentGroupId = -1;
                    for (int i = 0; i < entiresForChannel.Count(); i++) {
                        
                        ScheduleEntry currentEntry = entiresForChannel[i];

                        bool startNewGroup = false;
                        if (i > 0) {
                           
                            ScheduleEntry previousEntry = entiresForChannel[i - 1];
                            TimeSpan diff = currentEntry.StartTime - previousEntry.StopTime;
                            if (diff.Ticks>0) {
                                startNewGroup = true;
                            }
                        } else if (i==0) {
                            startNewGroup = true;
                        }

                        if (startNewGroup) {
                            currentGroupId++;
                            scheduleGroups.Add(currentGroupId, new List<ScheduleEntry>());
                        }

                        // add the current entry to the current group.
                        scheduleGroups[currentGroupId].Add(currentEntry);
                    }

                    // write out the grouped entries for a channel.
                    foreach (var scheduleGroup in scheduleGroups.Values) {

                        xmlWriter.WriteStartElement("ScheduleEntries");
                        xmlWriter.WriteAttributeString("service", channel.Service.Id);

                        foreach (var scheduleEntry in scheduleGroup) {

                            xmlWriter.WriteStartElement("ScheduleEntry");
                            if (scheduleEntry == scheduleGroup.First()) {
                                xmlWriter.WriteAttributeString("startTime", scheduleEntry.StartTime.ToMxfDate());
                            }

                            xmlWriter.WriteAttributeString("program", scheduleEntry.Program.Id);
                            xmlWriter.WriteAttributeString("duration", scheduleEntry.Duration);

                            if (scheduleEntry.IsHdtv) {
                                xmlWriter.WriteAttributeString("isHdtv", scheduleEntry.IsHdtv.ToMxfBoolean());
                            }
                            if (scheduleEntry.IsLetterbox) {
                                xmlWriter.WriteAttributeString("isLetterbox", scheduleEntry.IsLetterbox.ToMxfBoolean());
                            }

                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                    }
                }

                // lineups
                xmlWriter.WriteStartElement("Lineups");
                foreach (var lineup in provider.Lineups) {
                    xmlWriter.WriteStartElement("Lineup");
                    xmlWriter.WriteAttributeString("id", lineup.Id);
                    xmlWriter.WriteAttributeString("uid", lineup.Uid);
                    xmlWriter.WriteAttributeString("name", lineup.Name);
                    xmlWriter.WriteAttributeString("primaryProvider", lineup.PrimaryProvider);
                    
                    xmlWriter.WriteStartElement("channels");
                    foreach (var channel in provider.Channels) {
                        xmlWriter.WriteStartElement("Channel");
                        xmlWriter.WriteAttributeString("uid", channel.Uid);
                        xmlWriter.WriteAttributeString("lineup", lineup.Id);
                        xmlWriter.WriteAttributeString("service", channel.Service.Id);
                        xmlWriter.WriteAttributeString("number", channel.Number.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement(); 
                    xmlWriter.WriteEndElement();
                   
                }
                xmlWriter.WriteEndElement();
                
                
                xmlWriter.WriteEndElement();

                // close provider
                xmlWriter.WriteEndElement(); 
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            //log.Log(LogLevel.Info, "Total Programs: "+mxfFile.Providers.First().Programs.Count);
            //log.Log(LogLevel.Info, "Category Detection");
            //log.Log(LogLevel.Info, " Series: "+mxfFile.Providers.First().Programs.Where(e=>e.IsSeries).Count());
            //log.Log(LogLevel.Info, " Sports: "+mxfFile.Providers.First().Programs.Where(e=>e.IsSports).Count());
            //log.Log(LogLevel.Info, " Movies: "+mxfFile.Providers.First().Programs.Where(e=>e.IsMovie).Count());
            //log.Log(LogLevel.Info, " News: "+mxfFile.Providers.First().Programs.Where(e=>e.IsNews).Count());
            //log.Log(LogLevel.Info, " Reality: "+mxfFile.Providers.First().Programs.Where(e=>e.IsReality).Count());

            log.Log(LogLevel.Debug, "Movie Summary");
            mxfFile.Providers.First().Programs.Where(e=>e.IsMovie).ToList().ForEach(e=> log.Log(LogLevel.Debug, String.Format(" Movie: {0}, Filled: {1}, Duration: {2}", e.Title, e.IsMetadataLookup, e.DurationInMinutes)));
       

            return true;
        }
        
        private void WriteMxfHeader() {
            
            xmlWriter.WriteStartElement("Assembly");
            xmlWriter.WriteAttributeString("name", "mcepg");
            xmlWriter.WriteAttributeString("version", "6.0.6000.0");
            xmlWriter.WriteAttributeString("cultureInfo", "");
            xmlWriter.WriteAttributeString("publicKey", "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9");

            xmlWriter.WriteStartElement("NameSpace");
            xmlWriter.WriteAttributeString("name", "Microsoft.MediaCenter.Guide");

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Lineup");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Channel");
            xmlWriter.WriteAttributeString("parentFieldName", "lineup");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Service");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "ScheduleEntry");
            xmlWriter.WriteAttributeString("groupName", "ScheduleEntries");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Program");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Keyword");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "KeywordGroup");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Person");
            xmlWriter.WriteAttributeString("groupName", "People");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "ActorRole");
            xmlWriter.WriteAttributeString("parentFieldName", "program");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "DirectorRole");
            xmlWriter.WriteAttributeString("parentFieldName", "program");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "WriterRole");
            xmlWriter.WriteAttributeString("parentFieldName", "program");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "HostRole");
            xmlWriter.WriteAttributeString("parentFieldName", "program");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "GuestActorRole");
            xmlWriter.WriteAttributeString("parentFieldName", "program");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "ProducerRole");
            xmlWriter.WriteAttributeString("parentFieldName", "program");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "GuideImage");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Affiliate");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "SeriesInfo");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Season");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteEndElement(); 
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Assembly");
            xmlWriter.WriteAttributeString("name", "mcstore");
            xmlWriter.WriteAttributeString("version", "6.0.6000.0");
            xmlWriter.WriteAttributeString("cultureInfo", "");
            xmlWriter.WriteAttributeString("publicKey", "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9");

            xmlWriter.WriteStartElement("NameSpace");
            xmlWriter.WriteAttributeString("name", "Microsoft.MediaCenter.Store");

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "Provider");
            xmlWriter.WriteEndElement(); 

            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteAttributeString("name", "UId");
            xmlWriter.WriteAttributeString("parentFieldName", "target");
            xmlWriter.WriteEndElement();            

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement(); 

        }

    }
}
