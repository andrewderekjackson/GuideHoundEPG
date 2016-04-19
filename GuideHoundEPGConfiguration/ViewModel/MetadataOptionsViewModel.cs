using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Core.UI;
using GalaSoft.MvvmLight;
using GuideHoundEPG.UI.Controllers;

namespace GuideHoundEPG.UI.ViewModel {

    public class MetadataOptionsViewModel : Core.UI.ViewModel {

        public ConfigViewModel Config { get; set; }

        public MetadataOptionsViewModel() {
        }

        public MetadataOptionsViewModel(ConfigViewModel configViewModel) {
            this.Config = configViewModel;
        }

    }
}
