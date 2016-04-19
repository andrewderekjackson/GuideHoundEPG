using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuideHoundEPG.Common.Model;
using Core.UI;

namespace GuideHoundEPG.UI.ViewModel
{
    public class KeywordItem : Core.UI.ViewModel {

        public KeywordItem()
            : this("", ProgramCategory.None, false) {
        }
        
        public KeywordItem(string word, ProgramCategory category, bool removeKeyword) {
            this.Word = word;
            this.Category = category;
            this.RemoveKeyword = removeKeyword;
        }

        public String Word { get; set; }
        public ProgramCategory Category { get; set; }
        public bool RemoveKeyword { get; set; }
    }
}
