using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Core.UI.Controls {
   
    [ContentProperty("Content")]
    public class ProgressPanel : ContentControl {
        private DispatcherTimer timer;

        public static readonly DependencyProperty InProgressProperty = DependencyProperty.Register(
            "InProgress",
            typeof(bool),
            typeof(ProgressPanel),
            new UIPropertyMetadata(false, OnInProgressChanged));

        public static readonly DependencyProperty IsWaitCursorVisibleProperty = DependencyProperty.Register(
            "IsWaitCursorVisible",
            typeof(bool),
            typeof(ProgressPanel),
            new UIPropertyMetadata(false));

        public static readonly DependencyProperty ProgressTemplateProperty = DependencyProperty.Register(
            "ProgressTemplate",
            typeof(DataTemplate),
            typeof(ProgressPanel),
            new UIPropertyMetadata(null));

        static ProgressPanel() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressPanel), new FrameworkPropertyMetadata(typeof(ProgressPanel)));
        }

        public bool InProgress {
            get { return (bool)GetValue(InProgressProperty); }
            set { SetValue(InProgressProperty, value); }
        }

        public bool IsWaitCursorVisible {
            get { return (bool)GetValue(IsWaitCursorVisibleProperty); }
            set { SetValue(IsWaitCursorVisibleProperty, value); }
        }

        public DataTemplate ProgressTemplate {
            get { return (DataTemplate)GetValue(ProgressTemplateProperty); }
            set { SetValue(ProgressTemplateProperty, value); }
        }

        internal void StartInProgressTimer() {
            
            if (timer == null) {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 1);
                timer.Tick += OnTimerTick;
            }

            timer.Start();
        }

        private void StopInProgressTimer() {
            timer.Stop();
            timer.Tick -= OnTimerTick;
            timer = null;

            IsWaitCursorVisible = false;
        }

        void OnTimerTick(object sender, EventArgs e) {
            IsWaitCursorVisible = true;
        }

        public static void OnInProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ProgressPanel control = d as ProgressPanel;
            if (control == null) {
                return;
            }

            if ((bool)e.NewValue) {
                control.StartInProgressTimer();
            } else {
                control.StopInProgressTimer();
            }
        }

    }
}
