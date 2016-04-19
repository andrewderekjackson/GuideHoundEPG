using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Core.UI {
    public class CloseWindowCancelEventArgs : CancelEventArgs {

        public CloseWindowCancelEventArgs()
            : base() {
        }

        public CloseWindowCancelEventArgs(bool cancel)
            : base(cancel) {
        }

        public CloseWindowCancelEventArgs(bool cancel, WindowCloseReason closeReason)
            : base(cancel) {
                
            this.CloseReason = closeReason;
        }

        public WindowCloseReason CloseReason { get; set; }
    }
}
