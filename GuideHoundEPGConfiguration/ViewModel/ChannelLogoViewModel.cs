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
    
    public class ChannelLogoViewModel : ServicesViewModelBase {
        private ICommand clearAllLogos;


        public ConfigViewModel Config { get; set; }

        public ChannelLogoViewModel() {
        }


        public ChannelLogoViewModel(ConfigViewModel configViewModel) {
            this.Config = configViewModel;
        }

        public void SetLogoFromDroppedFiles(string[] files) {

            if (files.Count() == 0) {
                return;
            }

            if (files.Count() > 1) {
                // handle multiple files?
                // try and assign automatically

                List<string> unmatchedFilenames = new List<string>();

                files.ToList().ForEach(match => unmatchedFilenames.Add(match));

                foreach (var f in files) {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(f);

                    Config.Channels.Where(c => c.ChannelName.ToUpper().WithoutSpaces() == fileName.ToUpper().WithoutSpaces()).ToList().ForEach(match => {
                        match.LogoUrl = String.Format("file://{0}", f);
                        unmatchedFilenames.Remove(f);
                    });
                }

                // note: if we have unmatched items we could possibly try and get a bit cleverer by renaming numbers to characters ("ONE" -> "1") and try
                // again. Maybe later...

                return;
            }

            string file = files.FirstOrDefault();
            if (file != null && Config.SelectedChannel != null) {
                Config.SelectedChannel.LogoUrl = String.Format("file://{0}", file);
            }

        }

        public ICommand ClearAllLogos {
            get {
                return clearAllLogos ?? (clearAllLogos = new RelayCommand(() => {
                    Config.Channels.ToList().ForEach(c => c.LogoUrl = String.Empty);
                }));
            }
        }
    }

    public static class FilenameStringExtensions {

        public static string WithoutSpaces(this string str) {
            return str.Replace(" ", "");
        }

    }

}
