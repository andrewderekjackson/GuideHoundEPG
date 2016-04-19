using System;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Common {
    
    public class SeriesCachingHelper {
        
        /// <summary>
        /// Removes extra words which are not helpfull in detection.
        /// </summary>
        private static string RemoveFluffWords(string value) {
            return value.Replace("All New ", "");
        }

        public static void UpdateProgramSeriesCache(Provider provider, CacheProvider<string, SeriesInfo> seriesCache, ProgramInfo program, string title) {
            
            // string cacheTitle = RemoveFluffWords(title);

            if (program.IsSeries) {
                // 1. Is there a cached series entry for this item.
                SeriesInfo series = seriesCache.FindKey(title);
                if (series != null) {
                    // use existing cache series entry
                    if (!provider.Series.Contains(series)) {
                        provider.Series.Add(series);
                    }
                } else {
                    // create new entry
                    series = new SeriesInfo();
                    series.Id = "si" + Guid.NewGuid(); // make this unique
                    series.Title = program.Title; // leave the original name "All New " ,etc"
                    series.CacheKey = title;

                    // add to cache
                    seriesCache.Add(series);
                }

                program.Series = series;
                program.GuideImage = series.GuideImage;

                //if (series.GuideImage != null && !provider.GuideImages.Contains(series.GuideImage)) {
                //    provider.GuideImages.Add(series.GuideImage);
                //}
            } else {
                
                // remove from cache if it exists.
                SeriesInfo series = seriesCache.FindKey(title);
                if (series != null) {
                    seriesCache.Remove(series);
                }
            }
            
        }
    }
}
