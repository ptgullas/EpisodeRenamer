using Renamer.Data.Entities;
using Renamer.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Renamer.Services {
    public class RenamerFacade {
        private TVDBRetrieverService _retrieverService;
        private TVShowService _showService;
        private EpisodeContext _context;
        private string _tvdbInfoFilePath;

        public TVDBInfo _tvdbInfo;
        public RenamerFacade(TVDBRetrieverService retrieverService,
            TVShowService showService,
            EpisodeContext context,
            string tvdbPath) {
            _retrieverService = retrieverService;
            _showService = showService;
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
    }
}
