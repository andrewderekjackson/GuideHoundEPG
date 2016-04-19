using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuideHoundEPG.Common;
using GuideHoundEPG.Common.Detections;
using GuideHoundEPG.Common.Model;
using GuideHoundEPG.Common.Model.Configuration;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Tests {
    [TestClass]
    public class CategoryDetectionTests {
        
        
        [TestMethod]
        public void CanMatchExactKeyword() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("movie", ProgramCategory.Movie),
                new CategoryWordTrigger("news", ProgramCategory.News)
            };

            KeywordMatchInfo match1 = new KeywordMatchInfo("movie", triggers, ProgramCategory.Movie);
            Assert.IsTrue(match1.IsMatched);

            KeywordMatchInfo match2 = new KeywordMatchInfo("news", triggers, ProgramCategory.News);
            Assert.IsTrue(match2.IsMatched);

            KeywordMatchInfo match3 = new KeywordMatchInfo("movie", triggers, ProgramCategory.News);
            Assert.IsFalse(match3.IsMatched);

            KeywordMatchInfo match4 = new KeywordMatchInfo("news", triggers, ProgramCategory.Movie);
            Assert.IsFalse(match4.IsMatched);

            KeywordMatchInfo match6 = new KeywordMatchInfo("this is a movie about some news", triggers, ProgramCategory.Movie);
            Assert.IsTrue(match6.IsMatched);

            KeywordMatchInfo match7 = new KeywordMatchInfo("this is a movie about some news", triggers, ProgramCategory.News);
            Assert.IsTrue(match7.IsMatched);

        }

        [TestMethod]
        public void MoviesMatchExclusivly() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("movie", ProgramCategory.Movie),
                new CategoryWordTrigger("news", ProgramCategory.News)
            };

            KeywordMatchInfo match6 = new KeywordMatchInfo("this is a movie about some news", triggers, ProgramCategory.Movie, isExclusiveCategoryMatch: true);
            Assert.IsFalse(match6.IsMatched);

            KeywordMatchInfo match7 = new KeywordMatchInfo("this is a movie about some news", triggers, ProgramCategory.News);
            Assert.IsTrue(match7.IsMatched);

        }

        [TestMethod]
        public void CanMatchWordInTitle() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("movie", ProgramCategory.Movie)
            };

            KeywordMatchInfo match1 = new KeywordMatchInfo("this is a movie about stuff", triggers, ProgramCategory.Movie);
            Assert.IsTrue(match1.IsMatched);

        }

        [TestMethod]
        public void CanMatchCaseInsensitive() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("movie", ProgramCategory.Movie)
            };

            KeywordMatchInfo match1 = new KeywordMatchInfo("a MoViE about stuff", triggers, ProgramCategory.Movie);
            Assert.IsTrue(match1.IsMatched);

        }

        [TestMethod]
        public void MatchesMovieDurationInBetweenMinMax() {

            MovieDurationMatchInfo match1 = new MovieDurationMatchInfo(true, 10, 50, 15);
            Assert.IsTrue(match1.IsMatched);
        
        }

        [TestMethod]
        public void DoesNotMatchBelowMinMax() {

            MovieDurationMatchInfo match1 = new MovieDurationMatchInfo(true, 10, 50, 5);
            Assert.IsFalse(match1.IsMatched);

        }

        [TestMethod]
        public void DoesNotMatchAboveMinMax() {

            MovieDurationMatchInfo match1 = new MovieDurationMatchInfo(true, 10, 50, 60);
            Assert.IsFalse(match1.IsMatched);

        }

        [TestMethod]
        public void MatchesMinBound() {

            MovieDurationMatchInfo match1 = new MovieDurationMatchInfo(true, 10, 50, 10);
            Assert.IsTrue(match1.IsMatched);

        }

        [TestMethod]
        public void MatchesMaxBound() {

            MovieDurationMatchInfo match1 = new MovieDurationMatchInfo(true, 10, 50, 50);
            Assert.IsTrue(match1.IsMatched);

        }

        [TestMethod]
        public void KeywordDetectionSetsFlagsCorrectly() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("movie", ProgramCategory.Movie),
                new CategoryWordTrigger("news", ProgramCategory.News),
                new CategoryWordTrigger("sport", ProgramCategory.Sports),
                new CategoryWordTrigger("special", ProgramCategory.Special)
            };

            CacheProvider<string, SeriesInfo> seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);
            EPGDownloaderConfig config = new EPGDownloaderConfig();

            List<ProgramInfo> programs = new List<ProgramInfo>();

            var newsProgram = new ProgramInfo() {Title = "This is a news program"};
            var movieProgram = new ProgramInfo() {Title = "This is a movie program"};
            var sportsProgram = new ProgramInfo() { Title = "This is a sport program" };
            var specialProgram = new ProgramInfo() { Title = "This is a special program" };
            
            programs.Add(newsProgram);
            programs.Add(movieProgram);
            programs.Add(sportsProgram);
            programs.Add(specialProgram);

            KeywordCategoryDetection detection = new KeywordCategoryDetection(programs, null, triggers, seriesCache, config);
            detection.Detect();

            Assert.IsTrue(newsProgram.IsNews);
            Assert.IsFalse(newsProgram.IsMovie);
            Assert.IsTrue(newsProgram.IsSeries);
            Assert.IsFalse(newsProgram.IsSports);
            Assert.IsFalse(newsProgram.IsSpecial);
            
            Assert.IsTrue(movieProgram.IsMovie);
            Assert.IsFalse(movieProgram.IsNews);
            Assert.IsFalse(movieProgram.IsSeries);
            Assert.IsFalse(newsProgram.IsSports);
            Assert.IsFalse(newsProgram.IsSpecial);

            Assert.IsFalse(sportsProgram.IsMovie);
            Assert.IsFalse(sportsProgram.IsNews);
            Assert.IsTrue(sportsProgram.IsSeries);
            Assert.IsTrue(sportsProgram.IsSports);
            Assert.IsFalse(sportsProgram.IsSpecial);

            Assert.IsFalse(specialProgram.IsMovie);
            Assert.IsFalse(specialProgram.IsNews);
            Assert.IsTrue(specialProgram.IsSeries);
            Assert.IsFalse(specialProgram.IsSports);
            Assert.IsTrue(specialProgram.IsSpecial);

        }

        [TestMethod]
        public void KeywordDetectionDefaultsToSeriesIfMatchedAsNoneAndNoDefaultSet() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("movie", ProgramCategory.Movie),
                new CategoryWordTrigger("sports", ProgramCategory.None),
            };

            CacheProvider<string, SeriesInfo> seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);
            EPGDownloaderConfig config = new EPGDownloaderConfig();
            config.CategoryDetection.KeywordDetectionEnabled = true;
            config.CategoryDetection.UseDefaultIfUnmatched = false;

            List<ProgramInfo> programs = new List<ProgramInfo>();

            var testProgram = new ProgramInfo() { Title = "This is a movie about sports" };
            programs.Add(testProgram);
            
            KeywordCategoryDetection detection = new KeywordCategoryDetection(programs, null, triggers, seriesCache, config);
            detection.Detect();

            Assert.IsFalse(testProgram.IsNews);
            Assert.IsFalse(testProgram.IsMovie);
            Assert.IsTrue(testProgram.IsSeries);
            Assert.IsFalse(testProgram.IsSports);
            Assert.IsFalse(testProgram.IsSpecial);

            

        }

        [TestMethod]
        public void KeywordDetectionDefaultsToSpecifiedDefaultIfMatchedAsNone() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("movie", ProgramCategory.Movie),
                new CategoryWordTrigger("sports", ProgramCategory.None),
            };

            CacheProvider<string, SeriesInfo> seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);
            EPGDownloaderConfig config = new EPGDownloaderConfig();
            config.CategoryDetection.KeywordDetectionEnabled = true;
            config.CategoryDetection.UseDefaultIfUnmatched = true;
            config.CategoryDetection.UnmatchedCategory = ProgramCategory.Movie;

            List<ProgramInfo> programs = new List<ProgramInfo>();

            var testProgram = new ProgramInfo() { Title = "This is a movie about sports" };
            programs.Add(testProgram);

            KeywordCategoryDetection detection = new KeywordCategoryDetection(programs, null, triggers, seriesCache, config);
            detection.Detect();

            Assert.IsFalse(testProgram.IsNews);
            Assert.IsTrue(testProgram.IsMovie);
            Assert.IsFalse(testProgram.IsSeries);
            Assert.IsFalse(testProgram.IsSports);
            Assert.IsFalse(testProgram.IsSpecial);

        }

        [TestMethod]
        public void KeywordDetectionMatchesBasedOnProgramLength() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
            };

            CacheProvider<string, SeriesInfo> seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);
            EPGDownloaderConfig config = new EPGDownloaderConfig();
            config.CategoryDetection.KeywordDetectionEnabled = true;
            config.CategoryDetection.MovieDurationMatchingEnabled = true;
            config.CategoryDetection.MinimumMovieDurationInMinutes = 10;
            config.CategoryDetection.MaximumMovieDurationInMinutes = 150;

            List<ProgramInfo> programs = new List<ProgramInfo>();

            var testProgram = new ProgramInfo() { Title = "This is a movie about sports", DurationInMinutes = 100 };
            programs.Add(testProgram);

            KeywordCategoryDetection detection = new KeywordCategoryDetection(programs, null, triggers, seriesCache, config);
            detection.Detect();

            Assert.IsFalse(testProgram.IsNews);
            Assert.IsTrue(testProgram.IsMovie);
            Assert.IsFalse(testProgram.IsSeries);
            Assert.IsFalse(testProgram.IsSports);
            Assert.IsFalse(testProgram.IsSpecial);

        }

        [TestMethod]
        public void KeywordDetectionMatchedAsNoneOverridesMovieDurationMatch() {

            List<CategoryWordTrigger> triggers = new List<CategoryWordTrigger>() {
                new CategoryWordTrigger("sports", ProgramCategory.None),
            };

            CacheProvider<string, SeriesInfo> seriesCache = new CacheProvider<string, SeriesInfo>(e => e.CacheKey);
            EPGDownloaderConfig config = new EPGDownloaderConfig();
            config.CategoryDetection.KeywordDetectionEnabled = true;
            config.CategoryDetection.MovieDurationMatchingEnabled = true;
            config.CategoryDetection.MinimumMovieDurationInMinutes = 10;
            config.CategoryDetection.MaximumMovieDurationInMinutes = 150;

            List<ProgramInfo> programs = new List<ProgramInfo>();

            var testProgram = new ProgramInfo() { Title = "This is a movie about sports", DurationInMinutes = 100};
            programs.Add(testProgram);

            KeywordCategoryDetection detection = new KeywordCategoryDetection(programs, null, triggers, seriesCache, config);
            detection.Detect();

            Assert.IsFalse(testProgram.IsNews);
            Assert.IsFalse(testProgram.IsMovie);
            Assert.IsTrue(testProgram.IsSeries);
            Assert.IsFalse(testProgram.IsSports);
            Assert.IsFalse(testProgram.IsSpecial);

        }
       
        
    }
}


