using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Logging;
using GuideHoundEPG.Common.Model;
using GuideHoundEPG.Common.Model.Configuration;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Common.Detections {
    
    public abstract class Detection {

        public abstract void Detect();

        public void DumpStats(ILogger log, DetectionStatistics stats, string message) {
            // display summary
            log.Log(LogLevel.Debug, String.Format(message));
            log.Log(LogLevel.Debug, String.Format(" Series : {0}", stats.Series));
            log.Log(LogLevel.Debug, String.Format(" Movies  : {0}", stats.Movies));
            log.Log(LogLevel.Debug, String.Format(" News   : {0}", stats.News));
            log.Log(LogLevel.Debug, String.Format(" Sports : {0}", stats.Sports));
            log.Log(LogLevel.Debug, String.Format(" Special: {0}", stats.Special));
            log.Log(LogLevel.Debug, String.Format(" Unknown: {0}", stats.Unknown));
            log.Log(LogLevel.Debug, String.Format(" Unchanged: {0}", stats.Unchanged));
        }

    }

    public class KeywordCategoryDetection : Detection {

        private static readonly ILogger log = Logger.GetLogger();
        private IEnumerable<ProgramInfo> programs;
        private Provider provider;
        private List<CategoryWordTrigger> triggerWords;
        private CacheProvider<string, SeriesInfo> seriesCache;
        private EPGDownloaderConfig config;

        public KeywordCategoryDetection(IEnumerable<ProgramInfo> programs, Provider provider, List<CategoryWordTrigger> triggerWords, CacheProvider<string, SeriesInfo> seriesCache, EPGDownloaderConfig config) {
            this.config = config;
            this.seriesCache = seriesCache;
            this.triggerWords = triggerWords;
            this.provider = provider;
            this.programs = programs;
        }

        /// <summary>
        /// Perform category detection based on keywords.
        /// </summary>
        public override void Detect() {

            DetectionStatistics categoryDetectionStats = new DetectionStatistics();

            foreach (var program in programs.Where((p => !p.IsAudio))) {

                //if (((program.DurationInMinutes >= 90 && program.DurationInMinutes < 210) || (program.Title.MatchesWordInCategory(triggerWords, ProgramCategory.Movie))) && (!program.Title.MatchesWordInAnyOtherCategory(triggerWords, ProgramCategory.Movie))) {

                ProgramCategory currentCategory = program.GetProgramCategory();

                // determine whether to try and drill down any further
                if (currentCategory == ProgramCategory.None || currentCategory == ProgramCategory.Series) {

                    log.Log(LogLevel.Debug, String.Format("Identifing show '{0}'...", program.Title));

                    //// MOVIE DURATION DETECTION
                    //if (config.CategoryDetection.MovieDurationMatchingEnabled) {
                    //    if (IsMovieBasedOnDuration(program, config.CategoryDetection.MinimumMovieDurationInMinutes, config.CategoryDetection.MaximumMovieDurationInMinutes)) {
                    //        log.Log(LogLevel.Debug, String.Format("- MOVIE (duration {0}).", program.DurationInMinutes));
                    //    }
                    //}

                    Match actualMatch = null;

                    bool matched = false;

                    KeywordMatchInfo movieMatch = new KeywordMatchInfo(program.Title, triggerWords, ProgramCategory.Movie, isExclusiveCategoryMatch: true);
                    MovieDurationMatchInfo movieDurationMatch = new MovieDurationMatchInfo(config.CategoryDetection.MovieDurationMatchingEnabled, config.CategoryDetection.MinimumMovieDurationInMinutes, config.CategoryDetection.MaximumMovieDurationInMinutes, Convert.ToInt32(program.DurationInMinutes));

                    KeywordMatchInfo newsMatch = new KeywordMatchInfo(program.Title, triggerWords, ProgramCategory.News);
                    KeywordMatchInfo sportsMatch = new KeywordMatchInfo(program.Title, triggerWords, ProgramCategory.Sports);
                    KeywordMatchInfo specialMatch = new KeywordMatchInfo(program.Title, triggerWords, ProgramCategory.Special);
                    KeywordMatchInfo seriesMatch = new KeywordMatchInfo(program.Title, triggerWords, ProgramCategory.Series);
                    KeywordMatchInfo noneMatch = new KeywordMatchInfo(program.Title, triggerWords, ProgramCategory.None);

                    bool unmatched = false;

                    if (movieMatch.IsMatched) {
                        log.Log(LogLevel.Debug, String.Format(" - MOVIE ('{0}')", movieMatch.MatchedOnWord));
                        actualMatch = movieMatch;
                    } else if (movieDurationMatch.IsMatched) {
                        log.Log(LogLevel.Debug, String.Format(" - MOVIE (duration {0}).", program.DurationInMinutes));
                        actualMatch = movieDurationMatch;
                    } else if (newsMatch.IsMatched) {
                        log.Log(LogLevel.Debug, String.Format(" - NEWS ('{0}')", newsMatch.MatchedOnWord));
                        actualMatch = newsMatch;
                    } else if (sportsMatch.IsMatched) {
                        log.Log(LogLevel.Debug, String.Format(" - SPORTS ('{0}')", sportsMatch.MatchedOnWord));
                        actualMatch = sportsMatch;
                    } else if (specialMatch.IsMatched) {
                        log.Log(LogLevel.Debug, String.Format(" - SPECIAL ('{0}')", specialMatch.MatchedOnWord));
                        actualMatch = specialMatch;
                    } else if (seriesMatch.IsMatched) {
                        log.Log(LogLevel.Debug, String.Format(" - SERIES ('{0}')", seriesMatch.MatchedOnWord));
                        actualMatch = seriesMatch;
                    } else {
                        unmatched = true;
                    }

                    if (noneMatch.IsMatched) {
                        unmatched = true;
                        actualMatch = noneMatch;
                    }

                    if (actualMatch!=null) {
                        program.ApplyCategory(actualMatch.ProgramCategory);
                        categoryDetectionStats.Increment(actualMatch.ProgramCategory);

                        actualMatch.ApplyChanges(program);
                    }

                    if (unmatched) {
                        if (config.CategoryDetection.UseDefaultIfUnmatched) {
                            if (noneMatch.IsMatched) {
                                log.Log(LogLevel.Debug, String.Format(" - UNMATCHED ('{0}') - applying default.", noneMatch.MatchedOnWord));
                            } else {
                                log.Log(LogLevel.Debug, String.Format(" - UNMATCHED - applying default.", noneMatch.MatchedOnWord));
                            }
                            program.ApplyCategory(config.CategoryDetection.UnmatchedCategory);
                            categoryDetectionStats.Increment(config.CategoryDetection.UnmatchedCategory);
                        } else {
                            log.Log(LogLevel.Debug, String.Format(" - SERIES (none category)", noneMatch.MatchedOnWord));
                            program.ApplyCategory(ProgramCategory.Series);
                            categoryDetectionStats.Unchanged++;
                        }
                    }

                    

                    //if (program.Title.MatchesCategory(triggerWords, ProgramCategory.Movie)) {

                    //} else if (program.Title.MatchesCategory(triggerWords, ProgramCategory.News)) {
                    //    log.Log(LogLevel.Debug, "- NEWS.");
                    //    program.ApplyCategory(ProgramCategory.News);
                    //    categoryDetectionStats.Increment(ProgramCategory.News);
                    //    matched = true;
                    //} else if (program.Title.MatchesCategory(triggerWords, ProgramCategory.Sports)) {
                    //    log.Log(LogLevel.Debug, "- SPORTS.");
                    //    program.ApplyCategory(ProgramCategory.Sports);
                    //    categoryDetectionStats.Increment(ProgramCategory.Sports);
                    //    matched = true;
                    //} else if (program.Title.MatchesCategory(triggerWords, ProgramCategory.Special)) {
                    //    log.Log(LogLevel.Debug, "- SPECIAL.");
                    //    program.ApplyCategory(ProgramCategory.Special);
                    //    categoryDetectionStats.Increment(ProgramCategory.Special);
                    //    matched = true;
                    //} else if (program.Title.MatchesCategory(triggerWords, ProgramCategory.Series)) {
                    //    log.Log(LogLevel.Debug, "- SERIES.");
                    //    program.ApplyCategory(ProgramCategory.Series);
                    //    categoryDetectionStats.Increment(ProgramCategory.Series);
                    //    matched = true;
                    //} else if (program.Title.MatchesCategory(triggerWords, ProgramCategory.None)) {
                    //    log.Log(LogLevel.Debug, "- EXPLICIT UNMATCHED");
                    //    program.ApplyCategory(ProgramCategory.Series);
                    //    categoryDetectionStats.Increment(ProgramCategory.Series);
                    //} 

                    //if (!matched) {
                    //    if (config.CategoryDetection.UseDefaultIfUnmatched) {
                    //        log.Log(LogLevel.Debug, "- UNMATCHED.");
                    //        program.ApplyCategory(config.CategoryDetection.UnmatchedCategory);
                    //        categoryDetectionStats.Increment(config.CategoryDetection.UnmatchedCategory);
                    //    } else {
                    //        categoryDetectionStats.Unchanged++;
                    //    }
                    //}

                    SeriesCachingHelper.UpdateProgramSeriesCache(provider, seriesCache, program, program.Title);
                }

            }
            DumpStats(log, categoryDetectionStats, "Keyword category detection made the following changes:");
        }


    }
}
