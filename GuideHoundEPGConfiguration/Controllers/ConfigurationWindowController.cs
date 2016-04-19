using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Core.UI.Services;
using GuideHoundEPG.Common.Configuration;
using GuideHoundEPG.Common.Model.Configuration;
using GuideHoundEPG.UI.Services;
using GuideHoundEPG.UI.ViewModel;
using GuideHoundEPG.UI.Windows;
using Channel = GuideHoundEPG.Common.Model.Configuration.Channel;
using GuideHoundEPG.Common;
using System.ComponentModel;
using Core.Logging;
using System.Diagnostics;
using Core.UI;
using Action = System.Action;
using MessageBox = System.Windows.Forms.MessageBox;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.Controllers
{
    public class ConfigurationWindowController : WindowController<ConfigurationWindow> {
        
        private static readonly ILogger logWriter = Logger.GetLogger();
        private bool inProgress;

        private SourcesViewModel sourcesViewModel;
        private ChannelsViewModel channelsViewModel;
        private AboutViewModel aboutViewModel;
        private SchedulerOptionsViewModel schedulerOptionsViewModel;
        private ConfigViewModel config;
        private CategoryMappingViewModel categoryMappingViewModel;
        private CategoryDetectionViewModel categoryDetectionViewModel;
        private OutputOptionsViewModel outputOptionsViewModel;
        private MetadataOptionsViewModel metadataOptionsViewModel;
        private ChannelLogoViewModel channelLogoViewModel;

        #region ViewModels 

        public ConfigViewModel Config {
            get { return config; }
            set { config = value; RaisePropertyChanged("Config"); }
        }

      
        public SourcesViewModel SourcesViewModel {
            get { return sourcesViewModel; }
            set { 
                sourcesViewModel = value; 
                RaisePropertyChanged("SourcesViewModel"); 
            }
        }

        public ChannelsViewModel ChannelsViewModel {
            get { return channelsViewModel; }
            set {
                channelsViewModel = value;
                RaisePropertyChanged("ChannelsViewModel");
            }
        }

        public AboutViewModel AboutViewModel {
            get { return aboutViewModel; }
            set {
                aboutViewModel = value;
                RaisePropertyChanged("AboutViewModel");
            }
        }

        public SchedulerOptionsViewModel SchedulerOptionsViewModel {
            get { return schedulerOptionsViewModel; }
            set {
                schedulerOptionsViewModel = value;
                RaisePropertyChanged("SchedulerOptionsViewModel");
            }
        }

        public CategoryMappingViewModel CategoryMappingViewModel {
            get { return categoryMappingViewModel; }
            set {
                categoryMappingViewModel = value;
                RaisePropertyChanged("CategoryMappingViewModel");
            }
        }

        public CategoryDetectionViewModel CategoryDetectionViewModel {
            get { return categoryDetectionViewModel; }
            set {
                categoryDetectionViewModel = value;
                RaisePropertyChanged("CategoryDetectionViewModel");
            }
        }

        public OutputOptionsViewModel OutputOptionsViewModel {
            get { return outputOptionsViewModel; }
            set {
                outputOptionsViewModel = value;
                RaisePropertyChanged("OutputOptionsViewModel");
            }
        }

        public MetadataOptionsViewModel MetadataOptionsViewModel {
            get { return metadataOptionsViewModel; }
            set {
                metadataOptionsViewModel = value;
                RaisePropertyChanged("MetadataOptionsViewModel");
            }
        }

        public ChannelLogoViewModel ChannelLogoViewModel {
            get { return channelLogoViewModel; }
            set {
                channelLogoViewModel = value;
                RaisePropertyChanged("ChannelLogoViewModel");
            }
        }

        #endregion

        public bool InProgress {
            get { return inProgress; }
            set { 
                inProgress = value; 
                RaisePropertyChanged("InProgress"); 
            }
        }

        public override void OnWindowClosing(object sender, CloseWindowCancelEventArgs e) {

            if (e.CloseReason == WindowCloseReason.FromCode) {
                return;
            }

            DialogResult result = MessageBox.Show("Would you like to save before you quit?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) {
                SaveConfiguation();
            } else if (result == DialogResult.Cancel) {
                e.Cancel = true;
            }

        }

        private void RunImport() {
            
            // save changes?
            DialogResult result = MessageBox.Show("Would you like to save your current configuration first?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) {
                SaveConfiguation();
            } else if (result == DialogResult.Cancel) {
                return;
            }

            // validation
            if (Config.Sources.Count == 0) {
                MessageBox.Show("You have no sources set up. Please add one and try again.", "No Sources", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Config.Channels.Count == 0) {
                MessageBox.Show("You have no channels set up. Please add one and try again.", "No Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string pathToCommand = Path.Combine(EnvironmentInfo.AppPath, "GuideHoundEPG.exe");

            ProgressHelper.Start();
            

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, args) => {
                using (Process exeProcess = Process.Start(pathToCommand)) {
                    exeProcess.WaitForExit();
                }       
            };
            worker.RunWorkerCompleted += (s, args) => {
                ProgressHelper.Finish();
            };
            worker.RunWorkerAsync();

        }
       

        public void PerformActionWithProgress(Action action) {
            action();
        }


        public ICommand RunImportCommand {
            get {
                return new RelayCommand(RunImport);
            }
        }

        public ICommand CloseCommand {
            get {
                return new RelayCommand(Close);
            }
        }

        public ICommand OpenLogFolderCommand {
            get {
                return new RelayCommand(() =>
                {

                    try {
                        Process.Start(new ProcessStartInfo() { FileName = EnvironmentInfo.LogPath });
                    } catch (Exception) {
                        MessageBox.Show("Error opening log folder.", "Error opening folder.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                });
            }
        }

        public ICommand OpenOutputFolderCommand {
            get {
                return new RelayCommand(() =>
                {
                    
                    try {
                        Process.Start(new ProcessStartInfo() { FileName = EnvironmentInfo.DataPath });
                    } catch (Exception) {
                        MessageBox.Show("Error opening output folder.", "Error opening folder.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                });
            }
        }

        public ICommand SaveCommand {
            get {
                return new RelayCommand(() => {
                    SaveConfiguation();
                    MessageBox.Show("Configuration saved.", "Saved", MessageBoxButtons.OK);
                });
            }
        }

        public IProgressHelper ProgressHelper {
            get {
                return ServiceContainer.Instance.GetService<IProgressHelper>();
            }
        }

        public ICommand AboutCommand {
            get
            {
                return new RelayCommand(() => {

                    AboutWindowController about = new AboutWindowController(AboutViewModel);
                    about.ShowDialog();

                });
            }
        }

        public ConfigurationWindowController() {
            
            LoadConfiguration();

            SourcesViewModel = new SourcesViewModel(Config);
            ChannelsViewModel = new ChannelsViewModel(Config);
            AboutViewModel = new AboutViewModel(Config);
            SchedulerOptionsViewModel = new SchedulerOptionsViewModel(Config);
            CategoryMappingViewModel = new CategoryMappingViewModel(Config);
            CategoryDetectionViewModel = new CategoryDetectionViewModel(Config);
            MetadataOptionsViewModel = new MetadataOptionsViewModel(Config);
            OutputOptionsViewModel = new OutputOptionsViewModel(Config);
            ChannelLogoViewModel = new ViewModel.ChannelLogoViewModel(Config);

            ProgressHelper.OnStart += (s,e) => {
                InProgress = true;
                this.Window.Cursor = System.Windows.Input.Cursors.Wait;
            };
            ProgressHelper.OnFinish += (s, e) => {
                InProgress = false;
                this.Window.Cursor = null;
            };

           

        }

        private void LoadConfiguration() {
            logWriter.Log(LogLevel.Info, "Reading configuration file");

            ConfigurationProvider configurationProvider = new ConfigurationProvider();
            EPGDownloaderConfig configFile = configurationProvider.LoadConfigurationFile();
            if (configFile == null) {
                configFile = configurationProvider.CreateDefault();
            }

            config = new ConfigViewModel();
            foreach (var source in configFile.Sources) {
                logWriter.Log(LogLevel.Info, "Reading source: " + source.SourceName);

                XmlSource xmltvSource = source as XmlSource;
                if (xmltvSource != null) {
                    XmlTvSourceViewModel xmlTvSourceViewModel = new XmlTvSourceViewModel() {
                        SourceName = xmltvSource.SourceName,
                        SourceUri = xmltvSource.SourceUri,
                        CommandEnabled = xmltvSource.CommandEnabled,
                        CommandString = xmltvSource.Command,
                        SourceEnabled = xmltvSource.SourceEnabled,
                        CommandStartFolder = xmltvSource.CommandStartFolder
                    };

                    foreach (var channel in xmltvSource.Channels) {
                        logWriter.Log(LogLevel.Info, "Adding channel: " + channel.Name);
                        Config.Channels.Add(new TVChannelViewModel() { ChannelId = channel.Id, ChannelName = channel.Name, XmlTvChannelId = channel.XmlTvId, LogoUrl = channel.LogoUrl, Source = xmlTvSourceViewModel, IsAudio = channel.IsAudio, IsEnabled = channel.IsEnabled});
                    }
                    config.Sources.Add(xmlTvSourceViewModel);
                }
            }

            config.IsMovieMetadataLookupEnabled = configFile.MetadataOptions.MovieMetadataServiceEnabled;
            config.ImportIntoMediaCenter = configFile.Output.ImportIntoMediaCenter;
            config.IsTvSeriesMetadataLookupEnabled = configFile.MetadataOptions.TvSeriesMetadataServiceEnabled;
            config.IsKeywordDetectionEnabled = configFile.CategoryDetection.KeywordDetectionEnabled;
            config.IsBasicDetectionEnabled = configFile.CategoryDetection.BasicDetectionEnabled;
            config.MovieDurationMatchingEnabled = configFile.CategoryDetection.MovieDurationMatchingEnabled;
            config.MaximumMovieDurationInMinutes = configFile.CategoryDetection.MaximumMovieDurationInMinutes;
            config.MinimumMovieDurationInMinutes = configFile.CategoryDetection.MinimumMovieDurationInMinutes;
            config.IsCategoryMappingEnabled = configFile.CategoryMapping.CategoryMappingEnabled;
            config.UseDefaultIfUnmatched = configFile.CategoryDetection.UseDefaultIfUnmatched;
            config.UnmatchedCategory = configFile.CategoryDetection.UnmatchedCategory;
            config.SchedulerEnabled = configFile.SchedulerOptions.SchedulerEnabled;
            config.SchedulerStartTime = configFile.SchedulerOptions.StartTime;


            config.CategoryMapping = new ObservableCollection<CategoryMappingItem>();
            foreach (var categoryMap in configFile.CategoryMapping.CategoryMap) {
                config.CategoryMapping.Add(new CategoryMappingItem(categoryMap.SourceCategory, categoryMap.MappedCategory));
            }

            config.Keywords = new ObservableCollection<KeywordItem>();
            foreach (var categoryKeyword in configFile.CategoryDetection.Keywords) {
                config.Keywords.Add(new KeywordItem(categoryKeyword.Word, categoryKeyword.Category, categoryKeyword.RemoveKeyword));
            }
        }

        private void SaveConfiguation() {
            ConfigurationProvider provider = new ConfigurationProvider();
            EPGDownloaderConfig configFile = new EPGDownloaderConfig();

            foreach (var xmlTvSourceViewModel in config.Sources) {

                XmlSource xmlSource = new XmlSource() {
                    SourceName = xmlTvSourceViewModel.SourceName,
                    SourceUri = xmlTvSourceViewModel.SourceUri,
                    Command = xmlTvSourceViewModel.CommandString,
                    SourceEnabled = xmlTvSourceViewModel.SourceEnabled,
                    CommandEnabled = xmlTvSourceViewModel.CommandEnabled,
                    CommandStartFolder = xmlTvSourceViewModel.CommandStartFolder
                };

                foreach (var channel in Config.Channels.Where(c => c.Source == xmlTvSourceViewModel)) {
                    xmlSource.Channels.Add(new Channel() { Id = channel.ChannelId, Name = channel.ChannelName, XmlTvId = channel.XmlTvChannelId, LogoUrl = channel.LogoUrl, IsAudio = channel.IsAudio, IsEnabled = channel.IsEnabled});
                }
                configFile.Sources.Add(xmlSource);
            }

            configFile.MetadataOptions.MovieMetadataServiceEnabled = config.IsMovieMetadataLookupEnabled;
            configFile.MetadataOptions.TvSeriesMetadataServiceEnabled = config.IsTvSeriesMetadataLookupEnabled;
            configFile.CategoryDetection.KeywordDetectionEnabled = config.IsKeywordDetectionEnabled;
            configFile.Output.ImportIntoMediaCenter = config.ImportIntoMediaCenter;
            configFile.CategoryDetection.BasicDetectionEnabled = config.IsBasicDetectionEnabled;
            configFile.CategoryDetection.MaximumMovieDurationInMinutes = config.MaximumMovieDurationInMinutes;
            configFile.CategoryDetection.MinimumMovieDurationInMinutes = config.MinimumMovieDurationInMinutes;
            configFile.CategoryDetection.MovieDurationMatchingEnabled = config.MovieDurationMatchingEnabled;
            configFile.CategoryMapping.CategoryMappingEnabled = config.IsCategoryMappingEnabled;
            configFile.CategoryDetection.UseDefaultIfUnmatched = config.UseDefaultIfUnmatched;
            configFile.CategoryDetection.UnmatchedCategory = config.UnmatchedCategory;
            configFile.SchedulerOptions.SchedulerEnabled = config.SchedulerEnabled;
            configFile.SchedulerOptions.StartTime = config.SchedulerStartTime;


            foreach (var categoryKeyword in config.Keywords) {
                configFile.CategoryDetection.Keywords.Add(new CategoryKeyword() { Word = categoryKeyword.Word, Category = categoryKeyword.Category, RemoveKeyword = categoryKeyword.RemoveKeyword});
            }

            foreach (var categoryMapping in config.CategoryMapping) {
                configFile.CategoryMapping.CategoryMap.Add(new CategoryMap() { SourceCategory  = categoryMapping.SourceCategory, MappedCategory = categoryMapping.MappedCategory });
            }

            provider.SaveConfigurationFile(configFile);
        }
        
    }
}
 