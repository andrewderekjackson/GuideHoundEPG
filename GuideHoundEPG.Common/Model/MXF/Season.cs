using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class Season {
        public String Id { get; set; }
        public String Uid { get; set; }
        public SeriesInfo Series { get; set; }
        public String Title { get; set; }
        public String Studio { get; set; }
        public int Year { get; set; }
        public GuideImage GuideImage { get; set; }
    }
}