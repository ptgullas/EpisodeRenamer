using Renamer.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Serilog;

namespace Renamer.Services {
    public class TitleComparer {
        public TitleComparer() {

        }


        /// <summary>
        /// Returns Season & Episode in ep as string in "x.yy" format
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        public string GetSeasonEpisodeInNumberFormat(EpisodeForComparingDto ep) {
            return $"{ep.SeasonNumber}.{ep.EpisodeNumberInSeason.ToString("D2")}";
        }


        public bool FilenameMatchesEpisode(EpisodeForComparingDto ep, string filename) {
            return (FilenameContainsSeriesName(ep, filename) && FilenameContainsSeasonEpisode(ep, filename));
        }

        public bool FilenameContainsSeasonEpisode(EpisodeForComparingDto ep, string fileName) {
            string seasonEpisode = GetSeasonEpisodeInSEFormat(ep);
            return fileName.Contains(seasonEpisode);
        }

        public string GetSeasonEpisodeInSEFormat(EpisodeForComparingDto ep) {
            return $"s{ep.SeasonNumber.ToString("D2")}e{ep.EpisodeNumberInSeason.ToString("D2")}";
        }

        public bool FilenameContainsSeriesName(EpisodeForComparingDto ep, string filename) {
            string seriesNameToCompare = GetSeriesNameInLowercaseAndPeriodsWithoutStartingArticle(ep);
            return filename.ToLower().Contains(seriesNameToCompare);
        }

        public string GetSeriesNameInLowercaseAndPeriodsWithoutStartingArticle(EpisodeForComparingDto ep) {
            return ep.SeriesName.RemoveFirstWordArticlesFromTitle().ReplaceSpaces().ToLower();
        }

        public EpisodeForComparingDto CreateEpisodeObjectFromPath(string filePath) {
            EpisodeForComparingDto ep = CreateEpisodeObjectFromFilename(Path.GetFileName(filePath));
            if (ep != null) {
                ep.FilePath = filePath;
            }
            return ep;
        }

        public EpisodeForComparingDto CreateEpisodeObjectFromFilename(string filename) {
            EpisodeForComparingDto ep = ExtractSeasonEpisodeFromFilenameAsEpisodeObject(filename);
            if (ep != null) {
                ep.SeriesName = ExtractSeriesNameFromFilename(filename).ReplacePeriodsWithSpaces();
            }
            return ep;
        }
        public EpisodeForComparingDto ExtractSeasonEpisodeFromFilenameAsEpisodeObject(string filename) {
            EpisodeForComparingDto ep = new EpisodeForComparingDto();
            GroupCollection groups = GetSeasonEpisodeMatchFromFilename(filename).Groups;
            if (RegexGroupsContainSeasonEpisodeFormat(groups)) {
                ep.SeasonNumber = groups[1].ToString().ToInt();
                ep.EpisodeNumberInSeason = groups[2].ToString().ToInt();
                return ep;
            }
            else {
                Log.Information("Filename {a} does not contain episode number in s##e## format.", filename);
                return null;
            }
        }

        public string ExtractSeasonEpisodeFromFilename(string filename) {
            string seasonepisode = GetSeasonEpisodeMatchFromFilename(filename).Value.Replace(".", "");
            return seasonepisode;
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


        private Match GetSeasonEpisodeMatchFromFilename(string filename) {
            string seasonEpisodePatternLowercase = @"\.s(\d+)e(\d+)\.";
            Match lowerCaseMatch = GetRegExMatchFromPattern(filename, seasonEpisodePatternLowercase);
            if (lowerCaseMatch.Success) {
                return lowerCaseMatch;
            }
            else {
                string seasonEpisodePatternUppercase = @"\.S(\d+)E(\d+)\.";
                Match upperCaseMatch = GetRegExMatchFromPattern(filename, seasonEpisodePatternUppercase);
                return upperCaseMatch;
            }
        }

        private static Match GetRegExMatchFromPattern(string stringToCheck, string regExPattern) {
            Regex rgx = new Regex(regExPattern);
            Match lowerCaseMatch = rgx.Match(stringToCheck);
            return lowerCaseMatch;
        }

        public bool FilenameContainsSeasonEpisodeFormat(string filename) {
            GroupCollection groups = GetSeasonEpisodeMatchFromFilename(filename).Groups;
            if (RegexGroupsContainSeasonEpisodeFormat(groups)) {
                return true;
            }
            else {
                return false;
            }
        }
        private bool RegexGroupsContainSeasonEpisodeFormat(GroupCollection groups) {
            if (groups.Count == 3 && groups[1].ToString().IsNumeric() && groups[2].ToString().IsNumeric()) {
                return true;
            }
            else {
                return false;
            }

        }


    }
}
