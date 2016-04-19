using System;
using System.Collections.Generic;

namespace GuideHoundEPG.Common.Model.MXF {
    public class KeywordGroup {
        public KeywordGroup() {
            Keywords = new List<Keyword>();
        }
        public String GroupName { get; set; }
        public String Uid { 
            get {
                return "!KeywordGroup!" + GroupName;
            } 
        }
        public List<Keyword> Keywords { get; set; }
    }
}