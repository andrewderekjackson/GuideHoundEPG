using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Core.UI;
using Core.UI.Services;
using GuideHoundEPG.Common.Model;
using GuideHoundEPG.Common.Model.XmlTv;
using GuideHoundEPG.UI.Controllers;
using GuideHoundEPG.UI.Services;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.ViewModel {
    
    public class ChannelsViewModel : ServicesViewModelBase {

        public ConfigViewModel Config { get; set; }

        public ChannelsViewModel() {
        }
        
        public ChannelsViewModel(ConfigViewModel configViewModel) {
            this.Config = configViewModel;
        }

        public ICommand PopulateChannelsCommand {
            get {
                return new RelayCommand(PopulateChannelList);
            }
        }

        private void PopulateChannelList() {

            DialogResult result = MessageBox.Show("Are you sure you want to populate the channels from the source?" + Environment.NewLine + Environment.NewLine + "WARNING: This will clear your current channel mappings", "Populate channels", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) {
                return;
            }

            ProgressHelper.Start();

            Config.Channels.Clear();
            bool forceRefreshSource = true;
            bool runSourceCommands = true;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, args) => {

                IEnumerable<XmlTvSourceViewModel> sources = args.Argument as IEnumerable<XmlTvSourceViewModel>;

                SourceDownloader sourceDownloader = new SourceDownloader();
                List<SourceDownloadResult> results = sourceDownloader.DownloadSources(sources, forceRefreshSource, runSourceCommands);
                args.Result = results;
            };
            worker.RunWorkerCompleted += (s, args) => {

                List<SourceDownloadResult> results = args.Result as List<SourceDownloadResult>;

                if (results != null) {
                    // update sources

                    foreach (var source in Config.Sources) {
                        SourceDownloadResult foundResult = results.FirstOrDefault(r => r.SourceName == source.SourceName);
                        if (foundResult != null && source.SourceCache != foundResult.SourceCachePath) {
                            if (foundResult.Success) {
                                source.SourceCache = foundResult.SourceCachePath;
                                // update channels
                                IXmlTvReader xmltv = new XmlTvXSDReader();
                                List<XmlTvChannel> channelList = xmltv.LoadChannelList(source.SourceCache);

                                foreach (XmlTvChannel channel in channelList) {
                                    Config.Channels.Add(new TVChannelViewModel() { XmlTvChannelId = channel.Id, ChannelName = channel.DisplayName, LogoUrl = channel.Icon, Source = source, IsEnabled = true, ChannelId = Config.Channels.Count + 1 });
                                }

                                // fetch programs
                                List<XmlTvProgram> programList = xmltv.LoadProgramList(source.SourceCache);
                                List<string> categoryList = programList.SelectMany(p => p.CategoryList).OrderBy(p => p).Distinct().ToList();

                                foreach (var category in categoryList.Where(category => Config.CategoryMapping.Count(f => f.SourceCategory == category) == 0)) {
                                    Config.CategoryMapping.Add(new CategoryMappingItem(category, ProgramCategory.None));
                                }

                            }
                        }
                    }
                }
                ProgressHelper.Finish();
                // this.Window.Cursor = null;
            };
            worker.RunWorkerAsync(Config.Sources);

        }

    }
}
