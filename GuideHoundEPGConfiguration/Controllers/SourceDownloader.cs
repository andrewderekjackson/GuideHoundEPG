using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuideHoundEPG.UI.ViewModel;
using GuideHoundEPG.Common;
using System.Net;
using log4net;
using Core.Logging;
using System.Diagnostics;
using System.IO;

namespace GuideHoundEPG.UI.Controllers {
    
    public class SourceDownloader {
        
        private List<SourceDownloadResult> sourceDownloadResult = new List<SourceDownloadResult>();
        private static readonly ILogger logger = Logger.GetLogger();

        public SourceDownloader() {
        }

        public List<SourceDownloadResult> DownloadSources(IEnumerable<XmlTvSourceViewModel> sources, bool forceRefreshSource, bool runSourceCommands) {

            logger.Log(LogLevel.Info, "Downloading sources...");

            foreach (var source in sources.Where(e=>e.SourceEnabled)) {
                logger.Log(LogLevel.Info, String.Format("Source: {0}, {1}", source.SourceName, source.SourceUri));

                bool mustRefreshSource = true;
                if (!String.IsNullOrEmpty(source.SourceCache)) {
                    FileInfo sourceFileInfo = new FileInfo(source.SourceCache);
                    mustRefreshSource = (!sourceFileInfo.Exists || sourceFileInfo.CreationTime < DateTime.Now.AddMinutes(-15));
                }
                
                if (mustRefreshSource || forceRefreshSource) {
                    
                    if (source.CommandEnabled && !String.IsNullOrEmpty(source.CommandString) && runSourceCommands) {
                        if (!String.IsNullOrEmpty(source.CommandString)) {

                            logger.Log(LogLevel.Info, String.Format("Running source command: {0}", source.CommandString));
                            
                            using (Process exeProcess = Process.Start(source.CommandString)) {
                                exeProcess.WaitForExit();
                            }
                        }
                    }
                    logger.Log(LogLevel.Info, String.Format("Downloading source: {0}", source.SourceUri));

                    // download file
                    string sourceCache = EPGDownloader.GetTemporaryCacheFile();
                    SourceDownloadResult result = new SourceDownloadResult();

                    result.Success = true;
                    result.SourceCachePath = sourceCache;
                    result.SourceName = source.SourceName;
                    
                    try {
                        EPGDownloader downloader = new EPGDownloader(source.SourceUri, sourceCache);
                        downloader.Download();
                        result.Success = true;
                    } catch (WebException wex) {
                        logger.Log(LogLevel.Error, String.Format("Error downloading source uri: {0}.", source.SourceUri));

                        result.Success = false;
                        result.Exception = wex;
                    }
                    sourceDownloadResult.Add(result);
                }
            }
            logger.Log(LogLevel.Info, String.Format("{0} sources downloaded.", sourceDownloadResult.Count));

            return sourceDownloadResult;
        }

    }

    public class SourceDownloadResult {

        public String SourceName { get; set; }
        public String SourceCachePath { get; set; }
        public Boolean Success { get; set; }
        public Exception Exception { get; set; }

    }
}
