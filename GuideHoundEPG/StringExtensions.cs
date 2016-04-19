using System;
using System.Collections.Generic;
using Core.Logging;
using GuideHoundEPG.Common.Model;
using System.Linq;

namespace GuideHoundEPG {
    public static class StringExtensions {

        private static readonly ILogger logWriter = Logger.GetLogger();

        public static bool MatchesCategory(this string stringExpression, List<CategoryWordTrigger> wordList, ProgramCategory wordCategory) {

            return MatchesWordInCategory(stringExpression, wordList, wordCategory) &&
                   !MatchesWordInAnyOtherCategory(stringExpression, wordList, wordCategory);
        }

        public static bool MatchesWordInCategory(this string stringExpression, List<CategoryWordTrigger> wordList, ProgramCategory wordCategory) {
            bool containsWordInCategory = false;

            wordList.Where(e=>e.Category==wordCategory).ToList().ForEach(e=> {
                                 if (stringExpression.ToLower().Contains(e.Word.ToLower())) {
                                     containsWordInCategory = true;
                                     logWriter.Log(LogLevel.Info, String.Format("Matches word {0}", e.Word));
                                 }
                             });

            return containsWordInCategory;
        }

        public static bool MatchesWordInAnyOtherCategory(this string stringExpression, List<CategoryWordTrigger> wordList, ProgramCategory wordCategory) {
            bool containsWordInAnyOtherCategory = false;

            wordList.Where(e=>e.Category!=wordCategory).ToList().ForEach(e=> {
                                 if (stringExpression.ToLower().Contains(e.Word.ToLower())) {
                                     containsWordInAnyOtherCategory = true;
                                     logWriter.Log(LogLevel.Info, String.Format("Matches word '{0}' in other category.", e.Word));
                                 }
                             });

            return containsWordInAnyOtherCategory;
        }
    }
    
}