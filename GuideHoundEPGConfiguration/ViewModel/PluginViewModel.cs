using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.UI;

namespace GuideHoundEPG.UI.ViewModel {
    public class PluginViewModel : Core.UI.ViewModel {

        public String Name { get; set; }

        public Type Type { get; set; }

        public bool IsEnabled { get; set; }

        public int Order { get; set; }

    }
}
