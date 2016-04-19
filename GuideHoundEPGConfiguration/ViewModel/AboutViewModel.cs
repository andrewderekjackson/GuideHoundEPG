using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Core.UI.Services;
using GalaSoft.MvvmLight;
using GuideHoundEPG.Common;
using GuideHoundEPG.Common.Model;
using Core.UI;
using GuideHoundEPG.UI.Controllers;
using GuideHoundEPG.UI.Services;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.ViewModel {
    public class AboutViewModel : Core.UI.ViewModel {

        public ConfigViewModel Config { get; set; }

        public AboutViewModel() {
        }

        public AboutViewModel(ConfigViewModel configViewModel) {
            this.Config = configViewModel;

        }

        public String ApplicationVersion {
            get { return AppVersion.ShortAppVersion; }
        }

    }
}
