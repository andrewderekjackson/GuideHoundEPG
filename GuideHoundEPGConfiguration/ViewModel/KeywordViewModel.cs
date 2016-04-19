using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Win7EPGDownloader.Common.Model;
using Core.UI;

namespace Win7EPGDownloader.UI.ViewModel
{
    public class KeywordViewModel : ViewModelBase<KeywordViewModel> {

        public KeywordViewModel()
            : this("", ProgramCategory.None) {
        }
        
        public KeywordViewModel(string word, ProgramCategory category) {
            this.Word = word;
            this.Category = category;
        }

        public String Word { get; set; }
        public ProgramCategory Category { get; set; }
    }
}
