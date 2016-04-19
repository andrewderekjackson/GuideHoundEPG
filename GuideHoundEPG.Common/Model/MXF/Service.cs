using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class Service : IEquatable<Service> {

        public String Id { get; internal set; }
        public String Uid { 
            get {
                return "!Service!" + this.Id;
            } 
        }
        public String Name { get; set; }
        public String CallSign { 
            get {
                return this.Name;
            } 
        }
        public Affiliate Affiliate { get; set; }
        public GuideImage LogoImage { get; set; }

        public bool Equals(Service other) {
            return this.Uid.Equals(other.Uid);
        }
    }
}