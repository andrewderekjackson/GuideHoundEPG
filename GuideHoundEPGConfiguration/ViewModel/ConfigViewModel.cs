using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Core.UI;
using GalaSoft.MvvmLight;
using GuideHoundEPG.Common.Model;

namespace GuideHoundEPG.UI.ViewModel {

    public class CategoryType {
        public CategoryType(string category, string categoryName) {
            Category = category;
            CategoryName = categoryName;
        }
        public String Category { get; set; }
        public String CategoryName { get; set; }
    }

    public class ConfigViewModel : ViewModelBase {
        
        public ConfigViewModel() {

            Sources = new ObservableCollection<XmlTvSourceViewModel>();
            Keywords = new ObservableCollection<KeywordItem>();
            Channels = new ObservableCollection<TVChannelViewModel>();
            CategoryMapping = new ObservableCollection<CategoryMappingItem>();

            CategoryTypes =  new List<CategoryType>();
            CategoryTypes.Add(new CategoryType("None","None"));
            CategoryTypes.Add(new CategoryType("Series","Series"));
            CategoryTypes.Add(new CategoryType("Movie","Movie (Purple)"));
            CategoryTypes.Add(new CategoryType("News","News (Green)"));
            CategoryTypes.Add(new CategoryType("Sports","Sports (Yellow)"));
            CategoryTypes.Add(new CategoryType("Special","Special (Orange)"));

        }

        public ObservableCollection<TVChannelViewModel> Channels {
            get { return channels; }
            set {
                channels = value;
                RaisePropertyChanged("Channels");
            }
        }

        public ObservableCollection<CategoryMappingItem> CategoryMapping {
            get { return categoryMapping; }
            set {
                categoryMapping = value;
                RaisePropertyChanged("CategoryMapping");
            }
        }
        

        public List<CategoryType> CategoryTypes {
            get { return categoryTypes; }
            set { 
                categoryTypes = value; 
                RaisePropertyChanged("CategoryTypes"); 
            }
        }

        public XmlTvSourceViewModel SelectedSource {
            get { return selectedSource; }
            set { 
                selectedSource = value; 
                RaisePropertyChanged("SelectedSource");
                RaisePropertyChanged("Sources"); 
            }
        }

        public TVChannelViewModel SelectedChannel {
            get { return selectedChannel; }
            set {
                selectedChannel = value;
                RaisePropertyChanged("Channels");
                RaisePropertyChanged("SelectedChannel");
                RaisePropertyChanged("ChannelIdListForSelectedChannel");
            }
        }

        public IEnumerable<String> ChannelIdListForSelectedChannel {
            get {
                return Channels.Where(c => c.Source == SelectedChannel.Source).Select(i => i.XmlTvChannelId);
            }
        }

        public ObservableCollection<XmlTvSourceViewModel> Sources {
            get { return sources; }
            set { 
                sources = value; 
                RaisePropertyChanged("Sources"); 
            }
        }

        public string OutputFile {
            get { return outputFile; }
            set { outputFile = value; RaisePropertyChanged("OutputFile"); }
        }

        public string OutputFolder {
            get { return outputFolder; }
            set { 
                outputFolder = value; 
                RaisePropertyChanged("OutputFolder"); 
            }
        }

        private string externalCommand;
        
        public string ExternalCommand {
            get { return externalCommand; }
            set { 
                externalCommand = value; 
                RaisePropertyChanged("ExternalCommand"); 
            }
        }

        public ObservableCollection<KeywordItem> Keywords {
            get { return keywords; }
            set { 
                keywords = value; 
                RaisePropertyChanged("Keywords"); 
            }
        }

        public bool IsMovieMetadataLookupEnabled {
            get { return isMovieMetadataLookupEnabled; }
            set { 
                isMovieMetadataLookupEnabled = value; 
                RaisePropertyChanged("IsMovieMetadataLookupEnabled");
            }
        }

        public bool IsTvSeriesMetadataLookupEnabled {
            get { return isTvSeriesMetadataLookupEnabled; }
            set { 
                isTvSeriesMetadataLookupEnabled = value; 
                RaisePropertyChanged("IsTvSeriesMetadataLookupEnabled");
            }
        }

        public bool IsKeywordDetectionEnabled {
            get { return isKeywordDetectionEnabled; }
            set { 
                isKeywordDetectionEnabled = value; 
                RaisePropertyChanged("IsKeywordDetectionEnabled");
            }
        }

        public bool IsBasicDetectionEnabled {
            get { return isBasicDetectionEnabled; }
            set {
                isBasicDetectionEnabled = value;
                RaisePropertyChanged("IsBasicDetectionEnabled");
            }
        }

        public bool MovieDurationMatchingEnabled {
            get { return movieDurationMatchingEnabled; }
            set {
                movieDurationMatchingEnabled = value;
                RaisePropertyChanged("MovieDurationMatchingEnabled");
            }
        }

        public int MinimumMovieDurationInMinutes {
            get { return minimumMovieDurationInMinutes; }
            set {
                minimumMovieDurationInMinutes = value;
                RaisePropertyChanged("MinimumMovieDurationInMinutes");
            }
        }

        public int MaximumMovieDurationInMinutes {
            get { return maximumMovieDurationInMinutes; }
            set {
                maximumMovieDurationInMinutes = value;
                RaisePropertyChanged("MaximumMovieDurationInMinutes");
            }
        }

        public bool ImportIntoMediaCenter {
            get { return importIntoMediaCenter; }
            set {
                importIntoMediaCenter = value;
                RaisePropertyChanged("ImportIntoMediaCenter");
            }
        }

        public bool IsCategoryMappingEnabled {
            get { return isCategoryMappingEnabled; }
            set
            {
                isCategoryMappingEnabled = value;
                RaisePropertyChanged("IsCategoryMappingEnabled");
            }
        }

        public bool UseDefaultIfUnmatched {
            get { return useDefaultIfUnmatched; }
            set {
                useDefaultIfUnmatched = value;
                RaisePropertyChanged("UseDefaultIfUnmatched");
            }
        }

        public ProgramCategory UnmatchedCategory {
            get { return unmatchedCategory; }
            set {
                unmatchedCategory = value;
                RaisePropertyChanged("UnmatchedCategory");
            }
        }

        public bool SchedulerEnabled {
            get { return schedulerEnabled; }
            set {
                schedulerEnabled = value;
                RaisePropertyChanged("SchedulerEnabled");
            }
        }

        public DateTime SchedulerStartTime {
            get { return schedulerStartTime; }
            set {
                schedulerStartTime = value;
                RaisePropertyChanged("SchedulerStartTime");
            }
        }


        private ObservableCollection<XmlTvSourceViewModel> sources;
        private ObservableCollection<KeywordItem> keywords;
        private ObservableCollection<TVChannelViewModel> channels = new ObservableCollection<TVChannelViewModel>();
        private ObservableCollection<CategoryMappingItem> categoryMapping;
        private List<CategoryType> categoryTypes;
        private XmlTvSourceViewModel selectedSource;
        private TVChannelViewModel selectedChannel;

        private string outputFile;
        private string outputFolder;
        private bool isMovieMetadataLookupEnabled;
        private bool isTvSeriesMetadataLookupEnabled;
        private bool isKeywordDetectionEnabled;
        private bool isBasicDetectionEnabled;
        private bool movieDurationMatchingEnabled;
        private int minimumMovieDurationInMinutes;
        private int maximumMovieDurationInMinutes;
        private bool importIntoMediaCenter;
        private bool isCategoryMappingEnabled;
        private bool useDefaultIfUnmatched;
        private ProgramCategory unmatchedCategory;
        private bool schedulerEnabled;
        private DateTime schedulerStartTime;

        #region ICloneable Members

        public object Clone() {
            return this.MemberwiseClone();
        }

        #endregion
        
    }
}
