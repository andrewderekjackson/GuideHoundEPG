using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using GuideHoundEPG.UI.ViewModel;
using GuideHoundEPG.UI.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using Core.UI;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.Controllers
{
    class SourceDialogController : WindowController<SourceDialog> {

        public XmlTvSourceViewModel Source { get; set; }

        public SourceDialogController(XmlTvSourceViewModel viewModel) {
            this.Source = viewModel;
        }

        public override void OnBeforeShow() {
            base.OnBeforeShow();

            Window.SaveButton.Click += OnSaveButtonClick;
            Window.CancelButton.Click += OnCancelButtonClick;

            this.Window.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(OnErrorReceived));

        }

        public void OnErrorReceived(Object sender, RoutedEventArgs e) {

        }

        void OnSaveButtonClick(object sender, System.Windows.RoutedEventArgs e) {
            this.Window.DialogResult = true; 
        }

        void OnCancelButtonClick(object sender, System.Windows.RoutedEventArgs e) {
            this.Window.DialogResult = false;
        }

        public ICommand OpenSourceFileCommand {
            get {
                return new RelayCommand(() =>
                {

                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.DefaultExt = ".xml";
                    dialog.Filter = "Xml files|*.xml|All files|*.*";
                    dialog.Title = "Browse for Source File";

                    if (dialog.ShowDialog()??false) {
                        Source.SourceUri = dialog.FileName;
                    }

                });
            }
        }
      
    }
}
