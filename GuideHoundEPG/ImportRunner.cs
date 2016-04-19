using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using GuideHoundEPG.Common.Detections;
using Channel = GuideHoundEPG.Common.Model.MXF.Channel;
using Provider = GuideHoundEPG.Common.Model.MXF.Provider;
using GuideHoundEPG.Common.Model.Configuration;
using GuideHoundEPG.Common.Model.XmlTv;
using System.IO;
using GuideHoundEPG.Common.Model.MXF;
using GuideHoundEPG.Common.Model;
using System.Reflection;
using System.Diagnostics;
using GuideHoundEPG;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;
using Core.Logging;

namespace GuideHoundEPG.Common {
    public sealed class ImportRunner {

        private EPGDownloaderConfig config;
        private ILogger log;
        
        public ImportRunner(EPGDownloaderConfig config, ILogger log) {
            this.config = config;
            this.log = log;
        }


        public void Import() {

            // load up our caches
            CacheProvider<String, SeriesInfo> seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);
            seriesCache.LoadCache();
            CacheProvider<String, Keyword> keywordCache = new CacheProvider<string, Keyword>(e => e.Word);
            keywordCache.LoadCache();
            CacheProvider<String, Person> personCache = new CacheProvider<string, Person>(e => e.Name);
            personCache.LoadCache();

            // process our sources and return the xmltv data
            EPGSourceReader reader = new EPGSourceReader(config.Sources);
            XmlTvData xmlTvData = reader.ReadSources();

            if (xmlTvData.Programs.Count==0) {
                log.Log(LogLevel.Info, "No programs found to import - aborting.");
                return;
            }

            // map our xmltv model data to mxf model data
            XmlTvToMxfMapper mapper = new XmlTvToMxfMapper();
            MxfFile mxfFile = mapper.Map(xmlTvData, config.CategoryMapping, seriesCache, keywordCache, personCache);

            // run our detection and lookup functions

            TvdbFiller tvdbFiller = new TvdbFiller();
            MoviedbFiller moviedbFiller = new MoviedbFiller();

            foreach (var provider in mxfFile.Providers) {

                // CATEGORY DETECTION
                if (config.CategoryDetection.KeywordDetectionEnabled) {

                    log.Log(LogLevel.Info, "Performing category detection...");

                    // build category word list
                    List<CategoryWordTrigger> triggerWords = new List<CategoryWordTrigger>();
                    if (config.CategoryDetection != null && config.CategoryDetection.KeywordDetectionEnabled) {
                        triggerWords =
                            config.CategoryDetection.Keywords.Select(e => new CategoryWordTrigger(e.Word, e.Category, e.RemoveKeyword)).
                                ToList();
                    }

                    KeywordCategoryDetection keywordDetection = new KeywordCategoryDetection(provider.Programs, provider, triggerWords, seriesCache, config);
                    keywordDetection.Detect();

                }


                if (config.MetadataOptions.MovieMetadataServiceEnabled) {
                    log.Log(LogLevel.Info, "Looking up movie metadata...");

                    // MOVIE LOOKUP

                    // get a list of movies
                    var movies = provider.Programs.Where(e => e.IsMovie);

                    // get a list of unique titles to lookup
                    var moviesToLookup =
                        movies.Where(e => !e.IsMetadataLookup).GroupBy(e => e.Title).Select(e => e.First()).OrderBy(
                            e => e.Title);

                    foreach (var movieProgramInfo in moviesToLookup) {
                        log.Log(LogLevel.Info, String.Format("Looking up movie '{0}'...", movieProgramInfo.Title));

                        // try and fill using movie db
                        ProgramInfo filledProgramInfo = moviedbFiller.Fill(movieProgramInfo.Title, movieProgramInfo);

                        // update all program entries with that title.
                        var moviesToUpdate = movies.Where(e => e.Title == movieProgramInfo.Title);
                        log.Log(LogLevel.Info, String.Format("Updating {0} program enties...", moviesToUpdate.Count()));

                        foreach (var movieToUpdate in moviesToUpdate) {
                            movieToUpdate.Year = filledProgramInfo.Year;
                            movieToUpdate.GuideImage = filledProgramInfo.GuideImage;
                            if (movieToUpdate.GuideImage != null &&
                                !provider.GuideImages.Contains(movieToUpdate.GuideImage)) {
                                provider.GuideImages.Add(movieToUpdate.GuideImage);
                            }
                            movieToUpdate.IsMetadataLookup = true;
                        }

                    }
                }

                if (config.MetadataOptions.TvSeriesMetadataServiceEnabled) {
                    log.Log(LogLevel.Info, "Looking up TV Series Metadata...");

                    // TV SERIES LOOKUP
                    var series =
                        provider.Series.Where(e => !e.IsMetadataLookup).OrderBy(e => e.Title);

                    foreach (var seriesProgramInfo in series) {
                        log.Log(LogLevel.Info, String.Format("Looking up series '{0}'...", seriesProgramInfo.CacheKey));

                        // try and fill using movie db
                        tvdbFiller.Fill(seriesProgramInfo.CacheKey, seriesProgramInfo);

                        if (seriesProgramInfo.GuideImage != null &&
                            !provider.GuideImages.Contains(seriesProgramInfo.GuideImage)) {
                            provider.GuideImages.Add(seriesProgramInfo.GuideImage);
                        }

                        // update programs to use the new guide image (if required)
                        if (seriesProgramInfo.GuideImage != null) {
                            SeriesInfo info = seriesProgramInfo;
                            provider.Programs.Where(e => e.Series != null && e.Series.Id == info.Id).ToList().ForEach(
                                e => {
                                    e.GuideImage = info.GuideImage;
                                });
                        }
                        seriesProgramInfo.IsMetadataLookup = true;
                    }
                }
                
                // dump the statistics
                DumpPrograms(log, provider);

            }


            string outputFolder = EnvironmentInfo.OutputFolder;
            string outputFile = "tvguide.xml";
            string mxfOutputFile = Path.Combine(outputFolder, outputFile);
            string mxfOutputImportedFile = Path.Combine(outputFolder, "tvguide_imported.xml");

            MxfWriter mxfWriter = new MxfWriter(mxfFile, outputFolder, outputFile);
            mxfWriter.Save();

            //// serialize for later
            //using (XmlWriter xmlWriter = XmlWriter.Create(mxfOutputFileBin)) {
            //    DataContractSerializer ser = new DataContractSerializer(typeof(MxfFile));
            //    ser.WriteObject(xmlWriter, mxfFile);
            //    xmlWriter.Flush();
            //}

            // write out ImportIntoMediaCenter.bat
            string importFile = Path.Combine(outputFolder, "ImportIntoMediaCenter.bat");
            string content = EnvironmentInfo.MxfLoaderPath + " -i " + outputFile;

            FileStream importFileStream = new FileStream(importFile, FileMode.Create);
            importFileStream.Write(Encoding.ASCII.GetBytes(content), 0, content.Length);
            importFileStream.Close();

            // persist cache
            seriesCache.SaveCache();
            keywordCache.SaveCache();
            personCache.SaveCache();


            // output options
            if (config.Output.ImportIntoMediaCenter) {
                log.Log(LogLevel.Info, "Importing guide data into Windows Media Center...");
                
                ProcessStartInfo startInfo = new ProcessStartInfo(EnvironmentInfo.MxfLoaderPath);
                startInfo.Arguments = " -i " + mxfOutputFile;
                startInfo.UseShellExecute = false;

                Process readProcess = new Process();
                readProcess.StartInfo = startInfo;
                readProcess.Start();
                readProcess.WaitForExit();

                // rename imported guide file to something else
                try {
                    log.Log(LogLevel.Debug, "Renaming imported guide file to alternative name, deleting existing files if required.");
                    File.Delete(mxfOutputImportedFile);
                    File.Move(mxfOutputFile, mxfOutputImportedFile);
                } catch (Exception ex) {
                    log.Log(LogLevel.Debug, ex, "Unable to rename guide file to new filename.");
                }

                log.Log(LogLevel.Info, "");
            } else {
                log.Log(LogLevel.Info, "Importing into Windows Media Center is not enabled.");
                log.Log(LogLevel.Info, String.Format("You can perform a manual import by running '{0}'.", importFile));
            }

            log.Log(LogLevel.Info, String.Format("Done."));
        }

        ///// <summary>
        ///// Identify movies based on their duration
        ///// </summary>
        //private static bool IsMovieBasedOnDuration(ProgramInfo program, int minDuration, int maxDuration) {
        //    return (program.DurationInMinutes >= minDuration && program.DurationInMinutes <= maxDuration);
        //}

         

        private static void DumpPrograms(ILogger log, Provider provider) {
            // display summary

            log.Log(LogLevel.Info, String.Format("--------------------------"));
            log.Log(LogLevel.Info, String.Format("Total Programs : {0}", provider.Programs.Count()));
            log.Log(LogLevel.Info, String.Format(" Movies        : {0}", provider.Programs.Where(e => e.IsMovie).Count()));
            log.Log(LogLevel.Info, String.Format(" Series        : {0}", provider.Programs.Where(e => e.IsSeries).Count()));
            log.Log(LogLevel.Info, String.Format("  * News       : {0}", provider.Programs.Where(e => e.IsNews).Count()));
            log.Log(LogLevel.Info, String.Format("  * Sports     : {0}", provider.Programs.Where(e => e.IsSports).Count()));
            log.Log(LogLevel.Info, String.Format("  * Special    : {0}", provider.Programs.Where(e => e.IsSpecial).Count()));
            log.Log(LogLevel.Info, String.Format("--------------------------"));
        }


    }
}
