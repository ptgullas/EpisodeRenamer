using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Data.Entities;
using Renamer.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Renamer.Services {
    public class EpisodeService {
        private EpisodeContext _context;
        public EpisodeService(EpisodeContext context) {
            _context = context;
        }

        public void Add(Episode ep) {
            if (!_context.Episodes
                .Any(b => b.TVDBEpisodeId == ep.TVDBEpisodeId)) {
                _context.Episodes.Add(ep);
                _context.SaveChanges();
            }
        }


    }
}
