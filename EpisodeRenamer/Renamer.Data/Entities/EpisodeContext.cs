using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Data.Entities {
    public class EpisodeContext : DbContext {
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<TVSeries> Series { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=tvepisodes.db");
        }
    }
}
