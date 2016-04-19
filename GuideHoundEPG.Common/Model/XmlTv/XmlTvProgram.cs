using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GuideHoundEPG.Common.Model.XmlTv {
    [DebuggerDisplay("Program: {Title}, Start: {StartTime}, StopTime: {StopTime}")]
    public class XmlTvProgram {
        
        public XmlTvProgram() {
            GenreList = new List<string>();
            CategoryList = new List<string>();
            Credits = new List<XmlTvCredit>();
        }

        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public String Channel { get; set; }
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public String Description { get; set; }

        public String EpisodeOverview { get; set; }
        public String Rating { get; set; }
        
        // video
        public String VideoQuality { get; set; }
        public String VideoPresent { get; set; }
        public String VideoAspect { get; set; }

        public List<String> GenreList { get; set; }
        public List<String> CategoryList { get; set; }
        public List<XmlTvCredit> Credits { get; set; }

        public String StarRating { get; set; }

        public int HalfStars {
            get {

                double valInt = 0;
                if (!String.IsNullOrEmpty(StarRating)) {
                    string val = StarRating.Split('/').First().Trim();
                    valInt = double.Parse(val);
                }
                return Convert.ToInt32((valInt / 2));
            }
        }

        public BseEpisodeInfo BseEpisodeInfo { get; set; }
       

    }

    public class XmlTvCredit {
        public String Role { get; set; }
        public String Person { get; set; }

        public string CreditType { get; set; }
    }

    public class BseEpisodeInfo {

        public BseEpisodeInfo(string bseEpisodeNumber) {
            BseEpisodeNumber = bseEpisodeNumber;
            Parse();
        }

        private void Parse() {

            SeriesNumber = BseEpisodeNumber.Split('.')[0].Trim();
            EpisodeNumber = BseEpisodeNumber.Split('.')[1].Trim();
        }

        public string BseEpisodeNumber { get; set; }

        public string SeriesNumber { get; set; }

        public string EpisodeNumber { get; set; }

    }
}