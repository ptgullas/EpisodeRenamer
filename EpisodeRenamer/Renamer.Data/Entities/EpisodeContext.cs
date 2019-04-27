using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;


namespace Renamer.Data.Entities {
    public class EpisodeContext : DbContext {

        public EpisodeContext(DbContextOptions<EpisodeContext> options) 
            : base(options) {
        }

        public EpisodeContext() {

        }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<TVShow> Shows { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }

        // may have to comment this out after creating model
        // but uncomment when doing DB changes
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        //    optionsBuilder.UseSqlite("Data Source=C:\\Users\\Prime Time Pauly G\\Documents\\EpisodeRenamer DB\\tvepisodes.db");
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity("Renamer.Data.Entities.Episode", b => {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("TVDBEpisodeId");
                b.Property<int?>("Season");
                b.Property<int?>("AiredSeasonId");
                b.Property<int?>("AiredEpisodeNumber");
                b.Property<string>("EpisodeName")
                    .IsRequired();
                b.Property<int?>("AbsoluteNumber");
                b.Property<DateTime>("LastUpdated");
                b.Property<int>("SeriesId")
                    .IsRequired();
                b.HasAlternateKey("TVDBEpisodeId");
                b.HasKey("Id");
                b.HasIndex("SeriesId");
                b.ToTable("Episodes");
            });

            modelBuilder.Entity("Renamer.Data.Entities.TVShow", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int>("SeriesId")
                    .IsRequired();
                b.Property<string>("SeriesName");
                b.Property<string>("SeriesNamePreferred");
                b.HasAlternateKey("SeriesId");
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
                    .HasForeignKey("SeriesId")
                    .HasPrincipalKey("SeriesId");
            });
        }
    }
}
