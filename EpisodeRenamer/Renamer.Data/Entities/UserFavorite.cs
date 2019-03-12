using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Data.Entities {

    public class UserFavorite {
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public List<TVSeries> Series { get; set; }
    }
}
