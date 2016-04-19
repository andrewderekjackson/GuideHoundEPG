using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Core.Logging;
using log4net;
using GuideHoundEPG.Common;

namespace GuideHoundEPG
{
    
    /// <summary>
    /// Provides interogation of cached entries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheProvider<TKey, TItem> {

        private static Func<TItem, TKey> localGetKeyFunc;
        private static readonly ILogger logger = Logger.GetLogger();

        public class CacheCollection : KeyedCollection<TKey, TItem> {
            protected override TKey GetKeyForItem(TItem item) {
                return localGetKeyFunc(item);
            }
        }

        public KeyedCollection<TKey, TItem> Collection { 
            get {
                return objectCollection;
            }
        }

        private string cachePath;
        private KeyedCollection<TKey, TItem> objectCollection;

        public CacheProvider(Func<TItem, TKey> getKeyFunc): base() {
            logger.Log(LogLevel.Debug, String.Format("{0}: Instanciating cache provider.", this.GetType().Name));
            
            localGetKeyFunc = getKeyFunc;
            
            Type type = typeof (TItem);
            cachePath = EnvironmentInfo.CachePath+"\\" + type.Name + ".xml";

            objectCollection = new CacheCollection();
        }

        public void SaveCache() {
            logger.Log(LogLevel.Debug, String.Format("{0}: Saving cache.", this.GetType().Name));

            using (FileStream fs = File.Open(cachePath, FileMode.OpenOrCreate)) {
                XmlSerializer serializer = new XmlSerializer(typeof(CacheCollection));
                serializer.Serialize(fs, objectCollection);
                fs.Close();
            }
            
        }

        public void LoadCache() {
            logger.Log(LogLevel.Debug, String.Format("{0}: Loading cache.", this.GetType().Name));

            if (File.Exists(cachePath)) {
                using (FileStream fs = File.Open(cachePath, FileMode.OpenOrCreate)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(CacheCollection));
                    objectCollection = (CacheCollection)serializer.Deserialize(fs);
                    fs.Close();
                }
            } else {
                logger.Log(LogLevel.Debug, String.Format("{0}: File does not exist at cache path.", this.GetType().Name, cachePath));
            }
        }

        public TItem FindKey(TKey keyValue) {
            if (objectCollection.Contains(keyValue)) {
                return objectCollection[keyValue];
            }
            return default(TItem);
        }

        public void Add(TItem item) {
            objectCollection.Add(item);
        }

        public void Remove(TItem item) {
            objectCollection.Remove(item);
        }
    }
}
