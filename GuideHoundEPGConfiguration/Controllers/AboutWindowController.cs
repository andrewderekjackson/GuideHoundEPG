using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.UI;
using GuideHoundEPG.UI.Windows;
using GuideHoundEPG.UI.ViewModel;

namespace GuideHoundEPG.UI.Controllers {
    public class AboutWindowController : WindowController<AboutDialog> {

        private AboutViewModel aboutViewModel;

        public AboutWindowController(AboutViewModel model) {
            aboutViewModel = model;
        }

        public AboutViewModel AboutViewModel {
            get {
                return aboutViewModel;
            }
        }

    }
}
