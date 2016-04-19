using System;

namespace Services.MoviedbLookup {
    /// <summary>
    /// Configuration settings for the MoviedbLookup service.
    /// </summary>
    public class MoviedbServiceConfiguration {
        public String ApiKey { get; set; }
        public String CacheFolder { get; set; }
    }
}