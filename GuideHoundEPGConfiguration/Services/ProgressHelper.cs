using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuideHoundEPG.UI.Services {
    
    public interface IProgressHelper {
        void Start();
        void Finish();

        event EventHandler OnStart;
        event EventHandler OnFinish;

    }

    public class ProgressHelper : IProgressHelper {

        public void Start() {
            if (OnStart!=null) {
                OnStart(this, EventArgs.Empty);
            }

        }

        public void Finish() {
            if (OnFinish!=null) {
                OnFinish(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnStart;
        public event EventHandler OnFinish;
    }


}
