using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class Affiliate : IEquatable<Affiliate> {
        public String Name { get; set; }
        public String Uid {
            get {
                return "!Affiliate!" + this.Name;                
            }
        } 
        public GuideImage LogoImage { get; set; }

        public bool Equals(Affiliate other) {
            return this.Uid.Equals(other.Uid);
        }
    }
}