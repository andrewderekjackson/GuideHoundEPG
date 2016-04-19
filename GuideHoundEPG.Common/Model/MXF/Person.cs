using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class Person {

        public Person() {
            Id = Guid.NewGuid().ToString();
        }

        public String Id { get; set; }
        public String Uid {
            get {
                return "!Person!" + Id;
            }
        }
        public String Name { get; set; }
    }
}