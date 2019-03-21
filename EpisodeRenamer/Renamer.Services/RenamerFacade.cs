using Renamer.Data.Entities;
using Renamer.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.Linq;
using System.IO;

namespace Renamer.Services {
    public class RenamerFacade {
        private TVDBRetrieverService _retrieverService;
        private TVShowService _showService;
        private EpisodeService _episodeService;
        private LocalMediaService _localservice;
        private EpisodeContext _context;
        private string _tvdbInfoFilePath;

        private static System.Timers.Timer myTimer;

        public TVDBInfo _tvdbInfo;
        public RenamerFacade(TVDBRetrieverService retrieverService,
            TVShowService showService,
            EpisodeService episodeService,
            LocalMediaService localService,
            EpisodeContext context,
            string tvdbPath) {
            _retrieverService = retrieverService;
            _showService = showService;
            _episodeService = episodeService;
            _localservice = localService;
            _context = context;
            _tvdbInfoFilePath = tvdbPath;

            _tvdbInfo = TVDBInfo.ReadFromFile(_tvdbInfoFilePath);
        }

        public async Task FetchTokenIfNeeded() {
            if (_tvdbInfo.TokenIsExpired) {
                Log.Information("Token is expired. Fetching new one....");
                string newToken = await _retrieverService.FetchNewToken(_tvdbInfo.ToAuthenticator());
                SetNewTokenAndSaveToFile(newToken);
            }
            else if (_tvdbInfo.TokenIsAlmostExpired) {
                // refresh the token
                Log.Information("Token will expire soon. Will refresh it");
                string newToken = await _retrieverService.FetchRefreshToken(_tvdbInfo.Token);
                SetNewTokenAndSaveToFile(newToken);
            }
            else {
                Log.Information("Token is still good");
            }
        }

        private void SetNewTokenAndSaveToFile(string newToken) {
            _tvdbInfo.Token = newToken;
            _tvdbInfo.TokenRetrievedDate = DateTime.Now;
            _tvdbInfo.SaveToFile(_tvdbInfoFilePath);
            Log.Information($"New token expires on {_tvdbInfo.GetExpiration().ToString("MM/dd/yyyy hh:mm tt")}");
            Log.Information($"Saved authentication info to {_tvdbInfoFilePath}");
        }

        public async Task PopulateShowsFromFavorites() {
            try {
                var faveSeriesIds = await _retrieverService.FetchUserFavorites(_tvdbInfo.Token);
                Log.Information($"Retrieved {faveSeriesIds.Count} user favorites");
                var seriesIdsNotInDB = _showService.GetSeriesIdsNotInDatabase(faveSeriesIds);
                Log.Information($"{seriesIdsNotInDB.Count} item(s) not in database");
                if (seriesIdsNotInDB.Count != 0) {
                    foreach (int seriesId in seriesIdsNotInDB) {
                        await FetchTVShowAndAddItToDatabase(seriesId, _tvdbInfo.Token);
                        await PopulateEpisodesFromSeriesId(seriesId);
                        // set 2-second timer here (may need to also use Stopwatch class)
                        await Task.Delay(2000);
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e.Message);
            }
        }


        public async Task FetchTVShowAndAddItToDatabase(int seriesId, string token) {
            TVShowFromTVDBDto newShowDto = await _retrieverService.FetchTVShow(seriesId, token);
            TVShow newShow = newShowDto.ToTVShow();
            Log.Information($"Adding seriesId {seriesId}: {newShow.SeriesName} to Shows table");
            _showService.Add(newShow);
        } 

        public async Task PopulateEpisodesFromExistingShows(int numberOfPagesFromEndToFetch = 1) {
            var seriesIdsInDB = _context.Shows.Select(s => s.SeriesId);
            foreach (int seriesId in seriesIdsInDB) {
                await PopulateEpisodesFromSeriesId(seriesId, numberOfPagesFromEndToFetch);
                await Task.Delay(2000);
            }
        }

        public async Task PopulateEpisodesFromSeriesId(int seriesId, int numberOfPagesFromEndToFetch = 1) {
            Log.Information($"Populating Episodes from seriesId {seriesId}");
            try {
                var episodeOuter = await _retrieverService.FetchEpisodes(seriesId, _tvdbInfo.Token);
                AddEpisodesOnPageToDatabase(episodeOuter); // always add the episodes on page 1
                if (numberOfPagesFromEndToFetch > 0) {
                    int lastPage = episodeOuter.links.Last;
                    int pageToEndOn = lastPage - numberOfPagesFromEndToFetch;
                    if (pageToEndOn <= 0) {
                        pageToEndOn = 1; // we already added the episodes on page 1, so stop when we get down to 1
                    }
                    for (int i = lastPage; i > pageToEndOn; i--) {
                        Log.Information("Fetching episodes JSON from page {a}", i);
                        episodeOuter = await _retrieverService.FetchEpisodes(seriesId, _tvdbInfo.Token, i);
                        AddEpisodesOnPageToDatabase(episodeOuter);
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e.Message);
            }
        }
        private void AddEpisodesOnPageToDatabase(EpisodeOuterDto episodeOuter) {
            var episodeList = episodeOuter.episodes.ToList();
            foreach (EpisodeFromTVDBDto epDto in episodeList) {
                try {
                    Log.Information("Checking episode {a}: \"{b}\"...", epDto.EpisodeId, epDto.EpisodeName);
                    Episode ep = epDto.ToEpisode();
                    _episodeService.CheckIfUpdateNeeded(ep);
                    _episodeService.Add(ep);
                }
                catch (Exception e) {
                    Log.Error(e.Message);
                }
            }
        }

        public string FindTVShowNameBySeriesId(int seriesId) {
            TVShow show = _showService.FindBySeriesId(seriesId);
            string title = null;
            if (show != null) {
                title = show.SeriesName;
            }
            return title;
        }

        public void AddPreferredNameToTVShow(int seriesId, string preferredName) {
            Log.Information($"Adding preferred name {preferredName} to seriesId {seriesId}");
            try {
                _showService.AddPreferredName(seriesId, preferredName);
            }
            catch (Exception e) {
                Log.Error(e, "Trouble adding preferred name {a} to {b}", preferredName, seriesId);
            }
        }

        public EpisodeForComparingDto CreateEpisodeForComparingDtoFromEntity(Episode ep) {
            TVShow show = _context.Shows
                .FirstOrDefault(b => b.SeriesId == ep.SeriesId);
            string seriesName = GetSeriesTitleForRenaming(show);
            EpisodeForComparingDto targetEpisodeDto = ep.ToEpisodeForComparingDto(seriesName);
            return targetEpisodeDto;
        }

        private string GetSeriesTitleForRenaming(TVShow show) {
            if (show.SeriesNamePreferred == null) {
                return show.SeriesName;
            }
            else {
                return show.SeriesNamePreferred;
            }
        }

        public void RenameFiles() {
            List<EpisodeForComparingDto> sourceEps = _localservice.GetFilesAsDtosToCompare();
            foreach (EpisodeForComparingDto epDto in sourceEps) {
                try {
                    Episode epEntity = _episodeService.FindByEpisodeForComparingDto(epDto);
                    if (epEntity != null) {
                        EpisodeForComparingDto targetDto = CreateEpisodeForComparingDtoFromEntity(epEntity);
                        string localFile = Path.GetFileName(epDto.FilePath);

                        Console.WriteLine($"Rename {localFile} to {targetDto.GetFormattedFilename()}?");
                        string userInput = Console.ReadLine().ToUpper();
                        if (userInput == "Y") {
                            _localservice.RenameFile(epDto.FilePath, targetDto);
                        }
                    }
                }
                catch (Exception e) {
                    Log.Warning(e, "Could not rename file {a}", epDto.FilePath);
                }
            }
        }

    }

}
