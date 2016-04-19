using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuideHoundEPG.Common;
using GuideHoundEPG.Common.Detections;
using GuideHoundEPG.Common.Model.Configuration;
using GuideHoundEPG.Common.Model.MXF;
using GuideHoundEPG.Common.Model.XmlTv;
using Channel = GuideHoundEPG.Common.Model.MXF.Channel;

namespace GuideHoundEPG {
    public class XmlTvToMxfMapper {

        public MxfFile Map(XmlTvData xmlTvData, CategoryMapping categoryMapping, CacheProvider<string, SeriesInfo> seriesCache, CacheProvider<string, Keyword> keywordCache, CacheProvider<string, Person> personCache) {
            

            // build our MXF file from XMLTV data

            DetectionStatistics mxfDetectionStats = new DetectionStatistics();

            MxfFile mxfFile = new MxfFile();

            // Register provider
            Provider provider = new Provider() {
                Name = "Provider",
                DisplayName = "Provider"
            };

            // add the primary lineup
            Lineup lineup = Lineup.Primary;
            provider.AddLineup(Lineup.Primary);

            //// keywords
            //KeywordGroup primaryKeywordGroup = new KeywordGroup();
            //primaryKeywordGroup.GroupName = "General";

            //Keyword allKeyword = new Keyword();
            //allKeyword.Id = "k1";
            //allKeyword.Word = "General";
            //primaryKeywordGroup.Keywords.Add(allKeyword);

            //provider.KeywordGroups.Add(primaryKeywordGroup);
            //provider.Keywords.Add(allKeyword);

            // affiliates
            Affiliate affiliate = new Affiliate() {
                Name = "EPG Importer"
            };
            provider.AddAffiliate(affiliate);


            // channels
            foreach (Common.Model.Configuration.Channel channelInfo in xmlTvData.ChannelConfiguration.Where(c => c.IsEnabled)) {

                Service service = new Service();
                service.Name = channelInfo.Name;
                service.Affiliate = affiliate;

                if (!String.IsNullOrEmpty(channelInfo.LogoUrl)) {
                    GuideImage guideImage = new GuideImage(channelInfo.LogoUrl);
                    service.LogoImage = guideImage;
                }

                provider.AddService(service);

                Channel channel = new Channel();
                channel.Id = channelInfo.Id;
                channel.Number = channelInfo.Id;
                channel.Lineup = lineup;
                channel.Service = service;
                channel.XmlMappedChannel = channelInfo.XmlTvId;

                provider.AddChannel(channel);

                // find programs for this channel
                var programsForChannel = from p in xmlTvData.Programs
                                         where (p.Channel == channel.XmlMappedChannel)
                                         select p;

                foreach (var tvProgram in programsForChannel) {

                    // We're currently not providing support for multiple screening of a single program, therefore a 
                    // Program is the same concept as a ScheduleEntry.

                    // program identied as exact match on channel/startdate/enddate.
                    string programKey = channel.Number.ToString("0000") + tvProgram.StartTime.Ticks.ToString();

                    // add program entry
                    ProgramInfo program = new ProgramInfo();
                    program.Id = programKey;
                    program.Title = tvProgram.Title;
                    program.EpisodeTitle = tvProgram.SubTitle;
                    program.Description = tvProgram.Description;

                    // add the scheduled entry.
                    ScheduleEntry entry = new ScheduleEntry();
                    entry.Program = program;
                    entry.StartTime = tvProgram.StartTime;
                    entry.StopTime = tvProgram.StopTime;

                    //if (entry.StartTime==entry.StopTime) {
                    //    // start and end time are the same? 
                    //    var index = programsForChannel.ToList().IndexOf(tvProgram)+1;
                        
                    //    var nextTvProgram = programsForChannel.ToList()[index];

                    //    if (nextTvProgram!=null) {
                    //        entry.StopTime = nextTvProgram.StartTime;
                    //    }

                    //    if (entry.StartTime==entry.StopTime) {
                    //        entry.StopTime = entry.StartTime.AddMinutes(5);
                    //    }

                    //}

                    entry.Channel = channel;
                    program.DurationInMinutes = entry.DurationInMinutes;
                    program.IsSeries = true;

                    // update series cache
                    SeriesCachingHelper.UpdateProgramSeriesCache(provider, seriesCache, program, program.Title);
                    

                    // additional "nice to have" properties
                    entry.IsHdtv = tvProgram.VideoQuality == "HDTV";
                    program.HalfStars = tvProgram.HalfStars;

                    // credits
                    foreach (XmlTvCredit tvCredit in tvProgram.Credits) {

                        // lookup person
                        Person person = personCache.FindKey(tvCredit.Person);
                        if (person == null) {
                            person = new Person() { Name = tvCredit.Person };
                            personCache.Add(person);
                            provider.AddPerson(person);
                        }

                        if (tvCredit.CreditType == "director") {
                            program.AddRole<DirectorRole>(person);
                        }

                        if (tvCredit.CreditType == "actor") {
                            program.AddRole<ActorRole>(person);
                        }

                        if (tvCredit.CreditType == "writer") {
                            program.AddRole<WriterRole>(person);
                        }

                        if (tvCredit.CreditType == "producer") {
                            program.AddRole<ProducerRole>(person);
                        }

                    }

                    if (channelInfo.IsAudio) {
                        program.IsAudio = true;
                    }

                    // category mapping
                    if (categoryMapping.CategoryMappingEnabled && !program.IsAudio) {

                        foreach (var category in tvProgram.CategoryList) {
                            CategoryMap categoryMap = categoryMapping.CategoryMap.Where(m => m.SourceCategory == category).FirstOrDefault();
                            if (categoryMap != null) {
                                program.ApplyCategory(categoryMap.MappedCategory);
                                mxfDetectionStats.Increment(categoryMap.MappedCategory);
                            }
                        }

                    }

                    try {
                        provider.AddProgram(program);
                        provider.AddScheduleEntries(entry);
                    } catch (Exception ex) {
                        // unable to add this program due to error
                    }
                    

                }
            }
            mxfFile.AddProvider(provider);

            return mxfFile;
    
        }    
    }
}
