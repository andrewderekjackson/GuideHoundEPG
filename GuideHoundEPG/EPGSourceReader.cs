using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Logging;
using GuideHoundEPG.Common.Model.Configuration;
using GuideHoundEPG.Common.Model.XmlTv;

namespace GuideHoundEPG.Common {
    
    public class EPGSourceReader {

        private IXmlTvReader xmlTvReader;
        private IEnumerable<Source> sources;
        private ILogger log = Logger.GetLogger();

        public EPGSourceReader(IEnumerable<Source> sources) {
            this.sources = sources;
            this.xmlTvReader = new XmlTvXSDReader();
        }

        public XmlTvData ReadSources() {
            
            var xmlTvData = new XmlTvData();
            
            // download sources
            foreach (var source in sources.Where(e => e.SourceEnabled)) {

                log.Log(LogLevel.Info, String.Format("Processing source: '{0}'...", source.SourceName));

                XmlSource xmltvSource = source as XmlSource;

                if (xmltvSource != null) {

                    // sanity check
                    if (xmltvSource.Channels.Count == 0) {
                        log.Log(LogLevel.Error, "Warning: No channels have been set up for this source. Use the configuration program to setup channels.");
                    }

                    // run source command
                    if (xmltvSource.CommandEnabled && !String.IsNullOrEmpty(xmltvSource.Command)) {
                        string currentDirectory = Directory.GetCurrentDirectory();

                        Directory.SetCurrentDirectory(xmltvSource.CommandStartFolder);

                        log.Log(LogLevel.Info, String.Format("Running source prefetch command: {0}", xmltvSource.Command));
                        using (Process exeProcess = Process.Start(xmltvSource.Command))
                        {
                            exeProcess.WaitForExit();
                        }

                        Directory.SetCurrentDirectory(currentDirectory);
                    }

                    // fetch file 
                    String tempFilename = EPGDownloader.GetTemporaryCacheFile();
                    string sourceUri = xmltvSource.SourceUri;

                    log.Log(LogLevel.Info, String.Format("Downloading XMLTV file from '{0}'...", sourceUri));

                    EPGDownloader download = new EPGDownloader(sourceUri, tempFilename);
                    if (download.Download()) {
                        // read / import
                        log.Log(LogLevel.Info, String.Format("Reading..."));

                        try {
                            IXmlTvReader xmltv = new XmlTvXSDReader();
                            log.Log(LogLevel.Debug, String.Format("Using xmltv reader '{0}'...", xmlTvReader.Name));

                            List<XmlTvProgram> list = xmltv.LoadProgramList(tempFilename);

                            foreach (var channel in xmltvSource.Channels) {
                                var programsForChannel = list.Where(p => (p.Channel == channel.XmlTvId));
                                xmlTvData.Programs.AddRange(programsForChannel);
                            }

                            xmlTvData.ChannelConfiguration.AddRange(xmltvSource.Channels);

                        }
                        catch (FileNotFoundException fex) {
                            log.Log(LogLevel.Error, String.Format("Unable to find '{0}'. Make sure the file exists or specify another file.", tempFilename), fex);
                            throw;
                        } catch (Exception e) {
                            log.Log(LogLevel.Error, "A fatal exception has occured.", e);
                            throw;
                        }
                    } 

                } else {
                    log.Log(LogLevel.Info, "Unrecognised source type: " + source.GetType());
                }
            }

            return xmlTvData;

        }
    }
}