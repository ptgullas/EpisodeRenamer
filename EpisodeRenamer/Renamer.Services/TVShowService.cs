using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Data.Entities;
using Renamer.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Renamer.Services {
    public class TVShowService {
        private EpisodeContext _context;
        public TVShowService() {

        }

        public TVShowService(EpisodeContext context) {
            _context = context;
        }

        public void Add(TVShow show) {
            if (!_context.Shows
                .Any(b => b.SeriesId == show.SeriesId)) {
                _context.Shows.Add(show);
                _context.SaveChanges();
            }
        }

        public void AddPreferredName(int seriesId, string preferredName) {

            TVShow show = _context.Shows.FirstOrDefault(b => b.SeriesId == seriesId);
            if (show != null) {
                show.SeriesNamePreferred = preferredName;
                _context.SaveChanges();
            }
        }

        public TVShow FindByName(string seriesName) {
            TVShow show = _context.Shows.FirstOrDefault(b =>
                b.SeriesName.ToUpper().RemoveNonAlphanumeric() == seriesName.ToUpper().RemoveNonAlphanumeric());
            return show;
        }

    }
}
