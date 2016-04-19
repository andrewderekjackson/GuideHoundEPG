using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class GuideImage : IEquatable<GuideImage>  {
        
        public GuideImage() {
            Id = Guid.NewGuid().ToString();
        }

        public GuideImage(string imageUrl) : this() {
            this.ImageUrl = imageUrl;
        }

        public String Id { get; set; }
        public String Uid { 
            get {
                return "!Image!" + Id;    
            }
        }

        public String ImageUrl { get; set; }

        // other properties
        public GuideImageType OutputImageType { get; set; }
        public String SourceUrl { get; set; }


        public bool Equals(GuideImage other) {
            return this.Uid.Equals(other.Uid);
        }
    }
}