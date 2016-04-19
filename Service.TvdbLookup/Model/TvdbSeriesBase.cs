using System;
using System.Collections.Generic;
using System.Drawing;

namespace Services.TvdbLookup.Model {
    
    public class TvdbSeriesBase {

        public String SeriesId { get; set; }
        public String Language { get; set; }
        public String SeriesName { get; set; }
        public DateTime FirstAired { get; set; }
        public String TvdbId { get; set; }

        public String Banner { get; set; }
        public String ImdbId { get; set; }
        public String Overview { get; set; }

    }

    public class TvdbSeriesFull {
        
        public TvdbSeriesFull() {
            Genres = new List<TvdbGenre>();
        }

        public List<TvdbGenre> Genres { get; set; }
        public Bitmap PosterImage { get; set; }
        public String PosterUrl { get; set; }
    }

    public class TvdbGenre {
        public String GenreName { get; set; }
    }


}