using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class Channel : IEquatable<Channel> {
        public int Id { get; set; }
        public String Uid { 
            get {
                return "!Channel!" + ( Lineup.Name ?? "") + "!" + Id;
            }
        }
        public Lineup Lineup { get; set; }
        public Service Service { get; set; }
        public int Number { get; set; }
        public int SubNumber { get; set; }

        // additional properties
        public String XmlMappedChannel { get; set; }

        public bool Equals(Channel other) {
            return this.Uid.Equals(other.Uid);
        }
    }
}