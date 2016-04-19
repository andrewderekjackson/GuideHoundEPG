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
using GuideHoundEPG.UI.ViewModel;

namespace GuideHoundEPG.UI.Views {
    /// <summary>
    /// Interaction logic for ChannelLogoView.xaml
    /// </summary>
    public partial class ChannelLogoView : UserControl {
        public ChannelLogoView() {
            InitializeComponent();
        }

        private void itemRoot_DragOver(object sender, DragEventArgs e) {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effects = DragDropEffects.None;
            }


            ListBoxItem parent = UIHelper.TryFindParent<ListBoxItem>((DependencyObject)sender);
            if (parent != null) {
                parent.IsSelected = true;
            }

        }

        private void itemRoot_Drop(object sender, DragEventArgs e) {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                return;
            }

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            (DataContext as ChannelLogoViewModel).SetLogoFromDroppedFiles(files);

            
        
        }
    }
}
