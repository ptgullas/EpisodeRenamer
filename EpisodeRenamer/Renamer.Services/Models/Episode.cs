using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public class Episode {
        public int SeriesId { get; set; }
        public int Season { get; set; }
        public int NumberInSeason { get; set; }
        public string Title { get; set; }
        public bool IsInList { get; set; }

    }
}
