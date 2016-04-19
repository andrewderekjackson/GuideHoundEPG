using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class SeriesInfo : IEquatable<SeriesInfo> {
        public String Id { get; set; }
        public String Uid { 
            get {
                return "!Series!" + Id;    
            }
        }
        public String Title { get; set; }
        public String ShortTitle { get; set; }
        public String Description { get; set; }
        public String ShortDescription { get; set; }
        public DateTime StartAirDate { get; set; }
        public DateTime EndAirDate { get; set; }
        public GuideImage GuideImage { get; set; }

        // additional properties
        public String CacheKey { get; set; }
        
        /// <summary>
        /// Has this entry been filled by a sucessfull metadata lookup
        /// </summary>
        public bool IsMetadataFilled { get; set; }

        /// <summary>
        /// Has this entry been looked up against a metadata service (either successfull or not)
        /// </summary>
        public bool IsMetadataLookup { get; set; }
        

        public bool Equals(SeriesInfo other) {
            return this.Uid.Equals(other.Uid);
        }
    }
}