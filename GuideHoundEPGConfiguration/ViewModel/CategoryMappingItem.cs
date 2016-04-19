using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GuideHoundEPG.Common.Model;

namespace GuideHoundEPG.UI.ViewModel {
    public class CategoryMappingItem : Core.UI.ViewModel {

        public CategoryMappingItem() {
        }

        public CategoryMappingItem(string sourceCategory, ProgramCategory mappedCategory) {
            SourceCategory = sourceCategory;
            MappedCategory = mappedCategory;
        }

        public String SourceCategory { get; set; }
        public ProgramCategory MappedCategory { get; set; }
    }
}
