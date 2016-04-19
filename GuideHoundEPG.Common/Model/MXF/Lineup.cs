using System;

namespace GuideHoundEPG.Common.Model.MXF {
    
    public class Lineup : IEquatable<Lineup> {

        /// <summary>
        /// Returns a new primary lineup
        /// </summary>
        public static Lineup Primary {
            get {
                return new Lineup() {
                    Id = "l1",
                    Name = "Main Lineup",
                    PrimaryProvider = "!MCLineup!MainLineup",
                    IsPrimaryLineup = true
                };
            }
        }

        public String Id { get; set; }
        public String Uid {
            get {
                return "!Lineup!" + Id;
            }
        }
        public String Name { get; set; }
        public String PrimaryProvider { get; set; }

        public bool IsPrimaryLineup { get; set; }

        public bool Equals(Lineup other) {
            return this.Uid.Equals(other.Uid);
        }
    }
}