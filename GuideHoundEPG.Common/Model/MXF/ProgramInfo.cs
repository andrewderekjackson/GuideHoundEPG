using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GuideHoundEPG.Common.Model.MXF {

    [DebuggerDisplay("Program: {Title}, IsSeries: {IsSeries}, IsMovie: {IsMovie}")]
    public class ProgramInfo : IEquatable<ProgramInfo> {
        public ProgramInfo() {
            Keywords = new List<Keyword>();
            Roles = new List<Role>();
        }
        public String Id { get; set; }
        public String Uid { 
            get {
                return "!Program!" + Id;
            }
        }
        public String Title { get; set; }
        public String EpisodeTitle { get; set; }
        public String Description { get; set; }
        public String ShortDescription { get; set; }
        public String Language { get; set; }
        public int Year { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime OriginalAirDate { get; set; }
        public List<Keyword> Keywords { get; set; }
        public Season Season { get; set; }
        public SeriesInfo Series { get; set; }
        public List<Role> Roles { get; set; }
        public int HalfStars { get; set; }
        public int MpaaRating { get; set; }
        public bool IsMovie { get; set; }
        public bool IsMiniseries { get; set; }
        public bool IsLimitedSeries { get; set; }
        public bool IsSeries { get; set; }
        public bool IsSerial { get; set; }
        public bool IsShortFilm { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsSports { get; set; }
        public bool IsNews { get; set; }
        public bool IsReality { get; set; }
        public GuideImage GuideImage { get; set; }
        

        // MPAA Ratings
        public bool HasAdult { get; set; }
        public bool HasBriefNudity { get; set; }
        public bool HasGraphicViolence { get; set; }
        public bool HasLanguage { get; set; }
        public bool HasMildViolence { get; set; }
        public bool HasNudity { get; set; }
        public bool HasRape { get; set; }
        public bool HasStringSexualContent { get; set; }
        public bool HasViolence { get; set; }

        // other properties
        public double DurationInMinutes { get; set; }

        /// <summary>
        /// Has this entry been filled by a sucessfull metadata lookup
        /// </summary>
        public bool IsMetadataFilled { get; set; }

        /// <summary>
        /// Has this entry been looked up against a metadata service (either successfull or not)
        /// </summary>
        public bool IsMetadataLookup { get; set; }

        /// <summary>
        /// Is this an audio series.
        /// </summary>
        public bool IsAudio { get; set; }

        public bool Equals(ProgramInfo other) {
            return this.Uid.Equals(other.Uid);
        }

        public void AddRole<T>(Person person) where T : Role, new() {
            T role = new T();
            role.Person = person;
            Roles.Add(role);
        }

        public void ApplyCategory(ProgramCategory programCategory) {

            switch (programCategory) {

                case ProgramCategory.Movie:
                    IsMovie = true;
                    IsNews = false;
                    IsSpecial = false;
                    IsSports = false;

                    IsSeries = false;
                    break;
                case ProgramCategory.News:
                    IsMovie = false;
                    IsNews = true;
                    IsSpecial = false;
                    IsSports = false;

                    IsSeries = true;
                    break;
                case ProgramCategory.Series:
                    IsMovie = false;
                    IsNews = false;
                    IsSpecial = false;
                    IsSports = false;

                    IsSeries = true;
                    break;
                case ProgramCategory.Special:
                    IsMovie = false;
                    IsNews = false;
                    IsSpecial = true;
                    IsSports = false;

                    IsSeries = true;
                    break;
                case ProgramCategory.Sports:
                    IsMovie = false;
                    IsNews = false;
                    IsSpecial = false;
                    IsSports = true;
                    IsSeries = true;
                    break;

            }
        }

        public ProgramCategory GetProgramCategory() {
            if (IsMovie) {
                return ProgramCategory.Movie;
            } else if (!IsMovie && IsNews && IsSeries) {
                return ProgramCategory.News;
            } else if (!IsMovie && IsSpecial && IsSeries) {
                return ProgramCategory.Special;
            } else if (!IsMovie && IsSports && IsSeries) {
                return ProgramCategory.Special;
            } else if (IsSeries) {
                return ProgramCategory.Series;
            }

            return ProgramCategory.None;
        }
    }
}