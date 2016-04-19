using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Core.UI;
using GuideHoundEPG.UI.Controllers;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.ViewModel {

    public class OutputOptionsViewModel : Core.UI.ViewModel {

        public ConfigViewModel Config { get; set; }

        public OutputOptionsViewModel() {
        }

        public OutputOptionsViewModel(ConfigViewModel configViewModel) {
            this.Config = configViewModel;
        }

        public ICommand LaunchWebsiteCommand {
            get {
                return new RelayCommand(() => Process.Start("http://www.tvguidehound.com/GettingStarted/"));
            }
        }
    }
}
