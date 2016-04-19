using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GuideHoundEPG.Common.Model.MXF;
using GuideHoundEPG.UI.Windows;
using Core.UI;

namespace GuideHoundEPG.UI.Controllers
{
    public class SeriesCacheEditorWindowController : WindowController<SeriesCacheEditorWindow> {
        private CacheProvider<String, SeriesInfo> seriesCache;
        private CacheProvider<String, ProgramInfo> programCache;

        public ObservableCollection<SeriesInfo> SeriesInfoList { get; set; }
        public ObservableCollection<ProgramInfo>ProgramInfoList { get; set; }


        public SeriesCacheEditorWindowController() {
           
            // data provider for cache series information
            seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);
            seriesCache.LoadCache();
            programCache = new CacheProvider<string, ProgramInfo>(e => e.Id);
            programCache.LoadCache();

            // get the list into a view model
            SeriesInfoList = new ObservableCollection<SeriesInfo>(seriesCache.Collection.ToList());
            ProgramInfoList = new ObservableCollection<ProgramInfo>(programCache.Collection.ToList());
        
        }

    }
}
