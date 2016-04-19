using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Core.UI {

    /// <summary>
    /// Base class for window controllers. 
    /// </summary>
    public abstract class WindowController<T> : INotifyPropertyChanged where T : Window, new()  {

        private T window = null;
        private WindowCloseReason closeReason;

        /// <summary>
        /// Gets the instance of the window.
        /// </summary>
        /// <value>The window.</value>
        public T Window { 
            get { return window; } 
        }

        /// <summary>
        /// Occurs before the windows is created.
        /// </summary>
        public virtual void OnBeforeShow() {
            
        }

        /// <summary>
        /// Ovvurs after the window is created.
        /// </summary>
        public virtual void OnAfterShow() {
            
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        public void Show() {
            
            window = new T();
            window.Closing += OnWindowClosingInternal;
            window.Loaded += OnWindowLoaded;
            window.DataContext = this;

            OnBeforeShow();
            window.Show();

        }

        public void Close() {
            closeReason = WindowCloseReason.FromCode;
            window.Close();
        }

        
        public void OnWindowClosingInternal(object sender, CancelEventArgs e) {
            
            CloseWindowCancelEventArgs args = new CloseWindowCancelEventArgs(e.Cancel, closeReason);

            OnWindowClosing(sender, args);

            e.Cancel = args.Cancel;
        }
        
        public virtual void OnWindowClosing(object sender, CloseWindowCancelEventArgs e) {
          
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            window.Loaded -= OnWindowLoaded;

            // add hook to recieve win32 window messages
            HwndSource source = (HwndSource)PresentationSource.FromDependencyObject(this.window);
            source.AddHook(DetermineWindowCloseReason);
        }

        private IntPtr DetermineWindowCloseReason(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case 0x11:
                case 0x16:
                    closeReason = WindowCloseReason.WindowsShutdown;
                    break;
                case 0x112:
                    if ((LOWORD((int)wParam) & 0xfff0) == 0xf060)
                        closeReason = WindowCloseReason.UserShutdown;
                    break;
            }
            return IntPtr.Zero;
        }

        private static int LOWORD(int n) {
            return (n & 0xffff);
        }

        /// <summary>
        /// Shows the window as a modal dialog.
        /// </summary>
        /// <returns></returns>
        public bool? ShowDialog() {
            window = new T();
            window.DataContext = this;

            OnBeforeShow();
            return window.ShowDialog();
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Sends the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
