using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using GuideHoundEPG.Common.Model;
using GuideHoundEPG.Common.Model.MXF;

namespace GuideHoundEPG.Common.Detections {
    
    public class Match  {
        public bool IsMatched { get; set; }
        public ProgramCategory ProgramCategory { get; set; }
        public virtual void ApplyChanges(ProgramInfo program) { }
    }

    public class KeywordMatchInfo : Match {

        public bool RemoveWord { get; set; }
        public string MatchedOnWord { get; set; }
        private static readonly ILogger logWriter = Logger.GetLogger();

        public KeywordMatchInfo(string stringExpression, List<CategoryWordTrigger> wordList, ProgramCategory wordCategory, bool isExclusiveCategoryMatch = false) {
            this.ProgramCategory = wordCategory;

            CategoryWordTrigger categoryMatch = MatchesWordInCategory(stringExpression, wordList, wordCategory);

            if (categoryMatch==null) {
                IsMatched = false;
                return;
            }

            MatchedOnWord = categoryMatch.Word;
            RemoveWord = categoryMatch.RemoveWord;
            
            string matchInAnotherCategory = MatchesWordInAnyOtherCategory(stringExpression, wordList, wordCategory);

            if (isExclusiveCategoryMatch) {
                IsMatched = MatchedOnWord != String.Empty && matchInAnotherCategory == String.Empty;
            } else {
                IsMatched = MatchedOnWord != String.Empty;
            }
        }

        public CategoryWordTrigger MatchesWordInCategory(string stringExpression, List<CategoryWordTrigger> wordList, ProgramCategory wordCategory) {
            CategoryWordTrigger word = null;

            wordList.Where(e => e.Category == wordCategory).ToList().ForEach(e => {
                if (stringExpression.ToLower().Contains(e.Word.ToLower())) {
                    word = e;
                }
            });

            return word;
        }

        public string MatchesWordInAnyOtherCategory(string stringExpression, List<CategoryWordTrigger> wordList, ProgramCategory wordCategory) {
            string matchedWord = String.Empty;

            wordList.Where(e => e.Category != wordCategory).ToList().ForEach(e => {
                if (stringExpression.ToLower().Contains(e.Word.ToLower())) {
                    matchedWord = e.Word;
                }
            });

            return matchedWord;
        }

        public override void ApplyChanges(ProgramInfo program) {
            if (RemoveWord) {
                program.Title = program.Title.Replace(MatchedOnWord, String.Empty).Trim();
            }
        }
    }

    public class MovieDurationMatchInfo : Match {

        public MovieDurationMatchInfo(bool isEnabled, int minDuration, int maxDuration, int durationInMinutes) {
            ProgramCategory = ProgramCategory.Movie;
            IsMatched = isEnabled && (durationInMinutes >= minDuration && durationInMinutes <= maxDuration) && durationInMinutes != 90 && durationInMinutes !=120;
        }
        
    }
}