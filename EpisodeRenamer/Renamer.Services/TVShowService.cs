using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Data.Entities;
using Renamer.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Serilog;

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
            Log.Information($"Adding preferred name {preferredName} to seriesId {seriesId}");
            try {
                TVShow show = _context.Shows.FirstOrDefault(b => b.SeriesId == seriesId);
                if (show != null) {
                    show.SeriesNamePreferred = preferredName;
                    _context.SaveChanges();
                }
            }
            catch (Exception e) {
                Log.Error(e, "Trouble adding preferred name {a} to {b}", preferredName, seriesId);
            }
        }

        public void ToggleShowActiveStatus(int seriesId) {
            TVShow show = _context.Shows.FirstOrDefault(b => b.SeriesId == seriesId);
            try {
                if (show != null) {
                    show.IsActive = !show.IsActive;
                    Log.Information("Set {showName} IsActive status to {activeStatus}", show.SeriesName, show.IsActive);
                    _context.SaveChanges();
                }
            }
            catch (Exception e) {
                Log.Error(e, "Error toggling IsActive status for {showName}", show.SeriesName);
            }
        }

        public TVShow FindByName(string seriesName) {
            string seriesNameUpperAlphaOnly = seriesName.ToUpper().RemoveNonAlphanumeric();
            IEnumerable<TVShow> shows = _context.Shows.ToList();
            return shows.FirstOrDefault(
                s => s.SeriesName.ToUpper().RemoveNonAlphanumeric() == seriesNameUpperAlphaOnly);
        }

        public TVShow FindBySeriesId(int seriesId) {
            TVShow show = _context.Shows.FirstOrDefault(s => s.SeriesId == seriesId);
            return show;
        }

        public List<int> GetSeriesIdsNotInDatabase(List<int> seriesIdsToCheck) {
            var seriesIdsInDB = _context.Shows.Select(s => s.SeriesId);
            return seriesIdsToCheck.Except(seriesIdsInDB).ToList();
        }

    }
}
