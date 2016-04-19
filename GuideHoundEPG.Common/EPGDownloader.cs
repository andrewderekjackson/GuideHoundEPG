using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using log4net;
using Core.Logging;
using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;

namespace GuideHoundEPG.Common {
    public class EPGDownloader {

        private string sourceUrl;
        private string destinationPath;
        private static readonly ILogger logger = Logger.GetLogger();

        public EPGDownloader(string sourceUrl, string destinationPath) {
            this.sourceUrl = sourceUrl;
            this.destinationPath = destinationPath;
        }

        public bool Download() {

            WebClient client = new WebClient();
            try {

                string destinationPathTemp = Path.Combine(EnvironmentInfo.CachePath, "Download_" + Guid.NewGuid() + ".temp");
                logger.Log(LogLevel.Info, String.Format("Downloading '{0}'...", sourceUrl));

                client.DownloadFile(sourceUrl, destinationPathTemp);

                if (sourceUrl.ToLowerInvariant().EndsWith(".gz")) {
                    logger.Log(LogLevel.Info, String.Format("Decompressing...", sourceUrl));

                    using (FileStream file = File.Open(destinationPathTemp, FileMode.Open)) {

                        using (FileStream outputFile = new FileStream(destinationPath, FileMode.Create)) {
                            using (GZipStream zipStream = new GZipStream(file, CompressionMode.Decompress)) {
                                CopyStream(zipStream, outputFile);
                            }
                        }
                    }
                } else {
                    File.Copy(destinationPathTemp, destinationPath);
                }

                return true;
            } catch (WebException wex) {
                logger.Log(LogLevel.Error, wex, "Error downloading file.");
            } catch (Exception ex) {
                logger.Log(LogLevel.Error, ex, "Error downloading file.");
            }

            
            return false;
        }

        public static String GetTemporaryCacheFile() {
            return Path.Combine(EnvironmentInfo.CachePath, String.Format("xmltv_{0}.xml", Guid.NewGuid()));
        }

        public static void CopyStream(Stream input, Stream output) {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = input.Read (buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write (buffer, 0, read);
            }
        }

    }
}