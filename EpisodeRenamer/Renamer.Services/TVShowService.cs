using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Data.Entities;
using Renamer.Services.Models;

namespace Renamer.Services {
    public class TVShowService {
        private EpisodeContext _context;
        public TVShowService() {

        }

        public TVShowService(EpisodeContext context) {
            _context = context;
        }

        public TVShow ConvertToTVShow(TVShowDto showDto) {
            return new TVShow() {
                SeriesId = showDto.SeriesId,
                SeriesName = showDto.SeriesNameTVDB,
                SeriesNamePreferred = showDto.SeriesNamePreferred
            };
        }
    }
}
