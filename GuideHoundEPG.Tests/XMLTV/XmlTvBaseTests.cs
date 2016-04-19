using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common.Model.MXF;
using GuideHoundEPG.Tests.Properties;
using System.Reflection;
using System.IO;
using GuideHoundEPG.Common.Model.XmlTv;

namespace GuideHoundEPG.Tests.XMLTV {
    
    [TestClass]
    public abstract class XmlTvBaseTests {

        public abstract IXmlTvReader XmlTvReader { get; }

        public string WriteTextToFile(string text) {

            String tempFileName = System.IO.Path.GetTempFileName();

            using (StreamWriter sw = File.CreateText(tempFileName)) {
                sw.WriteLine(text);
                sw.Flush();
                sw.Close();
            }

            return tempFileName;
        }

        [TestMethod]
        public virtual void CanReadChannelList() {

            string xmlTv = WriteTextToFile(Resources.testfile1);

            List<XmlTvChannel> channels = XmlTvReader.LoadChannelList(xmlTv);
            Assert.AreEqual(4, channels.Count, "Unable to find all 4 channels in the source document");

        }
        
        [TestMethod]
        public virtual void CanReadBasicChannelDetails() {

            string xmlTv = WriteTextToFile(Resources.testfile1);

            List<XmlTvChannel> channels = XmlTvReader.LoadChannelList(xmlTv);
            Assert.AreEqual(4, channels.Count, "Unable to find all 4 channels in the source document");

            // display name & icon
            XmlTvChannel c1 = channels[0];
            Assert.AreEqual(c1.Id, "01", "Channel 1 - id not found");
            Assert.AreEqual(c1.DisplayName, "TV1", "Channel 1 - displayname not found");
            Assert.AreEqual(c1.Icon, "http://www.gbpvr.com/logo/TV1.jpeg", "Channel 1 - icon url not found");

        }

        [TestMethod]
        public virtual void ShouldIgnoreMultipleDisplayNames() {

            string xmlTv = WriteTextToFile(Resources.testfile1);

            List<XmlTvChannel> channels = XmlTvReader.LoadChannelList(xmlTv);
            Assert.AreEqual(4, channels.Count, "Unable to find all 4 channels in the source document");

            // multiple display names - ignore the second
            XmlTvChannel c2 = channels[1];
            Assert.AreEqual(c2.Id, "02", "Channel 2 - id not found");
            Assert.AreEqual(c2.DisplayName, "TV2", "Channel 2 - displayname not found");
            Assert.AreEqual(c2.Icon, @"icons\colour\2.png", "Channel 2 - icon url not found");

        }

        [TestMethod]
        public virtual void CanReadProgramList() {

            string xmlTv = WriteTextToFile(Resources.testfile1);

            List<XmlTvProgram> programs = XmlTvReader.LoadProgramList(xmlTv);
            Assert.AreEqual(6, programs.Count, "Unable to find all 6 programs in the source document");

        }

        [TestMethod]
        public virtual void ProgramsAreSortedByStartTime() {

            // THE REASON FOR THIS TEST IS THAT TVONE LISTINGS FROM SOME PROVIDERS ARE NOT SORTED BY
            // STARTDATE BY DEFAULT.

            string xmlTv = WriteTextToFile(Resources.freeview);

            XmlTvProgram[] programs = XmlTvReader.LoadProgramList(xmlTv).Where(e=>e.Channel=="tv1.freeviewnz.tv").ToArray();
            XmlTvProgram[] orderedPrograms = programs.Where(e=>e.Channel=="tv1.freeviewnz.tv").OrderBy(e => e.StartTime).ToArray();

            CollectionAssert.AreEquivalent(orderedPrograms, programs);
            CollectionAssert.AreEqual(orderedPrograms, programs);

        }

        [TestMethod]
        public virtual void CanReadCategories() {
            
            string xmlTv = WriteTextToFile(Resources.testfile1);

            List<XmlTvProgram> programs = XmlTvReader.LoadProgramList(xmlTv);

            Assert.AreEqual(0, programs[0].CategoryList.Count);
            Assert.AreEqual(0, programs[1].CategoryList.Count);

            Assert.AreEqual(2, programs[2].CategoryList.Count);
            Assert.AreEqual("tvshow", programs[2].CategoryList[0]);
            Assert.AreEqual("News", programs[2].CategoryList[1]);
            Assert.AreEqual(1, programs[3].CategoryList.Count);
            Assert.AreEqual("drama", programs[3].CategoryList[0]);
        }

        [TestMethod]
        public virtual void CanReadVideoAspect() {
            
            string xmlTv = WriteTextToFile(Resources.testfile1);

            List<XmlTvProgram> programs = XmlTvReader.LoadProgramList(xmlTv);

            XmlTvProgram program = programs.Where(e => e.Title == "Charles Stanley").First();

            Assert.IsNotNull(program);
            Assert.AreEqual("yes", program.VideoPresent);
            Assert.AreEqual("16:9", program.VideoAspect);
            Assert.AreEqual("HDTV", program.VideoQuality);

        }

        [TestMethod]
        public virtual void CanReadCastAndCrew()
        {

            string xmlTv = WriteTextToFile(Resources.testfile1);

            List<XmlTvProgram> programs = XmlTvReader.LoadProgramList(xmlTv);

            XmlTvProgram program = programs.Where(e => e.Title == "Blue Crush").First();

            Assert.IsNotNull(program);

            Assert.AreEqual(6, program.Credits.Count(e=>e.CreditType=="actor"));
            Assert.AreEqual(1, program.Credits.Count(e => e.CreditType == "director"));
            
        }

        [TestMethod]
        public void ProgramDoNotHaveGapsInThem() {
            string xmlTv = WriteTextToFile(Resources.freeview2);

            XmlTvProgram[] programs =
                XmlTvReader.LoadProgramList(xmlTv).Where(c => c.Channel == "tv1.freeviewnz.tv").OrderBy(c => c.StartTime)
                    .ToArray();

            for (int i = 0; i < programs.Count(); i++) {
                TimeSpan dif = new TimeSpan();
                if (i > 0) {
                     dif = programs[i].StartTime - programs[i - 1].StopTime;
                }
                Trace.WriteLine(String.Format("Start {0}, Stop {1}, Dif {2}", programs[i].StartTime, programs[i].StopTime, dif));
            }
            
        }

        [TestMethod]
        public void CanReadEpisodeNum() {
            string xmlTv = WriteTextToFile(Resources.bsepg_epid);

            XmlTvProgram[] programs =
                XmlTvReader.LoadProgramList(xmlTv).OrderBy(c => c.StartTime)
                    .ToArray();


            programs.Where(e=>e.BseEpisodeInfo!=null).GroupBy(e => e.BseEpisodeInfo.SeriesNumber).ToList().ForEach(g => {
                Trace.WriteLine(String.Format("Program: {0}, Episode Num: {1}", g.Key, g.Count()));

                g.ToList().ForEach(e => {
                    Trace.WriteLine(String.Format(" - Episode: {0}, Id: {3}, SubTitle: {1}, Desc: {2}", e.Title, e.SubTitle, e.Description, e.BseEpisodeInfo.EpisodeNumber));
                });
            });

        }



    }
    
        
}

