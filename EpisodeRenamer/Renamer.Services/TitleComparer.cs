using Renamer.Services.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Renamer.Services {
    public class TitleComparer {
        public TitleComparer() {

        }

        public bool FilenameContainsSeasonEpisode(EpisodeForComparing ep, string fileName) {
            string seasonEpisode = GetSeasonEpisodeInSEFormat(ep);
            return fileName.Contains(seasonEpisode);
        }

        public string GetSeasonEpisodeInSEFormat(EpisodeForComparing ep) {
            return $"s{ep.SeasonNumber.ToString("D2")}e{ep.EpisodeNumberInSeason.ToString("D2")}";
        }
        public string GetSeasonEpisodeInNumberFormat(EpisodeForComparing ep) {
            return $"{ep.SeasonNumber}.{ep.EpisodeNumberInSeason.ToString("D2")}";
        }
        public string GetFormattedFilename(EpisodeForComparing ep) {
            return ($"{ep.SeriesName} - {GetSeasonEpisodeInNumberFormat(ep)} - {ep.EpisodeTitle}");
        }


        public string GetSeriesNameInLowercaseAndPeriodsWithoutStartingArticle(EpisodeForComparing ep) {
            return ep.SeriesName.RemoveFirstWordArticlesFromTitle().ReplaceSpaces().ToLower();
        }

        public bool FilenameContainsSeriesName(EpisodeForComparing ep, string filename) {
            string seriesNameToCompare = GetSeriesNameInLowercaseAndPeriodsWithoutStartingArticle(ep);
            return filename.ToLower().Contains(seriesNameToCompare);
        }

        public bool FilenameMatchesEpisode(EpisodeForComparing ep, string filename) {
            return (FilenameContainsSeriesName(ep, filename) && FilenameContainsSeasonEpisode(ep, filename));
        }

        public string ExtractSeasonEpisodeFromFilename(string filename) {
            string seasonepisode = GetSeasonEpisodeMatchFromFilename(filename).Value.Replace(".", "");
            return seasonepisode;
        }

        private Match GetSeasonEpisodeMatchFromFilename(string filename) {
            string seasonEpisodePattern = @"\.s(\d+)e(\d+)\.";
            Regex rgx = new Regex(seasonEpisodePattern);
            return rgx.Match(filename);
        }

        public string ExtractSeriesNameFromFilename(string filename) {
            int seasonEpisodeIndex = GetSeasonEpisodeMatchFromFilename(filename).Index;
            if (seasonEpisodeIndex > 0) {
                return filename.Substring(0, seasonEpisodeIndex);
            }
            else {
                return "";
            }
        }
        public int ExtractSeasonNumberFromFilename(string filename) {
           GroupCollection groups = GetSeasonEpisodeMatchFromFilename(filename).Groups;
           return groups[1].ToString().ToInt();
        }

        public EpisodeForComparing ExtractSeasonEpisodeFromFilenameAsEpisodeObject(string filename) {
            EpisodeForComparing ep = new EpisodeForComparing();
            GroupCollection groups = GetSeasonEpisodeMatchFromFilename(filename).Groups;
            if (groups.Count == 3) {
                ep.SeasonNumber = groups[1].ToString().ToInt();
                ep.EpisodeNumberInSeason = groups[2].ToString().ToInt();
                return ep;
            }
            else {
                throw new ArgumentException("Filename does not contain episode number in s##e## format.", filename);
            }
        }

        public EpisodeForComparing CreateEpisodeObjectFromFilename(string filename) {
            EpisodeForComparing ep = ExtractSeasonEpisodeFromFilenameAsEpisodeObject(filename);
            ep.SeriesName = ExtractSeriesNameFromFilename(filename).Replace('.',' ');
            return ep;
        }

    }
}
