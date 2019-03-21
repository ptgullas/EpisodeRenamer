using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Data.Entities;
using Renamer.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Serilog;

namespace Renamer.Services {
    public class EpisodeService {
        private EpisodeContext _context;
        public EpisodeService(EpisodeContext context) {
            _context = context;
        }

        public void CheckIfUpdateNeeded(Episode ep) {
            if (_context.Episodes
                .FirstOrDefault(b => b.TVDBEpisodeId == ep.TVDBEpisodeId)
                .LastUpdated < ep.LastUpdated ) {
                Log.Information("LastUpdated property outdated on Episode {a}: {b} ", ep.Id, ep.EpisodeName, ep);
            }
        }

        public void Add(Episode ep) {
            if (!_context.Episodes
                .Any(b => b.TVDBEpisodeId == ep.TVDBEpisodeId)) {
                _context.Episodes.Add(ep);
                _context.SaveChanges();
                Log.Information($"Added EpisodeId {ep.TVDBEpisodeId} - \"{ep.EpisodeName}\" to database");
            }
        }

        // Don't use this (doesn't check TVDBEpisodeId)
        public void AddEpisodeToShow(Episode ep) {
            var show = _context.Shows
                .Include(s => s.Episodes)
                .Single(b => b.SeriesId == ep.SeriesId);
            show.Episodes.Add(ep);
            _context.SaveChanges();
        }

        public Episode FindByEpisodeForComparingDto(EpisodeForComparingDto epForComparing) {
            TVShowService tvshowService = new TVShowService(_context);
            TVShow show = tvshowService.FindByName(epForComparing.SeriesName);
            if (show != null) {
                return _context.Episodes
                    .FirstOrDefault(b => b.SeriesId == show.SeriesId
                        && b.Season == epForComparing.SeasonNumber
                        && b.AiredEpisodeNumber == epForComparing.EpisodeNumberInSeason);
            }
            else {
                return null;
            }
        }


    }
}
