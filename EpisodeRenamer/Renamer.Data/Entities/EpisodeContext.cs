using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace Renamer.Data.Entities {
    public class EpisodeContext : DbContext {

        public EpisodeContext(DbContextOptions<EpisodeContext> options) 
            : base(options) {

        }


        public DbSet<Episode> Episodes { get; set; }
        public DbSet<TVShow> Shows { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\Prime Time Pauly G\\source\\repos\\EpisodeRenamer\\EpisodeRenamer\\RenamerConsole\\tvepisodes.db");
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity("Renamer.Data.Entities.Episode", b => {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("AbsoluteNumber");
                b.Property<int>("AiredEpisodeNumber");
                b.Property<int>("AiredSeasonId");
                b.Property<string>("EpisodeName");
                b.Property<DateTime>("FirstAired");
                b.Property<int>("Season");
                b.Property<int>("SeriesId");
                b.HasKey("Id");
                b.HasIndex("SeriesId");
                b.ToTable("Episodes");
            });

            modelBuilder.Entity("Renamer.Data.Entities.TVShow", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int>("SeriesId");
                b.Property<string>("SeriesName");
                b.Property<string>("SeriesNamePreferred");
                b.HasKey("Id");
                b.ToTable("Shows");
            });

            modelBuilder.Entity("Renamer.Data.Entities.UserFavorite", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int>("SeriesId");
                b.HasKey("Id");
                b.HasIndex("SeriesId");
                b.ToTable("UserFavorites");
            });

            modelBuilder.Entity("Renamer.Data.Entities.Episode", b =>
            {
                b.HasOne("Renamer.Data.Entities.TVShow")
                    .WithMany("Episodes")
                    .HasForeignKey("SeriesId");
            });
        }
    }
}
