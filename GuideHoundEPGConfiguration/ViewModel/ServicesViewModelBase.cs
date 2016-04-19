using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.UI;
using Core.UI.Services;
using GalaSoft.MvvmLight;
using GuideHoundEPG.UI.Services;

namespace GuideHoundEPG.UI.ViewModel {
    public class ServicesViewModelBase : Core.UI.ViewModel {

        public IProgressHelper ProgressHelper {
            get {
                return ServiceContainer.Instance.GetService<IProgressHelper>();
            }
        }

    }
}
