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

        public async void FetchTokenIfNeeded() {
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
                        TVShowFromTVDBDto newShowDto = await _retrieverService.FetchTVShow(seriesId, _tvdbInfo.Token);
                        TVShow newShow = newShowDto.ToTVShow();
                        Log.Information($"Adding seriesId {seriesId}: {newShow.SeriesName} to Shows table");
                        _showService.Add(newShow);
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e.Message);
            }
        }

        public async Task PopulateEpisodesFromExistingShows(int numberOfPagesFromEndToFetch = 2) {
            var seriesIdsInDB = _context.Shows.Select(s => s.SeriesId);
            foreach (int seriesId in seriesIdsInDB) {
                await PopulateEpisodes(seriesId, numberOfPagesFromEndToFetch);
            }
        }

        public async Task PopulateEpisodes(int seriesId, int numberOfPagesFromEndToFetch = 0) {
            Log.Information($"Populating Episodes from seriesId {seriesId}");
            try {
                var episodeOuter = await _retrieverService.FetchEpisodes(seriesId, _tvdbInfo.Token);
                // always add the episodes on page 1
                AddEpisodesInEpisodeObject(episodeOuter);
                if (numberOfPagesFromEndToFetch > 0) {
                    int lastPage = episodeOuter.links.Last;
                    int pageToEndOn = lastPage - numberOfPagesFromEndToFetch;
                    if (pageToEndOn <= 0) {
                        pageToEndOn = 1; // we already added the episodes on page 1, so stop when we get down to 1
                    }
                    for (int i = lastPage; i > pageToEndOn; i--) {
                        episodeOuter = await _retrieverService.FetchEpisodes(seriesId, _tvdbInfo.Token, i);
                        AddEpisodesInEpisodeObject(episodeOuter);
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e.Message);
            }
        }
        private void AddEpisodesInEpisodeObject(EpisodeOuterDto episodeOuter) {
            var episodeList = episodeOuter.episodes.ToList();
            foreach (EpisodeFromTVDBDto epDto in episodeList) {
                try {
                    Log.Information("Adding episode {a}: \"{b}\"", epDto.EpisodeId, epDto.EpisodeName);
                    Episode ep = epDto.ToEpisode();
                    _episodeService.Add(ep);
                }
                catch (Exception e) {
                    Log.Error(e.Message);
                }
            }
        }
        public EpisodeForComparingDto CreateEpisodeForComparingDtoFromEntity(Episode ep) {
            TVShow show = _context.Shows
                .FirstOrDefault(b => b.SeriesId == ep.SeriesId);
            string seriesName;

            if (show.SeriesNamePreferred == null) {
                seriesName = show.SeriesName;
            }
            else {
                seriesName = show.SeriesNamePreferred;
            }
            EpisodeForComparingDto epDto = ep.ToEpisodeForComparingDto();
            epDto.SeriesName = seriesName;
            return epDto;
        }

        public void RenameFiles() {
            List<EpisodeForComparingDto> sourceEps = _localservice.GetFilesAsDtosToCompare();
            foreach (EpisodeForComparingDto epDto in sourceEps) {
                Episode epEntity = _episodeService.FindByEpisodeForComparingDto(epDto);
                EpisodeForComparingDto targetDto = CreateEpisodeForComparingDtoFromEntity(epEntity);
                string localFile = Path.GetFileName(epDto.FilePath);

                Console.WriteLine($"Rename {localFile} to {targetDto.GetFormattedFilename()}?");
                string userInput = Console.ReadLine().ToUpper();
                if (userInput == "Y") {
                    _localservice.RenameFile(epDto.FilePath, targetDto);
                }
            }
        }

    }

}
