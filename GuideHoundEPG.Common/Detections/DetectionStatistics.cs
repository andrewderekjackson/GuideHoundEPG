using GuideHoundEPG.Common.Model;

namespace GuideHoundEPG.Common.Detections {
    
    public class DetectionStatistics {

        public int Movies { get; set; }

        public int News { get; set; }

        public int Sports { get; set; }

        public int Special { get; set; }

        public int Series { get; set; }

        public int Unknown { get; set; }

        public int Unchanged { get; set; }

        public void Increment(ProgramCategory programCategory) {
            switch (programCategory) {
                case ProgramCategory.Movie:
                    Movies++;
                    break;
                case ProgramCategory.News:
                    News++;
                    break;
                case ProgramCategory.Sports:
                    Sports++;
                    break;
                case ProgramCategory.Special:
                    Special++;
                    break;
                case ProgramCategory.Series:
                    Series++;
                    break;
                case ProgramCategory.None:
                    Unknown++;
                    break;
                
            }
        }
    }
}
