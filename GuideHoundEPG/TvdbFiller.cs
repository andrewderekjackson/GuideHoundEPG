using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Core.Logging;
using log4net;
using Services.TvdbLookup;
using Services.TvdbLookup.Model;
using SoundsLikeExtensions;
using GuideHoundEPG.Common;
using GuideHoundEPG.Common.Model.MXF;
using System.Reflection;

namespace GuideHoundEPG {
    /// <summary>
    /// Use the TVDB to try and pad the program list with missing information.
    /// </summary>
    public class TvdbFiller {
        
        private static readonly ILogger logger = Logger.GetLogger();
        private TvdbService tv;

        private TvdbServiceConfiguration configuration;

        public TvdbFiller() {
            configuration = new TvdbServiceConfiguration();
            configuration.ApiKey = "CE72338B4ECE6EDA";
            configuration.CacheFolder = EnvironmentInfo.GetCacheSubPath("Tvdb");

            tv = new TvdbService(configuration);
        }
        
        public SeriesInfo Fill(string title, SeriesInfo  seriesInfo) {

            logger.Log(LogLevel.Debug, String.Format("TVDB: Looking up title '{0}'...", title));

            try {

                List<TvdbSeriesBase> results = tv.Search(title).OrderByDescending(e => e.FirstAired).ToList();
                
                // rank ratings by their similarity
                Dictionary<float, TvdbSeriesBase> rankedResults = new Dictionary<float, TvdbSeriesBase>();
                foreach (var searchResult in results) {
                    float rating = searchResult.SeriesName.SimilarText(title);

                    logger.Log(LogLevel.Info, String.Format("TVDB: Match: {0:00}% - {1}", rating, searchResult.SeriesName));
                    
                    if (!rankedResults.ContainsKey(rating)) {
                        rankedResults.Add(rating, searchResult);
                    } else {
                        if (searchResult.FirstAired > rankedResults[rating].FirstAired) {
                            rankedResults[rating] = searchResult;
                        }
                    }
                }

                var result = rankedResults.OrderByDescending(e => e.Key).Where(e=>e.Key>=80).FirstOrDefault();

                if (result.Value!=null) {
                    TvdbSeriesBase baseSeries = result.Value;

                    logger.Log(LogLevel.Info, String.Format("TVDB: Accepted Result: {0:00}% - {1}", result.Key, baseSeries.SeriesName));

                    seriesInfo.StartAirDate = baseSeries.FirstAired;
                    seriesInfo.Description = baseSeries.Overview;
                    seriesInfo.IsMetadataFilled = true;

                    // lets try and get more info
                    TvdbSeriesFull fullSeries =  tv.GetFullSeries(baseSeries.TvdbId);

                    if (fullSeries!=null) {
                        
                        // set the season poster
                        if (fullSeries.PosterImage!=null) {

                            string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            string filenameOnly = fullSeries.PosterUrl.Substring(fullSeries.PosterUrl.LastIndexOf('/') + 1);
                            string bannerFilename = Path.Combine(configuration.CacheFolder, filenameOnly);
                            
                            fullSeries.PosterImage.Save(bannerFilename);
                            
                            // create guide image
                            string guideImageKey = "i" + seriesInfo.Id;

                            GuideImage guideImage = new GuideImage();
                            guideImage.OutputImageType = GuideImageType.SeriesPoster;
                            guideImage.Id = guideImageKey;
                            guideImage.SourceUrl = bannerFilename;

                            seriesInfo.GuideImage = guideImage;

                        } 
                    }

                }
                

            } catch (Exception e) {
                logger.Log(LogLevel.Debug, "TVDB: Unable to retrieve series information.", e);
            }

            //int seriesId = results.First().Id;
            //TvdbSeriesBase series = tvdb.GetSeries(seriesId, TvdbLanguage.DefaultLanguage, true, false, false);
            //if (series!=null) {
            //    log.Log(LogLevel.Info, String.Format("* Found series information."));
            //    // .GenreList = series.Genre;
            //    //if (String.IsNullOrEmpty(tvProgram.SubTitle)) {
            //    //    continue;
            //    //}
            //    //var episode = (from e in series.Episodes
            //    //               where e.EpisodeName == tvProgram.SubTitle
            //    //               select e).FirstOrDefault();
            //    //if (episode != null) {
            //    //    log.Log(LogLevel.Info, String.Format("* Found additional episode information."));
            //    //    tvProgram.EpisodeOverview = episode.Overview;
            //    //    // tvProgram.Rating = episode.Rating.ToString();
            //    //}
            //}
            

            return seriesInfo;
        }

    }
}