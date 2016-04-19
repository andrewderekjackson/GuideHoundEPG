using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Core.UI;
using GuideHoundEPG.UI.Controllers;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.ViewModel {

    public class SourcesViewModel : Core.UI.ViewModel {

        public ConfigViewModel Config { get; set; }

        public SourcesViewModel() {
            if (IsInDesignMode) {
                this.Config = new ConfigViewModel();
            }
        }

        public SourcesViewModel(ConfigViewModel configViewModel) {
            this.Config = configViewModel;
        }



        private void EditSelectedSource() {
            if (Config.SelectedSource != null) {
                SourceDialogController controller = new SourceDialogController(Config.SelectedSource);
                bool? result = controller.ShowDialog();
                if (result.HasValue && result.Value) {
                    Config.SelectedSource = controller.Source;
                }
            }
        }

        private void DeleteSelectedSource() {
            if (Config.SelectedSource != null) {
                DialogResult result = MessageBox.Show(String.Format("Are you sure you want to delete the source '{0}'?", Config.SelectedSource.SourceName), "Delete Source", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    Config.Sources.Remove(Config.SelectedSource);
                }
            }
        }

        public ICommand DeleteSourceCommand {
            get {
                return new RelayCommand(DeleteSelectedSource, () => Config.SelectedSource != null);
            }
        }

        public ICommand EditSourceCommand {
            get {
                return new RelayCommand(EditSelectedSource, () => Config.SelectedSource != null);
            }
        }

        public ICommand AddSourceCommand {
            get {
                return new RelayCommand(() =>
                {
                    
                    XmlTvSourceViewModel model = new XmlTvSourceViewModel();
                    model.SourceName = "New Source";

                    SourceDialogController controller = new SourceDialogController(model);
                    bool? result = controller.ShowDialog();
                    if (result.HasValue && result.Value) {
                        this.Config.Sources.Add(controller.Source);
                    }

                });
            }
        }

    }
}
