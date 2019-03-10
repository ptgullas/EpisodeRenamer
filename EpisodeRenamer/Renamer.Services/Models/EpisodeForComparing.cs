using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public class EpisodeForComparing {
        public string SeriesName { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumberInSeason { get; set; }
        public string EpisodeTitle { get; set; }
    }
}
