using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Logging;
using log4net;
using Services.MoviedbLookup;
using SoundsLikeExtensions;
using GuideHoundEPG.Common;
using GuideHoundEPG.Common.Model.MXF;
using Services.MoviedbLookup.Model;

namespace GuideHoundEPG
{
    public class MoviedbFiller {
        
        private static readonly ILogger logger = Logger.GetLogger();

        private MoviedbService service;

        private MoviedbServiceConfiguration configuration;

        public MoviedbFiller() {
            configuration = new MoviedbServiceConfiguration();
            configuration.ApiKey = "04f727c1e1c9c461a8aa2c83471242a7";
            configuration.CacheFolder = EnvironmentInfo.GetCacheSubPath("Moviedb");

            service = new MoviedbService(configuration);
        }

        public ProgramInfo Fill(string title, ProgramInfo programInfo) {
        

            logger.Log(LogLevel.Debug, String.Format("THEMOVIEDB: Looking up title '{0}'...", title));

            try {

                List<MoviedbResult> results = service.Search(title).Where(e => e.Type == "movie").ToList();
                MoviedbResult baseResult = null;

                if (results.Count==0) {
                    // e.g.: "Sunday Night Movie: Transformers" - try evenything after a ":"
                    string parsedTitle = title.Substring(title.IndexOf(":") + 1).Trim();

                    logger.Log(LogLevel.Debug, String.Format("THEMOVIEDB: No results - trying '{0}'...", parsedTitle));
                    results = service.Search(parsedTitle).Where(e => e.Type == "movie").ToList();
                }
                
                // check for direct match
                baseResult = results.Where(e => e.Title.ToLower() == title.ToLower()).FirstOrDefault();

                // first in list
                if (baseResult==null) {
                    baseResult = results.FirstOrDefault();    
                }
                                
                if (baseResult!=null) {

                    logger.Log(LogLevel.Info, String.Format("THEMOVIEDB: Accepted Result: {0}", baseResult.Title));
                    programInfo.IsMetadataLookup = true;

                    // add properties here
                    
                    // get banner image here
                    MoviedbPoster poster = baseResult.Posters.Where(e=>e.Size=="cover").First() ??
                                           baseResult.Posters.First();

                    if (poster!=null) {
                        logger.Log(LogLevel.Info, String.Format("THEMOVIEDB: Downloading poster: {0}...", poster.Url));

                        string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        string filenameOnly = poster.Url.Substring(poster.Url.LastIndexOf('/') + 1);
                        string bannerFilename = Path.Combine(configuration.CacheFolder, filenameOnly);


                        if (!File.Exists(bannerFilename)) {
                            logger.Log(LogLevel.Debug, String.Format("THEMOVIEDB: filename does not exists in cache - downloading."));

                            Bitmap posterImage = service.DownloadBannerImage(poster.Url);
                            // set the season poster
                            if (posterImage != null) {
                                posterImage.Save(bannerFilename);
                            }
                        } else {
                            logger.Log(LogLevel.Debug, String.Format("THEMOVIEDB: Banner filename already exists in cache - not downloading."));
                        }
                        
                        // create guide image
                        string guideImageKey = "i" + baseResult.Id;

                        GuideImage guideImage = new GuideImage();
                        guideImage.OutputImageType = GuideImageType.MoviePoster;
                        guideImage.Id = guideImageKey;
                        guideImage.SourceUrl = bannerFilename;

                        programInfo.GuideImage = guideImage;
                        
                    }

                    //// lets try and get more info
                    //TvdbSeriesFull fullSeries =  tv.GetFullSeries(baseResult.TvdbId);

                    //if (fullSeries!=null) {
                        
                    //    // set the season poster
                    //    if (fullSeries.PosterImage!=null) {

                    //        string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    //        string cacheFolder = Path.Combine(currentFolder, configuration.CacheFolder);
                    //        string filenameOnly = fullSeries.PosterUrl.Substring(fullSeries.PosterUrl.LastIndexOf('/') + 1);
                    //        string bannerFilename = Path.Combine(cacheFolder, filenameOnly);
                            
                    //        fullSeries.PosterImage.Save(bannerFilename);
                            
                    //        // create guide image
                    //        string guideImageKey = "i" + seriesInfo.Id;

                    //        GuideImage guideImage = new GuideImage();
                    //        guideImage.OutputImageType = GuideImageType.SeriesPoster;
                    //        guideImage.Id = guideImageKey;
                    //        guideImage.SourceUrl = bannerFilename;

                    //        seriesInfo.GuideImage = guideImage;
                            
                    //    }

                    //}
                 }

            } catch (Exception e) {
                logger.Log(LogLevel.Debug, "THEMOVIEDB: Unable to retrieve movie information.", e);
            }

            return programInfo;
        }

    }
}
