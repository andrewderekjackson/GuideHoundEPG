using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Core.UI {
    
    /// <summary>
    /// Base class for view model objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ViewModel : GalaSoft.MvvmLight.ViewModelBase, ICloneable {
    
        #region ICloneable Members

        object ICloneable.Clone() {
            return this.MemberwiseClone();
        }

        #endregion

    }
}
