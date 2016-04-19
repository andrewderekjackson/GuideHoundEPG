using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace GuideHoundEPG.Common {
    public class AppVersion {

        private static Version version = Assembly.GetCallingAssembly().GetName().Version;

        public static string ShortAppVersion {
            get {
                return String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Revision);
            }
        }

        public static Version Version {
            get {
                return version;
            }
        }

        public static DateTime BuildDate {
            get {
                FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                return fileInfo.LastWriteTime;
            }
        }

    }
}
