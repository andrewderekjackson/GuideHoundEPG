using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.TvdbLookup
{

    /// <summary>
    /// Configuration settings for the TbdbLookup service.
    /// </summary>
    public class TvdbServiceConfiguration {
        public String ApiKey { get; set; }
        public String CacheFolder { get; set; }
    }
}
