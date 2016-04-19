using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.MoviedbLookup.Model
{
    public class MoviedbResult {

        //<movie>
        //<score>1.0</score>
        //<popularity>1</popularity>
        //<title>Funny Face</title>
        //<alternative_title>Funny Face (1957)</alternative_title>
        //<type>movie</type>
        //<id>13320</id>
        //<imdb>tt0050419</imdb>
        //<url>http://www.themoviedb.org/movie/13320</url>
        //<short_overview>A shy Greenwich Village book clerk is discovered by a fashion photographer and whisked off to Paris where she becomes a reluctant model.</short_overview>
        //<release>1957-02-13</release>
        //<poster size="original">http://images.themoviedb.org/posters/43184/ffs.jpg</poster>
        //<poster size="thumb">http://images.themoviedb.org/posters/43184/ffs_thumb.jpg</poster>
        //<poster size="mid">http://images.themoviedb.org/posters/43184/ffs_mid.jpg</poster>
        //<poster size="cover">http://images.themoviedb.org/posters/43184/ffs_cover.jpg</poster>
        //<backdrop size="original">http://images.themoviedb.org/backdrops/9665/funnyface.jpg</backdrop>
        //<backdrop size="mid">http://images.themoviedb.org/backdrops/9665/funnyface_poster.jpg</backdrop>
        //<backdrop size="thumb">http://images.themoviedb.org/backdrops/9665/funnyface_thumb.jpg</backdrop>
        //</movie>

        public String Score { get; set; }
        public String Popularity { get; set; }
        public String Title { get; set; }
        public String AlternativeTitle { get; set; }
        public String Type { get; set; }
        public String Id { get; set; }
        public String Imdb { get; set; }
        public String Url { get; set; }
        public String ShortOverview { get; set; }
        public String Release { get; set; }
        public List<MoviedbPoster> Posters { get; set; }
        public List<MoviedbBackdrop> Backdrops { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class MoviedbBackdrop {
        public String Size { get; set; }
        public String Url { get; set; }
    }

    public class MoviedbPoster {
        public String Size { get; set; }
        public String Url { get; set; }
    }
}
