using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace GuideHoundEPG.Common {
    
    public class EnvironmentInfo {
        
        private static string dataPath;

        static EnvironmentInfo() {
            dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EpgImporter");
        }
        
        /// <summary>
        /// Returns the installation path without ensuring the path exists.
        /// </summary>
        public static String InstallationDataPath {
            get { return dataPath; }
        }


        public static String DataPath {
            get {
                EnsurePathExists(dataPath);
                return dataPath;
            }
        }

        private static void EnsurePathExists(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        public static String CachePath {
            get {
                string cachePath = Path.Combine(DataPath, "Cache");
                EnsurePathExists(cachePath);
                return cachePath;
            }
        }

        public static String ConfigFileName {
            get { 
                return Path.Combine(EnvironmentInfo.DataPath, "config.xml");
            }
        }

        public static string OutputFolder {
            get {
                string outputPath = Path.Combine(DataPath, "Guide");
                EnsurePathExists(outputPath);
                return outputPath;
            }
        }

        public static string GetCacheSubPath(string p) {
            string path = Path.Combine(CachePath, p);
            EnsurePathExists(path);
            return path;
        }

        public static string DefaultConfigFileName {
            get {
                return Path.Combine(AppPath, "config_default.xml");   
            }
        }

        public static string LogPath {
            get {
                string path = Path.Combine(DataPath, "Logs");
                EnsurePathExists(path);
                return path;
            }
        }

        public static string MxfLoaderPath {
            get {
                return Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), @"eHome\loadmxf.exe");
            }
        }

        public static string AppPath {
            get {
                return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            }
        }
    }

}
