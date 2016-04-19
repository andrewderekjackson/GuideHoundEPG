using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuideHoundEPG.Common.Model
{
    /// <summary>
    /// Trigger word for a particular catagory
    /// </summary>
    public class CategoryWordTrigger {
        
        public CategoryWordTrigger(string word, ProgramCategory category, bool removeWord = false) {
            Word = word;
            Category = category;
            RemoveWord = removeWord;
        }

        public String Word { get; set; }
        public ProgramCategory Category { get; set; }
        public bool RemoveWord { get; set; }
       
    }
}
